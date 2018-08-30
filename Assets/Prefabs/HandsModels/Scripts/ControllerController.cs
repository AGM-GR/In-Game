using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerController : MonoBehaviour {

	[SerializeField]
	protected Transform controller;

	[Header("Controller Rotation")]
	public float rotationSpeed = 2f;
	public Vector3 rotationObjetive = Vector3.zero;

	[Header("Controller Colors")]
	public float minAlpha = 0.4f;

	public Color outlineIndicatorColor = Color.white;
	public Color outlineMenuColor = Color.blue;

	private Quaternion initialRotation;
	private Quaternion actionRotation;
	private Outline outline;
	private Renderer controllerRenderer;
	private Color controllerColor;
	private Color controllerIndicatorColor;

	private bool rotate = false;
	private float rotationStep = 0f;
	private Quaternion fromRotation;
	private Quaternion toRotation;

	void Awake () {
		initialRotation = controller.localRotation;
		actionRotation = initialRotation;
		actionRotation.eulerAngles = new Vector3 (actionRotation.eulerAngles.x + rotationObjetive.x, actionRotation.eulerAngles.y + rotationObjetive.y, actionRotation.eulerAngles.z + rotationObjetive.z);
		outline = controller.GetComponent<Outline> ();
		controllerRenderer = controller.GetComponent<Renderer> ();
		controllerColor = controllerRenderer.material.color;
		controllerIndicatorColor = controllerColor;
		controllerIndicatorColor.a = minAlpha;
	}

	void Start () {
		outline.enabled = false;
	}

	void Update () {
		if (rotate) {
			rotationStep += Time.deltaTime * rotationSpeed;
			if (rotationStep >= 1)
				rotationStep = 1;
			controller.localRotation = Quaternion.Lerp (fromRotation, toRotation, rotationStep);
			if (rotationStep == 1)
				rotate = false;
		}
	}
		
	public void ActiveControllerIndicator () {
		controllerRenderer.material.color = controllerIndicatorColor;
		outline.enabled = true;
		outline.OutlineColor = outlineIndicatorColor;
		rotationStep = 0f;
		fromRotation = controller.localRotation;
		toRotation = actionRotation;
		rotate = true;
	}

	public void ActiveControllerMenu () {
		outline.enabled = true;
		outline.OutlineColor = outlineMenuColor;
		rotationStep = 0f;
		fromRotation = controller.localRotation;
		toRotation = actionRotation;
		rotate = true;
	}

	public void DeactivateControllerAction () {
		controllerRenderer.material.color = controllerColor;
		outline.enabled = false;
		rotationStep = 0f;
		fromRotation = controller.localRotation;
		toRotation = initialRotation;
		rotate = true;
	}

	public Transform GetControllerTransform () {
		return controller;
	}
}
