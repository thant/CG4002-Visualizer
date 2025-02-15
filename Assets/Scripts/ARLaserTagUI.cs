using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ARLaserTagUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI playerhealthText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI playerAmmoText;
    public TextMeshProUGUI playerShieldText;
    public Image playerShieldCooldown;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI enemyAmmoText;
    public TextMeshProUGUI enemyShieldText;
    public Image enemyShieldCooldown;
    public Image[] playerBombIcons;  // Array to hold 2 bomb icons for the player
    public Image[] enemyBombIcons;   // Array to hold 2 bomb icons for the enemy

    public Button shootButton;
    public Button reloadButton;
    public Button shieldButton; 
    public Button enemyShieldButton;
    public Button badmintonButton;
    public Button boxingButton;
    public Button snowBombButton;
    public Button fencingButton;
    public Button golfButton;
    public SnowBombAttack snowBombAttack;
    public BadmintonAttack badmintonAttack;
    public BoxingAttack boxingAttack;
    public FencingAttack fencingAttack;
    public GolfAttack golfAttack;
    public Button hitButton;
    public Canvas gameCanvas;
    public GameObject shooterObject;
    public GameObject enemyObject;  // Reference to the enemy object
    public GameObject enemyIsVisible;  // Reference to the enemy's visibility status
    public GameObject enemyShieldActiveFlag;
    private int score = 0;
    private int ammo = 6;
    private int playerHealth = 100;
    private int playerShieldCount = 3;
    private int enemyHealth = 100;
    private int playerbomb = 2;
    private int enemybomb = 2;
    private int enemyShieldCount = 3;
    private int enemyAmmo = 6;
    private float timer = 300f; // 5-minute game timer
    public int shieldHealth = 30;  // Total HP of the shield
    private bool isPlayerShieldActive = false;  // Flag to check if the player's shield is active
    private bool isEnemyShieldActive = false;  // Flag to check if the enemy's shield is active
    private float shieldDepletionRate = 1f;  // Shield depletion rate (optional)
    private bool isReloading = false; // Track reload state
    private Button[] allButtons;

    // New Variables
    private int playerShieldHealth = 0;  // Player's shield health
    private int enemyShieldHealth = 0;   // Enemy's shield health

    void Start()
    {
        // Initialize button references
        allButtons = new Button[] { shootButton, reloadButton, shieldButton, badmintonButton, boxingButton, snowBombButton, fencingButton, golfButton, hitButton };

        // Ensure the UI is properly positioned for landscape mode
        gameCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        gameCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        gameCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        UpdateUI();

        shieldDepletionRate = 1f / shieldHealth;
        shootButton.onClick.AddListener(OnShootButtonPressed);
        reloadButton.onClick.AddListener(Reload);
        shieldButton.onClick.AddListener(ActivateShield);
        badmintonButton.onClick.AddListener(LaunchBadmintonAttack);
        boxingButton.onClick.AddListener(LaunchBoxingAttack);
        snowBombButton.onClick.AddListener(LaunchSnowBombAttack);
        fencingButton.onClick.AddListener(LaunchFencingAttack);
        golfButton.onClick.AddListener(LaunchGolfAttack);
        enemyShieldButton.onClick.AddListener(ActivateEnemyShield);
        hitButton.onClick.AddListener(TakeHit);
        InvokeRepeating("UpdateTimer", 1f, 1f);
    }

    void OnShootButtonPressed()
    {
        if (ammo > 0)
        {
            ammo--;
            if (shooterObject != null)
            {
                shooterObject.GetComponent<HoldGun>().Shoot();  // Ensure 'SomeShootComponent' is the script that has the Shoot method
            }
            else
            {
                Debug.LogWarning("❌ Shooter object not assigned!");
            }

            // If the enemy is visible and the enemy's shield is active
            if (enemyIsVisible.activeSelf)
            {
                if (isEnemyShieldActive)
                {
                    // Block damage with enemy's shield if it's active
                    int damageToShield = 5;

                    // If the enemy's shield can absorb all damage
                    if (enemyShieldHealth >= damageToShield)
                    {
                        enemyShieldHealth -= damageToShield;
                        if (enemyShieldHealth == 0){
                            isEnemyShieldActive = false;
                            enemyShieldActiveFlag.SetActive(false);  // Deactivate the enemy shield flag
                            }
                        Debug.Log("Enemy Shield Health: " + enemyShieldHealth);
                    }
                    else
                    {
                        // If the shield is depleted, apply the remainder damage to enemy health
                        int remainingDamage = damageToShield - enemyShieldHealth;
                        enemyHealth -= remainingDamage;
                        enemyShieldHealth = 0;
                        isEnemyShieldActive = false;  // Shield deactivated
                        Debug.Log("Enemy Health after shield depleted: " + enemyHealth);
                    }
                }
                else
                {
                    // No shield active, apply damage to enemy health directly
                    enemyHealth -= 5;
                    Debug.Log("Enemy Health: " + enemyHealth);
                }

                UpdateUI();
            }
        }
    }

    void TakeHit()
    {
        if (playerHealth > 0)
        {
            // If player's shield is active, absorb damage with the shield
            if (isPlayerShieldActive)
            {
                int damageToShield = 10;

                // If the player's shield can absorb all damage
                if (playerShieldHealth >= damageToShield)
                {
                    playerShieldHealth -= damageToShield;
                    if (playerShieldHealth == 0){
                        isPlayerShieldActive = false;
                    }
                    Debug.Log("Player Shield Health: " + playerShieldHealth);
                }
                else
                {
                    // If the shield is depleted, apply the remainder damage to player health
                    int remainingDamage = damageToShield - playerShieldHealth;
                    playerHealth -= remainingDamage;
                    playerShieldHealth = 0;
                    isPlayerShieldActive = false;  // Shield deactivated
                    Debug.Log("Player Health after shield depleted: " + playerHealth);
                }
            }
            else
            {
                // If no shield, damage the player's health directly
                playerHealth -= 10;
                Debug.Log("Player Health: " + playerHealth);
            }

            UpdateUI();
        }
    }

    void LaunchBadmintonAttack()
{
    badmintonAttack.LaunchBadmintonAttack();

    if (enemyIsVisible.activeSelf)
    {
        if (isEnemyShieldActive)
        {
            ShieldSparks();
            int damageToShield = 10;

            if (enemyShieldHealth >= damageToShield)
            {
                enemyShieldHealth -= damageToShield;
                if (enemyShieldHealth == 0){
                    isEnemyShieldActive = false;
                    enemyShieldActiveFlag.SetActive(false);  // Deactivate the enemy shield flag
                }
                Debug.Log("Enemy Shield Health: " + enemyShieldHealth);
            }
            else
            {
                int remainingDamage = damageToShield - enemyShieldHealth;
                enemyHealth -= remainingDamage;
                enemyShieldHealth = 0;
                isEnemyShieldActive = false;
                enemyShieldActiveFlag.SetActive(false);  // Deactivate the enemy shield flag
                Debug.Log("Enemy Health after shield depleted: " + enemyHealth);
            }
        }
        else
        {
            enemyHealth -= 10;
            Debug.Log("Enemy Health: " + enemyHealth);
        }

        UpdateUI();
    }
}

void LaunchBoxingAttack()
{
    boxingAttack.LaunchBoxingAttack();

    if (enemyIsVisible.activeSelf)
    {
        if (isEnemyShieldActive)
        {
            ShieldSparks();
            int damageToShield = 10;

            if (enemyShieldHealth >= damageToShield)
            {
                enemyShieldHealth -= damageToShield;
                if (enemyShieldHealth == 0){
                    isEnemyShieldActive = false;
                    enemyShieldActiveFlag.SetActive(false);  // Deactivate the enemy shield flag
                }
                Debug.Log("Enemy Shield Health: " + enemyShieldHealth);
            }
            else
            {
                int remainingDamage = damageToShield - enemyShieldHealth;
                enemyHealth -= remainingDamage;
                enemyShieldHealth = 0;
                isEnemyShieldActive = false;
                enemyShieldActiveFlag.SetActive(false);  // Deactivate the enemy shield flag
                Debug.Log("Enemy Health after shield depleted: " + enemyHealth);
            }
        }
        else
        {
            enemyHealth -= 10;
            Debug.Log("Enemy Health: " + enemyHealth);
        }

        UpdateUI();
    }
}

void LaunchSnowBombAttack()
{
    if (playerbomb > 0)
    {
        snowBombAttack.LaunchSnowbomb();
        playerbomb--;
    }
}

void LaunchFencingAttack()
{
    if (fencingAttack != null)
    {
        fencingAttack.LaunchFencingAttack();
    }

    if (enemyIsVisible.activeSelf)
    {
        if (isEnemyShieldActive)
        {
            ShieldSparks();
            int damageToShield = 10;

            if (enemyShieldHealth >= damageToShield)
            {
                enemyShieldHealth -= damageToShield;
                if (enemyShieldHealth == 0){
                    isEnemyShieldActive = false;
                    enemyShieldActiveFlag.SetActive(false);  // Deactivate the enemy shield flag
                }
                Debug.Log("Enemy Shield Health: " + enemyShieldHealth);
            }
            else
            {
                int remainingDamage = damageToShield - enemyShieldHealth;
                enemyHealth -= remainingDamage;
                enemyShieldHealth = 0;
                isEnemyShieldActive = false;
                enemyShieldActiveFlag.SetActive(false);  // Deactivate the enemy shield flag
                Debug.Log("Enemy Health after shield depleted: " + enemyHealth);
            }
        }
        else
        {
            enemyHealth -= 10;
            Debug.Log("Enemy Health: " + enemyHealth);
        }

        UpdateUI();
    }
}

void LaunchGolfAttack()
{
    if (golfAttack != null)
    {
        golfAttack.LaunchGolfAttack();
    }

    if (enemyIsVisible.activeSelf)
    {
        if (isEnemyShieldActive)
        {
            ShieldSparks();
            int damageToShield = 10;

            if (enemyShieldHealth >= damageToShield)
            {
                enemyShieldHealth -= damageToShield;
                if (enemyShieldHealth == 0){
                    isEnemyShieldActive = false;
                    enemyShieldActiveFlag.SetActive(false);  // Deactivate the enemy shield flag
                }
                Debug.Log("Enemy Shield Health: " + enemyShieldHealth);
            }
            else
            {
                int remainingDamage = damageToShield - enemyShieldHealth;
                enemyHealth -= remainingDamage;
                enemyShieldHealth = 0;
                isEnemyShieldActive = false;
                enemyShieldActiveFlag.SetActive(false);  // Deactivate the enemy shield flag
                Debug.Log("Enemy Health after shield depleted: " + enemyHealth);
            }
        }
        else
        {
            enemyHealth -= 10;
            Debug.Log("Enemy Health: " + enemyHealth);
        }

        UpdateUI();
    }
}


    private IEnumerator ReloadAndRefillAmmo()
    {
        isReloading = true;

        // Disable all buttons during reload
        foreach (Button button in allButtons)
        {
            button.interactable = false;
        }

        // Call the reload function on the shooter object
        shooterObject.GetComponent<HoldGun>().Reload();

        // Wait for 2 seconds before refilling ammo
        yield return new WaitForSeconds(2f);

        // Refill ammo
        ammo = 6;
        UpdateUI();

        // Re-enable all buttons after reload
        foreach (Button button in allButtons)
        {
            button.interactable = true;
        }

        isReloading = false;
    }

    // Reload method
    void Reload()
    {
        if (!isReloading && ammo == 0) // Prevent reload while already reloading
        {
            StartCoroutine(ReloadAndRefillAmmo());
        }
    }

    void ActivateShield()
    {
        if (playerShieldCount > 0 && !isPlayerShieldActive)
        {
            playerShieldHealth = 30;
            isPlayerShieldActive = true;  // Activate the player's shield
            playerShieldCooldown.fillAmount = 1f; // Set the shield to full
            playerShieldCount--;
            playerShieldText.text = "Shield: " + playerShieldCount; // Update the text immediately
        }
    }

    void ActivateEnemyShield()
    {
        if (enemyShieldCount > 0 && !isEnemyShieldActive)
        {
            enemyShieldHealth = 30;
            isEnemyShieldActive = true;
            enemyShieldCount--;
            enemyShieldText.text = "Enemy Shield: " + enemyShieldCount;
            enemyShieldActiveFlag.SetActive(true);
        }
    }

    void Update()
    {
        // Check if player health drops below 0
        if (playerHealth <= 0)
        {
            ResetPlayerStats();
        }

        // Check if enemy health drops below 0
        if (enemyHealth <= 0)
        {
            ResetEnemyStats();
        }
    }

    void ResetPlayerStats()
    {
        playerHealth = 100;
        ammo = 6;
        playerShieldCount = 3;
        playerbomb = 2;
        UpdateUI();
    }

    void ResetEnemyStats()
    {
        enemyHealth = 100;
        enemyAmmo = 6;
        enemyShieldCount = 3;
        enemybomb = 2;
        UpdateUI();
    }

    void UpdateTimer()
    {
        if (timer > 0)
        {
            timer--;
            UpdateUI();
        }
    }

    void ShieldSparks()
{
    if (enemyShieldActiveFlag != null)
    {
        // Assuming 'ShieldSparks()' is a method on the enemy shield flag that creates sparks when the shield is hit
        enemyShieldActiveFlag.GetComponent<EnemyShieldHandler>().ShieldSparks();  
    }
}

    void UpdateUI()
{
    scoreText.text = "Score: " + score;
    timerText.text = "Time: " + Mathf.FloorToInt(timer / 60) + ":" + (timer % 60).ToString("00");
    playerAmmoText.text = "Ammo: " + ammo;
    playerShieldText.text = "Shield: " + playerShieldCount;
    playerhealthText.text = "Health: " + playerHealth;
    enemyHealthText.text = "Enemy Health: " + enemyHealth;
    enemyAmmoText.text = "Enemy Ammo: " + enemyAmmo;
    enemyShieldText.text = "Enemy Shield: " + enemyShieldCount;

    // Update the player's shield cooldown based on the remaining shield health
    if (playerShieldHealth > 0)
    {
        playerShieldCooldown.fillAmount = (float)playerShieldHealth / 30f;  // 30 is the maximum shield health
    }
    else
    {
        playerShieldCooldown.fillAmount = 0f;  // No shield left
    }

    // Update the enemy shield cooldown (based on enemy's shield health)
    enemyShieldCooldown.fillAmount = (float)enemyShieldHealth / 30f;  // Assuming 30 is max enemy shield health

    // Update the bomb icons for the player
    for (int i = 0; i < playerBombIcons.Length; i++)
    {
        if (i < playerbomb)
        {
            playerBombIcons[i].enabled = true;  // Show bomb icon
        }
        else
        {
            playerBombIcons[i].enabled = false;  // Hide bomb icon
        }
    }

    // Update the bomb icons for the enemy
    for (int i = 0; i < enemyBombIcons.Length; i++)
    {
        if (i < enemybomb)
        {
            enemyBombIcons[i].enabled = true;  // Show bomb icon
        }
        else
        {
            enemyBombIcons[i].enabled = false;  // Hide bomb icon
        }
    }
}

}
