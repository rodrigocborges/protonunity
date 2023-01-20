using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proton.Sync {
    public class SyncText : MonoBehaviour
    {
        [SerializeField] private EntityIdentity entityIdentity;
        [SerializeField] private string dataKey;
        private TMPro.TMP_Text textObject;
        private SendData _sendData;
        private SendDataGeneric _sendDataGeneric;
        
        void Awake(){
            textObject = GetComponent<TMPro.TMP_Text>();
        }

        void Start()
        {
            gameObject.name = string.Format("Text_{0}_{1}", entityIdentity.GetPeerID(), dataKey);
            _sendData = new SendData(0.05f); //0.75f ok, 0f ok
            _sendDataGeneric = new SendDataGeneric(entityIdentity.GetPeerID());
        }

        void Update(){
            if(!entityIdentity.IsMine() || string.IsNullOrEmpty(entityIdentity.GetPeerID()))
                return;

            _sendData.Update(() => {
                _sendDataGeneric.Add(dataKey, textObject.text);
                _sendData.Setup(SendDataType.GenericData, _sendDataGeneric);
            });
        }
    }

}
