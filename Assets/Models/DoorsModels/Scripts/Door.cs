using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	[Header("Door Elements")]
	public Rigidbody leaf;
	public HingeJoint doorHinge;
	public List<HingeJoint> handles = new List<HingeJoint>();

	[Header("Key Lock")]
	[SerializeField]
	protected bool locked = false;
	[SerializeField]
	protected int keyId = 0;

	private float openingAngle = 40f;
	private float clossingAngle = 2f;

	void Start () {
		
		LockDoor ();
	}

	void Update () {

		if (!locked) {
			if (handles.Exists (handle => Mathf.Abs (handle.angle) >= Mathf.Abs (openingAngle)))
				OpenDoor ();
			else if (Mathf.Abs (doorHinge.angle) <= clossingAngle)
				CloseDoor ();
		}
	}


	private void OpenDoor () {
		leaf.isKinematic = false;
	}

	private void CloseDoor () {
		leaf.isKinematic = true;
	}


	private void UnlockDoor () {

		locked = false;
	}

	private void LockDoor () {

		locked = true;
	}


	public bool isLocked() {
		return locked;
	}

	public int GetKeyId () {
		return keyId;
	}
}