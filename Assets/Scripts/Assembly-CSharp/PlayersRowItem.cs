using UnityEngine;
using UnityEngine.UI;

public class PlayersRowItem : MonoBehaviour
{
	[SerializeField]
	private Text playerName;

	[SerializeField]
	private Text score;

	[SerializeField]
	private Color highlightColor;

	public void SetPlayerName(string val)
	{
		playerName.text = val;
	}

	public void SetScore(int val)
	{
		score.text = val.ToString();
	}

	public void HighlightAsSelf()
	{
		GetComponent<Image>().color = highlightColor;
	}
}
