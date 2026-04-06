using System.Collections.Generic;
using UnityEngine;

public class ArenaScript : MonoBehaviour
{
	public static ArenaScript instance;

	[SerializeField]
	private Transform RespawnPointsContainer;

	[SerializeField]
	private Transform TeamAPointsContainer;

	[SerializeField]
	private Transform TeamBPointsContainer;

	[SerializeField]
	private Transform ZombieRespawnPointsContainer;

	public Transform BonusPointsContainer;

	[SerializeField]
	private Transform BattleRoyalePvPLobbyPointsContainer;

	[SerializeField]
	private Transform BattleRoyaleTeamALobbyPointsContainer;

	[SerializeField]
	private Transform BattleRoyaleTeamBLobbyPointsContainer;

	[HideInInspector]
	public Transform[] RespawnPoints;

	[HideInInspector]
	public Transform[] TeanARespawnPoints;

	[HideInInspector]
	public Transform[] TeanBRespawnPoints;

	[HideInInspector]
	public Transform[] BattleRoyalePvPLobbyPoints;

	[HideInInspector]
	public Transform[] BattleRoyaleTeamALobbyPoints;

	[HideInInspector]
	public Transform[] BattleRoyaleTeamBLobbyPoints;

	[HideInInspector]
	public Transform[] ZombieSpawnPoints;

	public Transform HiddenPoint;

	public int maxInvObjectsCount = 100;

	public int maxVehiclesCount = 10;

	public bool isNavMeshArena;

	[SerializeField]
	protected List<InventoryCategoryType> allowedInvCategories;

	private void Awake()
	{
		instance = this;
		RespawnPoints = new Transform[RespawnPointsContainer.childCount];
		for (int i = 0; i < RespawnPointsContainer.childCount; i++)
		{
			RespawnPoints[i] = RespawnPointsContainer.GetChild(i);
		}
		TeanARespawnPoints = new Transform[TeamAPointsContainer.childCount];
		for (int j = 0; j < TeamAPointsContainer.childCount; j++)
		{
			TeanARespawnPoints[j] = TeamAPointsContainer.GetChild(j);
		}
		TeanBRespawnPoints = new Transform[TeamBPointsContainer.childCount];
		for (int k = 0; k < TeamBPointsContainer.childCount; k++)
		{
			TeanBRespawnPoints[k] = TeamBPointsContainer.GetChild(k);
		}
		if (BattleRoyalePvPLobbyPointsContainer != null)
		{
			BattleRoyalePvPLobbyPoints = new Transform[BattleRoyalePvPLobbyPointsContainer.childCount];
			for (int l = 0; l < BattleRoyalePvPLobbyPointsContainer.childCount; l++)
			{
				BattleRoyalePvPLobbyPoints[l] = BattleRoyalePvPLobbyPointsContainer.GetChild(l);
			}
		}
		if (BattleRoyaleTeamALobbyPointsContainer != null)
		{
			BattleRoyaleTeamALobbyPoints = new Transform[BattleRoyaleTeamALobbyPointsContainer.childCount];
			for (int m = 0; m < BattleRoyaleTeamALobbyPointsContainer.childCount; m++)
			{
				BattleRoyaleTeamALobbyPoints[m] = BattleRoyaleTeamALobbyPointsContainer.GetChild(m);
			}
		}
		if (BattleRoyaleTeamBLobbyPointsContainer != null)
		{
			BattleRoyaleTeamBLobbyPoints = new Transform[BattleRoyaleTeamBLobbyPointsContainer.childCount];
			for (int n = 0; n < BattleRoyaleTeamBLobbyPointsContainer.childCount; n++)
			{
				BattleRoyaleTeamBLobbyPoints[n] = BattleRoyaleTeamBLobbyPointsContainer.GetChild(n);
			}
		}
		if (ZombieRespawnPointsContainer != null)
		{
			ZombieSpawnPoints = new Transform[ZombieRespawnPointsContainer.childCount];
			for (int num = 0; num < ZombieRespawnPointsContainer.childCount; num++)
			{
				ZombieSpawnPoints[num] = ZombieRespawnPointsContainer.GetChild(num);
			}
		}
	}

	private void Start()
	{
	}

	public Transform GetRandomRespawnPoint()
	{
		return RespawnPoints[Random.Range(0, RespawnPoints.Length)];
	}

	public Transform GetHiddedRespawnPoint()
	{
		return HiddenPoint;
	}

	public Transform GetTeamRespawnPoint(TeamID team)
	{
		if (team == TeamID.TeamA)
		{
			return TeanARespawnPoints[Random.Range(0, TeanARespawnPoints.Length)];
		}
		int num = Random.Range(0, TeanBRespawnPoints.Length);
		return TeanBRespawnPoints[num];
	}

	public bool IsCategoryAllowed(InventoryCategoryType ctg)
	{
		if (allowedInvCategories.Count == 0)
		{
			return true;
		}
		return allowedInvCategories.FindIndex((InventoryCategoryType c) => c == ctg) != -1;
	}

	public Transform GetBattleRoyalePvPLobbyPoint()
	{
		return BattleRoyalePvPLobbyPoints[Random.Range(0, BattleRoyalePvPLobbyPoints.Length)];
	}

	public Transform GetBattleRoyaleTeamsLobbyPoint(TeamID team)
	{
		if (team == TeamID.TeamA)
		{
			return BattleRoyaleTeamALobbyPoints[Random.Range(0, BattleRoyaleTeamALobbyPoints.Length)];
		}
		int num = Random.Range(0, BattleRoyaleTeamBLobbyPoints.Length);
		return BattleRoyaleTeamBLobbyPoints[num];
	}
}
