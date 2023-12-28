using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTargetButton : MonoBehaviour
{
    public string actionName;
    public int activeBattleTarget;
    public Text targetName;
    // Start is called before the first frame update
    
    public void Press() {
        BattleManager.instance.PlayerAttack(actionName, activeBattleTarget);
    }
}
