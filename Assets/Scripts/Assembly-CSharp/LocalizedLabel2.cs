using UnityEngine;
using UnityEngine.UI;

public class LocalizedLabel2 : MonoBehaviour
{
	[SerializeField]
	private string englishText;

	[SerializeField]
	private string russianText;

	[SerializeField]
	private string spanishText;

	[SerializeField]
	private string portugueseText;

	[SerializeField]
	private string frenchText;

	[SerializeField]
	private string germanText;

	public bool saveMyFont;

	private Text text => GetComponent<Text>();

	private void Start()
	{
		if (Application.systemLanguage == SystemLanguage.English && !string.IsNullOrEmpty(englishText))
		{
			text.text = englishText;
		}
		else if (Application.systemLanguage == SystemLanguage.Russian && !string.IsNullOrEmpty(russianText))
		{
			text.text = russianText;
		}
		else if (Application.systemLanguage == SystemLanguage.Spanish && !string.IsNullOrEmpty(spanishText))
		{
			text.text = spanishText;
		}
		else if (Application.systemLanguage == SystemLanguage.Portuguese && !string.IsNullOrEmpty(portugueseText))
		{
			text.text = portugueseText;
		}
		else if (Application.systemLanguage == SystemLanguage.French && !string.IsNullOrEmpty(frenchText))
		{
			text.text = frenchText;
		}
		else if (Application.systemLanguage == SystemLanguage.German && !string.IsNullOrEmpty(germanText))
		{
			text.text = germanText;
		}
		else
		{
			text.text = englishText;
		}
		if (!saveMyFont && LocalizatioManager.instance != null)
		{
			GetComponent<Text>().font = LocalizatioManager.instance.GetLocalizedFont();
		}
	}
}
