﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;

public class DialougeManagerV2 : MonoBehaviour
{
    public static DialougeManagerV2 instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private Queue<Sentence> sentences;
    [SerializeField] private Text NPCNameUIObject;
    [SerializeField] private TMP_Text SentenceUIObject;
    [SerializeField] private Image AvatarUIObject;

    [SerializeField] private float speedText = 0.05f;
    private float speedTextStart;
    [SerializeField] private float speedTextFast = 0.025f;

    //Commands
    private List<SpecialCommand> specialCommands;

    //TextMeshPro uses this to adjust the text object if we change its content while a coroutine is going on.
    //This means we can change the dialogue live and the shaking text animation will adjust itself to the new content!
    private bool hasTextChanged = false;

    //Make the text shake, if true before animating
    private bool isTextShaking = true;

    //Related to shaking animation
    [SerializeField] private float AngleMultiplier = 1.0f;
    [SerializeField] private float CurveScale = 1.0f;

    private void Start()
    {
        sentences = new Queue<Sentence>();
        speedTextStart = speedText;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Z))
        {
            speedText = speedTextFast;
        }
        else
        {
            speedText = speedTextStart;
        }
    }

    public void StartDialouge(Dialouge dialouge)
    {
        //Debug.Log("starting dialouge with " + dialouge.sentences[0].nameOfCharacter);
        sentences.Clear();

        foreach(Sentence sentence in dialouge.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }


    private Coroutine coroutine;
    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialouge();
            return;
        }

        
        Sentence sentence = sentences.Dequeue();
        //string nameOfCharacter = ConvertEnumToCharacterName.instance.GetNameOfCharacter(sentence.characterActive);
        DialougeSingleCharacterData characterData = DialougeCharacterData.instance.GetSingleCharacterData(sentence.characterActive);

        if (characterData != null)
        {
            if (sentence.characterAvatar != null)
            {
                Debug.Log("Set to sentence avatar");
                AvatarUIObject.sprite = sentence.characterAvatar;
            }
            else if(characterData.characterAvatar != null)
            {
                Debug.Log("Set to character avatar");
                AvatarUIObject.sprite = characterData.characterAvatar;
            }

            if (characterData.characterName != null)
            {
                NPCNameUIObject.text = characterData.characterName;
            }
        }


        //SentenceUIObject.text = sentence.sentence;
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        

        coroutine = StartCoroutine(AnimateTextCoroutine(sentence.sentence));

        Debug.Log(sentence.sentence);
    }


    
    public void EraseLastSentence()
    {
        
    }


    //Coroutine for animating the dialogue text.
    //Instead of using maxVisibleCharacters, we know animate based on the character's alpha value.
    private IEnumerator AnimateTextCoroutine(string text)
    {
        //Du sätter in text, tar bort alla commands från det och lägger in det i ui-objektet
        SentenceUIObject.text = StripAllCommands(text);
        SentenceUIObject.ForceMeshUpdate();

        specialCommands = BuildSpecialCommandList(text);

        TMP_TextInfo textInfo = SentenceUIObject.textInfo;
        //Här gjorde jag annorlunda
        int totalCharacters = textInfo.characterCount;

        //Color of all chracters vertices
        Color32[] newVertexColors;

        //Base color for our text.
        Color32 c0 = SentenceUIObject.color;

        if(isTextShaking)
        {
            //StartCoroutine(ShakingText());
        }

        //Hides text based on each character's alpha value
        HideText();

        //SentenceUIObject.text = "";
        int i = 0;

        while(i < totalCharacters)
        {

            //If we change the text live on runtime in our inspector, adjust the character count
            //Do i really need this?? For my project, I already have functionality where 
            //the sentences are updated each by their own, so this shouldnt really be needed I think
            if(hasTextChanged)
            {
                totalCharacters = textInfo.characterCount;
                hasTextChanged = false;
            }

            /*  Note: implementing a color command is easy now! All you need to do is
             *  extract the value, create a bool isColorizing = true, and use this color instead
             *  of the base c0 color. A second command can put isColorizing to false.
             *  I leave it up to you to figure this out.
            */
            if(specialCommands.Count > 0)
            {
                CheckForCommands(i);
            }

            //Instead of incrementing maxVisibleCharacters or add the current character to our string, we do this :

            // Get the index of the material used by the current character.
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            // Get the vertex colors of the mesh used by this text element (character or sprite).
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            // Only change the vertex color if the text element is visible. (It's visible, only the alpha color is 0)
            if(textInfo.characterInfo[i].isVisible)
            {
                newVertexColors[vertexIndex + 0] = c0;
                newVertexColors[vertexIndex + 1] = c0;
                newVertexColors[vertexIndex + 2] = c0;
                newVertexColors[vertexIndex + 3] = c0;

                // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
                SentenceUIObject.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }

            i++;
            yield return new WaitForSeconds(speedText);

        }
        
        Debug.Log("End of animations");
    }

    //Hide our text by making all our characters invisible
    private void HideText()
    {
        SentenceUIObject.ForceMeshUpdate();

        TMP_TextInfo textInfo = SentenceUIObject.textInfo;

        Color32[] newVertexColors;
        Color32 c0 = SentenceUIObject.color;

        for(int i = 0; i < textInfo.characterCount; i++)
        {
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            //Get the vertex colors of the mesh used by this text element (Character or sprite)
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            //Alpha = 0
            c0 = new Color32(c0.r, c0.g, c0.b, 0);

            //Apply it to all vertex.
            newVertexColors[vertexIndex + 0] = c0;
            newVertexColors[vertexIndex + 1] = c0;
            newVertexColors[vertexIndex + 2] = c0;
            newVertexColors[vertexIndex + 3] = c0;

            // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
            SentenceUIObject.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }


    //No clue what these does
    void OnEnable()
    {
        // Subscribe to event fired when text object has been regenerated.
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
    }

    void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
    }


    // Event received when the text object has changed.
    void ON_TEXT_CHANGED(Object obj)
    {
        hasTextChanged = true;
    }

    /// <summary>
    /// Structure to hold pre-computed animation data.
    /// </summary>
    private struct VertexAnim
    {
        public float angleRange;
        public float angle;
        public float speed;
    }

    //Shaking example taken from the TextMeshPro demo.
    private IEnumerator ShakingText()
    {

        // We force an update of the text object since it would only be updated at the end of the frame. Ie. before this code is executed on the first frame.
        // Alternatively, we could yield and wait until the end of the frame when the text object will be generated.
        SentenceUIObject.ForceMeshUpdate();

        TMP_TextInfo textInfo = SentenceUIObject.textInfo;

        Matrix4x4 matrix;

        int loopCount = 0;
        hasTextChanged = true;

        // Create an Array which contains pre-computed Angle Ranges and Speeds for a bunch of characters.
        VertexAnim[] vertexAnim = new VertexAnim[1024];
        for (int i = 0; i < 1024; i++)
        {
            vertexAnim[i].angleRange = Random.Range(10f, 25f);
            vertexAnim[i].speed = Random.Range(1f, 3f);
        }

        // Cache the vertex data of the text object as the Jitter FX is applied to the original position of the characters.
        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        while (true)
        {

            // Get new copy of vertex data if the text has changed.
            if (hasTextChanged)
            {
                // Update the copy of the vertex data for the text object.
                cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
                hasTextChanged = false;
            }

            int characterCount = textInfo.characterCount;

            // If No Characters then just yield and wait for some text to be added
            if (characterCount == 0)
            {
                yield return new WaitForSeconds(0.25f);
                continue;
            }


            for (int i = 0; i < characterCount; i++)
            {

                // Retrieve the pre-computed animation data for the given character.
                VertexAnim vertAnim = vertexAnim[i];

                // Get the index of the material used by the current character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                // Get the index of the first vertex used by this text element.
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Get the cached vertices of the mesh used by this text element (character or sprite).
                Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;

                // Determine the center point of each character.
                Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
                // This is needed so the matrix TRS is applied at the origin for each character.
                Vector3 offset = charMidBasline;

                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                vertAnim.angle = Mathf.SmoothStep(-vertAnim.angleRange, vertAnim.angleRange, Mathf.PingPong(loopCount / 25f * vertAnim.speed, 1f));
                Vector3 jitterOffset = new Vector3(Random.Range(-.25f, .25f), Random.Range(-.25f, .25f), 0);

                matrix = Matrix4x4.TRS(jitterOffset * CurveScale, Quaternion.Euler(0, 0, Random.Range(-5f, 5f) * AngleMultiplier), Vector3.one);

                destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                destinationVertices[vertexIndex + 0] += offset;
                destinationVertices[vertexIndex + 1] += offset;
                destinationVertices[vertexIndex + 2] += offset;
                destinationVertices[vertexIndex + 3] += offset;

                vertexAnim[i] = vertAnim;
            }

            // Push changes into meshes
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                SentenceUIObject.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            loopCount += 1;

            yield return new WaitForSeconds(0.1f);
        }
    }










    //Used to build a list containing ALL our commands
    private List<SpecialCommand> BuildSpecialCommandList(string text)
    {
        List<SpecialCommand> listCommand = new List<SpecialCommand>();

        string command = ""; //Current command name
        char[] squiggles = { '{', '}' };    //Trim these characters when the command is found.

        //Go through the dialogue line, get all our special commands.
        for (int i = 0; i < text.Length; i++)
        {
            string currentChar = text[i].ToString();
            //If true, it's at the start of a command
            if (currentChar == "{")
            {
                //Get the full command
                while(currentChar != "}" && i < text.Length)
                {
                    currentChar = text[i].ToString(); //Needed as we remove characters from the text.
                    command += currentChar; //Adds each character in the command in the text to the "command" variable
                    text = text.Remove(i, 1); //removes the current character to get to the next. This is so we'll remove the command from the text that'll be displayed
                }

                if(currentChar == "}")
                {
                    command = command.Trim(squiggles); //Trims away the { and } from the actual command
                    SpecialCommand newCommand = GetSpecialCommand(command); //Gets command name and value
                    newCommand.Index = i; //Command index position in the string
                    listCommand.Add(newCommand); //Adds to the list
                    command = ""; //Reset so we can use the command again

                    //Take a step back otherwise a character will be skipped. 
                    //i = 0 also works, but it means going through characters we already checked.
                    i--;
                }
                else
                {
                    Debug.Log("Command in dialogue line not closed.");
                }
            }
        }

        return listCommand;
    }

    //We use regex to strip all {commands} from our current dialogue line.
    //We have two strings: one with commands and the one printing on screen.
    //We keep track of both in order to know when there's a command to execute, if any.
    private string StripAllCommands(string text)
    {
        //Clean string to return.
        string cleanString;

        //Regex Pattern. Remove all "{stuff:value}" from our dialogue line.
        string pattern = "\\{.[^}]+\\}";

        cleanString = Regex.Replace(text, pattern, "");
        return cleanString;
    }


    //Since our command is {command:value}, we want to extract the name of the command and its value.
    private SpecialCommand GetSpecialCommand(string text)
    {
        SpecialCommand newCommand = new SpecialCommand();

        //Regex to get the command name and the commands values. Used to split the command/values
        string commandRegex = "[:]";

        //Split the command and its values.
        string[] matches = Regex.Split(text, commandRegex);

        //Get the command and its values
        if(matches.Length > 0)
        {
            for(int i = 0; i < matches.Length; i++)
            {
                if (i == 0)
                    newCommand.Name = matches[i];
                else
                {
                    newCommand.Values.Add(matches[i]);
                }
            }
        }
        else
        {
            //Oh no..
            return null;
        }
        return newCommand;
    }

    //Check all commands in a given index. 
    //It's possible to have two commands next to each other in the dialogue line.
    //This means both will share the same index.
    private void CheckForCommands(int index)
    {
        for (int i = 0; i < specialCommands.Count; i++)
        {
            if (specialCommands[i].Index == index)
            {
                //Execute if found a match.
                ExecuteCommand(specialCommands[i]);

                //Remove it.
                specialCommands.RemoveAt(i);

                //Take a step back since we removed one command from the list. Otherwise, the script will skip one command.
                i--;
            }
        }
    }

    private void ExecuteCommand(SpecialCommand command)
    {
        if(command == null)
        {
            return;
        }

        Debug.Log("Command " + command.Name + " found");

        if(command.Name == "sound")
        {
            Debug.Log("BOOOOM! Command played a sound");
        }
        else
        {
            Debug.Log("Command " + command.Name + " doesn't exist");
        }
    }



    public void EndDialouge()
    {
        Debug.Log("End of conversation");
    }



    
}


class SpecialCommand
{
    //Command name
    public string Name { get; set; }

    //A list of all values needed for our command. 
    //If you only need one value per command, consider not making this a list.
    public List<string> Values { get; set; }

    //Which character index should we execute this command.
    public int Index { get; set; }

    public SpecialCommand()
    {
        Name = "";
        Values = new List<string>();
        Index = 0;
    }
}
