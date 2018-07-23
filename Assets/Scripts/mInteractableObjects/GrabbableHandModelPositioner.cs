using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#endif

public class GrabbableHandModelPositioner : MonoBehaviour {

	[SerializeField]
	protected Transform grabPoint;

	public HandActionsEnum handAction = HandActionsEnum.GRAB;

	[Header("Left Hand")]
	public Vector3 leftPosition;
	public Vector3 leftRotation;
	public Vector3 leftScale;

	[Header("Right Hand")]
	public Vector3 rightPosition;
	public Vector3 rightRotation;
	public Vector3 rightScale;

	private BaseGrabbable grabbable;
	private BaseGrabber grabber;
	private HandModelConnector handModel;
	private HandAnimationsController handAnimator;


	void Awake () {
		grabbable = GetComponent<BaseGrabbable> ();

		if (grabbable == null)
			this.enabled = false;
		else {
			grabbable.OnContactStateChange += UpdateGrabState;
			grabbable.OnGrabStateChange += UpdateGrabState;
		}
	}

	private void UpdateGrabState(BaseGrabbable baseGrab) {
		if (baseGrab.GrabState != GrabStateEnum.Inactive)
			AttachHandModel (baseGrab.GrabberPrimary);
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

	private void DetachHandModel () {
		if (handModel != null && handAnimator != null) {
			handModel.ReconectHandModel ();
			handAnimator.SetUndoAnimation (handAction);
		}

		handModel = null;
		handAnimator = null;
		grabber = null;
	}

}
