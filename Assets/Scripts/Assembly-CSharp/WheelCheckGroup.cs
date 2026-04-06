using System;

[Serializable]
public class WheelCheckGroup
{
	public Wheel[] wheels;

	public HoverWheel[] hoverWheels;

	public void Activate()
	{
		Wheel[] array = wheels;
		foreach (Wheel wheel in array)
		{
			wheel.getContact = true;
		}
		HoverWheel[] array2 = hoverWheels;
		foreach (HoverWheel hoverWheel in array2)
		{
			hoverWheel.getContact = true;
		}
	}

	public void Deactivate()
	{
		Wheel[] array = wheels;
		foreach (Wheel wheel in array)
		{
			wheel.getContact = false;
		}
		HoverWheel[] array2 = hoverWheels;
		foreach (HoverWheel hoverWheel in array2)
		{
			hoverWheel.getContact = false;
		}
	}
}
