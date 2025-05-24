using UnityEngine;

public class BaseScreen : MonoBehaviour
{
	[HideInInspector]
	public bool isActive;

	[HideInInspector]
	public BaseScreen previousScreen;

	public virtual void Awake()
	{
	}

	public virtual void Update()
	{
		chechBackKey();
	}

	public void chechBackKey()
	{
		Input.GetKeyDown(KeyCode.Escape);
	}

	public virtual void MoveToPreviousScreen()
	{
		ScreenManager.instance.HideScreen(this);
	}

	public virtual void MoveToNextScreen()
	{
	}

	public virtual void OnBackButtonClick()
	{
		MoveToPreviousScreen();
	}

	protected virtual void OnShow()
	{
		isActive = true;
	}

	protected virtual void OnHide()
	{
		isActive = false;
	}

	public virtual void Show()
	{
		base.gameObject.SetActive(value: true);
		GetComponent<RectTransform>().localPosition = Vector3.zero;
		OnShow();
	}

	public virtual void Hide()
	{
		base.gameObject.SetActive(value: false);
		OnHide();
	}
}
