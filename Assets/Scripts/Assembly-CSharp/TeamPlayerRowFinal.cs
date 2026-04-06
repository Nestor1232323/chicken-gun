using UnityEngine;
using UnityEngine.UI;

public class TeamPlayerRowFinal : MonoBehaviour
{
	[SerializeField]
	private Image playerIcon;

	[SerializeField]
	private Text playerName;

	[SerializeField]
	private Text kills;

	[SerializeField]
	private Text killAssists;

	[SerializeField]
	private Text deaths;

	[SerializeField]
	private Text score;

	[SerializeField]
	private Image background;

	[SerializeField]
	private Color highlightColor;

	public void SetName(string val)
	{
		playerName.text = val;
	}

	public void SetKills(int val)
	{
		kills.text = val.ToString();
	}

	public void SetKillAssists(int val)
	{
		killAssists.text = val.ToString();
	}

	public void SetDeaths(int val)
	{
		deaths.text = val.ToString();
	}

	public void SetScore(int val)
	{
		score.text = val.ToString();
	}

	public void SetIcon(Sprite val)
	{
		playerIcon.sprite = val;
	}

	public void HighlightAsSelf()
	{
		background.color = highlightColor;
		playerName.fontStyle = FontStyle.Bold;
		kills.fontStyle = FontStyle.Bold;
		killAssists.fontStyle = FontStyle.Bold;
		score.fontStyle = FontStyle.Bold;
		deaths.fontStyle = FontStyle.Bold;
	}
}
