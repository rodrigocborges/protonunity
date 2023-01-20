using System.Collections;
using System.Collections.Generic;
using Proton;
using UnityEngine;

public class BulletNetworkProton : MonoBehaviour
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
        PlayerNetworkProton otherPlayer = col.gameObject.GetComponent<PlayerNetworkProton>();
        if(otherPlayer != null){
            //Se não for eu, aplica o dano
            if(!otherPlayer.GetComponent<EntityIdentity>().IsMine())
                otherPlayer.TakeDamage(damage);
        }

        localPlayer?.GetComponent<PlayerNetworkProton>().AddScore(Random.Range(0, 3));
        localPlayer?.GetComponent<CameraShake>().ShakeCamera();
    }

    public void SetLocalPlayer(GameObject localPlayer)
    {
        this.localPlayer = localPlayer;
    }
}
