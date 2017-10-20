using System.Collections;
using System.Collections.Generic;
using Commom.Dto;
using Common.Code;
using UnityEngine;
using UnityEngine.EventSystems;

public class SeatClick : MonoBehaviour {

    public void OnSeatClick()
    {
    
        //判断座位上是否有人
        int id = transform.name[transform.name.Length - 1] - '0';
 

 
//        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
//        parameters[50] = MajongCode.Sit;
//        parameters[0] = LitJson.JsonMapper.ToJson(seatDto);
//        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }
}
