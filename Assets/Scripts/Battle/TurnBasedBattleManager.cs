using System.Collections;
using UnityEngine;

public class TurnBasedBattleManager : MonoBehaviour
{
    [Header("Character")]
    public BattleCharacter player;
    public BattleCharacter enemy;

    [Header("Battle Setting")]
    public float moveSpeed = 5f;
    public float attackDistance = 1.2f;
    public float delayBeforeAttack = 0.3f;
    public float delayAfterAttack = 0.5f;
    public float delayBetweenTurns = 1f;

    private Vector3 playerStartPosition;
    private Vector3 enemyStartPosition;

    private bool battleEnded = false;

    void Start()
    {
        playerStartPosition = player.transform.position;
        enemyStartPosition = enemy.transform.position;

        StartCoroutine(BattleLoop());
    }

    IEnumerator BattleLoop()
    {
        yield return new WaitForSeconds(1f);

        while (!battleEnded)
        {
            yield return StartCoroutine(DoTurn(player, enemy, playerStartPosition));

            if (CheckBattleEnd())
                yield break;

            yield return new WaitForSeconds(delayBetweenTurns);

            yield return StartCoroutine(DoTurn(enemy, player, enemyStartPosition));

            if (CheckBattleEnd())
                yield break;

            yield return new WaitForSeconds(delayBetweenTurns);
        }
    }

    IEnumerator DoTurn(BattleCharacter attacker, BattleCharacter target, Vector3 attackerStartPosition)
    {
        Debug.Log("Giliran " + attacker.characterName);

        Vector3 attackPosition = GetAttackPosition(attacker.transform, target.transform);

        yield return StartCoroutine(MoveToPosition(attacker.transform, attackPosition));

        yield return new WaitForSeconds(delayBeforeAttack);

        target.TakeDamage(attacker.attackDamage);

        yield return new WaitForSeconds(delayAfterAttack);

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
            battleEnded = true;
            Debug.Log("Enemy menang!");
            return true;
        }

        if (enemy.IsDead)
        {
            battleEnded = true;
            Debug.Log("Player menang!");
            return true;
        }

        return false;
    }
}