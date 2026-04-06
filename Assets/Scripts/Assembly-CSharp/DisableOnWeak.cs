using UnityEngine;

public class DisableOnWeak : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.SetActive(!Device.isWeakDevice);
	}
}
