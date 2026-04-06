using UnityEngine;

public class JetPack : MonoBehaviour
{
	private bool isRunning;

	private float fuelCount = 20f;

	public ParticleSystem[] jetFlames;

	public CharacterMotor myPlayer;

	private void Awake()
	{
		myPlayer = GetComponentInParent<CharacterMotor>();
		myPlayer.myJetPack = this;
	}

	public void StartFlight()
	{
		if (fuelCount > 0f)
		{
			myPlayer.OnStartJetFlying();
			isRunning = true;
		}
		else
		{
			GameMessageLogger.instance.LogMessage("Fuel have ended!");
		}
	}

	public void StopFlight()
	{
		isRunning = false;
		myPlayer.OnStopJetFlying();
	}

	public void StartEngine()
	{
		for (int i = 0; i < jetFlames.Length; i++)
		{
			if (jetFlames[i] != null)
			{
				jetFlames[i].Play();
			}
		}
	}

	public void StopEngine()
	{
		for (int i = 0; i < jetFlames.Length; i++)
		{
			if (jetFlames[i] != null)
			{
				jetFlames[i].Stop();
			}
		}
	}

	private void Update()
	{
		if (!(fuelCount < 0f) && isRunning && myPlayer.photonView.isMine)
		{
			GameController.instance.OurPlayer.vertVelocity = 6.5f;
			UpdateUI();
		}
	}

	public void AddFuel(float count)
	{
		fuelCount += count;
		UpdateUI();
	}

	private void UpdateUI()
	{
		GameWindow.instance.jetpackFuelIndicator.fillAmount = fuelCount * 0.05f;
	}
}
