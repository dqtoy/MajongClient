using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChowPongKongClick : MonoBehaviour {

	public void OnClick()
	{
	    string parentName = transform.parent.name;
	    int posId = int.Parse(parentName.Substring(parentName.Length-1,1));
	    Scene02Manager.Ins.KongClick(posId);
	}
}
