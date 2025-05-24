using UnityEngine;
using UnityEngine.UI;

public class CameraSwitch : MonoBehaviour
{
	public GameObject[] objects;

	public Text text;

	private int m_CurrentActiveObject;

	private void OnEnable()
	{
		text.text = objects[m_CurrentActiveObject].name;
	}

	public void NextCamera()
	{
		int num = ((m_CurrentActiveObject + 1 < objects.Length) ? (m_CurrentActiveObject + 1) : 0);
		for (int i = 0; i < objects.Length; i++)
		{
			objects[i].SetActive(i == num);
		}
		m_CurrentActiveObject = num;
		text.text = objects[m_CurrentActiveObject].name;
	}
}
