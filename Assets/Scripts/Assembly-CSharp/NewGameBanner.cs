using System;
using System.Collections;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class NewGameBanner : MonoBehaviour
{
	private string gameId;

	private string gameUrl;

	[SerializeField]
	private Image previewImage;

	[SerializeField]
	private Text gameTitle;

	[SerializeField]
	private GameObject window;

	private bool HasTried
	{
		get
		{
			return PlayerPrefs.GetInt("HasTried" + gameId) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("HasTried" + gameId, 1);
		}
	}

	private void Awake()
	{
		window.SetActive(value: false);
		NetworkManager.ConfigsLoaded = (Action<string>)Delegate.Combine(NetworkManager.ConfigsLoaded, new Action<string>(OnConfigsLoaded));
	}

	private void OnDestroy()
	{
		NetworkManager.ConfigsLoaded = (Action<string>)Delegate.Remove(NetworkManager.ConfigsLoaded, new Action<string>(OnConfigsLoaded));
	}

	private void OnConfigsLoaded(string jsonAppData)
	{
		if (!ScreenManager.instance.IsInMenuScene())
		{
			return;
		}
		JSONNode jSONNode = JSON.Parse(jsonAppData);
		if (jSONNode != null)
		{
			jSONNode = jSONNode["NewGameBanner"];
			if (jSONNode == null)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			if (!jSONNode["IsActive"].AsBool)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			gameId = jSONNode["Id"].Value;
			if (Application.identifier.Equals(gameId))
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			if (HasTried)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			gameTitle.text = jSONNode["gameTitle"].Value;
			gameUrl = jSONNode["AndroidUrl"].Value;
			if (gameUrl == string.Empty)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			window.SetActive(value: true);
			string value = jSONNode["Image"].Value;
			StartCoroutine(LoadPreviewImg(value));
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator LoadPreviewImg(string imgUrl)
	{
		WWW www = new WWW(imgUrl);
		yield return www;
		Sprite sprite = Sprite.Create(www.texture, new Rect(0f, 0f, www.texture.width, www.texture.height), new Vector2(0f, 0f), 100f);
		previewImage.type = Image.Type.Simple;
		previewImage.sprite = sprite;
	}

	public void OnTryBtn()
	{
		HasTried = true;
		base.gameObject.SetActive(value: false);
		Application.OpenURL(gameUrl);
	}

	public void OnHideBtn()
	{
		HasTried = true;
		base.gameObject.SetActive(value: false);
	}
}
