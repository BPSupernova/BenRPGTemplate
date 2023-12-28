using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTalentSelect : MonoBehaviour
{
    public string talentName;
    public int talentCost;
    public Text nameText;
    public Text costText;
    // Start is called before the first frame update

    public void Press() {
        if (BattleManager.instance.activeBattlers[BattleManager.instance.currentTurn].currentTP >= talentCost) {
            BattleManager.instance.talentSelectionMenu.SetActive(false);
            BattleManager.instance.OpenTargetSelectionMenu(talentName);
            BattleManager.instance.activeBattlers[BattleManager.instance.currentTurn].currentTP -= talentCost;
        } else {
            BattleManager.instance.notice.myText.text = "Not Enough TP!";
            BattleManager.instance.notice.Activate();
            BattleManager.instance.talentSelectionMenu.SetActive(false);
        }
    }
}
