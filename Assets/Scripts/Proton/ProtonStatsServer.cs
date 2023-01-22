using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Proton.Stats {
    public class ProtonStatsServer : MonoBehaviour
    {
        public static ProtonStatsServer Instance = null;

        [SerializeField] private bool statsOn;

        private bool _initialized = false;

        private string _urlSendStats = "https://proton-server-stats.squareweb.app/logs";
        private string _serializedStats = "";

        public void SetSerializedStats(string serializedStats){
            _serializedStats = serializedStats;
        }

        public bool IsActived() => statsOn && _initialized;

        public void Initialize() => _initialized = true;

        public void Send(){
            
            if(!IsActived()) 
                return;

            if(string.IsNullOrEmpty(_serializedStats))
                return;

            StartCoroutine(SendStats());
        }

        void Awake(){
            Instance = this;
        }

        IEnumerator SendStats(){
            var request = new UnityWebRequest(_urlSendStats, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(_serializedStats);
            Debug.Log("_serializedStats=" + _serializedStats);
            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            // Debug.Log("[ProtonStatsServer, SendStats()] Status Code: " + request.responseCode);
        }
    }
}
