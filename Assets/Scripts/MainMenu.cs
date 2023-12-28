using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string loadGameScene;
    public string newGameScene;

    public GameObject continueButton;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Current_Scene")) {
            continueButton.SetActive(true);
        } else {
            continueButton.SetActive(false);
        }
    }

    public void Continue() {
        SceneManager.LoadScene(loadGameScene);
    }

    public void NewGame() {
        SceneManager.LoadScene(newGameScene);
    }

    public void ExitApplication() {
        Application.Quit();
    }
}
