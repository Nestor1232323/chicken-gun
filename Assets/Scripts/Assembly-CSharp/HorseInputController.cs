using UnityEngine;

public class HorseInputController : MonoBehaviour
{
	private HorseController horseController;

	public float accel;

	public float steer;

	private void Start()
	{
	}

	private void OnEnable()
	{
		horseController = GameController.instance.OurPlayer.myCar as HorseController;
	}

	private void Update()
	{
		if (!GameController.isMobile)
		{
			float axis = Input.GetAxis("Vertical");
			accel = axis;
			steer = Input.GetAxis("Horizontal");
		}
		horseController.SetAccel(accel);
		horseController.SetSteer(steer);
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
		accel = -1f;
	}

	public void OnBrakeUp()
	{
		accel = 0f;
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

	public void ShootBtnDown()
	{
		horseController.StartShoot(0f);
	}

	public void ShootBtnUp()
	{
		horseController.StopShooting();
	}
}
