using System.Collections.Generic;
using DG.Tweening;
using Photon;
using UnityEngine;

public class AmmoGenerator : Photon.MonoBehaviour
{
	public static AmmoGenerator instance;

	[SerializeField]
	private float spawnPeriod = 10f;

	private Transform[] respawnPoints;

	public GameObject AmmoPrefab;

	public GameObject LifeHeartPrefab;

	public GameObject SmokeGrenadePrefab;

	public GameObject MolotovGrenadePrefab;

	public List<GameObject> lootItems;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (MultiplayerController.gameType != GameMode.BattleRoyalePvP && MultiplayerController.gameType != GameMode.BattleRoyaleTeams)
		{
			if (PhotonNetwork.isMasterClient)
			{
				InvokeRepeating("CreateBonus", 0f, spawnPeriod);
			}
			respawnPoints = ArenaScript.instance.BonusPointsContainer.GetComponentsInChildren<Transform>();
		}
	}

	private void CreateBonus()
	{
		BonusType bonusType = BonusType.Ammo;
		float value = Random.value;
		if (value > 0f && value <= 0.2f)
		{
			bonusType = BonusType.LifeHeart;
		}
		else if (value > 0.2f && value <= 0.4f)
		{
			bonusType = BonusType.Smoke;
		}
		else if (value > 0.4f && value <= 0.6f)
		{
			bonusType = BonusType.Molotov;
		}
		else if (value > 0.6f && value <= 1f)
		{
			bonusType = BonusType.Ammo;
		}
		int num = Random.Range(0, respawnPoints.Length);
		PhotonNetwork.RPC(base.photonView, "CreateBonus", PhotonTargets.All, false, num, (int)bonusType);
	}

	[PunRPC]
	private void CreateBonus(int posIndex, int type)
	{
		if (respawnPoints[posIndex].childCount == 0)
		{
			Vector3 pos = respawnPoints[posIndex].position;
			UpdatePointPos(ref pos);
			switch ((BonusType)type)
			{
			case BonusType.Ammo:
				Object.Instantiate(AmmoPrefab, pos, Quaternion.identity).transform.SetParent(respawnPoints[posIndex]);
				break;
			case BonusType.LifeHeart:
				Object.Instantiate(LifeHeartPrefab, pos, Quaternion.identity).transform.SetParent(respawnPoints[posIndex]);
				break;
			case BonusType.Smoke:
				Object.Instantiate(SmokeGrenadePrefab, pos, Quaternion.identity).transform.SetParent(respawnPoints[posIndex]);
				break;
			case BonusType.Molotov:
				Object.Instantiate(MolotovGrenadePrefab, pos, Quaternion.identity).transform.SetParent(respawnPoints[posIndex]);
				break;
			}
		}
	}

	private void UpdatePointPos(ref Vector3 pos)
	{
		if (Physics.Raycast(pos + Vector3.up * 500f, Vector3.down, out var hitInfo))
		{
			pos = hitInfo.point;
		}
	}

	private void OnMasterClientSwitched()
	{
		if (MultiplayerController.gameType != GameMode.BattleRoyalePvP && MultiplayerController.gameType != GameMode.BattleRoyaleTeams)
		{
			Debug.Log("OnMasterClientSwitched");
			CancelInvoke("CreateBonus");
			if (PhotonNetwork.isMasterClient)
			{
				InvokeRepeating("CreateBonus", 0f, spawnPeriod);
			}
		}
	}

	public void OnBoomLootBox(Vector3 lootPos)
	{
		Vector3 vector = lootPos + Vector3.up * 0.5f;
		byte b = (byte)Random.Range(0, lootItems.Count);
		PhotonNetwork.RPC(base.photonView, "OnBoomLootBoxR", PhotonTargets.All, false, b, vector);
	}

	[PunRPC]
	public void OnBoomLootBoxR(byte index, Vector3 pos)
	{
		GameObject obj = lootItems[index].Spawn(pos);
		obj.transform.localScale = Vector3.one * 0.1f;
		obj.name = lootItems[index].name;
		obj.transform.DOScale(1f, 0.3f);
	}
}
