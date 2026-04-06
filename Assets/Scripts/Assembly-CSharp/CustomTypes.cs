using ExitGames.Client.Photon;
using UnityEngine;

internal static class CustomTypes
{
	public static readonly byte[] memVector3 = new byte[12];

	public static readonly byte[] memVector2 = new byte[8];

	public static readonly byte[] memQuarternion = new byte[16];

	public static readonly byte[] memPlayer = new byte[4];

	internal static void Register()
	{
		PhotonPeer.RegisterType(typeof(Vector2), 87, SerializeVector2, DeserializeVector2);
		PhotonPeer.RegisterType(typeof(Vector3), 86, SerializeVector3, DeserializeVector3);
		PhotonPeer.RegisterType(typeof(Quaternion), 81, SerializeQuaternion, DeserializeQuaternion);
		PhotonPeer.RegisterType(typeof(PhotonPlayer), 80, SerializePhotonPlayer, DeserializePhotonPlayer);
	}

	private static short SerializeVector3(StreamBuffer outStream, object customobject)
	{
		Vector3 vector = (Vector3)customobject;
		int targetOffset = 0;
		lock (memVector3)
		{
			byte[] array = memVector3;
			Protocol.Serialize(vector.x, array, ref targetOffset);
			Protocol.Serialize(vector.y, array, ref targetOffset);
			Protocol.Serialize(vector.z, array, ref targetOffset);
			outStream.Write(array, 0, 12);
		}
		return 12;
	}

	private static object DeserializeVector3(StreamBuffer inStream, short length)
	{
		Vector3 vector = default(Vector3);
		lock (memVector3)
		{
			inStream.Read(memVector3, 0, 12);
			int offset = 0;
			Protocol.Deserialize(out vector.x, memVector3, ref offset);
			Protocol.Deserialize(out vector.y, memVector3, ref offset);
			Protocol.Deserialize(out vector.z, memVector3, ref offset);
		}
		return vector;
	}

	private static short SerializeVector2(StreamBuffer outStream, object customobject)
	{
		Vector2 vector = (Vector2)customobject;
		lock (memVector2)
		{
			byte[] array = memVector2;
			int targetOffset = 0;
			Protocol.Serialize(vector.x, array, ref targetOffset);
			Protocol.Serialize(vector.y, array, ref targetOffset);
			outStream.Write(array, 0, 8);
		}
		return 8;
	}

	private static object DeserializeVector2(StreamBuffer inStream, short length)
	{
		Vector2 vector = default(Vector2);
		lock (memVector2)
		{
			inStream.Read(memVector2, 0, 8);
			int offset = 0;
			Protocol.Deserialize(out vector.x, memVector2, ref offset);
			Protocol.Deserialize(out vector.y, memVector2, ref offset);
		}
		return vector;
	}

	private static short SerializeQuaternion(StreamBuffer outStream, object customobject)
	{
		Quaternion quaternion = (Quaternion)customobject;
		lock (memQuarternion)
		{
			byte[] array = memQuarternion;
			int targetOffset = 0;
			Protocol.Serialize(quaternion.w, array, ref targetOffset);
			Protocol.Serialize(quaternion.x, array, ref targetOffset);
			Protocol.Serialize(quaternion.y, array, ref targetOffset);
			Protocol.Serialize(quaternion.z, array, ref targetOffset);
			outStream.Write(array, 0, 16);
		}
		return 16;
	}

	private static object DeserializeQuaternion(StreamBuffer inStream, short length)
	{
		Quaternion quaternion = default(Quaternion);
		lock (memQuarternion)
		{
			inStream.Read(memQuarternion, 0, 16);
			int offset = 0;
			Protocol.Deserialize(out quaternion.w, memQuarternion, ref offset);
			Protocol.Deserialize(out quaternion.x, memQuarternion, ref offset);
			Protocol.Deserialize(out quaternion.y, memQuarternion, ref offset);
			Protocol.Deserialize(out quaternion.z, memQuarternion, ref offset);
		}
		return quaternion;
	}

	private static short SerializePhotonPlayer(StreamBuffer outStream, object customobject)
	{
		int iD = ((PhotonPlayer)customobject).ID;
		lock (memPlayer)
		{
			byte[] array = memPlayer;
			int targetOffset = 0;
			Protocol.Serialize(iD, array, ref targetOffset);
			outStream.Write(array, 0, 4);
			return 4;
		}
	}

	private static object DeserializePhotonPlayer(StreamBuffer inStream, short length)
	{
		int value;
		lock (memPlayer)
		{
			inStream.Read(memPlayer, 0, length);
			int offset = 0;
			Protocol.Deserialize(out value, memPlayer, ref offset);
		}
		if (PhotonNetwork.networkingPeer.mActors.ContainsKey(value))
		{
			return PhotonNetwork.networkingPeer.mActors[value];
		}
		return null;
	}
}
