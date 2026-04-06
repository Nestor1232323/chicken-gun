using UnityEngine;
using UnityEngine.Rendering;

public class ContentOptimizer : MonoBehaviour
{
	public bool DestroyIfWeakDevice;

	public bool DestroyIfStrongDevice;

	private void Start()
	{
		if (Device.isWeakDevice)
		{
			if (DestroyIfWeakDevice)
			{
				Object.Destroy(base.gameObject);
			}
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].receiveShadows = false;
				componentsInChildren[i].shadowCastingMode = ShadowCastingMode.Off;
			}
		}
		else if (DestroyIfStrongDevice)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
