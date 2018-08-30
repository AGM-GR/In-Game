using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandItems : MonoBehaviour {
	
	protected Inventory inventory;

	void Awake () {
		inventory = GameObject.FindObjectOfType<Inventory> ();
	}

	public InventoryItem GetActiveInventoryItem () {
		InventorySelector inventory = GetComponentInChildren<InventorySelector> ();
		if (inventory)
			return inventory.GetActiveItem ();
		return null;
	}

	public Transform GetDetachedActiveInventoryItem () {
		InventoryItem item = GetActiveInventoryItem ();
		if (item) {
			inventory.RemoveItemElement (item);
			return item.transform;
		}
		return null;
	}

}
