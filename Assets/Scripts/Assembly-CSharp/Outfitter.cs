using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Outfitter : MonoBehaviour
{
	private CharacterDemoController ac;

	private int oldWeaponIndex;

	[SerializeField]
	public List<WeaponSlot> weapons;

	private void Start()
	{
		ac = GetComponentInChildren<CharacterDemoController>();
		for (int i = 0; i < weapons.Count; i++)
		{
			for (int j = 0; j < weapons[i].models.Count; j++)
			{
				weapons[i].models[j].enabled = false;
			}
		}
		for (int k = 0; k < weapons[ac.WeaponState].models.Count; k++)
		{
			weapons[ac.WeaponState].models[k].enabled = true;
		}
		oldWeaponIndex = ac.WeaponState;
	}

	private void Update()
	{
		if (ac.WeaponState != oldWeaponIndex)
		{
			for (int i = 0; i < weapons[oldWeaponIndex].models.Count; i++)
			{
				weapons[oldWeaponIndex].models[i].enabled = false;
			}
			for (int j = 0; j < weapons[ac.WeaponState].models.Count; j++)
			{
				weapons[ac.WeaponState].models[j].enabled = true;
			}
			oldWeaponIndex = ac.WeaponState;
		}
	}
}
