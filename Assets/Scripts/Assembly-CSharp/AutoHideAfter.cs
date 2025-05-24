using UnityEngine;

public class AutoHideAfter : MonoBehaviour
{
	private void OnEnable()
	{
		CancelInvoke("Hide");
		Invoke("Hide", 0.5f);
	}

	private void Hide()
	{
		base.gameObject.SetActive(value: false);
	}
}
