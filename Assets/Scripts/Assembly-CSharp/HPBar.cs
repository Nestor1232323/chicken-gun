using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
	public Text playerName;

	public RectTransform HPLine;

	public Text teamName;

	public Image teamIcon;

	public CharacterMotor characterMotor;

	private float initWidth;

	private void Start()
	{
		initWidth = HPLine.sizeDelta.x;
	}

	private void Update()
	{
		if (Camera.main != null)
		{
			base.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
		}
	}

	public void SetPlayerName(string name)
	{
		playerName.text = name;
	}

	public void SetTeamName(TeamID t)
	{
		if (t == TeamID.None)
		{
			teamName.gameObject.SetActive(value: false);
			teamIcon.gameObject.SetActive(value: false);
		}
		else
		{
			teamName.text = MultiplayerController.instance.GetTeamCustomName(t);
			teamIcon.sprite = DataModel.instance.TeamIcon(t);
		}
		teamName.color = DataModel.instance.TeamColor(t);
	}

	public void UpdateHP()
	{
		if (!(characterMotor == null))
		{
			initWidth = HPLine.parent.GetComponent<RectTransform>().sizeDelta.x;
			HPLine.sizeDelta = new Vector2(initWidth * characterMotor.HP / (float)characterMotor.playerInfo.max_hp, HPLine.sizeDelta.y);
		}
	}

	public void Show(bool show)
	{
		base.gameObject.SetActive(show);
	}
}
