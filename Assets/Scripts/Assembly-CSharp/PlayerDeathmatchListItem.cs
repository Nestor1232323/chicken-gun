using UnityEngine;
using UnityEngine.UI;

public class PlayerDeathmatchListItem : MonoBehaviour
{
	[SerializeField]
	private Text playerName;

	[SerializeField]
	private Text score;

	[SerializeField]
	private Color highlightColor;

	public void SetName(string val)
	{
		playerName.text = val;
	}

	public void SetScore(int val)
	{
		score.text = val.ToString();
	}

	public void Highlight(bool h)
	{
		if (h)
		{
			playerName.transform.parent.GetComponent<Image>().color = highlightColor;
		}
	}
}
