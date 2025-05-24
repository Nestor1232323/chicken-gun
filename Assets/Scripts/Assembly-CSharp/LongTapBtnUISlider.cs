using UnityEngine;
using UnityEngine.UI;

public class LongTapBtnUISlider : MonoBehaviour, ILongBtnTapUIUpdater
{
	[SerializeField]
	private Slider slider;

	public void SetValue(float val)
	{
		slider.value = val;
	}
}
