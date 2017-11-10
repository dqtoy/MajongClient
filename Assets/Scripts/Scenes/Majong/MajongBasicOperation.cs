using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MajongBasicOperation : MonoBehaviour {

    private Button passButton;
    private Button huBtn;
    private Button okBtn;

    // Use this for initialization
    void Start ()
    {
        passButton = transform.Find("Pass").GetComponent<Button>();
        passButton.onClick.AddListener(pass);
        huBtn = transform.parent.Find("Hu/HuBtn").GetComponent<Button>();
        huBtn.onClick.AddListener(hu);
        okBtn = transform.parent.Find("Hu/WinMsg/ok").GetComponent<Button>();
        okBtn.onClick.AddListener(ok);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void pass()
    {
        MajongCardsController.ins.Pass();
    }

    void hu()
    {
        Debug.Log("Hu");
        MajongCardsController.ins.Hu();
    }

    void ok()
    {
        MajongCardsController.ins.ResetUnPlayMajong();
    }

}
