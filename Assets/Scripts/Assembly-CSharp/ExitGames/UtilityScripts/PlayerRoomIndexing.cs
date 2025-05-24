using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

namespace ExitGames.UtilityScripts
{
	public class PlayerRoomIndexing : PunBehaviour
	{
		public delegate void RoomIndexingChanged();

		public static PlayerRoomIndexing instance;

		public RoomIndexingChanged OnRoomIndexingChanged;

		public const string RoomPlayerIndexedProp = "PlayerIndexes";

		private int[] _playerIds;

		private object _indexes;

		private Dictionary<int, int> _indexesLUT;

		private List<bool> _indexesPool;

		private PhotonPlayer _p;

		public int[] PlayerIds => _playerIds;

		public void Awake()
		{
			if (instance != null)
			{
				Debug.LogError("Existing instance of PlayerRoomIndexing found. Only One instance is required at the most. Please correct and have only one at any time.");
			}
			instance = this;
			if (PhotonNetwork.room != null)
			{
				SanitizeIndexing(forceIndexing: true);
			}
		}

		public override void OnJoinedRoom()
		{
			if (PhotonNetwork.isMasterClient)
			{
				AssignIndex(PhotonNetwork.player);
			}
			else
			{
				RefreshData();
			}
		}

		public override void OnLeftRoom()
		{
			RefreshData();
		}

		public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
		{
			if (PhotonNetwork.isMasterClient)
			{
				AssignIndex(newPlayer);
			}
		}

		public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
		{
			if (PhotonNetwork.isMasterClient)
			{
				UnAssignIndex(otherPlayer);
			}
		}

		public override void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
		{
			if (propertiesThatChanged.ContainsKey("PlayerIndexes"))
			{
				RefreshData();
			}
		}

		public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
		{
			if (PhotonNetwork.isMasterClient)
			{
				SanitizeIndexing();
			}
		}

		public int GetRoomIndex(PhotonPlayer player)
		{
			if (_indexesLUT != null && _indexesLUT.ContainsKey(player.ID))
			{
				return _indexesLUT[player.ID];
			}
			return -1;
		}

		private void SanitizeIndexing(bool forceIndexing = false)
		{
			if ((!forceIndexing && !PhotonNetwork.isMasterClient) || PhotonNetwork.room == null)
			{
				return;
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			if (PhotonNetwork.room.CustomProperties.TryGetValue("PlayerIndexes", out _indexes))
			{
				dictionary = _indexes as Dictionary<int, int>;
			}
			if (dictionary.Count == PhotonNetwork.room.PlayerCount)
			{
				return;
			}
			PhotonPlayer[] playerList = PhotonNetwork.playerList;
			foreach (PhotonPlayer photonPlayer in playerList)
			{
				if (!dictionary.ContainsKey(photonPlayer.ID))
				{
					AssignIndex(photonPlayer);
				}
			}
		}

		private void RefreshData()
		{
			if (PhotonNetwork.room != null)
			{
				_playerIds = new int[PhotonNetwork.room.MaxPlayers];
				if (PhotonNetwork.room.CustomProperties.TryGetValue("PlayerIndexes", out _indexes))
				{
					_indexesLUT = _indexes as Dictionary<int, int>;
					foreach (KeyValuePair<int, int> item in _indexesLUT)
					{
						_p = PhotonPlayer.Find(item.Key);
						_playerIds[item.Value] = _p.ID;
					}
				}
			}
			else
			{
				_playerIds = new int[0];
			}
			if (OnRoomIndexingChanged != null)
			{
				OnRoomIndexingChanged();
			}
		}

		private void AssignIndex(PhotonPlayer player)
		{
			if (PhotonNetwork.room.CustomProperties.TryGetValue("PlayerIndexes", out _indexes))
			{
				_indexesLUT = _indexes as Dictionary<int, int>;
			}
			else
			{
				_indexesLUT = new Dictionary<int, int>();
			}
			List<bool> list = new List<bool>(new bool[PhotonNetwork.room.MaxPlayers]);
			foreach (KeyValuePair<int, int> item in _indexesLUT)
			{
				list[item.Value] = true;
			}
			_indexesLUT[player.ID] = Mathf.Max(0, list.IndexOf(item: false));
			PhotonNetwork.room.SetCustomProperties(new Hashtable { { "PlayerIndexes", _indexesLUT } });
			RefreshData();
		}

		private void UnAssignIndex(PhotonPlayer player)
		{
			if (PhotonNetwork.room.CustomProperties.TryGetValue("PlayerIndexes", out _indexes))
			{
				_indexesLUT = _indexes as Dictionary<int, int>;
				_indexesLUT.Remove(player.ID);
				PhotonNetwork.room.SetCustomProperties(new Hashtable { { "PlayerIndexes", _indexesLUT } });
			}
			RefreshData();
		}
	}
}
