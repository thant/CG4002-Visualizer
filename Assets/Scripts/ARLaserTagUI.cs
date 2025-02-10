using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ARLaserTagUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public GameObject filledHeartPrefab;     
    public GameObject emptyHeartPrefab;      
    public Transform healthContainer;  
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI playerAmmoText;
    public TextMeshProUGUI playerShieldText;
    public Image playerShieldCooldown;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI enemyAmmoText;
    public TextMeshProUGUI enemyShieldText;
    public Image enemyShieldCooldown;
    public Button shootButton;
    public Button reloadButton;
    public Button shieldButton;
    public Button badmintonButton;
    public Button boxingButton;
    public Button snowBombButton;

    public SnowBombAttack snowBombAttack;
    public BadmintonAttack badmintonAttack;
    public BoxingAttack boxingAttack;
    public Button hitButton; 
    public Canvas gameCanvas;
    
    private int score = 0;
    private int ammo = 10;
    private int playerHealth = 5;
    private int playerShield = 3;
    private int enemyHealth = 100;
    private int enemyShield = 3;
    private float timer = 300f; // 5-minute game timer

    private bool isShieldActive = false;
    private float shieldDepletionTime = 2f;
    private float shieldDepletionRate;
    
    void Start()
    {
        // Ensure the UI is properly positioned for landscape mode
        gameCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        gameCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        gameCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        
        UpdateUI();
        UpdateHealthUI(playerHealth);

        shieldDepletionRate = 1f / shieldDepletionTime;
        shootButton.onClick.AddListener(Shoot);
        reloadButton.onClick.AddListener(Reload);
        shieldButton.onClick.AddListener(ActivateShield);
        badmintonButton.onClick.AddListener(LaunchBadmintonAttack);
        boxingButton.onClick.AddListener(LaunchBoxingAttack);
        snowBombButton.onClick.AddListener(snowBombAttack.LaunchSnowbomb);
        hitButton.onClick.AddListener(TakeHit);
        InvokeRepeating("UpdateTimer", 1f, 1f);
    }
    
    void Shoot()
    {
        if (ammo > 0)
        {
            ammo--;
            Debug.Log("Shot fired!");
            UpdateUI();
        }
    }

    void TakeHit()
    {
        if (playerHealth > 0)
        {
            playerHealth--;  // Decrease health by 1
            Debug.Log("Player Health: " + playerHealth);
            UpdateUI();
        }
    }

    
    
    void Reload()
    {
        ammo = 10;
        Debug.Log("Reloaded!");
        UpdateUI();
    }

    void LaunchBadmintonAttack()
    {
    // Call the LaunchBadmintonAttack() method from your BadmintonAttack.cs
        badmintonAttack.LaunchBadmintonAttack();  // This will trigger the attack from your BadmintonAttack script
    }

    void LaunchBoxingAttack()
    {
    // Call the LaunchBadmintonAttack() method from your BadmintonAttack.cs
        boxingAttack.LaunchBoxingAttack();  // This will trigger the attack from your BadmintonAttack script
    }

    void LaunchSnowBombAttack()
    {
    // Call the LaunchBadmintonAttack() method from your BadmintonAttack.cs
        snowBombAttack.LaunchSnowbomb();  // This will trigger the attack from your BadmintonAttack script
    }

    void ActivateShield()
    {
        if (playerShield > 0)
        {
            isShieldActive = true;  // Activate the shield
            playerShieldCooldown.fillAmount = 1f; // Set the shield to full
            playerShieldText.text = "Shield: " + playerShield; // Update the text immediately
        }
    }


    void Update()
    {
        if (isShieldActive)
        {
            // Deplete the shield over time
            playerShieldCooldown.fillAmount -= shieldDepletionRate * Time.deltaTime;

            // If the shield is depleted, stop the depletion and set it to 0
            if (playerShieldCooldown.fillAmount <= 0f)
            {
                playerShieldCooldown.fillAmount = 0f;
                isShieldActive = false; // Shield is no longer active
                playerShield--; // Set the shield to 0 when it's depleted
                UpdateUI(); // Update the UI
            }
        }
    }
    
    void UpdateTimer()
    {
        if (timer > 0)
        {
            timer--;
            UpdateUI();
        }
    }

    void UpdateHealthUI(int health)
{
    // Clear previous health icons
    foreach (Transform child in healthContainer)
    {
        Destroy(child.gameObject);
    }

    int maxHealth = 5;

    for (int i = 0; i < maxHealth; i++)
    {
        GameObject heart;
        if (i < health)
        {
            // Instantiate filled heart (full health)
            heart = Instantiate(filledHeartPrefab, healthContainer);

            // Trigger animation if it's a filled heart
            Animator heartAnimator = heart.GetComponent<Animator>();
            if (heartAnimator != null)
            {
                heartAnimator.Play("HeartFillAnimation");  // Play the fill animation
            }
        }
        else
        {
            // Instantiate empty heart (no health)
            heart = Instantiate(emptyHeartPrefab, healthContainer);
        }
    }
}
    
    void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        timerText.text = "Time: " + Mathf.FloorToInt(timer / 60) + ":" + (timer % 60).ToString("00");
        UpdateHealthUI(playerHealth);
        playerAmmoText.text = "Ammo: " + ammo;
        playerShieldText.text = "Shield: " + playerShield;
        enemyHealthText.text = "Enemy Health: " + enemyHealth;
        enemyAmmoText.text = "Enemy Ammo: ?";
        enemyShieldText.text = "Enemy Shield: " + enemyShield;
        enemyShieldCooldown.fillAmount = (enemyShield > 0) ? 1f : 0f;
    }
}
