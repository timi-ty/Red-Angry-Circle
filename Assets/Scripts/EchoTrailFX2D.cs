using UnityEngine;

public class EchoTrailFX2D : MonoBehaviour
{
    public Sprite echoGraphic;
    public RuntimeAnimatorController echoAnimation;
    private GameObject echoObject;

    private Transform echoSourceTransform;
    private SpriteRenderer echoSourceRenderer;

    [Range(0.01f, 0.1f)]
    public float echoInterval;

    [Range(0.1f, 5f)]
    public float trailLife;
    private float timeEllapsed;

    private Vector3 lastFramePosition;
    void Start()
    {
        echoSourceTransform = GetComponent<Transform>();
        echoSourceRenderer = GetComponent<SpriteRenderer>();

        echoObject = new GameObject();

        SpriteRenderer mySP = echoObject.AddComponent<SpriteRenderer>();
        mySP.sortingOrder = echoSourceRenderer.sortingOrder - 1;

        if(echoGraphic == null){
            mySP.sprite = echoSourceRenderer.sprite;
            mySP.color = echoSourceRenderer.color;
        }
        else{
            mySP.sprite = echoGraphic;
        }
        if(echoAnimation != null){
            Animator myAnim = echoObject.AddComponent<Animator>();
            myAnim.runtimeAnimatorController = echoAnimation;
        }

        echoObject.SetActive(false);
    }

    void Update()
    {
        if(timeEllapsed >= echoInterval && echoSourceRenderer.enabled 
            && lastFramePosition != transform.position){
            GameObject echo = Instantiate(echoObject, echoSourceTransform.position, 
                                            echoSourceTransform.rotation);
            echo.SetActive(true);
            timeEllapsed = 0;
            Destroy(echo, trailLife);
        }
        timeEllapsed += Time.deltaTime;
        lastFramePosition = transform.position;
    }

    private void OnDestroy() {
        Destroy(echoObject);
    }
}
