using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Setting")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Health Bar")]
    public Transform healthBarFill;

    private Vector3 originalFillScale;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBarFill != null)
        {
            originalFillScale = healthBarFill.localScale;
            UpdateHealthBar();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBarFill == null)
            return;

        float healthPercent = (float)currentHealth / maxHealth;

        healthBarFill.localScale = new Vector3(
            originalFillScale.x * healthPercent,
            originalFillScale.y,
            originalFillScale.z
        );
    }

    void Die()
    {
        Debug.Log("Player mati");

        // Nanti bisa diganti dengan animasi mati / restart scene
        gameObject.SetActive(false);
    }
}