using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHUD : MonoBehaviour
{
	public int id;

	[SerializeField]
	private Text playerName;

	[SerializeField]
	private Image hpLine;

	public CharacterMotor player;

	public RectTransform rectTransform;

	public Transform target;

	public void SetTeam(TeamID team)
	{
		playerName.color = GameController.instance.GetTeamColor(team);
		hpLine.color = GameController.instance.GetTeamColor(team);
	}

	public void SetPlayerName(string plName)
	{
		playerName.text = plName;
	}

	public void UpdateHP_K(float k)
	{
		hpLine.rectTransform.anchoredPosition = new Vector2((0f - (1f - k)) * hpLine.rectTransform.sizeDelta.x, hpLine.rectTransform.anchoredPosition.y);
	}

	public void ShowHPLine(bool show)
	{
		hpLine.transform.parent.gameObject.SetActive(show);
	}
}
