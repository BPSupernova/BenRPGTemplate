using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    private bool battleActive;

    public GameObject battleScene;
    public GameObject commandButtons;
    //public GameObject enemyAttackIndicator; Particle System I don't use to keep track of enemy turns anymore
    public GameObject enemyAttackDetailsPanel;
    public GameObject targetSelectionMenu;
    public GameObject talentSelectionMenu;
    public GameObject itemSelectMenu;
    public GameObject itemNamePanel;
    public GameObject itemDescriptionPanel;
    public GameObject itemActionMenu;
    public GameObject charSelectForBattleItemMenu;

    public Item activeItem;
    public Text activeEnemyAttackText;
    public Text itemNameDurBattle, itemDescriptionDurBattle, useButtonText;

    public Text[] playerName, playerHP, playerTP;
    public Transform[] playerActiveTagPositions; 
    public Transform[] enemyPositions;
    public BattleCharacter[] playerPrefabs;
    public BattleCharacter[] enemyPrefabs;
    public BattleTargetButton[] targetButtons;
    public BattleTalentSelect[] talentButtons;
    public ItemButton[] itemButtons;
    public Text[] itemCharChoiceNames;

    public List<BattleCharacter> activeBattlers = new List<BattleCharacter>();

    private Color deactivePlayerIndicator;
    public int currentTurn;
    public bool waitingTurn;
    public int chanceToSkedaddle = 40;
    public bool fleeing;
    public bool cannotFlee;
    private int expSum = 0;

    public BattleActions[] attacksAndTalents;

    public DamageNumber damageNumber;
    public BattleNotification notice;
    public string gameOverScene;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Y)) {
        //     BattleStart((new string[] {"Slime", "Bad Mage"}), false);
        // }

        if (battleActive) {
            if (waitingTurn) {
                if (activeBattlers[currentTurn].isPlayer) {
                    commandButtons.SetActive(true);
                } else {
                    commandButtons.SetActive(false);
                    // Enemy should attack
                    StartCoroutine(EnemyMoveCo());
                }
            }
            // if (Input.GetKeyDown(KeyCode.N)) {
            //     NextTurn();
            // }
        }

        // if (Input.GetKeyDown(KeyCode.C)) {
        //     bool check = false;
        //     if (charSelectForBattleItemMenu.activeInHierarchy || targetSelectionMenu.activeInHierarchy) { 
        //         CloseOutOfMenu(targetSelectionMenu);
        //         CloseOutOfMenu(charSelectForBattleItemMenu);
        //         check = true; 
        //     } else if (talentSelectionMenu.activeInHierarchy && check == false) {
        //         CloseOutOfMenu(talentSelectionMenu);
        //         check = true;
        //     } else if (itemSelectMenu.activeInHierarchy && check == false) {
        //         CloseOutOfMenu(itemSelectMenu);
        //         CloseOutOfMenu(itemActionMenu);
        //         CloseOutOfMenu(itemNamePanel);
        //         CloseOutOfMenu(itemDescriptionPanel);
        //     } 
        // }
    }

    public void BattleStart(string[] enemysToSpawn, bool ableToFlee) {
        if (!battleActive) {
            ableToFlee = cannotFlee;
            GameManager.instance.battleActive = true;
            battleActive = true;
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);

            battleScene.SetActive(true);

            AudioManager.instance.PlayBGM(0);

            for (int i = 0; i < playerActiveTagPositions.Length; i++) {
                if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy) {
                    for (int j = 0; j < playerPrefabs.Length; j++) {
                        if (playerPrefabs[j].characterName == GameManager.instance.playerStats[i].charName) {
                            BattleCharacter newPlayer = Instantiate(playerPrefabs[j], playerActiveTagPositions[i].position, playerActiveTagPositions[i].rotation);
                            newPlayer.transform.parent = playerActiveTagPositions[i];
                            activeBattlers.Add(newPlayer);
                            newPlayer.gameObject.SetActive(true);

                            CharStats player = GameManager.instance.playerStats[i];
                            activeBattlers[i].currentHP = player.currentHealth;
                            activeBattlers[i].currentTP = player.currentTalent; 
                            activeBattlers[i].maxHP = player.maxHP; 
                            activeBattlers[i].maxTP = player.maxTalentPoints; 
                            activeBattlers[i].attack = player.attack; 
                            activeBattlers[i].defense = player.defense; 
                            activeBattlers[i].weaponPow = player.weaponPower; 
                            activeBattlers[i].armorStrength = player.armourPower;

                        }
                    }
                }
            }

            for (int i = 0; i < enemysToSpawn.Length; i++) {
                if (enemysToSpawn[i] != "") {
                    for (int j = 0; j < enemyPrefabs.Length; j++) {
                        if (enemyPrefabs[j].characterName == enemysToSpawn[i]) {
                            BattleCharacter newEnemy = Instantiate(enemyPrefabs[j], enemyPositions[i].position, enemyPositions[i].rotation);
                            newEnemy.transform.parent = enemyPositions[i];
                            newEnemy.gameObject.SetActive(true);
                            activeBattlers.Add(newEnemy);
                        }
                    }
                }
            }
            waitingTurn = true;
            currentTurn = Random.Range(0, activeBattlers.Count);

            UpdatePlayerStats();
        }
    }

    public void NextTurn() {
        currentTurn++;
        if (currentTurn >= activeBattlers.Count) { currentTurn = 0; }

        waitingTurn = true;
        UpdateBattle();
        UpdatePlayerStats();
    }

    public void UpdateBattle() {
        bool allEnemiesDead = true;
        bool allPlayersDead = true;

        for (int i = 0; i < activeBattlers.Count; i++) {
            if (activeBattlers[i].currentHP < 0) {
                activeBattlers[i].currentHP = 0;
            }

            if (activeBattlers[i].currentHP == 0) {
                if (activeBattlers[i].isPlayer) {
                    activeBattlers[i].dead = true;
                } else {
                    activeBattlers[i].EnemyFade();
                }
            } else {
                if (activeBattlers[i].isPlayer) {
                    allPlayersDead = false;
                } else {
                    allEnemiesDead = false;
                }
            }
        }

        if (allEnemiesDead || allPlayersDead) {
            if (allEnemiesDead) {
                // End battle in victory
                for (int i = 0; i < activeBattlers.Count; i++) {
                    if (!activeBattlers[i].isPlayer) {
                        expSum += activeBattlers[i].enemyEXP;
                        int chanceToGetItem = Random.Range(0, 100);
                        if (chanceToGetItem < 33) { 
                            BattleReward.instance.itemDrops.Add(activeBattlers[i].enemyDrop);
                            GameManager.instance.AddItem(activeBattlers[i].enemyDrop); 
                        }
                    }
                }
                StartCoroutine(EndBattleCo());
            } else {
                // End battle in failure
                StartCoroutine(GameOverCo());
            }
        } else {
            while (activeBattlers[currentTurn].currentHP == 0) {
                currentTurn++;
                if (currentTurn >= activeBattlers.Count) {
                    currentTurn = 0;
                }
            }
        }
    }

    public IEnumerator EnemyMoveCo() {
       waitingTurn = false;
       yield return new WaitForSeconds(1f);
       EnemyAttack();
       yield return new WaitForSeconds(1f);
       NextTurn(); 
    }

    public void EnemyAttack() {
        List<int> players = new List<int>();
        for (int i = 0; i < activeBattlers.Count; i++) {
            if (activeBattlers[i].isPlayer &&  activeBattlers[i].currentHP > 0) {
                players.Add(i);
            }
        }

        int selectedPlayerTarget = players[Random.Range(0, players.Count)];

        int selectAttack = Random.Range(0, activeBattlers[currentTurn].availableTalents.Length);
        int movePower = 0;

        for (int i = 0; i < attacksAndTalents.Length; i++) {
            if (attacksAndTalents[i].actionName == activeBattlers[currentTurn].availableTalents[selectAttack]) {
                Instantiate(attacksAndTalents[i].effect, activeBattlers[2].transform.position, activeBattlers[2].transform.rotation);
                movePower = attacksAndTalents[i].actionPower;
            }
        }

        activeEnemyAttackText.text = "" + activeBattlers[currentTurn].characterName + " used " + activeBattlers[currentTurn].availableTalents[selectAttack] + "!";
        enemyAttackDetailsPanel.SetActive(true);

        DealDamage(selectedPlayerTarget, movePower);
    }

    public void DealDamage(int target, int actionPower) {
        float attackPow = activeBattlers[currentTurn].attack + activeBattlers[currentTurn].weaponPow;
        float defensivePow = activeBattlers[currentTurn].defense + activeBattlers[currentTurn].armorStrength;
        if (defensivePow == 0) { defensivePow = 1; }

        float damage = (attackPow / defensivePow) * actionPower * Random.Range(0.9f, 1.1f);
        int damageToDeal = Mathf.RoundToInt(damage);

        Debug.Log(activeBattlers[currentTurn].characterName + " is dealing " + damage + "(" + damageToDeal + ")" + activeBattlers[target].characterName);
    
        activeBattlers[target].currentHP -= damageToDeal;

        Instantiate(damageNumber, activeBattlers[target].transform.position, activeBattlers[target].transform.rotation).SetDamage(damageToDeal);
    
        UpdatePlayerStats();
    }

    public void UpdatePlayerStats() {
        for (int i = 0; i < playerName.Length; i++) {
            if (activeBattlers.Count > i) 
            {
                if (activeBattlers[i].isPlayer) { 
                    BattleCharacter playerData = activeBattlers[i];

                    playerName[i].gameObject.SetActive(true);
                    playerName[i].text = playerData.characterName;
                    if (activeBattlers[currentTurn].characterName == playerData.characterName) { playerName[i].color = Color.white; }
                    else if (activeBattlers[i].currentHP == 0) {
                        playerName[i].color = Color.black;
                        Color textColor = playerName[i].color;
                        textColor.a = 0.58f; // Set the alpha value to 0.5
                        playerName[i].color = textColor;
                    }
                    else { 
                        ColorUtility.TryParseHtmlString("#FA9A9A", out deactivePlayerIndicator);
                        playerName[i].color = deactivePlayerIndicator;
                    }
                    playerHP[i].text = "" + Mathf.Clamp(playerData.currentHP, 0, int.MaxValue) + "/" + playerData.maxHP;
                    playerTP[i].text = "" + Mathf.Clamp(playerData.currentTP, 0, int.MaxValue) + "/" + playerData.maxTP; 
                } else {
                    playerName[i].gameObject.SetActive(false);
                }
            } else { playerName[i].gameObject.SetActive(false); }
        }
    }

    public void PlayerAttack(string actionName, int selectedTarget) {
        int movePower = 0;

        for (int i = 0; i < attacksAndTalents.Length; i++) {
            if (attacksAndTalents[i].actionName == actionName) {
                Instantiate(attacksAndTalents[i].effect, activeBattlers[selectedTarget].transform.position, activeBattlers[selectedTarget].transform.rotation);
                movePower = attacksAndTalents[i].actionPower;
            }
        }
        
        if (actionName == "Slash") { activeEnemyAttackText.text = "" + activeBattlers[currentTurn].characterName + " attacked!"; }
        else { activeEnemyAttackText.text = "" + activeBattlers[currentTurn].characterName + " used " + actionName + "!"; }
        enemyAttackDetailsPanel.SetActive(true);

        DealDamage(selectedTarget, movePower);

        commandButtons.SetActive(false);
        targetSelectionMenu.SetActive(false);
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        NextTurn();
    }

    public void OpenTargetSelectionMenu(string actionName) {
        targetSelectionMenu.SetActive(true);
        List<int> Enemies = new List<int>();
        for (int i = 0; i < activeBattlers.Count; i++) {
            if (!activeBattlers[i].isPlayer) {
                Enemies.Add(i);
            }
        }

        for (int j = 0; j < targetButtons.Length; j++) {
            if (Enemies.Count > j && (activeBattlers[Enemies[j]].currentHP > 0)) {
                targetButtons[j].gameObject.SetActive(true);
                targetButtons[j].actionName = actionName;
                targetButtons[j].activeBattleTarget = Enemies[j];
                targetButtons[j].targetName.text = activeBattlers[Enemies[j]].characterName;
            } else {
                targetButtons[j].gameObject.SetActive(false);
            }
        }
    }

    public void OpenTalentSelectionMenu() {
        talentSelectionMenu.SetActive(true);

        for (int i = 0; i < talentButtons.Length; i++) {
            if (activeBattlers[currentTurn].availableTalents.Length > i) {
                talentButtons[i].gameObject.SetActive(true);
                talentButtons[i].talentName = activeBattlers[currentTurn].availableTalents[i];
                talentButtons[i].nameText.text = talentButtons[i].talentName;

                for (int j = 0; j < attacksAndTalents.Length; j++) {
                    if (attacksAndTalents[j].actionName == talentButtons[i].talentName) {
                        talentButtons[i].talentCost = attacksAndTalents[j].actionCost;
                        talentButtons[i].costText.text = talentButtons[i].talentCost.ToString();
                    }
                }
            } else {
                talentButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenItemSelectionMenu() {
        itemSelectMenu.SetActive(true);
        itemActionMenu.SetActive(true);
        itemNamePanel.SetActive(true);
        itemDescriptionPanel.SetActive(true);
        itemDescriptionDurBattle.text = "";
        ShowItems();
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

        itemNameDurBattle.text = activeItem.itemName;
        itemDescriptionDurBattle.text = activeItem.description;        
    }

    public void OpenItemCharSelect() {
        charSelectForBattleItemMenu.SetActive(true);
        int j = 0;
        for (int i = 0; i < activeBattlers.Count; i++) {
            if (activeBattlers[i].isPlayer) {
                itemCharChoiceNames[j].text = GameManager.instance.playerStats[j].charName;
                itemCharChoiceNames[j].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[j].gameObject.activeInHierarchy);
                j++;
            }
        }


        for (int i = 0; i < itemCharChoiceNames.Length - 1; i++) {
            itemCharChoiceNames[i].text = GameManager.instance.playerStats[i].charName;
            itemCharChoiceNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[i].gameObject.activeInHierarchy);
        }
    }

    public void UseItem(int selectedChar) {
        activeItem.UseInBattle(selectedChar);
        charSelectForBattleItemMenu.SetActive(false);
        itemSelectMenu.SetActive(false);
        itemActionMenu.SetActive(false);
        itemNamePanel.SetActive(false);
        itemDescriptionPanel.SetActive(false);
        NextTurn();
    }

    public void CloseOutOfMenu(GameObject menu) {
        menu.SetActive(false);
    }

    public void Flee() {
        if (cannotFlee) {
            notice.myText.text = "You Can't Run Now!";
            notice.Activate();
        }
        else {
            int successfulSkedaddle = Random.Range(0,100);
            if (successfulSkedaddle < chanceToSkedaddle) {
                // battleActive = false;
                // battleScene.SetActive(false);
                fleeing = true;
                StartCoroutine(EndBattleCo());
            } else {
                NextTurn();
                notice.myText.text = "Stay Where You Are!";
                notice.Activate(); 
            }
        }
    }

    public IEnumerator EndBattleCo() {
        battleActive = false;
        commandButtons.SetActive(false);
        enemyAttackDetailsPanel.SetActive(false);
        targetSelectionMenu.SetActive(false);
        talentSelectionMenu.SetActive(false);
        itemActionMenu.SetActive(false);
        itemDescriptionPanel.SetActive(false);
        itemNamePanel.SetActive(false);
        itemSelectMenu.SetActive(false);
        charSelectForBattleItemMenu.SetActive(false);
        
        yield return new WaitForSeconds(0.5f);
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < activeBattlers.Count; i++) {
            if (activeBattlers[i].isPlayer) {
                for (int j = 0; j < GameManager.instance.playerStats.Length; j++) {
                    if (activeBattlers[i].characterName == GameManager.instance.playerStats[j].charName) {
                        GameManager.instance.playerStats[j].currentHealth = activeBattlers[i].currentHP;
                        GameManager.instance.playerStats[j].currentTalent = activeBattlers[i].currentTP;

                    }
                } 
            }

            Destroy(activeBattlers[i].gameObject);
        }

        UIFade.instance.FadeFromBlack();
        battleScene.SetActive(false);
        activeBattlers.Clear();
        currentTurn = 0;

        if (fleeing) {
            GameManager.instance.battleActive = false;
            fleeing = false;
        } else {
            BattleReward.instance.OpenBattleRewardsScreen(expSum, BattleReward.instance.itemDrops);
        }
        expSum = 0;
        AudioManager.instance.PlayBGM(FindObjectOfType<CameraController>().musicToPlay);
    }

    public IEnumerator GameOverCo() {
        expSum = 0;
        battleActive = false;
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1.5f);

        battleScene.SetActive(false);
        SceneManager.LoadScene(gameOverScene);
    }
}
