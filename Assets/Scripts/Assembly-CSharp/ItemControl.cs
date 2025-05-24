using System.Collections;
using UnityEngine;

public class ItemControl : MonoBehaviour
{
	public MeshRenderer mat;

	public Texture2D[] tex;

	public Collider coll;

	public float waitTime;

	public void InitBullet()
	{
		coll.enabled = true;
		StartCoroutine(DestroySelf(waitTime));
	}

	public void DestroyItem(float wait)
	{
		StartCoroutine(DestroySelf(wait));
	}

	private IEnumerator DestroySelf(float wait)
	{
		yield return new WaitForSeconds(wait);
		coll.enabled = false;
		Object.Destroy(base.gameObject);
	}

	public void ChangeTexture(int idx)
	{
		if (idx >= tex.Length || idx < 0)
		{
			idx = 0;
		}
		mat.material.mainTexture = tex[idx];
	}

	public void ChangeTextureRandom()
	{
		int idx = Random.Range(0, tex.Length);
		ChangeTexture(idx);
	}
}
