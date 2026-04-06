using System.Collections;
using UnityEngine;

public class CarInputController : MonoBehaviour
{
	public static CarInputController instance;

	public GameObject[] CarControlPanels;

	public GameObject shootBtn;

	private CarControlType carControlType;

	private CarController carController;

	public float accel;

	public float brake;

	public float steer;

	public float ebrake;

	private IEnumerator Start()
	{
		GameController.MatchFinidhed += OnMatchFinished;
		while (GameController.instance.OurPlayer.myCar == null)
		{
			yield return null;
		}
		carController = GameController.instance.OurPlayer.myCar as CarController;
	}

	private void OnDestroy()
	{
		GameController.MatchFinidhed -= OnMatchFinished;
	}

	private void OnEnable()
	{
		carController = GameController.instance.OurPlayer.myCar as CarController;
		shootBtn.SetActive(carController.Shootable());
	}

	private void OnMatchFinished()
	{
		StopAllActions();
	}

	public void ShowThisCarControlPanel(int index)
	{
	}

	private void Update()
	{
		if (!(carController == null))
		{
			if (carController.Alive && !GameController.isMobile)
			{
				float axis = Input.GetAxis("Vertical");
				accel = ((!(axis >= 0f)) ? 0f : axis);
				brake = ((!(axis <= 0f)) ? 0f : (0f - axis));
				steer = Input.GetAxis("Horizontal");
				carController.pitch = Input.GetAxis("Vertical");
				carController.roll = Input.GetAxis("Horizontal");
			}
			if (accel == 0f && brake == 0f)
			{
				ebrake = 0.63181f;
			}
			else
			{
				ebrake = 0f;
			}
			carController.accel = accel;
			carController.brake = brake;
			carController.steer = steer;
			carController.ebrake = ebrake;
		}
	}

	private void UpdateGyroscope()
	{
		steer = Input.acceleration.x * 4f;
	}

	public void OnAccelDown()
	{
		accel = 1f;
	}

	public void OnAccelUp()
	{
		accel = 0f;
	}

	public void OnBrakeDown()
	{
		brake = 1f;
	}

	public void OnBrakeUp()
	{
		brake = 0f;
	}

	public void OnLeftDown()
	{
		steer = -1f;
	}

	public void OnLeftUp()
	{
		steer = 0f;
	}

	public void OnRightDown()
	{
		steer = 1f;
	}

	public void OnRightUp()
	{
		steer = 0f;
	}

	public void StopAllActions()
	{
		carController.accel = accel;
		carController.brake = brake;
		carController.steer = steer;
		carController.ebrake = ebrake;
		OnAccelUp();
		OnBrakeUp();
		OnRightUp();
		OnLeftUp();
	}

	private void OnDisable()
	{
		StopAllActions();
	}

	public void ShootBtnDown()
	{
		carController.StartShoot(0f);
	}

	public void ShootBtnUp()
	{
		carController.StopShooting();
	}
}
