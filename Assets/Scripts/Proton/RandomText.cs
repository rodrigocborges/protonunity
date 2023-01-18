using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomText : MonoBehaviour
{
    [SerializeField] private string prefix;
    [SerializeField] private int minValue;
    [SerializeField] private int maxValue;

    private TMPro.TMP_Text textComponent;

    void Awake(){
        textComponent = GetComponent<TMPro.TMP_Text>();
        textComponent.text = string.Format("{0}{1}", (string.IsNullOrEmpty(prefix) ? "" : prefix + "_"), Random.Range(minValue, maxValue));
    }
}
