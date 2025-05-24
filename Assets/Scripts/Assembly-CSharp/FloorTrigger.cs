using UnityEngine;

public class FloorTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider collider)
	{
		if (!collider.CompareTag("Player"))
		{
			if (collider.GetComponent<Vehicle>() != null && collider.GetComponent<Vehicle>().photonView.isMine)
			{
				collider.GetComponent<DamageReciver2>().Damage(10000f);
			}
			else if (collider.GetComponent<InventoryObject>() != null && collider.GetComponent<InventoryObject>().photonView.isMine)
			{
				PhotonNetwork.Destroy(collider.gameObject);
			}
		}
	}
}
