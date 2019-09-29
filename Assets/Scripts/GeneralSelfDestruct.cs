using UnityEngine;

public class GeneralSelfDestruct : MonoBehaviour
{
    public float objectLifetime;
    private void OnEnable() 
    {
        float timeToDestroy = objectLifetime + 0.5f;
        Destroy(this.gameObject, timeToDestroy);
    }
}
