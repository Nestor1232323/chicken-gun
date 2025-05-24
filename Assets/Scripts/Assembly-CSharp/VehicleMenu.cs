using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Demo Scripts/Vehicle Menu", 0)]
public class VehicleMenu : MonoBehaviour
{
	public CameraControl cam;

	public Vector3 spawnPoint;

	public Vector3 spawnRot;

	public GameObject[] vehicles;

	public GameObject chaseVehicle;

	public GameObject chaseVehicleDamage;

	private float chaseCarSpawnTime;

	private GameObject newVehicle;

	private bool autoShift;

	private bool assist;

	private bool stuntMode;

	public Toggle autoShiftToggle;

	public Toggle assistToggle;

	public Toggle stuntToggle;

	public Text speedText;

	public Text gearText;

	public Slider rpmMeter;

	public Slider boostMeter;

	public Text propertySetterText;

	public Text stuntText;

	public Text scoreText;

	public Toggle camToggle;

	private VehicleParent vp;

	private Motor engine;

	private Transmission trans;

	private GearboxTransmission gearbox;

	private ContinuousTransmission varTrans;

	private StuntDetect stunter;

	private float stuntEndTime = -1f;

	private PropertyToggleSetter propertySetter;

	private void Update()
	{
		autoShift = autoShiftToggle.isOn;
		assist = assistToggle.isOn;
		stuntMode = stuntToggle.isOn;
		cam.stayFlat = camToggle.isOn;
		chaseCarSpawnTime = Mathf.Max(0f, chaseCarSpawnTime - Time.deltaTime);
		if (!vp)
		{
			return;
		}
		speedText.text = (vp.velMag * 2.23694f).ToString("0") + " MPH";
		if ((bool)trans)
		{
			if ((bool)gearbox)
			{
				gearText.text = "Gear: " + ((gearbox.currentGear == 0) ? "R" : ((gearbox.currentGear != 1) ? (gearbox.currentGear - 1).ToString() : "N"));
			}
			else if ((bool)varTrans)
			{
				gearText.text = "Ratio: " + varTrans.currentRatio.ToString("0.00");
			}
		}
		if ((bool)engine)
		{
			rpmMeter.value = engine.targetPitch / (engine.maxPitch - engine.minPitch);
			if (engine.maxBoost > 0f)
			{
				boostMeter.value = engine.boost / engine.maxBoost;
			}
		}
		if (stuntMode && (bool)stunter)
		{
			stuntEndTime = ((!string.IsNullOrEmpty(stunter.stuntString)) ? 2f : Mathf.Max(0f, stuntEndTime - Time.deltaTime));
			if (stuntEndTime == 0f)
			{
				stuntText.text = string.Empty;
			}
			else if (!string.IsNullOrEmpty(stunter.stuntString))
			{
				stuntText.text = stunter.stuntString;
			}
			scoreText.text = "Score: " + stunter.score.ToString("n0");
		}
		if ((bool)propertySetter)
		{
			propertySetterText.text = ((propertySetter.currentPreset == 0) ? "Normal Steering" : ((propertySetter.currentPreset != 1) ? "Crab Steering" : "Skid Steering"));
		}
	}

	public void SpawnVehicle(int vehicle)
	{
		newVehicle = Object.Instantiate(vehicles[vehicle], spawnPoint, Quaternion.LookRotation(spawnRot, GlobalControl.worldUpDir));
		cam.target = newVehicle.transform;
		cam.Initialize();
		vp = newVehicle.GetComponent<VehicleParent>();
		trans = newVehicle.GetComponentInChildren<Transmission>();
		if ((bool)trans)
		{
			trans.automatic = autoShift;
			newVehicle.GetComponent<VehicleParent>().brakeIsReverse = autoShift;
			if (trans is GearboxTransmission)
			{
				gearbox = trans as GearboxTransmission;
			}
			else if (trans is ContinuousTransmission)
			{
				varTrans = trans as ContinuousTransmission;
				if (!autoShift)
				{
					vp.brakeIsReverse = true;
				}
			}
		}
		if ((bool)newVehicle.GetComponent<VehicleAssist>())
		{
			newVehicle.GetComponent<VehicleAssist>().enabled = assist;
		}
		if ((bool)newVehicle.GetComponent<FlipControl>() && (bool)newVehicle.GetComponent<StuntDetect>())
		{
			newVehicle.GetComponent<FlipControl>().flipPower = ((!stuntMode || !assist) ? Vector3.zero : new Vector3(10f, 10f, -10f));
			newVehicle.GetComponent<FlipControl>().rotationCorrection = (stuntMode ? Vector3.zero : ((!assist) ? Vector3.zero : new Vector3(5f, 1f, 10f)));
			newVehicle.GetComponent<FlipControl>().stopFlip = assist;
			stunter = newVehicle.GetComponent<StuntDetect>();
		}
		engine = newVehicle.GetComponentInChildren<Motor>();
		propertySetter = newVehicle.GetComponent<PropertyToggleSetter>();
		stuntText.gameObject.SetActive(stuntMode);
		scoreText.gameObject.SetActive(stuntMode);
	}

	public void SpawnChaseVehicle()
	{
		if (chaseCarSpawnTime == 0f)
		{
			chaseCarSpawnTime = 1f;
			Object.Instantiate(chaseVehicle, spawnPoint, Quaternion.LookRotation(spawnRot, GlobalControl.worldUpDir)).GetComponent<FollowAI>().target = newVehicle.transform;
		}
	}

	public void SpawnChaseVehicleDamage()
	{
		if (chaseCarSpawnTime == 0f)
		{
			chaseCarSpawnTime = 1f;
			Object.Instantiate(chaseVehicleDamage, spawnPoint, Quaternion.LookRotation(spawnRot, GlobalControl.worldUpDir)).GetComponent<FollowAI>().target = newVehicle.transform;
		}
	}
}
