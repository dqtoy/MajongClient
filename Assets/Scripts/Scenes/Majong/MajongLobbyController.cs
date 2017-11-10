using System.Collections;
using System.Collections.Generic;
using Common.Code;
using UnityEngine;
using UnityEngine.UI;

public class MajongLobbyController : MonoBehaviour
{

    private Button toSelectPanelBtn;

	// Use this for initialization
	void Start ()
	{
	    toSelectPanelBtn = transform.Find("Panel/ToSelectPanelBtn").GetComponent<Button>();
	    toSelectPanelBtn.onClick.AddListener(onClicktoSelectPanelBtn);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void onClicktoSelectPanelBtn()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = MajongCode.Leave;
        PhotonManager.Ins.OnOperationRequest((byte) OpCode.Majong, parameters);
    }
}
