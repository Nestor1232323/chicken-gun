using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

public class PunTurnManager : PunBehaviour
{
	public float TurnDuration = 20f;

	public IPunTurnManagerCallbacks TurnManagerListener;

	private readonly HashSet<PhotonPlayer> finishedPlayers = new HashSet<PhotonPlayer>();

	public const byte TurnManagerEventOffset = 0;

	public const byte EvMove = 1;

	public const byte EvFinalMove = 2;

	private bool _isOverCallProcessed;

	public int Turn
	{
		get
		{
			return PhotonNetwork.room.GetTurn();
		}
		private set
		{
			_isOverCallProcessed = false;
			PhotonNetwork.room.SetTurn(value, true);
		}
	}

	public float ElapsedTimeInTurn
	{
		get
		{
			return (float)(PhotonNetwork.ServerTimestamp - PhotonNetwork.room.GetTurnStart()) / 1000f;
		}
	}

	public float RemainingSecondsInTurn
	{
		get
		{
			return Mathf.Max(0f, TurnDuration - ElapsedTimeInTurn);
		}
	}

	public bool IsCompletedByAll
	{
		get
		{
			return PhotonNetwork.room != null && Turn > 0 && finishedPlayers.Count == PhotonNetwork.room.PlayerCount;
		}
	}

	public bool IsFinishedByMe
	{
		get
		{
			return finishedPlayers.Contains(PhotonNetwork.player);
		}
	}

	public bool IsOver
	{
		get
		{
			return RemainingSecondsInTurn <= 0f;
		}
	}

	private void Start()
	{
		PhotonNetwork.OnEventCall += OnEvent;
	}

	private void Update()
	{
		if (Turn > 0 && IsOver && !_isOverCallProcessed)
		{
			_isOverCallProcessed = true;
			TurnManagerListener.OnTurnTimeEnds(Turn);
		}
	}

	public void BeginTurn()
	{
		Turn++;
	}

	public void SendMove(object move, bool finished)
	{
		if (IsFinishedByMe)
		{
			Debug.LogWarning("Can't SendMove. Turn is finished by this player.");
			return;
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add("turn", Turn);
		hashtable.Add("move", move);
		byte eventCode = (byte)((!finished) ? 1 : 2);
		PhotonNetwork.RaiseEvent(eventCode, hashtable, true, new RaiseEventOptions
		{
			CachingOption = EventCaching.AddToRoomCache
		});
		if (finished)
		{
			PhotonNetwork.player.SetFinishedTurn(Turn);
		}
		OnEvent(eventCode, hashtable, PhotonNetwork.player.ID);
	}

	public bool GetPlayerFinishedTurn(PhotonPlayer player)
	{
		if (player != null && finishedPlayers != null && finishedPlayers.Contains(player))
		{
			return true;
		}
		return false;
	}

	public void OnEvent(byte eventCode, object content, int senderId)
	{
		PhotonPlayer photonPlayer = PhotonPlayer.Find(senderId);
		switch (eventCode)
		{
		case 1:
		{
			Hashtable hashtable2 = content as Hashtable;
			int turn = (int)hashtable2["turn"];
			object move2 = hashtable2["move"];
			TurnManagerListener.OnPlayerMove(photonPlayer, turn, move2);
			break;
		}
		case 2:
		{
			Hashtable hashtable = content as Hashtable;
			int num = (int)hashtable["turn"];
			object move = hashtable["move"];
			if (num == Turn)
			{
				finishedPlayers.Add(photonPlayer);
				TurnManagerListener.OnPlayerFinished(photonPlayer, num, move);
			}
			if (IsCompletedByAll)
			{
				TurnManagerListener.OnTurnCompleted(Turn);
			}
			break;
		}
		}
	}

	public override void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
	{
		if (propertiesThatChanged.ContainsKey("Turn"))
		{
			_isOverCallProcessed = false;
			finishedPlayers.Clear();
			TurnManagerListener.OnTurnBegins(Turn);
		}
	}
}
