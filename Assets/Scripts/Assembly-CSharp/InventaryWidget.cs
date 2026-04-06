using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventaryWidget : MonoBehaviour
{
	public RectTransform categoriesTabsContainer;

	public RectTransform itemsContainer;

	public GameObject categoryButtonPrefab;

	public GameObject inventaryItenButtonPrefab;

	public RectTransform window;

	public GameObject mainCallBtn;

	[SerializeField]
	private GameObject loadingLockPanel;

	private List<InventaryItemButton> items = new List<InventaryItemButton>();

	private void Start()
	{
		CteateCategories();
		CloseWidget();
	}

	private void CteateCategories()
	{
		InventoryCategory[] inventoryCategories = DataModel.instance.InventoryCategories;
		foreach (InventoryCategory inventoryCategory in inventoryCategories)
		{
			if (ArenaScript.instance.IsCategoryAllowed(inventoryCategory.categoryType))
			{
				GameObject gameObject = Object.Instantiate(categoryButtonPrefab);
				gameObject.transform.SetParent(categoriesTabsContainer);
				gameObject.transform.localScale = Vector3.one;
				InventaryItemButton component = gameObject.GetComponent<InventaryItemButton>();
				component.SetIcon(inventoryCategory.icon);
				component.SetTitle(inventoryCategory.name);
				InventoryCategory bufCat = inventoryCategory;
				gameObject.GetComponent<Button>().onClick.AddListener(delegate
				{
					CreateItems(bufCat);
				});
			}
		}
		CreateItems(DataModel.instance.InventoryCategories[0]);
	}

	private void CreateItems(InventoryCategory category)
	{
		foreach (InventaryItemButton item in items)
		{
			item.GetComponent<Button>().onClick.RemoveAllListeners();
			Object.Destroy(item.gameObject);
		}
		items.Clear();
		foreach (InventoryObject item2 in category.items)
		{
			if (ArenaScript.instance.IsCategoryAllowed(item2.category))
			{
				GameObject gameObject = Object.Instantiate(inventaryItenButtonPrefab);
				gameObject.transform.SetParent(itemsContainer);
				gameObject.transform.localScale = Vector3.one;
				InventaryItemButton component = gameObject.GetComponent<InventaryItemButton>();
				component.SetIcon(item2.icon);
				component.SetTitle(item2.name);
				if (item2.gameObject.GetComponent<ShopItem>() != null && !item2.gameObject.GetComponent<ShopItem>().IsBought)
				{
					component.SetLocked();
				}
				InventoryObject bufCat = item2;
				gameObject.GetComponent<Button>().onClick.AddListener(delegate
				{
					OnItemClick(bufCat);
				});
				items.Add(component);
			}
		}
	}

	private void OnItemClick(InventoryObject item)
	{
		if (!(item.gameObject.GetComponent<ShopItem>() != null) || item.gameObject.GetComponent<ShopItem>().IsBought)
		{
			GameController.instance.OurPlayer.playerInventaryManager.CreateItem(item);
			if (item.category == InventoryCategoryType.Pets)
			{
				loadingLockPanel.SetActive(true);
				Invoke("HideLoadingLock", 3f);
				CloseWidget();
			}
			else
			{
				loadingLockPanel.SetActive(true);
				Invoke("HideLoadingLock", 0.9f);
			}
		}
	}

	private void HideLoadingLock()
	{
		loadingLockPanel.SetActive(false);
	}

	public void CloseWidget()
	{
		window.gameObject.SetActive(false);
	}

	public void Show()
	{
		window.gameObject.SetActive(!window.gameObject.activeSelf);
	}

	public void ShawAllInventoryWidgetUI(bool show)
	{
		mainCallBtn.SetActive(show);
		window.gameObject.SetActive(show);
	}

	public void ShawInventoryCallBtn(bool show)
	{
		mainCallBtn.SetActive(show);
	}
}
