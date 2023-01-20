using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviourPun
{
    [SerializeField] private float speed;
    [SerializeField] private int damage;

    private GameObject localPlayer;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();    
        Destroy(gameObject, 3);
    }

    void FixedUpdate()
    {
        rb.velocity = transform.up * speed;
    }

    void OnTriggerEnter2D(Collider2D col){
        col.gameObject.GetComponent<Health>()?.TakeDamage(damage);

        localPlayer?.GetComponent<Score>().AddScore(Random.Range(0, 3));
        localPlayer?.GetComponent<CameraShake>().ShakeCamera();
    }

    public void SetLocalPlayer(GameObject localPlayer)
    {
        this.localPlayer = localPlayer;
    }
}
