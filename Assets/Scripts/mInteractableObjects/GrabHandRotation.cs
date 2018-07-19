using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

public class GrabHandRotation : MonoBehaviour {

	[SerializeField]
	private BaseGrabbable grabbable;

	[Header("Rotation Restriction")]
	[SerializeField]
	protected Transform grabbableTransform;
	[SerializeField]
	protected Transform grabSpot;
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
	private Transform grabberTransform;
	private Quaternion initialrotation;
	private Quaternion worldRotation;
	private Vector3 dest;
	private Vector3 lastDest;

	private Transform RotateTransform {
		get {
			return grabSpot != null ? grabSpot : transform; 
		}
	}

	private Vector3 GlobalForwardDirection {
		get { 
			if (grabbableTransform != null)
				return (grabbableTransform.rotation * forwardDirection);
			return forwardDirection;
		}
	}

	private Vector3 GlobalRotationAxis {
		get { 
			if (grabbableTransform != null)
				return grabbableTransform.rotation * rotationAxis;
			return rotationAxis;
		}
	}

	void Awake() {
		grabState = GrabStateEnum.Inactive;
		initialrotation = RotateTransform.localRotation;
		dest = GlobalForwardDirection;
		lastDest = GlobalForwardDirection;

		grabbable = GetComponent<BaseGrabbable> ();

		grabbable.OnContactStateChange += UpdateGrabState;
		grabbable.OnGrabStateChange += UpdateGrabState;
	}

	private void UpdateGrabState(BaseGrabbable baseGrab) {
		if (baseGrab.GrabState != GrabStateEnum.Inactive)
			grabberTransform = baseGrab.GrabberPrimary.GetComponent<Transform> ();
		grabState = baseGrab.GrabState;
	}

	void Update() {

		if (grabState != GrabStateEnum.Inactive) {
			dest = grabberTransform.position - RotateTransform.position;
			//Se proyecta el punto de agarre en el plano perpendicular al eje de giro para obtener el vector de destino
			dest = Vector3.ProjectOnPlane (dest, GlobalRotationAxis.normalized);

			if (!useLimits || (GetAngle () >= minAngle && GetAngle () <= maxAngle)) {
				//Devuelve el objeto a su rotación inicial local y obtiene la rotación global de este
				RotateTransform.localRotation = initialrotation;
				worldRotation = RotateTransform.rotation;
				//Rota el object desde la posición indicada como forward hasta el vector de destino
				RotateTransform.rotation = Quaternion.FromToRotation (GlobalForwardDirection.normalized, dest.normalized);
				//Añade la rotación del object global inicial
				RotateTransform.Rotate (worldRotation.eulerAngles);
			} else {
				dest = lastDest;
			}

			lastDest = dest;
		}
	}

	public float GetAngle() {
		return Vector3.SignedAngle (GlobalForwardDirection, dest, GlobalRotationAxis);
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
