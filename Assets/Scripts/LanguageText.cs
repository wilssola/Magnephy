using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageText : MonoBehaviour {
    

    public string[] Text;
 
	// Use this for initialization
	void Start () {

        Text TextComponent = GetComponent<Text>();

        TextComponent.text = Text[Player.LanguageNumber];

    }
}
