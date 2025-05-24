using UnityEngine;

public class ShowStatusWhenConnecting : MonoBehaviour
{
	public GUISkin Skin;

	private void OnGUI()
	{
		if (Skin != null)
		{
			GUI.skin = Skin;
		}
		float num = 400f;
		float num2 = 100f;
		GUILayout.BeginArea(new Rect(((float)Screen.width - num) / 2f, ((float)Screen.height - num2) / 2f, num, num2), GUI.skin.box);
		GUILayout.Label("Connecting" + GetConnectingDots(), GUI.skin.customStyles[0]);
		GUILayout.Label("Status: " + PhotonNetwork.connectionStateDetailed);
		GUILayout.EndArea();
		if (PhotonNetwork.inRoom)
		{
			base.enabled = false;
		}
	}

	private string GetConnectingDots()
	{
		string text = string.Empty;
		int num = Mathf.FloorToInt(Time.timeSinceLevelLoad * 3f % 4f);
		for (int i = 0; i < num; i++)
		{
			text += " .";
		}
		return text;
	}
}
