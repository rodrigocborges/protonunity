using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorObject : MonoBehaviour
{
    [SerializeField] private float speed;
    
    void Update()
    {
        transform.eulerAngles += new Vector3(0, 0, Time.deltaTime * speed * 45f);
    }
}
