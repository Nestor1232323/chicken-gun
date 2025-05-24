using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LongTapButton : MonoBehaviour
{
	[Serializable]
	public class ToggleButtonClickCallBack : UnityEvent<float>
	{
	}

	public ToggleButtonClickCallBack OnLongTapUpIncomplete = new ToggleButtonClickCallBack();

	[SerializeField]
	private ILongBtnTapUIUpdater progressListener;

	[SerializeField]
	private UnityEvent OnLongTapComplete;

	public Action TouchDown;

	public Action<float> TouchUp;

	private float holdTime;

	public float PeriodTime = 1.8f;

	private float progress;

	private IEnumerator touchCRT;

	private void Start()
	{
		progressListener = GetComponentInChildren<ILongBtnTapUIUpdater>();
		progressListener.SetValue(0f);
	}

	public void OnTouchDown()
	{
		holdTime = Time.time;
		if (TouchDown != null)
		{
			TouchDown();
		}
		touchCRT = TouchDownCRT();
		StartCoroutine(touchCRT);
	}

	private IEnumerator TouchDownCRT()
	{
		float t = 0f;
		while (t < PeriodTime)
		{
			t += Time.deltaTime;
			progress = t / PeriodTime;
			progressListener.SetValue(progress);
			yield return null;
		}
		if (OnLongTapComplete != null)
		{
			OnLongTapComplete.Invoke();
		}
	}

	public void OnTouchUp()
	{
		if (OnLongTapUpIncomplete != null)
		{
			OnLongTapUpIncomplete.Invoke(progress);
		}
		StopCoroutine(touchCRT);
		if (TouchUp != null)
		{
			float num = Time.time - holdTime;
			TouchUp(num / PeriodTime);
		}
		progressListener.SetValue(0f);
		progress = 0f;
	}

	private void OnDestroy()
	{
		OnLongTapUpIncomplete.RemoveAllListeners();
	}
}
