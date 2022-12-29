using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proton.Sync { 
    public enum SyncTransformType {
        Position,
        Rotation,
        Scale,
        All
    }
    public class SyncTransform : MonoBehaviour
    {
        public SyncTransformType Type;
        public float Delay;

        private EntityIdentity _identity;
        private SendDataPosition _sendDataPosition;

        private SendData _sendDataManager;

        void Awake(){
            _identity = GetComponent<EntityIdentity>();
            _sendDataManager = new SendData(Delay);
        }

        void Start()
        {
            switch(Type)
            {
                case SyncTransformType.Position:
                    _sendDataPosition = new SendDataPosition(_identity.GetPeerID());
                break;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(!_identity.IsMine() || string.IsNullOrEmpty(_identity.GetPeerID()))
                return;

            switch(Type){
                case SyncTransformType.Position:
                    _sendDataManager.Update(() => {
                        _sendDataPosition.Add(transform.position);
                        _sendDataManager.Setup(SendDataType.Position, _sendDataPosition);            
                    });
                break;
            }
        }
    }
}