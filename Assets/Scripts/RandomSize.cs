using UnityEngine;

public class RandomSize : MonoBehaviour
{
    public float minScale;
    public float maxScale;
    void Start()
    {
        float scale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector3(scale, scale, scale) * GameManager.Scale;

        return;
    }
}
