using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TurnBasedSkillBattleManager : MonoBehaviour
{
    public enum BattleState
    {
        PlayerTurn,
        EnemyTurn,
        Busy,
        Win,
        Lose
    }

    [Header("Character")]
    public BattleCharacter player;
    public BattleCharacter enemy;

    [Header("UI Skill")]
    public GameObject skillPanel;
    public Button attackButton;
    public Button defendButton;
    public Button healButton;

    [Header("Damage Setting")]
    public int playerAttackMin = 20;
    public int playerAttackMax = 30;

    public int enemyAttackMin = 20;
    public int enemyAttackMax = 30;

    [Header("Defend Setting")]
    public int defendReduceMin = 10;
    public int defendReduceMax = 15;

    [Header("Heal Setting")]
    public int healMin = 15;
    public int healMax = 25;

    [Header("Movement Setting")]
    public float moveSpeed = 5f;
    public float attackDistance = 1.2f;
    public float delayBeforeAttack = 0.3f;
    public float delayAfterAttack = 0.5f;
    public float delayBetweenTurns = 0.8f;

    private Vector3 playerStartPosition;
    private Vector3 enemyStartPosition;

    private BattleState state;
    private bool playerIsDefending = false;

    void Start()
    {
        playerStartPosition = player.transform.position;
        enemyStartPosition = enemy.transform.position;

        attackButton.onClick.AddListener(PlayerAttack);
        defendButton.onClick.AddListener(PlayerDefend);
        healButton.onClick.AddListener(PlayerHeal);

        HideSkillPanel();

        StartPlayerTurn();
    }

    void StartPlayerTurn()
    {
        state = BattleState.PlayerTurn;
        ShowSkillPanel();

        Debug.Log("Giliran Player. Pilih Attack, Defend, atau Heal.");
    }

    void StartEnemyTurn()
    {
        state = BattleState.EnemyTurn;
        HideSkillPanel();

        StartCoroutine(EnemyTurn());
    }

    public void PlayerAttack()
    {
        if (state != BattleState.PlayerTurn)
            return;

        HideSkillPanel();
        StartCoroutine(PlayerAttackRoutine());
    }

    public void PlayerDefend()
    {
        if (state != BattleState.PlayerTurn)
            return;

        HideSkillPanel();
        StartCoroutine(PlayerDefendRoutine());
    }

    public void PlayerHeal()
    {
        if (state != BattleState.PlayerTurn)
            return;

        HideSkillPanel();
        StartCoroutine(PlayerHealRoutine());
    }

    IEnumerator PlayerAttackRoutine()
    {
        state = BattleState.Busy;

        int damage = Random.Range(playerAttackMin, playerAttackMax + 1);

        Debug.Log("Player menggunakan Attack. Damage: " + damage);

        yield return StartCoroutine(DoAttackMovement(player, enemy, playerStartPosition, damage));

        if (CheckBattleEnd())
            yield break;

        yield return new WaitForSeconds(delayBetweenTurns);

        StartEnemyTurn();
    }

    IEnumerator PlayerDefendRoutine()
    {
        state = BattleState.Busy;

        playerIsDefending = true;

        Debug.Log("Player menggunakan Defend. Giliran langsung pindah ke Enemy.");

        yield return new WaitForSeconds(delayBetweenTurns);

        StartEnemyTurn();
    }

    IEnumerator PlayerHealRoutine()
    {
        state = BattleState.Busy;

        int healAmount = Random.Range(healMin, healMax + 1);

        Debug.Log("Player menggunakan Heal. Heal: " + healAmount);

        player.Heal(healAmount);

        yield return new WaitForSeconds(delayBetweenTurns);

        StartEnemyTurn();
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(delayBetweenTurns);

        int damage = Random.Range(enemyAttackMin, enemyAttackMax + 1);

        if (playerIsDefending)
        {
            int reduceDamage = Random.Range(defendReduceMin, defendReduceMax + 1);

            damage -= reduceDamage;

            if (damage < 0)
            {
                damage = 0;
            }

            Debug.Log("Player sedang Defend. Damage enemy dikurangi: " + reduceDamage);
        }

        Debug.Log("Enemy menyerang. Damage masuk: " + damage);

        yield return StartCoroutine(DoAttackMovement(enemy, player, enemyStartPosition, damage));

        playerIsDefending = false;

        if (CheckBattleEnd())
            yield break;

        yield return new WaitForSeconds(delayBetweenTurns);

        StartPlayerTurn();
    }

    IEnumerator DoAttackMovement(BattleCharacter attacker, BattleCharacter target, Vector3 attackerStartPosition, int damage)
    {
        Vector3 attackPosition = GetAttackPosition(attacker.transform, target.transform);

        // Maju mendekati target
        yield return StartCoroutine(MoveToPosition(attacker.transform, attackPosition));

        // Delay sedikit sebelum damage masuk
        yield return new WaitForSeconds(delayBeforeAttack);

        // HP target langsung berkurang saat attacker sudah dekat
        target.TakeDamage(damage);

        // Delay setelah menyerang
        yield return new WaitForSeconds(delayAfterAttack);

        // Baru balik ke posisi awal
        yield return StartCoroutine(MoveToPosition(attacker.transform, attackerStartPosition));
    }

    Vector3 GetAttackPosition(Transform attacker, Transform target)
    {
        Vector3 direction = (attacker.position - target.position).normalized;

        Vector3 attackPosition = target.position + direction * attackDistance;

        attackPosition.y = attacker.position.y;
        attackPosition.z = attacker.position.z;

        return attackPosition;
    }

    IEnumerator MoveToPosition(Transform character, Vector3 targetPosition)
    {
        while (Vector3.Distance(character.position, targetPosition) > 0.02f)
        {
            character.position = Vector3.MoveTowards(
                character.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        character.position = targetPosition;
    }

    bool CheckBattleEnd()
    {
        if (player.IsDead)
        {
            state = BattleState.Lose;
            HideSkillPanel();
            Debug.Log("Player kalah!");
            return true;
        }

        if (enemy.IsDead)
        {
            state = BattleState.Win;
            HideSkillPanel();
            Debug.Log("Player menang!");
            return true;
        }

        return false;
    }

    void ShowSkillPanel()
    {
        skillPanel.SetActive(true);

        // Paksa SkillPanel berada paling depan di Canvas
        skillPanel.transform.SetAsLastSibling();

        attackButton.interactable = true;
        defendButton.interactable = true;
        healButton.interactable = true;
    }

    void HideSkillPanel()
    {
        skillPanel.SetActive(false);
    }
}