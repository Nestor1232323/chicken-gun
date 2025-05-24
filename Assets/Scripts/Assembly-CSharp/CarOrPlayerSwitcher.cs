using System;
using System.Collections;
using UnityEngine;

public class CarOrPlayerSwitcher : MonoBehaviour
{
	public static CarOrPlayerSwitcher instance;

	private FPSCamera fpsCam;

	private TPSCamera tpsCam;

	private CarCamera carCamera;

	private HeliCamera heliCamera;

	private HorseCamera horseCamera;

	private KillerCamera killerCamera;

	private BaseCamera activeCamera;

	private void Awake()
	{
		instance = this;
		fpsCam = Camera.main.GetComponent<FPSCamera>();
		tpsCam = Camera.main.GetComponent<TPSCamera>();
		carCamera = Camera.main.GetComponent<CarCamera>();
		heliCamera = Camera.main.GetComponent<HeliCamera>();
		horseCamera = Camera.main.GetComponent<HorseCamera>();
		killerCamera = Camera.main.GetComponent<KillerCamera>();
		activeCamera = fpsCam;
	}

	private IEnumerator Start()
	{
		while (GameController.instance.OurPlayer == null)
		{
			yield return null;
		}
		CharacterMotor ourPlayer = GameController.instance.OurPlayer;
		ourPlayer.PlayerCrashed = (Action<UnityEngine.Object>)Delegate.Combine(ourPlayer.PlayerCrashed, new Action<UnityEngine.Object>(OnPlayerDied));
		yield return null;
		activeCamera.Enable();
	}

	public void EnterCar()
	{
		if (GameController.instance.OurPlayer.myCar != null)
		{
			VehicleType vehicleType = GameController.instance.OurPlayer.myCar.vehicleType;
			GameController.instance.OurPlayer.OnEnterCar(GameController.instance.OurPlayer.myCar);
			fpsCam.Disable();
			tpsCam.Disable();
			switch (vehicleType)
			{
			case VehicleType.Car:
			case VehicleType.Moto:
				heliCamera.Disable();
				horseCamera.Disable();
				carCamera.Enable();
				activeCamera = carCamera;
				carCamera.SetTarget(GameController.instance.OurPlayer.myCar.transform);
				break;
			case VehicleType.Heli:
				carCamera.Disable();
				horseCamera.Disable();
				heliCamera.Enable();
				activeCamera = heliCamera;
				heliCamera.SeTarget(((HelicopterController)GameController.instance.OurPlayer.myCar).CamTarget());
				break;
			case VehicleType.Horse:
				carCamera.Disable();
				heliCamera.Disable();
				horseCamera.Enable();
				activeCamera = horseCamera;
				horseCamera.SeTarget(GameController.instance.OurPlayer.myCar.transform);
				break;
			case VehicleType.Plane:
				carCamera.Disable();
				heliCamera.Disable();
				horseCamera.Disable();
				break;
			}
			GameWindow.instance.ShowCrosshair(val: false);
			GameWindow.instance.OnEnterVehicleMode(GameController.instance.OurPlayer.myCar.vehicleType);
		}
	}

	public void GetOutFromCar()
	{
		if (GameController.instance.OurPlayer.myCar != null)
		{
			GameController.instance.OurPlayer.OnLeaveCar(GameController.instance.OurPlayer.myCar);
			fpsCam.Enable();
			tpsCam.Disable();
			activeCamera = fpsCam;
			heliCamera.Disable();
			carCamera.Disable();
			horseCamera.Disable();
			GameWindow.instance.OnEnterPlayerMode();
		}
	}

	public void TottleCameras()
	{
		tpsCam.ToggleModes();
	}

	public void SwitchCameraToSniperMode()
	{
		fpsCam.EnterSniperMode();
		GameWindow.instance.OnEnterSniperMode(fpsCam.isInSniperMode);
	}

	private void OnDestroy()
	{
		if ((bool)GameController.instance.OurPlayer)
		{
			CharacterMotor ourPlayer = GameController.instance.OurPlayer;
			ourPlayer.PlayerCrashed = (Action<UnityEngine.Object>)Delegate.Remove(ourPlayer.PlayerCrashed, new Action<UnityEngine.Object>(OnPlayerDied));
		}
	}

	private void OnPlayerDied(object sender)
	{
		if (fpsCam == activeCamera && fpsCam.isInSniperMode)
		{
			GameWindow.instance.OnEnterSniperMode(enter: false);
		}
	}

	public void ActivateKillercamera(Transform target)
	{
		fpsCam.Disable();
		tpsCam.Disable();
		carCamera.Disable();
		heliCamera.Disable();
		horseCamera.Disable();
		killerCamera.SetTarget(target);
		killerCamera.Enable();
	}

	public void DisableKillerCamera(bool withRespawn = true)
	{
		GameController.instance.OurPlayer.Respawn();
		if (killerCamera.isActiveAndEnabled)
		{
			killerCamera.Disable();
			fpsCam.Enable();
		}
	}

	public void ToggleSniperMode()
	{
		tpsCam.ToggleSniperMode();
	}

	public void RefreshTPSCamFollowSettings(TPSCamSettings[] list)
	{
		tpsCam.SetSettingsList(list);
	}

	public void OnOutFromPauseMode()
	{
		fpsCam.Enable();
	}

	public bool IsActiveCamera(BaseCamera camera)
	{
		return activeCamera == camera;
	}
}
