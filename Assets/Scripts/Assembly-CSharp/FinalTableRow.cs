using UnityEngine;
using UnityEngine.UI;

public class FinalTableRow : MonoBehaviour
{
	public Text orderLabel;

	public Text nameLabel;

	public Text fragsLabel;

	public Text rewardLabel;

	public Image icon;

	[SerializeField]
	private Color highlightColor;

	private void Start()
	{
	}

	public void Highlight()
	{
		GetComponent<Image>().color = highlightColor;
	}
}
