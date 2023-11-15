using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(Rigidbody2D))]

public class Player : Character
{
    #region FIELDS
    [SerializeField] StatsBar_HUD statsBar_HUD;
    [SerializeField] bool regenrateHealth = true;
    [SerializeField] float healthRegenerateTime;
    [SerializeField, Range(0f, 1f)] float healthRegeneratePercent;

    [Header("---- INPUT ----")]
    [SerializeField] PlayerInput input;

    [Header("---- MOVE ----")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float accelerationTime = 3f;
    [SerializeField] float decelerationTime = 3f;
    [SerializeField] float moveRotationAngle = 50f;

    [Header("---- FIRE ----")]
    [SerializeField] GameObject projectile1;
    [SerializeField] GameObject projectile2;
    [SerializeField] GameObject projectile3;
    [SerializeField] GameObject projectileOverDrive;
    [SerializeField] ParticleSystem muzzleVFX;
    [SerializeField] Transform muzzleMiddle;
    [SerializeField] Transform muzzleTop;
    [SerializeField] Transform muzzleBottom;
    [SerializeField] AudioData projectileLaunchSFX;
    [SerializeField, Range(0, 2)] int weaponPower = 0;
    [SerializeField] float fireInterval = 0.2f;

    [Header("---- DODGE ----")]
    [SerializeField] AudioData dodgeSFX;
    [SerializeField] int dodgeEnergyCost = 25;
    [SerializeField] float maxRoll = 720f;
    [SerializeField] float rollSpeed = 360f;
    [SerializeField] Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);

    [Header("---- OVERDRIVE ----")]
    [SerializeField] int overdriveDodgeFactor = 2;
    [SerializeField] float overdriveSpeedFactor = 1.2f;
    [SerializeField] float overdriveFireFactor = 1.2f;

    [Header("---- PROJECTILE LEVEUP ----")]
    [SerializeField] float projectileDamageIncrease = 1.0f;

    bool isDodgeing = false;
    bool isOverdriving = false;
    public bool IsFullHealth => health == maxHealth;
    public bool IsFullPower => weaponPower == 2;
    float currentRoll;
    float dodgeDuration;
    readonly float slowMotionDuration = 1f;
    readonly float InvincibleTime = 1f;
    float t;
    float paddingX;
    float paddingY;

    Vector2 moveDirection;
    Vector2 previousVelocity;

    Quaternion previousRotation;



    WaitForSeconds waitForFireInterval;
    WaitForSeconds waitForOverdriveFireInterval;
    WaitForSeconds waitHealthRegenerateTime;
    WaitForSeconds waitDecelerationTime;
    WaitForSeconds waitInvincibleTime;
    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    Coroutine moveCoroutine;
    Coroutine healthRegenerateCoroutine;

    new Rigidbody2D rigidbody;

    new Collider2D collider;

    MissileSystem missile;
    #endregion

    #region UNITY EVENT FUNCTIONS
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        missile = GetComponent<MissileSystem>();

        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;

        dodgeDuration = maxRoll / rollSpeed;
        rigidbody.gravityScale = 0f;

        waitForFireInterval = new WaitForSeconds(fireInterval);
        waitForOverdriveFireInterval = new WaitForSeconds(fireInterval / overdriveFireFactor);
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime);
        waitDecelerationTime = new WaitForSeconds(decelerationTime);
        waitInvincibleTime = new WaitForSeconds(InvincibleTime);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        input.onMove += Move;
        input.onStopMove += StopMove;
        input.onFire += Fire;
        input.onStopFire += StopFire;
        input.onDodge += Dodge;
        input.onOverDrive += Overdrive;
        input.onLaunchMissile += LaunchMissile;

        PlayerOverdrive.on += OverdriveOn;
        PlayerOverdrive.off += OverdriveOff;
    }

    void OnDisable()
    {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
        input.onFire -= Fire;
        input.onStopFire -= StopFire;
        input.onDodge -= Dodge;
        input.onOverDrive -= Overdrive;
        input.onLaunchMissile -= LaunchMissile;

        PlayerOverdrive.on -= OverdriveOn;
        PlayerOverdrive.off -= OverdriveOff;

    }
    
    void Start()
    {
        statsBar_HUD.Initialize(health, maxHealth);

        input.EnableGameplayInput();
        StartCoroutine(nameof(MovePostionLimitCoroutine));
     
        rigidbody.gravityScale = 0f;
        waitForFireInterval = new WaitForSeconds(fireInterval);
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime);
        
        
        
    }
    #endregion

    #region HEALTH
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        //PowerDown();
        statsBar_HUD.UpdateStats(health, maxHealth);
        TimeController.Instance.BulletTime(slowMotionDuration);

        if (gameObject.activeSelf)
        {
            Move(moveDirection);
            StartCoroutine(InvincibleCoroutine());

            if (regenrateHealth)
            {
                if (healthRegenerateCoroutine != null)
                {
                    StopCoroutine(healthRegenerateCoroutine);
                }

                healthRegenerateCoroutine = StartCoroutine(HealthRegenerateCoroutine(waitHealthRegenerateTime, healthRegeneratePercent));
            }
        }
    }

    public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
        statsBar_HUD.UpdateStats(health, maxHealth);
    }

    public override void Die()
    {
        GameManager.onGameOver?.Invoke();
        GameManager.GameState = GameState.GameOver;
        statsBar_HUD.UpdateStats(0f, maxHealth);
        base.Die();
    }

    IEnumerator InvincibleCoroutine()
    {
        collider.isTrigger = true;
        yield return waitInvincibleTime;
        collider.isTrigger = false;
    }
    #endregion

    #region MOVE
    void Move(Vector2 moveInput)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveDirection = moveInput.normalized;
        moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime, moveDirection * moveSpeed, Quaternion.AngleAxis(moveRotationAngle * moveInput.y, Vector3.right)));
        StopCoroutine(nameof(DecelerationCoroutine));
        StartCoroutine(nameof(MovePostionLimitCoroutine));
    }

    void StopMove()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveDirection = Vector2.zero;
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime, moveDirection, Quaternion.identity));
        StartCoroutine(nameof(DecelerationCoroutine));
    }

    IEnumerator MoveCoroutine(float time, Vector2 moveVelocity, Quaternion moveRotation)
    {
        t = 0f;
        previousVelocity = rigidbody.velocity;
        previousRotation = transform.rotation;

        while (t < 1f)
        {
            t += Time.fixedDeltaTime / time;
            rigidbody.velocity = Vector2.Lerp(previousVelocity, moveVelocity, t);
            transform.rotation =  Quaternion.Lerp(previousRotation, moveRotation, t);

            yield return new WaitForFixedUpdate();
        }
    }


    IEnumerator MovePostionLimitCoroutine()
    {
        while (true)
        {
            transform.position = Viewport.Instance.PlayerMovablePosition(transform.position, paddingX, paddingY);
            yield return null;
        }
    }

    IEnumerator DecelerationCoroutine()
    {
        yield return waitDecelerationTime;

        StopCoroutine(nameof(MovePostionLimitCoroutine));
    }

    #endregion

    #region FIRE
    void Fire()
    {
        if (isDodgeing)
        {
            muzzleVFX.Stop();
            return;
        }
        muzzleVFX.Play();
        StartCoroutine(nameof(FireCoroutine));
    }

    void StopFire()
    {
        muzzleVFX.Stop();
        StopCoroutine(nameof(FireCoroutine));
    }

    public void IncreaseProjectileDamage()
    {
        // Assuming that each of your projectiles should have their damage increased
        projectile1.GetComponent<PlayerProjectile>().IncreaseBaseDamage(projectileDamageIncrease);
        projectile2.GetComponent<PlayerProjectile>().IncreaseBaseDamage(projectileDamageIncrease);
        projectile3.GetComponent<PlayerProjectile>().IncreaseBaseDamage(projectileDamageIncrease);
        projectileOverDrive.GetComponent<PlayerProjectile>().IncreaseBaseDamage(projectileDamageIncrease);
    }

    IEnumerator FireCoroutine()
    {
        while (true)
        {

            switch (weaponPower)
            {
                case 0:
                    PoolManager.Release(isOverdriving ? projectileOverDrive : projectile1, muzzleMiddle.position);
                    break;
                case 1:
                    PoolManager.Release(isOverdriving ? projectileOverDrive : projectile1, muzzleTop.position);
                    PoolManager.Release(isOverdriving ? projectileOverDrive : projectile1, muzzleBottom.position);
                    break;
                case 2:
                    PoolManager.Release(isOverdriving ? projectileOverDrive : projectile1, muzzleMiddle.position);
                    PoolManager.Release(isOverdriving ? projectileOverDrive : projectile2, muzzleTop.position);
                    PoolManager.Release(isOverdriving ? projectileOverDrive : projectile3, muzzleBottom.position);
                    break;
                default:
                    break;
            }

            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);

            yield return isOverdriving ? waitForOverdriveFireInterval : waitForFireInterval;
           
        }

     
    }

    #endregion

    #region DODGE
    void Dodge()
    {
        if (isDodgeing || ! PlayerEnergy.Instance.IsEnough(dodgeEnergyCost)) return;
        StartCoroutine (nameof(DodgeCoroutine));
        TimeController.Instance.BulletTime(slowMotionDuration, slowMotionDuration);
    }

    IEnumerator DodgeCoroutine()
    {
        isDodgeing = true;
        AudioManager.Instance.PlayRandomSFX(dodgeSFX);
        PlayerEnergy.Instance.Use(dodgeEnergyCost);
        collider.isTrigger = true;

        StopCoroutine(nameof(FireCoroutine));
        muzzleVFX.Stop();

        currentRoll = 0f;

        var scale = transform.localScale;

        while (currentRoll < maxRoll)
        {
            currentRoll += rollSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);
            transform.localScale = BezierCurve.QuadraticPoint(Vector3.one, Vector3.one, dodgeScale, currentRoll / maxRoll);

            yield return null;
        }

        collider.isTrigger = false;
        isDodgeing = false;

         //闪避结束后如果火力按钮仍然被按住，则恢复开火
        if (input.IsFiring)
        {
            muzzleVFX.Play();
            StartCoroutine(nameof(FireCoroutine));
        }
    }

    #endregion

    #region OVERDRIVE
    void Overdrive()
    {
        if (!PlayerEnergy.Instance.IsEnough(PlayerEnergy.MAX)) return;

        PlayerOverdrive.on.Invoke();
    }

    void OverdriveOn()
    {
        isOverdriving = true;
        dodgeEnergyCost *= overdriveDodgeFactor;
        moveSpeed *= overdriveSpeedFactor;
        TimeController.Instance.BulletTime(slowMotionDuration, slowMotionDuration);
    }

    void OverdriveOff()
    {
        isOverdriving = false;
        dodgeEnergyCost /= overdriveDodgeFactor;
        moveSpeed /= overdriveSpeedFactor;
    }
    #endregion

    #region MISSILE
    void LaunchMissile()
    {
        missile.Launch(muzzleMiddle);
    }

    public void PickUpMissile()
    {
        missile.PickUp();
    }

    #endregion

    #region WEAPON POWER

    public void PowerUp()
    {
        weaponPower = Mathf.Min(weaponPower + 1, 2);
    }

    void PowerDown()
    {
        weaponPower = Mathf.Max(--weaponPower, 0);
    }

    #endregion
}
