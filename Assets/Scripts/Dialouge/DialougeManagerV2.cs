using UnityEngine;
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
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);
    }





    [Space]
    [Header("Dialouge objects")]
    private Queue<Sentence> sentences;
    [SerializeField] private Text NPCNameUIObject;
    [SerializeField] private TMP_Text SentenceUIObject;
    [SerializeField] private Image AvatarUIObject;
    private Coroutine animateTextCoroutine;
    private bool inDialouge = false;


    [Space]
    [Header("Dialouge Controllers")]
    private bool displayFullSentence = false;
    [HideInInspector] public bool endOfAnimations = false;
    private bool lockPlayerInputForRestOfSentence = false;
    private bool goToNextSentenceAutomatically = false;


    [Space]
    [Header("Dialouge speeds")]
    [SerializeField] private float speedText = 0.05f;
    [SerializeField] private float speedTextDot = 0.08f;
    [SerializeField] private float speedTextComma = 0.08f;
    private float speedTextStart;
    private float speedTextDotStart;
    private float speedTextCommaStart;
    [SerializeField] private float textSpeedMultiplier = 2;
    private float commandTextSpeedMultiplier = 1;
    private bool displayFullWordAtTheTime = false;


    [HideInInspector]
    [Header("Commands")]
    private List<SpecialCommand> specialCommands;

    //TextMeshPro uses this to adjust the text object if we change its content while a coroutine is going on.
    //This means we can change the dialogue live and the shaking text animation will adjust itself to the new content!
    private bool hasTextChanged = false;


    
    [Space]
    [Header("FXs")]
    //Make the text shake, if true before animating
    [SerializeField] private bool isTextShaking = false;

    //Related to shaking animation. Might remove this from this script later?
    [SerializeField] private float AngleMultiplier = 1.0f;
    [SerializeField] private float CurveScale = 1.0f;



    [Space]
    [Header("Audio")]
    [SerializeField] private AudioClip basicDotSound;
    [SerializeField] private AudioSource dotSoundAudioSource;



    [Space]
    [Header("Misc")]
    [SerializeField] private Canvas canvas;
    [HideInInspector] public int sentencesLeft;


    [Space]
    [Header("Testing & Debug")]
    public Dialouge testDialouge;





    private void Start()
    {
        sentencesLeft = 0;
        sentences = new Queue<Sentence>();
        speedTextStart = speedText;
        speedTextDotStart = speedTextDot;
        speedTextCommaStart = speedTextComma;
    }

    public void SpeedUpDialouge()
    {
        if(!lockPlayerInputForRestOfSentence)
        {
            speedText = speedTextStart / textSpeedMultiplier;
            speedTextDot = speedTextDotStart / textSpeedMultiplier;
            speedTextComma = speedTextCommaStart / textSpeedMultiplier;
        }
    }

    public void SetDialougeSpeedToNormal()
    {
        speedText = speedTextStart;
        speedTextDot = speedTextDotStart;
        speedTextComma = speedTextCommaStart;
    }

    public void QuicklySkipText()
    {
        if(!lockPlayerInputForRestOfSentence)
        {
            displayFullSentence = true;
        }
        
    }


    /*    DIALOUGE MANAGEMENT    */

    public void StartDialouge(Dialouge dialouge)
    {
        inDialouge = true;
        canvas.enabled = true;
        ClearAndAddNewSentences(dialouge);
        DisplayNextSentence();
    }

    private void ClearAndAddNewSentences(Dialouge dialouge)
    {
        sentences.Clear();
        foreach (Sentence sentence in dialouge.sentences)
        {
            sentences.Enqueue(sentence);
        }
        sentencesLeft = sentences.Count;
    }

    public void DisplayNextSentence()
    {
        endOfAnimations = false;
        lockPlayerInputForRestOfSentence = false;
        displayFullWordAtTheTime = false;

        if (sentences.Count == 0)
        {
            EndDialouge();
            return;
        }

        Sentence sentence = sentences.Dequeue();
        sentencesLeft = sentences.Count;
        SetNameAvatarAndAudioDotInSentence(sentence);

        //SentenceUIObject.text = sentence.sentence;
        if(animateTextCoroutine != null)
        {
            StopCoroutine(animateTextCoroutine);
        }
        
        animateTextCoroutine = StartCoroutine(AnimateTextCoroutine(sentence.sentence));

        Debug.Log(sentence.sentence);
    }

    public void EndDialouge()
    {
        inDialouge = false;
        StopCoroutine(animateTextCoroutine);
        canvas.enabled = false;
        Debug.Log("End of conversation");
    }



    /*    SINGLE SENTENCE MANAGEMENT    */

    private void SetNameAvatarAndAudioDotInSentence(Sentence sentence)
    {
        DialougeSingleCharacterData characterData = DialougeCharacterData.instance.GetSingleCharacterData(sentence.characterActive);

        dotSoundAudioSource.clip = basicDotSound;

        if (characterData != null)
        {
            if (sentence.characterAvatar != null)
            {
                Debug.Log("Set to sentence avatar");
                AvatarUIObject.sprite = sentence.characterAvatar;
            }
            else if (characterData.characterAvatar != null)
            {
                Debug.Log("Set to character avatar");
                AvatarUIObject.sprite = characterData.characterAvatar;
            }
            if (characterData.characterName != null)
            {
                NPCNameUIObject.text = characterData.characterName;
            }
            if(characterData.characterAudioDot != null)
            {
                dotSoundAudioSource.clip = characterData.characterAudioDot;
            }
        }
    }

    private IEnumerator AnimateTextCoroutine(string text)
    {
        

        //Du sätter in text, tar bort alla commands från det och lägger in det i ui-objektet
        StripCommandsFromTextAndCreateCommandList(text);

        if(isTextShaking)
        {
            StartCoroutine(ShakingText());
        }

        //Hides text based on each character's alpha value
        HideText();

        TMP_TextInfo textInfo = SentenceUIObject.textInfo;
        //Här gjorde jag annorlunda
        int totalCharacters = textInfo.characterCount;

        //Base color for our text.
        Color32 baseColor = SentenceUIObject.color;

        //SentenceUIObject.text = "";
        int i = 0;

        //Loop for the text
        while(i < totalCharacters)
        {
            //Every other
            //if((i % 2) == 0)


            /*  Note: implementing a color command is easy now! All you need to do is
             *  extract the value, create a bool isColorizing = true, and use this color instead
             *  of the base c0 color. A second command can put isColorizing to false.
             *  I leave it up to you to figure this out.
            */
            if (specialCommands.Count > 0)
            {
                CheckForCommands(i);
            }

            ShowCharacter(i, textInfo, baseColor);

            if(!displayFullSentence)
            {
                //If not in perWordMode
                if(!displayFullWordAtTheTime)
                {
                    yield return new WaitForSeconds(SetDelayBeforeNextLetter(textInfo, i));
                }
                else
                {
                    if (textInfo.characterInfo[i].character == ' ')
                    {
                        yield return new WaitForSeconds(SetDelayBeforeNextWord(textInfo, i));
                        
                    }
                    else
                    {
                        PlayDotSound();
                        //TODO
                        //Fix so dotsound only plays once per word
                        //PlayDotSound();
                        //Should be nothing here as it's taken care of above?
                    }
                    
                }

                while (pauseTime > 0)
                {
                    pauseTime -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
            }

            


            i++;
        }

        ResetValuesForDialougeAnimation();

        if (goToNextSentenceAutomatically)
        {
            goToNextSentenceAutomatically = false;
            DisplayNextSentence();
        }
        
        Debug.Log("End of animations");
    }

    private void ResetValuesForDialougeAnimation()
    {
        displayFullSentence = false;
        endOfAnimations = true;
        commandTextSpeedMultiplier = 1;
    }

    private void StripCommandsFromTextAndCreateCommandList(string text)
    {
        SentenceUIObject.text = StripAllCommands(text);
        SentenceUIObject.ForceMeshUpdate();
        specialCommands = BuildSpecialCommandList(text);
    }

    private void ShowCharacter(int i, TMP_TextInfo textInfo, Color32 c0)
    {
        //Color of all chracters vertices
        Color32[] newVertexColors;

        //Instead of incrementing maxVisibleCharacters or add the current character to our string, we do this :

        // Get the index of the material used by the current character.
        int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

        // Get the vertex colors of the mesh used by this text element (character or sprite).
        newVertexColors = textInfo.meshInfo[materialIndex].colors32;

        // Get the index of the first vertex used by this text element.
        int vertexIndex = textInfo.characterInfo[i].vertexIndex;

        // Only change the vertex color if the text element is visible. (It's visible, only the alpha color is 0)
        if (textInfo.characterInfo[i].isVisible)
        {
            newVertexColors[vertexIndex + 0] = c0;
            newVertexColors[vertexIndex + 1] = c0;
            newVertexColors[vertexIndex + 2] = c0;
            newVertexColors[vertexIndex + 3] = c0;

            // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
            SentenceUIObject.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }

    private float SetDelayBeforeNextLetter(TMP_TextInfo textInfo, int i)
    {
        char character = textInfo.characterInfo[i].character;

        //Set delay to 0 if it's a space
        if (character == ' ')
        {
            return 0f;
        }

        if (character == '.' || character == '?' || character == '!')
        {
            PlayDotSound();
            return speedTextDot / commandTextSpeedMultiplier;
        }
        else if (character == ',')
        {
            PlayDotSound();
            return speedTextComma / commandTextSpeedMultiplier;
        }
        else
        {
            PlayDotSound();
            return speedText / commandTextSpeedMultiplier;
        }
    }

    private float SetDelayBeforeNextWord(TMP_TextInfo textInfo, int i)
    {
        char character = textInfo.characterInfo[i].character;

        //Set delay to 0 if it's a space
        if (character == ' ')
        {
            return speedTextDot / commandTextSpeedMultiplier;
        }

        return 0;
    }

    private void PlayDotSound()
    {
        dotSoundAudioSource.Stop();
        dotSoundAudioSource.Play();
    }

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




    /*    FXS    */

    //TODO
    //I think these are needed to change the FXs of the text, but I'm unsure. Look it up.
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
    //TODO
    //Create seperate class for FXs
    //TODO
    //Continue re-working the text from here
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





    /*    COMMANDS    */

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
        cleanString = Regex.Replace(cleanString, "   ", " ");
        cleanString = Regex.Replace(cleanString, "  ", " ");
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


    private float pauseTime = 0f;
    private void ExecuteCommand(SpecialCommand command)
    {
        if(command == null)
        {
            return;
        }

        Debug.Log("Command: " + command.Name + " Was started");

        //Dialouge

        if(command.Name == "Speed")
        {
            commandTextSpeedMultiplier = float.Parse(command.Values[0]);
        }

        if(command.Name == "FullWords")
        {
            if (command.Values.Count > 0)
            {
                if(int.Parse(command.Values[0]) == 0)
                {
                    displayFullWordAtTheTime = false;
                }
                else
                {
                    displayFullWordAtTheTime = true;
                }
            }
            else
            {
                displayFullWordAtTheTime = true;
            }
        }

        if(command.Name == "LockInput")
        {
            lockPlayerInputForRestOfSentence = true;
        }
        if(command.Name == "UnlockInput")
        {
            lockPlayerInputForRestOfSentence = false;
        }

        if(command.Name == "NextSentenceAuto")
        {
            goToNextSentenceAutomatically = true;
        }

        if(command.Name == "Pause")
        {
            if(command.Values.Count > 0)
            {
                pauseTime = float.Parse(command.Values[0]);
            }
        }

        //Audio

        if(command.Name == "Sound")
        {
            ServiceLocator.GetAudio().PlaySound(command.Values[0]);
        }


        //FXs

        if(command.Name == "ScreenShake")
        {
            
            if (command.Values.Count <= 0)
            {
                ServiceLocator.GetScreenShake().StartScreenShake(2f, 1f);
            }
            else if(command.Values.Count <= 1)
            {
                Debug.Log(command.Values[0]);
                ServiceLocator.GetScreenShake().StartScreenShake(float.Parse(command.Values[0]), 1f);
            }
            else
            {
                Debug.Log(command.Values[0]);
                Debug.Log(command.Values[1]);
                ServiceLocator.GetScreenShake().StartScreenShake(float.Parse(command.Values[0]), float.Parse(command.Values[1]));
            }
        }

        if (command.Name == "ScreenFlash")
        {

            if (command.Values.Count <= 0)
            {
                ServiceLocator.GetScreenShake().StartScreenFlash(2, 0.3f);
            }
            else if (command.Values.Count <= 1)
            {
                ServiceLocator.GetScreenShake().StartScreenFlash(int.Parse(command.Values[0]), 1f);
            }
            else
            {
                ServiceLocator.GetScreenShake().StartScreenFlash(int.Parse(command.Values[0]), float.Parse(command.Values[1]));
            }
        }

    }

    public bool CheckInDialouge()
    {
        bool newInDialouge = inDialouge;
        return newInDialouge;
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
