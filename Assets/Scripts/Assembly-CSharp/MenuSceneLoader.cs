using UnityEngine;

public class MenuSceneLoader : MonoBehaviour
{
	public GameObject menuUI;

	private GameObject m_Go;

	private void Awake()
	{
		if (m_Go == null)
		{
			m_Go = Object.Instantiate(menuUI);
		}
	}
}
