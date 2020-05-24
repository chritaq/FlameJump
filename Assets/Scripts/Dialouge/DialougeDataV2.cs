using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectDropdown;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DialougeData", menuName = "Dialouge/DialougeData", order = 1)]
public class DialougeDataV2 : ScriptableObject
{
    [Header("CHARACTERS")]
    [ScriptableObjectDropdown(typeof(DialougeCharacterDataV2))] public ScriptableObjectReference character1;
    [ScriptableObjectDropdown(typeof(DialougeCharacterDataV2))] public ScriptableObjectReference character2;
    [ScriptableObjectDropdown(typeof(DialougeCharacterDataV2))] public ScriptableObjectReference character3;
    [ScriptableObjectDropdown(typeof(DialougeCharacterDataV2))] public ScriptableObjectReference character4;
    [ScriptableObjectDropdown(typeof(DialougeCharacterDataV2))] public ScriptableObjectReference character5;
    [Space]
    [Space]
    [Header("SENTENCES")]
    public SentenceV2[] sentences;
    [HideInInspector] private DialougeCharacterDataV2[] dialougeCharacters;
}

[System.Serializable]
public class SentenceV2
{
    public enum WordingStyle
    {
        standard,
        fullWords,
        fullSentence
    }

    public CharacterDialougeSelection character;
    public Sprite emotionSprite;
    public bool leftSide;

    [Space]
    public WordingStyle wordingStyle;
    public float textSpeedMultiplier = 1;
    public bool lockInput;
    [Space]
    [TextArea(3, 10)]
    public string sentence;
}

public enum CharacterDialougeSelection
{
    characterOne,
    characterTwo,
    characterThree,
    characterFour,
    characterFive
}



