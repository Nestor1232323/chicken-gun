using System.Collections;
using UnityEngine;

public class ReviewGameWidget : MonoBehaviour
{
	[SerializeField]
	private GameObject window;

	[SerializeField]
	private int askingLauchPeriod = 5;

	private int MenuLaunchesCount
	{
		get
		{
			return PlayerPrefs.GetInt("MenuLaunchesCount");
		}
		set
		{
			PlayerPrefs.SetInt("MenuLaunchesCount", value);
		}
	}

	private bool NeverAskRate
	{
		get
		{
			return PlayerPrefs.GetInt("NeverAskRate") == 1;
		}
		set
		{
			PlayerPrefs.SetInt("NeverAskRate", value ? 1 : 0);
		}
	}

	private IEnumerator Start()
	{
		if (ScreenManager.instance.IsINMainMenu() && !NeverAskRate)
		{
			MenuLaunchesCount++;
			if (MenuLaunchesCount % askingLauchPeriod != 0)
			{
				window.SetActive(value: false);
			}
			else
			{
				window.SetActive(value: true);
			}
			if (DataModel.isAndroid && DataModel.instance.gameUrlAndroid == string.Empty)
			{
				window.SetActive(value: false);
			}
			else if (DataModel.isIOS && DataModel.instance.gameUrlIOS == string.Empty)
			{
				window.SetActive(value: false);
			}
		}
		yield break;
	}

	public void OnBtnYes()
	{
		window.SetActive(value: false);
		if (DataModel.isAndroid && DataModel.instance.gameUrlAndroid != null)
		{
			Application.OpenURL(DataModel.instance.gameUrlAndroid);
		}
		else if (DataModel.isIOS && DataModel.instance.gameUrlIOS != null)
		{
			Application.OpenURL(DataModel.instance.gameUrlIOS);
		}
	}

	public void OnBtnLater()
	{
		window.SetActive(value: false);
	}

	public void OnBtnNever()
	{
		NeverAskRate = true;
		window.SetActive(value: false);
	}
}
