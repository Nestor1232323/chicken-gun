using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponSlot
{
	[SerializeField]
	public List<Renderer> models = new List<Renderer>();
}
