using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
	public static HUDManager instance;

	[SerializeField]
	private List<PlayerUIHUD> huds = new List<PlayerUIHUD>();

	[SerializeField]
	private GameObject hudPrefab;

	private Camera cam;

	private bool initialized;

	[SerializeField]
	private Canvas canvas;

	private PlayerUIHUD bufHud;

	private Vector3 ViewportPosition;

	private Vector2 WorldObject_ScreenPosition;

	private Vector3 screenPointUnscaled;

	[SerializeField]
	private GameObject textDmgPrefab;

	[SerializeField]
	private Color dmgTextColor;

	private void Awake()
	{
		instance = this;
		textDmgPrefab.CreatePool(5);
	}

	private void Start()
	{
		cam = Camera.main;
		GameController.PlayerJoined = (Action<CharacterMotor>)Delegate.Combine(GameController.PlayerJoined, new Action<CharacterMotor>(OnNewPlayerConnected));
		GameController.PlayerDisconnected = (Action<CharacterMotor>)Delegate.Combine(GameController.PlayerDisconnected, new Action<CharacterMotor>(OnNewPlayerDisonnected));
	}

	private void LateUpdate()
	{
		if (!initialized && GameController.instance != null && GameController.instance.OurPlayer != null)
		{
			foreach (CharacterMotor player in GameController.instance.Players)
			{
				OnNewPlayerConnected(player);
			}
			initialized = true;
		}
		for (int i = 0; i < huds.Count; i++)
		{
			bufHud = huds[i];
			if (bufHud == null)
			{
				continue;
			}
			if (bufHud.target == null)
			{
				UnityEngine.Object.Destroy(huds[i].gameObject);
				huds.RemoveAt(i);
				break;
			}
			if (bufHud.player != null)
			{
				if (!bufHud.player.IsAlive())
				{
					bufHud.gameObject.SetActive(false);
					continue;
				}
				if (GameController.gameConfigData.gameMode == GameMode.Sandbox)
				{
					if (Time.time - bufHud.player.lastAtGunPointTime < 0.13f)
					{
						bufHud.gameObject.SetActive(true);
					}
					else
					{
						bufHud.gameObject.SetActive(false);
					}
				}
				else if (Time.time - bufHud.player.lastAtGunPointTime < 0.13f)
				{
					bufHud.gameObject.SetActive(true);
				}
				else
				{
					bufHud.gameObject.SetActive(false);
				}
				bufHud.UpdateHP_K(bufHud.player.HP / (float)bufHud.player.playerInfo.max_hp);
			}
			ViewportPosition = cam.WorldToViewportPoint(huds[i].target.position);
			if (ViewportPosition.z < 0f)
			{
				bufHud.rectTransform.anchoredPosition = -Vector2.one * 9900f;
				continue;
			}
			screenPointUnscaled = cam.WorldToScreenPoint(huds[i].target.position);
			WorldObject_ScreenPosition.x = screenPointUnscaled.x / canvas.scaleFactor;
			WorldObject_ScreenPosition.y = screenPointUnscaled.y / canvas.scaleFactor;
			huds[i].rectTransform.anchoredPosition = WorldObject_ScreenPosition;
		}
	}

	public void OnNewPlayerConnected(CharacterMotor player)
	{
		StartCoroutine(NewPlayerConnected(player));
	}

	private IEnumerator NewPlayerConnected(CharacterMotor player)
	{
		if (!(player == null))
		{
			while (GameController.instance == null || GameController.instance.OurPlayer == null)
			{
				yield return null;
			}
			if (!(player == null) && !(huds.Find((PlayerUIHUD h) => h.id == player.ViewId) != null))
			{
				PlayerUIHUD hud = UnityEngine.Object.Instantiate(hudPrefab).GetComponent<PlayerUIHUD>();
				hud.transform.SetParent(base.transform);
				hud.transform.localScale = Vector3.one;
				hud.player = player;
				hud.target = player.hudPivot;
				hud.id = player.ViewId;
				hud.SetPlayerName(player.playerInfo.name);
				hud.SetTeam(player.myTeam);
				huds.Add(hud);
			}
		}
	}

	private void OnNewPlayerDisonnected(CharacterMotor player)
	{
		PlayerUIHUD playerUIHUD = huds.Find((PlayerUIHUD h) => h.id == player.ViewId);
		if (playerUIHUD != null)
		{
			huds.Remove(playerUIHUD);
			UnityEngine.Object.Destroy(playerUIHUD.gameObject);
		}
	}

	private void OnDestroy()
	{
		GameController.PlayerJoined = (Action<CharacterMotor>)Delegate.Remove(GameController.PlayerJoined, new Action<CharacterMotor>(OnNewPlayerConnected));
		GameController.PlayerDisconnected = (Action<CharacterMotor>)Delegate.Remove(GameController.PlayerDisconnected, new Action<CharacterMotor>(OnNewPlayerDisonnected));
	}

	public void ShowHUDofPlayer(CharacterMotor player, bool show)
	{
		PlayerUIHUD playerUIHUD = huds.Find((PlayerUIHUD h) => h.id == player.ViewId);
		if (playerUIHUD != null)
		{
			playerUIHUD.gameObject.SetActive(show);
		}
	}

	public void CreateCarTeamIndicator(Vehicle car)
	{
		StartCoroutine(CreateCarTeamIndicatorC(car));
	}

	private IEnumerator CreateCarTeamIndicatorC(Vehicle car)
	{
		yield return null;
		yield return null;
		yield return null;
		while (GameController.instance == null || GameController.instance.OurPlayer == null)
		{
			yield return null;
		}
		if (!(huds.Find((PlayerUIHUD h) => h.id == car.photonView.viewID) != null))
		{
			PlayerUIHUD hud = UnityEngine.Object.Instantiate(hudPrefab).GetComponent<PlayerUIHUD>();
			hud.transform.SetParent(base.transform);
			hud.transform.localScale = Vector3.one;
			hud.target = car.hudPivot;
			hud.id = car.photonView.viewID;
			if (car.myDriver != null)
			{
				hud.SetPlayerName(car.myDriver.playerInfo.name);
				hud.SetTeam(car.myDriver.myTeam);
			}
			else
			{
				hud.SetPlayerName("lol car");
				hud.SetTeam(TeamID.None);
			}
			hud.ShowHPLine(false);
			huds.Add(hud);
		}
	}

	public void RemoveCarTeamIndicator(Vehicle car)
	{
		List<PlayerUIHUD> list = huds.FindAll((PlayerUIHUD h) => h.id == car.photonView.viewID);
		foreach (PlayerUIHUD item in list)
		{
			huds.Remove(item);
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}

	public void ShowTextDamage(float dmg, Vector3 point)
	{
		Text text = textDmgPrefab.Spawn(base.transform).GetComponent<Text>();
		text.text = dmg.ToString();
		text.transform.SetParent(base.transform);
		text.transform.localScale = Vector3.one;
		text.color = dmgTextColor;
		screenPointUnscaled = cam.WorldToScreenPoint(point);
		WorldObject_ScreenPosition.x = screenPointUnscaled.x / canvas.scaleFactor;
		WorldObject_ScreenPosition.y = screenPointUnscaled.y / canvas.scaleFactor;
		text.rectTransform.anchoredPosition = WorldObject_ScreenPosition;
		text.transform.DOScale(1.4f, 0.35f);
		text.transform.DOMoveY(text.transform.position.y + (float)Screen.height * 0.065f, 0.35f);
		text.DOFade(0.1f, 0.35f).OnComplete(delegate
		{
			text.gameObject.Recycle();
		});
	}
}
