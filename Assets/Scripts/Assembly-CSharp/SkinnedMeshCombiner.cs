using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshCombiner : MonoBehaviour
{
	private Material baseMat;

	private SkinnedMeshRenderer newSkin;

	private List<SkinnedMeshRenderer> smRenderers;

	private void Start()
	{
		smRenderers = new List<SkinnedMeshRenderer>();
		List<Transform> list = new List<Transform>();
		List<BoneWeight> list2 = new List<BoneWeight>();
		List<CombineInstance> list3 = new List<CombineInstance>();
		SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
		MonoBehaviour.print(componentsInChildren.Length);
		SkinnedMeshRenderer[] array = componentsInChildren;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
		{
			if (skinnedMeshRenderer.enabled)
			{
				if (baseMat == null)
				{
					MonoBehaviour.print(skinnedMeshRenderer.name);
					baseMat = skinnedMeshRenderer.sharedMaterial;
				}
				if (skinnedMeshRenderer.sharedMaterial == baseMat)
				{
					MonoBehaviour.print(skinnedMeshRenderer.name);
					smRenderers.Add(skinnedMeshRenderer);
				}
			}
		}
		int num = 0;
		foreach (SkinnedMeshRenderer smRenderer in smRenderers)
		{
			num += smRenderer.sharedMesh.subMeshCount;
		}
		int[] array2 = new int[num];
		for (int j = 0; j < smRenderers.Count; j++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer2 = smRenderers[j];
			Transform[] bones = skinnedMeshRenderer2.bones;
			foreach (Transform item in bones)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			BoneWeight[] boneWeights = skinnedMeshRenderer2.sharedMesh.boneWeights;
			for (int k = 0; k < boneWeights.Length; k++)
			{
				BoneWeight boneWeight = boneWeights[k];
				BoneWeight item2 = boneWeight;
				item2.boneIndex0 = list.IndexOf(skinnedMeshRenderer2.bones[boneWeight.boneIndex0]);
				item2.boneIndex1 = list.IndexOf(skinnedMeshRenderer2.bones[boneWeight.boneIndex1]);
				item2.boneIndex2 = list.IndexOf(skinnedMeshRenderer2.bones[boneWeight.boneIndex2]);
				item2.boneIndex3 = list.IndexOf(skinnedMeshRenderer2.bones[boneWeight.boneIndex3]);
				list2.Add(item2);
			}
			CombineInstance item3 = new CombineInstance
			{
				transform = skinnedMeshRenderer2.transform.localToWorldMatrix,
				mesh = skinnedMeshRenderer2.sharedMesh
			};
			array2[j] = item3.mesh.vertexCount;
			list3.Add(item3);
			skinnedMeshRenderer2.enabled = false;
		}
		List<Matrix4x4> list4 = new List<Matrix4x4>();
		for (int l = 0; l < list.Count; l++)
		{
			list4.Add(list[l].worldToLocalMatrix);
		}
		newSkin = base.gameObject.AddComponent<SkinnedMeshRenderer>();
		newSkin.sharedMesh = new Mesh();
		newSkin.sharedMesh.CombineMeshes(list3.ToArray(), mergeSubMeshes: true, useMatrices: true);
		newSkin.bones = list.ToArray();
		newSkin.material = baseMat;
		newSkin.sharedMesh.boneWeights = list2.ToArray();
		newSkin.sharedMesh.bindposes = list4.ToArray();
		newSkin.sharedMesh.RecalculateBounds();
	}
}
