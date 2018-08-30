using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour {

	[Header("Door Elements")]
	public GrabHingeRotation doorHinge;
	public List<GrabbableRotation> handles = new List<GrabbableRotation>();

	[Header("Key Lock")]
	[SerializeField]
	protected bool locked = false;
	[SerializeField]
	protected int keyId = 0;
    public UnityEvent OnUnlockDoor;
    public UnityEvent OnLockDoor;

    private float openingAngle = 40f;
	private float clossingAngle = 2f;

	void Awake() {
		if (doorHinge == null)
			doorHinge = GetComponentInChildren<GrabHingeRotation> ();
	}

	void Start () {
		CloseDoor ();
	}

	void Update () {

		if (!locked) {
			if (handles.Exists (handle => (Mathf.Abs (handle.GetAngle()) >= Mathf.Abs (openingAngle))))
				OpenDoor ();
			else if (Mathf.Abs (doorHinge.GetAngle()) <= clossingAngle)
				CloseDoor ();
		}
	}


	private void OpenDoor () {
		doorHinge.enableRotation = true;
	}

	private void CloseDoor () {
		doorHinge.enableRotation = false;
	}


	public void UnlockDoor () {
        OnUnlockDoor.Invoke();
        locked = false;
	}

	public void LockDoor () {

        OnLockDoor.Invoke();
        locked = true;
	}


	public bool isLocked() {
		return locked;
	}

	public int GetKeyId () {
		return keyId;
	}
}