using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleRoyaleLobbyWaitingPanel : MonoBehaviour
{
	[SerializeField]
	private Text maxPlayersText;

	[SerializeField]
	private Text currentPlayersText;

	[SerializeField]
	private AudioSource gameStartSoundNope;

	[SerializeField]
	private GameObject GO;

	public bool isWaiting = true;

	public static BattleRoyaleLobbyWaitingPanel instance;

	private void Awake()
	{
		if (MultiplayerController.gameType != GameMode.BattleRoyalePvP && MultiplayerController.gameType != GameMode.BattleRoyaleTeams)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		instance = this;
		GameController.PlayerJoined = (Action<CharacterMotor>)Delegate.Combine(GameController.PlayerJoined, new Action<CharacterMotor>(OnNewPlayerConnected));
		GameController.PlayerDisconnected = (Action<CharacterMotor>)Delegate.Combine(GameController.PlayerDisconnected, new Action<CharacterMotor>(OnPlayerDisonnected));
	}

	private void Start()
	{
		maxPlayersText.text = PhotonNetwork.room.MaxPlayers.ToString();
	}

	public void OnNewPlayerConnected(CharacterMotor player)
	{
		Debug.Log("OnNewPlayerConnected " + player.name);
		currentPlayersText.text = GameController.instance.AllPlayers().Count.ToString();
		if (GameController.instance.AllPlayers().Count == PhotonNetwork.room.MaxPlayers)
		{
			StartCoroutine(StartBattleRoyaleGame());
		}
	}

	private void OnPlayerDisonnected(CharacterMotor player)
	{
		currentPlayersText.text = GameController.instance.AllPlayers().Count.ToString();
	}

	private IEnumerator StartBattleRoyaleGame()
	{
		gameStartSoundNope.Play();
		GameController.PlayerJoined = (Action<CharacterMotor>)Delegate.Remove(GameController.PlayerJoined, new Action<CharacterMotor>(OnNewPlayerConnected));
		GameController.PlayerDisconnected = (Action<CharacterMotor>)Delegate.Remove(GameController.PlayerDisconnected, new Action<CharacterMotor>(OnPlayerDisonnected));
		GO.SetActive(value: false);
		yield return new WaitForSeconds(0.6f);
		GameController.instance.StartBattleRoyaleGame();
		base.gameObject.SetActive(value: false);
		isWaiting = false;
	}
}
