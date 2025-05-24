using System;
using System.Collections;
using UnityEngine;

public class FPSCamera : BaseCamera
{
	public static FPSCamera instance;

	public Transform CameraPosition;

	private bool allowUpdate;

	public LayerMask layerMask;

	private Camera cam;

	public bool isInSniperMode;

	private CharacterMotor ourPlayer;

	public static Action Enabled;

	public static Action Disabled;

	[Space]
	[Space]
	[Space]
	[Space]
	[SerializeField]
	private float normalFieldOfView = 65f;

	[SerializeField]
	private float sniperFieldOfView = 18f;

	[SerializeField]
	public float normalSmoothTime = 0.1f;

	[SerializeField]
	public float sniperSmoothTime = 0.1f;

	protected Vector2 initAngles;

	private float camInitAng = -1f;

	private Vector2 mouseDelta;

	private Vector2 mouseDeltaVel;

	private float offsY;

	private float yAng;

	private Vector3 spineAngles;

	private Vector3 moveOffset = Vector3.zero;

	private Vector3 bufMoveOffset = Vector3.zero;

	private Vector3 refVel;

	private Ray ray;

	private RaycastHit hitinfo;

	private bool hasTarget;

	private BodyPartDmgReceiver bodyPartDmgReceiver;

	private float aimDelay;

	private IEnumerator Start()
	{
		cam = Camera.main;
		instance = this;
		GameController.instance.cameraMode = CameraMode.FPS;
		yield return null;
		GameInputController.RotationStarted = (Action)Delegate.Combine(GameInputController.RotationStarted, new Action(OnRotationStarted));
		GameInputController.RotationFinished = (Action)Delegate.Combine(GameInputController.RotationFinished, new Action(OnRotationFinished));
		while (GameController.instance == null || GameController.instance.OurPlayer == null || GameController.instance.OurPlayer.playerWeaponManager == null)
		{
			yield return null;
		}
		GameController.MatchFinidhed += OnMatchFinidhed;
		GameController.ContinueMatch = (Action)Delegate.Combine(GameController.ContinueMatch, new Action(OnContinueMatch));
		ourPlayer = GameController.instance.OurPlayer;
		cam.transform.SetParent(ourPlayer.FPSCameraPosition);
		cam.transform.localPosition = Vector3.zero;
		cam.transform.localRotation = Quaternion.identity;
		GameWindow.instance.OnEnterFPSCameraMode();
		cam.nearClipPlane = 0.17f;
		cam.fieldOfView = normalFieldOfView;
		yield return null;
		Enable();
		OnRotationStarted();
		PlayerWeaponManager playerWeaponManager = ourPlayer.playerWeaponManager;
		playerWeaponManager.WeaponSwitched = (Action)Delegate.Combine(playerWeaponManager.WeaponSwitched, new Action(OnWeaponSwitched));
	}

	private void OnMatchFinidhed()
	{
		if (base.enabled)
		{
			cam.transform.SetParent(null);
			base.enabled = false;
			EnterSniperMode(val: false);
		}
	}

	private void OnContinueMatch()
	{
		if (CarOrPlayerSwitcher.instance.IsActiveCamera(this))
		{
			base.enabled = true;
			cam.transform.SetParent(ourPlayer.FPSCameraPosition);
			cam.transform.localPosition = Vector3.zero;
			cam.transform.localRotation = Quaternion.identity;
		}
	}

	private void OnWeaponSwitched()
	{
		if (base.isActiveAndEnabled)
		{
			cam.transform.SetParent(ourPlayer.FPSCameraPosition);
			cam.transform.localPosition = Vector3.zero;
			cam.transform.localRotation = Quaternion.identity;
		}
	}

	private void OnRotationStarted()
	{
		if (camInitAng == -1f)
		{
			initAngles.x = ourPlayer.characterAnimation.topBody.transform.parent.localEulerAngles.z;
			initAngles.y = ourPlayer.transform.eulerAngles.y;
			camInitAng = base.transform.parent.parent.localEulerAngles.x;
			allowUpdate = true;
		}
	}

	private void OnRotationFinished()
	{
	}

	public void OnEnable()
	{
	}

	private void OnDestroy()
	{
		GameController.MatchFinidhed -= OnMatchFinidhed;
		GameController.ContinueMatch = (Action)Delegate.Remove(GameController.ContinueMatch, new Action(OnContinueMatch));
		GameInputController.RotationStarted = (Action)Delegate.Remove(GameInputController.RotationStarted, new Action(OnRotationStarted));
		GameInputController.RotationFinished = (Action)Delegate.Remove(GameInputController.RotationFinished, new Action(OnRotationFinished));
		if (ourPlayer != null)
		{
			PlayerWeaponManager playerWeaponManager = ourPlayer.playerWeaponManager;
			playerWeaponManager.WeaponSwitched = (Action)Delegate.Remove(playerWeaponManager.WeaponSwitched, new Action(OnWeaponSwitched));
		}
	}

	private void LateUpdate()
	{
		if (allowUpdate)
		{
			bufMoveOffset = moveOffset;
			moveOffset = Vector3.SmoothDamp(bufMoveOffset, GameInputController.instance.moveOffset, ref refVel, normalSmoothTime);
			ourPlayer.transform.SetLocalEulerY(initAngles.y + moveOffset.x);
			offsY = moveOffset.y;
			ourPlayer.characterAnimation.topBody.SetLocalEulerX(0f - offsY);
			UpdateBulletPoint();
		}
	}

	public Vector3 GetCamLookBulletPoint()
	{
		ray = cam.ScreenPointToRay(GameWindow.instance.AimSprite.rectTransform.position);
		if (Physics.Raycast(ray, out hitinfo, 300f, layerMask, QueryTriggerInteraction.Collide))
		{
			return hitinfo.point;
		}
		return ray.origin + ray.direction * 300f;
	}

	private void UpdateBulletPoint()
	{
		ray = cam.ScreenPointToRay(GameWindow.instance.AimSprite.rectTransform.position);
		if (Physics.Raycast(ray, out hitinfo, 300f, layerMask, QueryTriggerInteraction.Collide))
		{
			ourPlayer.playerWeaponManager.bulletTargetPoint = hitinfo.point;
			bodyPartDmgReceiver = hitinfo.collider.GetComponent<BodyPartDmgReceiver>();
			if (bodyPartDmgReceiver != null)
			{
				if (bodyPartDmgReceiver.myPlayer.ViewId != ourPlayer.ViewId && (bodyPartDmgReceiver.myPlayer.myTeam != ourPlayer.myTeam || ourPlayer.myTeam == TeamID.None))
				{
					hasTarget = true;
					aimDelay += Time.deltaTime;
					GameWindow.instance.HighlightAim();
					if (GameInputController.instance.autoshootEnabled && aimDelay >= ourPlayer.playerWeaponManager.CurrentWeapon.autoshootAimDelay)
					{
						ourPlayer.OnCursorFindAim();
					}
				}
			}
			else if (hasTarget)
			{
				aimDelay = 0f;
				GameWindow.instance.DeHighlightAim();
				if (GameInputController.instance.autoshootEnabled && ourPlayer.isNowShooting)
				{
					ourPlayer.OnCursorLostAim();
				}
				hasTarget = false;
			}
			return;
		}
		ourPlayer.playerWeaponManager.bulletTargetPoint = ray.origin + ray.direction * 60f;
		if (hasTarget)
		{
			hasTarget = false;
			aimDelay = 0f;
			GameWindow.instance.DeHighlightAim();
			if (ourPlayer.isNowShooting && GameInputController.instance.autoshootEnabled)
			{
				ourPlayer.OnCursorLostAim();
			}
		}
	}

	public override void Enable()
	{
		base.Enable();
		cam.transform.SetParent(ourPlayer.FPSCameraPosition);
		cam.transform.localPosition = Vector3.zero;
		cam.transform.localRotation = Quaternion.identity;
		EnterSniperMode(val: false);
	}

	public override void Disable()
	{
		base.Disable();
		base.enabled = false;
		EnterSniperMode(val: false);
	}

	public override void EnterSniperMode()
	{
		isInSniperMode = !isInSniperMode;
		GameInputController.instance.EnterSniperMode(isInSniperMode);
		cam.fieldOfView = ((!isInSniperMode) ? normalFieldOfView : sniperFieldOfView);
	}

	public void EnterSniperMode(bool val)
	{
		isInSniperMode = val;
		GameInputController.instance.EnterSniperMode(isInSniperMode);
		cam.fieldOfView = ((!isInSniperMode) ? normalFieldOfView : sniperFieldOfView);
	}

	public void OnGoToMainMenu()
	{
		Disable();
		base.transform.SetParent(null);
	}
}
