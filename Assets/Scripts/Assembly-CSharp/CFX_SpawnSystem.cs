using System.Collections.Generic;
using UnityEngine;

public class CFX_SpawnSystem : MonoBehaviour
{
	private static CFX_SpawnSystem instance;

	public GameObject[] objectsToPreload = new GameObject[0];

	public int[] objectsToPreloadTimes = new int[0];

	public bool hideObjectsInHierarchy;

	private bool allObjectsLoaded;

	private Dictionary<int, List<GameObject>> instantiatedObjects = new Dictionary<int, List<GameObject>>();

	private Dictionary<int, int> poolCursors = new Dictionary<int, int>();

	public static bool AllObjectsLoaded => instance.allObjectsLoaded;

	public static GameObject GetNextObject(GameObject sourceObj, bool activateObject = true)
	{
		int instanceID = sourceObj.GetInstanceID();
		if (!instance.poolCursors.ContainsKey(instanceID))
		{
			Debug.LogError("[CFX_SpawnSystem.GetNextPoolObject()] Object hasn't been preloaded: " + sourceObj.name + " (ID:" + instanceID + ")");
			return null;
		}
		int index = instance.poolCursors[instanceID];
		instance.poolCursors[instanceID]++;
		if (instance.poolCursors[instanceID] >= instance.instantiatedObjects[instanceID].Count)
		{
			instance.poolCursors[instanceID] = 0;
		}
		GameObject gameObject = instance.instantiatedObjects[instanceID][index];
		if (activateObject)
		{
			gameObject.SetActive(value: true);
		}
		return gameObject;
	}

	public static void PreloadObject(GameObject sourceObj, int poolSize = 1)
	{
		instance.addObjectToPool(sourceObj, poolSize);
	}

	public static void UnloadObjects(GameObject sourceObj)
	{
		instance.removeObjectsFromPool(sourceObj);
	}

	private void addObjectToPool(GameObject sourceObject, int number)
	{
		int instanceID = sourceObject.GetInstanceID();
		if (!instantiatedObjects.ContainsKey(instanceID))
		{
			instantiatedObjects.Add(instanceID, new List<GameObject>());
			poolCursors.Add(instanceID, 0);
		}
		for (int i = 0; i < number; i++)
		{
			GameObject gameObject = Object.Instantiate(sourceObject);
			gameObject.SetActive(value: false);
			CFX_AutoDestructShuriken[] componentsInChildren = gameObject.GetComponentsInChildren<CFX_AutoDestructShuriken>(includeInactive: true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].OnlyDeactivate = true;
			}
			CFX_LightIntensityFade[] componentsInChildren2 = gameObject.GetComponentsInChildren<CFX_LightIntensityFade>(includeInactive: true);
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].autodestruct = false;
			}
			instantiatedObjects[instanceID].Add(gameObject);
			if (hideObjectsInHierarchy)
			{
				gameObject.hideFlags = HideFlags.HideInHierarchy;
			}
		}
	}

	private void removeObjectsFromPool(GameObject sourceObject)
	{
		int instanceID = sourceObject.GetInstanceID();
		if (!instantiatedObjects.ContainsKey(instanceID))
		{
			Debug.LogWarning("[CFX_SpawnSystem.removeObjectsFromPool()] There aren't any preloaded object for: " + sourceObject.name + " (ID:" + instanceID + ")");
			return;
		}
		for (int num = instantiatedObjects[instanceID].Count - 1; num >= 0; num--)
		{
			GameObject obj = instantiatedObjects[instanceID][num];
			instantiatedObjects[instanceID].RemoveAt(num);
			Object.Destroy(obj);
		}
		instantiatedObjects.Remove(instanceID);
		poolCursors.Remove(instanceID);
	}

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogWarning("CFX_SpawnSystem: There should only be one instance of CFX_SpawnSystem per Scene!");
		}
		instance = this;
	}

	private void Start()
	{
		allObjectsLoaded = false;
		for (int i = 0; i < objectsToPreload.Length; i++)
		{
			PreloadObject(objectsToPreload[i], objectsToPreloadTimes[i]);
		}
		allObjectsLoaded = true;
	}
}
