using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ConvertEnumToCharacterName : MonoBehaviour
{
    public static ConvertEnumToCharacterName instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public string GetNameOfCharacter(object characterActive)
    {
        return Enum.GetName(typeof(DialougeCharacterActive), characterActive);
    }

    //public Image GetCharacterImage(object characterActive)
    //{

    //    return null;
    //}
}