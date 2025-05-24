using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionWidget : MonoBehaviour
{
	public static WeaponSelectionWidget instance;

	public Text ammoCountLabel;

	public Image weaponIcon;

	[SerializeField]
	private GameObject scroll;

	[SerializeField]
	private Transform scollContent;

	[SerializeField]
	private GameObject weaponSelectionButtonPrefab;

	private Coroutine crt;

	private List<GameObject> btns = new List<GameObject>();

	private void Awake()
	{
		instance = this;
	}

	public void SetAmmoCount(int count)
	{
		ammoCountLabel.text = count.ToString();
	}

	public void SetWeaponIcon(Sprite icon)
	{
		weaponIcon.sprite = icon;
	}

	private void OnEnable()
	{
		if (crt != null)
		{
			StopCoroutine(crt);
		}
		crt = StartCoroutine(ShowBoughtGunsList());
	}

	private IEnumerator ShowBoughtGunsList()
	{
		btns.ForEach(delegate(GameObject btn)
		{
			Object.Destroy(btn);
		});
		btns.Clear();
		while (GameController.instance == null || GameController.instance.OurPlayer == null)
		{
			yield return null;
		}
		int num = 0;
		foreach (BaseWeaponScript item in GameController.instance.OurPlayer.playerWeaponManager.AllPlayerGuns())
		{
			Button component = Object.Instantiate(weaponSelectionButtonPrefab).GetComponent<Button>();
			component.GetComponent<Image>().sprite = item.myShopItem.icon;
			component.transform.SetParent(scollContent);
			component.transform.localScale = Vector3.one;
			if (MultiplayerController.gameType == GameMode.BattleRoyalePvP || MultiplayerController.gameType == GameMode.BattleRoyaleTeams)
			{
				if (item.isActiveBattleRoyale)
				{
					int gunid2 = num;
					component.onClick.AddListener(delegate
					{
						GameController.instance.OurPlayer.SelectWeapon(gunid2);
						scroll.SetActive(value: false);
						ShowMyBtn(show: true);
					});
				}
				else
				{
					component.interactable = false;
					component.gameObject.SetActive(value: false);
				}
			}
			else if (item.myShopItem.IsBought)
			{
				int gunid3 = num;
				component.onClick.AddListener(delegate
				{
					GameController.instance.OurPlayer.SelectWeapon(gunid3);
					scroll.SetActive(value: false);
					ShowMyBtn(show: true);
				});
			}
			else
			{
				component.interactable = false;
				component.gameObject.SetActive(value: false);
			}
			num++;
			btns.Add(component.gameObject);
		}
	}

	public void OnClickMe()
	{
		scroll.SetActive(value: true);
		ShowMyBtn(show: false);
	}

	private void ShowMyBtn(bool show)
	{
		GetComponent<Image>().enabled = show;
		weaponIcon.gameObject.SetActive(show);
	}

	public void CloseWidget()
	{
		ammoCountLabel.gameObject.SetActive(value: true);
		scroll.SetActive(value: false);
		ShowMyBtn(show: true);
	}

	public void UpdateGunsList()
	{
		if (crt != null)
		{
			StopCoroutine(crt);
		}
		crt = StartCoroutine(ShowBoughtGunsList());
	}
}
