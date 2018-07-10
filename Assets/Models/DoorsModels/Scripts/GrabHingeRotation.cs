using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

public class GrabHingeRotation : MonoBehaviour {

	public bool enableRotation = true;

	[SerializeField]
	private List<BaseGrabbable> grabbables = new List<BaseGrabbable>();

	[Header("Rotation Restriction")]
	[SerializeField]
	protected Transform hingeParent;
	[SerializeField]
	protected Transform hinge;
	[SerializeField]
	protected Vector3 forwardDirection = Vector3.forward;
	[SerializeField]
	protected Vector3 rotationAxis = Vector3.up;

	[Header("Rotation Limits")]
	[SerializeField]
	protected bool useLimits = false;
	[SerializeField]
	protected int minAngle = 0;
	[SerializeField]
	protected int maxAngle = 0;

	private GrabStateEnum grabState;
	private GrabStateEnum lastGrabState;
	private Quaternion rotationOffset;
	private float angleOffset;
	private Transform grabberTransform;
	private Quaternion initialrotation;
	private Quaternion worldRotation;
	private Vector3 dest;
	private Vector3 lastDest;

	private Transform RotateTransform {
		get {
			return hinge != null ? hinge : transform; 
		}
	}

	private Vector3 GlobalForwardDirection {
		get { 
			if (hingeParent != null)
				return (hingeParent.rotation * forwardDirection);
			return forwardDirection;
		}
	}

	private Vector3 GlobalRotationAxis {
		get { 
			if (hingeParent != null)
				return hingeParent.rotation * rotationAxis;
			return rotationAxis;
		}
	}

	void Awake() {
		grabState = GrabStateEnum.Inactive;
		initialrotation = RotateTransform.localRotation;
		dest = GlobalForwardDirection;
		lastDest = GlobalForwardDirection;
		lastGrabState = GrabStateEnum.Inactive;
		rotationOffset = Quaternion.identity;
		angleOffset = 0f;

		if (grabbables.Count == 0)
			grabbables.Add(GetComponent<BaseGrabbable> ());

		foreach (BaseGrabbable grabbable in grabbables) {
			//Subscribe los eventos al tocar un objeto y agarrarlo
			grabbable.OnContactStateChange += UpdateGrabState;
			grabbable.OnGrabStateChange += UpdateGrabState;
		}
	}

	private void UpdateGrabState(BaseGrabbable baseGrab) {
		if (baseGrab.GrabState != GrabStateEnum.Inactive)
			grabberTransform = baseGrab.GrabberPrimary.GetComponent<Transform> ();
		grabState = baseGrab.GrabState;
	}

	void Update() {
		
		if (grabState != GrabStateEnum.Inactive) {
			//En cuanto agarra el objeto calcula el offset de rotación con la posición inicial del grabber
			if (lastGrabState == GrabStateEnum.Inactive) {
				dest = grabberTransform.position - RotateTransform.position;
				dest = Vector3.ProjectOnPlane (dest, GlobalRotationAxis.normalized);
				rotationOffset = Quaternion.FromToRotation (lastDest.normalized, dest.normalized);
				angleOffset = Vector3.SignedAngle (lastDest, dest, GlobalRotationAxis);
				dest = lastDest;
			}

			if (enableRotation) {
				dest = grabberTransform.position - RotateTransform.position;
				//Se proyecta el punto de agarre en el plano perpendicular al eje de giro para obtener el vector de destino
				dest = Vector3.ProjectOnPlane (dest, GlobalRotationAxis.normalized);

				if (!useLimits || (GetAngleWithOffset () >= minAngle && GetAngleWithOffset () <= maxAngle)) {
					//Devuelve el objeto a su rotación inicial local y obtiene la rotación global de este
					RotateTransform.localRotation = initialrotation;
					worldRotation = RotateTransform.rotation;
					//Rota el object desde la posición indicada como forward hasta el vector de destino
					RotateTransform.rotation = Quaternion.FromToRotation (GlobalForwardDirection.normalized, dest.normalized) * Quaternion.Inverse (rotationOffset);
					//Añade la rotación del object global inicial
					RotateTransform.Rotate (worldRotation.eulerAngles);

					//Corrige el vector dest para que punte en la misma dirección que el objeto
					dest = Quaternion.Inverse (rotationOffset) * dest;
					lastDest = dest;

				} else {
					dest = lastDest;
				}
			}
		}

		lastGrabState = grabState;
	}

	public float GetAngle() {
		return Vector3.SignedAngle (GlobalForwardDirection, dest, GlobalRotationAxis);
	}

	public float GetAngleWithOffset() {
		return Vector3.SignedAngle (GlobalForwardDirection, dest, GlobalRotationAxis) - angleOffset;
	}

	void OnDrawGizmosSelected() {

		//Forward
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(RotateTransform.position, GlobalForwardDirection /4);
		//Eje de rotación
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(RotateTransform.position, RotateTransform.position + (GlobalRotationAxis / 10));
	}
}
