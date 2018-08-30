using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableLinealMovement : BaseInteractable {

    [SerializeField]
    protected Transform objectToMove;
    [SerializeField]
    protected float maxDistance = 0.1f;

    [Header("Movement Restriction")]
    [SerializeField]
    protected Vector3 linealMovementDirection = Vector3.forward;
    [SerializeField]
    protected bool autoReturnToInitialPosition = false;
    [SerializeField]
    protected float autoReturnSpeed = 1f;

    [Header("Movement Limits")]
    [SerializeField]
    protected bool useLimits = false;
    [SerializeField]
    protected float minPosition = 0;
    [SerializeField]
    protected float maxPosition = 0;

    [Header("Interact Methods")]
    public UnityEvent reachedMinLimit;
    public UnityEvent reachedMaxLimit;

    private bool inMinLimit = false;
    private bool inMaxLimit = false;

    private Interactor interactor;
    private Transform interactorTransform;
    private Vector3 interactableInitialPosition;
    private Vector3 initialPosition;
    private Vector3 movement;
    private Vector3 dest;

    private Vector3 lastposition;
    private float lerpStep;

    private Transform MovementTransform {
        get {
            return objectToMove != null ? objectToMove : transform;
        }
    }

    private Vector3 GlobalMovementDirection {
        get {
            return (MovementTransform.rotation * linealMovementDirection.normalized);
        }
    }

    void Awake() {
        interactor = null;
        interactableInitialPosition = InteractPoint;
        initialPosition = MovementTransform.position;
    }

    protected override void AttachToInteractor(BaseInteractor interactor) {
        base.AttachToInteractor(interactor);
        this.interactor = (Interactor)interactor;
        interactorTransform = InteractorPrimary.InteractHandle;
        StopCoroutine(AutoReturn());
    }

    protected override void DetachFromInteractor(BaseInteractor interactor) {
        base.DetachFromInteractor(interactor);
        this.interactor = null;
        if (autoReturnToInitialPosition)
            StartCoroutine(AutoReturn());
    }

    protected override void Update() {
        base.Update();
        if (interactor != null) {

            dest = interactorTransform.position - interactableInitialPosition;
            movement = new Vector3(Mathf.Abs(GlobalMovementDirection.x) * dest.x, Mathf.Abs(GlobalMovementDirection.y) * dest.y, Mathf.Abs(GlobalMovementDirection.z) * dest.z);
            float distance = (interactorTransform.position - InteractPoint).magnitude;

            if (distance > maxDistance)
                interactor.FinishInteraction();
            else if (!useLimits || (movement.magnitude < maxPosition && movement.magnitude > minPosition && GlobalMovementDirection.normalized == movement.normalized)) {
                MovementTransform.position = initialPosition + movement;
            }

            if (useLimits) {
                if (!inMaxLimit && movement.magnitude >= maxPosition) {
                    reachedMaxLimit.Invoke();
                    inMaxLimit = true;
                }
                if (!inMinLimit && (movement.magnitude <= minPosition || GlobalMovementDirection.normalized != movement.normalized)) {
                    reachedMinLimit.Invoke();
                    inMinLimit = true;
                }
                if (inMaxLimit && movement.magnitude < maxPosition) {
                    inMaxLimit = false;
                }
                if (inMinLimit && movement.magnitude > minPosition) {
                    inMinLimit = false;
                }
            }
        }
    }

    private IEnumerator AutoReturn() {
        lerpStep = 0f;
        lastposition = MovementTransform.position;
        while (lerpStep < 1f) {
            lerpStep = lerpStep + Time.deltaTime * autoReturnSpeed;
            MovementTransform.position = Vector3.Lerp(lastposition, initialPosition, lerpStep);
            yield return null;
        }

        //WARNING: La posición inicial no tiene por que ser el limite mínimo
        reachedMinLimit.Invoke();
        inMinLimit = true;
    }

    void OnDrawGizmosSelected() {
        //Distancia a la que el Grab se desactiva
        Color color = Color.blue;
        color.a = 0.1f;
        Gizmos.color = color;
        Gizmos.DrawSphere(InteractPoint, maxDistance);
        color.a = 0.4f;
        Gizmos.color = color;
        Gizmos.DrawWireSphere(InteractPoint, maxDistance);

        //Movimiento
        Gizmos.color = Color.yellow;
        float cubeSize = 0.02f;
        if (!useLimits) {
            Gizmos.DrawWireCube(MovementTransform.position, Vector3.one * cubeSize);
            Gizmos.DrawRay(MovementTransform.position, GlobalMovementDirection);
        } else {
            Gizmos.DrawWireCube(new Vector3(MovementTransform.position.x + GlobalMovementDirection.x * minPosition,
                MovementTransform.position.y + GlobalMovementDirection.y * minPosition,
                MovementTransform.position.z + GlobalMovementDirection.z * minPosition), Vector3.one * cubeSize);
            Gizmos.DrawWireCube(new Vector3(MovementTransform.position.x + GlobalMovementDirection.x * maxPosition,
                MovementTransform.position.y + GlobalMovementDirection.y * maxPosition,
                MovementTransform.position.z + GlobalMovementDirection.z * maxPosition), Vector3.one * cubeSize);
            Gizmos.DrawLine(new Vector3(MovementTransform.position.x + GlobalMovementDirection.x * minPosition, 
                MovementTransform.position.y + GlobalMovementDirection.y * minPosition , 
                MovementTransform.position.z + GlobalMovementDirection.z * minPosition),
                new Vector3(MovementTransform.position.x + GlobalMovementDirection.x * maxPosition,
                MovementTransform.position.y + GlobalMovementDirection.y * maxPosition,
                MovementTransform.position.z + GlobalMovementDirection.z * maxPosition));
        }
    }
}
