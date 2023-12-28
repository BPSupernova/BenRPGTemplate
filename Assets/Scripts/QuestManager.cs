using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public string[] questMarkerNames;
    public bool[] questMarkersComplete;

    public static QuestManager instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        questMarkersComplete = new bool[questMarkerNames.Length];
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Q)) {
        //     Debug.Log(CheckIfComplete("Test Quest"));
        //     MarkQuestComplete("Test Quest");
        //     MarkQuestIncomplete("Defeat The Final Boss");
        // }

        // if (Input.GetKeyDown(KeyCode.P)) {
        //     SaveQuestData();
        // }

        // if (Input.GetKeyDown(KeyCode.G)) {
        //     LoadQuestData();
        // }
    }

    public int GetQuestNum(string questToFind) {
        for (int i = 0; i < questMarkerNames.Length; i++) {
            if (questMarkerNames[i] == questToFind) {
                return i;
            }
        }

        Debug.LogError("Quest " + questToFind + " does not exist");
        return 0;
    }

    public bool CheckIfComplete(string questToCheck) {
        if (GetQuestNum(questToCheck) != 0) {
            return questMarkersComplete[GetQuestNum(questToCheck)];
        }        

        return false;
    }

    public void MarkQuestComplete(string questToCheck) {
        questMarkersComplete[GetQuestNum(questToCheck)] = true;
        UpdateLocalQuestObjects();
    }

    public void MarkQuestIncomplete(string questToCheck) {
        questMarkersComplete[GetQuestNum(questToCheck)] = false;
        UpdateLocalQuestObjects();
    }

    public void UpdateLocalQuestObjects() {
        QuestObjActivator[] questObjs = FindObjectsOfType<QuestObjActivator>();
        if (questObjs.Length > 0) {
            for (int i = 0; i < questObjs.Length; i++) {
                questObjs[i].CheckCompletion();
            }
        }
    }

    public void SaveQuestData() {
        for (int i = 0; i < questMarkerNames.Length; i++) {
            if (questMarkersComplete[i]) {
                PlayerPrefs.SetInt("QuestMarker_" + questMarkerNames[i] , 1);
            } else {
                PlayerPrefs.SetInt("QuestMarker_" + questMarkerNames[i] , 0);
            }
        }
    }

    public void LoadQuestData() {
        for (int i = 0; i < questMarkerNames.Length; i++) {
            int valueToSet = 0;
            if (PlayerPrefs.HasKey("QuestMarker_" + questMarkerNames[i])) {
                valueToSet = PlayerPrefs.GetInt("QuestMarker_" + questMarkerNames[i]);
            }

            if (valueToSet == 0) {
                questMarkersComplete[i] = false;
            } else {
                questMarkersComplete[i] = true;
            }
        }
    }
}
