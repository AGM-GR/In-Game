﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.UX;

public class Inventory : MonoBehaviour {

	public InventorySelector inventorySelector;
	public LineObjectCollection itemsCollection;

	private int itemSize = 0;

	void Awake () {
		itemSize = itemsCollection.Objects.Count;
	}

	public void AddItemElement (InventoryItem newItem) {
		newItem.transform.SetParent (transform);

		itemsCollection.Objects.Add (newItem.transform);
		itemSize = itemsCollection.Objects.Count;

		int newItemIdex = ((int)itemSize / 2) - 1;
		if (newItemIdex < 0)
			newItemIdex = 0;
		
		inventorySelector.SetActiveItemindex (newItemIdex);
	}

	public void RemoveItemElement (InventoryItem item) {

		itemsCollection.Objects.Remove (item.transform);
		itemSize = itemsCollection.Objects.Count;

		int newItemIdex = ((int)itemSize / 2) - 1;
		if (newItemIdex < 0)
			newItemIdex = 0;

		inventorySelector.SetActiveItemindex (newItemIdex);
	}

    public void ClearItems() {
        InventoryItem noneItem = null;
        for (int i = transform.childCount - 1; i >= 0; i--) {
            if (transform.GetChild(i).name == "NoneItem")
                noneItem = transform.GetChild(i).GetComponent<InventoryItem>();
            else {
                RemoveItemElement(transform.GetChild(i).GetComponent<InventoryItem>());
                Destroy(transform.GetChild(i));
            }
        }

        itemsCollection.Objects.Clear();
        if (noneItem)
            AddItemElement(noneItem);
    }
}
