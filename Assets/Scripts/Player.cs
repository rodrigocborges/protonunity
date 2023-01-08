using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rb;
    private float nextFire = 0;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void LookAtMouse(){
        Vector2 mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.up = (Vector3)(mousePosition - new Vector2(transform.position.x, transform.position.y));
    }

    public void Move(){
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.velocity = input.normalized * moveSpeed;
    }

    public void Fire(GameObject localPlayer){
        if(Input.GetMouseButton(0) && Time.time > nextFire){
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);
            bullet.GetComponent<Bullet>().SetLocalPlayer(localPlayer);
            AudioUtil.PlayOneShot(audioSource, fireSound);
            nextFire = Time.time + fireRate;
        }
    }
}
