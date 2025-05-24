using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonPlayer : IComparable<PhotonPlayer>, IComparable<int>, IEquatable<PhotonPlayer>, IEquatable<int>
{
	private int actorID = -1;

	private string nameField = string.Empty;

	public readonly bool IsLocal;

	public object TagObject;

	public int ID => actorID;

	public string NickName
	{
		get
		{
			return nameField;
		}
		set
		{
			if (!IsLocal)
			{
				Debug.LogError("Error: Cannot change the name of a remote player!");
			}
			else if (!string.IsNullOrEmpty(value) && !value.Equals(nameField))
			{
				nameField = value;
				PhotonNetwork.playerName = value;
			}
		}
	}

	public string UserId { get; internal set; }

	public bool IsMasterClient => PhotonNetwork.networkingPeer.mMasterClientId == ID;

	public bool IsInactive { get; set; }

	public Hashtable CustomProperties { get; internal set; }

	public Hashtable AllProperties
	{
		get
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Merge(CustomProperties);
			hashtable[byte.MaxValue] = NickName;
			return hashtable;
		}
	}

	[Obsolete("Please use NickName (updated case for naming).")]
	public string name
	{
		get
		{
			return NickName;
		}
		set
		{
			NickName = value;
		}
	}

	[Obsolete("Please use UserId (updated case for naming).")]
	public string userId
	{
		get
		{
			return UserId;
		}
		internal set
		{
			UserId = value;
		}
	}

	[Obsolete("Please use IsLocal (updated case for naming).")]
	public bool isLocal => IsLocal;

	[Obsolete("Please use IsMasterClient (updated case for naming).")]
	public bool isMasterClient => IsMasterClient;

	[Obsolete("Please use IsInactive (updated case for naming).")]
	public bool isInactive
	{
		get
		{
			return IsInactive;
		}
		set
		{
			IsInactive = value;
		}
	}

	[Obsolete("Please use CustomProperties (updated case for naming).")]
	public Hashtable customProperties
	{
		get
		{
			return CustomProperties;
		}
		internal set
		{
			CustomProperties = value;
		}
	}

	[Obsolete("Please use AllProperties (updated case for naming).")]
	public Hashtable allProperties => AllProperties;

	public PhotonPlayer(bool isLocal, int actorID, string name)
	{
		CustomProperties = new Hashtable();
		IsLocal = isLocal;
		this.actorID = actorID;
		nameField = name;
	}

	protected internal PhotonPlayer(bool isLocal, int actorID, Hashtable properties)
	{
		CustomProperties = new Hashtable();
		IsLocal = isLocal;
		this.actorID = actorID;
		InternalCacheProperties(properties);
	}

	public override bool Equals(object p)
	{
		if (p is PhotonPlayer photonPlayer)
		{
			return GetHashCode() == photonPlayer.GetHashCode();
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ID;
	}

	internal void InternalChangeLocalID(int newID)
	{
		if (!IsLocal)
		{
			Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
		}
		else
		{
			actorID = newID;
		}
	}

	internal void InternalCacheProperties(Hashtable properties)
	{
		if (properties != null && properties.Count != 0 && !CustomProperties.Equals(properties))
		{
			if (properties.ContainsKey(byte.MaxValue))
			{
				nameField = (string)properties[byte.MaxValue];
			}
			if (properties.ContainsKey((byte)253))
			{
				UserId = (string)properties[(byte)253];
			}
			if (properties.ContainsKey((byte)254))
			{
				IsInactive = (bool)properties[(byte)254];
			}
			CustomProperties.MergeStringKeys(properties);
			CustomProperties.StripKeysWithNullValues();
		}
	}

	public void SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedValues = null, bool webForward = false)
	{
		if (propertiesToSet != null)
		{
			Hashtable hashtable = propertiesToSet.StripToStringKeys();
			Hashtable hashtable2 = expectedValues.StripToStringKeys();
			bool flag = hashtable2 == null || hashtable2.Count == 0;
			bool num = actorID > 0 && !PhotonNetwork.offlineMode;
			if (flag)
			{
				CustomProperties.Merge(hashtable);
				CustomProperties.StripKeysWithNullValues();
			}
			if (num)
			{
				PhotonNetwork.networkingPeer.OpSetPropertiesOfActor(actorID, hashtable, hashtable2, webForward);
			}
			if (!num || flag)
			{
				InternalCacheProperties(hashtable);
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, this, hashtable);
			}
		}
	}

	public static PhotonPlayer Find(int ID)
	{
		if (PhotonNetwork.networkingPeer != null)
		{
			return PhotonNetwork.networkingPeer.GetPlayerWithId(ID);
		}
		return null;
	}

	public PhotonPlayer Get(int id)
	{
		return Find(id);
	}

	public PhotonPlayer GetNext()
	{
		return GetNextFor(ID);
	}

	public PhotonPlayer GetNextFor(PhotonPlayer currentPlayer)
	{
		if (currentPlayer == null)
		{
			return null;
		}
		return GetNextFor(currentPlayer.ID);
	}

	public PhotonPlayer GetNextFor(int currentPlayerId)
	{
		if (PhotonNetwork.networkingPeer == null || PhotonNetwork.networkingPeer.mActors == null || PhotonNetwork.networkingPeer.mActors.Count < 2)
		{
			return null;
		}
		Dictionary<int, PhotonPlayer> mActors = PhotonNetwork.networkingPeer.mActors;
		int num = int.MaxValue;
		int num2 = currentPlayerId;
		foreach (int key in mActors.Keys)
		{
			if (key < num2)
			{
				num2 = key;
			}
			else if (key > currentPlayerId && key < num)
			{
				num = key;
			}
		}
		if (num != int.MaxValue)
		{
			return mActors[num];
		}
		return mActors[num2];
	}

	public int CompareTo(PhotonPlayer other)
	{
		if (other == null)
		{
			return 0;
		}
		return GetHashCode().CompareTo(other.GetHashCode());
	}

	public int CompareTo(int other)
	{
		return GetHashCode().CompareTo(other);
	}

	public bool Equals(PhotonPlayer other)
	{
		if (other == null)
		{
			return false;
		}
		return GetHashCode().Equals(other.GetHashCode());
	}

	public bool Equals(int other)
	{
		return GetHashCode().Equals(other);
	}

	public override string ToString()
	{
		if (string.IsNullOrEmpty(NickName))
		{
			return string.Format("#{0:00}{1}{2}", ID, (!IsInactive) ? " " : " (inactive)", (!IsMasterClient) ? string.Empty : "(master)");
		}
		return string.Format("'{0}'{1}{2}", NickName, (!IsInactive) ? " " : " (inactive)", (!IsMasterClient) ? string.Empty : "(master)");
	}

	public string ToStringFull()
	{
		return string.Format("#{0:00} '{1}'{2} {3}", ID, NickName, (!IsInactive) ? string.Empty : " (inactive)", CustomProperties.ToStringFull());
	}
}
