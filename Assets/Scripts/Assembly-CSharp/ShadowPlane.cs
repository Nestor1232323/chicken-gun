using UnityEngine;

public class ShadowPlane : MonoBehaviour
{
	private MeshRenderer shadowPlane;

	private RaycastHit hitinfo;

	[SerializeField]
	private LayerMask shadowPlaneLayers;

	private void Start()
	{
		shadowPlane = GetComponent<MeshRenderer>();
	}

	private void LateUpdate()
	{
		RaycastShadowPos();
	}

	private void RaycastShadowPos()
	{
		if (Physics.Linecast(base.transform.position + Vector3.up * 0.1f, base.transform.position + Vector3.down * 10f, out hitinfo, shadowPlaneLayers, QueryTriggerInteraction.Ignore))
		{
			shadowPlane.enabled = true;
			shadowPlane.transform.position = hitinfo.point + hitinfo.normal * 0.02f;
		}
		else
		{
			shadowPlane.enabled = false;
		}
	}
}
