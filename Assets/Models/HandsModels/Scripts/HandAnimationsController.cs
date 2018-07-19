using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimationsController : MonoBehaviour {

	Animator animator;

	void Awake () {
		animator = GetComponentInChildren<Animator> ();
	}

	public void SetAnimation (HandActionsEnum handAction) {
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
			animator.SetBool ("Spot", true);
			break;
		}
	}

	public void SetUndoAnimation (HandActionsEnum handAction) {
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
			animator.SetBool ("Spot", false);
			break;
		}
	}
}
