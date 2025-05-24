using System;
using System.Collections.Generic;
using UnityEngine;

public class DeathMatchPanel : MonoBehaviour
{
	private class PlayerItem
	{
		public CharacterMotor player;

		public PlayerDeathmatchListItem playerDeathmatchListItem;

		public PlayerItem(CharacterMotor player, PlayerDeathmatchListItem playerDeathmatchListItem)
		{
			this.player = player;
			this.playerDeathmatchListItem = playerDeathmatchListItem;
		}
	}

	[SerializeField]
	private GameObject listItemPrefab;

	[SerializeField]
	private Transform listContainer;

	private List<PlayerItem> list = new List<PlayerItem>();

	private void Awake()
	{
		GameController.PlayerJoined = (Action<CharacterMotor>)Delegate.Combine(GameController.PlayerJoined, new Action<CharacterMotor>(OnNewPlayerConnected));
		GameController.PlayerDisconnected = (Action<CharacterMotor>)Delegate.Combine(GameController.PlayerDisconnected, new Action<CharacterMotor>(OnPlayerDisonnected));
		CharacterMotor.ScoreChanged = (Action<CharacterMotor>)Delegate.Combine(CharacterMotor.ScoreChanged, new Action<CharacterMotor>(OnScoreChanged));
	}

	public void OnNewPlayerConnected(CharacterMotor player)
	{
		PlayerDeathmatchListItem component = UnityEngine.Object.Instantiate(listItemPrefab).GetComponent<PlayerDeathmatchListItem>();
		component.transform.SetParent(listContainer);
		component.Highlight(player.photonView.isMine);
		component.transform.localScale = Vector3.one;
		PlayerItem playerItem = new PlayerItem(player, component);
		list.Add(playerItem);
		playerItem.playerDeathmatchListItem.SetName(player.playerInfo.name);
		playerItem.playerDeathmatchListItem.SetScore(player.playerInfo.score);
		SortList();
	}

	private void OnPlayerDisonnected(CharacterMotor player)
	{
		PlayerItem playerItem = list.Find((PlayerItem v) => v.player == player);
		if (playerItem != null)
		{
			UnityEngine.Object.Destroy(playerItem.playerDeathmatchListItem.gameObject);
		}
		list.Remove(playerItem);
		SortList();
	}

	private void OnScoreChanged(CharacterMotor player)
	{
		list.Find((PlayerItem item) => item.player == player)?.playerDeathmatchListItem.SetScore(player.playerInfo.score);
		SortList();
	}

	private void OnDestroy()
	{
		GameController.PlayerJoined = (Action<CharacterMotor>)Delegate.Remove(GameController.PlayerJoined, new Action<CharacterMotor>(OnNewPlayerConnected));
		GameController.PlayerDisconnected = (Action<CharacterMotor>)Delegate.Remove(GameController.PlayerDisconnected, new Action<CharacterMotor>(OnPlayerDisonnected));
		CharacterMotor.ScoreChanged = (Action<CharacterMotor>)Delegate.Remove(CharacterMotor.ScoreChanged, new Action<CharacterMotor>(OnScoreChanged));
	}

	private void SortList()
	{
		list.Sort((PlayerItem x, PlayerItem y) => y.player.playerInfo.score.CompareTo(x.player.playerInfo.score));
		for (int num = 0; num < list.Count; num++)
		{
			list[num].playerDeathmatchListItem.transform.SetSiblingIndex(num);
		}
	}
}
