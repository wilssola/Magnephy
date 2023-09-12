using System;
using UnityEngine;
using GooglePlayGames;
using System.Collections;
using GooglePlayGames.BasicApi;
using UnityEngine.EventSystems;

public class GPGS : MonoBehaviour {

    public GameObject AchievementButton, ScoreButton;
#if UNITY_ANDROID

    public void Start() {
        // Create client configuration.
        PlayGamesClientConfiguration Config = new PlayGamesClientConfiguration.Builder().Build();

        // Enable debugging output (recommended).
        // PlayGamesPlatform.DebugLogEnabled = true;

        // Initialize and activate the platform.
        PlayGamesPlatform.InitializeInstance(Config);
        PlayGamesPlatform.Activate();

        // PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);

        // Start the Google Plus Overlay Login.
        Social.localUser.Authenticate((bool Success) => { /* Do something depending on the success achieved */ });
    }

    private void Update()
    {
        AchievementButton.SetActive(Social.localUser.authenticated);
        ScoreButton.SetActive(Social.localUser.authenticated);

        // Submit leaderboard scores, if authenticated.
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            /*
            if (Player.GameOver) {
                PostScore(Convert.ToInt64(Player.HighScore), GPGSIds.leaderboard_high_score);                
                PlayerPrefs.SetInt("HighScore", Convert.ToInt32(GetPlayerScore(GPGSIds.leaderboard_high_score)));

                PostScore(Convert.ToInt64(Player.HighLevel), GPGSIds.leaderboard_high_level);
                PlayerPrefs.SetInt("HighLevel", Convert.ToInt32(GetPlayerScore(GPGSIds.leaderboard_high_level)));
            }
            */

            if (Player.HighScore >= 100)
            {
                UnlockAchievement(GPGSIds.achievement_very_good);
            }
            if (Player.HighScore >= 150)
            {
                UnlockAchievement(GPGSIds.achievement_very_cool);
            }
            if (Player.HighScore >= 250)
            {
                UnlockAchievement(GPGSIds.achievement_very_awesome);
            }
            if (Player.HighScore >= 500)
            {
                UnlockAchievement(GPGSIds.achievement_very_hard);
            }
            if (Player.HighScore >= 1000)
            {
                UnlockAchievement(GPGSIds.achievement_very_impossible);
            }
            if (Player.HighScore >= 5000)
            {
                UnlockAchievement(GPGSIds.achievement_very_god);
            }
        }
    }

    public void SignInCallback(bool Success)
    {
        if (Success)
        {
            Debug.Log("Signed in!");
        }
        else
        {
            Debug.Log("Signed out!");
        }
    }

    public void ShowLeaderboards()
    {
        // if (Social.localUser.authenticated)
        // {
        // PlayGamesPlatform.Instance.ShowLeaderboardUI();
        Social.ShowLeaderboardUI();
        // }
        // else
        // {
        // Debug.Log("Cannot show leaderboard: not authenticated");
        // }
    }

    public void ShowAchievements()
    {
        // if (Social.localUser.authenticated)
        // {
        // PlayGamesPlatform.Instance.ShowAchievementsUI();
        Social.ShowAchievementsUI();
        // }
        // else
        // {
        // Debug.Log("Cannot show Achievements: not authenticated");
        // }
    }

    public static void PostScore(long Score, string Id)
    {
        Social.ReportScore(Score, Id, (Success => { }));
    }

    public static long GetPlayerScore(string Id)
    {
        long Score = 0;
        PlayGamesPlatform.Instance.LoadScores(Id, LeaderboardStart.PlayerCentered, 1, LeaderboardCollection.Public, LeaderboardTimeSpan.AllTime, (LeaderboardScoreData Data) => { Score = Data.PlayerScore.value; });
        return Score;
    }

    public static void UnlockAchievement(string Id)
    {
        Social.ReportProgress(Id, 100, Success => { });
    }

    public static void IncrementAchievement(string Id, int Step)
    {
        PlayGamesPlatform.Instance.IncrementAchievement(Id, Step, Success => { });
    }
#endif
}