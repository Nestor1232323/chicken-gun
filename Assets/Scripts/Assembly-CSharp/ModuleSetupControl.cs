using UnityEngine;

public class ModuleSetupControl : MonoBehaviour
{
	private SantaAnimatorControl chrCtrl;

	[SerializeField]
	private Transform rootBone;

	private Transform[] boneStructure;

	[SerializeField]
	public ModuleList modules;

	private SkinnedMeshRenderer[] current_head;

	private Texture2D[] current_head_tex;

	private SkinnedMeshRenderer[] current_outfitUp;

	private Texture2D[] current_outfitUp_tex;

	private SkinnedMeshRenderer[] current_outfitDown;

	private Texture2D[] current_outfitDown_tex;

	private SkinnedMeshRenderer[] current_bag;

	private Texture2D[] current_bag_tex;

	private void Awake()
	{
		chrCtrl = GetComponent<SantaAnimatorControl>();
		boneStructure = rootBone.GetComponentsInChildren<Transform>();
	}

	private SkinnedMeshRenderer[] ModuleSetter(GameObject module)
	{
		GameObject gameObject = Object.Instantiate(module);
		SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].transform.parent = base.transform;
			componentsInChildren[i].transform.localPosition = Vector3.zero;
			componentsInChildren[i].transform.localRotation = Quaternion.identity;
			componentsInChildren[i].transform.localScale = Vector3.one;
			for (int j = 0; j < boneStructure.Length; j++)
			{
				if (componentsInChildren[i].rootBone.name == boneStructure[j].name)
				{
					componentsInChildren[i].rootBone = boneStructure[j];
					break;
				}
			}
			Transform[] array = new Transform[componentsInChildren[i].bones.Length];
			int num = 0;
			for (int k = 0; k < componentsInChildren[i].bones.Length; k++)
			{
				for (int l = 0; l < boneStructure.Length; l++)
				{
					if (componentsInChildren[i].bones[k].name == boneStructure[l].name)
					{
						array[num] = boneStructure[l];
						num++;
						break;
					}
				}
			}
			componentsInChildren[i].bones = array;
		}
		Object.Destroy(gameObject);
		return componentsInChildren;
	}

	public void ModuleRemove(SkinnedMeshRenderer[] currentModule)
	{
		if (currentModule.Length != 0)
		{
			for (int i = 0; i < currentModule.Length; i++)
			{
				Object.Destroy(currentModule[i].gameObject);
			}
		}
	}

	public void ModuleSelector(int parts, int partsID)
	{
		switch (parts)
		{
		case 0:
			if (current_head != null)
			{
				ModuleRemove(current_head);
			}
			current_head = ModuleSetter(modules.head[partsID].parts);
			current_head_tex = modules.head[partsID].texCol;
			break;
		case 1:
			if (current_outfitUp != null)
			{
				ModuleRemove(current_outfitUp);
			}
			current_outfitUp = ModuleSetter(modules.outfitUp[partsID].parts);
			current_outfitUp_tex = modules.outfitUp[partsID].texCol;
			break;
		case 2:
			if (current_outfitDown != null)
			{
				ModuleRemove(current_outfitDown);
			}
			current_outfitDown = ModuleSetter(modules.outfitDown[partsID].parts);
			current_outfitDown_tex = modules.outfitDown[partsID].texCol;
			break;
		case 3:
			if (current_bag != null)
			{
				ModuleRemove(current_bag);
			}
			current_bag = ModuleSetter(modules.bag[partsID].parts);
			current_bag_tex = modules.bag[partsID].texCol;
			break;
		}
	}

	public void SetAllModule(int headId, int outfitUpId, int outfitDownId, int bagId)
	{
		ModuleSelector(0, headId);
		ModuleSelector(1, outfitUpId);
		ModuleSelector(2, outfitDownId);
		ModuleSelector(3, bagId);
	}

	public void ColorSetter(SkinnedMeshRenderer[] currentModule, Texture2D tex)
	{
		if (currentModule.Length != 0)
		{
			for (int i = 0; i < currentModule.Length; i++)
			{
				currentModule[i].material.mainTexture = tex;
			}
		}
	}

	public void ColorSelector(int parts, int colorID)
	{
		switch (parts)
		{
		case 0:
			if (current_head != null)
			{
				ColorSetter(current_head, current_head_tex[colorID]);
			}
			break;
		case 1:
			if (current_outfitUp != null)
			{
				ColorSetter(current_outfitUp, current_outfitUp_tex[colorID]);
			}
			break;
		case 2:
			if (current_outfitDown != null)
			{
				ColorSetter(current_outfitDown, current_outfitDown_tex[colorID]);
			}
			break;
		case 3:
			if (current_bag != null)
			{
				ColorSetter(current_bag, current_bag_tex[colorID]);
			}
			break;
		}
	}

	public void SetAllColor(int headId, int outfitUpId, int outfitDownId, int bagId)
	{
		ColorSelector(0, headId);
		ColorSelector(1, outfitUpId);
		ColorSelector(2, outfitDownId);
		ColorSelector(3, bagId);
	}

	public void UpdateMeshData()
	{
		int num = 0;
		chrCtrl.meshData = new GameObject[current_head.Length + current_outfitUp.Length + current_outfitDown.Length + current_bag.Length];
		for (int i = 0; i < current_head.Length; i++)
		{
			chrCtrl.meshData[num] = current_head[i].gameObject;
			num++;
		}
		for (int j = 0; j < current_outfitUp.Length; j++)
		{
			chrCtrl.meshData[num] = current_outfitUp[j].gameObject;
			num++;
		}
		for (int k = 0; k < current_outfitDown.Length; k++)
		{
			chrCtrl.meshData[num] = current_outfitDown[k].gameObject;
			num++;
		}
		for (int l = 0; l < current_bag.Length; l++)
		{
			chrCtrl.meshData[num] = current_bag[l].gameObject;
			num++;
		}
	}
}
