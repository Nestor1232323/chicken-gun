using UnityEngine;

public class GPSController : MonoBehaviour
{
	private const string leaderbordId = "CgkIrbu-2bUMEAIQBg";

	private void Start()
	{
	}

	public void Authenticate()
	{
		Social.localUser.Authenticate(delegate(bool success)
		{
			if (success)
			{
				Debug.Log("Login successful!");
			}
			else
			{
				Debug.LogWarning("Failed to sign in with Google Play Games.");
			}
		});
	}

	public static void PostScoreToLeaderboard(int score)
	{
	}

	public void ShowLeaderboard()
	{
		Social.ShowLeaderboardUI();
	}
}
