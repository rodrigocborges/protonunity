using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Proton {

    public class ReceiveData
    {
        public class ReceivedDataElement {
            public string PeerID { get; set; }
            public SendDataType DataType { get; set; }
            public string[] Data { get; set; }

            public Vector3 GetVector3() {
                if(DataType != SendDataType.Position && DataType != SendDataType.Rotation && DataType != SendDataType.Scale)
                    return Vector3.zero;

                return new Vector3(float.Parse(Data[0]), float.Parse(Data[1]), float.Parse(Data[2]));
            }

            public string GetInstantiatePrefabPath() {
                if(DataType != SendDataType.Instantiate)
                    return null;

                return Data[0];
            }

            public Vector3 GetInstantiatePosition() {
                if(DataType != SendDataType.Instantiate)
                    return Vector3.zero;

                return new Vector3(float.Parse(Data[1]), float.Parse(Data[2]), float.Parse(Data[3]));
            }

            public Quaternion GetInstantiateRotation() {
                if(DataType != SendDataType.Instantiate)
                    return Quaternion.identity;

                return Quaternion.Euler(float.Parse(Data[4]), float.Parse(Data[5]), float.Parse(Data[6]));
            }

            public string GetGenericDataKey() {
                if(DataType != SendDataType.GenericData)
                    return null;
                return Data[0];
            }

            public string GetGenericDataValue() {
                if(DataType != SendDataType.GenericData)
                    return null;
                return Data[1];
            }
        }

        private ProtonManager _connectionManager;

        public ReceiveData (ProtonManager connectionManager ) {
            _connectionManager = connectionManager;
        }  

        public ReceivedDataElement ConvertRawData(string data){
            string[] allData = data.Split('|');
            SendDataType dataType = (SendDataType) System.Enum.Parse(typeof(SendDataType), allData[0], true);
            string[] dataArray = allData[1].Split(':');
            
            string peerID = dataArray[0];
            string[] infoRaw = dataArray[1].Split(';');

            return new ReceivedDataElement {
                PeerID = peerID,
                DataType = dataType,
                Data = infoRaw
            };
        }      

        public void Handle(string data){
            if(_connectionManager == null) return;
            
            if(string.IsNullOrEmpty(data) || (!data.Contains('|') && !data.Contains(':') && !data.Contains(';'))) return;
            //Exemplo de dado correto vindo: Position|0939d-80fds089f-dfs90fsdf-12921:1;2;3
            ReceivedDataElement receivedDataElement = ConvertRawData(data);

            if(receivedDataElement.DataType == SendDataType.Position)
                _connectionManager.playersGameObjects[receivedDataElement.PeerID].transform.position = receivedDataElement.GetVector3();
            else if(receivedDataElement.DataType == SendDataType.Rotation)
                _connectionManager.playersGameObjects[receivedDataElement.PeerID].transform.eulerAngles = receivedDataElement.GetVector3();
            else if(receivedDataElement.DataType == SendDataType.Scale)
                _connectionManager.playersGameObjects[receivedDataElement.PeerID].transform.localScale = receivedDataElement.GetVector3();
            else if(receivedDataElement.DataType == SendDataType.Instantiate) 
            {
                //TODO: Adicionar num dicionário dentro do ProtonManager para futura manipulações
                GameObject spawnedObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>(receivedDataElement.GetInstantiatePrefabPath()), receivedDataElement.GetInstantiatePosition(), receivedDataElement.GetInstantiateRotation());
                spawnedObject.name = string.Format("GameObject_{0}", receivedDataElement.PeerID);
            }
            else if(receivedDataElement.DataType == SendDataType.GenericData)
            {
                _connectionManager.FindAndSetText(receivedDataElement.PeerID, receivedDataElement.GetGenericDataKey(), receivedDataElement.GetGenericDataValue());     
            }           
        }

    }
}


