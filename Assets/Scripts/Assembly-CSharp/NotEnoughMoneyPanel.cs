using UnityEngine;

public class NotEnoughMoneyPanel : MonoBehaviour
{
	public void OnOkClick()
	{
		base.gameObject.SetActive(value: false);
	}
}
