using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public string SerializeData() => string.Format("{0}:{1};{2};{3};{4};{5};{6};{7};{8}", _peerID, _prefabPath, _position.x, _position.y, _position.z, _rotation.x, _rotation.y, _rotation.z, _rotation.w);
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

    public class SendDataElement {
        public SendDataType DataType { get; set; }
        public string SerializedData { get; set; }
        public int AmountTimes { get; set; } = 0;
        public int AmountTimesLimit { get; set; }

        public bool IsAvailable() => AmountTimes <= AmountTimesLimit;

        public void ClearBuffer() { 
            Debug.Log("Clearing buffer of " + DataType.ToString());
            AmountTimes = 0;
        }
    }

    public class SendData
    {
        private SendDataType _dataType;
        private string _serializedData;

        private byte[] _dataBuffer;

        private List<string> _allData = new List<string>();
        private Dictionary<SendDataType, SendDataElement> _sendBuffer = new Dictionary<SendDataType, SendDataElement>();

        private float _timeToSendData;
        private float _currentTimeToSendData = 0;

        public SendData(float delayData = 1) => _timeToSendData = delayData;

        public void Setup(SendDataType dataType, ISendData data, bool automaticSend = true){
            _dataType = dataType;
            _serializedData = dataType.ToString() + "|" + data.SerializeData();

            //TODO: Apenas teste
            // _dataBuffer = System.Text.Encoding.ASCII.GetBytes(_serializedData);
            
            /*SendDataElement sendDataElement = new SendDataElement();
            sendDataElement.DataType = dataType;
            sendDataElement.SerializedData = _serializedData;

            switch(dataType){
                case SendDataType.GenericData: 
                    sendDataElement.AmountTimesLimit = 5;
                break;
                case SendDataType.Instantiate:
                    sendDataElement.AmountTimesLimit = 1;
                break;
                default: 
                    sendDataElement.AmountTimesLimit = 10;
                break;
            }*/

            if(automaticSend)
                Send();
        }
        
        /*public bool BufferAvailabled(SendDataElement element){
            if(_sendBuffer.ContainsKey(element.DataType)){
                _sendBuffer[element.DataType].AmountTimes++;
                //NAO FUNCIONOU (O TIMER PELO MENOS)
                if(!_sendBuffer[element.DataType].IsAvailable()) {
                    // System.Threading.Tasks.Task.Delay(new System.TimeSpan(0, 0, 10)).ContinueWith(o => {  });
                    _sendBuffer[element.DataType].ClearBuffer();
                    return false;
                }

                return true;
            }

            _sendBuffer.Add(element.DataType, element);
            return true;
        }*/

        public void Send(){
            //FIXME: Os dados são enviados antes de conectar com um player... por isso a regra abaixo nao funciona direito porque já vai estar o dado na lista e nao foi enviado pro player
            
            if(_dataType == SendDataType.GenericData) { 
                ProtonManager.Instance.SendToAll(_serializedData);
                return;
            }
            // Debug.Log(_serializedData);
            if(!_containsData()){
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

        private bool _containsData(){
            if(string.IsNullOrEmpty(_serializedData))
                return false;

            return _allData.FirstOrDefault(t => t.Equals(_serializedData)) != null ? true : false;
        }
        
    }

}