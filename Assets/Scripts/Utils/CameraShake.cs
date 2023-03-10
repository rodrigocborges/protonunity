using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    private Vector3 startCameraPos;

    // Shake Parameters
    [SerializeField] private float shakeDuration = 2f;
    [SerializeField] private float shakeAmount = 0.7f;

    private bool canShake = false;
    private float _shakeTimer;
 
    void Start()
    {
        if(cameraTransform == null)
            cameraTransform = Camera.main.transform;

        startCameraPos = cameraTransform.localPosition;

    }

    void Update()
    {   
        #if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Space))
            ShakeCamera();
        #endif
        
        if (canShake)
        {
            StartCameraShakeEffect();
        }
    }

    public void ShakeCamera()
    {
        canShake = true;
        _shakeTimer = shakeDuration;
    }

    public void StartCameraShakeEffect()
    {
        if (_shakeTimer > 0)
        {
            cameraTransform.localPosition = startCameraPos + Random.insideUnitSphere * shakeAmount;
            _shakeTimer -= Time.deltaTime;
        }
        else
        {
            _shakeTimer = 0f;
            cameraTransform.position = startCameraPos;
            canShake = false;
        }
    }

}