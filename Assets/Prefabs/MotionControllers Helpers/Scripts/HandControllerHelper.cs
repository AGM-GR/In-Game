using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

public class HandControllerHelper : MonoBehaviour {

    public BaseGrabber grabber;
    public BaseInteractor interactor;

    [Header("Helpers")]
    public GameObject grabHelper;
    public GameObject interactHelper;

    private Vector3 initialGrabScale;
    private Vector3 initialInteractScale;
    private Vector3 hideScale;
    private float scaleSpeed = 18f;

    private Coroutine grabCoroutine;
    private Coroutine interactCoroutine;

    private void Awake() {
        initialGrabScale = grabHelper.transform.localScale;
        initialInteractScale = interactHelper.transform.localScale;

        hideScale = Vector3.zero;

        grabHelper.transform.localScale = hideScale;
        interactHelper.transform.localScale = hideScale;

        if (grabber == null)
            grabber = GetComponent<BaseGrabber>();
        if (interactor == null)
            interactor = GetComponent<BaseInteractor>();

        if (grabber) {
            grabber.OnContactStateChange += UpdateHelpers;
            grabber.OnGrabStateChange += UpdateHelpers;
        }

        if (interactor) {
            interactor.OnContactStateChange += UpdateHelpers;
            interactor.OnInteractStateChange += UpdateHelpers;
        }
    }

    private void UpdateHelpers(BaseGrabber baseGrabber) {
        if (grabCoroutine != null)
            StopCoroutine(grabCoroutine);
        if (baseGrabber.GrabState != GrabStateEnum.Inactive) {
            grabCoroutine = StartCoroutine(HideHelper(grabHelper.transform));
        }
        else if (baseGrabber.ContactState != GrabStateEnum.Inactive) {
            grabCoroutine = StartCoroutine(ShowHelper(grabHelper.transform, initialGrabScale));
        }
        else if (baseGrabber.ContactState == GrabStateEnum.Inactive) {
            grabCoroutine = StartCoroutine(HideHelper(grabHelper.transform));
        }
    }

    private void UpdateHelpers(BaseInteractor baseInteractor) {
        if (interactCoroutine != null)
            StopCoroutine(interactCoroutine);
        if (baseInteractor.InteractState != GrabStateEnum.Inactive) {
            interactCoroutine = StartCoroutine(HideHelper(interactHelper.transform));
        }
        else if (baseInteractor.ContactState != GrabStateEnum.Inactive) {
            interactCoroutine = StartCoroutine(ShowHelper(interactHelper.transform, initialInteractScale));
        }
        else if (baseInteractor.ContactState == GrabStateEnum.Inactive) {
            interactCoroutine = StartCoroutine(HideHelper(interactHelper.transform));
        }
    }

    IEnumerator ShowHelper(Transform helper, Vector3 scale) {
        while (helper.localScale != scale) {
            helper.localScale = Vector3.MoveTowards(helper.localScale, scale, Time.deltaTime * scaleSpeed);
            yield return null;
        }
    }

    IEnumerator HideHelper(Transform helper) {
        while (helper.localScale != hideScale) {
            helper.localScale = Vector3.MoveTowards(helper.localScale, hideScale, Time.deltaTime * scaleSpeed);
            yield return null;
        }
    }


}
