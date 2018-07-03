using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

public class GrabbableRotation : BaseGrabbable {

	[SerializeField]
	protected Transform objectToRotate;
	[SerializeField]
	protected float maxDistance = 1f;

	[Header("Rotation Restriction")]
	[SerializeField]
	protected Vector3 forwardDirection = Vector3.forward;
	[SerializeField]
	protected Vector3 rotationAxis = Vector3.up;

	[SerializeField]
	protected bool limits = false;
	[SerializeField]
	protected int minAngle = 0;
	[SerializeField]
	protected int maxAngle = 0;

	private Grabber grabber;
	private Transform grabberTransform;
	private Vector3 initialrotation;
	private Vector3 valuesMultiplier;

	private Transform RotateTransform {
		get { 
			return objectToRotate != null ? objectToRotate : transform; 
		}
	}

	void Awake() {
		grabber = null;
		initialrotation = RotateTransform.rotation.eulerAngles;
		valuesMultiplier = new Vector3 (1 - rotationAxis.normalized.x, 
										1 - rotationAxis.normalized.y, 
										1 - rotationAxis.normalized.z);
	}

	protected override void AttachToGrabber(BaseGrabber grabber) {
		base.AttachToGrabber(grabber);
		this.grabber = (Grabber) grabber;
		grabberTransform = GrabberPrimary.GetComponent<Transform> ();
	}

	protected override void DetachFromGrabber(BaseGrabber grabber) {
		base.DetachFromGrabber(grabber);
		this.grabber = null;
	}

	protected override void Update() {
		base.Update ();
		if (grabber != null) {

			Vector3 dest = grabberTransform.position - RotateTransform.position;
			float distance = (grabberTransform.position - GrabPoint).sqrMagnitude;

			if (distance > maxDistance)
				grabber.FinishGrab ();
			else if (!limits || (GetAngle() >= minAngle && GetAngle() <= maxAngle)){

				//En el editor de Unity actualiza el eje de rotación si este se modifica
				#if UNITY_EDITOR
				if (Application.isPlaying)
					valuesMultiplier = new Vector3 (1 - rotationAxis.normalized.x, 
													1 - rotationAxis.normalized.y, 
													1 - rotationAxis.normalized.z);
				#endif

				//Calcula el punto objetivo teniendo el cuenta el eje ce rotación
				dest = new Vector3 (dest.x * valuesMultiplier.x, 
									dest.y * valuesMultiplier.y, 
									dest.z * valuesMultiplier.z);

				//Rota el object desde la posición indicada como forward hasta el vector de destino
				RotateTransform.rotation = Quaternion.FromToRotation (forwardDirection.normalized, dest.normalized);
				//Añade la rotación del object inicial
				RotateTransform.Rotate (initialrotation);
			}
		}
	}

	public float GetAngle() {
		return Vector2.Angle (forwardDirection, grabberTransform.position - RotateTransform.position);
	}

	void OnDrawGizmosSelected() {
		//Distancia a la que el Grab se desactiva
		Color color = Color.blue;
		color.a = 0.1f;
		Gizmos.color = color;
		Gizmos.DrawSphere(GrabPoint, maxDistance);
		color.a = 0.4f;
		Gizmos.color = color;
		Gizmos.DrawWireSphere(GrabPoint, maxDistance);

		//Forward
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(RotateTransform.position, forwardDirection / 4);
		//Eje de rotación
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(RotateTransform.position - (rotationAxis / 10), RotateTransform.position + (rotationAxis / 10));
	}
}
