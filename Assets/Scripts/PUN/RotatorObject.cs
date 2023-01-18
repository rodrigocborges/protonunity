using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RotatorObject : MonoBehaviourPun
{
    [SerializeField] private float speed;
    
    void Update()
    {
        transform.eulerAngles += new Vector3(0, 0, Time.deltaTime * speed * 45f);
    }
}
