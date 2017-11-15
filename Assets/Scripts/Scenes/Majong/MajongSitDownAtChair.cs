using Commom.Dto;
using System.Collections;
using System.Collections.Generic;
using Common.Code;
using UnityEngine;
using UnityEngine.EventSystems;

public class MajongSitDownAtChair : MonoBehaviour {

    /// <summary>
    /// Click seat
    /// </summary>
    public void ActSitDowm()
    {
        GameObject curObj = EventSystem.current.currentSelectedGameObject;
        int tableId = int.Parse(curObj.transform.parent.name);
        int seatId = int.Parse(curObj.transform.name.Substring(curObj.transform.name.Length - 1,1));
        //判断是否座位已被占
        if (MajongManager.ins.IsSitted(tableId, seatId))
        {
            return;
        }
        SeatDto seatDto = new SeatDto()
        {
            Id = seatId,
            TableID = tableId,
            PlayName = PhotonManager.Ins.userDetail.Name,
            State = 0,
        };
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[0] = LitJson.JsonMapper.ToJson(seatDto);
        parameters[50] = MajongCode.Sit;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }
}
