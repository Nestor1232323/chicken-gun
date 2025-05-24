using UnityEngine;
using UnityEngine.UI;

public class BodyguardsWidget : MonoBehaviour
{
	[SerializeField]
	private GameObject callButton;

	[SerializeField]
	private Transform scrollContent;

	[SerializeField]
	private GameObject bodyguardButtonPrefab;

	[SerializeField]
	private GameObject scroll;

	[SerializeField]
	private GameObject currentBodyguardIcon;

	private void Start()
	{
		CreateListOfBodyguards();
	}

	public void OnClickMe()
	{
		if (scroll.activeSelf)
		{
			scroll.SetActive(value: false);
		}
		else if (GameController.instance.OurPlayer.MyBodyguard == null)
		{
			scroll.SetActive(value: true);
		}
		else
		{
			MonoBehaviour.print("You already have a boduguard " + GameController.instance.OurPlayer.MyBodyguard.name);
		}
	}

	private void CreateListOfBodyguards()
	{
		GameObject[] bodyguards = DataModel.instance.Bodyguards;
		for (int i = 0; i < bodyguards.Length; i++)
		{
			ShopItem component = bodyguards[i].GetComponent<ShopItem>();
			if (component.IsBought)
			{
				GameObject obj = Object.Instantiate(bodyguardButtonPrefab);
				obj.transform.SetParent(scrollContent);
				obj.transform.localScale = Vector3.one;
				obj.GetComponent<Image>().sprite = component.icon;
				string guardId = component.id;
				obj.GetComponent<Button>().onClick.AddListener(delegate
				{
					CallGuard(guardId);
				});
			}
		}
	}

	private void CallGuard(string guardId)
	{
		scroll.SetActive(value: false);
		GameController.instance.OurPlayer.CallBodyguard(guardId);
		currentBodyguardIcon.SetActive(value: true);
		callButton.SetActive(value: false);
		currentBodyguardIcon.transform.GetChild(0).GetComponent<Image>().sprite = GameController.instance.OurPlayer.MyBodyguard.GetComponent<ShopItem>().icon;
	}

	public void CloseWidget()
	{
		scroll.SetActive(value: false);
		callButton.SetActive(value: true);
	}

	public void DestroyCurrentBodyguard()
	{
		MonoBehaviour.print("DestroyCurrentBodyguard");
		GameController.instance.OurPlayer.DestroyCurrentBodyguard();
		currentBodyguardIcon.SetActive(value: false);
		scroll.SetActive(value: false);
		callButton.SetActive(value: true);
	}
}
