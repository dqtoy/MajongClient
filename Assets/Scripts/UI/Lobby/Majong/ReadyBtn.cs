using Common.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyBtn : MonoBehaviour {

	public void OnReadyBtnClick()
	{
	    gameObject.transform.localScale = Vector3.zero;

        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = MajongCode.Ready;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }
}
