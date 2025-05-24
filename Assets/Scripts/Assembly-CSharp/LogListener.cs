using UnityEngine;
using UnityEngine.UI;

public class LogListener : MonoBehaviour
{
	public Text log;

	private string logStr;

	public string output = string.Empty;

	public string stack = string.Empty;

	private void OnEnable()
	{
		Application.logMessageReceived += HandleLog;
	}

	private void OnDisable()
	{
		Application.logMessageReceived -= HandleLog;
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		output = logString;
		stack = stackTrace;
		logStr = logStr + "\n" + logString;
		log.text = logStr;
	}
}
