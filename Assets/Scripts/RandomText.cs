using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomText : MonoBehaviour
{
    public List<string> TextList = new List<string>(); 
    void Start()
    {
        string text = TextList[Random.Range(0, TextList.Count)];
        TextMeshPro mTextMesh = GetComponent<TextMeshPro>();
        Text mText = GetComponent<Text>();
        if(mTextMesh){
            mTextMesh.text = text;
        }
        if(mText){
            mText.text = text;
        }
    }
}
