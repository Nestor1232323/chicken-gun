using UnityEngine;

public static class ExtensionsMethods
{
	public static void SetPositionX(this Transform t, float newX)
	{
		t.position = new Vector3(newX, t.position.y, t.position.z);
	}

	public static void SetPositionY(this Transform t, float val)
	{
		t.position = new Vector3(t.position.x, val, t.position.z);
	}

	public static void SetPositionZ(this Transform t, float newZ)
	{
		t.position = new Vector3(t.position.x, t.position.y, newZ);
	}

	public static void SetLocalPositionX(this Transform t, float val)
	{
		t.localPosition = new Vector3(val, t.localPosition.y, t.localPosition.z);
	}

	public static void SetLocalPositionY(this Transform t, float val)
	{
		t.localPosition = new Vector3(t.localPosition.x, val, t.localPosition.z);
	}

	public static void SetLocalPositionZ(this Transform t, float val)
	{
		t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, val);
	}

	public static void SetLocalEulerX(this Transform t, float newX)
	{
		t.localEulerAngles = new Vector3(newX, t.localEulerAngles.y, t.localEulerAngles.z);
	}

	public static void SetLocalEulerY(this Transform t, float newY)
	{
		t.localEulerAngles = new Vector3(t.localEulerAngles.x, newY, t.localEulerAngles.z);
	}

	public static void SetLocalEulerZ(this Transform t, float newZ)
	{
		t.localEulerAngles = new Vector3(t.localEulerAngles.x, t.localEulerAngles.y, newZ);
	}

	public static void SetEulerX(this Transform t, float val)
	{
		t.eulerAngles = new Vector3(val, t.eulerAngles.y, t.eulerAngles.z);
	}

	public static void SetEulerY(this Transform t, float val)
	{
		t.eulerAngles = new Vector3(t.eulerAngles.x, val, t.eulerAngles.z);
	}

	public static void SetEulerZ(this Transform t, float val)
	{
		t.eulerAngles = new Vector3(t.eulerAngles.x, t.eulerAngles.y, val);
	}

	public static void SetLocalScaleX(this Transform t, float newX)
	{
		t.localScale = new Vector3(newX, t.localScale.y, t.localScale.z);
	}

	public static void SetLocalScaleY(this Transform t, float newY)
	{
		t.localScale = new Vector3(t.localScale.x, newY, t.localScale.z);
	}

	public static void SetLocalScaleZ(this Transform t, float newZ)
	{
		t.localScale = new Vector3(t.localScale.x, t.localScale.y, newZ);
	}
}
