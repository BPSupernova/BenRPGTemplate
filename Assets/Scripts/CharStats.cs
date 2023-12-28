using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStats : MonoBehaviour
{
    public string charName;
    public int charLevel = 0;
    public int currentEXP;
    public int[] expToNextLevel;
    public int maxLevel = 100;
    public int expToLevelOne = 250;

    public int currentHealth;
    public int maxHP = 100;

    public int currentTalent;
    public int maxTalentPoints = 25;
    public int[] talentLevelUpBonus;

    public int attack;
    public int defense;

    public int weaponPower;
    public int armourPower;
    public string equippedWeapon;
    public string equippedArmour;

    public Sprite charImage;

    void Start()
    {
        SetEXPCriteraForAllLevels();
    }

    void SetEXPCriteraForAllLevels()
    {
        expToNextLevel = new int[maxLevel];
        expToNextLevel[0] = expToLevelOne;
        for (int i = 1; i < maxLevel; i++)
        {
            expToNextLevel[i] = Mathf.FloorToInt(expToNextLevel[i -1] * 1.05f);
        }
    }

    void Update()
    {
        // For testing leveling system
        // if (Input.GetKeyDown(KeyCode.L)) {
        //     AddExp(50);
        // }
    }

    public void AddExp(int expToAdd) {
        currentEXP += expToAdd;
        if (charLevel < maxLevel) {
            if (currentEXP > expToNextLevel[charLevel])
            {
                currentEXP -= expToNextLevel[charLevel];
                charLevel++;
            
                GainAttackOrDefense();
                GainMaxHP();
                GainTalentPoints();
            }
        }
            // Stops the player from getting experience at the max level
            if (charLevel >= maxLevel - 1) {
                currentEXP = 0;
            }
    }

    void GainTalentPoints()
    {
        maxTalentPoints += talentLevelUpBonus[charLevel];
        currentTalent = maxTalentPoints;
    }

    void GainMaxHP()
    {
        maxHP = Mathf.FloorToInt(maxHP * 1.05f);
        currentHealth = maxHP;
    }

    private void GainAttackOrDefense()
    {
        if (charLevel % 2 == 0)
        {
            attack += Mathf.RoundToInt(Random.Range(1, 3));
        }
        else
        {
            defense += Mathf.RoundToInt(Random.Range(1, 3));
        }
    }
}
