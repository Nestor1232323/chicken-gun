using UnityEngine;
using UnityEngine.UI;

public class KickOutListRow : MonoBehaviour
{
	[SerializeField]
	private Text indexText;

	[SerializeField]
	private Text playerNameText;

	public void SetIndex(int val)
	{
		indexText.text = val.ToString();
	}

	public void SetPlayerName(string val)
	{
		playerNameText.text = val.ToString();
	}
}
