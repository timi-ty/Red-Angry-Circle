using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float waveAmplitude;
    public float waveSpeed;
    float angle;
    Vector2 direction;
    int playerRelatedLayer = 8;
    public GameObject destructionEffect;
    TrailRenderer myTrail;
    void Start()
    {
        myTrail = GetComponent<TrailRenderer>();
        myTrail.startColor = GetComponent<SpriteRenderer>().color;
        myTrail.endColor = new Color(1, 1, 1, 0);

        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        direction = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle)) * 1000;

        Destroy(gameObject, 2);
    }

    void FixedUpdate()
    {
        angle += (waveSpeed * 2 * Mathf.PI * Time.fixedDeltaTime) % (2 * Mathf.PI);
        float nudgeValue = waveAmplitude * Mathf.Sin(angle);
        Vector3 waveVector = Vector2.Perpendicular(direction.normalized) * nudgeValue;

        transform.position = Vector2.MoveTowards(transform.position, direction, speed * Time.fixedDeltaTime);
        transform.position += waveVector;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.layer != playerRelatedLayer){
            GameObject ob = Instantiate(destructionEffect, transform.position, transform.rotation);

            var ps = ob.GetComponent<ParticleSystem>().main;
            ps.startColor = GetComponent<SpriteRenderer>().color;

            Destroy(gameObject);
        }
    }
}
