using System;
using UnityEngine;
using UnityEngine.UI;

public class StoreScreen : BaseScreen
{
	[SerializeField]
	private CoinsPack[] coinsPacks;

	public Text balanceCountLabel;

	[SerializeField]
	private GameObject coinsPackPrefab;

	[SerializeField]
	private Transform packsContainer;

	public Text NoAdsButton;

	private void Start()
	{
		Hide();
		LocalStore.CurrencyBalanceChanged = (Action<int>)Delegate.Combine(LocalStore.CurrencyBalanceChanged, new Action<int>(OnCurrencyBalanceChanged));
		LocalStore.IAPCompleted = (Action)Delegate.Combine(LocalStore.IAPCompleted, new Action(UpdateAfterIAP));
		CheckAdsState();
	}

	private void OnDestroy()
	{
		LocalStore.CurrencyBalanceChanged = (Action<int>)Delegate.Remove(LocalStore.CurrencyBalanceChanged, new Action<int>(OnCurrencyBalanceChanged));
		LocalStore.IAPCompleted = (Action)Delegate.Remove(LocalStore.IAPCompleted, new Action(UpdateAfterIAP));
	}

	private void OnCurrencyBalanceChanged(int newVal)
	{
		balanceCountLabel.text = newVal.ToString();
	}

	protected override void OnShow()
	{
		balanceCountLabel.text = LocalStore.currencyBalance.ToString();
	}

	public override void Update()
	{
		chechBackKey();
	}

	public void DisableAds()
	{
	}

	public void UpdateAfterIAP()
	{
		CheckAdsState();
	}

	public CoinsPack GetCoinsPack(int index)
	{
		return coinsPacks[index];
	}

	public CoinsPack GetCoinsPack(string name)
	{
		return Array.Find(coinsPacks, (CoinsPack pack) => pack.packName == name);
	}

	private void CheckAdsState()
	{
		NoAdsButton.text = ((!LocalStore.isAdsDisabled) ? LocalizatioManager.GetStringByKey("any_purchace_will_dis_ads") : LocalizatioManager.GetStringByKey("ads_disabled"));
	}
}
