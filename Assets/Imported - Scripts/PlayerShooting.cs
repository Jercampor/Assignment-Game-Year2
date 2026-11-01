using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class PlayerShooting : MonoBehaviour
{
    
    public Image ammoCircle;
    public Image reserveIndicator;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public Transform firePoint;
    
    public Light muzzleLight;
    public float muzzleFlashDuration = 0.5f;
    
    public GameObject grenadePrefab;
    public float throwForce = 15f;
    

    public int maxAmmo = 10;
    private int currentAmmo;
    public int maxReserveAmmo = 100;
    public int startingReserveAmmo = 30;
    private int reserveAmmo;
    public float reloadTime = 1.5f;
    private bool isReloading = false;

    public TextMeshProUGUI ammoText;
    private PlayerController playerController;
    private PowerUpType? activePowerUp = null;

    // Machinegun
    public float machinegunFireRate = 0.1f;
    private float machinegunTimer = 0f;

    // Shotgun
    public int shotgunPellets = 5;
    public float shotgunSpread = 15f;

    void Start()
    {
        currentAmmo = maxAmmo;
        reserveAmmo = startingReserveAmmo;
        UpdateAmmoUI();
        playerController = GetComponent<PlayerController>();
        muzzleLight.enabled = true;
        muzzleLight.enabled = false;
    }

    void Update()
    {
        if (isReloading) return;
        if (playerController.isMeleeMode) return;

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            if (reserveAmmo > 0 && currentAmmo < maxAmmo)
                StartCoroutine(Reload());
        }

        if (activePowerUp == PowerUpType.Machinegun)
        {
            machinegunTimer += Time.deltaTime;
            if (Input.GetMouseButton(0) && currentAmmo > 0 && machinegunTimer >= machinegunFireRate)
            {
                Shoot();
                machinegunTimer = 0f;
            }
        }
        else if (activePowerUp == PowerUpType.Shotgun)
        {
            if (Input.GetMouseButtonDown(0) && currentAmmo > 0)
                ShootShotgun();
        }
        else if (activePowerUp == PowerUpType.Grenade)
        {
            if (Input.GetMouseButtonDown(0) && currentAmmo > 0)
                ThrowGrenade();
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && currentAmmo > 0)
                Shoot();
        }
        
        
        
        
    }

    void Shoot()
    {
        currentAmmo--;
        AudioManager.instance.PlayGunshot();
        StartCoroutine(MuzzleFlash());
        UpdateAmmoUI();
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody>().linearVelocity = firePoint.forward * bulletSpeed;
        Destroy(bullet, 3f);
        
    }

    void ShootShotgun()
    {
        currentAmmo--;
        StartCoroutine(MuzzleFlash());
        AudioManager.instance.PlayGunshot();
        UpdateAmmoUI();
        for (int i = 0; i < shotgunPellets; i++)
        {
            float spread = Random.Range(-shotgunSpread, shotgunSpread);
            Quaternion spreadRotation = Quaternion.Euler(0, spread, 0) * firePoint.rotation;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, spreadRotation);
            bullet.GetComponent<Rigidbody>().linearVelocity = spreadRotation * Vector3.forward * bulletSpeed;
            Destroy(bullet, 3f);
        }
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);
        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;
        isReloading = false;
        UpdateAmmoUI();
    }
    
    System.Collections.IEnumerator MuzzleFlash()
    {
        Debug.Log("Muzzle flash started");
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(muzzleFlashDuration);
        muzzleLight.enabled = false;
        Debug.Log("Muzzle flash ended");
    }

    public void AddAmmo(int amount)
    {
        reserveAmmo = Mathf.Min(reserveAmmo + amount, maxReserveAmmo);
        UpdateAmmoUI();
    }

    public void SetPowerUp(PowerUpType? type)
    {
        activePowerUp = type;
    }

    void UpdateAmmoUI()
    {
        ammoCircle.fillAmount = (float)currentAmmo / maxAmmo;
        reserveIndicator.color = reserveAmmo > 0 ? Color.yellow : Color.red;
    }
    
    public void ThrowGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * throwForce;
        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }
}