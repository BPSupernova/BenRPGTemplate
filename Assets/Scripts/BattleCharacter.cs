using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacter : MonoBehaviour
{
    public string characterName;
    public int currentHP, currentTP, maxHP, maxTP, attack, defense, weaponPow, armorStrength;
    public SpriteRenderer mySprite;

    public string[] availableTalents;

    public bool isPlayer;
    public bool dead;

    public int enemyEXP;
    public string enemyDrop;
    private bool shouldFade;
    public float fadeSpeed = 1f;
    
    void Update()
    {
        if (shouldFade) {
            mySprite.color = new Color((Mathf.MoveTowards(mySprite.color.r, 0.5f, fadeSpeed * Time.deltaTime)), (Mathf.MoveTowards(mySprite.color.g, 0.5f, fadeSpeed * Time.deltaTime)), (Mathf.MoveTowards(mySprite.color.b, 0.5f, fadeSpeed * Time.deltaTime)), (Mathf.MoveTowards(mySprite.color.a, 0f, fadeSpeed * Time.deltaTime)));
            if (mySprite.color.a == 0) { gameObject.SetActive(false); }
        }
    }

    public void EnemyFade() {
        shouldFade = true;
    }
}
