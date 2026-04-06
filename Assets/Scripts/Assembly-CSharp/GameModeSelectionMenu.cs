using System;
using UnityEngine;

public class GameModeSelectionMenu : MonoBehaviour
{
	public Action<GameMode> OnGameModeSelected;

	private void Start()
	{
		DeathmatchSelected();
	}

	public void DeathmatchSelected()
	{
		if (OnGameModeSelected != null)
		{
			OnGameModeSelected(GameMode.PvP);
		}
	}

	public void TeamfightSelected()
	{
		if (OnGameModeSelected != null)
		{
			OnGameModeSelected(GameMode.TeamFight);
		}
	}

	public void BattleRoyalePvPSelected()
	{
		if (OnGameModeSelected != null)
		{
			OnGameModeSelected(GameMode.BattleRoyalePvP);
		}
	}

	public void BattleRoyaleTeamsSelected()
	{
		if (OnGameModeSelected != null)
		{
			OnGameModeSelected(GameMode.BattleRoyaleTeams);
		}
	}
}
