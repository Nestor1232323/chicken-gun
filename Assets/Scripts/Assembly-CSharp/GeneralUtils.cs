using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

public class GeneralUtils : MonoBehaviour
{
	public static void SetLayerRecursively(GameObject go, int newLayer)
	{
		go.layer = newLayer;
		foreach (Transform item in go.transform)
		{
			SetLayerRecursively(item.gameObject, newLayer);
		}
	}

	public static void RemoveAllChilds(Transform parent)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Transform item in parent)
		{
			list.Add(item.gameObject);
		}
		list.ForEach(delegate(GameObject child)
		{
			UnityEngine.Object.Destroy(child);
		});
	}

	public static string ColorToHex(Color32 color)
	{
		return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
	}

	public static Color HexToColor(string hex)
	{
		byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
		return new Color32(r, g, b, byte.MaxValue);
	}

	public static Component CopyComponent(Component original, GameObject destination)
	{
		Type type = original.GetType();
		Component component = destination.AddComponent(type);
		FieldInfo[] fields = type.GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			fieldInfo.SetValue(component, fieldInfo.GetValue(original));
		}
		return component;
	}
}
