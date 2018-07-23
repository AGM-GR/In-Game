using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour {

	public enum DisplayModeEnum {
		InMenu,
		InMenuSelected,
		InRightHand,
		InLeftHand,
		Hidden
	}

	public DisplayModeEnum DisplayMode {
		set { displayMode = value; }
	}

	public HandActionsEnum HandAction {
		get { return inHandAction; }
	}

	[Header("Item")]
	[SerializeField]
	private GameObject inventoryItem = null;
	private Transform itemTransform;

	[Header("Mode Settings")]
	[SerializeField]
	private Vector3 inMenuPosition = Vector3.zero;
	[SerializeField]
	private Vector3 inMenuRotation = Vector3.zero;
	[SerializeField]
	private Vector3 inMenuSelectedPosition = Vector3.zero;
	[SerializeField]
	private Vector3 inMenuSelectedRotation = Vector3.zero;
	[SerializeField]
	private Vector3 inRightHandPosition = Vector3.zero;
	[SerializeField]
	private Vector3 inRightHandRotation = Vector3.zero;
	[SerializeField]
	private Vector3 inLeftHandPosition = Vector3.zero;
	[SerializeField]
	private Vector3 inLeftHandRotation = Vector3.zero;
	[SerializeField]
	private HandActionsEnum inHandAction = HandActionsEnum.GRABKEY;
	[SerializeField]
	private DisplayModeEnum displayMode = DisplayModeEnum.InMenu;
	[SerializeField]
	private float transitionDuration = 0.5f;
	[SerializeField]
	private AnimationCurve transitionCurve = null;

	private void Awake ()  {
		itemTransform = inventoryItem.transform;
	}

	private void OnEnable() {
		StartCoroutine(UpdateDisplayMode());
	}

	private IEnumerator UpdateDisplayMode() {
		// Variables we'll be using
		Vector3 targetPosition = inMenuPosition;
		Vector3 targetScale = Vector3.one;
		Quaternion targetRotation = Quaternion.Euler(inMenuRotation);
		Vector3 startPosition = targetPosition;
		Vector3 startScale = targetScale;
		Quaternion startRotation = targetRotation;

		// Reset before starting
		displayMode = DisplayModeEnum.InMenu;
		DisplayModeEnum lastDisplayMode = displayMode;
		itemTransform.localPosition = targetPosition;
		itemTransform.localRotation = targetRotation;

		while (isActiveAndEnabled){
			// Wait for displayMode to change
			while (displayMode == lastDisplayMode) {
				yield return null;
			}

			startPosition = itemTransform.localPosition;
			startRotation = itemTransform.localRotation;
			startScale = itemTransform.localScale;
			lastDisplayMode = displayMode;
			switch (displayMode) {
			case DisplayModeEnum.InRightHand:
				targetPosition = inRightHandPosition;
				targetScale = Vector3.one;
				targetRotation = Quaternion.Euler(inRightHandRotation);
				itemTransform.gameObject.SetActive(true);
				break;
			
			case DisplayModeEnum.InLeftHand:
				targetPosition = inLeftHandPosition;
				targetScale = Vector3.one;
				targetRotation = Quaternion.Euler(inLeftHandRotation);
				itemTransform.gameObject.SetActive(true);
				break;

			case DisplayModeEnum.InMenu:
				targetPosition = inMenuPosition;
				targetScale = Vector3.one;
				targetRotation = Quaternion.Euler(inMenuRotation);
				itemTransform.gameObject.SetActive(true);
				break;

			case DisplayModeEnum.InMenuSelected:
				targetPosition = inMenuSelectedPosition;
				targetScale = Vector3.one;
				targetRotation = Quaternion.Euler(inMenuSelectedRotation);
				itemTransform.gameObject.SetActive(true);
				break;

			case DisplayModeEnum.Hidden:
				targetPosition = inMenuPosition;
				targetRotation = Quaternion.Euler(inMenuRotation);
				targetScale = startScale * 0.01f;
				break;
			}

			// Keep going until we're done transitioning, or until the mode changes, whichever comes first
			float startTime = Time.unscaledTime;
			while ((Time.unscaledTime < startTime + transitionDuration) && lastDisplayMode == displayMode) {
				float normalizedTime = (Time.unscaledTime - startTime) / transitionDuration;
				itemTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, transitionCurve.Evaluate(normalizedTime));
				itemTransform.localScale = Vector3.Lerp(startScale, targetScale, transitionCurve.Evaluate(normalizedTime));
				itemTransform.localRotation = Quaternion.Lerp(startRotation, targetRotation, transitionCurve.Evaluate(normalizedTime));
				yield return null;
			}
			itemTransform.localPosition = targetPosition;
			itemTransform.localScale = targetScale;
			itemTransform.localRotation = targetRotation;

			if (displayMode == DisplayModeEnum.Hidden) {
				itemTransform.gameObject.SetActive(false);
			}

			yield return null;
		}
		yield break;
	}
}
