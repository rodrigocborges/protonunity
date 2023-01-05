using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proton.Sync {
    public class SyncText : MonoBehaviour
    {
        [SerializeField] private EntityIdentity entityIdentity;
        [SerializeField] private string dataKey;
        private TMPro.TMP_Text textObject;
        private string _currentText;

        private string onlyTestText = "";

        private SendData _sendData;
        private SendDataGeneric _sendDataGeneric;

        void Awake()
        {
            textObject = GetComponent<TMPro.TMP_Text>();
            gameObject.name = string.Format("Text_{0}_{1}", entityIdentity.GetPeerID(), dataKey);

            onlyTestText = "User_" + Random.Range(1000, 9999);
        }

        void Start(){            
            _sendData = new SendData(2f);
            _sendDataGeneric = new SendDataGeneric(entityIdentity.GetPeerID());    
        }

        void Update(){
            _setText(ProtonManager.Instance.GenericDataManager.Get(entityIdentity.GetPeerID(), dataKey)?.ToString());            
            
            if(!entityIdentity.IsMine() || string.IsNullOrEmpty(entityIdentity.GetPeerID()))
                return;

            if(ProtonManager.Instance.GenericDataManager == null)
                return;    
                
            _sendData.Update(() => {
                _sendDataGeneric.Add(dataKey, onlyTestText);
                _sendData.Setup(SendDataType.GenericData, _sendDataGeneric);
            });
        }

        private void _setText(string text){
            // Debug.Log("Setting text");
            if(string.IsNullOrEmpty(text))
            {
                textObject.text = onlyTestText;
                return;
            }
            if(_currentText.Equals(text))
                return;

            _currentText = text;
            textObject.text = _currentText;
        }
    }

}
