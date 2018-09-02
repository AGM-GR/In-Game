using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour {

    [Header("Sounds")]
    public AudioClip switchOnSound;
    public AudioClip switchOffSound;

    [Header("Switch Methods")]
	public UnityEvent switchOnMethods;
	public UnityEvent switchOffMethods;

	[Header("Switcher Options")]
	public GrabbableRotation grabbable;
	public bool lockOnSwitchOn = false;

    private AudioSource audioSource;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void SwitchOn() {
        audioSource.PlayOneShot(switchOnSound);
		switchOnMethods.Invoke();
	}

	public void SwitchOff() {
        audioSource.PlayOneShot(switchOffSound);
        switchOffMethods.Invoke();
	}
}
