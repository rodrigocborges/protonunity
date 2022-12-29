using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proton {
    public class ReceiveData
    {
        private ProtonManager _connectionManager;

        public ReceiveData (ProtonManager connectionManager) {
            _connectionManager = connectionManager;
        }

        public void Handle(string data){
            if(_connectionManager == null) return;

            if(data.Contains('|') && data.Contains(':') && data.Contains(';')){ //Ex: Position|0939d-80fds089f-dfs90fsdf-12921:1;2;3
                string[] allData = data.Split('|');
                SendDataType dataType = (SendDataType) System.Enum.Parse(typeof(SendDataType), allData[0], true);
                string[] dataArray = allData[1].Split(':');
                
                string peerID = dataArray[0];
                string[] infoRaw = dataArray[1].Split(';');

                switch(dataType){
                    case SendDataType.Position:                    
                        Vector3 targetPosition = new Vector3(float.Parse(infoRaw[0]), float.Parse(infoRaw[1]), float.Parse(infoRaw[2]));
                        _connectionManager.playersGameObjects[peerID].transform.position = targetPosition;
                    break;
                    case SendDataType.Instantiate:
                        string prefabPath = infoRaw[0];
                        Vector3 position = new Vector3(float.Parse(infoRaw[1]), float.Parse(infoRaw[2]), float.Parse(infoRaw[3]));
                        Quaternion rotation = Quaternion.Euler(float.Parse(infoRaw[4]), float.Parse(infoRaw[5]), float.Parse(infoRaw[6]));
                        
                        GameObject spawnedObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>(prefabPath), position, rotation);
                        spawnedObject.name = string.Format("GameObject_{0}", peerID);
                    break;
                }
                 
            }
        }
    }
}


