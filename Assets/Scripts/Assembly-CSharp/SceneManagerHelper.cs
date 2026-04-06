using UnityEngine.SceneManagement;

public class SceneManagerHelper
{
	public static string ActiveSceneName
	{
		get
		{
			return SceneManager.GetActiveScene().name;
		}
	}

	public static int ActiveSceneBuildIndex
	{
		get
		{
			return SceneManager.GetActiveScene().buildIndex;
		}
	}
}
