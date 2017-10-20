using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Common.Code;
using UnityEngine.UI;

public class SingleMajongCard : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler {

    private Vector3 oldPos;

    public void OnPointerEnter(PointerEventData eventData)
    {
        oldPos = transform.position;
        transform.position = oldPos + new Vector3(0, 20, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.position = oldPos;
    }

    public void OnPointerClick(PointerEventData eventData)
    { 

        if (!MajongManager.ins.CanDiscard())
            return;

        int tileId = int.Parse(transform.Find("Tile").GetComponent<Image>().sprite.name);
        Debug.Log(tileId);
        Dictionary<byte, object> paramerters = new Dictionary<byte, object>();
        paramerters[50] = MajongCode.Discard;
        paramerters[0] = tileId;
        PhotonManager.Ins.OnOperationRequest((byte) OpCode.Majong, paramerters);
    }
}
