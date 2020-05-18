﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialougeCharacterData", menuName = "Dialouge/CharacterData", order = 1)]
public class DialougeCharacterDataV2 : ScriptableObject
{
    public Sprite characterAvatar;
    public string characterName;
    [Header("Leave empty for standard sound")]
    public SoundFileData characterAudioDot;
}
