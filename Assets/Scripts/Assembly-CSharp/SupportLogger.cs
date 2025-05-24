using UnityEngine;

public class SupportLogger : MonoBehaviour
{
	public bool LogTrafficStats = true;

	public void Start()
	{
		if (GameObject.Find("PunSupportLogger") == null)
		{
			GameObject obj = new GameObject("PunSupportLogger");
			Object.DontDestroyOnLoad(obj);
			obj.AddComponent<SupportLogging>().LogTrafficStats = LogTrafficStats;
		}
	}
}
