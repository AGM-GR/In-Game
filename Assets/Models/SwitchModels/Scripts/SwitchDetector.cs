using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchDetector : MonoBehaviour {

	public Switch switchObject;
	public GameObject switcherCollider;

	public float flashDuration = 0.05f;

	private GameObject sparks;
	private Light flashLight;

	void Awake () {
		if (GetComponentInChildren<ParticleSystem> ())
			sparks = GetComponentInChildren<ParticleSystem> ().gameObject;
		flashLight = GetComponentInChildren<Light> ();

		sparks.SetActive (false);
	}

	protected virtual void OnTriggerEnter(Collider collider) {

		if (collider.gameObject == switcherCollider)
			SwitchOn ();
	}

	protected virtual void OnTriggerExit(Collider collider){
		if (collider.gameObject == switcherCollider)
			SwitchOff ();
	}

	private void SwitchOn() {
		StartCoroutine (Flash());
		sparks.SetActive (true);
		switchObject.SwitchOn ();
	}

	private void SwitchOff() {
		sparks.SetActive (false);
		switchObject.SwitchOff ();
	}

	IEnumerator Flash() {
		flashLight.enabled = true;
		yield return new WaitForSeconds (flashDuration);
		flashLight.enabled = false;
	}
}
