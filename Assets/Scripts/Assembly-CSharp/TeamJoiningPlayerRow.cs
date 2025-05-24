using UnityEngine;
using UnityEngine.UI;

public class TeamJoiningPlayerRow : MonoBehaviour
{
	public Text playerName;

	public Text fragsCount;

	public void SetPlayerName(string name)
	{
		playerName.text = name;
	}

	public void SetFragsCount(int fragsCount)
	{
		this.fragsCount.text = fragsCount.ToString();
	}
}
