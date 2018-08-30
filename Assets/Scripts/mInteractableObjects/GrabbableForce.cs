using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableForce : BaseGrabbable {

	[SerializeField]
	protected float force = 100;

	[SerializeField]
	protected float maxDistance = 1f;

	private Grabber grabber;
	private Rigidbody rb;
	private Transform grabberTransform;

	void Awake() {

		grabber = null;
		rb = GetComponent<Rigidbody> ();
	}

	protected override void AttachToGrabber(BaseGrabber grabber) {
		base.AttachToGrabber(grabber);
		this.grabber = (Grabber) grabber;
		grabberTransform = GrabberPrimary.GetComponent<Transform> ();
	}

	protected override void DetachFromGrabber(BaseGrabber grabber) {
		base.DetachFromGrabber(grabber);
		this.grabber = null;
		if (GrabState == GrabStateEnum.Inactive)
			rb.velocity = Vector2.zero;
	}

	void FixedUpdate() {
		if (grabber != null) {
			
			Vector3 forceDirection = grabberTransform.position - GrabPoint;
			float distance = forceDirection.magnitude;
			forceDirection = forceDirection.normalized;

			if (distance > maxDistance)
				grabber.FinishGrab ();
			else
				rb.velocity = forceDirection * distance * force;
		}
	}

	void OnDrawGizmosSelected() {
		Color color = Color.blue;
		color.a = 0.1f;
		Gizmos.color = color;
		Gizmos.DrawSphere(GrabPoint, maxDistance);

		color.a = 0.4f;
		Gizmos.color = color;
		Gizmos.DrawWireSphere(GrabPoint, maxDistance);
	}

}
