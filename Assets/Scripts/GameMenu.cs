using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject theMenu;
    public GameObject[] windows;

    private CharStats[] playerStats;
    
    public Text[] nameText;
    public Text[] hpText, tpText, attackText, defenseText, levelText, expText;
    public Slider[] expSlider;
    public Image[] charImage;
    public GameObject[] charStatHolder;

    public GameObject[] statusButtons;

    public Text statusHP, statusTP, statusAttack, statusDefense, statusWeapon, statusWeaponPow, statusArmour, statusArmourStr, statusEXP;
    public Image statusImage;

    public ItemButton[] itemButtons;
    public string selectedItem;
    public Item activeItem;
    public Text itemName, itemDescription, useButtonText;

    public GameObject itemCharChoicePanel;
    public Text[] itemCharChoiceNames;

    public Text goldText;

    public string mainMenuName;

    public static GameMenu instance;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire2")) {
            if (theMenu.activeInHierarchy) {
                CloseMenu();
            } else {
                theMenu.SetActive(true);
                UpdateMainStats();
                GameManager.instance.gameMenuOpen = true;
            }

            AudioManager.instance.PlaySFX(5);
        }
    }

    public void UpdateMainStats() {
        playerStats = GameManager.instance.playerStats;

        for (int i = 0; i < playerStats.Length; i++) {
            if (playerStats[i].gameObject.activeInHierarchy)
            {
                charStatHolder[i].SetActive(true);
                UpdateStatsInMenu(i);
            }
            else {
                charStatHolder[i].SetActive(false);
            }
        }

        goldText.text = "$" + GameManager.instance.currentGold.ToString();
    }

    void UpdateStatsInMenu(int i)
    {
        nameText[i].text = playerStats[i].charName;
        hpText[i].text = "HP: " + playerStats[i].currentHealth + "/" + playerStats[i].maxHP;
        tpText[i].text = "TP: " + playerStats[i].currentTalent + "/" + playerStats[i].maxTalentPoints;
        attackText[i].text = "Attack: " + playerStats[i].attack;
        defenseText[i].text = "Defense: " + playerStats[i].defense;
        levelText[i].text = "Level: " + playerStats[i].charLevel;
        expText[i].text = "" + playerStats[i].currentEXP + "/" + playerStats[i].expToNextLevel[playerStats[i].charLevel];
        expSlider[i].maxValue = playerStats[i].expToNextLevel[playerStats[i].charLevel];
        expSlider[i].value = playerStats[i].currentEXP;
        charImage[i].sprite = playerStats[i].charImage;
    }

    public void ToggleWindow(int windowIndicator) {
        UpdateMainStats();
        for (int i = 0; i < windows.Length; i++) {
            if (i == windowIndicator) {
                windows[i].SetActive(!windows[i].activeInHierarchy);
            } else {
                windows[i].SetActive(false);
            }
        }
        itemCharChoicePanel.SetActive(false);
    }

    public void CloseMenu() {
        for (int i = 0; i < windows.Length; i++) {
            windows[i].SetActive(false);
        }

        theMenu.SetActive(false);
        GameManager.instance.gameMenuOpen = false;
        itemCharChoicePanel.SetActive(false);
    }

    public void OpenStatus() {
        UpdateMainStats();

        ShowRespectivePlayerStats(0);

        for (int i = 0; i < statusButtons.Length; i++) {
            statusButtons[i].SetActive(playerStats[i].gameObject.activeInHierarchy);
        }
    }

    public void ShowRespectivePlayerStats(int selectedCharIndex) {
        statusHP.text = "" + playerStats[selectedCharIndex].currentHealth + "/" + playerStats[selectedCharIndex].maxHP;
        statusTP.text = "" + playerStats[selectedCharIndex].currentTalent + "/" + playerStats[selectedCharIndex].maxTalentPoints;
        statusAttack.text = playerStats[selectedCharIndex].attack.ToString();
        statusDefense.text = playerStats[selectedCharIndex].defense.ToString();
        
        if (playerStats[selectedCharIndex].equippedWeapon != "") {
            statusWeapon.text = playerStats[selectedCharIndex].equippedWeapon;
        } else { statusWeapon.text = "None"; }
        statusWeaponPow.text = playerStats[selectedCharIndex].weaponPower.ToString();
        
        if (playerStats[selectedCharIndex].equippedArmour != "") {
            statusArmour.text = playerStats[selectedCharIndex].equippedArmour;
        } else { statusArmour.text = "None"; }
        statusArmourStr.text = playerStats[selectedCharIndex].armourPower.ToString();

        statusEXP.text = (playerStats[selectedCharIndex].expToNextLevel[playerStats[selectedCharIndex].charLevel] - playerStats[selectedCharIndex].currentEXP).ToString();

        statusImage.sprite = playerStats[selectedCharIndex].charImage;
    }

    public void ShowItems() {
        GameManager.instance.SortItems();
        
        for (int i = 0; i < itemButtons.Length; i++) {
            itemButtons[i].buttonVal = i;

            if (GameManager.instance.itemsHeld[i] != "") {
                itemButtons[i].buttonImage.gameObject.SetActive(true);
                itemButtons[i].buttonImage.sprite = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]).itemSprite;
                itemButtons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString(); 
            } else {
                itemButtons[i].buttonImage.gameObject.SetActive(false);
                itemButtons[i].amountText.text = "";
            }
        }
    }

    public void SelectItem(Item newItem) {
        activeItem = newItem;

        if (activeItem.isItem) {
            useButtonText.text = "Use";
        }

        if (activeItem.isWeapon || activeItem.isArmour) {
            useButtonText.text = "Equip";
        }

        itemName.text = activeItem.itemName;
        itemDescription.text = activeItem.description;        
    }

    public void DiscardItem() {
        if (activeItem != null) {
            GameManager.instance.RemoveItem(activeItem.itemName);
        }
    }

    public void OpenItemCharSelect() {
        itemCharChoicePanel.SetActive(true);

        for (int i = 0; i < itemCharChoiceNames.Length; i++) {
            itemCharChoiceNames[i].text = GameManager.instance.playerStats[i].charName;
            itemCharChoiceNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[i].gameObject.activeInHierarchy);
        }
    }

    public void CloseItemCharSelect() {
        itemCharChoicePanel.SetActive(false);
    }

    public void UseItemInMenu(int selectedChar) {
        activeItem.Use(selectedChar);
        CloseItemCharSelect();
    }

    public void SaveGame() {
        GameManager.instance.SaveData();
        QuestManager.instance.SaveQuestData();
    }

    public void QuitToTitleScreen() {
        SceneManager.LoadScene(mainMenuName);
        Destroy(GameManager.instance.gameObject);
        
        Destroy(AudioManager.instance.gameObject);
        Destroy(gameObject);
    }

    public void PlayButtonSound() {
        AudioManager.instance.PlaySFX(4);
    }
}
