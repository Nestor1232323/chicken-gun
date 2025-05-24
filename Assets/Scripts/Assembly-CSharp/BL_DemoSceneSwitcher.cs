using UnityEngine;
using UnityEngine.SceneManagement;

public class BL_DemoSceneSwitcher : MonoBehaviour
{
	private bool _hideUI;

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Alpha1))
		{
			SceneManager.LoadScene("BL_Scene_Demo", LoadSceneMode.Single);
		}
		if (Input.GetKey(KeyCode.Alpha2))
		{
			SceneManager.LoadScene("BL_Scene_Demo_CrossFire", LoadSceneMode.Single);
		}
		if (Input.GetKey(KeyCode.Alpha3))
		{
			SceneManager.LoadScene("BL_Scene_Demo_Multiple", LoadSceneMode.Single);
		}
		if (Input.GetKey(KeyCode.Alpha4))
		{
			SceneManager.LoadScene("BL_Scene_Demo_RoundRobin", LoadSceneMode.Single);
		}
		if (Input.GetKey(KeyCode.Alpha5))
		{
			SceneManager.LoadScene("BL_Scene_Demo_Screenshots", LoadSceneMode.Single);
		}
		if (!Input.GetKeyDown(KeyCode.Tab))
		{
			return;
		}
		Canvas[] array = Object.FindObjectsOfType<Canvas>();
		if (_hideUI)
		{
			Canvas[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = true;
			}
			_hideUI = false;
		}
		else
		{
			Canvas[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
			_hideUI = true;
		}
	}
}
