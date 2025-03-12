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
    public Image[] playerBombIcons;  // bomb array
    public Image[] enemyBombIcons;
    public Image shieldOverlay;  // blue tint
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
    public GameObject enemyObject;  
    public GameObject enemyIsVisible;  // visibility flag -- very important
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
    public int shieldHealth = 30; 
    private bool isPlayerShieldActive = false;  // flag for if own shield is active
    private bool isEnemyShieldActive = false;  // enemy shield active flag
    private float shieldDepletionRate = 1f;  // if need timer based tickdown of shield value
    private bool isReloading = false; // check if reloading to disable buttons
    private Button[] allButtons;
    private int playerShieldHealth = 0; 
    private int enemyShieldHealth = 0; 

    void Start()
    {
        //button array for easy expansion
        allButtons = new Button[] { shootButton, reloadButton, shieldButton, badmintonButton, boxingButton, snowBombButton, fencingButton, golfButton, hitButton };

        //scaling UI to screen size
        gameCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        gameCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        gameCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        if (shieldOverlay != null){
        RectTransform rt = shieldOverlay.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;  // Bottom-left of blue tint
        rt.anchorMax = Vector2.one;   // Top-right of blue tint
        rt.offsetMin = Vector2.zero;  // No offset
        rt.offsetMax = Vector2.zero;  // No offset
        }

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
                shooterObject.GetComponent<HoldGun>().Shoot();
            }
            else
            {
                Debug.LogWarning("❌ Shooter object not assigned!");
            }
            if (enemyIsVisible.activeSelf)
            {
                if (isEnemyShieldActive)
                {
                    ShieldSparks();
                    int damageToShield = 5;
                    if (enemyShieldHealth >= damageToShield)
                    {
                        enemyShieldHealth -= damageToShield;
                        if (enemyShieldHealth == 0){
                            isEnemyShieldActive = false;
                            enemyShieldActiveFlag.SetActive(false);
                            }
                        Debug.Log("Enemy Shield Health: " + enemyShieldHealth);
                    }
                    else
                    {
                        int remainingDamage = damageToShield - enemyShieldHealth;
                        enemyHealth -= remainingDamage;
                        enemyShieldHealth = 0;
                        isEnemyShieldActive = false;
                        Debug.Log("Enemy Health after shield depleted: " + enemyHealth);
                    }
                }
                else
                {
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
        if (isPlayerShieldActive)
        {
            int damageToShield = 10;
            if (playerShieldHealth >= damageToShield)
            {
                playerShieldHealth -= damageToShield;
                if (playerShieldHealth == 0)
                {
                    isPlayerShieldActive = false;
                    shieldOverlay.enabled = false;
                }
            }
            else
            {
                int remainingDamage = damageToShield - playerShieldHealth;
                playerHealth -= remainingDamage;
                playerShieldHealth = 0;
                isPlayerShieldActive = false;  // Shield deactivated
                shieldOverlay.enabled = false;
            }
        }
        else
        {
            playerHealth -= 10;
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
                    enemyShieldActiveFlag.SetActive(false);
                }
                Debug.Log("Enemy Shield Health: " + enemyShieldHealth);
            }
            else
            {
                int remainingDamage = damageToShield - enemyShieldHealth;
                enemyHealth -= remainingDamage;
                enemyShieldHealth = 0;
                isEnemyShieldActive = false;
                enemyShieldActiveFlag.SetActive(false);
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

 public void UpdatePlayerStats(int hp, int bullets, int shieldHp)
    {
        this.playerHealth = hp;
        this.ammo = bullets;
        this.playerShieldHealth = shieldHp;
    }

public void UpdateAction(string action)
{
        if (action == "badminton"){
            LaunchBadmintonAttack();
        }
        else if (action == "boxing"){
            LaunchBoxingAttack();
        }
        else if (action == "fencing"){
            LaunchFencingAttack();
        }
        else if (action == "snowball"){
            LaunchSnowBombAttack();
        }
        else if (action == "golf"){
            LaunchGolfAttack();
        }
        else if (action == "reload"){
            Reload();
        }
        else if (action == "shield"){
            ActivateShield();
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
                    enemyShieldActiveFlag.SetActive(false);
                }
                Debug.Log("Enemy Shield Health: " + enemyShieldHealth);
            }
            else
            {
                int remainingDamage = damageToShield - enemyShieldHealth;
                enemyHealth -= remainingDamage;
                enemyShieldHealth = 0;
                isEnemyShieldActive = false;
                enemyShieldActiveFlag.SetActive(false);
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
                    enemyShieldActiveFlag.SetActive(false);
                }
                Debug.Log("Enemy Shield Health: " + enemyShieldHealth);
            }
            else
            {
                int remainingDamage = damageToShield - enemyShieldHealth;
                enemyHealth -= remainingDamage;
                enemyShieldHealth = 0;
                isEnemyShieldActive = false;
                enemyShieldActiveFlag.SetActive(false);
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
                    enemyShieldActiveFlag.SetActive(false);
                }
                Debug.Log("Enemy Shield Health: " + enemyShieldHealth);
            }
            else
            {
                int remainingDamage = damageToShield - enemyShieldHealth;
                enemyHealth -= remainingDamage;
                enemyShieldHealth = 0;
                isEnemyShieldActive = false;
                enemyShieldActiveFlag.SetActive(false);
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
        foreach (Button button in allButtons)
        {
            button.interactable = false;
        }
        shooterObject.GetComponent<HoldGun>().Reload();
        yield return new WaitForSeconds(2f);
        ammo = 6;
        UpdateUI();
        foreach (Button button in allButtons)
        {
            button.interactable = true;
        }
        isReloading = false;
    }
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
        shieldOverlay.enabled = true;  // Show the blue tint
        playerShieldCooldown.fillAmount = 1f;
        playerShieldCount--;
        playerShieldText.text = "Shield: " + playerShieldCount;
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

    if (playerShieldHealth > 0)
    {
        playerShieldCooldown.fillAmount = (float)playerShieldHealth / 30f;
    }
    else
    {
        playerShieldCooldown.fillAmount = 0f;
    }

    enemyShieldCooldown.fillAmount = (float)enemyShieldHealth / 30f;

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
