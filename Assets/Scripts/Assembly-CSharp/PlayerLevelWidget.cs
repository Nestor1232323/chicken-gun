using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelWidget : MonoBehaviour
{
	public RectTransform progressLine;

	public Text playerLevelLabel;

	private void Start()
	{
		StatisticsManager.ExpChanged = (Action)Delegate.Combine(StatisticsManager.ExpChanged, new Action(OnExpChanged));
		OnExpChanged();
	}

	private void OnExpChanged()
	{
		playerLevelLabel.text = DataModel.instance.PlayerLevelIndex.ToString();
		progressLine.SetLocalPositionX(0f - progressLine.sizeDelta.x + (float)DataModel.PlayerExp * 1f / (float)DataModel.instance.CurrentLevelMaxExp() * progressLine.sizeDelta.x);
	}

	private void OnDestroy()
	{
		StatisticsManager.ExpChanged = (Action)Delegate.Remove(StatisticsManager.ExpChanged, new Action(OnExpChanged));
	}
}
