using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LibraryCodeController : MonoBehaviour {

    public Transform objectToMove;
    public Vector3 openPosition;
    public float movementSpeed = 1f;

    [Header("Events")]
    public UnityEvent SecretUnlock;
    public UnityEvent SecretLock;

    private Dictionary<NumberSelector, bool> codeStatus = new Dictionary<NumberSelector, bool>();

    private Vector3 initialPosition;
    private Vector3 initialMovementPosition;
    private float lerpStep = 0f;
    private bool opened = false;

    public Transform ObjectToMove {
        get {
            return objectToMove ? objectToMove : transform;
        }
    }

    private void Awake() {
        initialPosition = ObjectToMove.position;
    }

    public void AddCodeStatus(NumberSelector numSelector, bool status) {
        codeStatus.Add(numSelector, status);
    }

    public void UpdateCodeStatus(NumberSelector numSelector, bool status) {
        codeStatus[numSelector] = status;
        if (status) {
            CheckCodeStatus();
        }
        else if (opened) {

            SecretLock.Invoke();
            StopCoroutine(OpenSecret());
            StartCoroutine(CloseSecret());
        }
    }

    private void CheckCodeStatus() {
        foreach (KeyValuePair<NumberSelector, bool> status in codeStatus) {
            if (!status.Value)
                return;
        }

        SecretUnlock.Invoke();
        StopCoroutine(CloseSecret());
        StartCoroutine(OpenSecret());
    }

    IEnumerator OpenSecret() {
        lerpStep = 0f;
        opened = true;
        initialMovementPosition = ObjectToMove.position;
        while (lerpStep < 1f) {
            lerpStep = lerpStep + Time.deltaTime * movementSpeed;
            ObjectToMove.position = Vector3.Lerp(initialMovementPosition, openPosition, lerpStep);
            yield return null;
        }
    }

    IEnumerator CloseSecret() {
        lerpStep = 0f;
        opened = false;
        initialMovementPosition = ObjectToMove.position;
        while (lerpStep < 1f) {
            lerpStep = lerpStep + Time.deltaTime * movementSpeed;
            ObjectToMove.position = Vector3.Lerp(initialMovementPosition, initialPosition, lerpStep);
            yield return null;
        }
    }

    private void OnDrawGizmosSelected() {
        float cubeSize = 0.02f;
        Gizmos.DrawWireCube(ObjectToMove.position, Vector3.one * cubeSize);
        Gizmos.DrawWireCube(openPosition, Vector3.one * cubeSize);
        Gizmos.DrawLine(ObjectToMove.position, openPosition);
    }

}
