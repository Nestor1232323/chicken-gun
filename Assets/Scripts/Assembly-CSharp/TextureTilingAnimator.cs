using UnityEngine;

public class TextureTilingAnimator : MonoBehaviour
{
	[SerializeField]
	private Material material;

	[SerializeField]
	private Vector2 texOffsetSpeed;

	private Vector2 offset;

	private void Update()
	{
		offset += texOffsetSpeed;
		material.mainTextureOffset = offset;
	}
}
