using System.Collections;
using System.Collections.Generic;
using Proton;
using UnityEngine;

public class EntityScore : MonoBehaviour
{
    [SerializeField] private string prefix;
    private TMPro.TMP_Text textComponent;

    private int _score = 0;

    void Awake(){
        textComponent = GetComponent<TMPro.TMP_Text>();
    }

    public void AddScore(int val){
        _score += val;
        _updateLocalText();
    }

    public void DecreaseScore(int val){
        _score += val;
        _updateLocalText();
    }

    private void _updateLocalText() => textComponent.text = string.IsNullOrEmpty(prefix) ? _score.ToString() : prefix + _score;
}
