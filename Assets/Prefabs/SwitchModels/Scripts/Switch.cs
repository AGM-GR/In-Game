using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour {

	[Header("Switch Methods")]
	public UnityEvent switchOnMethods;
	public UnityEvent switchOffMethods;

	[Header("Switcher Options")]
	public GrabbableRotation grabbable;
	public bool lockOnSwitchOn = false;

	public void SwitchOn() {
		switchOnMethods.Invoke();
	}

	public void SwitchOff() {
		switchOffMethods.Invoke();
	}
}
