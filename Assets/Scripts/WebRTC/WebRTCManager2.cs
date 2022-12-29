/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using System;
using TMPro;
using UnityEngine.UI;

public class WebRTCManager2 : MonoBehaviour
{
    [SerializeField] private TMP_InputField offerField;
    [SerializeField] private TMP_InputField answerField;
    [SerializeField] private Button btnCreateOffer;
    [SerializeField] private Button btnCreateAnswer;
    [SerializeField] private Button btnAddAnswer;

    [SerializeField] private TMP_InputField messageField;
    [SerializeField] private TMP_Text messageContent;
    private RTCPeerConnection localConnection;
    private RTCDataChannel sendChannel, receiveChannel;
    private string roomName = "webrtc_rodrigo_tcc";

    private IEnumerator CreateOffer(){
        SetupPeer(RTCSdpType.Offer);

        var offer = localConnection.CreateOffer();
        yield return offer;
        RTCSessionDescription offerDescription = offer.Desc;
        var op2 = localConnection.SetLocalDescription(ref offerDescription);
        yield return op2;
        offerField.text = offerDescription.sdp;
    }

    private IEnumerator CreateAnswer(){
        SetupPeer(RTCSdpType.Answer);

        var offer = new RTCSessionDescription { sdp = offerField.text, type = RTCSdpType.Offer };
        var op3 = localConnection.SetRemoteDescription(ref offer);
        yield return op3;
        var answer = localConnection.CreateAnswer();
        yield return answer;
        RTCSessionDescription answerDescription = answer.Desc;
        var op5 = localConnection.SetLocalDescription(ref answerDescription);
        yield return op5;
        answerField.text = answerDescription.sdp;
    }

    private IEnumerator AddAnswer(){
        var answer = new RTCSessionDescription { sdp = answerField.text, type = RTCSdpType.Answer };
        var op6 = localConnection.SetRemoteDescription(ref answer);
        yield return op6;
    }

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
        messageContent.text = "";

        WebRTC.Initialize();      

        messageField.interactable = false;

        btnCreateOffer.onClick.AddListener(() => {
            StartCoroutine(CreateOffer());
        });

        btnCreateAnswer.onClick.AddListener(() => {
            StartCoroutine(CreateAnswer());
        });

        btnAddAnswer.onClick.AddListener(() => {
            StartCoroutine(AddAnswer());
        });
    }

    private void SetupPeer(RTCSdpType sdpType){
        StartCoroutine(WebRTC.Update());        

        RTCConfiguration rtcConfig = GetRTCConfig();

        localConnection = new RTCPeerConnection(ref rtcConfig);
        localConnection.OnConnectionStateChange = HandleConnectionStateChange;
        localConnection.OnIceCandidate = (RTCIceCandidate candidate) => {
            if(!string.IsNullOrEmpty(candidate.Candidate)){
                if(sdpType == RTCSdpType.Offer){
                    offerField.text = localConnection.LocalDescription.sdp;
                }
                else {
                    answerField.text = localConnection.LocalDescription.sdp;
                }
                Log(candidate.Candidate);
                localConnection.AddIceCandidate(candidate);
            }
        };
        localConnection.OnIceConnectionChange = HandleIceConnectionChange;        
        localConnection.OnDataChannel = HandleDataChannel;

        sendChannel = localConnection.CreateDataChannel(roomName);
        sendChannel.OnOpen = HandleDataChannelOpen;
        sendChannel.OnClose = HandleDataChannelClose;
    }

    void Start(){
    }

    void OnDestroy(){
        sendChannel.Close();
        receiveChannel.Close();
        localConnection.Close();

        WebRTC.Dispose();       
    }

    private void HandleIceCandidateLocal(RTCIceCandidate candidate){
        Log("Candidate Local: " + candidate.Candidate);
        if(!string.IsNullOrEmpty(candidate.Candidate))
            localConnection.AddIceCandidate(candidate);
    }

    private void HandleIceCandidateRemote(RTCIceCandidate candidate){
        Log("Candidate Remote: " + candidate.Candidate);

        if(!string.IsNullOrEmpty(candidate.Candidate))
            localConnection.AddIceCandidate(candidate);
    }

    private void HandleIceConnectionChange(RTCIceConnectionState state){
        Log("IceConnectionChange: " + state.ToString());
    }

    private void HandleDataChannel(RTCDataChannel channel)
    {
        Log("HandleDataChannel: " + channel.Label);

        receiveChannel = channel;
        receiveChannel.OnMessage = HandleDataChannelMessage;
        receiveChannel.OnOpen = HandleDataChannelOpen;
        receiveChannel.OnClose = HandleDataChannelClose;
    }

    private void HandleConnectionStateChange(RTCPeerConnectionState state)
    {
        Log("ConnectionStateChange: " + state.ToString());
    }

    private void HandleDataChannelOpen()
    {
        messageField.interactable = true;
        Log("Data Channel Open");
        messageField.onEndEdit.AddListener((text) => {
            sendChannel.Send(text);
            messageContent.text += $"eu: {text}\n";

            messageField.text = "";
        });
    }

    private void HandleDataChannelClose()
    {
        Log("Data Channel Close");
    }

    private void HandleDataChannelMessage(byte[] data){
        string dataString = System.Text.Encoding.UTF8.GetString(data);
        Log(string.Format("Received message: {0}", dataString));
        messageContent.text += dataString + "\n";
    }

    private void Log(string text){
        messageContent.text += $"[DEBUG] {text}\n";
    }

}*/
