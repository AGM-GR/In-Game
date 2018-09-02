using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPuzzleController : MonoBehaviour {

    public Transform objectToRotate;
    public Vector3 openRotation;
    public float rotationSpeed = 1f;

    [Header("Content")]
    public ContentController contentController;

    [Header("Sounds")]
    public AudioClip openingSecret;

    private Dictionary<LightKey, bool> codeStatus = new Dictionary<LightKey, bool>();
    private AudioSource audioSource;

    private Quaternion initialRotation;
    private Quaternion initialMovementRotation;
    private float lerpStep = 0f;
    private bool opened = false;

    public Transform ObjectToRotate {
        get {
            return objectToRotate ? objectToRotate : transform;
        }
    }

    private void Awake() {
        initialRotation = ObjectToRotate.rotation;
        if (contentController == null)
            contentController = GetComponent<ContentController>();
        audioSource = GetComponent<AudioSource>();
    }

    public void AddLightStatus(LightKey lightKey, bool status) {
        codeStatus.Add(lightKey, status);
    }

    public void UpdateLightStatus(LightKey lightKey, bool status) {
        codeStatus[lightKey] = status;
        if (status) {
            CheckCodeStatus();
        }
        else if (opened) {

            StopCoroutine(OpenSecret());
            StartCoroutine(CloseSecret());

            contentController.DeactivateContent();
        }
    }

    private void CheckCodeStatus() {
        foreach (KeyValuePair<LightKey, bool> status in codeStatus) {
            if (!status.Value)
                return;
        }

        StopCoroutine(CloseSecret());
        StartCoroutine(OpenSecret());

        contentController.ActivateContent();
    }

    IEnumerator OpenSecret() {
        lerpStep = 0f;
        opened = true;
        audioSource.PlayOneShot(openingSecret);
        initialMovementRotation = ObjectToRotate.rotation;
        while (lerpStep < 1f) {
            lerpStep = lerpStep + Time.deltaTime * rotationSpeed;
            ObjectToRotate.rotation = Quaternion.Lerp(initialMovementRotation, Quaternion.Euler(openRotation), lerpStep);
            yield return null;
        }
    }

    IEnumerator CloseSecret() {
        lerpStep = 0f;
        opened = false;
        initialMovementRotation = ObjectToRotate.rotation;
        while (lerpStep < 1f) {
            lerpStep = lerpStep + Time.deltaTime * rotationSpeed;
            ObjectToRotate.rotation = Quaternion.Lerp(initialMovementRotation, initialRotation, lerpStep);
            yield return null;
        }
    }
}
