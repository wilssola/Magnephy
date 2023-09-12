using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GoogleFirebase : MonoBehaviour
{
    public string[] TitleText;
    public string[] MessageText;
    public string[] ActionText;

    private Vector2 scrollViewVector = Vector2.zero;
    private string logText = "";
    const int kMaxLogSize = 16382;

#if UNITY_ANDROID

    void Start()
    {
        Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    public void SendInvite()
    {
        SendInviteAsync();
    }

    // Set the listeners for the various Invite received events.
    void InitializeFirebase()
    {
        DebugLog("Setting up firebase...");
        Firebase.Invites.FirebaseInvites.InviteReceived += OnInviteReceived;
        Firebase.Invites.FirebaseInvites.InviteNotReceived += OnInviteNotReceived;
        Firebase.Invites.FirebaseInvites.ErrorReceived += OnErrorReceived;
        DebugLog("Invites initialized!");

        DebugLog("Enabling data collection...");
        Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
    }

    public void OnInviteReceived(object sender, Firebase.Invites.InviteReceivedEventArgs e)
    {
        if (e.InvitationId != "")
        {
            DebugLog("Invite Received: Invitation ID: " + e.InvitationId);
            Firebase.Invites.FirebaseInvites.ConvertInvitationAsync(
              e.InvitationId).ContinueWith(HandleConversionResult);
        }
        if (e.DeepLink.ToString() != "")
        {
            DebugLog("Invite Received: Deep Link: " + e.DeepLink);
        }
    }

    public void OnInviteNotReceived(object sender, System.EventArgs e)
    {
        DebugLog("No Invite or Deep Link received on start up");
    }

    public void OnErrorReceived(object sender,
                                Firebase.Invites.InviteErrorReceivedEventArgs e)
    {
        DebugLog("Error occurred received the invite: " + e.ErrorMessage);
    }

    void HandleConversionResult(Task convertTask)
    {
        if (convertTask.IsCanceled)
        {
            DebugLog("Conversion canceled.");
        }
        else if (convertTask.IsFaulted)
        {
            DebugLog("Conversion encountered an error:");
            DebugLog(convertTask.Exception.ToString());
        }
        else if (convertTask.IsCompleted)
        {
            DebugLog("Conversion completed successfully!");
            DebugLog("ConvertInvitation: Successfully converted invitation");
        }
    }

    public Task<Firebase.Invites.SendInviteResult> SendInviteAsync()
    {
        Firebase.Invites.Invite invite = new Firebase.Invites.Invite()
        {
            TitleText = TitleText[Player.LanguageNumber],
            MessageText = MessageText[Player.LanguageNumber],
            CallToActionText = ActionText[Player.LanguageNumber],
            DeepLinkUrl = new Uri("https://bit.ly/magnephy-google/"),
        };
        return Firebase.Invites.FirebaseInvites.SendInviteAsync(invite).ContinueWith<Firebase.Invites.SendInviteResult>(HandleSentInvite);
    }

    Firebase.Invites.SendInviteResult HandleSentInvite(Task<Firebase.Invites.SendInviteResult> sendTask)
    {
        if (sendTask.IsCanceled)
        {
            DebugLog("Invitation canceled.");
        }
        else if (sendTask.IsFaulted)
        {
            DebugLog("Invitation encountered an error:");
            DebugLog(sendTask.Exception.ToString());
        }
        else if (sendTask.IsCompleted)
        {
            DebugLog("SendInvite: " +
            (new List<string>(sendTask.Result.InvitationIds)).Count +
            " invites sent successfully.");
            foreach (string id in sendTask.Result.InvitationIds)
            {
                DebugLog("SendInvite: Invite code: " + id);
            }
        }
        return sendTask.Result;
    }

    // Output text to the debug log text field, as well as the console.
    public void DebugLog(string s)
    {
        print(s);
        logText += s + "\n";

        while (logText.Length > kMaxLogSize)
        {
            int index = logText.IndexOf("\n");
            logText = logText.Substring(index + 1);
        }

        scrollViewVector.y = int.MaxValue;
    }

#endif
}