using System;
using UnityEngine;

public class ModuleList : ScriptableObject
{
	[Serializable]
	public class characterModule
	{
		public GameObject parts;

		public Texture2D[] texCol;
	}

	public characterModule[] head;

	public characterModule[] outfitUp;

	public characterModule[] outfitDown;

	public characterModule[] bag;
}
