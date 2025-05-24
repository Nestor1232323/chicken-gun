using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonStatsGui : MonoBehaviour
{
	public bool statsWindowOn = true;

	public bool statsOn = true;

	public bool healthStatsVisible;

	public bool trafficStatsOn;

	public bool buttonsOn;

	public Rect statsRect = new Rect(0f, 100f, 200f, 50f);

	public int WindowId = 100;

	public void Start()
	{
		if (statsRect.x <= 0f)
		{
			statsRect.x = (float)Screen.width - statsRect.width;
		}
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
		{
			statsWindowOn = !statsWindowOn;
			statsOn = true;
		}
	}

	public void OnGUI()
	{
		if (PhotonNetwork.networkingPeer.TrafficStatsEnabled != statsOn)
		{
			PhotonNetwork.networkingPeer.TrafficStatsEnabled = statsOn;
		}
		if (statsWindowOn)
		{
			statsRect = GUILayout.Window(WindowId, statsRect, TrafficStatsWindow, "Messages (shift+tab)");
		}
	}

	public void TrafficStatsWindow(int windowID)
	{
		bool flag = false;
		TrafficStatsGameLevel trafficStatsGameLevel = PhotonNetwork.networkingPeer.TrafficStatsGameLevel;
		long num = PhotonNetwork.networkingPeer.TrafficStatsElapsedMs / 1000;
		if (num == 0L)
		{
			num = 1L;
		}
		GUILayout.BeginHorizontal();
		buttonsOn = GUILayout.Toggle(buttonsOn, "buttons");
		healthStatsVisible = GUILayout.Toggle(healthStatsVisible, "health");
		trafficStatsOn = GUILayout.Toggle(trafficStatsOn, "traffic");
		GUILayout.EndHorizontal();
		string text = $"Out {trafficStatsGameLevel.TotalOutgoingMessageCount,4} | In {trafficStatsGameLevel.TotalIncomingMessageCount,4} | Sum {trafficStatsGameLevel.TotalMessageCount,4}";
		string text2 = $"{num}sec average:";
		string text3 = $"Out {trafficStatsGameLevel.TotalOutgoingMessageCount / num,4} | In {trafficStatsGameLevel.TotalIncomingMessageCount / num,4} | Sum {trafficStatsGameLevel.TotalMessageCount / num,4}";
		GUILayout.Label(text);
		GUILayout.Label(text2);
		GUILayout.Label(text3);
		if (buttonsOn)
		{
			GUILayout.BeginHorizontal();
			statsOn = GUILayout.Toggle(statsOn, "stats on");
			if (GUILayout.Button("Reset"))
			{
				PhotonNetwork.networkingPeer.TrafficStatsReset();
				PhotonNetwork.networkingPeer.TrafficStatsEnabled = true;
			}
			flag = GUILayout.Button("To Log");
			GUILayout.EndHorizontal();
		}
		string text4 = string.Empty;
		string text5 = string.Empty;
		if (trafficStatsOn)
		{
			GUILayout.Box("Traffic Stats");
			text4 = "Incoming: \n" + PhotonNetwork.networkingPeer.TrafficStatsIncoming.ToString();
			text5 = "Outgoing: \n" + PhotonNetwork.networkingPeer.TrafficStatsOutgoing.ToString();
			GUILayout.Label(text4);
			GUILayout.Label(text5);
		}
		string text6 = string.Empty;
		if (healthStatsVisible)
		{
			GUILayout.Box("Health Stats");
			text6 = string.Format("ping: {6}[+/-{7}]ms resent:{8} \n\nmax ms between\nsend: {0,4} \ndispatch: {1,4} \n\nlongest dispatch for: \nev({3}):{2,3}ms \nop({5}):{4,3}ms", trafficStatsGameLevel.LongestDeltaBetweenSending, trafficStatsGameLevel.LongestDeltaBetweenDispatching, trafficStatsGameLevel.LongestEventCallback, trafficStatsGameLevel.LongestEventCallbackCode, trafficStatsGameLevel.LongestOpResponseCallback, trafficStatsGameLevel.LongestOpResponseCallbackOpCode, PhotonNetwork.networkingPeer.RoundTripTime, PhotonNetwork.networkingPeer.RoundTripTimeVariance, PhotonNetwork.networkingPeer.ResentReliableCommands);
			GUILayout.Label(text6);
		}
		if (flag)
		{
			Debug.Log($"{text}\n{text2}\n{text3}\n{text4}\n{text5}\n{text6}");
		}
		if (GUI.changed)
		{
			statsRect.height = 100f;
		}
		GUI.DragWindow();
	}
}
