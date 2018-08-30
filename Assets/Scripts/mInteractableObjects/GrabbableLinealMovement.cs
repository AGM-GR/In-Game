using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

public class GrabbableLinealMovement : BaseGrabbable {

    [SerializeField]
    protected Transform objectToMove;
    [SerializeField]
    protected float maxDistance = 0.1f;

    [Header("Movement Restriction")]
    [SerializeField]
    protected Vector3 linealMovementDirection = Vector3.forward;
    [SerializeField]
    protected float movementSpeed = 1f;
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

    private Grabber grabber;
    private Transform grabberTransform;
    private Vector3 grabbableInitialPosition;
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
        grabber = null;
        grabbableInitialPosition = GrabPoint;
        initialPosition = MovementTransform.position;
    }

    protected override void AttachToGrabber(BaseGrabber grabber) {
        base.AttachToGrabber(grabber);
        this.grabber = (Grabber)grabber;
        grabberTransform = GrabberPrimary.GrabHandle;
        StopCoroutine(AutoReturn());
    }

    protected override void DetachFromGrabber(BaseGrabber grabber) {
        base.DetachFromGrabber(grabber);
        this.grabber = null;
        if (autoReturnToInitialPosition)
            StartCoroutine(AutoReturn());
    }

    protected override void Update() {
        base.Update();
        if (grabber != null) {

            dest = grabberTransform.position - grabbableInitialPosition;
            movement = new Vector3(Mathf.Abs(GlobalMovementDirection.x) * dest.x, Mathf.Abs(GlobalMovementDirection.y) * dest.y, Mathf.Abs(GlobalMovementDirection.z) * dest.z);
            float distance = (grabberTransform.position - GrabPoint).magnitude;

            if (distance > maxDistance)
                grabber.FinishGrab();
            else if (!useLimits || (movement.magnitude < maxPosition && movement.magnitude > minPosition && GlobalMovementDirection.normalized == movement.normalized)) {
                MovementTransform.position = Vector3.MoveTowards(MovementTransform.position, initialPosition + movement, Time.deltaTime * movementSpeed);
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
