using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RespawnFadePanel : MonoBehaviour
{
	[SerializeField]
	private Image fadeImg;

	private Tweener _tweener;

	public void Show()
	{
		base.gameObject.SetActive(value: true);
		StartCoroutine(ShowCRT());
	}

	private IEnumerator ShowCRT()
	{
		if (_tweener != null)
		{
			_tweener.Complete();
		}
		_tweener = fadeImg.DOFade(0.75f, 0.6f);
		yield return new WaitForSeconds(0.9f);
		_tweener = fadeImg.DOFade(0f, 0.3f);
		base.gameObject.SetActive(value: false);
	}
}
