using UnityEngine;
using UnityEngine.UI;

public class LocalizedLabel : MonoBehaviour
{
	public string localizationKey;

	public bool saveMyFont;

	private void Start()
	{
		if (localizationKey != string.Empty)
		{
			GetComponent<Text>().text = LocalizatioManager.GetStringByKey(localizationKey);
		}
		if (!saveMyFont)
		{
			GetComponent<Text>().font = LocalizatioManager.instance.GetLocalizedFont();
		}
	}

	public void SetTextByKey(string key)
	{
		GetComponent<Text>().text = LocalizatioManager.GetStringByKey(key);
		GetComponent<Text>().font = LocalizatioManager.instance.GetLocalizedFont();
	}
}
