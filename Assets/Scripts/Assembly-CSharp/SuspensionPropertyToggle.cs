using UnityEngine;

[RequireComponent(typeof(Suspension))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Suspension/Suspension Property", 2)]
public class SuspensionPropertyToggle : MonoBehaviour
{
	public SuspensionToggledProperty[] properties;

	private Suspension sus;

	private void Start()
	{
		sus = GetComponent<Suspension>();
	}

	public void ToggleProperty(int index)
	{
		if (properties.Length - 1 >= index)
		{
			properties[index].toggled = !properties[index].toggled;
			if ((bool)sus)
			{
				sus.UpdateProperties();
			}
		}
	}

	public void SetProperty(int index, bool value)
	{
		if (properties.Length - 1 >= index)
		{
			properties[index].toggled = value;
			if ((bool)sus)
			{
				sus.UpdateProperties();
			}
		}
	}
}
