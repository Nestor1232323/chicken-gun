using UnityEngine;
using UnityEngine.UI;

public class PointersTest : MonoBehaviour
{
	public Text logger;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void CustomPointerDown(int i)
	{
		log(string.Empty + i + " PointerDown");
	}

	public void CustomPointerUp(int i)
	{
		log(string.Empty + i + " PointerUp");
	}

	public void CustomPointerEnter(int i)
	{
	}

	public void CustomPointerExit(int i)
	{
	}

	private void log(string str)
	{
		Text text = logger;
		text.text = text.text + " -" + str + "- ";
	}
}
