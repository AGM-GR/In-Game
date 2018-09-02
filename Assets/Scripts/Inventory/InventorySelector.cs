using HoloToolkit.Unity.UX;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;
using UnityEngine;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#endif

public class InventorySelector : AttachToController {

	public enum SwipeEnum {
		None,
		Left,
		Right
	}

	[Header("InventorySelector Elements")]
	[SerializeField]
	private LineObjectCollection itemsCollection = null;
	[SerializeField]
	private SwipeEnum currentAction;
	[SerializeField]
	private AnimationCurve swipeCurve = null;
	[SerializeField]
	private float swipeDuration = 0.5f;
	[SerializeField]
	private float menuTimeout = 2f;
	[SerializeField]
	private int displayItemindex = 0;
	[SerializeField]
	private int activeItemindex = 3;

	private float menuOpenTime = 0f;
	private bool menuOpen = false;
	private bool switchingItem = false;

	private float startOffset;
	private float targetOffset;

	private float startTime;
	private bool resetInput;

	private InventoryItem activeItem;

	private BaseGrabber activeHandGrabber = null;

	protected override void OnEnable() {
		base.OnEnable();
		displayItemindex = -1;
		currentAction = SwipeEnum.Left;
	}

	private void Update() {
		if (menuOpen) {
			if (Time.unscaledTime - menuOpenTime > menuTimeout) {
				menuOpen = false;
				ExitMenuMode ();
			}
		}

		#if UNITY_WSA && UNITY_2017_2_OR_NEWER
		for (int i = 0; i < itemsCollection.Objects.Count; i++) {
			InventoryItem item = itemsCollection.Objects[i].GetComponent<InventoryItem>();
			if (item == activeItem && !menuOpen && !switchingItem) {
				if (Handedness == InteractionSourceHandedness.Right)
					item.DisplayMode = InventoryItem.DisplayModeEnum.InRightHand;
				else if(Handedness == InteractionSourceHandedness.Left)
					item.DisplayMode = InventoryItem.DisplayModeEnum.InLeftHand;
			} else if (item == activeItem) {
				item.DisplayMode = InventoryItem.DisplayModeEnum.InMenuSelected;
			} else {
				item.DisplayMode = menuOpen ? InventoryItem.DisplayModeEnum.InMenu : InventoryItem.DisplayModeEnum.Hidden;
			}
		}

		if (!switchingItem) {
			if (currentAction == SwipeEnum.None) {
				return;
			}

			if (!menuOpen) {
				menuOpenTime = Time.unscaledTime;
				menuOpen = true;
				EnterMenuMode ();
			}

			// Stop the active item if we have one
			if (activeItem != null) {
				activeItem = null;
			}

			// Get the current offset and the target offset from our collection
			if (itemsCollection.Objects.Count % 2 != 0) {
				startOffset = itemsCollection.GetOffsetFromObjectIndex(displayItemindex) + (itemsCollection.DistributionOffsetPerObject / 2.0f);
				if (startOffset > 1) startOffset = startOffset - 1;
			} else {
				startOffset = itemsCollection.GetOffsetFromObjectIndex(displayItemindex);
			}

			targetOffset = startOffset;

			switch (currentAction) {
			case SwipeEnum.Right:
				displayItemindex = itemsCollection.GetPrevObjectIndex(displayItemindex);
				activeItemindex = itemsCollection.GetNextObjectIndex(activeItemindex);
				targetOffset -= itemsCollection.DistributionOffsetPerObject;
				break;

			case SwipeEnum.Left:
				default:
				displayItemindex = itemsCollection.GetNextObjectIndex(displayItemindex);
				activeItemindex = itemsCollection.GetPrevObjectIndex(activeItemindex);
				targetOffset += itemsCollection.DistributionOffsetPerObject;
				break;
			}

			// Get the current item from the object list
			Transform itemTransform = itemsCollection.Objects[activeItemindex];
			activeItem = itemTransform.GetComponent<InventoryItem>();

			// Lerp from current to target offset
			startTime = Time.unscaledTime;
			resetInput = false;
			switchingItem = true;
		}
		else {
			if (Time.unscaledTime < startTime + swipeDuration) {
				float normalizedTime = (Time.unscaledTime - startTime) / swipeDuration;

				if (!resetInput && normalizedTime > 0.5f) {
					// If we're past the halfway point, set our swipe action to none
					// If the user swipes again before we're done switching, we'll move to the next item
					resetInput = true;
					currentAction = SwipeEnum.None;
				}

				itemsCollection.DistributionOffset = Mathf.Lerp(startOffset, targetOffset, swipeCurve.Evaluate(normalizedTime));
				menuOpenTime = Time.unscaledTime;
			}
			else {
				switchingItem = false;
				itemsCollection.DistributionOffset = targetOffset;
			}
		}
		#endif
	}

	#if UNITY_WSA && UNITY_2017_2_OR_NEWER
	private void InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj) {
		if (obj.state.touchpadPressed) {
			if (activeHandGrabber == null)
				activeHandGrabber = ControllerInfo.ControllerParent.GetComponentInChildren<BaseGrabber> ();
			
			if (Handedness != obj.state.source.handedness) {
				// Quita el modo menú y las animaciones si estubiese activo de la mano actual
				ExitMenuMode ();
				ControllerInfo.ControllerParent.GetComponentInChildren<HandAnimationsController> ().SetUndoAnimation (activeItem.HandAction);
				ControllerInfo.ControllerParent.GetComponentInChildren<BaseGrabber> ().enabled = true;
				//Cambia la mano que tiene el menú si se pulsa el touchpad con otra mano
				ChangeHandedness (obj.state.source.handedness);
                //Entra en el modo menú con la otra mano
                activeHandGrabber = ControllerInfo.ControllerParent.GetComponentInChildren<BaseGrabber>();
                EnterMenuMode ();
			}

			if (activeHandGrabber.GrabState == GrabStateEnum.Inactive && activeHandGrabber.ContactState == GrabStateEnum.Inactive) {
				// Check which side we clicked
				if (obj.state.touchpadPosition.x < 0) {
					currentAction = SwipeEnum.Left;
				} else {
					currentAction = SwipeEnum.Right;
				}
			}
		}
	}
	#endif

	protected override void OnAttachToController() {
		#if UNITY_WSA && UNITY_2017_2_OR_NEWER
		// Subscribe to input now that we're parented under the controller
		InteractionManager.InteractionSourceUpdated += InteractionSourceUpdated;
		#endif
	}

	protected override void OnDetachFromController() {
		#if UNITY_WSA && UNITY_2017_2_OR_NEWER
		// Unsubscribe from input
		InteractionManager.InteractionSourceUpdated -= InteractionSourceUpdated;
		#endif
	}

	private void EnterMenuMode () {
		if (ControllerInfo != null && ControllerInfo.ControllerParent != null) {
			GameObject controller = ControllerInfo.ControllerParent;
			if (controller.GetComponentInChildren<ControllerController> () != null) {
				controller.GetComponentInChildren<ControllerController> ().ActiveControllerMenu ();
				controller.GetComponentInChildren<HandModelConnector> ().HideHandModel ();
				controller.GetComponentInChildren<HandAnimationsController> ().SetUndoAnimation (activeItem.HandAction);

				activeHandGrabber.enabled = false;
			}
		}
	}

	private void ExitMenuMode () {
		if (ControllerInfo != null && ControllerInfo.ControllerParent != null) {
			GameObject controller = ControllerInfo.ControllerParent;
			if (controller.GetComponentInChildren<ControllerController> () != null) {
				controller.GetComponentInChildren<ControllerController> ().DeactivateControllerAction ();
				controller.GetComponentInChildren<HandModelConnector> ().ShowHandModel ();
				controller.GetComponentInChildren<HandAnimationsController> ().SetAnimation (activeItem.HandAction);

				if (activeItem == itemsCollection.Objects[0].GetComponent<InventoryItem>())
					activeHandGrabber.enabled = true;
			}
		}
	}

	public void SetActiveItemindex (int activeIndex) {
		activeItemindex = activeIndex - displayItemindex;
		if (activeItemindex < 0)
			activeItemindex = (itemsCollection.Objects.Count + activeItemindex);
	}

	public InventoryItem GetActiveItem () {
		return activeItem;
	}
}
