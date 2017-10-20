using Common.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectGame : MonoBehaviour {


    public void OnPlayMajongClick()
    {
        //通知服务器
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = MajongCode.Enter;
        parameters[0] = LitJson.JsonMapper.ToJson(PhotonManager.Ins.userDetail);
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }
}
