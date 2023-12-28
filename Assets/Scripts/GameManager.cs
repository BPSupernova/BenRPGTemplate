using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public CharStats[] playerStats;

    public bool gameMenuOpen, dialogActive, fadingBetweenAreas, shopActive, battleActive;

    public string[] itemsHeld;
    public int[] numberOfItems;
    public Item[] referenceItems;

    public int currentGold;

    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject); 
        SortItems();   
    }

    void Update()
    {
        RestrainPlayerMovementWhenDoingSomething();
        // if (Input.GetKeyDown(KeyCode.J)) {
        //     AddItem("Cheap Robe");
        //     RemoveItem("Apple Juice");
        // }

        // if (Input.GetKeyDown(KeyCode.P)) {
        //     SaveData();
        // }

        // if (Input.GetKeyDown(KeyCode.G)) {
        //     LoadData();
        // }
    }

    private void RestrainPlayerMovementWhenDoingSomething()
    {
        if (gameMenuOpen || dialogActive || fadingBetweenAreas || shopActive || battleActive)
        {
            PlayerController.instance.canMove = false;
        }
        else
        {
            PlayerController.instance.canMove = true;
        }
    }

    public Item GetItemDetails(string itemToGrab) {
        for (int i = 0; i < referenceItems.Length; i++) {
            if (referenceItems[i].itemName == itemToGrab) {
                return referenceItems[i];
            }
        }
        return null;
    }

    public void SortItems() {
        bool itemAfterSpace = true;

        while (itemAfterSpace) 
        {
            itemAfterSpace = false;
            for (int i = 0; i < itemsHeld.Length - 1; i++) {
                if (itemsHeld[i] == "") {
                    itemsHeld[i] = itemsHeld[i+1];
                    itemsHeld[i+1] = "";

                    numberOfItems[i] = numberOfItems[i+1];
                    numberOfItems[i+1] = 0;

                    if (itemsHeld[i] != "") {
                        itemAfterSpace = true;
                    }
                }
            }
        }
    }

    public void AddItem(string itemToAdd) {
        int newItemPosition = 0;
        bool foundSpace = false;
        for (int i = 0; i < itemsHeld.Length; i++) {
            if (itemsHeld[i] == "" || itemsHeld[i] == itemToAdd) {
                newItemPosition = i;
                i = itemsHeld.Length;
                foundSpace = true;
            }
        }
        if (foundSpace) {
            bool itemExists = false;
            for (int i = 0; i < referenceItems.Length; i++) {
                if (referenceItems[i].itemName == itemToAdd) { 
                        itemExists = true;
                        i = referenceItems.Length; 
                    }
            }

            if (itemExists) {
                itemsHeld[newItemPosition] = itemToAdd;
                numberOfItems[newItemPosition]++;
            } else {
                Debug.LogError(itemToAdd + " Does not exist");
            }
        }
        GameMenu.instance.ShowItems();
    }

    public void RemoveItem(string itemToRemove) {
        bool foundItem = false;
        int itemToRemovePosition = 0;
        for (int i = 0; i < itemsHeld.Length; i++) {
            if (itemsHeld[i] == itemToRemove) {
                foundItem = true;
                itemToRemovePosition = i;
                i = itemsHeld.Length;
            }
        }

        if (foundItem) {
            numberOfItems[itemToRemovePosition]--;
            if (numberOfItems[itemToRemovePosition] <= 0) {
                itemsHeld[itemToRemovePosition] = "";
            }
            GameMenu.instance.ShowItems();
        } else {
            Debug.LogError("Couldn't find " + itemToRemove);
        }
    }

    public void SaveData() {
        PlayerPrefs.SetString("Current_Scene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("Player_Position_x", PlayerController.instance.transform.position.x);
        PlayerPrefs.SetFloat("Player_Position_y", PlayerController.instance.transform.position.y);
        PlayerPrefs.SetFloat("Player_Position_z", PlayerController.instance.transform.position.z);
    
        // Save character info
        for (int i = 0; i < playerStats.Length; i++) {
            if (playerStats[i].gameObject.activeInHierarchy) {
                PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_active", 1);
            } else {
                PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_active", 0);
            }

            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_Level", playerStats[i].charLevel);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_CurrentExp", playerStats[i].currentEXP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_CurrentHP", playerStats[i].currentHealth);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_MaxHP", playerStats[i].maxHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_CurrentTP", playerStats[i].currentTalent);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_MaxTP", playerStats[i].maxTalentPoints);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_Attack", playerStats[i].attack);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_Defense", playerStats[i].defense);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_WeaponPower", playerStats[i].weaponPower);
            PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_ArmourStrength", playerStats[i].armourPower);
            PlayerPrefs.SetString("Player_" + playerStats[i].charName + "_EquippedWeapon", playerStats[i].equippedWeapon);
            PlayerPrefs.SetString("Player_" + playerStats[i].charName + "_EquippedArmour", playerStats[i].equippedArmour);
        }

        // Store inventory data
        for (int i = 0; i < itemsHeld.Length; i++) {
            PlayerPrefs.SetString("ItemInInventory_" + i, itemsHeld[i]);
            PlayerPrefs.SetInt("ItemAmount_" + i, numberOfItems[i]);
        }
    }

    public void LoadData() {
        PlayerController.instance.transform.position = new Vector3(PlayerPrefs.GetFloat("Player_Position_x"), PlayerPrefs.GetFloat("Player_Position_y"), PlayerPrefs.GetFloat("Player_Position_z"));

        for (int i = 0; i < playerStats.Length; i++) {
            if (PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_active") == 0) {
                playerStats[i].gameObject.SetActive(false);
            } else {
                playerStats[i].gameObject.SetActive(true);
            }

            playerStats[i].charLevel = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_Level");
            playerStats[i].currentEXP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_CurrentExp");
            playerStats[i].currentHealth = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_CurrentHP");
            playerStats[i].maxHP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_MaxHP");
            playerStats[i].currentTalent = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_CurrentTP");
            playerStats[i].maxTalentPoints = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_MaxTP");
            playerStats[i].attack = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_Attack");
            playerStats[i].defense = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_Defense");
            playerStats[i].weaponPower = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_WeaponPower");
            playerStats[i].armourPower = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_ArmourStrength");
            playerStats[i].equippedWeapon = PlayerPrefs.GetString("Player_" + playerStats[i].charName + "_EquippedWeapon");
            playerStats[i].equippedArmour = PlayerPrefs.GetString("Player_" + playerStats[i].charName + "_EquippedArmour");
        }

        for (int i = 0; i < itemsHeld.Length; i++) {
            itemsHeld[i] = PlayerPrefs.GetString("ItemInInventory_" + i);
            numberOfItems[i] = PlayerPrefs.GetInt("ItemAmount_" + i);
        }
    }
}
