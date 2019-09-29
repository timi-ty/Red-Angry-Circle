using UnityEngine;

public class CinemaFX : MonoBehaviour
{
    public int impactCycles = 8;
    private static int _impactCycles;

    private static Camera sceneCamera;

    [Range(0.0f, 1.0f)]
    public float cycleLifetime;
    private static float _cycleLifetime;
    private static MonoBehaviour monoBehaviour;

    [Range(0.0f, 1.0f)]
    public float maxDeflection;
    public GameObject gameBoundary;

    public Animator overlayFXAnimator, cameraFXAnimator;

    private static Animator _overlayFXAnimator, _cameraFXAnimator;

    private void Awake(){
        _impactCycles = impactCycles;
        _cycleLifetime = cycleLifetime;

        _overlayFXAnimator = overlayFXAnimator;
        _cameraFXAnimator = cameraFXAnimator;

        sceneCamera = Camera.main;

        monoBehaviour = this;

        AdgustGameBoundaryToFitScreen();
    }

    public static void ImpactEffect()
    {
        for(int i = 0; i < _impactCycles; i ++){
            monoBehaviour.Invoke("NudgeCamera", i * _cycleLifetime);
        }
        monoBehaviour.Invoke("ResetCamera", _impactCycles * _cycleLifetime);
    }

    public static void BigImpactEffect()
    {
        for(int i = 0; i < _impactCycles*2; i ++){
            monoBehaviour.Invoke("BigNudgeCamera", i * _cycleLifetime*1.2f);
        }
        monoBehaviour.Invoke("ResetCamera", _impactCycles * _cycleLifetime*1.2f);
    }

    public static void StartSlowMoFX() => _overlayFXAnimator.SetTrigger("slowMoEnter");

    public static void EndSlowMoFX() => _overlayFXAnimator.SetTrigger("slowMoExit");

    public static void StartDarkRushFX()
    {
        _cameraFXAnimator.SetTrigger("darkRushEnter");
        _overlayFXAnimator.SetTrigger("darkRushEnter");
    }

    public static void EndDarkRushFX()
    {
        _cameraFXAnimator.SetTrigger("darkRushExit");
        _overlayFXAnimator.SetTrigger("darkRushExit");
    }

    private void NudgeCamera()
    {
        float nudgeY = Random.Range(-maxDeflection, maxDeflection);
        float nudgeX = Random.Range(-maxDeflection, maxDeflection);
        float posZ = sceneCamera.transform.position.z;

        sceneCamera.transform.position = new Vector3(nudgeX, nudgeY, posZ);
    }

    private void BigNudgeCamera()
    {
        float nudgeY = Random.Range(-maxDeflection*2, maxDeflection*2);
        float nudgeX = Random.Range(-maxDeflection*2, maxDeflection*2);
        float posZ = sceneCamera.transform.position.z;

        sceneCamera.transform.position = new Vector3(nudgeX, nudgeY, posZ);
    }

    private void ResetCamera()
    {
        float posZ = sceneCamera.transform.position.z;

        sceneCamera.transform.position = new Vector3(0, 0, posZ);
    }

    void AdgustGameBoundaryToFitScreen()
    {
        //Ensures unity reference scale for boundary fitting
        gameBoundary.transform.localScale = new Vector3(1, 1, 1);

        Vector3 topRight = sceneCamera.WorldToScreenPoint
                        (gameBoundary.GetComponent<SpriteRenderer>().bounds.max);
        Vector3 bottomLeft = sceneCamera.WorldToScreenPoint
                        (gameBoundary.GetComponent<SpriteRenderer>().bounds.min);
        float innerBoxPixelHeight = (topRight.y - bottomLeft.y) * 0.9091f;

        float scaleFactorY = sceneCamera.pixelHeight / innerBoxPixelHeight;
        float scaleFactorX = scaleFactorY * sceneCamera.aspect;

        gameBoundary.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 0);
    }

    
}
