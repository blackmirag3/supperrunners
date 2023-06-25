using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunFire : MonoBehaviour
{
    [SerializeField]
    private GunData gunData;

    // Bullet
    public GameObject bullet;

    // Bullet Force
    public float shootForce;

    // Gun stats
    public int ammoLeft, bulletsShot;
    public float rpm;
    public float spread;
    //public float timeBetweenBullets;
    public int bulletsPerShot;
    public int magSize;

    bool shooting, canShoot;
    private Vector3 shotDir;

    [HideInInspector] 
    public Camera playerCam;
    public Transform attackPoint;
    [HideInInspector] 
    public GameObject muzzleFlash;
    [HideInInspector] 
    public TextMeshProUGUI ammoDisplay;

    [SerializeField] private string camName = "PlayerCam";
    [SerializeField] private string guiTextname = "AmmoIndicator";

    // bug fixing
    public bool allowInvoke = true;

    public AudioSource shootingSound;
    public AudioSource emptySound;

    private bool isPaused;

    public GameEvent onPlayerAction;

    private void Awake()
    {
        shootForce = gunData.shootForce;
        rpm = gunData.fireRate;
        spread = gunData.spread;
        //timeBetweenBullets = gunData.timeBetweenBullets;
        bulletsPerShot = gunData.bulletsPerShot;
        magSize = gunData.magSize;        
        canShoot = true;
        ammoLeft = magSize;
    }

    void Start()
    {
        isPaused = false;
        playerCam = GameObject.Find(camName).GetComponent<Camera>();
        ammoDisplay = GameObject.Find(guiTextname).GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (!isPaused)
        {
            MyInput();

            if (ammoDisplay != null)
                ammoDisplay.SetText(ammoLeft.ToString());
        }
    }

    private void MyInput()
    {
        // Check for auto/semi-auto firing mechanism
        if (gunData.isAuto)
            shooting = Input.GetKey(KeyCode.Mouse0);
        else
            shooting = Input.GetKeyDown(KeyCode.Mouse0);

        // Shoot
        if (shooting)
        {
            if (canShoot && ammoLeft > 0) //false if: ammo <= 0 or yet to reset shot
            {
                bulletsShot = 0;
                Shoot();
            }
            else if (ammoLeft <= 0)
            {
                NoAmmo();
            }
        }
    }

 
    private void Shoot()
    { 
        shootingSound.Play();
        canShoot = false;
        onPlayerAction.CallEvent(this, null);

        // Find exact hit pos using a raycast
        // (0.5f, 0.5f, 0) is middle of screen
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        Vector3 targetPoint;
        // Check if ray hits
        if (Physics.Raycast(ray, out RaycastHit hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        // Calculate direction from attackPoint to targetPoint
        shotDir = targetPoint - attackPoint.position;

        ShootOneBullet();
    }

    private void ShootOneBullet()
    {
        Vector3 shotDirSpread = shotDir.normalized;

        // Calculate new dir with spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        float z = Random.Range(-spread, spread);
        shotDirSpread = shotDirSpread + new Vector3(x, y, z);
        shotDirSpread = shotDirSpread.normalized;

        // Instantiate bullet
        GameObject currBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currBullet.SetActive(true);
        currBullet.GetComponent<BulletScript>().damage = gunData.damage;

        // Rotate bullet to shooting direction
        currBullet.transform.forward = shotDirSpread;

        // Add force to bullet
        currBullet.GetComponent<Rigidbody>().AddForce(shotDirSpread * shootForce, ForceMode.Impulse);

        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        
        ammoLeft--;
        bulletsShot++;

        if (allowInvoke && ammoLeft > 0)
        {
            Invoke(nameof(ResetShot), rpm);
            allowInvoke = false; // Invoke once only
        }

        // More than one bullet per tap
        if (bulletsShot < bulletsPerShot && ammoLeft > 0)
            ShootOneBullet();
            //Invoke("ShootOneBullet", timeBetweenBullets);
    }

    private void ResetShot()
    {
        canShoot = true;
        allowInvoke = true;
    }

    private void OnDisable()
    {
        if (ammoDisplay != null)
            ammoDisplay.SetText("\n");
    }

    private void NoAmmo()
    {
       if (Input.GetKeyDown(KeyCode.Mouse0))
            emptySound.Play();
    }

    public void PauseCalled(Component sender, object data)
    {
        if (data is bool)
        {
            isPaused = (bool)data;
            return;
        }
        Debug.Log($"Unwanted event call from {sender}");
    }
}
