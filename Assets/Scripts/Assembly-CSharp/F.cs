using UnityEngine;

public static class F
{
	public static float MaxAbs(params float[] nums)
	{
		float num = 0f;
		for (int i = 0; i < nums.Length; i++)
		{
			if (Mathf.Abs(nums[i]) > Mathf.Abs(num))
			{
				num = nums[i];
			}
		}
		return num;
	}

	public static Component GetTopmostParentComponent<T>(Transform tr) where T : Component
	{
		Component result = null;
		while (tr.parent != null)
		{
			if (tr.parent.GetComponent<T>() != null)
			{
				result = tr.parent.GetComponent<T>();
			}
			tr = tr.parent;
		}
		return result;
	}
}
