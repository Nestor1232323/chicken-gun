using System;
using SimpleJSON;
using UnityEngine;

public class NewVersionChecker : MonoBehaviour
{
	private void Awake()
	{
		NetworkManager.ConfigsLoaded = (Action<string>)Delegate.Combine(NetworkManager.ConfigsLoaded, new Action<string>(OnConfigsLoaded));
	}

	private void OnDestroy()
	{
		NetworkManager.ConfigsLoaded = (Action<string>)Delegate.Remove(NetworkManager.ConfigsLoaded, new Action<string>(OnConfigsLoaded));
	}

	private void OnConfigsLoaded(string jsonAppData)
	{
		if (!ScreenManager.instance.IsINMainMenu())
		{
			return;
		}
		JSONNode jSONNode = JSON.Parse(jsonAppData);
		int num = -1;
		if (jSONNode != null)
		{
			JSONNode jSONNode2 = jSONNode["Apps"];
			if (jSONNode2 == null)
			{
				return;
			}
			foreach (JSONNode item in jSONNode2.AsArray)
			{
				if (Application.identifier.Equals(item["Id"].Value))
				{
					if (DataModel.isAndroid)
					{
						num = item["v_and"].AsInt;
					}
					else if (DataModel.isIOS)
					{
						num = item["v_ios"].AsInt;
					}
					if (!(jSONNode["AdsPeriod"] != null))
					{
					}
					break;
				}
			}
		}
		if (num > MultiplayerController.instance.PhotonGameVersion)
		{
			base.transform.GetChild(0).gameObject.SetActive(value: true);
		}
	}

	public void UpdateBtnClick()
	{
		if (DataModel.isAndroid)
		{
			Application.OpenURL(DataModel.instance.gameUrlAndroid);
		}
		else if (DataModel.isIOS)
		{
			Application.OpenURL(DataModel.instance.gameUrlIOS);
		}
		else
		{
			Application.OpenURL(DataModel.instance.gameUrlAndroid);
		}
	}

	public void NoThanksBtn()
	{
		base.transform.GetChild(0).gameObject.SetActive(value: false);
	}
}
