using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();    
        Destroy(gameObject, 5);
    }

    void FixedUpdate()
    {
        rb.velocity = transform.up * speed;
    }
}
