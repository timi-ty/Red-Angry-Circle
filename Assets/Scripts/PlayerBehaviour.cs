using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PlayerBehaviour : MonoBehaviour
{
    public EnemyCommander enemyWarLord;
    [Header("Player Arsenal")]
    public GameObject arcShield;
    public GameObject shieldUpgrade;
    public GameObject singleSword;
    public GameObject swordUpgrade;
    public GameObject machineGun;
    public GameObject bulletPrefab;
    public GameObject explosiveDevice;

    [Header("SFX")]
    public AudioClip gunShotSound;
    public AudioClip powerUpSound;
    public AudioClip deathSound;

    SpriteRenderer playerSpriteRenderer;

    [Header("Effects")]
    public GameObject deathEffect;
    public Animator slowMoAnimEffect;
    public TextMeshPro powerUpText;
    Animator playerAnimator, shieldAnimator, swordAnimator, powerUpTextAnimator, bladeLogoAnimator;
    Image bladeMeter;

    [Header("")]
    private bool _darkRush;
    private bool _darkRushFading;
    private float _darkRushLife;
    private bool _isAttacking;
    private float _attackTime;
    public bool darkRush
    {
        set 
        {
            _darkRush = value;
            if(value)
            {
                CinemaFX.StartDarkRushFX();

                powerUpText.text = "DARK RUSH!";

                playerAnimator.SetTrigger("darkPowerUp");
                swordAnimator.SetTrigger("gainingDarkArm");
                shieldAnimator.SetTrigger("gainingDarkArm");
                powerUpTextAnimator.SetTrigger("darkPowerUp");

                arcShield.SetActive(false);
                shieldUpgrade.SetActive(false);

                darkRushLife = 4f;
                swordUpgradeLife = Mathf.Clamp(swordUpgradeLife, darkRushLife, Mathf.Infinity);
                roundsLeft = (int) (darkRushLife * 8);

                CancelInvoke("ShootSporadically");

                for(int i = 0; i < roundsLeft; i++){
                    Invoke("ShootSporadically", 0.125f * i);
                }

                enemyWarLord.EnemyBarage();

                Debug.Log("Entered Dark Rush!");
            }
            else{
                enemyWarLord.KillAllTroopsAfter(0.25f); //ensure that all enemies are dead

                Debug.Log("Ended Dark Rush!");
            }
        }
        get {return _darkRush;}
    }
    private bool darkRushFading
    {
        set 
        {
            _darkRushFading = value;
            if(value)
            {
                CinemaFX.EndDarkRushFX();

                playerAnimator.SetTrigger("darkPowerFade");
                swordAnimator.SetTrigger("losingDarkArm");
                shieldAnimator.SetTrigger("losingDarkArm");
            }
        }
        get {return _darkRushFading;}
    }
    private float darkRushLife
    {
        set
        {
            if(!darkRush) return;
            _darkRushLife = Mathf.Clamp(value, 0, Mathf.Infinity);
            if(_darkRushLife <= 0){
                darkRush = false;
            }
            else if(_darkRushLife <= 0.1f && darkRushFading){
                enemyWarLord.NormalizeSpawn();

                arcShield.SetActive(!shieldUpgraded);
                shieldUpgrade.SetActive(shieldUpgraded);

                GameObject explosion = Instantiate(explosiveDevice, transform.position, 
                                        Quaternion.identity, transform);
                ExplosionImpact impact = explosion.GetComponent<ExplosionImpact>();
                impact.maxExplosionLife = 0.2f;
                impact.maxImpactRadius = Mathf.Max(Boundary.visibleWorldWidth, Boundary.visibleWorldHeight);

                darkRushFading = false;
            }
            else if(_darkRushLife <= 2.0f && _darkRushLife > 1.2f && !darkRushFading){
                darkRushFading = true;
            }
        }
        get {return _darkRushLife;}
    }
    public bool isAttacking
    {
        get => _isAttacking; 
        set
        {
            _isAttacking = value;
            if(attackTime <= 0){
                _isAttacking = false;
            }
            if(_isAttacking){
                BroadcastMessage("Attack", SendMessageOptions.DontRequireReceiver);
            }
            else{
                BroadcastMessage("Defend", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    public float attackTime 
    { 
        get => _attackTime; 
        set 
        {
            _attackTime = value;
            if(_attackTime >= 1){
                _attackTime = 1;
                bladeLogoAnimator.speed = 1;
            } 
            else if(_attackTime <= 0){
                _attackTime = 0;
                isAttacking = false;
                bladeLogoAnimator.speed = 0;
                StartCoroutine("RestoreBladeMeter", true);
            }
            bladeMeter.fillAmount = _attackTime;
        }
    }

    float fireCirlceRadius;
    int roundsLeft;
    bool swordUpgraded, shieldUpgraded;
    bool isSwordFading;
    bool isShieldFading;
    float swordUpgradeLife, shieldUpgradeLife, maxUpgradeLife = 10;
    SpriteRenderer singleSwordSprite, doubleSwordSprite;


    private void Start() {
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        fireCirlceRadius = playerSpriteRenderer.bounds.extents.magnitude - 
                            bulletPrefab.GetComponent<SpriteRenderer>().bounds.extents.y;

        ResetShield();
        ResetSword();

        playerAnimator = GetComponent<Animator>();
        shieldAnimator = shieldUpgrade.GetComponent<Animator>();
        swordAnimator = swordUpgrade.GetComponent<Animator>();
        singleSwordSprite = singleSword.GetComponent<SpriteRenderer>();
        doubleSwordSprite = swordUpgrade.GetComponent<SpriteRenderer>();
        powerUpTextAnimator = powerUpText.GetComponent<Animator>();

        isAttacking = false;
    }
    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.collider.tag.Equals("Enemy")){
            attackTime += 0.1f;
            if(other.otherCollider.tag.Equals("Player")){

                Debug.Log(other.otherCollider.name + " with tag " + other.otherCollider.tag + 
                            " failed to protect your player");
                            
                if(darkRush || (shieldUpgraded && !isAttacking)) return; //invincible in dark rush
                Die();
            }
        }
    }

    public void CollectPowerUp(PowerUp powerUp)
    {
        powerUpText.text = "";

        switch(powerUp.powerType){
            case PowerUp.PowerTypes.SwordUpgrade:
                if(darkRush) return;

                swordUpgraded = true;
                isSwordFading = false;

                swordUpgrade.transform.rotation = singleSword.transform.rotation;

                singleSword.SetActive(false);
                swordUpgrade.SetActive(true);

                
                swordUpgradeLife += maxUpgradeLife;

                playerAnimator.SetTrigger("powerUp");
                swordAnimator.SetTrigger("gainingArm");

                powerUpText.text = "Double Sword!";

                RestoreBladeMeter(false);
                break;
            case PowerUp.PowerTypes.ShieldUpgrade:
                if(darkRush) return;

                shieldUpgraded = true;
                isShieldFading = false;

                shieldUpgrade.transform.rotation = arcShield.transform.rotation;

                arcShield.SetActive(false);
                shieldUpgrade.SetActive(true);

                shieldUpgradeLife += maxUpgradeLife;

                playerAnimator.SetTrigger("powerUp");
                shieldAnimator.SetTrigger("gainingArm");

                powerUpText.text = "Full Shield!";
                break;
            case PowerUp.PowerTypes.MachineGun:
                if(darkRush) return;

                for(int i = 0; i < 20; i++){
                    Invoke("ShootSporadically", 0.2f * (i + roundsLeft));
                }
                roundsLeft += 20;

                powerUpText.text = "Machine Gun!";
                break;
            case PowerUp.PowerTypes.SlowMo:
                CinemaFX.StartSlowMoFX();

                slowMoAnimEffect.SetTrigger("startRipple");

                if(Time.timeScale != 1){
                    CancelInvoke("SlowMoControl");
                    CancelInvoke("ResetTimeScale");
                }
                if(Time.timeScale == 1){
                    Time.timeScale = 0.4f;
                    Time.fixedDeltaTime *= Time.timeScale;
                }
                float duration = 1.5f;
                Invoke("SlowMoControl", duration - 0.5f);

                powerUpText.text = "Slow Mo!";
                break;
        }

        powerUpTextAnimator.SetTrigger("gotPowerUp");

        SoundFX.PlaySound(powerUpSound);

        StartCoroutine("RestoreBladeMeter", false);
    }

    private void Update() {
        if(swordUpgraded){
            swordUpgradeLife -= Time.deltaTime;
            if(swordUpgradeLife <= 0){
                ResetSword();
            }
            else if(swordUpgradeLife <= 2 && !isSwordFading){
                isSwordFading = true;
                swordAnimator.SetTrigger("losingArm");
                if(darkRush){
                    swordAnimator.SetTrigger("losingDarkArm");
                } 
            }
        }
        if(shieldUpgraded){
            shieldUpgradeLife -= Time.deltaTime;
            if(shieldUpgradeLife <= 0){
                ResetShield();
            }
            else if(shieldUpgradeLife <= 2 && !isShieldFading){
                isShieldFading = true;
                shieldAnimator.SetTrigger("losingArm");
                if(darkRush){
                    shieldAnimator.SetTrigger("losingDarkArm");
                }
            }
        }
        if(darkRush){
            darkRushLife -= Time.deltaTime;
        }

        if(isAttacking && !swordUpgraded){
            attackTime -= Time.deltaTime/3.5f;
        }
        else if(attackTime > 0){
            attackTime += Time.deltaTime/1.5f; 
        }
    }

    private void LateUpdate() {
        float r = (1 - attackTime) + (attackTime * 0.9137f);
        float g = (1 - attackTime) + (attackTime * 0.1882f);
        float b = (1 - attackTime) + (attackTime * 0.0706f);
        float a = Mathf.Sqrt(attackTime);
        singleSwordSprite.color = new Color(r, g, b, a);
    }

    void ResetSword(){
        swordUpgradeLife = 0;
        
        //frameOffset predicts that this function is called before the pending rotation frame
        //therefore, it offsets the rotation by one frame ahead to match the pending rotation frame
        Quaternion frameOffset = Quaternion.AngleAxis(7.2f, Vector3.forward);
        singleSword.transform.rotation = swordUpgrade.transform.rotation * frameOffset;
        swordUpgrade.SetActive(false);
        singleSword.SetActive(true);

        swordUpgraded = false;
        isSwordFading = false;

        if(playerAnimator)
            playerAnimator.SetTrigger("powerLost");
    }

    void ResetShield(){
        shieldUpgradeLife = 0;

        //frameOffset predicts that this function is called before the pending rotation frame
        //therefore, it offsets the rotation by one frame ahead to match the pending rotation frame
        Quaternion frameOffset = Quaternion.AngleAxis(7.2f, Vector3.forward);
        arcShield.transform.rotation = shieldUpgrade.transform.rotation * frameOffset;
        shieldUpgrade.SetActive(false);
        arcShield.SetActive(!darkRush); //delay shield activation till "dark rush" has finished

        shieldUpgraded = false;
        isShieldFading = false;

        if(playerAnimator)
            playerAnimator.SetTrigger("powerLost");
    }

    void ShootSporadically()
    {
        if(!gameObject.activeInHierarchy) return;

        if(swordUpgraded && !darkRush){
            darkRush = true;
            return;
        } 

        float offsetAngle = 30 * (roundsLeft % 3);
        if(offsetAngle == 60) offsetAngle = 0;
        else if(offsetAngle == 0) offsetAngle = 60;

        for(int j = 0; j < 4; j++){
            Shoot(offsetAngle + j * 90);
        }

        roundsLeft--;

        SoundFX.PlaySound(gunShotSound);
        CinemaFX.ImpactEffect();
    }

    void Shoot(float angle)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        angle *= Mathf.Deg2Rad;
        Vector3 offsetPos = new Vector3(-Mathf.Sin(angle), Mathf.Cos(angle), 0) * fireCirlceRadius;
        SpriteRenderer sr = Instantiate(bulletPrefab, offsetPos + transform.position, rotation, 
                        machineGun.transform).GetComponent<SpriteRenderer>();
        if(darkRush)
            sr.color = playerSpriteRenderer.color;
    }

    void SlowMoControl(){
        CinemaFX.EndSlowMoFX();

        slowMoAnimEffect.SetTrigger("endRipple");

        Invoke("ResetTimeScale", 0.5f);
    }

    void ResetTimeScale(){
        float resetScale = 1/Time.timeScale;
        Time.timeScale = 1;
        Time.fixedDeltaTime *= resetScale;
    }


    void Die()
    {
        Instantiate(deathEffect, transform.position, transform.rotation, transform.parent);
        CinemaFX.BigImpactEffect();
        SoundFX.PlaySound(deathSound);
        isAttacking = false;
        SendMessageUpwards("GameOver");
        Debug.Log("Player Dead!");
        gameObject.SetActive(false);
    }

    void Restart()
    {
        Respawn();
    }

    void Respawn()
    {
        ResetShield();
        ResetSword();
    }

    public void SetBladeMeter(Image meter){
        bladeMeter = meter;
        bladeLogoAnimator = bladeMeter.GetComponentInChildren<Animator>();
        attackTime = 1.0f;
    }

    IEnumerator RestoreBladeMeter(bool wait){
        if(wait) yield return new WaitForSeconds(5);
        for(;;){
            if(attackTime < 1){
                attackTime += Time.deltaTime;
            }
            else{
                break;
            }
            yield return null;
        }
        
    }
}
