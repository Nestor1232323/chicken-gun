using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
	public static ShopController instance;

	private ProductMode productMode;

	public Transform scrollViewContent;

	public GameObject ShopItemButtonPrefab;

	public List<Button> shopItemButtons = new List<Button>();

	public PlayerShopPreview playerShopPreview;

	public Button BuyButton;

	public Button ApplyButton;

	private ShopItem lastSelectedShopItem;

	public Text BalanceLabel;

	public Text PriceLabel;

	public Text ApplyBtnLabel;

	public GameObject NotEnoughMoneyPanel;

	[SerializeField]
	private Transform menuPlayerPivot;

	public Transform shopCarPivot;

	[SerializeField]
	private Transform shopBogyguardPivot;

	[SerializeField]
	private Transform shopPetPivot;

	private GameObject shopCarHolder;

	[SerializeField]
	private Transform mainCamPosForGuardMode;

	[SerializeField]
	private Transform mainCamPosForVehicleMode;

	[SerializeField]
	private Transform mainCamPosForGeneralMode;

	[SerializeField]
	private GameObject carsCamera;

	[SerializeField]
	private GunPropsWidget gunPropsWidget;

	[SerializeField]
	private PlayerTextureWidget playerTextureWidget;

	[SerializeField]
	private Button buyBtn;

	[SerializeField]
	private GameObject goToBankBtn;

	private List<ShopItem> tempList = new List<ShopItem>();

	private GameObject guardHolder;

	private void Awake()
	{
		instance = this;
		_ = DataModel.instance == null;
	}

	private void Start()
	{
		LocalStore.CurrencyBalanceChanged = (Action<int>)Delegate.Combine(LocalStore.CurrencyBalanceChanged, new Action<int>(OnCurrencyBalanceChanged));
		productMode = ProductMode.WeaponsCat1;
		BuyButton.gameObject.SetActive(value: false);
		ApplyButton.gameObject.SetActive(value: false);
		if (DataModel.isIOS)
		{
			buyBtn.enabled = false;
			goToBankBtn.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		LocalStore.CurrencyBalanceChanged = (Action<int>)Delegate.Remove(LocalStore.CurrencyBalanceChanged, new Action<int>(OnCurrencyBalanceChanged));
		instance = null;
	}

	public void OnShow()
	{
		BuyButton.gameObject.SetActive(value: false);
		ApplyButton.gameObject.SetActive(value: false);
		UpdateScrollViewContent();
		ApplyLastBoughtParams();
	}

	public void OnHide()
	{
		gunPropsWidget.Show(show: false);
	}

	public void CreateMenuPlayer()
	{
		if (!playerShopPreview)
		{
			string selectedPlayer = StorageController.instance.SelectedPlayer;
			GameObject gameObject = UnityEngine.Object.Instantiate(DataModel.instance.GetPlayerId(selectedPlayer));
			gameObject.transform.position = menuPlayerPivot.position;
			gameObject.transform.rotation = menuPlayerPivot.rotation;
			UnityEngine.Object.Destroy(gameObject.GetComponent<CharacterMotor>());
			playerShopPreview = gameObject.GetComponent<PlayerShopPreview>();
			gameObject.GetComponent<PlayerWeaponManager>().EquipSelectedGun();
			gameObject.GetComponent<CharacterAnimation>().enabled = false;
			playerShopPreview.transform.SetParent(menuPlayerPivot);
			gameObject.GetComponent<CharacterAnimation>().SetSpeedK(0f);
			gameObject.GetComponent<CharacterAnimation>().PlayMenuState();
			BoneRotation[] componentsInChildren = gameObject.GetComponentsInChildren<BoneRotation>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			playerTextureWidget.SetPlayer(playerShopPreview.gameObject);
			ApplyLastBoughtParams();
		}
	}

	private void ReplaceMenuPlayer(ShopItem newPlayer)
	{
		UnityEngine.Object.Destroy(playerShopPreview.gameObject);
		GameObject gameObject = UnityEngine.Object.Instantiate(DataModel.instance.GetPlayerId(newPlayer.id));
		gameObject.transform.position = menuPlayerPivot.position;
		gameObject.transform.rotation = menuPlayerPivot.rotation;
		UnityEngine.Object.Destroy(gameObject.GetComponent<CharacterMotor>());
		playerShopPreview = gameObject.GetComponent<PlayerShopPreview>();
		gameObject.GetComponent<PlayerWeaponManager>().EquipSelectedGun();
		gameObject.GetComponent<CharacterAnimation>().enabled = false;
		playerShopPreview.transform.SetParent(menuPlayerPivot);
		gameObject.GetComponent<CharacterAnimation>().SetSpeedK(0.5f);
		gameObject.GetComponent<CharacterAnimation>().PlayMenuState();
		gameObject.transform.localPosition = Vector3.zero;
		BoneRotation[] componentsInChildren = gameObject.GetComponentsInChildren<BoneRotation>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		playerTextureWidget.SetPlayer(playerShopPreview.gameObject);
	}

	public void UpdateAfterIAP()
	{
		BalanceLabel.text = LocalStore.currencyBalance.ToString();
		if (lastSelectedShopItem != null)
		{
			if (lastSelectedShopItem.IsBought)
			{
				BuyButton.gameObject.SetActive(value: false);
				ApplyButton.gameObject.SetActive(value: true);
			}
			else
			{
				BuyButton.gameObject.SetActive(value: true);
				ApplyButton.gameObject.SetActive(value: false);
			}
			ApplyBought();
		}
	}

	public void OnBtnChangeMode(int mode)
	{
		productMode = (ProductMode)mode;
		UpdateScrollViewContent();
		ApplyLastBoughtParams();
		if (productMode != ProductMode.Vehicles)
		{
			DestroyShopCar();
			playerShopPreview.gameObject.SetActive(value: true);
		}
		if (productMode != ProductMode.Bodyguards)
		{
			DestroyShopGuard();
		}
		UpdateMainCamPosForThisProductType();
		if (productMode == ProductMode.Vehicles || productMode == ProductMode.OtherVehicles || productMode == ProductMode.Horses || productMode == ProductMode.Pets)
		{
			OnShopItemButtonClick(tempList[0]);
			carsCamera.SetActive(value: true);
		}
		else
		{
			carsCamera.SetActive(value: false);
		}
		if (productMode == ProductMode.WeaponsCat1 || productMode == ProductMode.WeaponsCat2)
		{
			gunPropsWidget.SetGun(playerShopPreview.GetComponent<PlayerWeaponManager>().CurrentWeapon.gunInfo);
		}
		else
		{
			gunPropsWidget.Show(show: false);
		}
		if (productMode == ProductMode.Players)
		{
			playerTextureWidget.SetPlayer(playerShopPreview.gameObject);
		}
		else
		{
			playerTextureWidget.SetPlayer(null);
		}
	}

	private void ApplyLastBoughtParams()
	{
		playerShopPreview.GetComponent<PlayerWeaponManager>().EquipSelectedGun();
		playerShopPreview.playerClothingManager.WearEyes(StorageController.SelectedEyes);
		playerShopPreview.playerClothingManager.WearHat(StorageController.SelectedHat);
		playerShopPreview.playerClothingManager.WearSmile(StorageController.SelectedSmile);
		playerShopPreview.playerClothingManager.WearShoes(StorageController.SelectedShoes);
	}

	private void UpdateScrollViewContent()
	{
		foreach (Button shopItemButton in shopItemButtons)
		{
			UnityEngine.Object.Destroy(shopItemButton.gameObject);
		}
		shopItemButtons.Clear();
		tempList.Clear();
		switch (productMode)
		{
		case ProductMode.WeaponsCat1:
			foreach (BaseWeaponScript item in playerShopPreview.GetComponent<PlayerWeaponManager>().AllPlayerGuns())
			{
				if (item.weaponMenuCategory == WeaponMenuCategory.Cat1)
				{
					tempList.Add(item.GetComponent<BaseWeaponScript>().gunInfo.myShopItem);
				}
			}
			gunPropsWidget.SetGun(tempList[0].GetComponent<BaseWeaponScript>().gunInfo);
			break;
		case ProductMode.Hats:
			tempList = playerShopPreview.playerClothingManager.AllHats;
			break;
		case ProductMode.Eyes:
			tempList = playerShopPreview.playerClothingManager.AllEyes;
			break;
		case ProductMode.Smiles:
			tempList = playerShopPreview.playerClothingManager.AllBeaks;
			break;
		case ProductMode.Shoes:
			tempList = playerShopPreview.playerClothingManager.AllShoes;
			break;
		case ProductMode.WeaponsCat2:
		{
			GameObject[] bodyguards = DataModel.instance.Weapons;
			foreach (GameObject gameObject3 in bodyguards)
			{
				if (gameObject3.GetComponent<BaseWeaponScript>().weaponMenuCategory == WeaponMenuCategory.Cat2)
				{
					tempList.Add(gameObject3.GetComponent<ShopItem>());
				}
			}
			gunPropsWidget.SetGun(tempList[0].GetComponent<BaseWeaponScript>().gunInfo);
			break;
		}
		case ProductMode.Players:
		{
			GameObject[] bodyguards = DataModel.instance.Players;
			foreach (GameObject gameObject2 in bodyguards)
			{
				tempList.Add(gameObject2.GetComponent<ShopItem>());
			}
			break;
		}
		case ProductMode.Vehicles:
			foreach (GameObject vehicle in DataModel.instance.Vehicles)
			{
				tempList.Add(vehicle.GetComponent<ShopItem>());
			}
			break;
		case ProductMode.Pets:
			foreach (InventoryObject item2 in DataModel.instance.GetInvCategory(InventoryCategoryType.Pets).items)
			{
				ShopItem component3 = item2.GetComponent<ShopItem>();
				if (component3 != null)
				{
					tempList.Add(component3);
				}
			}
			break;
		case ProductMode.Horses:
			foreach (InventoryObject item3 in DataModel.instance.GetInvCategory(InventoryCategoryType.Horses).items)
			{
				ShopItem component2 = item3.GetComponent<ShopItem>();
				if (component2 != null)
				{
					tempList.Add(component2);
				}
			}
			break;
		case ProductMode.OtherVehicles:
			foreach (InventoryObject item4 in DataModel.instance.GetInvCategory(InventoryCategoryType.OtherVehicles).items)
			{
				ShopItem component = item4.GetComponent<ShopItem>();
				if (component != null)
				{
					tempList.Add(component);
				}
			}
			break;
		case ProductMode.Bodyguards:
		{
			GameObject[] bodyguards = DataModel.instance.Bodyguards;
			foreach (GameObject gameObject in bodyguards)
			{
				tempList.Add(gameObject.GetComponent<ShopItem>());
			}
			break;
		}
		}
		_ = ShopItemButtonPrefab.GetComponent<RectTransform>().sizeDelta;
		for (int j = 0; j < tempList.Count; j++)
		{
			GameObject obj = UnityEngine.Object.Instantiate(ShopItemButtonPrefab);
			Button component4 = obj.GetComponent<Button>();
			ShopItem shopItem = tempList[j];
			AddEventListener(component4, shopItem);
			obj.transform.SetParent(scrollViewContent);
			obj.transform.localScale = Vector3.one;
			obj.GetComponent<ShopItemButton>().SetData(tempList[j]);
			shopItemButtons.Add(component4);
		}
		BuyButton.gameObject.SetActive(value: false);
		ApplyButton.gameObject.SetActive(value: false);
		BalanceLabel.text = LocalStore.currencyBalance.ToString();
	}

	private void AddEventListener(Button b, ShopItem shopItem)
	{
		b.onClick.AddListener(delegate
		{
			OnShopItemButtonClick(shopItem);
		});
	}

	private void OnShopItemButtonClick(ShopItem shopItem)
	{
		lastSelectedShopItem = shopItem;
		if (productMode == ProductMode.Players)
		{
			ReplaceMenuPlayer(shopItem);
			playerTextureWidget.SetPlayer(playerShopPreview.gameObject);
		}
		else if (productMode == ProductMode.WeaponsCat1 || productMode == ProductMode.WeaponsCat2)
		{
			playerShopPreview.GetComponent<PlayerWeaponManager>().EquipGun(shopItem.id);
			gunPropsWidget.SetGun(shopItem.GetComponent<BaseWeaponScript>().gunInfo);
		}
		if (productMode == ProductMode.Hats)
		{
			playerShopPreview.playerClothingManager.WearHat(shopItem.id);
		}
		if (productMode == ProductMode.Shoes)
		{
			playerShopPreview.playerClothingManager.WearShoes(shopItem.id);
		}
		if (productMode == ProductMode.Eyes)
		{
			playerShopPreview.playerClothingManager.WearEyes(shopItem.id);
		}
		if (productMode == ProductMode.Smiles)
		{
			playerShopPreview.playerClothingManager.WearSmile(shopItem.id);
		}
		else if (productMode == ProductMode.Vehicles)
		{
			StartCoroutine(CreateShopCar(shopItem));
		}
		else if (productMode == ProductMode.Pets)
		{
			StartCoroutine(CreateShopPet(shopItem, InventoryCategoryType.Pets));
		}
		else if (productMode == ProductMode.Horses)
		{
			StartCoroutine(CreateShopPet(shopItem, InventoryCategoryType.Horses));
		}
		else if (productMode == ProductMode.OtherVehicles)
		{
			StartCoroutine(CreateShopCar(shopItem));
		}
		else if (productMode == ProductMode.Bodyguards)
		{
			StartCoroutine(CreateBodyguard(shopItem));
		}
		if (productMode != ProductMode.Vehicles && productMode != ProductMode.Pets && productMode != ProductMode.Horses && productMode != ProductMode.OtherVehicles)
		{
			DestroyShopCar();
		}
		if (productMode != ProductMode.Bodyguards)
		{
			DestroyShopGuard();
		}
		PriceLabel.text = shopItem.price.ToString();
		if (shopItem.IsBought)
		{
			BuyButton.gameObject.SetActive(value: false);
			ApplyButton.gameObject.SetActive(value: true);
			ApplyBtnLabel.text = LocalizatioManager.GetStringByKey("apply");
		}
		else
		{
			BuyButton.gameObject.SetActive(value: true);
			ApplyButton.gameObject.SetActive(value: false);
		}
		if (shopItem.IsLocked)
		{
			BuyButton.gameObject.SetActive(value: false);
			ApplyButton.gameObject.SetActive(value: false);
		}
		UpdateMainCamPosForThisProductType();
	}

	private IEnumerator CreateShopCar(ShopItem shopItem)
	{
		if (shopCarHolder != null)
		{
			UnityEngine.Object.Destroy(shopCarHolder);
		}
		shopCarHolder = UnityEngine.Object.Instantiate(DataModel.instance.GetInvObjectByShopId(shopItem.id), shopCarPivot.position, shopCarPivot.rotation);
		if (shopCarHolder.GetComponent<Vehicle>().vehicleType == VehicleType.Car)
		{
			shopCarHolder.GetComponent<CarController>().enabled = false;
			shopCarHolder.GetComponent<CarController>().EnablePhysics(enable: false);
		}
		else if (shopCarHolder.GetComponent<HelicopterController>() != null && shopCarHolder.GetComponent<HelicopterController>().vehicleType == VehicleType.Heli)
		{
			shopCarHolder.GetComponent<HelicopterController>().enabled = false;
			shopCarHolder.GetComponent<HelicopterController>().EnablePhysics(enable: false);
			shopCarHolder.GetComponent<SmoothSyncMovement>().enabled = false;
		}
		else if (shopCarHolder.GetComponent<Vehicle>().vehicleType == VehicleType.Moto)
		{
			shopCarHolder.GetComponent<MotoController>().enabled = false;
			shopCarHolder.GetComponent<MotoController>().EnablePhysics(enable: false);
		}
		else if (shopCarHolder.GetComponent<Vehicle>().vehicleType == VehicleType.Plane)
		{
			shopCarHolder.GetComponent<PlaneController>().enabled = false;
			shopCarHolder.GetComponent<PlaneController>().EnablePhysics(enable: false);
			shopCarHolder.GetComponent<SmoothSyncMovement>().enabled = false;
		}
		shopCarHolder.transform.localScale = new Vector3(shopItem.scaleForShop, shopItem.scaleForShop, shopItem.scaleForShop);
		Array.ForEach(shopCarHolder.GetComponentsInChildren<Wheel>(), delegate(Wheel wheel)
		{
			wheel.transform.SetLocalEulerX(0f);
		});
		shopCarHolder.GetComponent<Rigidbody>().isKinematic = true;
		shopCarHolder.transform.SetParent(shopCarPivot);
		shopCarHolder.transform.localPosition = Vector3.zero;
		yield return null;
		shopCarHolder.GetComponent<Rigidbody>().isKinematic = true;
		shopCarHolder.transform.SetParent(shopCarPivot);
		shopCarHolder.transform.localPosition = Vector3.zero;
	}

	private IEnumerator CreateShopPet(ShopItem shopItem, InventoryCategoryType category)
	{
		if (shopCarHolder != null)
		{
			UnityEngine.Object.Destroy(shopCarHolder);
		}
		shopCarHolder = UnityEngine.Object.Instantiate(DataModel.instance.GetPetByShopId(shopItem.id, category), shopPetPivot.position, shopPetPivot.rotation);
		if (shopCarHolder.GetComponent<Vehicle>() != null && shopCarHolder.GetComponent<Vehicle>().vehicleType == VehicleType.Horse)
		{
			shopCarHolder.GetComponent<HorseController>().enabled = false;
		}
		else if (shopCarHolder.GetComponent<InvZombie>() != null)
		{
			shopCarHolder.GetComponent<InvZombie>().enabled = false;
		}
		GeneralUtils.SetLayerRecursively(shopCarHolder, LayerMask.NameToLayer("Vehicles"));
		shopCarHolder.GetComponent<SmoothSyncMovement>().enabled = false;
		shopCarHolder.transform.SetParent(shopPetPivot);
		shopCarHolder.transform.localPosition = Vector3.zero;
		float scaleForShop = shopCarHolder.GetComponent<ShopItem>().scaleForShop;
		shopCarHolder.transform.localScale = new Vector3(scaleForShop, scaleForShop, scaleForShop);
		yield return null;
		shopCarHolder.transform.localPosition = Vector3.zero;
	}

	private IEnumerator CreateBodyguard(ShopItem shopItem)
	{
		if (guardHolder != null)
		{
			UnityEngine.Object.Destroy(guardHolder);
		}
		guardHolder = UnityEngine.Object.Instantiate(DataModel.instance.GetGuardByShopId(shopItem.id), shopBogyguardPivot.position, shopBogyguardPivot.rotation);
		guardHolder.GetComponent<Bodyguard>().enabled = false;
		guardHolder.GetComponent<CharacterController>().enabled = false;
		GeneralUtils.SetLayerRecursively(guardHolder, LayerMask.NameToLayer("Vehicles"));
		guardHolder.GetComponent<SmoothSyncMovement>().enabled = false;
		guardHolder.transform.SetParent(shopBogyguardPivot);
		float scaleForShop = guardHolder.GetComponent<ShopItem>().scaleForShop;
		guardHolder.transform.localPosition = Vector3.zero;
		guardHolder.transform.localScale = new Vector3(scaleForShop, scaleForShop, scaleForShop);
		yield return null;
		guardHolder.transform.localPosition = Vector3.zero;
	}

	private void DestroyShopCar()
	{
		playerShopPreview.gameObject.SetActive(value: true);
		if ((bool)shopCarHolder)
		{
			UnityEngine.Object.Destroy(shopCarHolder);
		}
	}

	private void DestroyShopGuard()
	{
		playerShopPreview.gameObject.SetActive(value: true);
		if ((bool)guardHolder)
		{
			UnityEngine.Object.Destroy(guardHolder);
		}
	}

	public void OmApplyBtnClick()
	{
		ApplyBought();
	}

	private void ApplyBought()
	{
		if (productMode == ProductMode.Players)
		{
			StorageController.instance.SelectedPlayer = lastSelectedShopItem.id;
		}
		else if (productMode == ProductMode.WeaponsCat1 || productMode == ProductMode.WeaponsCat2)
		{
			StorageController.instance.SetPlayerSelectedGun(playerShopPreview.GetComponent<ShopItem>().id, lastSelectedShopItem.id);
			gunPropsWidget.SetGun(lastSelectedShopItem.GetComponent<BaseWeaponScript>().gunInfo);
		}
		else if (productMode == ProductMode.Hats)
		{
			StorageController.SelectedHat = lastSelectedShopItem.id;
		}
		else if (productMode == ProductMode.Eyes)
		{
			StorageController.SelectedEyes = lastSelectedShopItem.id;
		}
		else if (productMode == ProductMode.Smiles)
		{
			StorageController.SelectedSmile = lastSelectedShopItem.id;
		}
		else if (productMode == ProductMode.Shoes)
		{
			StorageController.SelectedShoes = lastSelectedShopItem.id;
		}
		ApplyBtnLabel.text = "Applied";
	}

	public void OmBuyBtnClick()
	{
		if (LocalStore.BuyItem(lastSelectedShopItem))
		{
			ApplyBought();
			BuyButton.gameObject.SetActive(value: false);
			ApplyButton.gameObject.SetActive(value: true);
			StatisticsManager.AddExp(lastSelectedShopItem.experience);
			AnaliticsManager.LogBuyGood(lastSelectedShopItem.id);
		}
		else
		{
			MonoBehaviour.print("not enough coins");
			NotEnoughMoneyPanel.SetActive(value: true);
		}
	}

	public void OnGoToBankBtnClick()
	{
		ScreenManager.instance.ShowScreen(ScreenManager.instance.bankScreen);
	}

	private void OnCurrencyBalanceChanged(int newVal)
	{
		BalanceLabel.text = newVal.ToString();
	}

	public void OnLeaveShop()
	{
		DestroyShopCar();
		DestroyShopGuard();
		playerShopPreview.gameObject.SetActive(value: true);
		gunPropsWidget.Show(show: false);
	}

	private void UpdateMainCamPosForThisProductType()
	{
		if (productMode == ProductMode.Bodyguards)
		{
			Camera.main.transform.rotation = mainCamPosForGuardMode.rotation;
			Camera.main.transform.DOMove(mainCamPosForGuardMode.position, 0.4f);
		}
		else if (productMode == ProductMode.Vehicles || productMode == ProductMode.OtherVehicles || productMode == ProductMode.Pets || productMode == ProductMode.Horses)
		{
			Camera.main.transform.rotation = mainCamPosForVehicleMode.rotation;
			Camera.main.transform.DOMove(mainCamPosForVehicleMode.position, 0.4f);
		}
		else
		{
			Camera.main.transform.rotation = mainCamPosForGeneralMode.rotation;
			Camera.main.transform.DOMove(mainCamPosForGeneralMode.position, 0.4f);
		}
	}
}
