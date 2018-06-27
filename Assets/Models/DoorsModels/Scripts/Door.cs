using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	public GameObject leaf;
	public GameObject handle;

	public float openingAngle = 90f;

	public bool startLocked = true;

	Rigidbody leafRigidbody;
	HingeJoint handleJoint;

	void Awake () {

		leafRigidbody = leaf.GetComponent<Rigidbody> ();
		handleJoint = handle.GetComponent<HingeJoint> ();
	}

	void Start () {

		if (startLocked)
			LockDoor ();
	}

	void LockDoor () {

		leafRigidbody.isKinematic = true;
	}

	void UnlockDoor () {

		leafRigidbody.isKinematic = false;
	}

	void Update () {

		if (Mathf.Abs(handleJoint.angle) >= Mathf.Abs(40))
			UnlockDoor ();
	}
}