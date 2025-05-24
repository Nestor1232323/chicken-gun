using UnityEngine;

public class PlayerShopPreview : MonoBehaviour
{
	public PlayerClothingManager playerClothingManager;

	public bool IsUnlocked => GetComponent<ShopItem>().IsBought;

	private void Awake()
	{
		playerClothingManager = GetComponent<PlayerClothingManager>();
	}
}
