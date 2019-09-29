using UnityEngine;

public class PartSysSelfDestruct : MonoBehaviour
{
    ParticleSystem me;
    private void OnEnable() 
    {
        me = GetComponent<ParticleSystem>();
        InvokeRepeating("DestroyIfAllParticlesAreGone", 1.0f, 1.0f);
    }

    void DestroyIfAllParticlesAreGone(){
        if(me.particleCount == 0){
            Destroy(this.gameObject, 0.1f);
        }
    }
}
