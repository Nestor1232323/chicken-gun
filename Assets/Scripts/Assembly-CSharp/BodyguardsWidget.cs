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
			scroll.SetActive(false);
		}
		else if (GameController.instance.OurPlayer.MyBodyguard == null)
		{
			scroll.SetActive(true);
		}
		else
		{
			MonoBehaviour.print("You already have a boduguard " + GameController.instance.OurPlayer.MyBodyguard.name);
		}
	}

	private void CreateListOfBodyguards()
	{
		GameObject[] bodyguards = DataModel.instance.Bodyguards;
		foreach (GameObject gameObject in bodyguards)
		{
			ShopItem component = gameObject.GetComponent<ShopItem>();
			if (component.IsBought)
			{
				GameObject gameObject2 = Object.Instantiate(bodyguardButtonPrefab);
				gameObject2.transform.SetParent(scrollContent);
				gameObject2.transform.localScale = Vector3.one;
				gameObject2.GetComponent<Image>().sprite = component.icon;
				string guardId = component.id;
				gameObject2.GetComponent<Button>().onClick.AddListener(delegate
				{
					CallGuard(guardId);
				});
			}
		}
	}

	private void CallGuard(string guardId)
	{
		scroll.SetActive(false);
		GameController.instance.OurPlayer.CallBodyguard(guardId);
		currentBodyguardIcon.SetActive(true);
		callButton.SetActive(false);
		currentBodyguardIcon.transform.GetChild(0).GetComponent<Image>().sprite = GameController.instance.OurPlayer.MyBodyguard.GetComponent<ShopItem>().icon;
	}

	public void CloseWidget()
	{
		scroll.SetActive(false);
		callButton.SetActive(true);
	}

	public void DestroyCurrentBodyguard()
	{
		MonoBehaviour.print("DestroyCurrentBodyguard");
		GameController.instance.OurPlayer.DestroyCurrentBodyguard();
		currentBodyguardIcon.SetActive(false);
		scroll.SetActive(false);
		callButton.SetActive(true);
	}
}
