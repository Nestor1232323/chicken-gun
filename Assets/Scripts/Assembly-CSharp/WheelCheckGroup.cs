using System;

[Serializable]
public class WheelCheckGroup
{
	public Wheel[] wheels;

	public HoverWheel[] hoverWheels;

	public void Activate()
	{
		Wheel[] array = wheels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].getContact = true;
		}
		HoverWheel[] array2 = hoverWheels;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].getContact = true;
		}
	}

	public void Deactivate()
	{
		Wheel[] array = wheels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].getContact = false;
		}
		HoverWheel[] array2 = hoverWheels;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].getContact = false;
		}
	}
}
