using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
	[SerializeField]
	private Text state;

	[SerializeField]
	private Text persent;

	[SerializeField]
	private Text gameLoadingText;

	[SerializeField]
	private GameObject myWindow;

	[SerializeField]
	private GameObject menuSceneCanvas;

	private void Awake()
	{
		if (DataModel.instance == null)
		{
			myWindow.SetActive(true);
		}
		DataModel.PrefabsLoaded = (Action)Delegate.Combine(DataModel.PrefabsLoaded, new Action(OnPrefabsLoaded));
		DataModel.LoagingProgress = (Action<int>)Delegate.Combine(DataModel.LoagingProgress, new Action<int>(LoagingProgress));
	}

	private void OnPrefabsLoaded()
	{
		menuSceneCanvas.SetActive(true);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		DataModel.PrefabsLoaded = (Action)Delegate.Remove(DataModel.PrefabsLoaded, new Action(OnPrefabsLoaded));
		DataModel.LoagingProgress = (Action<int>)Delegate.Remove(DataModel.LoagingProgress, new Action<int>(LoagingProgress));
	}

	private void LoagingProgress(int val)
	{
		persent.text = string.Format("{0}%", val);
	}
}
