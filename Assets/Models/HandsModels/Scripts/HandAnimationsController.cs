using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimationsController : MonoBehaviour {

	public bool grab = false;
	public bool sign = false;

	Animator animator;

	void Awake () {
		animator = GetComponentInChildren<Animator> ();
	}

	void Update () {

		animator.SetBool ("Grab", grab);
		animator.SetBool ("Point", sign);
	}


}
