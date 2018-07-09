﻿using System.Collections;
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
	private Transform grabberTransform;
	private Vector3 initialrotation;
	private Vector3 dest;

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
		initialrotation = RotateTransform.rotation.eulerAngles;
		dest = GlobalForwardDirection;

		if (grabbables.Count == 0)
			grabbables.Add(GetComponent<BaseGrabbable> ());

		foreach (BaseGrabbable grabbable in grabbables) {
			//Subscribe los eventos al tocar un objeto y agarrarlo
			grabbable.OnContactStateChange += UpdateGrabState;
			grabbable.OnGrabStateChange += UpdateGrabState;
		}
	}

	private void UpdateGrabState(BaseGrabbable baseGrab) {
		Debug.Log ("Updated: " + baseGrab.GrabState + " Angle: " + GetAngle() + " Enabled Rotation: " + enableRotation);
		if (baseGrab.GrabState != GrabStateEnum.Inactive)
			grabberTransform = baseGrab.GrabberPrimary.GetComponent<Transform> ();
		grabState = baseGrab.GrabState;
	}

	void FixedUpdate() {
		if (grabState != GrabStateEnum.Inactive && enableRotation) {
			if (!useLimits || (GetAngle() >= minAngle && GetAngle() <= maxAngle)){
				dest = grabberTransform.position - RotateTransform.position;
				//Se proyecta el punto de agarre en el plano perpendicular al eje de giro para obtener el vector de destino
				dest = Vector3.ProjectOnPlane (dest, GlobalRotationAxis.normalized);
				//Rota el object desde la posición indicada como forward hasta el vector de destino
				RotateTransform.rotation = Quaternion.FromToRotation (GlobalForwardDirection.normalized, dest.normalized);
				//Añade la rotación del object inicial
				RotateTransform.Rotate (initialrotation);
			}
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