using UnityEngine.UI;

public class MultiplayerConnectScreen : BaseScreen
{
	public static MultiplayerConnectScreen instance;

	public Text log;

	public override void Awake()
	{
		instance = this;
	}

	private void Start()
	{
	}

	public override void Update()
	{
	}

	public void PrintPhotonLog(string logStr)
	{
		log.text = logStr;
	}
}
