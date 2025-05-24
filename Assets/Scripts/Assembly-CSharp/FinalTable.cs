using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalTable : MonoBehaviour
{
	public RectTransform scrollContent;

	public GameObject finalTableRowPrefab;

	public Text totalExpLabel;

	public Text totalCoinsLabel;

	[SerializeField]
	private GameObject lockPanel;

	public void Show()
	{
		base.gameObject.SetActive(value: true);
		MakeTable();
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	private void MakeTable()
	{
		lockPanel.SetActive(value: true);
		int num = 0;
		List<CharacterMotor> list = new List<CharacterMotor>();
		foreach (CharacterMotor player in GameController.instance.Players)
		{
			list.Add(player);
		}
		list.Sort((CharacterMotor p1, CharacterMotor p2) => p2.fragsCount.CompareTo(p1.fragsCount));
		float fragsSum = 0f;
		list.ForEach(delegate(CharacterMotor p)
		{
			fragsSum += p.fragsCount;
		});
		if (fragsSum == 0f)
		{
			fragsSum = 1f;
		}
		float num2 = list.Count * 5;
		GeneralUtils.RemoveAllChilds(scrollContent);
		int num3 = 1;
		foreach (CharacterMotor item in list)
		{
			TeamPlayerRowFinal component = Object.Instantiate(finalTableRowPrefab).GetComponent<TeamPlayerRowFinal>();
			component.transform.SetParent(scrollContent);
			component.transform.localScale = Vector3.one;
			component.SetName(item.playerInfo.name);
			component.SetKills(item.fragsCount);
			component.SetKillAssists(item.playerInfo.killAssistsCount);
			component.SetDeaths(item.playerInfo.deathCount);
			component.SetScore(item.playerInfo.score);
			if (item.photonView.isMine)
			{
				component.HighlightAsSelf();
				int num4 = (int)Mathf.Ceil((float)item.fragsCount / fragsSum * num2);
				if (num4 == 0)
				{
					num4 = 1;
				}
				num = num4;
				if (num3 == 1)
				{
					StorageController.instance.IncreaseWinsCount();
				}
				totalExpLabel.text = num.ToString();
				totalCoinsLabel.text = num4.ToString();
				StatisticsManager.AddExp(num);
				LocalStore.GiveMoney(num4);
			}
			num3++;
		}
		Invoke("ShowAds", 0.1f);
	}

	private void ShowAds()
	{
		lockPanel.SetActive(value: false);
	}

	public void OnLoadMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}

	public void OnPlayAgain()
	{
		lockPanel.SetActive(value: false);
		GameController.instance.OnClickPlayAgain();
	}
}
