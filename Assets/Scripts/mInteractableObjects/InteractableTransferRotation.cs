using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

public class InteractableTransferRotation : BaseInteractable {

	[SerializeField]
	protected Transform objectToRotate;
	[SerializeField]
	protected float maxDistance = 1f;

	[Header("Rotation Interactable")]
	[SerializeField]
	protected Transform parentJoin;
	[SerializeField]
	protected Vector3 rotationAxis;

	[Header("Rotation Limits")]
	[SerializeField]
	protected bool useLimits = false;
	[SerializeField]
	protected int minAngle = 0;
	[SerializeField]
	protected int maxAngle = 0;

	public UnityEvent minLimitReached;
	public UnityEvent maxLimitReached;
	public UnityEvent limitsLeave;


	[Header("Rotation Controller")]
	[SerializeField]
	protected Vector3 controllerRotationAxis;

	private Interactor interactor;
	private Transform interactorTransform;
	private float initialAngleRotation;
	private Quaternion initialLocalRotation;
	private bool firstInteraction;
	private float currentAngle;
	private float angleToRotate;
	private float distance;
	private bool outValues;
	private float outValuesLastAngle;

	private Transform RotateTransform {
		get {
			return objectToRotate != null ? objectToRotate : transform; 
		}
	}

	private Vector3 GlobalRotationAxis {
		get { 
			if (parentJoin != null)
				return parentJoin.rotation * rotationAxis;
			return rotationAxis;
		}
	}

	void Awake () {
		interactor = null;
		firstInteraction = true;
		currentAngle = 0f;
		angleToRotate = 0f;
		outValues = false;
	}

	protected override void AttachToInteractor(BaseInteractor interactor) {
		base.AttachToInteractor (interactor);
		this.interactor = (Interactor)interactor;
		interactorTransform = InteractorPrimary.InteractHandle;
	}

	protected override void DetachFromInteractor(BaseInteractor interactor) {
		base.DetachFromInteractor(interactor);
		this.interactor = null;
	}

	protected override void Update() {
		base.Update ();
		if (interactor != null) {

			if (firstInteraction) {
				initialAngleRotation = (Vector3.Scale (interactorTransform.rotation.eulerAngles, controllerRotationAxis)).magnitude;
				initialLocalRotation = RotateTransform.localRotation;
				firstInteraction = false;
			}

			distance = (interactorTransform.position - InteractPoint).magnitude;

			if (distance > maxDistance)
				interactor.FinishInteraction ();
			else {
				angleToRotate = Mathf.DeltaAngle(initialAngleRotation, (Vector3.Scale (interactorTransform.rotation.eulerAngles, controllerRotationAxis)).magnitude);
				if (useLimits) {
					if (outValues) {
						if (currentAngle + angleToRotate > minAngle && currentAngle + angleToRotate < maxAngle) {
							limitsLeave.Invoke ();
							outValues = false;
						} else {
							angleToRotate = outValuesLastAngle;
						}
					} else if ((currentAngle + angleToRotate) < minAngle) {
						angleToRotate = minAngle - currentAngle;
						outValuesLastAngle = angleToRotate;
						outValues = true;
						minLimitReached.Invoke ();
					} else if ((currentAngle + angleToRotate) > maxAngle) {
						angleToRotate = maxAngle - currentAngle;
						outValuesLastAngle = angleToRotate;
						outValues = true;
						maxLimitReached.Invoke ();
					}
				}

				RotateTransform.localRotation = initialLocalRotation * Quaternion.AngleAxis(angleToRotate, rotationAxis);
			}
		} else if (!firstInteraction) {
			currentAngle += angleToRotate;
			firstInteraction = true;
		}
	}

	void OnDrawGizmosSelected() {
		//Distancia a la que el Grab se desactiva
		Color color = Color.blue;
		color.a = 0.1f;
		Gizmos.color = color;
		Gizmos.DrawSphere(RotateTransform.position, maxDistance);
		color.a = 0.4f;
		Gizmos.color = color;
		Gizmos.DrawWireSphere(RotateTransform.position, maxDistance);

		//Eje de rotación
		Gizmos.color = Color.red;
		Gizmos.DrawLine(RotateTransform.position, RotateTransform.position + (GlobalRotationAxis / 4));
	}
}
