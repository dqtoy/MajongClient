using Common.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MajongUIEvent : MonoBehaviour {

    public void OnToMajongLobbyBtnClick()
    {
        if (MajongManager.ins.IsMajongPlaying())
        {
            
        }
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = MajongCode.Stand;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }

    public void OnReadyBtnClick()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = MajongCode.Ready;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }
}
