using System;
using UnityEngine;

[Serializable]
public class LanguageData
{
	[SerializeField]
	private string _name;

	public string Name => _name;
}
