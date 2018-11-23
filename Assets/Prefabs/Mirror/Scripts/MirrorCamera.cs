using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorCamera : MonoBehaviour {

    public float horizontalSpace = 1.0f;
    public float verticalSpace = 1.0f;

    private Transform mainCamera;
    private Vector3 cameraPosition;
    private Quaternion cameraRotation;
    private Vector3 mirrorForward;

    private Vector3 reflectionVector;
    private Vector3 position;

    private void Awake() {
        mainCamera = Camera.main.transform;
        cameraPosition = transform.position;
        cameraRotation = transform.rotation;
        mirrorForward = transform.forward;
    }

    private void Update() {

        position = transform.position;
        if (cameraPosition.y + verticalSpace > mainCamera.position.y && cameraPosition.y - verticalSpace < mainCamera.position.y) {
            position.y = mainCamera.position.y;
        }
        if (cameraPosition.x + horizontalSpace > mainCamera.position.x && cameraPosition.x - horizontalSpace < mainCamera.position.x) {
            position.x = mainCamera.position.x;
        }
        transform.position = position;

        reflectionVector = Vector3.Reflect(transform.position - mainCamera.position, mirrorForward);
        transform.rotation = cameraRotation * Quaternion.FromToRotation(mirrorForward, reflectionVector);

    }

    void OnDrawGizmosSelected() {
        Gizmos.DrawLine(new Vector3(transform.position.x + horizontalSpace, transform.position.y, transform.position.z), new Vector3(transform.position.x - horizontalSpace, transform.position.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + verticalSpace, transform.position.z), new Vector3(transform.position.x, transform.position.y - verticalSpace, transform.position.z));
    }
}
