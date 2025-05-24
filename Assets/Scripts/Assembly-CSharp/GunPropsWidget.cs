using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunPropsWidget : MonoBehaviour
{
	[SerializeField]
	private Text gunTitle;

	[SerializeField]
	private Text currentLevel;

	[SerializeField]
	private Text damage;

	[SerializeField]
	private Text firerate;

	[SerializeField]
	private Text accurancy;

	[SerializeField]
	private Text upgPrice;

	[SerializeField]
	private Button upgButton;

	[SerializeField]
	private Text buttonText;

	[SerializeField]
	private Transform PreviewGunsContainer;

	private List<GameObject> previewGuns;

	private BaseWeaponScript currentGun;

	private void Awake()
	{
		previewGuns = new List<GameObject>();
		foreach (Transform item in PreviewGunsContainer.transform)
		{
			previewGuns.Add(item.gameObject);
		}
	}

	private void Start()
	{
		upgButton.interactable = false;
		base.gameObject.SetActive(value: false);
	}

	public void SetGun(BaseWeaponScript.GunInfo gun)
	{
		Show(show: true);
		currentLevel.text = $"{gun.currentLevel + 1}/{gun.maxUpgradeLevel}";
		damage.text = gun.damagePerSec.ToString();
		accurancy.text = gun.accurancy.ToString();
		previewGuns.ForEach(delegate(GameObject item)
		{
			item.SetActive(item.name == gun.myShopItem.id);
		});
	}

	public void OnUpgradeBtnClick()
	{
	}

	public void Show(bool show)
	{
		base.gameObject.SetActive(show);
		PreviewGunsContainer.gameObject.SetActive(show);
	}
}
