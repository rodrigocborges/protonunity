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
        private SendData _sendDataManager;
        private SendDataVector _sendDataVector;
        private List<Vector3> _positions = new List<Vector3>();
        private List<Vector3> _rotations = new List<Vector3>();
        private List<Vector3> _scales = new List<Vector3>();

        private const int MAX_CAPACITY_LISTS = 100;

        private Vector3 _lastScale = Vector3.zero;

        //FIXME: Caso haja já um valor nessa lista simplesmente não envia, só após dar clear (problema notável na escala)

        void ClearBufferSyncLists(){
            if(_scales.Count >= 2)
                _scales.Clear();
            
            if(_positions.Count >= MAX_CAPACITY_LISTS)
                _positions.Clear();

            if(_rotations.Count >= MAX_CAPACITY_LISTS)
                _rotations.Clear();
        }

        void Awake(){
            _identity = GetComponent<EntityIdentity>();
            _sendDataManager = new SendData(Delay);

            _lastScale = transform.localScale;
        }

        void Start()
        {
            _sendDataVector = new SendDataVector(_identity.GetPeerID());
            InvokeRepeating("ClearBufferSyncLists", 0, 60); //TODO: Conferir esse tempo
        }

        void Update()
        {
            if(!_identity.IsMine() || string.IsNullOrEmpty(_identity.GetPeerID()))
                return;

            switch(Type){
                case SyncTransformType.Position:
                    _syncPosition();
                    break;
                case SyncTransformType.Rotation:
                    _syncRotation();
                    break;
                case SyncTransformType.Scale:
                    _syncScale();
                    break;
                case SyncTransformType.All:
                    _syncPosition();
                    _syncRotation();
                    _syncScale();
                break;
            }
        }

        private void _syncScale()
        {
            _sendDataManager.Update(() =>
            {
                /*if(!_scales.Contains(transform.localScale))
                {
                    _scales.Add(transform.localScale);
                    // _sendDataScale.Add(transform.localScale);
                    _sendDataVector.Add(transform.localScale);
                    _sendDataManager.Setup(SendDataType.Scale, _sendDataVector);
                }*/

                if(!MathUtil.Vector3Equal(_lastScale, transform.localScale)){
                    _lastScale = transform.localScale;
                    _sendDataVector.Add(_lastScale);
                    _sendDataManager.Setup(SendDataType.Scale, _sendDataVector);
                }
            });
        }

        private void _syncRotation()
        {
            _sendDataManager.Update(() =>
            {
                if(!_rotations.Contains(transform.eulerAngles)){
                    _rotations.Add(transform.eulerAngles);
                    _sendDataVector.Add(transform.eulerAngles);
                    _sendDataManager.Setup(SendDataType.Rotation, _sendDataVector);
                }
            });
        }

        private void _syncPosition()
        {
            _sendDataManager.Update(() =>
            {
                if(!_positions.Contains(transform.position)){
                    _positions.Add(transform.position);
                    _sendDataVector.Add(transform.position);
                    _sendDataManager.Setup(SendDataType.Position, _sendDataVector);
                }
            });
        }
    }
}