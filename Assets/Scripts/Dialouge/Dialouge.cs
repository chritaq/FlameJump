using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public enum DialougeCharacterActive
{
    Player,
    ChunkySteve,
    Sprite,
    Other
}

public enum SentenceCharacterActive
{

}

[CreateAssetMenu(fileName = "DialougeData", menuName = "ScriptableObjects/DialougeScriptableObject", order = 1)]
public class Dialouge : ScriptableObject
{
    public DialougeCharacterActive[] charactersActive;
    public Sentence[] sentences;
}

[System.Serializable]
public class Sentence
{
    public DialougeCharacterActive characterActive;

    public bool leftSide;
    //public string nameOfCharacter;

    public Sprite characterAvatar;
    [TextArea(3, 10)]
    public string sentence;
}
