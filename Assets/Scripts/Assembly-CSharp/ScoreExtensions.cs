using ExitGames.Client.Photon;

public static class ScoreExtensions
{
	public static void SetScore(this PhotonPlayer player, int newScore)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["score"] = newScore;
		player.SetCustomProperties(hashtable);
	}

	public static void AddScore(this PhotonPlayer player, int scoreToAddToCurrent)
	{
		int score = player.GetScore();
		score += scoreToAddToCurrent;
		Hashtable hashtable = new Hashtable();
		hashtable["score"] = score;
		player.SetCustomProperties(hashtable);
	}

	public static int GetScore(this PhotonPlayer player)
	{
		object value;
		if (player.CustomProperties.TryGetValue("score", out value))
		{
			return (int)value;
		}
		return 0;
	}
}
