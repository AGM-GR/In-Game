using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

public class OnGrabLinealMovement : MonoBehaviour {

    [Header("Grabbable")]
    [SerializeField]
    private BaseGrabbable grabbable;

    [Header("Movement Restriction")]
    [SerializeField]
    protected Transform objectToMove;
    [SerializeField]
    protected Vector3 linealMovementDirection = Vector3.forward;
    [SerializeField]
    protected bool invertMovementDirection = false;

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
        initialPosition = MovementTransform.position;

        if (grabbable == null)
            grabbable = GetComponent<BaseGrabbable>();

        grabbable.OnGrabStateChange += UpdateGrabState;
        grabbableInitialPosition = grabbable.GrabPoint;
    }

    private void UpdateGrabState(BaseGrabbable baseGrab) {
        if (baseGrab.GrabState != GrabStateEnum.Inactive)
            AttachToGrabber(baseGrab.GrabberPrimary);
        else
            DetachFromGrabber(baseGrab.GrabberPrimary);
    }

    protected void AttachToGrabber(BaseGrabber grabber) {
        this.grabber = (Grabber)grabber;
        grabberTransform = grabber.GrabHandle;
    }

    protected void DetachFromGrabber(BaseGrabber grabber) {
        this.grabber = null;
    }

    protected void Update() {
        if (grabber != null) {
            dest = grabberTransform.position - grabbableInitialPosition;
            movement = new Vector3(Mathf.Abs(GlobalMovementDirection.x) * dest.x, Mathf.Abs(GlobalMovementDirection.y) * dest.y, Mathf.Abs(GlobalMovementDirection.z) * dest.z);
            if (invertMovementDirection)
                movement = movement * -1;
            float distance = (grabberTransform.position - grabbable.GrabPoint).magnitude;

            if (!useLimits || (movement.magnitude < maxPosition && movement.magnitude > minPosition && GlobalMovementDirection.normalized == movement.normalized)) {
                MovementTransform.position = initialPosition + movement;
            }
        }
    }

    void OnDrawGizmosSelected() {
        //Movimiento
        Gizmos.color = Color.yellow;
        float cubeSize = 0.02f;
        if (!useLimits) {
            Gizmos.DrawWireCube(MovementTransform.position, Vector3.one * cubeSize);
            Gizmos.DrawRay(MovementTransform.position, GlobalMovementDirection);
        }
        else {
            Gizmos.DrawWireCube(new Vector3(MovementTransform.position.x + GlobalMovementDirection.x * minPosition,
                MovementTransform.position.y + GlobalMovementDirection.y * minPosition,
                MovementTransform.position.z + GlobalMovementDirection.z * minPosition), Vector3.one * cubeSize);
            Gizmos.DrawWireCube(new Vector3(MovementTransform.position.x + GlobalMovementDirection.x * maxPosition,
                MovementTransform.position.y + GlobalMovementDirection.y * maxPosition,
                MovementTransform.position.z + GlobalMovementDirection.z * maxPosition), Vector3.one * cubeSize);
            Gizmos.DrawLine(new Vector3(MovementTransform.position.x + GlobalMovementDirection.x * minPosition,
                MovementTransform.position.y + GlobalMovementDirection.y * minPosition,
                MovementTransform.position.z + GlobalMovementDirection.z * minPosition),
                new Vector3(MovementTransform.position.x + GlobalMovementDirection.x * maxPosition,
                MovementTransform.position.y + GlobalMovementDirection.y * maxPosition,
                MovementTransform.position.z + GlobalMovementDirection.z * maxPosition));
        }
    }
}