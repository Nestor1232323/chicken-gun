using UnityEngine;

public class HeliInputController : MonoBehaviour, IJoystickListener
{
	public JoystickController myJoystick;

	public Vector2 joystickDelta = Vector2.zero;

	public GameObject shootBtn;

	public HelicopterController myHeli;

	private void OnEnable()
	{
		myHeli = GameController.instance.OurPlayer.myCar as HelicopterController;
		myJoystick.myInputListener = this;
		shootBtn.SetActive(myHeli.Shootable());
	}

	public void SetDelta(Vector2 delta)
	{
		joystickDelta = delta;
		myHeli.SetDelta(delta);
	}

	public void ShootBtnDown()
	{
		myHeli.StartShoot(0f);
	}

	public void ShootBtnUp()
	{
		myHeli.StopShooting();
	}

	public void OnEngineUpBtnDown()
	{
		myHeli.SetLiftForce(1f);
	}

	public void OnEngineUpBtnUp()
	{
		myHeli.SetLiftForce(0f);
	}

	public void OnEngineDownBtnDown()
	{
		myHeli.SetLiftForce(-1f);
	}

	public void OnEngineDownBtnUp()
	{
		myHeli.SetLiftForce(0f);
	}
}
