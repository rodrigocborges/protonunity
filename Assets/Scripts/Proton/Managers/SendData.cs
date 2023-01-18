using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proton {

    public enum SendDataType { 
        Position,
        Scale,
        Rotation,
        Instantiate,
        GenericData
    }

    public interface ISendData {
        public string SerializeData();
    }

    [System.Obsolete]
    public class SendDataPosition : ISendData {

        private string _peerID = null;
        private float _x;
        private float _y;
        private float _z;

        public SendDataPosition(string peerID) => _peerID = peerID;

        public void Add(float x, float y, float z){
            _x = x;
            _y = y;
            _z = z;
        }

        public void Add(Vector3 position){
            Add(position.x, position.y, position.z);
        }

        public string SerializeData() => string.Format("{0}:{1};{2};{3}", _peerID, _x, _y, _z);
    }

    [System.Obsolete]
    public class SendDataScale : ISendData {

        private string _peerID = null;
        private float _x;
        private float _y;
        private float _z;

        public SendDataScale(string peerID) => _peerID = peerID;

        public void Add(float x, float y, float z){
            _x = x;
            _y = y;
            _z = z;
        }

        public void Add(Vector3 position){
            Add(position.x, position.y, position.z);
        }

        public string SerializeData() => string.Format("{0}:{1};{2};{3}", _peerID, _x, _y, _z);
    }

    [System.Obsolete]
    public class SendDataRotation : ISendData
    {
        private string _peerID = null;
        private float _x;
        private float _y;
        private float _z;

        public SendDataRotation(string peerID) => _peerID = peerID;

        public void Add(float x, float y, float z){
            _x = x;
            _y = y;
            _z = z;
        }

        public void Add(Quaternion rotation){
            Add(rotation.x, rotation.y, rotation.z);
        }

        public string SerializeData() => string.Format("{0}:{1};{2};{3}", _peerID, _x, _y, _z);
    }

    public class SendDataInstantiate : ISendData {

        private string _peerID = null;
        private Vector3 _position;
        private Quaternion _rotation;
        private string _prefabPath;

        public SendDataInstantiate(string peerID) => _peerID = peerID;

        public void Add(string prefabPath, Vector3 pos, Quaternion rot){
            _prefabPath = prefabPath;
            _position = pos;
            _rotation = rot;
        }

        public string SerializeData() => string.Format("{0}:{1};{2};{3};{4};{5};{6};{7}", _peerID, _prefabPath, _position.x, _position.y, _position.z, _rotation.x, _rotation.y, _rotation.z);
    }

    public class SendDataGeneric : ISendData {

        private string _peerID = null;
        private string _key;
        private object _data;

        public SendDataGeneric(string peerID) => _peerID = peerID;

        public void Add(string key, object data){
            _key = key;
            _data = data;    
        }

        public string SerializeData() => string.Format("{0}:{1};{2}", _peerID, _key, _data);
    }

    public class SendDataVector : ISendData {

        private string _peerID = null;
        private float _x;
        private float _y;
        private float _z;

        public SendDataVector(string peerID) => _peerID = peerID;

        public void Add(float x, float y, float z){
            _x = x;
            _y = y;
            _z = z;
        }

        public void Add(float x, float y) => Add(x, y, 0);

        public void Add(Vector3 vector) => Add(vector.x, vector.y, vector.z);
        public void Add(Vector2 vector) => Add(vector.x, vector.y);

        public string SerializeData() => string.Format("{0}:{1};{2};{3}", _peerID, _x, _y, _z);
    }


    public class SendData
    {
        private SendDataType _dataType;
        private ISendData _data;

        private string _serializedData;

        private List<string> _allData = new List<string>();

        private float _timeToSendData;
        private float _currentTimeToSendData = 0;

        public SendData(float delayData = 1) => _timeToSendData = delayData;

        public void Setup(SendDataType dataType, ISendData data, bool automaticSend = true){
            _dataType = dataType;
            _serializedData = dataType.ToString() + "|" + data.SerializeData();     
            if(automaticSend)
                Send();
        }

        public void Send(){
            if(_dataType == SendDataType.GenericData)
            {
                ProtonManager.Instance.SendToAll(_serializedData);
                return;
            }

            if(!_allData.Contains(_serializedData) && _dataType != SendDataType.GenericData){
                _allData.Add(_serializedData);
                ProtonManager.Instance.SendToAll(_serializedData);
                return;
            }

            if(_allData.Count > 20)
                _allData.Clear();
        }   

        public void Update(System.Action tick){
            _currentTimeToSendData += Time.deltaTime;

            if(_currentTimeToSendData >= _timeToSendData){
                tick();
                _currentTimeToSendData = 0;
            }
        } 
        
    }

}