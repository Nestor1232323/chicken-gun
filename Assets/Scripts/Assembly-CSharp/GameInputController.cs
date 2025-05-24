using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameInputController : MonoBehaviour, IJoystickListener
{
	public static GameInputController instance;

	public JoystickController myJoystick;

	public Vector2 mouseDelta = Vector2.zero;

	public RectTransform shootBtnRect;

	public Text mouseDeltaTestLabel;

	private Vector2 joystickDelta = Vector2.zero;

	public ShootBtnMode shootBtnMode;

	public LongTapButton shootLongTapBtn;

	public GameObject shootNormalBtn;

	public GameObject sniperModeBtn;

	[SerializeField]
	private Toggle autoshootToggle;

	public static Action RotationStarted;

	public static Action RotationFinished;

	[HideInInspector]
	public Vector2 moveOffset;

	private Vector2 baseMoveOffset;

	public bool autoshootEnabled;

	public static Action<bool> AutoshootToggled;

	private float _inwWidthGrad;

	private float _inwHeightGrad;

	private int fingerId = -1;

	private Vector3 startTouchPos;

	private bool isMousePressed;

	public Vector3 prevTouchPos = Vector3.zero;

	public Vector3 curTouchPos = Vector3.zero;

	private float shootBtnDownTime;

	private float jumpBtnDownTime;

	private void Awake()
	{
		instance = this;
		myJoystick.myInputListener = this;
		EnterSniperMode(enter: false);
		autoshootEnabled = StorageController.AutoshootEnabled;
		autoshootToggle.isOn = autoshootEnabled;
		shootNormalBtn.SetActive(!autoshootEnabled);
	}

	private IEnumerator start()
	{
		GameController.MatchFinidhed += StopAll;
		yield return null;
	}

	private void OnDestroy()
	{
		GameController.MatchFinidhed -= StopAll;
	}

	private void Update()
	{
		UpdateMouseDeltaMove();
	}

	public void OnAutoshootToggle(bool toggle)
	{
		autoshootEnabled = autoshootToggle.isOn;
		StorageController.AutoshootEnabled = autoshootEnabled;
		shootNormalBtn.SetActive(!autoshootEnabled);
		if (AutoshootToggled != null)
		{
			AutoshootToggled(autoshootEnabled);
		}
	}

	public void UpdateMouseDeltaMove()
	{
		if (fingerId == -1)
		{
			return;
		}
		if (GameController.isMobile)
		{
			if (fingerId != -1)
			{
				Touch[] touches = Input.touches;
				for (int i = 0; i < touches.Length; i++)
				{
					Touch touch = touches[i];
					if (touch.fingerId == fingerId)
					{
						prevTouchPos = curTouchPos;
						curTouchPos = touch.position;
					}
				}
			}
			else
			{
				prevTouchPos = curTouchPos;
			}
		}
		else if (fingerId == 1)
		{
			prevTouchPos = curTouchPos;
			curTouchPos = Input.mousePosition;
		}
		else
		{
			mouseDelta = Vector2.zero;
		}
		if (moveOffset.y >= 60f && mouseDelta.y > 0f)
		{
			startTouchPos = curTouchPos;
			baseMoveOffset = moveOffset;
		}
		if (moveOffset.y <= -70f && mouseDelta.y < 0f)
		{
			startTouchPos = curTouchPos;
			baseMoveOffset = moveOffset;
		}
		mouseDelta = curTouchPos - prevTouchPos;
		moveOffset = curTouchPos - startTouchPos;
		moveOffset.x *= _inwWidthGrad;
		moveOffset.y *= _inwHeightGrad;
		moveOffset += baseMoveOffset;
		moveOffset.y = Mathf.Clamp(moveOffset.y, -70f, 60f);
	}

	public void EnterSniperMode(bool enter)
	{
		_inwWidthGrad = 1f / (float)Screen.width * (float)((!enter) ? 320 : 50);
		_inwHeightGrad = 1f / (float)Screen.height * ((!enter) ? ((float)Screen.height * 1f / (float)Screen.width * 320f) : 50f);
	}

	public bool DoesRectContainPoint(RectTransform rect, Vector2 point)
	{
		return RectTransformUtility.RectangleContainsScreenPoint(rect, point, GameWindow.instance.gameWindowCanvas.worldCamera);
	}

	public bool IsDragging()
	{
		if (Input.GetMouseButton(0) && mouseDelta.sqrMagnitude > 9f)
		{
			return true;
		}
		return false;
	}

	public void EnterSniperModeBtnClick()
	{
		CarOrPlayerSwitcher.instance.ToggleSniperMode();
	}

	public void ShootBtnDown()
	{
		GeneralListenerAreaTouchDown();
		if (shootBtnMode == ShootBtnMode.ShootOnDown)
		{
			GameController.instance.OurPlayer.StartShoot();
		}
	}

	public void ShootBtnUp()
	{
		GeneralListenerAreaTouchUp();
		if (shootBtnMode == ShootBtnMode.ShootOnDown)
		{
			GameController.instance.OurPlayer.StopShooting();
			return;
		}
		GameController.instance.OurPlayer.StartShoot();
		GameController.instance.OurPlayer.StopShooting();
	}

	public void ShootBtnUp(float holdDuration)
	{
		GeneralListenerAreaTouchUp();
		GameController.instance.OurPlayer.StartShoot(holdDuration);
	}

	public void OnJumpBtn()
	{
		GameController.instance.OurPlayer.Jump();
		jumpBtnDownTime = Time.time;
	}

	public void OnJumpBtnUp()
	{
	}

	private IEnumerator JumpBtnCRT()
	{
		float t = 0.3f;
		if (GameController.instance.OurPlayer.timeInAir > 0.12f)
		{
			t = 0f;
		}
		yield return null;
		while (!(Time.time - jumpBtnDownTime > t))
		{
			yield return null;
		}
		GameController.instance.OurPlayer.myJetPack.StartFlight();
	}

	public void ChangeCameraType()
	{
		if (!Camera.main.GetComponent<FPSCamera>().enabled)
		{
			Camera.main.GetComponent<FPSCamera>().enabled = true;
			Camera.main.GetComponent<TPSCamera>().enabled = false;
		}
		else if (!Camera.main.GetComponent<TPSCamera>().enabled)
		{
			Camera.main.GetComponent<TPSCamera>().enabled = true;
			Camera.main.GetComponent<FPSCamera>().enabled = false;
		}
	}

	public void StopAll()
	{
		GameController.instance.OurPlayer.myJetPack.StopFlight();
		GeneralListenerAreaTouchUp();
		myJoystick.StopActions();
	}

	public void EnterCarBtnClick()
	{
		CarOrPlayerSwitcher.instance.EnterCar();
	}

	public void LeaveCarBtnClick()
	{
		CarOrPlayerSwitcher.instance.GetOutFromCar();
	}

	public void GeneralListenerAreaTouchDown()
	{
		if (GameController.isMobile)
		{
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				if (touch.phase == TouchPhase.Began)
				{
					fingerId = touch.fingerId;
					curTouchPos = touch.position;
					prevTouchPos = curTouchPos;
					startTouchPos = touch.position;
				}
			}
		}
		else
		{
			fingerId = 1;
			curTouchPos = Input.mousePosition;
			prevTouchPos = curTouchPos;
			startTouchPos = Input.mousePosition;
		}
		if (RotationStarted != null)
		{
			RotationStarted();
		}
	}

	public void GeneralListenerAreaTouchUp()
	{
		if (GameController.isMobile)
		{
			Touch[] touches = Input.touches;
			foreach (Touch touch in touches)
			{
				if (touch.fingerId == fingerId)
				{
					fingerId = -1;
				}
			}
		}
		else
		{
			fingerId = -1;
		}
		prevTouchPos = curTouchPos;
		baseMoveOffset = moveOffset;
		if (RotationFinished != null)
		{
			RotationFinished();
		}
	}

	public void SetDelta(Vector2 delta)
	{
		joystickDelta = delta;
		if ((bool)GameController.instance.OurPlayer)
		{
			GameController.instance.OurPlayer.moveNormalDir = joystickDelta;
		}
	}

	public void SetShootBtnMode(ShootBtnMode mode)
	{
		shootBtnMode = mode;
	}

	public void ShowSniperModeBtn(bool show)
	{
		sniperModeBtn.SetActive(show);
	}

	public void OnToggleCamerasBtnTap()
	{
		CarOrPlayerSwitcher.instance.TottleCameras();
	}

	public void GrenadeThrowBtnClick(int grenadeType)
	{
		GameController.instance.OurPlayer.ThrowGrenade(grenadeType);
	}

	public void ReloadBtnClick()
	{
		GameController.instance.OurPlayer.playerWeaponManager.CurrentWeapon.Reload();
	}
}
