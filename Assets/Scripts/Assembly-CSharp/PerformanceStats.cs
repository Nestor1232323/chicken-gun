using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[AddComponentMenu("RVP/C#/Demo Scripts/Performance Stats", 1)]
public class PerformanceStats : MonoBehaviour
{
	public Text fpsText;

	private float fpsUpdateTime;

	private int frames;

	private void Update()
	{
		fpsUpdateTime = Mathf.Max(0f, fpsUpdateTime - Time.deltaTime);
		if (fpsUpdateTime == 0f)
		{
			fpsText.text = "FPS: " + frames;
			fpsUpdateTime = 1f;
			frames = 0;
		}
		else
		{
			frames++;
		}
	}

	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		Time.timeScale = 1f;
	}
}
