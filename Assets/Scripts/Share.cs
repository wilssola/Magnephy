using UnityEngine;
using System.Collections;
using System.IO;

public class Share : MonoBehaviour
{
    public string ShareImageName;

    public string[] ShareText;
    public string[] ShareExtraText;

    private bool Sharing = false;
    private bool Focus = false;

    public void SharePress()
    {
        if (!Sharing)
        {
            StartCoroutine(ShareScreenshot());
        }
    }

    IEnumerator ShareScreenshot()
    {
        Sharing = true;

        yield return new WaitForEndOfFrame();

        Application.CaptureScreenshot(ShareImageName + ".png", 2);
        string Destination = Path.Combine(Application.persistentDataPath, ShareImageName + ".png");

        yield return new WaitForSecondsRealtime(0.3f);

        if (!Application.isEditor)
        {
            AndroidJavaClass IntentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject IntentObject = new AndroidJavaObject("android.content.Intent");
            IntentObject.Call<AndroidJavaObject>("setAction", IntentClass.GetStatic<string>("ACTION_SEND"));
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + Destination);
            IntentObject.Call<AndroidJavaObject>("putExtra", IntentClass.GetStatic<string>("EXTRA_STREAM"),
                uriObject);
            IntentObject.Call<AndroidJavaObject>("putExtra", IntentClass.GetStatic<string>("EXTRA_TEXT"),
                ShareExtraText[Player.LanguageNumber]);
            IntentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject chooser = IntentClass.CallStatic<AndroidJavaObject>("createChooser",
                IntentObject, ShareText[Player.LanguageNumber]);
            currentActivity.Call("startActivity", chooser);

            yield return new WaitForSecondsRealtime(1);
        }

        yield return new WaitUntil(() => Focus);
        Sharing = false;
    }

    private void OnApplicationFocus(bool FocusBool)
    {
        Focus = FocusBool;
    }
}