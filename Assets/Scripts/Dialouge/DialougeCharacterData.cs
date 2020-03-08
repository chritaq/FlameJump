using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialougeCharacterData : MonoBehaviour
{
    public static DialougeCharacterData instance = null;
    //DialougeSingleCharacterData chunkySteve = new DialougeSingleCharacterData();
    //DialougeSingleCharacterData player = new DialougeSingleCharacterData();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public DialougeSingleCharacterData GetSingleCharacterData(DialougeCharacterActive character) {
        switch(character)
        {
            case DialougeCharacterActive.Player:
                return new DialougeSingleCharacterData(PlayerImage, PlayerName);
            case DialougeCharacterActive.ChunkySteve:
                return new DialougeSingleCharacterData(ChunkySteveImage, ChunkySteveName);
            default:
                break;
        }

        return null;
    }

    [SerializeField] private Sprite PlayerImage;
    [SerializeField] private string PlayerName;


    [SerializeField] private Sprite ChunkySteveImage;
    [SerializeField] private string ChunkySteveName;
}


public class DialougeSingleCharacterData
{
    public string characterName;
    public Sprite characterAvatar;

    public DialougeSingleCharacterData(Sprite newCharacterImage, string newCharacterName)
    {
        characterAvatar = newCharacterImage;
        characterName = newCharacterName;
    }

    
}