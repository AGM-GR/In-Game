using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour {

	[Header("Switch Methods")]
	public UnityEvent switchOnMethods;
	public UnityEvent switchOffMethods;

	[Header("Switcher Options")]
	public bool blockOnSwitch = false;
	public GrabbableRotation grabbable;

	public void SwitchOn() {
		if (blockOnSwitch) {
			grabbable.FinishGrab ();
			grabbable.enabled = false;
		}
		switchOnMethods.Invoke();
	}

	public void SwitchOff() {
		if (blockOnSwitch)
			grabbable.enabled = false;
		switchOffMethods.Invoke();
	}
}
