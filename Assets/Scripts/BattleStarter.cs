using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStarter : MonoBehaviour
{
    public BattleType[] potentialBattleEncounters;
    public float timeBetweenBattles = 10f;
    private float betweenBattleCounter;
    
    private bool inArea;

    public bool activateOnEnter, activateOnStay, activateOnExit;
    
    public bool deActiveAfterStarting;
    
    public bool cannotFlee;

    public bool shouldCompleteQuest;
    public string questToComplete;

    private void Start() {
        betweenBattleCounter = Random.Range(timeBetweenBattles * 0.7f, timeBetweenBattles * 1.3f);
    }

    private void Update() {
        if (inArea && PlayerController.instance.canMove) {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
                betweenBattleCounter -= Time.deltaTime;
            }

            if (betweenBattleCounter <= 0) {
                betweenBattleCounter = Random.Range(timeBetweenBattles * 0.7f, timeBetweenBattles * 1.3f);
                StartCoroutine(StartBattleCo());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            if (activateOnEnter) {
                StartCoroutine(StartBattleCo());
            } else {
                inArea = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            if (activateOnExit) {

            } else {
                inArea = false;
            }
        }    
    }

    public IEnumerator StartBattleCo() {
        UIFade.instance.FadeToBlack();
        GameManager.instance.battleActive = true;
        int selectedBattle = Random.Range(0, potentialBattleEncounters.Length);

        yield return new WaitForSeconds(1.5f);
        BattleManager.instance.cannotFlee = this.cannotFlee;
        BattleManager.instance.BattleStart(potentialBattleEncounters[selectedBattle].enemies, cannotFlee);
        UIFade.instance.FadeFromBlack();

        if (deActiveAfterStarting) {
            gameObject.SetActive(false);
        }

        BattleReward.instance.markQuestComplete = shouldCompleteQuest;
        BattleReward.instance.questToMark = questToComplete;
    }
}
