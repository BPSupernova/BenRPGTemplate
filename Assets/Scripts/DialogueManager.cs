using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text dialogText;
    public Text nameText;
    public GameObject dialogBox;
    public GameObject nameBox;

    public string[] dialougeLines;
    public int currentLineIndex;

    private bool justStarted;

    public static DialogueManager instance;

    private string questToMark;
    private bool markQuestComplete;
    private bool shouldMarkQuest;

    void Start()
    {
        instance = this;
    }

    void Update()    
    {
        if (dialogBox.activeInHierarchy) {
            if (Input.GetButtonUp("Fire1"))
            {
                StartDialogueBox();
            }
        }
    }

    private void StartDialogueBox()
    {
        if (!justStarted)
        {
            currentLineIndex++;
            if (currentLineIndex >= dialougeLines.Length)
            {
                dialogBox.SetActive(false);
                GameManager.instance.dialogActive = false;

                if (shouldMarkQuest) {
                    shouldMarkQuest = false;
                    if (markQuestComplete) {
                        QuestManager.instance.MarkQuestComplete(questToMark);
                    } else {
                        QuestManager.instance.MarkQuestIncomplete(questToMark);    
                    }
                }
            }
            else
            {
                CheckIfName();
                dialogText.text = dialougeLines[currentLineIndex];
            }
        }
        else
        {
            justStarted = false;
        }
    }

    public void ShowDialogue(string[] newLines, bool isPerson) {
        dialougeLines = newLines;
        currentLineIndex = 0;
        CheckIfName();
        dialogText.text = dialougeLines[currentLineIndex];
        dialogBox.SetActive(true);
        justStarted = true;

        nameBox.SetActive(isPerson);

        GameManager.instance.dialogActive = true;
    }

    public void CheckIfName() {
        if (dialougeLines[currentLineIndex].StartsWith("n-")) {
            nameText.text = dialougeLines[currentLineIndex].Replace("n-", "");
            currentLineIndex++;
        }
    }

    public void ShouldActivateQuestAtEnd(string questName, bool markComplete) {
        questToMark = questName;
        markQuestComplete = markComplete;
        shouldMarkQuest = true;
    }
}
