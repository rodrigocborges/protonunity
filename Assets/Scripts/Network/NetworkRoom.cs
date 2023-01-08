using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;

    void Start()
    {
        LocalPlayer = PhotonNetwork.Instantiate(playerPrefab.name, GetRandomSpawnPoint(), Quaternion.identity);
    }

    public static Vector3 GetRandomSpawnPoint() => new Vector3(Random.Range(-12, 12), Random.Range(-9, 9), 0);
    public static GameObject LocalPlayer = null;
}
