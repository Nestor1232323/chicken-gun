using UnityEngine;

public class OnJoinedInstantiate : MonoBehaviour
{
	public Transform SpawnPosition;

	public float PositionOffset = 2f;

	public GameObject[] PrefabsToInstantiate;

	public void OnJoinedRoom()
	{
		if (PrefabsToInstantiate == null)
		{
			return;
		}
		GameObject[] prefabsToInstantiate = PrefabsToInstantiate;
		foreach (GameObject gameObject in prefabsToInstantiate)
		{
			Debug.Log("Instantiating: " + gameObject.name);
			Vector3 vector = Vector3.up;
			if (SpawnPosition != null)
			{
				vector = SpawnPosition.position;
			}
			Vector3 insideUnitSphere = Random.insideUnitSphere;
			insideUnitSphere.y = 0f;
			insideUnitSphere = insideUnitSphere.normalized;
			Vector3 position = vector + PositionOffset * insideUnitSphere;
			PhotonNetwork.Instantiate(gameObject.name, position, Quaternion.identity, 0);
		}
	}
}
