using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Proton {
    public class ProtonLauncher : MonoBehaviour
    {
        [SerializeField] private string gameplaySceneName;
        [SerializeField] private UnityEngine.UI.InputField usernameField;
        public static ProtonLauncher Instance;

        private string _localUsername;

        public string GetLocalUsername() => _localUsername;
        
        public void Join(){
            if(string.IsNullOrEmpty(usernameField.text))
                SetRandomUsername();

            _localUsername = usernameField.text;
            
            if(string.IsNullOrEmpty(gameplaySceneName)){
                Debug.LogError("[ProtonLauncher, Join()]: Invalid gameplay scene name!");
                return;
            }

            SceneManager.LoadSceneAsync(gameplaySceneName);
        }        

        void Awake(){
            Instance = this;

            SetRandomUsername();
            
            DontDestroyOnLoad(this);
        }

        void SetRandomUsername(){
            usernameField.text = "Usuario_" + Random.Range(1000, 9999);
        }
        
    }
}