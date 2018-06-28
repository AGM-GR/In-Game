using System;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

public class GrabbableColorOutliner : MonoBehaviour {

	[Header("Colors")]
	[SerializeField]
	private Color colorOnContactSingle = Color.blue;

	[SerializeField]
	private Color colorOnContactMulti = Color.cyan;

	[SerializeField]
	private Color colorOnGrabSingle = Color.yellow;

	[SerializeField]
	private Color colorOnGrabMulti = Color.red;

	[Header("Outline")]
	[SerializeField]
	private Outline.Mode outlineMode = Outline.Mode.OutlineAndSilhouette;

	[SerializeField, Range(0f, 10f)]
	private float outlineWidth = 2f;

	[Header("Objects")]
	[SerializeField]
	private Outline targetOutlineObject;

	[SerializeField]
	private bool outlineAllChildrens;

	[SerializeField]
	private BaseGrabbable grabbable;

	private Outline [] targetOutlineObjects;

	void Awake() {
		if (outlineAllChildrens)
			targetOutlineObjects = GetComponentsInChildren<Outline> ();
		else if (targetOutlineObject == null)
			targetOutlineObject = GetComponentInChildren<Outline> ();

		if (grabbable == null)
			grabbable = GetComponent<BaseGrabbable> ();

		//Subscribe los eventos al tocar un objeto y agarrarlo
		grabbable.OnContactStateChange += RefreshColor;
		grabbable.OnGrabStateChange += RefreshColor;
	}

	void Start() {
		
		DeactivateOutline ();
	}

	private void RefreshColor(BaseGrabbable baseGrab) {

		switch (baseGrab.ContactState){
		case GrabStateEnum.Inactive:
			DeactivateOutline ();
			break;

		case GrabStateEnum.Multi:
			ActivateOutline(colorOnContactMulti);
			break;

		case GrabStateEnum.Single:
			ActivateOutline(colorOnContactSingle);
			break;

		default:
			throw new ArgumentOutOfRangeException();
		}

		switch (baseGrab.GrabState) {
		case GrabStateEnum.Inactive:
			break;

		case GrabStateEnum.Multi:
			ActivateOutline(colorOnGrabMulti);
			break;

		case GrabStateEnum.Single:
			ActivateOutline(colorOnGrabSingle);
			break;

		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void DeactivateOutline() {
		
		if (outlineAllChildrens)
			foreach (Outline outline in targetOutlineObjects)
				outline.enabled = false;
		else if (targetOutlineObject != null)
			targetOutlineObject.enabled = false;
	}

	private void ActivateOutline(Color color) {
		
		if (outlineAllChildrens) {
			foreach (Outline outline in targetOutlineObjects) {
				outline.enabled = true;
				outline.OutlineColor = color;
				outline.OutlineMode = outlineMode;
				outline.OutlineWidth = outlineWidth;
			}
		} else if (targetOutlineObject != null) {
			targetOutlineObject.enabled = true;
			targetOutlineObject.OutlineColor = color;
			targetOutlineObject.OutlineMode = outlineMode;
			targetOutlineObject.OutlineWidth = outlineWidth;
		}
	}
}
