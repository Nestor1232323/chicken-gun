using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryCategory
{
	public byte id;

	public InventoryCategoryType categoryType;

	public Sprite icon;

	public string name;

	public List<InventoryObject> items;
}
