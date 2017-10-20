using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Assets.Scripts.Logic;
using Commom.Dto;
using Common.Code;
using UnityEngine.SceneManagement;


public class PhotonManager : MonoBehaviour,IPhotonPeerListener
{
    public static PhotonManager Ins;
    private PhotonPeer peer;
    private bool isConnected = false;

    private AccountReceive accountReciever = new AccountReceive();
    private DetailReceive detailReceive = new DetailReceive();
    private MajongReceive majongReceive = new MajongReceive();
    #region 全局变量
    public DetailDto userDetail = new DetailDto();
    public Sprite HeadSculptureSprite;
    #endregion
    void Awake()
    {
        Ins = this;
        peer = new PhotonPeer(this, ConnectionProtocol.Udp);
    }
	void Start ()
	{
        DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
	    if (!isConnected)
            peer.Connect("127.0.0.1:5155", "Game2DouDiZhu");
        peer.Service();
    }

    void OnDestroy()
    {
        peer.Disconnect();
    }
    public void DebugReturn(DebugLevel level, string message)
    {
        
    }

    public void OnEvent(EventData eventData)
    {
       
    }
    /// <summary>
    /// 向服务器发送请求
    /// </summary>
    /// <param name="opCode"></param>
    /// <param name="opParameters"></param>
    /// <param name="subCode"></param>
    public void OnOperationRequest(byte opCode,Dictionary<byte,object> opParameters)
    {
        peer.OpCustom(opCode, opParameters, true);
    }
    /// <summary>
    /// 获取服务器的响应。
    /// </summary>
    /// <param name="operationResponse"></param>
    public void OnOperationResponse(OperationResponse operationResponse)
    {
        OpCode opCode = (OpCode)operationResponse.OperationCode;
        switch (opCode)
        {
            case OpCode.Account:
                accountReciever.OnDoResponse(operationResponse);
                break;
            case OpCode.Detail:
                detailReceive.OnDoResponse(operationResponse);
                break;
            case OpCode.Majong:
                majongReceive.OnDoResponse(operationResponse);
                break;
        }
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        switch (statusCode)
        {
            case StatusCode.Connect:
                isConnected = true;
                print(StatusCode.Connect);
                break;
            case StatusCode.Disconnect:
                isConnected = false;
                break;
        }
    }
}
