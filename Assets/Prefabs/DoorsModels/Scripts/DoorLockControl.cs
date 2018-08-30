using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule.Examples.Grabbables;

public class DoorLockControl : MonoBehaviour {

	[SerializeField]
	protected Door door;
	[SerializeField]
	protected InteractableTransferRotation lockInteractable;

	void Awake () {
		if (lockInteractable == null)
			lockInteractable = GetComponent<InteractableTransferRotation> ();
		
		if (door.isLocked ())
			lockInteractable.InteractionEnabled (false);

		lockInteractable.OnContactStateChange += CheckKeyID;
	}

	private void CheckKeyID (BaseInteractable baseInteract) {
		switch (baseInteract.ContactState) {

		case GrabStateEnum.Inactive:
		case GrabStateEnum.Multi:
			lockInteractable.InteractionEnabled (false);
			break;

		case GrabStateEnum.Single:
			InventoryItem item = baseInteract.InteractorContactPrimary.GetComponentInChildren<HandItems> ().GetActiveInventoryItem ();
			if (item) {
				Key key = item.GetComponent<Key> ();
				if (key && key.keyID == door.GetKeyId ()) {
					lockInteractable.InteractionEnabled (true);
					return;
				}
			}
			lockInteractable.InteractionEnabled (false);
			break;
		}
	}
}
