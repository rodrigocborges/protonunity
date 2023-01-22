using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Proton;

public class PlayerNetworkProton : MonoBehaviour
{
    public static Vector3 GetRandomSpawnPoint() => new Vector3(Random.Range(-11.4f, 11.4f), Random.Range(-7.13f, 8f), 0);
    [SerializeField] private TMPro.TMP_Text nametagText;
    [SerializeField] private TMPro.TMP_Text scoreText;
    [SerializeField] private TMPro.TMP_Text healthText;
    [SerializeField] private int startHealth;
    [SerializeField] private EntityIdentity entityIdentity;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate;
    [SerializeField] private AudioClip[] fireSound;
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rb;
    private float nextFire = 0;
    private AudioSource audioSource;

    private int score = 0;
    private int health = 0;

    Vector3 lookPosition = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer.color = entityIdentity.IsMine() ? Color.red : Color.blue;

        health = startHealth;

        if(!entityIdentity.IsMine())
            return;

        nametagText.text = ProtonLauncher.Instance.GetLocalUsername();
    }

    public void LookAtMouse(){
        Vector2 mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lookPosition = (Vector3)(mousePosition - new Vector2(transform.position.x, transform.position.y));
        transform.up = lookPosition;
    }

    public void Move(){
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.velocity = input.normalized * moveSpeed;
    }

    public void Fire(GameObject localPlayer){
        if(Input.GetMouseButton(0) && Time.time > nextFire){
            GameObject bullet = ProtonManager.Instance.SpawnObject(entityIdentity.GetPeerID(), "BulletProton", firePoint.position, Quaternion.FromToRotation(lookPosition, Vector3.left));
            bullet.GetComponent<BulletNetworkProton>().SetLocalPlayer(localPlayer);
            AudioUtil.PlayOneShot(audioSource, fireSound[Random.Range(0, fireSound.Length)]);
            nextFire = Time.time + fireRate;
        }
    }

    void Update(){
        if(!entityIdentity.IsMine())
            return;

        LookAtMouse();
        Fire(this.gameObject);
    }

    void FixedUpdate(){
        if(!entityIdentity.IsMine())
            return;

        Move();
    }

    public void AddScore(int val){
        score += val;
        _updateLocalScoreText();
    }

    public void TakeDamage(int val){
        if(!entityIdentity.IsMine())
            return;

        health -= val;
        if(health <= 0){
            health = startHealth;
            transform.position = GetRandomSpawnPoint();
        }
        _updateLocalHealthText();
    }

    private void _updateLocalScoreText() => scoreText.text = "Pontos - " + score;
    private void _updateLocalHealthText() => healthText.text = health + "%";
}
