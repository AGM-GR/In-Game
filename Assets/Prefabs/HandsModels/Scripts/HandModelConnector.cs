using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandModelConnector : MonoBehaviour {

	[SerializeField]
	protected Transform hand;
	[SerializeField]
	protected Transform handModel;

	public float changeSpeed = 1f;
	public float scaleSpeed = 1f;

	private ControllerController controller;

	private Vector3 handLocalPosition;
	private Vector3 handLocalScale;
	private Quaternion handLocalRotation;

	private Vector3 handModelLocalPosition;
	private Vector3 handModelLocalScale;
	private Quaternion handModelLocalRotation;

	private Vector3 initialPosition;
	private Vector3 destPosition;
	private Vector3 initialScale;
	private Vector3 destScale;
	private Quaternion initialRotation;
	private Quaternion destRotation;

	private bool changeParent = false;
	private float changeStep = 0f;

	private bool changeScale = false;
	private float scaleStep = 0f;

	void Awake () {
		controller = GetComponent<ControllerController> ();
		handLocalPosition = hand.localPosition;
		handLocalScale = hand.localScale;
		handLocalRotation = hand.localRotation;
		handModelLocalPosition = handModel.localPosition;
		handModelLocalScale = handModel.localScale;
		handModelLocalRotation = handModel.localRotation;
	}

	void Update () {
		if (changeParent) {
			changeStep += Time.deltaTime * changeSpeed;
			if (changeStep >= 1)
				changeStep = 1;
			hand.localRotation = Quaternion.Lerp (initialRotation, destRotation, changeStep);
			hand.localPosition = Vector3.Lerp (initialPosition, destPosition, changeStep);
			if (changeStep == 1)
				changeParent = false;
		}

		if (changeScale) {
			scaleStep += Time.deltaTime * scaleSpeed;
			if (scaleStep >= 1)
				scaleStep = 1;
			handModel.localScale = Vector3.Lerp (initialScale, destScale, scaleStep);
			if (scaleStep == 1)
				changeScale = false;
		}
	}

	public Transform SetHandModelParent (Transform newParent, Vector3 newPosition, Vector3 newRotation, Vector3 newScale) {
		controller.ActiveControllerIndicator ();
		hand.SetParent (newParent);
		hand.localScale = newScale;
		initialPosition = hand.localPosition;
		initialRotation = hand.localRotation;
		destPosition = newPosition;
		destRotation = Quaternion.Euler(newRotation);
		changeStep = 0f;
		changeParent = true;

		return hand;
	}

	public void ReconectHandModel () {
		controller.DeactivateControllerAction ();
		hand.SetParent (transform);
		hand.localScale = handLocalScale;
		initialPosition = hand.localPosition;
		initialRotation = hand.localRotation;
		destPosition = handLocalPosition;
		destRotation = handLocalRotation;
		changeStep = 0f;
		changeParent = true;
	}

	public bool IsChangingParent () {
		return changeParent;
	}

	public void HideHandModel () {
		initialScale = handModel.localScale;
		destScale = Vector3.zero;
		scaleStep = 0f;
		changeScale = true;
	}

	public void ShowHandModel () {
		initialScale = handModel.localScale;
		destScale = handModelLocalScale;
		scaleStep = 0f;
		changeScale = true;
	}

	public bool IsHidingOrShowing () {
		return changeScale;
	}
}
