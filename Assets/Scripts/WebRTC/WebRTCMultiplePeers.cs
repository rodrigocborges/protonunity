/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using TMPro;

public class WebRTCMultiplePeers : MonoBehaviour
{
    [SerializeField] private TMP_InputField messageField;
    [SerializeField] private TMP_Text messageContent;

    private RTCPeerConnection pc1Local, pc1Remote, pc2Local, pc2Remote;
    private string roomName = "webrtc_rodrigo_tcc";
    private RTCDataChannel sendChannel, sendChannel2, receiveChannel;

    private RTCConfiguration GetRTCConfig()
    {
        RTCConfiguration config = default;
        config.iceServers = new RTCIceServer[]
        {
            new RTCIceServer { urls = new string[] { "stun:stun.l.google.com:19302" } }
        };

        return config;
    }

    void Awake(){
        WebRTC.Initialize();    
        messageField.interactable = false;
    }

    void Start()
    {
        StartCoroutine(WebRTC.Update());        

        RTCConfiguration rtcConfig = GetRTCConfig();

        pc1Local = new RTCPeerConnection(ref rtcConfig);
        pc1Remote = new RTCPeerConnection(ref rtcConfig);
        
        pc1Local.OnIceCandidate = (RTCIceCandidate candidate) => {
            HandleCandidate(candidate, pc1Remote);
        };
        pc1Remote.OnIceCandidate = (RTCIceCandidate candidate) => {
            HandleCandidate(candidate, pc1Local);
        };
        pc1Remote.OnDataChannel = HandleDataChannel;

        pc2Local = new RTCPeerConnection(ref rtcConfig);
        pc2Remote = new RTCPeerConnection(ref rtcConfig);

        pc2Local.OnIceCandidate = (RTCIceCandidate candidate) => {
            HandleCandidate(candidate, pc2Remote);
        };
        pc2Remote.OnIceCandidate = (RTCIceCandidate candidate) => {
            HandleCandidate(candidate, pc2Local);
        };
        pc2Remote.OnDataChannel = HandleDataChannel;

        sendChannel = pc2Local.CreateDataChannel(roomName);
        sendChannel.OnOpen = HandleDataChannelOpen;
        sendChannel.OnClose = () => { Debug.Log("Canal pc2Local fechado!"); };

        StartCoroutine(NegotiationPeer(pc1Local, pc1Remote));
        StartCoroutine(NegotiationPeer(pc2Local, pc2Remote));
    }

    IEnumerator NegotiationPeer(RTCPeerConnection localPeer, RTCPeerConnection remotePeer)
    {
        var opCreateOffer = localPeer.CreateOffer();
        yield return opCreateOffer;

        if (opCreateOffer.IsError)
        {
            OnCreateSessionDescriptionError(opCreateOffer.Error);
            yield break;
        }

        var offerDesc = opCreateOffer.Desc;
        yield return localPeer.SetLocalDescription(ref offerDesc);
        Debug.Log($"Oferta do peer local \n {offerDesc.sdp}");
        yield return remotePeer.SetRemoteDescription(ref offerDesc);

        var opCreateAnswer = remotePeer.CreateAnswer();
        yield return opCreateAnswer;

        if (opCreateAnswer.IsError)
        {
            OnCreateSessionDescriptionError(opCreateAnswer.Error);
            yield break;
        }

        var answerDesc = opCreateAnswer.Desc;
        yield return remotePeer.SetLocalDescription(ref answerDesc);
        Debug.Log($"Resposta do peer remoto \n {answerDesc.sdp}");
        yield return localPeer.SetRemoteDescription(ref answerDesc);
    }

    private void HandleCandidate(RTCIceCandidate candidate, RTCPeerConnection connectionDestiny)
    {
        connectionDestiny.AddIceCandidate(candidate);
    }

    private void HandleDataChannel(RTCDataChannel channel){
        receiveChannel = channel;
        receiveChannel.OnMessage = HandleDataChannelMessage;
        receiveChannel.OnOpen = HandleDataChannelOpen;
        receiveChannel.OnClose = () => { Debug.Log("Canal de recebimento fechado!"); };
    }

    private void HandleDataChannelOpen(){
        messageField.interactable = true;

        messageField.onEndEdit.AddListener((text) => {
            sendChannel.Send(text);
            messageContent.text += $"eu: {text}\n";
            print("send");
            messageField.text = "";
        });
    }

     private void HandleDataChannelMessage(byte[] data){
        string dataString = System.Text.Encoding.UTF8.GetString(data);
        messageContent.text += dataString + "\n";
    }

    private void OnCreateSessionDescriptionError(RTCError error)
    {
        Debug.LogError($"Erro ao criar a descrição da seção: {error.message}");
    }

    void Update()
    {
        
    }
    
    void OnDestroy(){
        sendChannel.Close();
        receiveChannel.Close();
        
        pc1Local.Close();
        pc1Remote.Close();
        pc2Local.Close();
        pc2Remote.Close();
        
        pc1Local.Dispose();
        pc1Remote.Dispose();
        pc2Local.Dispose();
        pc2Remote.Dispose();

        pc1Local = null;
        pc1Remote = null;
        pc2Local = null;
        pc2Remote = null;

        WebRTC.Dispose();       
    }
}
*/