using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

public class InteractZone : MonoBehaviour {

    public HandActionsEnum handAction = HandActionsEnum.SPOT;
    public float maxDistanceInteract = 1f;

    private HandAnimationsController handAnimator;
    private float distance;

    public void SetHandAnimator(HandAnimationsController handAnimations) {
        if (handAnimations != null) {
            handAnimator = handAnimations;
            handAnimator.SetAnimation(handAction);
            StartCoroutine(DistanceCheck());
        }
    }

    public void DisconnectHand() {
        if (handAnimator != null) {
            handAnimator.SetUndoAnimation(handAction);
            handAnimator = null;
        }
    }

    IEnumerator DistanceCheck() {
        distance = Vector3.Distance(transform.position, handAnimator.GetComponent<Grabber>().GrabHandle.position);
        while (distance < maxDistanceInteract && handAnimator != null) {
            distance = Vector3.Distance(transform.position, handAnimator.GetComponent<Grabber>().GrabHandle.position);
            yield return null;
        }

        DisconnectHand();
    }

    void OnDrawGizmosSelected() {
        if (!Application.isPlaying) {
            Gizmos.DrawWireSphere(transform.position, maxDistanceInteract);
            Color color = Color.white;
            color.a = 0.4f;
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, maxDistanceInteract);
        }
    }

    void OnDrawGizmos() {
        if (Application.isPlaying && handAnimator) {
            Gizmos.DrawLine(transform.position, handAnimator.transform.position);
        }
    }
}
