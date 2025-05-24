public class GameConfigData
{
	public int ArenaID;

	public double levelDuration = 220.0;

	public SavedGameInfo savedGame;

	public GameMode gameMode => MultiplayerController.gameType;
}
