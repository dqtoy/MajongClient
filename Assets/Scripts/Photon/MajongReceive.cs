using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Common.Code;
using Commom.Dto;
using UnityEngine.UI;

public class MajongReceive : IReceiveHandler
{
    public void OnDoResponse(OperationResponse response)
    {
        MajongCode subCode = (MajongCode)response.Parameters[50];
        switch (subCode)
        {
            case MajongCode.Enter:
                enter(response);
                break;
            case MajongCode.Sit:
                sit(response);
                break;
            case MajongCode.Ready:
                ready(response);
                break;
            case MajongCode.Stand:
                stand(response);
                break;
            case MajongCode.Play:
                break;
            case MajongCode.Leave:
                leave(response);
                break;
            case MajongCode.Start:
                start(response);
                break;
            case MajongCode.Getcard:
                getCard(response);
                break;
            case MajongCode.Discard:
                disCard(response);
                break;
            case MajongCode.Operate:
                operate(response);
                break;
            case MajongCode.End:
                end(response);
                break;
        }
    }

    void enter(OperationResponse response)
    {
        MajongManager.ins.Excute4Enter(response); 
    }

    void sit(OperationResponse response)
    {
        MajongManager.ins.Excute4Sit(response);

    }

    void ready(OperationResponse response)
    {
        MajongManager.ins.Excute4Ready(response);
    }

    void stand(OperationResponse response)
    {
        MajongManager.ins.Excute4Stand(response);
    }

    void leave(OperationResponse response)
    {
        DetailDto detailDto = LitJson.JsonMapper.ToObject<DetailDto>(response.Parameters[0].ToString());
        Scene02Manager.Ins.MyRoomDto.NameDetailDict.Remove(detailDto.Name);
        if (response.ReturnCode == 0)
        {
            Scene02Manager.Ins.BackToLobby();
        }
    }

    void start(OperationResponse response)
    {
        MajongManager.ins.Excute4GameStart(response);
    }

    void getCard(OperationResponse response)
    {
        MajongManager.ins.Excute4GetCard(response);
    }
    void disCard(OperationResponse response)
    {
        MajongManager.ins.Excute4DisCard(response);
    }
    void operate(OperationResponse response)
    {

    }
    void end(OperationResponse response)
    {

    }
}
