using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#endif

public class HandModelPositioner : MonoBehaviour {

	[SerializeField]
	protected Transform grabPoint;

	public HandActionsEnum handAction = HandActionsEnum.GRAB;

	[Header("Left Hand")]
	public Vector3 leftPosition = Vector3.zero;
	public Vector3 leftRotation = Vector3.zero;
	public Vector3 leftScale = Vector3.one;

	[Header("Right Hand")]
	public Vector3 rightPosition = Vector3.zero;
	public Vector3 rightRotation = Vector3.zero;
	public Vector3 rightScale = Vector3.one;

	private BaseGrabbable grabbable;
	private BaseInteractable interactable;
	private BaseGrabber grabber;
	private BaseInteractor interactor;
	private HandModelConnector handModel;
	private HandAnimationsController handAnimator;


	void Awake () {
		grabbable = GetComponent<BaseGrabbable> ();

		if (grabPoint == null)
			grabPoint = transform;

		if (grabbable == null)
			grabbable = GetComponent<BaseGrabbable> ();
		if (grabbable != null) {
			//Subscribe los eventos al tocar un objeto y agarrarlo
			grabbable.OnContactStateChange += UpdateGrabState;
			grabbable.OnGrabStateChange += UpdateGrabState;
		} else {

			if (interactable == null)
				interactable = GetComponent<BaseInteractable> ();
			if (interactable != null) {
				interactable.OnContactStateChange += UpdateGrabState;
				interactable.OnInteractStateChange += UpdateGrabState;
			}

		}
	}

	private void UpdateGrabState(BaseGrabbable baseGrab) {
		if (baseGrab.GrabState != GrabStateEnum.Inactive)
			AttachHandModel (baseGrab.GrabberPrimary);
		else
			DetachHandModel ();
	}

	private void UpdateGrabState(BaseInteractable baseInteract) {
		if (baseInteract.InteractState != GrabStateEnum.Inactive)
			AttachHandModel (baseInteract.InteractorPrimary);
		else
			DetachHandModel ();
	}

	private void AttachHandModel (BaseGrabber grabber) {
		if (this.grabber != null && this.grabber != grabber)
			DetachHandModel ();
		
		this.grabber = grabber;
		handModel = grabber.GetComponent<HandModelConnector> ();
		handAnimator = grabber.GetComponent<HandAnimationsController> ();
		handAnimator.SetAnimation (handAction);
		#if UNITY_WSA && UNITY_2017_2_OR_NEWER
		if (grabber.Handedness == InteractionSourceHandedness.Left)
			handModel.SetHandModelParent (grabPoint, leftPosition, leftRotation, leftScale);
		else
			handModel.SetHandModelParent (grabPoint, rightPosition, rightRotation, rightScale);
		#endif
	}

	private void AttachHandModel (BaseInteractor interactor) {
		if (this.interactor != null && this.interactor != interactor)
			DetachHandModel ();

		this.interactor = interactor;
		handModel = interactor.GetComponent<HandModelConnector> ();
		handAnimator = interactor.GetComponent<HandAnimationsController> ();
		handAnimator.SetAnimation (handAction);
		#if UNITY_WSA && UNITY_2017_2_OR_NEWER
		if (interactor.Handedness == InteractionSourceHandedness.Left)
			handModel.SetHandModelParent (grabPoint, leftPosition, leftRotation, leftScale);
		else
			handModel.SetHandModelParent (grabPoint, rightPosition, rightRotation, rightScale);
		#endif
	}

	private void DetachHandModel () {
		if (handModel != null && handAnimator != null) {
			handModel.ReconectHandModel ();
			handAnimator.SetUndoAnimation (handAction);
		}

		handModel = null;
		handAnimator = null;
		grabber = null;
		interactor = null;
	}

}
