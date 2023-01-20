using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;

    public override void OnEnable()
    {
        LocalPlayer = PhotonNetwork.Instantiate(playerPrefab.name, GetRandomSpawnPoint(), Quaternion.identity);
    }

    public static Vector3 GetRandomSpawnPoint() => new Vector3(Random.Range(-11.4f, 11.4f), Random.Range(-7.13f, 8f), 0);
    public static GameObject LocalPlayer = null;
}
