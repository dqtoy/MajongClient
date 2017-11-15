using Common.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MajongUIEvent : MonoBehaviour {

    /// <summary>
    /// 返回麻将大厅
    /// </summary>
    public void OnToMajongLobbyBtnClick()
    {
        if (MajongManager.ins.IsMajongPlaying())
        {
            return;
        }
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = MajongCode.Stand;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }
    /// <summary>
    /// 游戏准备按钮
    /// </summary>
    public void OnReadyBtnClick()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = MajongCode.Ready;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }
}
