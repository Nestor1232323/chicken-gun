using UnityEngine;

public class SaveFixedRotation : MonoBehaviour
{
	[SerializeField]
	private float fixedRotationZ;

	private void LateUpdate()
	{
		base.transform.SetLocalEulerZ(fixedRotationZ);
	}
}
