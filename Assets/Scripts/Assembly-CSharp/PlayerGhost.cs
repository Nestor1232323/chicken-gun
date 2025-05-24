using UnityEngine;

public class PlayerGhost : MonoBehaviour
{
	[SerializeField]
	private Transform hatPivot;

	[SerializeField]
	private Transform leftShoesPivot;

	[SerializeField]
	private Transform rightShoesPivot;

	[SerializeField]
	private Material grayMat;

	public PlayerClothingManager clothingManager;

	private void Start()
	{
	}
}
