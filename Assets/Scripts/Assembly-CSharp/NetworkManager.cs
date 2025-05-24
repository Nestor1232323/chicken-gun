using System;
using System.Collections;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
	[SerializeField]
	private string configsUrl;

	public static Action<string> ConfigsLoaded;

	private IEnumerator Start()
	{
		WWW www = new WWW(configsUrl);
		yield return www;
		if (string.IsNullOrEmpty(www.error) && ConfigsLoaded != null)
		{
			ConfigsLoaded(www.text);
		}
	}
}
