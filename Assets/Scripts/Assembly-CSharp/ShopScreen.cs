using UnityEngine;

public class ShopScreen : BaseScreen
{
	public Transform CameraPosForThisScreen;

	public Transform playerPosForThisScreen;

	private void Start()
	{
		Camera.main.transform.position = CameraPosForThisScreen.position;
		Camera.main.transform.rotation = CameraPosForThisScreen.rotation;
	}

	protected override void OnShow()
	{
		Camera.main.transform.position = CameraPosForThisScreen.position;
		Camera.main.transform.rotation = CameraPosForThisScreen.rotation;
		Camera.main.transform.SetParent(CameraPosForThisScreen);
		ShopController.instance.OnShow();
	}

	protected override void OnHide()
	{
		base.OnHide();
		Camera.main.transform.SetParent(null);
		ShopController.instance.OnLeaveShop();
	}

	public void OnFreeCoinsBtn()
	{
		ScreenManager.instance.ShowScreen(ScreenManager.instance.freeMoneyScreen);
	}
}
