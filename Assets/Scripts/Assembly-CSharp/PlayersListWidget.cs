using System.Collections.Generic;
using UnityEngine;

public class PlayersListWidget : MonoBehaviour
{
	[SerializeField]
	private Transform totalListContainer;

	[SerializeField]
	private Transform myteamListContainer;

	[SerializeField]
	private Transform enemyListContainer;

	private List<GameObject> rows = new List<GameObject>();

	[SerializeField]
	private GameObject playerListRowItem;

	public void ShowPlayersList()
	{
		if (GameController.gameConfigData.gameMode == GameMode.TeamFight)
		{
			totalListContainer.parent.parent.gameObject.SetActive(value: false);
			myteamListContainer.parent.parent.gameObject.SetActive(value: true);
			enemyListContainer.parent.parent.gameObject.SetActive(value: true);
		}
		else
		{
			totalListContainer.parent.parent.gameObject.SetActive(value: true);
			myteamListContainer.parent.parent.gameObject.SetActive(value: false);
			enemyListContainer.parent.parent.gameObject.SetActive(value: false);
		}
		base.gameObject.SetActive(value: true);
		for (int num = rows.Count - 1; num >= 0; num--)
		{
			Object.Destroy(rows[num]);
		}
		rows.Clear();
		List<CharacterMotor> list = GameController.instance.AllPlayersNotDublicated();
		list.Sort((CharacterMotor x, CharacterMotor y) => y.fragsCount.CompareTo(x.fragsCount));
		foreach (CharacterMotor item in list)
		{
			PlayersRowItem component = Object.Instantiate(playerListRowItem).GetComponent<PlayersRowItem>();
			if (GameController.gameConfigData.gameMode == GameMode.TeamFight)
			{
				if (item.myTeam == TeamID.TeamA)
				{
					component.transform.SetParent(myteamListContainer);
				}
				else
				{
					component.transform.SetParent(enemyListContainer);
				}
			}
			else
			{
				component.transform.SetParent(totalListContainer);
			}
			component.transform.localScale = Vector3.one;
			component.SetPlayerName(item.playerInfo.name);
			component.SetScore(item.playerInfo.score);
			rows.Add(component.gameObject);
			if (item == GameController.instance.OurPlayer)
			{
				component.HighlightAsSelf();
			}
		}
	}

	public void Close()
	{
		base.gameObject.SetActive(value: false);
	}
}
