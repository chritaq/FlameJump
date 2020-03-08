using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;

public class DialougeManager : MonoBehaviour
{
    public static DialougeManager instance = null;

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

    private IEnumerator AnimateTextCoroutine(string text)
    {
        SentenceUIObject.text = "";
        int i = 0;

        //Get all special commands in the text before printing. Hold our special commands in a dictionary.
        specialCommands = BuildSpecialCommandList(text);

        string stripText = StripAllCommands(text);

        while(i < stripText.Length)
        {
            // Check if we have commands in the current dialogue line.. If so, execute it.
            if (specialCommands.Count > 0)
            {
                CheckForCommands(i);    //feed it the current character index.
            }

            SentenceUIObject.text += stripText[i];
            i++;
            yield return new WaitForSeconds(speedText);
        }

        Debug.Log("End of animations");
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
