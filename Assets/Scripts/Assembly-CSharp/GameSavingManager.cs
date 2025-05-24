using System;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSavingManager : MonoBehaviour
{
	public static void SaveCurrentGameWithName(string gameTitle)
	{
		string text = "{ \"scene_name\": \"" + SceneManager.GetActiveScene().name + "\",";
		text = text + "\"custom_game_title\": \"" + gameTitle + "\",";
		text += "\"objects\": [";
		InventoryObject[] array = UnityEngine.Object.FindObjectsOfType<InventoryObject>();
		foreach (InventoryObject inventoryObject in array)
		{
			text += "{";
			string text2 = text;
			text = text2 + "\"catId\": " + inventoryObject.categoryId + ",";
			text2 = text;
			text = text2 + "\"id\": " + inventoryObject.id + ",";
			string prefabName = inventoryObject.prefabName;
			text = text + "\"prefab\": \"" + prefabName + "\",";
			text2 = text;
			text = text2 + "\"px\": " + inventoryObject.transform.position.x + ",";
			text2 = text;
			text = text2 + "\"py\": " + inventoryObject.transform.position.y + ",";
			text2 = text;
			text = text2 + "\"pz\": " + inventoryObject.transform.position.z + ",";
			text2 = text;
			text = text2 + "\"rx\": " + inventoryObject.transform.eulerAngles.x + ",";
			text2 = text;
			text = text2 + "\"ry\": " + inventoryObject.transform.eulerAngles.y + ",";
			text = text + "\"rz\": " + inventoryObject.transform.eulerAngles.z;
			text += "},";
		}
		text += "]}";
		MonoBehaviour.print(text);
		AddNewGameTitle(gameTitle);
		SaveGameInPrefs(gameTitle, text);
		GetSavedGame(gameTitle);
	}

	private static void AddNewGameTitle(string title)
	{
		string text = PlayerPrefs.GetString("all_game_titles");
		text = ((!(PlayerPrefs.GetString("all_game_titles", string.Empty) == string.Empty)) ? (text + "|||" + title) : (text + title));
		PlayerPrefs.SetString("all_game_titles", text);
		PlayerPrefs.Save();
		MonoBehaviour.print("list0 = " + PlayerPrefs.GetString("all_game_titles"));
	}

	private static void SaveGameInPrefs(string gameTitle, string json)
	{
		PlayerPrefs.SetString(gameTitle, json);
	}

	public static void RemoveGame(string gameTitle)
	{
		string text = PlayerPrefs.GetString("all_game_titles");
		MonoBehaviour.print("list = " + text);
		string[] array = text.Split(new string[1] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
		array = Array.FindAll(array, (string element) => element != gameTitle);
		string.Join("|||", array);
		PlayerPrefs.SetString("all_game_titles", string.Join("|||", array));
		PlayerPrefs.Save();
		MonoBehaviour.print("list0 = " + PlayerPrefs.GetString("all_game_titles"));
		PlayerPrefs.DeleteKey(gameTitle);
	}

	public static string[] GetSavedGamesList()
	{
		return PlayerPrefs.GetString("all_game_titles").Split(new string[1] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
	}

	public static SavedGameInfo GetSavedGame(string gameTitle)
	{
		SavedGameInfo result = default(SavedGameInfo);
		JSONNode jSONNode = JSON.Parse(PlayerPrefs.GetString(gameTitle));
		string sceneName = jSONNode["scene_name"];
		string gameTitle2 = jSONNode["custom_game_title"];
		result.sceneName = sceneName;
		result.gameTitle = gameTitle2;
		JSONNode jSONNode2 = jSONNode["objects"];
		InventoryItem[] array = new InventoryItem[jSONNode2.Count];
		for (int i = 0; i < array.Length; i++)
		{
			JSONNode jSONNode3 = jSONNode2[i];
			MonoBehaviour.print(jSONNode3["prefab"]);
			InventoryItem inventoryItem = new InventoryItem();
			inventoryItem.id = (byte)jSONNode3["id"].AsInt;
			inventoryItem.categoryId = (byte)jSONNode3["catId"].AsInt;
			inventoryItem.prefabName = jSONNode3["prefab"];
			inventoryItem.pos.x = jSONNode3["px"].AsFloat;
			inventoryItem.pos.y = jSONNode3["py"].AsFloat;
			inventoryItem.pos.z = jSONNode3["pz"].AsFloat;
			inventoryItem.rot.x = jSONNode3["rx"].AsFloat;
			inventoryItem.rot.y = jSONNode3["ry"].AsFloat;
			inventoryItem.rot.z = jSONNode3["rz"].AsFloat;
			array[i] = inventoryItem;
		}
		result.items = array;
		return result;
	}
}
