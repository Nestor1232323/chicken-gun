using System.Collections;
using UnityEngine;

public class FlagPoint : MonoBehaviour
{
	public TeamID team;

	public TeamID parentTeamBase;

	private Material mat => GetComponent<MeshRenderer>().material;

	private IEnumerator Start()
	{
		if (GameController.gameConfigData.gameMode != GameMode.CaptureFlag)
		{
			base.gameObject.SetActive(value: false);
			yield break;
		}
		while (GameController.instance == null || GameController.instance.OurPlayer == null)
		{
			yield return null;
		}
		yield return null;
		if (GameController.instance.OurPlayer.myTeam != TeamID.None)
		{
			if (team == GameController.instance.OurPlayer.myTeam)
			{
				mat.color = DataModel.instance.teamA_Color;
			}
			else
			{
				mat.color = DataModel.instance.teamB_Color;
			}
		}
		else
		{
			mat.color = Color.black;
		}
	}
}
