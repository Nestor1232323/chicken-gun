using UnityEngine;

[AddComponentMenu("RVP/C#/Suspension/Suspension Property Setter", 3)]
public class PropertyToggleSetter : MonoBehaviour
{
	[Tooltip("Steering Controller")]
	public SteeringControl steerer;

	public Transmission transmission;

	[Tooltip("Suspensions with properties to be toggled")]
	public SuspensionPropertyToggle[] suspensionProperties;

	public PropertyTogglePreset[] presets;

	public int currentPreset;

	[Tooltip("Input manager button which increments the preset")]
	public string changeButton;

	private void Update()
	{
		if (!string.IsNullOrEmpty(changeButton) && Input.GetButtonDown(changeButton))
		{
			ChangePreset(currentPreset + 1);
		}
	}

	public void ChangePreset(int preset)
	{
		currentPreset = preset % presets.Length;
		if ((bool)steerer)
		{
			steerer.limitSteer = presets[currentPreset].limitSteer;
		}
		if ((bool)transmission)
		{
			transmission.skidSteerDrive = presets[currentPreset].skidSteerTransmission;
		}
		for (int i = 0; i < suspensionProperties.Length; i++)
		{
			for (int j = 0; j < suspensionProperties[i].properties.Length; j++)
			{
				suspensionProperties[i].SetProperty(j, presets[currentPreset].wheels[i].preset[j]);
			}
		}
	}
}
