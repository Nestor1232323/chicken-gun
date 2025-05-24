using UnityEngine;
using UnityEngine.UI;

public class GameMessageLogger : MonoBehaviour
{
	[SerializeField]
	private Text logTest;

	public static GameMessageLogger instance;

	private void Awake()
	{
		instance = this;
	}

	public void LogMessage(string msg)
	{
		logTest.text = msg;
		CancelInvoke("Clear");
		Invoke("Clear", 3f);
	}

	private void Clear()
	{
		logTest.text = string.Empty;
	}
}
