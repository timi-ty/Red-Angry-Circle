using System.Collections.Generic;
using UnityEngine;

public class EnemyCommander : MonoBehaviour
{
    public List<GameObject> EnemyTypes = new List<GameObject>();
    public Transform spawnLines;
    Transform topSpawnLine, bottomSpawnLine;
    public Collider2D screenBounds;
    public PlayerBehaviour thePlayer;
    bool inBattle;
    public int totalCasualties;
    public int maxFieldEnemies;
    public List<EnemyBehaviour> EnemiesAlive = new List<EnemyBehaviour>();
    public GameObject powerUp;
    [Header("Death Effects")]
    public GameObject particleDeathEffect;
    public GameObject comicalDeathEffect;
    public GameObject explosiveDevice;
    void Start()
    {
        topSpawnLine = spawnLines.GetChild(0);
        bottomSpawnLine = spawnLines.GetChild(1);
        InvokeRepeating("SpawnEnemies", 0f, 1.0f);
        InvokeRepeating("CleanUp", 20.0f, 20.0f);
    }

    public void EnemyBarage(){
        CancelInvoke("SpawnEnemies");

        InvokeRepeating("SpawnEnemies", 0f, 0.33f);
    }

    public void NormalizeSpawn(){
        CancelInvoke("SpawnEnemies");

        InvokeRepeating("SpawnEnemies", 0f, 1.0f);
    }

    void RetreatAllTroops()
    {
        foreach (var enemy in EnemiesAlive)
        {
            GameObject.Destroy(enemy.gameObject);
        }
        EnemiesAlive.Clear();
    }

    public void KillAllTroops()
    {
        foreach (var enemy in EnemiesAlive)
        {
            enemy.Die(enemy.transform.rotation, gameObject.tag);
        }
    }

    public void KillAllTroopsAfter(float delayInSeconds){
        Invoke("KillAllTroops", delayInSeconds);
    }

    void CleanUp()
    {
        foreach (var enemy in EnemiesAlive)
        {
            if(!(screenBounds.bounds.Contains(enemy.transform.position) || 
                    enemy.GetComponentInChildren<Collider2D>().IsTouching(screenBounds))){
                GameObject.Destroy(enemy.gameObject);
                // EnemyBehaviour's OnDestroy() method updates EnemiesAlive List 
            }
        }
    }
    
    void SpawnEnemies()
    {
        if (!inBattle)
        {
            return;
        }
        int maxSpawn = Mathf.Clamp(totalCasualties/30, 2, maxFieldEnemies);

        if(thePlayer.darkRush) maxSpawn = 30; //dark rush creates enemy rush
        
        for(int i = 0; i < maxSpawn; i++){
            if (EnemiesAlive.Count >= maxFieldEnemies)
            {
                // don't spawn
                break;
            }
            int enemyDecider = Random.Range(0, EnemyTypes.Count - 1);
            GameObject enemyObject = null;
            enemyObject = Instantiate(EnemyTypes[enemyDecider], transform);
            if(enemyObject != null){

                EnemiesAlive.Add(enemyObject.GetComponent<EnemyBehaviour>());
                Collider2D enemyBounds = enemyObject.GetComponentInChildren<Collider2D>();

                if (i % 2 == 0 || !thePlayer.darkRush)
                { //dark rush spawns enemies on both sides
                    enemyObject.transform.position = new Vector2(Random.Range(screenBounds.bounds.min.x
                        + enemyBounds.bounds.extents.x, screenBounds.bounds.max.x
                        - enemyBounds.bounds.extents.x), topSpawnLine.position.y);
                }
                else
                {
                    enemyObject.transform.position = new Vector2(Random.Range(screenBounds.bounds.min.x
                        + enemyBounds.bounds.extents.x, screenBounds.bounds.max.x
                        - enemyBounds.bounds.extents.x), bottomSpawnLine.position.y);
                }

                EnemyBehaviour enBehav = enemyObject.GetComponent<EnemyBehaviour>();
                enBehav.TakeOrders(this, currentPlayer: thePlayer);
            }
        }
    }

    private void OnEnable() 
    {
        inBattle = true;
    }

    private void OnDisable() {
        inBattle = false;
    }

    void Restart(){
        totalCasualties = 0;
        RetreatAllTroops();
    }
}
