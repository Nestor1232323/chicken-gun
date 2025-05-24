using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Map
{
	public MapID mapID;

	public string name;

	public Sprite icon;

	public int order;

	public bool isZombie;

	public bool PvPCompatible;

	public bool teamFightCompatible;

	public bool captureFlagCompatible;

	public bool BattleRoyale_PvPCompatible;

	public bool BattleRoyale_TeamsCompatible;

	public List<GameMode> compModes;
}
