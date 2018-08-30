using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

public class GrabbableToInventory : BaseGrabbable {

	[Header("Inventory Management")]
	public Inventory inventory;
	[SerializeField]
	protected InventoryItem inventoryItem = null;
	[SerializeField]
	protected HandActionsEnum onGrabAnimation = HandActionsEnum.GRAB;
	[SerializeField]
	protected float transitionSpeed = 2f;
	[SerializeField]
	protected AnimationCurve transitionCurve = null;

	void Awake () {
		this.OnGrabStateChange += GrabItem;

        if (inventory == null)
            if(GameObject.FindGameObjectWithTag("Inventory"))
                inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();

		if (inventoryItem == null)
			inventoryItem = GetComponent<InventoryItem> ();
	}

	private void GrabItem (BaseGrabbable baseGrab) {

		if (baseGrab.GrabState != GrabStateEnum.Inactive) {

			foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody> ())
				rb.isKinematic = true;

			foreach (Collider collider in GetComponents<Collider> ())
				collider.enabled = false;

			((Grabber)GrabberPrimary).FinishGrab ();
			((Grabber)GrabberPrimary).FinishContact (this);

			ColorOutliner colorOutliner = GetComponent<ColorOutliner> ();
			if (colorOutliner) {
				colorOutliner.SetInteraction (false);
				colorOutliner.ActivateOutline (Color.white);
				colorOutliner.enabled = false;
			}

			StartCoroutine(MoveToGrabber(baseGrab.GrabberPrimary));
		}
	}


	private IEnumerator MoveToGrabber(BaseGrabber grabber) {
		Vector3 initialPosition = transform.position;
		Vector3 initialScale = transform.lossyScale;
		Vector3 targetScale = Vector3.zero;

		HandAnimationsController handAnim = grabber.GetComponent<HandAnimationsController> ();
		if (handAnim)
			handAnim.SetAnimation (onGrabAnimation);

		float lerpStep = 0f;
		while (lerpStep < 1f) {
			lerpStep += Time.fixedDeltaTime * transitionSpeed;
			transform.localPosition = Vector3.Lerp(initialPosition, grabber.GrabHandle.position, transitionCurve.Evaluate(lerpStep));
			transform.localScale = Vector3.Lerp(initialScale, targetScale, transitionCurve.Evaluate(lerpStep));
			yield return new WaitForEndOfFrame ();
		}

		transform.localPosition = grabber.GrabHandle.position;
		transform.localScale = targetScale;

		if (handAnim)
			handAnim.SetUndoAnimation (onGrabAnimation);

		gameObject.layer = LayerMask.NameToLayer ("Inventory");

		foreach (Collider collider in GetComponents<Collider> ())
			if (collider.isTrigger)
				collider.enabled = true;

		inventory.AddItemElement (inventoryItem);
		Destroy (this);

		yield break;
	}
}
