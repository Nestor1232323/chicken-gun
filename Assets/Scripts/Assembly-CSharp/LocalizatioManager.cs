using System.Collections.Generic;
using UnityEngine;

public class LocalizatioManager : MonoBehaviour
{
	public LocFont EnFont;

	public LocFont RuFont;

	public static LanguageKey currentLangKey;

	private static Dictionary<string, string> localizedStringsEnglish = new Dictionary<string, string>
	{
		{ "play", "Play" },
		{ "play_teamfight", "Battle" },
		{ "play_custom", "Custom" },
		{ "shop", "Shop" },
		{ "toch_to_enter_name", "Touch to enter name" },
		{ "back", "Back" },
		{ "balance", "Balance" },
		{ "capture_flag", "Capture flag" },
		{ "password", "Password" },
		{ "team_fight", "Team fight" },
		{ "deathmatch", "Deathmatch" },
		{ "sandbox", "Sandbox" },
		{ "loading", "Loading..." },
		{ "connection_screen_title", "Connect to game" },
		{ "load_game_btn", "Load game" },
		{ "create_game_btn", "Create game" },
		{ "room_title", "room" },
		{ "room_players_count", "players" },
		{ "mode", "mode" },
		{ "map", "map" },
		{ "shop_screen_title", "Shop" },
		{ "free_coins", "Free \ncoins" },
		{ "apply", "Apply" },
		{ "not_enough_money", "Not enough money" },
		{ "buy", "Buy" },
		{ "new_game_screen_title", "New game" },
		{ "create_new_game_btn", "Create game" },
		{ "select_game_mode_lbl", "Select game mode" },
		{ "select_map_lbl", "Select map" },
		{ "enter_game_title", "Enter title" },
		{ "map_players", "Players count" },
		{ "saved_game_screen_title", "Saved games" },
		{ "select_saved_map", "Select saved map" },
		{ "watch_clips_msg", "Watch clips and get  coins" },
		{ "get_free_money_btn", "Get" },
		{ "failed_to_load_video", "Failed toload video,\n try later" },
		{ "waiting_other_players", "Waiting other players . . ." },
		{ "me_was_kicked_msg", "You was kicked from the room" },
		{ "menu", "Menu" },
		{ "continue", "Continue" },
		{ "team", "Team" },
		{ "won", "Won" },
		{ "it_is_a_draw", "It's a draw" },
		{ "winner_reward", "winner reward" },
		{ "exp", "exp" },
		{ "coins", "coins" },
		{ "player_name", "Player" },
		{ "frags", "Kills" },
		{ "kill_assists", "Kill assists" },
		{ "deaths", "Deaths" },
		{ "reward", "Reward" },
		{ "raund_completed", "Raund completed" },
		{ "join_team_lbl", "Join team" },
		{ "join_btn", "Join" },
		{ "score", "Score" },
		{ "update", "Update" },
		{ "not_now_update", "Not now" },
		{ "new_version_msg", "New version available.\nIt's better to update" },
		{ "too_many_objects", "Too many objects in the scene, remove some to create new" },
		{ "too_many_vehicles", "Too many vehicles in the scene, remove some to create new" },
		{ "any_purchace_will_dis_ads", "Any purchace will disable ads" },
		{ "ads_disabled", "Ads is disabled!" },
		{ "incorrect_password", "Incorrect password" }
	};

	private static Dictionary<string, string> localizedStringsRussian = new Dictionary<string, string>
	{
		{ "play", "Играть" },
		{ "play_teamfight", "Battle" },
		{ "play_custom", "Custom" },
		{ "shop", "Магазин" },
		{ "toch_to_enter_name", "Введи свой ник" },
		{ "back", "Назад" },
		{ "balance", "Баланс" },
		{ "capture_flag", "Захват флага" },
		{ "password", "Пароль" },
		{ "team_fight", "Командный бой" },
		{ "deathmatch", "Все против всех" },
		{ "sandbox", "Песочница" },
		{ "loading", "Загрузка..." },
		{ "connection_screen_title", "Подключение к игре" },
		{ "load_game_btn", "Загрузить игру" },
		{ "create_game_btn", "Создать игру" },
		{ "room_title", "название" },
		{ "room_players_count", "игроков" },
		{ "mode", "режим" },
		{ "map", "карта" },
		{ "shop_screen_title", "Настройки игрока" },
		{ "free_coins", "Бесплатные\n монеты" },
		{ "apply", "Применить" },
		{ "not_enough_money", "Не хватает монет чтобы получить" },
		{ "buy", "Купить" },
		{ "new_game_screen_title", "Новая игра" },
		{ "create_new_game_btn", "Создать игру" },
		{ "select_game_mode_lbl", "Выбери режим" },
		{ "select_map_lbl", "Выбери карту" },
		{ "enter_game_title", "Название" },
		{ "map_players", "Игроков" },
		{ "saved_game_screen_title", "Сохраненные игры" },
		{ "select_saved_map", "Выбери игру" },
		{ "watch_clips_msg", "Смотри видео и получай монеты" },
		{ "get_free_money_btn", "Получить" },
		{ "failed_to_load_video", "Не удалость загрузить видео,\n попробуйте позже" },
		{ "waiting_other_players", "Ожидание других игроков" },
		{ "me_was_kicked_msg", "Тебя кикнули o_O" },
		{ "menu", "Меню" },
		{ "continue", "Продолжить" },
		{ "team", "Команда" },
		{ "won", "выиграла" },
		{ "it_is_a_draw", "Ничья" },
		{ "winner_reward", "награда" },
		{ "exp", "опыта" },
		{ "coins", "монет" },
		{ "player_name", "Игрок" },
		{ "frags", "Kills" },
		{ "kill_assists", "Kill assists" },
		{ "deaths", "Deaths" },
		{ "reward", "Награда" },
		{ "raund_completed", "Раунд окончен" },
		{ "join_team_lbl", "Выбери команду" },
		{ "join_btn", "Присоединиться" },
		{ "score", "Score" },
		{ "update", "Обновить" },
		{ "not_now_update", "Не сейчас" },
		{ "new_version_msg", "Новая версия доступна, пожалуйста обновите" },
		{ "too_many_objects", "Слишком много объектов на сцене, удалите некоторые чтобы создать новые" },
		{ "too_many_vehicles", "Слишком много машин на сцене, удалите некоторые чтобы создать новые" },
		{ "any_purchace_will_dis_ads", "Любая покупка отключит рекламу" },
		{ "ads_disabled", "Реклама отключена!" },
		{ "incorrect_password", "Неверный пароль" }
	};

	public static string[] PlayerTemplateNamesEn = new string[14]
	{
		"Crazy cat",
		"Chuck Borris",
		"Kitty",
		"Deadshot",
		"Big Boss",
		"Sniper",
		"Warrior",
		"Skilled hunter",
		"Hunter",
		"Destroyer",
		"Terminator",
		"Fat ball",
		"Ballman",
		string.Empty
	};

	public static string[] PlayerTemplateNamesRu = new string[9]
	{
		"Brave Lolo",
		"Chuck Borris",
		"Kitty",
		"Deadshot",
		"Злой бобер",
		"Добрый мишка",
		"Sniper",
		"Охранник пятерочки",
		string.Empty
	};

	private static Dictionary<string, string> currentLocDic;

	public static LocalizatioManager instance;

	public static string[] PlayerTemplateNames => PlayerTemplateNamesEn;

	private void Awake()
	{
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		if (Application.systemLanguage == SystemLanguage.English)
		{
			SetLanguageKey(LanguageKey.English);
		}
		else if (Application.systemLanguage == SystemLanguage.Russian)
		{
			SetLanguageKey(LanguageKey.Russian);
		}
		else
		{
			SetLanguageKey(LanguageKey.English);
		}
	}

	public static void SetLanguageKey(LanguageKey key)
	{
		currentLangKey = key;
		switch (key)
		{
		case LanguageKey.English:
			currentLocDic = localizedStringsEnglish;
			break;
		case LanguageKey.Russian:
			currentLocDic = localizedStringsRussian;
			break;
		}
	}

	public static string GetStringByKey(string key)
	{
		if (currentLocDic.ContainsKey(key))
		{
			return currentLocDic[key];
		}
		return "not found";
	}

	public Font GetLocalizedFont()
	{
		if (currentLangKey == LanguageKey.Russian)
		{
			return RuFont.font;
		}
		return EnFont.font;
	}
}
