using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimationsController : MonoBehaviour {

    [SerializeField]
    private bool changeColliderSizeOnPoint = false;
    [SerializeField]
    private Vector3 center = Vector3.zero;
    [SerializeField]
    private Vector3 size = Vector3.one;

    Vector3 originalCenter;
    Vector3 originalSize;
    BoxCollider boxCollider;
	Animator animator;
    bool inAnimation = false;

	void Awake () {
		animator = GetComponentInChildren<Animator> ();
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider) {
            originalCenter = boxCollider.center;
            originalSize = boxCollider.size;
        }
    }

	public void SetAnimation (HandActionsEnum handAction) {
        inAnimation = true;
        switch (handAction) {
		    case HandActionsEnum.GRAB:
			    animator.SetBool ("Grab", true);
			    break;
		    case HandActionsEnum.GRABKEY:
			    animator.SetBool ("GrabKey", true);
			    break;
		    case HandActionsEnum.HANDLE:
			    animator.SetBool ("Handle", true);
			    break;
		    case HandActionsEnum.SPOT:
			    animator.SetBool ("Point", true);
                if (changeColliderSizeOnPoint) {
                    boxCollider.center = center;
                    boxCollider.size = size;
                }
                break;
		}
	}

	public void SetUndoAnimation (HandActionsEnum handAction) {
        boxCollider.center = originalCenter;
        boxCollider.size = originalSize;
        inAnimation = false;
        switch (handAction) {
		    case HandActionsEnum.GRAB:
			    animator.SetBool ("Grab", false);
			    break;
		    case HandActionsEnum.GRABKEY:
			    animator.SetBool ("GrabKey", false);
			    break;
		    case HandActionsEnum.HANDLE:
			    animator.SetBool ("Handle", false);
			    break;
		    case HandActionsEnum.SPOT:
			    animator.SetBool ("Point", false);
			    break;
		}
	}

    private void OnTriggerEnter(Collider other) {
        if (other.enabled && other.gameObject.GetComponent<InteractZone>() != null)
            if(!inAnimation)
                SetAnimation(other.gameObject.GetComponent<InteractZone>().handAction);
    }

    private void OnTriggerExit(Collider other) {
        if (other.enabled && other.gameObject.GetComponent<InteractZone>() != null)
            SetUndoAnimation(other.gameObject.GetComponent<InteractZone>().handAction);
    }

    void OnDrawGizmosSelected() {
        if (!Application.isPlaying && changeColliderSizeOnPoint) {
            Gizmos.DrawWireCube(transform.position + center, size);
        }
    }
}
