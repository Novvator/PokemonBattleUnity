using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public class BattleSystem : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public GameObject enemyBattleStationPrefab;

    Vector3 offscreenVector = new Vector3(12,1,0);
    Vector3 bsVector = new Vector3(4.5f,1,0);
    Unit playerUnit;
    Unit enemyUnit;

    public Text dialogueText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;
    void Start() {

        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle() {

        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        
        GameObject enemyBS = Instantiate(enemyBattleStationPrefab,offscreenVector,new Quaternion(0,0,0,0));
        GameObject enemyGO = Instantiate(enemyPrefab, enemyBS.transform);
        enemyUnit = enemyGO.GetComponent<Unit>();

        dialogueText.text = "A wild " + enemyUnit.unitName + " approaches!";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);
        
        while(Vector3.Distance(enemyBS.transform.position, bsVector)!=0) {
            
            enemyBS.transform.position = Vector3.MoveTowards(enemyBS.transform.position,bsVector, 3.5f*Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }


    IEnumerator PlayerAttack() {

        state = BattleState.ENEMYTURN;
        
        dialogueText.text = "You used Attack!";
        yield return new WaitForSeconds(1.5f);

        dialogueText.text = "The attack is successful!";
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        enemyHUD.SetHP(enemyUnit.currentHP);
        yield return new WaitForSeconds(1.5f);

        if(isDead) {
            state = BattleState.WON;
            EndBattle();
        } else {
            
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn() {
        dialogueText.text = enemyUnit.unitName + " attacks!";
        yield return new WaitForSeconds(1.5f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        playerHUD.SetHP(playerUnit.currentHP);
        yield return new WaitForSeconds(1.5f);

        if(isDead){
            state = BattleState.LOST;
            EndBattle();
        } else {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }
    void EndBattle() {
        if(state == BattleState.WON) {
            dialogueText.text = "You won the battle!";

        } else if(state == BattleState.LOST) {
            dialogueText.text = "You were defeated.";
        }
    }
    void PlayerTurn() {

        dialogueText.text = "Choose an action: ";
    }

    public void OnAttackButton() {
        if( state != BattleState.PLAYERTURN) {
            return;
        }

        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton() {
        if( state != BattleState.PLAYERTURN) {
            return;
        }

        StartCoroutine(PlayerHeal());
    }

    IEnumerator PlayerHeal() {
        state = BattleState.ENEMYTURN;
        dialogueText.text = "You used Heal!";
        yield return new WaitForSeconds(1.5f);

        playerUnit.Heal(1);
        playerHUD.SetHP(playerUnit.currentHP);
        dialogueText.text = "You healed!";
        yield return new WaitForSeconds(1.5f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }


}
