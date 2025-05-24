using System.Collections;
using UnityEngine;

public class HipPointsWidget : MonoBehaviour
{
	public static HipPointsWidget instance;

	public RectTransform darkBack;

	public RectTransform line;

	private float inversMaxHp = -1f;

	private void Awake()
	{
		instance = this;
	}

	private IEnumerator Start()
	{
		while (GameController.instance == null || GameController.instance.OurPlayer == null)
		{
			yield return null;
		}
		UpdateHP(GameController.instance.OurPlayer.playerInfo.max_hp);
	}

	public void UpdateHP(float value)
	{
		if (!(darkBack == null))
		{
			if (inversMaxHp < 0f)
			{
				inversMaxHp = 1f / (float)GameController.instance.OurPlayer.playerInfo.max_hp;
			}
			float x = darkBack.sizeDelta.x * value * inversMaxHp;
			line.sizeDelta = new Vector2(x, line.sizeDelta.y);
		}
	}
}
