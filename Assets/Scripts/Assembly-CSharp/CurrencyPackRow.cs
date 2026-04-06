using UnityEngine;
using UnityEngine.UI;

public class CurrencyPackRow : MonoBehaviour
{
	[SerializeField]
	private Text coinsCount;

	[SerializeField]
	private Text price;

	public void SetData(CoinsPack pack)
	{
		coinsCount.text = pack.coinsCount.ToString();
		price.text = pack.price.ToString();
	}
}
