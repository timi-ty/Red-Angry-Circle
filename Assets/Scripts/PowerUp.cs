using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerTypes{SwordUpgrade, ShieldUpgrade, MachineGun, SlowMo}
    public PowerTypes powerType;
    PlayerBehaviour player;
    int playerRelatedLayer;
    float fallSpeed;
    public GameObject collectionEffect;
    public GameObject upgradeEffect;
    private bool isCollected;

    void Start()
    {
        player = FindObjectOfType<PlayerBehaviour>();
        playerRelatedLayer = 8;
        powerType = (PowerTypes) Random.Range(0, 4);
        if(powerType == PowerTypes.SlowMo){
            powerType = (PowerTypes) Random.Range(0, 4);
            if(powerType != PowerTypes.SlowMo){
                Destroy(gameObject);
                return;
            }
        }
        switch(powerType){
            case PowerTypes.SwordUpgrade:
                fallSpeed = 1.0f;
                GetComponent<SpriteRenderer>().color = new Color(0.91f, 0.18f, 0.17f);
                break;
            case PowerTypes.ShieldUpgrade:
                fallSpeed = 0.5f;
                GetComponent<SpriteRenderer>().color = new Color(0.50f, 1.0f, 0.35f);
                break;
            case PowerTypes.MachineGun:
                fallSpeed = 1.5f;
                GetComponent<SpriteRenderer>().color = new Color(0.93f, 0.24f, 0.62f);
                break;
            case PowerTypes.SlowMo:
                fallSpeed = 2.0f;
                GetComponent<SpriteRenderer>().color = new Color(0.35f, 1.0f, 1.0f);
                break;
        }
        
        Debug.Log("Power Up Type is: " + powerType);

        isCollected = false;

        StartCoroutine("DestroyIfBeyondScreen");
    }


    private void Update() {
        transform.position = Vector2.MoveTowards(transform.position, Vector2.down * 1000, 
                                fallSpeed * Time.deltaTime);
    }

    IEnumerator DestroyIfBeyondScreen(){
        for(;;){
            if(transform.position.y < -Boundary.visibleWorldExtents.y){
                Destroy(gameObject, 0.5f);
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(isCollected) return;
        if(other.gameObject.layer == playerRelatedLayer && other.isTrigger == false){
            isCollected = true;

            //Power Up exploding
            GameObject ob = Instantiate(collectionEffect, transform.position, transform.rotation);

            //Player upgrade activating
            if(player == null){
                player = FindObjectOfType<PlayerBehaviour>();
            }
            GameObject ob1 = Instantiate(upgradeEffect, player.transform.position, 
                                Quaternion.identity, player.transform);

            Debug.Log(other.gameObject.name + " hit the power up");

            var ps = ob.GetComponent<ParticleSystem>().main;
            ps.startColor = GetComponent<SpriteRenderer>().color;

            var ps1 = ob1.GetComponent<ParticleSystem>().main;
            ps1.startColor = GetComponent<SpriteRenderer>().color;

            player.CollectPowerUp(this);

            Destroy(gameObject);
        }
    }
}
