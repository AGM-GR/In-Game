using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableForce : BaseGrabbable {

	[SerializeField]
	protected float force = 100;

	[SerializeField]
	protected float minDistance;

	[SerializeField]
	protected float maxDistance;

	private Rigidbody rb;
	private Transform grabberTransform;
	private Transform grabTransform;

	void Awake() {

		rb = GetComponent<Rigidbody> ();

		if (grabSpot)
			grabTransform = grabSpot;
		else
			grabTransform = transform;
	}

	protected override void AttachToGrabber(BaseGrabber grabber) {
		base.AttachToGrabber(grabber);
		grabberTransform = GrabberPrimary.GetComponent<Transform> ();
	}

	protected override void DetachFromGrabber(BaseGrabber grabber) {
		base.DetachFromGrabber(grabber);
		if (GrabState == GrabStateEnum.Inactive)
			rb.AddForce (Vector3.zero);
	}

	void FixedUpdate() {
		if (GrabState != GrabStateEnum.Inactive) {
			
			Vector3 forceDirection = grabberTransform.position - grabTransform.position;
			float distance = forceDirection.sqrMagnitude;
			forceDirection = forceDirection.normalized;

			rb.AddForce (forceDirection * force);
		}
	}

}
