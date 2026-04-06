using UnityEngine;
using UnityEngine.UI;

public class LongBtnTapRadialImage : MonoBehaviour, ILongBtnTapUIUpdater
{
	[SerializeField]
	private Image img;

	private void Start()
	{
		img.fillAmount = 0f;
	}

	public void SetValue(float val)
	{
		img.fillAmount = val;
	}
}
