using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionImpact : MonoBehaviour
{
    CircleCollider2D impactCollider;
    public float maxExplosionLife;
    public float maxImpactRadius;
    private float explosionAcceleration;
    private float explosionLife;
    SpriteRenderer impactCircle;

    void Start()
    {
        impactCollider = GetComponent<CircleCollider2D>();
        impactCircle = GetComponentInChildren<SpriteRenderer>();
        if(!impactCollider){
           impactCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        impactCollider.radius = 0;
        explosionLife = 0;

        explosionAcceleration = (2.0f * maxImpactRadius)/(Mathf.Pow(maxExplosionLife, 2.0f));

        SoundFX.PlayExplosionSound();
        CinemaFX.BigImpactEffect();
    }

    // Update is called once per frame
    void Update()
    {
        explosionLife += Time.deltaTime;
        explosionLife = Mathf.Clamp(explosionLife, 0, maxExplosionLife);
        // explosionLife = Mathf.Lerp(explosionLife, maxExplosionLife, 0.5f);
        impactCollider.radius = (explosionAcceleration * Mathf.Pow(explosionLife, 2.0f))/2.0f;

        impactCircle.transform.localScale = Vector3.one * impactCollider.radius * 0.3858f;

        float a = 1 - impactCollider.radius/maxImpactRadius;
        float r = impactCircle.color.r * a;
        float g = impactCircle.color.g * a;
        float b = impactCircle.color.b * a;
        impactCircle.color = new Color(r, g, b, a);

        if(Mathf.Approximately(impactCollider.radius, maxImpactRadius)){
            Destroy(impactCollider);
            Destroy(impactCircle);
            Destroy(this);
        }
    }
}
