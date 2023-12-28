using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Type")]
    public bool isItem;
    public bool isWeapon;
    public bool isArmour;

    [Header("Basic Details")]
    public string itemName;
    public string description;

    public int cost;
    public Sprite itemSprite;

    [Header("Item Details")]
    public int amountToChange;

    public bool affectHP, affectTP, affectAttack, affectDefense, canRevive;

    [Header("Weapon/Armour Details")]
    public int weaponStrength;
    public int armourStrength;    

    public void Use(int charToUseOn) {
        CharStats selectedChar = GameManager.instance.playerStats[charToUseOn];

        if (isItem)
        {
            UseItem(selectedChar);
        }

        if (isWeapon)
        {
            EquipWeapon(selectedChar);
        }

        if (isArmour)
        {
            EquipArmour(selectedChar);
        }

        GameManager.instance.RemoveItem(itemName);
    }

    private void UseItem(CharStats selectedChar)
    {
        if (affectHP)
        {
            selectedChar.currentHealth += amountToChange;
            if (selectedChar.currentHealth > selectedChar.maxHP)
            {
                selectedChar.currentHealth = selectedChar.maxHP;
            }
        }

        if (affectTP)
        {
            selectedChar.currentTalent += amountToChange;
            if (selectedChar.currentTalent > selectedChar.maxTalentPoints)
            {
                selectedChar.currentTalent = selectedChar.maxTalentPoints;
            }
        }

        if (affectAttack)
        {
            selectedChar.attack += amountToChange;
        }

        if (affectDefense)
        {
            selectedChar.defense += amountToChange;
        }
    }

    private void EquipWeapon(CharStats selectedChar)
    {
        if (selectedChar.equippedWeapon != "")
        {
            GameManager.instance.AddItem(selectedChar.equippedWeapon);
            selectedChar.equippedWeapon = itemName;
            selectedChar.weaponPower = weaponStrength;
        } else { 
            selectedChar.equippedWeapon = "";
        }
    }

    private void EquipArmour(CharStats selectedChar)
    {
        if (selectedChar.equippedArmour != "")
        {
            GameManager.instance.AddItem(selectedChar.equippedArmour);
            selectedChar.equippedArmour = itemName;
            selectedChar.armourPower = armourStrength;
        } else {
            selectedChar.equippedArmour = "";
        }
    }

    public void UseInBattle(int charToUseOn) {
        CharStats selectedChar = GameManager.instance.playerStats[charToUseOn];
        List<BattleCharacter> activeBattlers = BattleManager.instance.activeBattlers;
        List<BattleCharacter> players = new List<BattleCharacter>();

        for (int i = 0; i < activeBattlers.Count; i++) {
            if (activeBattlers[i].isPlayer) {
                players.Add(activeBattlers[i]);
            }
        }

        if (isItem)
        {
            if (players[charToUseOn].dead == false) {
                UseItemInBattle(selectedChar, players[charToUseOn]);
            } else if (players[charToUseOn].dead && canRevive) {
                UseItemInBattle(selectedChar, players[charToUseOn]);
                players[charToUseOn].dead = false;
            } else  if (players[charToUseOn].dead) {
                BattleManager.instance.notice.myText.text = "You Wish!";
                BattleManager.instance.notice.Activate();
            }
        }

        if (isWeapon)
        {
            EquipWeaponInBattle(selectedChar, players[charToUseOn]);
        }

        if (isArmour)
        {
            EquipArmourInBattle(selectedChar, players[charToUseOn]);
        }

        GameManager.instance.RemoveItem(itemName);
    }

    private void UseItemInBattle(CharStats selectedChar, BattleCharacter battleChar)
    {
        if (affectHP)
        {
            selectedChar.currentHealth += amountToChange;
            battleChar.currentHP += amountToChange;
            if (battleChar.currentHP > battleChar.maxHP)
            {
                battleChar.currentHP = battleChar.maxHP;
            }
        }

        if (affectTP)
        {
            selectedChar.currentTalent += amountToChange;
            battleChar.currentTP += amountToChange;
            if (battleChar.currentTP > battleChar.maxTP)
            {
                battleChar.currentTP = battleChar.maxTP;
            }
        }

        if (affectAttack)
        {
            selectedChar.attack += amountToChange;
            battleChar.attack += amountToChange;
        }

        if (affectDefense)
        {
            selectedChar.defense += amountToChange;
            battleChar.defense += amountToChange;
        }
    }

    private void EquipWeaponInBattle(CharStats selectedChar, BattleCharacter battleChar)
    {
        if (selectedChar.equippedWeapon != "")
        {
            GameManager.instance.AddItem(selectedChar.equippedWeapon);
            selectedChar.equippedWeapon = itemName;
            selectedChar.weaponPower = weaponStrength;
            battleChar.weaponPow = weaponStrength;
        } else { 
            selectedChar.equippedWeapon = "";
            selectedChar.weaponPower = 0;
            battleChar.weaponPow = 0;
        }
    }

    private void EquipArmourInBattle(CharStats selectedChar, BattleCharacter battleChar)
    {
        if (selectedChar.equippedArmour != "")
        {
            GameManager.instance.AddItem(selectedChar.equippedArmour);
            selectedChar.equippedArmour = itemName;
            selectedChar.armourPower = armourStrength;
            battleChar.armorStrength = armourStrength;
        } else {
            selectedChar.equippedArmour = "";
            selectedChar.armourPower = armourStrength;
            battleChar.armorStrength = armourStrength;
        }
    }
}
