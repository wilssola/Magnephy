﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenURL : MonoBehaviour {

    public string Adress;

	public void Browse ()
    {
        Application.OpenURL(Adress);
    }
}
