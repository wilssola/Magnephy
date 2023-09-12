using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RandomText : MonoBehaviour
{
    public string[] Prefix;
    public string[] SuffixPortuguese;
    public string[] SuffixEnglish;

    // Use this for initialization.
    private void OnEnable()
    {
        Text TextComponent = GetComponent<Text>();

        int RandomSuffix = Random.Range(0, SuffixPortuguese.Length);

        TextComponent.text = Prefix[Player.LanguageNumber];

        if (Player.LanguageNumber == 0)
        {
            TextComponent.text = TextComponent.text + SuffixPortuguese[RandomSuffix];
        }
        if (Player.LanguageNumber == 1)
        {
            TextComponent.text = TextComponent.text + SuffixEnglish[RandomSuffix];
        }
    }
}
