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
        private SendDataScale _sendDataScale;
        private SendDataRotation _sendDataRotation;
        private SendData _sendDataManager;
        private SendData _sendDataManager2;
        private SendData _sendDataManager3;

        void Awake(){
            _identity = GetComponent<EntityIdentity>();
            _sendDataManager = new SendData(Delay);
            _sendDataManager2 = new SendData(Delay);
            _sendDataManager3 = new SendData(Delay);
        }

        void Start()
        {
            switch(Type)
            {
                case SyncTransformType.Position:
                    _sendDataPosition = new SendDataPosition(_identity.GetPeerID());
                break;
                case SyncTransformType.Rotation:
                    _sendDataRotation = new SendDataRotation(_identity.GetPeerID());
                break;
                case SyncTransformType.Scale:
                    _sendDataScale = new SendDataScale(_identity.GetPeerID());
                break;
                case SyncTransformType.All:
                    _sendDataPosition = new SendDataPosition(_identity.GetPeerID());
                    _sendDataRotation = new SendDataRotation(_identity.GetPeerID());
                    _sendDataScale = new SendDataScale(_identity.GetPeerID());
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
                case SyncTransformType.Rotation:
                    _sendDataManager.Update(() => {
                        _sendDataRotation.Add(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                        _sendDataManager.Setup(SendDataType.Rotation, _sendDataRotation);            
                    });
                break;
                case SyncTransformType.Scale:
                    _sendDataManager.Update(() => {
                        _sendDataScale.Add(transform.localScale);
                        _sendDataManager.Setup(SendDataType.Scale, _sendDataScale);            
                    });
                break;
                case SyncTransformType.All:
                    _sendDataManager.Update(() => {
                        _sendDataPosition.Add(transform.position);
                        _sendDataManager.Setup(SendDataType.Position, _sendDataPosition);
                    });

                    _sendDataManager2.Update(() => {
                        _sendDataRotation.Add(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                        _sendDataManager.Setup(SendDataType.Rotation, _sendDataRotation);
                    });

                    _sendDataManager3.Update(() => {
                        _sendDataScale.Add(transform.localScale);
                        _sendDataManager.Setup(SendDataType.Scale, _sendDataScale);
                    });
                break;
            }
        }
    }
}