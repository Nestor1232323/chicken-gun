using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelReset : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public void OnPointerClick(PointerEventData data)
	{
		SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
	}

	private void Update()
	{
	}
}
