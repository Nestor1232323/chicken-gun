using UnityEngine;

public class BowLine : MonoBehaviour
{
	[SerializeField]
	private Transform lookPoint;

	private void LateUpdate()
	{
		base.transform.LookAt(lookPoint);
	}
}
