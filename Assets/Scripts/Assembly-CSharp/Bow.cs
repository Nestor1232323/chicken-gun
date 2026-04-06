using UnityEngine;

public class Bow : BaseMeleeWeapon
{
	[SerializeField]
	private GameObject arrow;

	[SerializeField]
	private GameObject arrowLines;

	public void ShowArrow()
	{
		arrow.SetActive(true);
	}

	public void HideArrow()
	{
		arrow.SetActive(false);
	}
}
