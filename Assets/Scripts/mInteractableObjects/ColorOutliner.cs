using System;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

public class ColorOutliner : MonoBehaviour {

	[Header("Colors")]
	[SerializeField]
	private Color colorOnContactSingle = Color.blue;

	[SerializeField]
	private Color colorOnContactMulti = Color.cyan;

	[SerializeField]
	private Color colorOnGrabSingle = Color.yellow;

	[SerializeField]
	private Color colorOnGrabMulti = Color.red;

	[Header("Outline")]
	[SerializeField]
	private Outline.Mode outlineMode = Outline.Mode.OutlineVisible;

	[SerializeField, Range(0f, 10f)]
	private float outlineWidth = 2f;

	[Header("Objects")]
	[SerializeField]
	private Outline targetOutlineObject;

	[SerializeField]
	private BaseGrabbable grabbable;

	[SerializeField]
	private BaseInteractable interactable;

	private bool enabledInteraction = true;

	void Awake() {
		if (targetOutlineObject == null)
			targetOutlineObject = GetComponentInChildren<Outline> ();

		if (grabbable == null)
			grabbable = GetComponent<BaseGrabbable> ();
		if (grabbable != null) {
			//Subscribe los eventos al tocar un objeto y agarrarlo
			grabbable.OnContactStateChange += RefreshColor;
			grabbable.OnGrabStateChange += RefreshColor;
		} else {
		
			if (interactable == null)
				interactable = GetComponent<BaseInteractable> ();
			if (interactable != null) {
				interactable.OnContactStateChange += RefreshColor;
				interactable.OnInteractStateChange += RefreshColor;
			}

		}
	}

	void Start() {
		
		DeactivateOutline ();
	}

	private void RefreshColor(BaseGrabbable baseGrab) {
		if (enabledInteraction) {
			switch (baseGrab.ContactState) {
			case GrabStateEnum.Inactive:
				DeactivateOutline ();
				break;

			case GrabStateEnum.Multi:
				ActivateOutline (colorOnContactMulti);
				break;

			case GrabStateEnum.Single:
				ActivateOutline (colorOnContactSingle);
				break;

			default:
				throw new ArgumentOutOfRangeException ();
			}

			switch (baseGrab.GrabState) {
			case GrabStateEnum.Inactive:
				break;

			case GrabStateEnum.Multi:
				ActivateOutline (colorOnGrabMulti);
				break;

			case GrabStateEnum.Single:
				ActivateOutline (colorOnGrabSingle);
				break;

			default:
				throw new ArgumentOutOfRangeException ();
			}
		}
	}

	private void RefreshColor(BaseInteractable baseInteract) {
		Debug.Log ("Interact");
		if (enabledInteraction) {
			switch (baseInteract.ContactState) {
			case GrabStateEnum.Inactive:
				DeactivateOutline ();
				break;

			case GrabStateEnum.Multi:
				ActivateOutline (colorOnContactMulti);
				break;

			case GrabStateEnum.Single:
				ActivateOutline (colorOnContactSingle);
				break;

			default:
				throw new ArgumentOutOfRangeException ();
			}

			switch (baseInteract.InteractState) {
			case GrabStateEnum.Inactive:
				break;

			case GrabStateEnum.Multi:
				ActivateOutline (colorOnGrabMulti);
				break;

			case GrabStateEnum.Single:
				ActivateOutline (colorOnGrabSingle);
				break;

			default:
				throw new ArgumentOutOfRangeException ();
			}
		}
	}

	public void DeactivateOutline() {
		
		if (targetOutlineObject != null)
			targetOutlineObject.enabled = false;
	}

	public void ActivateOutline(Color color) {
		
		if (targetOutlineObject != null) {
			targetOutlineObject.enabled = true;
			targetOutlineObject.OutlineColor = color;
			targetOutlineObject.OutlineMode = outlineMode;
			targetOutlineObject.OutlineWidth = outlineWidth;
		}
	}

	public void SetInteraction (bool enabled) {
		enabledInteraction = enabled;
	}
}
