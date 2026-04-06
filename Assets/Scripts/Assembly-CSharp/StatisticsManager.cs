using System;

public class StatisticsManager
{
	public static Action ExpChanged;

	public static void AddExp(int val)
	{
		DataModel.AddExp(val);
		if (ExpChanged != null)
		{
			ExpChanged();
		}
	}
}
