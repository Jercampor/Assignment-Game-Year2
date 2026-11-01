using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarManager : MonoBehaviour
{
    public static HotbarManager instance;

    public PlayerHealth playerHealth;
    public PlayerShooting playerShooting;
    public Slider powerUpTimerSlider;
    public Image powerUpTimerFill;
    
    [System.Serializable]
    public class HotbarSlot
    {
        public PowerUpType powerUpType;
        public int charges;
        public bool isEmpty;
        public Image slotImage;
        public TextMeshProUGUI chargeText;
        public Color powerUpColor;
    }

    public HotbarSlot[] slots = new HotbarSlot[4];
    public float powerUpDuration = 10f;
    private float powerUpTimer = 0f;
    private bool powerUpActive = false;
    private PowerUpType activePowerUp;

    void Awake()
    {
        instance = this;
        foreach (var slot in slots)
        {
            slot.isEmpty = true;
            slot.charges = 0;
            slot.chargeText.text = "";
            slot.slotImage.color = Color.grey;
        }
        
        powerUpTimerSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        if (powerUpActive)
        {
            powerUpTimer -= Time.deltaTime;
            powerUpTimerSlider.value = powerUpTimer / powerUpDuration;
            if (powerUpTimer <= 0f)
                DeactivatePowerUp();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) ActivateSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ActivateSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ActivateSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ActivateSlot(3);
    }

    public void AddPowerUp(PowerUpType type)
    {
        // Check if this power up type already exists in a slot
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].isEmpty && slots[i].powerUpType == type)
            {
                slots[i].charges++;
                UpdateSlotUI(i);
                return;
            }
        }

        // Find first empty slot
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].isEmpty)
            {
                slots[i].powerUpType = type;
                slots[i].charges = 1;
                slots[i].isEmpty = false;
                UpdateSlotUI(i);
                return;
            }
        }

        Debug.Log("Hotbar full!");
    }

    void ActivateSlot(int index)
    {
        if (slots[index].isEmpty) return;
        

        PowerUpType type = slots[index].powerUpType;

        // Block activation if a power up is already active (except health and grenade)
        if (type == PowerUpType.HealthPack)
        {
            Debug.Log("Healing player! Current health before: " + playerHealth.GetCurrentHealth());
            playerHealth.Heal(3);
        }
        else if (type == PowerUpType.Grenade)
        {
            playerShooting.ThrowGrenade();
        }
        else
        {
            activePowerUp = type;
            powerUpActive = true;
            powerUpTimer = powerUpDuration;
            powerUpTimerSlider.maxValue = 1f;
            powerUpTimerSlider.value = 1f;
            powerUpTimerSlider.gameObject.SetActive(true);
            playerShooting.SetPowerUp(type);

            // Set slider color based on power up type
            if (type == PowerUpType.Shotgun)
                powerUpTimerFill.color = Color.cyan;
            else if (type == PowerUpType.Machinegun)
                powerUpTimerFill.color = Color.magenta;
        }

        slots[index].charges--;
        if (slots[index].charges <= 0)
        {
            slots[index].isEmpty = true;
            slots[index].charges = 0;
        }

        UpdateSlotUI(index);
    }

    void DeactivatePowerUp()
    {
        powerUpActive = false;
        powerUpTimerSlider.gameObject.SetActive(false);
        playerShooting.SetPowerUp(null);
    }

    void UpdateSlotUI(int index)
    {
        if (slots[index].isEmpty)
        {
            slots[index].chargeText.text = "";
            slots[index].slotImage.color = Color.grey;
        }
        else
        {
            slots[index].chargeText.text = "x" + slots[index].charges;
            switch (slots[index].powerUpType)
            {
                case PowerUpType.Machinegun:
                    slots[index].slotImage.color = Color.purple;
                    break;
                case PowerUpType.Shotgun:
                    slots[index].slotImage.color = Color.deepSkyBlue;
                    break;
                case PowerUpType.Grenade:
                    slots[index].slotImage.color = Color.orange;
                    break;
                case PowerUpType.HealthPack:
                    slots[index].slotImage.color = Color.red;
                    break;
            }
        }
    }
}