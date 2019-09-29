using UnityEngine;
using TMPro;

public class EnemyBehaviour : MonoBehaviour
{
    private Rigidbody2D playerBody;
    private Rigidbody2D enemyBody;
    private Collider2D enemyCollider;
    private enum EnemyTypes{Class1 = 1, Class2, Class3, Class4, Class5, Boss = 10}
    private EnemyTypes _enemyType;
    private bool _isBomber, _aboutToExplode; 
    private float _payOff;
    private float maxPayOff;
    private EnemyCommander myCommander;
    private Animator enemyAnimator;
    private float level, scale, _lifeTime;
    private SpriteRenderer valueSprite;
    

    private EnemyTypes enemyType
    {
        get => _enemyType; 
        set
        {
            _enemyType = value;
            int explosiveDecider = Random.Range(0, 7);
            isBomber = explosiveDecider == 5;
        }
    }

    public bool isBomber
    {
        get => _isBomber; 
        set
        {
            _isBomber = value;
            if(_isBomber){
                enemyAnimator.enabled = true;
                lifeTime = 100;//placeholder value for lifetime
            }
        }
    }

    private float lifeTime
    {
        get => _lifeTime; 
        set
        {
            _lifeTime = value;
            if(_lifeTime <= 3 && !aboutToExplode) aboutToExplode = true;
            if(lifeTime <= 0) Die(Quaternion.identity, "Suicide");
        }
    }

    private bool aboutToExplode
    {
        get => _aboutToExplode; 
        set
        {
            if(value && !_aboutToExplode) enemyAnimator.SetBool("aboutToExplode", true);
            else if(!value && _aboutToExplode) enemyAnimator.SetBool("aboutToExplode", false);
            _aboutToExplode = value;
        }
    }

    public float payOff 
    { 
        get => _payOff; 
        set 
        {
            _payOff = value; 

            _payOff = Mathf.Clamp(_payOff, 1, _payOff);
            float r = payOff/maxPayOff;
            float g = 0.84313f * r ;
            float b = 1 - (r*r);
            valueSprite.color = new Color(r, g, b);
        }
    }

    public AudioClip deathSound;


    private void OnEnable() 
    {
        enemyBody = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();
        enemyCollider = GetComponentInChildren<Collider2D>();
        valueSprite = GetComponentsInChildren<SpriteRenderer>()[0];
        if(name.Contains("Enemy 1")){
            enemyType = EnemyTypes.Class1;
        }
        else if(name.Contains("Enemy 2")){
            enemyType = EnemyTypes.Class2;
        }
        else if(name.Contains("Enemy 3")){
            enemyType = EnemyTypes.Class3;
        }
        else if(name.Contains("Enemy 4")){
            enemyType = EnemyTypes.Class4;
        }
        else if(name.Contains("Enemy 5")){
            enemyType = EnemyTypes.Class5;
        }
        else if(name.Contains("Boss")){
            enemyType = EnemyTypes.Boss;
        }
        else if(name.Contains("Enemy X")){
            enemyType = (EnemyTypes) Random.Range(1, 6);
        }
        else{
            Debug.LogWarning("Could not determine enemy type.\nAssumed random enemy settings.");
            enemyType = (EnemyTypes) Random.Range(1, 6);
        }
    }

    public void TakeOrders(EnemyCommander myCreator, PlayerBehaviour currentPlayer)
    {
        playerBody = currentPlayer.GetComponent<Rigidbody2D>();

        myCommander = myCreator;

        level = Mathf.Clamp(myCommander.totalCasualties/16.0f, 5.0f, 20.0f);

        if(currentPlayer.darkRush) {
            level = 60; //dark rush creates enemy rush
            payOff = 10;
        }
        else{
            if(isBomber){
                maxPayOff = level * 7.5f;
                lifeTime = maxPayOff;//real lifetime
            }
            else{
                payOff = maxPayOff = level * 2.5f;
            }
        }

        // To let enemy pass through walls when entering the screen space
        enemyCollider.isTrigger = true; 

        InvokeRepeating("ChasePlayer", 0.5f, 0.5f * (level + 5)/level);
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        // Activate the enemy collider to begin hitting walls
        if(other.tag.Equals("Wall")){
            enemyCollider.isTrigger = false;
        }   
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(enemyCollider.isTrigger && (other.tag.Equals("Weapon") || other.tag.Equals("Player") 
            || (other.tag.Equals("Projectile")))){
            Die(Quaternion.identity, other.name);
        }
    }

    void ChasePlayer()
    {
        Vector2 chaseForce = playerBody.position - enemyBody.position;
        float errorAngle;
        switch(enemyType){
            case EnemyTypes.Class1:
                scale = Random.Range(4.5f, 6.0f);
                break;
            case EnemyTypes.Class2:
                scale = Random.Range(6.0f, 7.0f);
                errorAngle = Mathf.Atan2(chaseForce.y, chaseForce.x);
                errorAngle += Random.Range(0.5f, 1.0f) * Mathf.PI/16;
                chaseForce.y = (chaseForce.magnitude) * Mathf.Sin(errorAngle);
                chaseForce.x = (chaseForce.magnitude) * Mathf.Cos(errorAngle);
                break;
            case EnemyTypes.Class3:
                scale = 3;
                break;
            case EnemyTypes.Class4:
                scale = Random.Range(7.0f, 8.0f);
                errorAngle = Mathf.Atan2(chaseForce.y, chaseForce.x);
                errorAngle += Random.Range(0.5f, 1.0f) * Mathf.PI/20;
                chaseForce.y = (chaseForce.magnitude) * Mathf.Sin(errorAngle);
                chaseForce.x = (chaseForce.magnitude) * Mathf.Cos(errorAngle);
                break;
            case EnemyTypes.Class5:
                scale = 9f;
                break;
            case EnemyTypes.Boss:
                scale = 8;
                break;
            default:
                scale = 4;
                break;
        }
        enemyBody.AddForce(scale * (level/10.0f) * 2.5f * chaseForce);
    }

    void Update()
    {
        float angle = enemyCollider.transform.eulerAngles.z;
        float angVel = 45 * scale;

        angle += angVel * Time.deltaTime;
        enemyCollider.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if(isBomber){
            lifeTime -= Time.deltaTime;
            payOff += Time.deltaTime;
        }
        else{
            payOff -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.collider.tag.Equals("Weapon") || other.collider.tag.Equals("Player") 
            || (other.collider.tag.Equals("Projectile"))){
            
            Quaternion rotation = Quaternion.FromToRotation(Vector2.up, 
                                    Vector2.Perpendicular(other.GetContact(0).normal));
            Die(rotation, other.collider.name);
        }
    }

    public void Die(Quaternion rotation, string murderWeapon)
    {
        myCommander.totalCasualties++;

        // GameManager handles scene related effects
        string[] data = new string[2];
        data[0] = murderWeapon;
        data[1] = ((int)payOff).ToString();
        SendMessageUpwards("EnemyDied", data);

        Debug.Log("Enemy killed by " + murderWeapon);

        SoundFX.PlaySound(deathSound);
        CinemaFX.ImpactEffect();

        Quaternion q1 = Quaternion.AngleAxis(Random.Range(-45.0f, 50.0f), Vector3.forward);

        Instantiate(myCommander.comicalDeathEffect, transform.position, 
                            q1, transform.parent.parent);
        
        GameObject psObject = Instantiate(myCommander.particleDeathEffect, transform.position, 
                            rotation, transform.parent.parent);

        var ps = psObject.GetComponent<ParticleSystem>().main;
        ps.startColor = GetComponentsInChildren<SpriteRenderer>()[1].color;

        if(isBomber)
        {
            Explode(rotation);
        }

        if (Random.Range(0, 15) == 3){
            DropPowerUp();
        }

        GameObject.Destroy(gameObject);
    }

    private void Explode(Quaternion rotation)
    {
        GameObject explosion = Instantiate(myCommander.explosiveDevice, transform.position,
                                    rotation, transform.parent);
        ExplosionImpact impact = explosion.GetComponent<ExplosionImpact>();
        impact.maxExplosionLife = 0.5f;
        impact.maxImpactRadius = enemyCollider.bounds.size.magnitude * 3;
    }

    void DropPowerUp()
    {
        if(myCommander.thePlayer.darkRush) return;
        Vector2 pos = new Vector2(transform.position.x, myCommander.screenBounds.bounds.max.y * 1.05f);
        Instantiate(myCommander.powerUp, pos, transform.rotation, transform.parent.parent);
    }

    private void OnDestroy() 
    {
        myCommander.EnemiesAlive.Remove(this);
    }
}
