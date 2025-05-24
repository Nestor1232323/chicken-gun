using UnityEngine;

public class BL_Impact : MonoBehaviour
{
	public float life = 0.5f;

	private bool _poolFlag;

	private void OnEnable()
	{
		if (!_poolFlag)
		{
			_poolFlag = true;
		}
		else
		{
			Invoke("Disable", life);
		}
	}

	private void Disable()
	{
		base.gameObject.SetActive(value: false);
	}
}
