using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleReward : MonoBehaviour
{
    public static BattleReward instance;

    public Text xpText, itemText, itemTwoText, itemThreeText, congratsText;
    public GameObject rewardScreen;

    public List<string> itemDrops = new List<string>();
    public int xpEarned;

    public bool markQuestComplete;
    public string questToMark;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
    //    if (Input.GetKeyDown(KeyCode.L)) {
    //         List<string> testRewards = new List<string>();
    //         testRewards.Add("Apple Juice");
    //         testRewards.Add("Apple Juice");
    //         OpenBattleRewardsScreen(54, testRewards);
    //    } 
    }

    public void OpenBattleRewardsScreen(int exp, List<string> rewards) {
        xpEarned = exp;
        itemDrops = rewards;

        xpText.text = "Everyone earned " + xpEarned + " experience!";
        
        if (itemDrops.Count == 3) {
            itemText.text = itemDrops[0];
            itemTwoText.text = itemDrops[1];
            itemThreeText.text = itemDrops[2];
        } else if (itemDrops.Count == 2) {
            itemText.text = itemDrops[0];
            itemTwoText.text = itemDrops[1];
            itemThreeText.text = "";
        } else if (itemDrops.Count == 1) {
            itemText.text = itemDrops[0];
            itemTwoText.text = "";
            itemThreeText.text = "";
        } else {
            itemText.text = "None";
            itemTwoText.text = "";
            itemThreeText.text = "";
        }

        rewardScreen.SetActive(true);
    }

    public void CloseBattleRewardsScreen() {
        for (int i = 0; i < GameManager.instance.playerStats.Length; i++) {
            if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy) {
                GameManager.instance.playerStats[i].AddExp(xpEarned);
            }
        }
        
        rewardScreen.SetActive(false);
        GameManager.instance.battleActive = false;
        if (markQuestComplete) {
            QuestManager.instance.MarkQuestComplete(questToMark);
        }
    }
}
