using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DamageFXWidget : MonoBehaviour
{
	[SerializeField]
	private GameObject fxPrefab;

	private List<GameObject> FXList = new List<GameObject>();

	private CharacterMotor ourPlayer;

	[SerializeField]
	private Image rangeDmgFXImg;

	private Vector3 d;

	private void Start()
	{
		ourPlayer = GameController.instance.OurPlayer;
		GameController.OurPlayerCreated = (Action<CharacterMotor>)Delegate.Combine(GameController.OurPlayerCreated, new Action<CharacterMotor>(OnOurPlayerCreated));
	}

	private void OnOurPlayerCreated(CharacterMotor player)
	{
		player.DamagedBySomeone = (Action<UnityEngine.Object, int>)Delegate.Combine(player.DamagedBySomeone, new Action<UnityEngine.Object, int>(PlayerDamaged));
	}

	private void OnDestroy()
	{
		GameController.OurPlayerCreated = (Action<CharacterMotor>)Delegate.Remove(GameController.OurPlayerCreated, new Action<CharacterMotor>(OnOurPlayerCreated));
		if (!(GameController.instance == null) && !(GameController.instance.OurPlayer == null))
		{
			CharacterMotor characterMotor = GameController.instance.OurPlayer;
			characterMotor.DamagedBySomeone = (Action<UnityEngine.Object, int>)Delegate.Remove(characterMotor.DamagedBySomeone, new Action<UnityEngine.Object, int>(PlayerDamaged));
		}
	}

	private void LateUpdate()
	{
		if (ourPlayer != null)
		{
			d = ourPlayer.transform.forward;
			d.y = 0f - d.z;
			d.z = 0f;
			base.transform.up = d;
		}
	}

	private void PlayerDamaged(object sender, int fromWhom)
	{
		PhotonView photonView = PhotonView.Find(fromWhom);
		if (photonView == null)
		{
			rangeDmgFXImg.gameObject.SetActive(value: true);
			rangeDmgFXImg.color = new Color(1f, 1f, 1f, 0f);
			rangeDmgFXImg.DOColor(new Color(1f, 1f, 1f, 1f), 0.3f).OnComplete(delegate
			{
				rangeDmgFXImg.DOFade(0f, 0.3f).OnComplete(delegate
				{
					rangeDmgFXImg.gameObject.SetActive(value: false);
				});
			});
			return;
		}
		Vector3 vector = (sender as CharacterMotor).transform.InverseTransformPoint(photonView.transform.position);
		GameObject fx = UnityEngine.Object.Instantiate(fxPrefab);
		fx.transform.SetParent(base.transform);
		fx.transform.localScale = Vector3.one;
		fx.GetComponent<RectTransform>().anchoredPosition = Vector2.one * 0f;
		fx.transform.up = new Vector3(vector.x, vector.z, 0f);
		FXList.Add(fx);
		if (FXList.Count > 5)
		{
			UnityEngine.Object.Destroy(FXList[0]);
			FXList.RemoveAt(0);
		}
		fx.GetComponent<Image>().DOFade(0f, 2f).OnComplete(delegate
		{
			FXList.Remove(fx);
			UnityEngine.Object.Destroy(fx);
		});
	}
}
