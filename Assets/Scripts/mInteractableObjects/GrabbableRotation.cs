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
	protected Transform parentLocalDirectionJoin;
	[SerializeField]
	protected Vector3 forwardDirection = Vector3.forward;
	[SerializeField]
	protected Vector3 rotationAxis = Vector3.up;

    [Header("Auto Return")]
    [SerializeField]
    protected bool autoReturnToInitialRotation = false;
    [SerializeField]
    protected float autoReturnSpeed = 2f;

    [Header("Rotation Limits")]
	[SerializeField]
	protected bool useLimits = false;
	[SerializeField]
	protected int minAngle = 0;
	[SerializeField]
	protected int maxAngle = 0;

	private Grabber grabber;
	private Transform grabberTransform;
	private Quaternion initialRotation;
	private Quaternion worldRotation;
	private Vector3 dest;
	private Vector3 lastDest;

    private Quaternion lastRotation;
    private float lerpStep;

    private Transform RotateTransform {
		get {
			return objectToRotate != null ? objectToRotate : transform; 
		}
	}

	private Vector3 GlobalForwardDirection {
		get { 
			if (parentLocalDirectionJoin != null)
				return (parentLocalDirectionJoin.rotation * forwardDirection);
			return forwardDirection;
		}
	}

	private Vector3 GlobalRotationAxis {
		get { 
			if (parentLocalDirectionJoin != null)
				return parentLocalDirectionJoin.rotation * rotationAxis;
			return rotationAxis;
		}
	}

	void Awake() {
		grabber = null;
        initialRotation = RotateTransform.localRotation;
		dest = GlobalForwardDirection;
		lastDest = GlobalForwardDirection;
	}

	protected override void AttachToGrabber(BaseGrabber grabber) {
		base.AttachToGrabber (grabber);
		this.grabber = (Grabber)grabber;
		grabberTransform = GrabberPrimary.GrabHandle;
        StopCoroutine(AutoReturn());
    }

	protected override void DetachFromGrabber(BaseGrabber grabber) {
		base.DetachFromGrabber(grabber);
		this.grabber = null;
        if (autoReturnToInitialRotation)
            StartCoroutine(AutoReturn());
    }

	protected override void Update() {
		base.Update ();
		if (grabber != null) {

			dest = grabberTransform.position - RotateTransform.position;
			float distance = (grabberTransform.position - GrabPoint).magnitude;
			//Se proyecta el punto de agarre en el plano perpendicular al eje de giro para obtener el vector de destino
			dest = Vector3.ProjectOnPlane (dest, GlobalRotationAxis.normalized);

			if (distance > maxDistance)
				grabber.FinishGrab ();
			else if (!useLimits || (GetAngle() >= minAngle && GetAngle() <= maxAngle)){
				//Devuelve el objeto a su rotación inicial local y obtiene la rotación global de este
				RotateTransform.localRotation = initialRotation;
				worldRotation = RotateTransform.rotation;
				//Rota el object desde la posición indicada como forward hasta el vector de destino
				RotateTransform.rotation = Quaternion.FromToRotation (GlobalForwardDirection.normalized, dest.normalized);
				//Añade la rotación del object global inicial
				RotateTransform.Rotate(worldRotation.eulerAngles);
			} else {
				dest = lastDest;
			}

			lastDest = dest;
		}
	}

	public float GetAngle() {
		return Vector3.SignedAngle (GlobalForwardDirection, dest, GlobalRotationAxis);
	}

    private IEnumerator AutoReturn() {
        lerpStep = 0f;
        lastRotation = RotateTransform.localRotation;
        while (lerpStep < 1f) {
            lerpStep = lerpStep + Time.deltaTime * autoReturnSpeed;
            RotateTransform.localRotation = Quaternion.Lerp(lastRotation, initialRotation, lerpStep);
            yield return null;
        }
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
		Gizmos.DrawRay(RotateTransform.position, GlobalForwardDirection /4);
		//Eje de rotación
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(RotateTransform.position, RotateTransform.position + (GlobalRotationAxis / 10));
	}
}
