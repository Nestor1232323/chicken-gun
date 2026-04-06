using UnityEngine;

public class InternetChecker : MonoBehaviour
{
	private const bool allowCarrierDataNetwork = false;

	private const string pingAddress = "8.8.8.8";

	private const float waitingTime = 2f;

	private Ping ping;

	private float pingStartTime;

	public static bool connectedToInternet;

	public void Start()
	{
		bool flag;
		switch (Application.internetReachability)
		{
		case NetworkReachability.ReachableViaLocalAreaNetwork:
			flag = true;
			break;
		case NetworkReachability.ReachableViaCarrierDataNetwork:
			flag = false;
			break;
		default:
			flag = false;
			break;
		}
		if (!flag)
		{
			InternetIsNotAvailable();
			return;
		}
		ping = new Ping("8.8.8.8");
		pingStartTime = Time.time;
	}

	public void Update()
	{
		if (ping != null)
		{
			bool flag = true;
			if (ping.isDone)
			{
				InternetAvailable();
			}
			else if (Time.time - pingStartTime < 2f)
			{
				flag = false;
			}
			else
			{
				InternetIsNotAvailable();
			}
			if (flag)
			{
				ping = null;
			}
		}
	}

	private void InternetIsNotAvailable()
	{
		connectedToInternet = false;
		Debug.Log("No Internet :(");
	}

	private void InternetAvailable()
	{
		connectedToInternet = true;
		Debug.Log("Internet is available! ;)");
	}
}
