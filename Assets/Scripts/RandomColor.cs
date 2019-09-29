using UnityEngine;

public class RandomColor : MonoBehaviour
{
    SpriteRenderer myRenderer;
    void Start()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        myRenderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.7f, 1f, 1f, 1f);
    }
}
