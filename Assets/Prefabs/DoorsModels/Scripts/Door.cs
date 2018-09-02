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

    [Header("Sounds")]
    public AudioClip doorOpen;
    public AudioClip doorClosed;
    public AudioClip doorLocked;
    public AudioClip unlockDoor;

    private AudioSource audioSource;
    private float openingAngle = 40f;
	private float clossingAngle = 2f;
    private bool triedToOpenDoorLocked = false;

	void Awake() {
		if (doorHinge == null)
			doorHinge = GetComponentInChildren<GrabHingeRotation> ();

        audioSource = GetComponent<AudioSource>();
    }

	void Start () {
        //CloseDoor
        doorHinge.enableRotation = false;
    }

	void Update () {

        if (!locked) {
            if (handles.Exists(handle => (Mathf.Abs(handle.GetAngle()) >= Mathf.Abs(openingAngle))))
                OpenDoor();
            else if (Mathf.Abs(doorHinge.GetAngle()) <= clossingAngle)
                CloseDoor();
        }
        else if (handles.Exists(handle => (Mathf.Abs(handle.GetAngle()) >= Mathf.Abs(openingAngle)))) {
            if (!triedToOpenDoorLocked)
                audioSource.PlayOneShot(doorLocked);
            triedToOpenDoorLocked = true;
        }
        else {
            triedToOpenDoorLocked = false;
        }

    }


	private void OpenDoor () {
        if (!doorHinge.enableRotation)
            audioSource.PlayOneShot(doorOpen);
        doorHinge.enableRotation = true;
	}

	private void CloseDoor () {
        if (doorHinge.enableRotation)
            audioSource.PlayOneShot(doorClosed);
        doorHinge.enableRotation = false;
	}


	public void UnlockDoor () {
        audioSource.PlayOneShot(unlockDoor);
        OnUnlockDoor.Invoke();
        locked = false;
	}

	public void LockDoor () {
        audioSource.PlayOneShot(unlockDoor);
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