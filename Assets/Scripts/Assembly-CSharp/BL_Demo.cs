using UnityEngine;
using UnityEngine.UI;

public class BL_Demo : MonoBehaviour
{
	public int[] altRateOfFire = new int[5] { 60, 120, 240, 480, 960 };

	public int currentRateOfFire = 2;

	public float[] altVelocity = new float[6] { 50f, 100f, 200f, 400f, 800f, 1600f };

	public int currentVelocity = 3;

	public GameObject[] altBulletType;

	public int currentBulletType;

	public GameObject[] altImpactType;

	public int currentImpactType;

	public GameObject[] altMuzzleFlashType;

	public int currentMuzzleFlashType;

	public AudioClip[] altSFX;

	public int currentSFX;

	public Transform[] altCameraPosition;

	public int currentCameraPosition;

	public bool sequential = true;

	public bool lockY = true;

	public Text uiRateOfFire;

	public Text uiVelocity;

	public Text uiSFX;

	public Text uiSequential;

	public Text uiLockY;

	private BL_Turret[] _turrets;

	private void Start()
	{
		_turrets = Object.FindObjectsOfType<BL_Turret>();
		Refresh(_repoolBullets: true);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.W))
		{
			currentRateOfFire++;
			if (currentRateOfFire >= altRateOfFire.Length)
			{
				currentRateOfFire = altRateOfFire.Length - 1;
			}
			Refresh();
		}
		if (Input.GetKeyDown(KeyCode.Q))
		{
			currentRateOfFire--;
			if (currentRateOfFire < 0)
			{
				currentRateOfFire = 0;
			}
			Refresh();
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			currentVelocity++;
			if (currentVelocity >= altVelocity.Length)
			{
				currentVelocity = altVelocity.Length - 1;
			}
			Refresh();
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			currentVelocity--;
			if (currentVelocity < 0)
			{
				currentVelocity = 0;
			}
			Refresh();
		}
		if (Input.GetKeyDown(KeyCode.PageDown) || Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			currentSFX++;
			if (currentSFX >= altSFX.Length)
			{
				currentSFX = altSFX.Length - 1;
			}
			currentBulletType++;
			if (currentBulletType >= altBulletType.Length)
			{
				currentBulletType = 0;
			}
			currentImpactType++;
			if (currentImpactType >= altImpactType.Length)
			{
				currentImpactType = 0;
			}
			currentMuzzleFlashType++;
			if (currentMuzzleFlashType >= altMuzzleFlashType.Length)
			{
				currentMuzzleFlashType = 0;
			}
			Refresh(_repoolBullets: true);
		}
		if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			currentSFX--;
			if (currentSFX < 0)
			{
				currentSFX = 0;
			}
			currentBulletType--;
			if (currentBulletType < 0)
			{
				currentBulletType = altBulletType.Length - 1;
			}
			currentImpactType--;
			if (currentImpactType < 0)
			{
				currentImpactType = altImpactType.Length - 1;
			}
			currentMuzzleFlashType--;
			if (currentMuzzleFlashType < 0)
			{
				currentMuzzleFlashType = altMuzzleFlashType.Length - 1;
			}
			Refresh(_repoolBullets: true);
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			currentCameraPosition++;
			if (currentCameraPosition >= altCameraPosition.Length)
			{
				currentCameraPosition = 0;
			}
			Refresh();
		}
		if (Input.GetKeyDown(KeyCode.Z))
		{
			if (sequential)
			{
				sequential = false;
			}
			else
			{
				sequential = true;
			}
			BL_Turret[] turrets = _turrets;
			for (int i = 0; i < turrets.Length; i++)
			{
				turrets[i].fireSequential = sequential;
			}
			Refresh();
		}
		if (Input.GetKeyDown(KeyCode.X))
		{
			if (lockY)
			{
				lockY = false;
			}
			else
			{
				lockY = true;
			}
			BL_Turret[] turrets = _turrets;
			for (int i = 0; i < turrets.Length; i++)
			{
				turrets[i].lockYAxis = lockY;
			}
			Refresh();
		}
	}

	private void Refresh(bool _repoolBullets = false)
	{
		BL_Turret[] turrets = _turrets;
		foreach (BL_Turret bL_Turret in turrets)
		{
			bL_Turret.rateOfFire = altRateOfFire[currentRateOfFire];
			bL_Turret.bulletVelocity = altVelocity[currentVelocity];
			if (_repoolBullets)
			{
				bL_Turret.SetBullet(altBulletType[currentBulletType]);
				bL_Turret.SetMuzzleFlash(altMuzzleFlashType[currentMuzzleFlashType]);
				bL_Turret.SetImpact(altImpactType[currentImpactType]);
			}
			if (bL_Turret.sfxFire.Length != 0)
			{
				bL_Turret.sfxFire[0] = altSFX[currentSFX];
			}
		}
		uiRateOfFire.text = "Q/W RATE OF FIRE: " + altRateOfFire[currentRateOfFire];
		uiVelocity.text = "A/S VELOCITY: " + altVelocity[currentVelocity];
		uiSequential.text = "Z SEQUENTIAL: " + sequential.ToString().ToUpper();
		uiLockY.text = "X LOCK Y-AXIS: " + lockY.ToString().ToUpper();
		uiSFX.text = "MOUSE WHEEL SFX: " + altSFX[currentSFX].name.ToUpper();
		Camera.main.transform.position = altCameraPosition[currentCameraPosition].position;
		Camera.main.transform.rotation = altCameraPosition[currentCameraPosition].rotation;
	}
}
