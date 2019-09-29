using UnityEngine;

public class RotatingArms : MonoBehaviour
{
    GameObject playerObject;
    public GameObject sparkEffect;
    PlayerBehaviour playerState;
    public float armClearance, armSpeed;
    float orbitRadius, scale;
    Vector3 armPos;
    Collider2D heartCollider, armCollider;
    SpriteRenderer armRenderer;
    
    private void Awake() {
        playerObject = transform.parent.gameObject;
        playerState = playerObject.GetComponent<PlayerBehaviour>();

        armRenderer = GetComponent<SpriteRenderer>();
        armCollider = GetComponent<Collider2D>();
    }
    void Start()
    {
        //ensures zero rotation for initial reference
        transform.rotation = Quaternion.identity;

        if(name.Equals("ArcShield")){
            armPos = new Vector2(0, transform.localPosition.magnitude);
        }
        else if (name.Equals("SingleSword")){
            heartCollider = playerObject.GetComponent<Collider2D>();
            armPos = new Vector2(0, armRenderer.bounds.extents.y/playerObject.transform.localScale.y);
        }
        else if(name.Equals("ShieldUpgrade") || name.Equals("SwordUpgrade")){
            armPos = Vector2.zero;
        }
        else{
            Debug.LogWarning("Player Arm Type not recognized."+
                                "\n Arm should have one of the following names: ArcShield, SingleSword, " + 
                                "ShieldUpgrade, SwordUpgrade");
            Debug.LogWarning("Assuming default arm settings");
            armPos = Vector2.zero;
        }

        if(!tag.Equals("Weapon") && !tag.Equals("Shield")){
            Debug.LogWarning("Tag your Player Arm as either a 'Weapon' or 'Shield' for proper operation");
        }


        transform.localPosition = armPos;
        orbitRadius = transform.localPosition.magnitude;
    }

    private void OnEnable() {
        if(playerState.isAttacking){
            Attack();
        }
        else {
            Defend();
        }
    }
    
    void FixedUpdate()
    {
        float angVel = armSpeed * 360 * scale;

        angVel *= Time.fixedDeltaTime;
        transform.rotation *= Quaternion.AngleAxis(angVel, Vector3.forward); 

        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

        armPos.y = (orbitRadius + armClearance) * Mathf.Cos(angle);
        armPos.x = (orbitRadius + armClearance) * Mathf.Sin(-angle);
        transform.localPosition = armPos;  
    }

    private void OnCollisionEnter2D(Collision2D other) {
        GameObject ob = Instantiate(sparkEffect, other.GetContact(0).point, Quaternion.identity);
        var ps = ob.GetComponent<ParticleSystem>().main;
        ps.startColor = GetComponent<SpriteRenderer>().color;
    }

    void Attack()
    {
        scale = 2;
        if(tag.Equals("Weapon")){
            armCollider.isTrigger = false;
            armRenderer.enabled = true;
        }
        else if(tag.Equals("Shield")){
            armCollider.isTrigger = true;
            armRenderer.enabled = false;
        }
    }

    void Defend()
    {
        scale = 1.0f;
        if(tag.Equals("Shield")){
            armCollider.isTrigger = false;
            armRenderer.enabled = true;
        }
        else if(tag.Equals("Weapon")){
            armCollider.isTrigger = true;
            armRenderer.enabled = false;
        }
    }


    void Restart()
    {
        Start();
    }
}
