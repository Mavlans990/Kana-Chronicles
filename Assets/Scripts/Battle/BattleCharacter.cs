using UnityEngine;

public class BattleCharacter : MonoBehaviour
{
    [Header("Status")]
    public string characterName;
    public int maxHealth = 100;
    public int currentHealth;
    public int attackDamage = 10;

    [Header("Health Bar")]
    public Transform healthBarFill;
    public SpriteRenderer healthBarFillRenderer;

    private Vector3 originalHealthBarScale;
    private Vector3 originalHealthBarPosition;

    public bool IsDead
    {
        get { return currentHealth <= 0; }
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBarFill != null)
        {
            originalHealthBarScale = healthBarFill.localScale;
            originalHealthBarPosition = healthBarFill.localPosition;
        }

        if (healthBarFillRenderer == null && healthBarFill != null)
        {
            healthBarFillRenderer = healthBarFill.GetComponent<SpriteRenderer>();
        }

        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthBar();

        Debug.Log(characterName + " terkena damage: " + damage);
        Debug.Log(characterName + " HP: " + currentHealth + "/" + maxHealth);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthBar();

        // Debug.Log(characterName + " heal: " + amount);
        // Debug.Log(characterName + " HP: " + currentHealth + "/" + maxHealth);
    }

    void UpdateHealthBar()
    {
        if (healthBarFill == null)
            return;

        float healthPercent = (float)currentHealth / maxHealth;

        // Mengubah panjang health bar
        float newScaleX = originalHealthBarScale.x * healthPercent;

        healthBarFill.localScale = new Vector3(
            newScaleX,
            originalHealthBarScale.y,
            originalHealthBarScale.z
        );

        // Membuat health bar berkurang dari kanan ke kiri
        float leftEdge = originalHealthBarPosition.x - (originalHealthBarScale.x / 2f);
        float newPosX = leftEdge + (newScaleX / 2f);

        healthBarFill.localPosition = new Vector3(
            newPosX,
            originalHealthBarPosition.y,
            originalHealthBarPosition.z
        );

        // Mengubah warna berdasarkan jumlah HP
        UpdateHealthBarColor(healthPercent);
    }

    void UpdateHealthBarColor(float healthPercent)
    {
        if (healthBarFillRenderer == null)
            return;

        if (healthPercent > 0.5f)
        {
            healthBarFillRenderer.color = Color.green;
        }
        else if (healthPercent > 0.25f)
        {
            healthBarFillRenderer.color = Color.yellow;
        }
        else
        {
            healthBarFillRenderer.color = Color.red;
        }
    }
    
}