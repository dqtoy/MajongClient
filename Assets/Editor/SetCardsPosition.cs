using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SetCardsPosition : MonoBehaviour {

	[MenuItem("Toolbar/SetPosition")]
    static void Set()
    {
        Transform transform = GameObject.Find("PlayerCards").transform;
        foreach(Transform tsm in transform)
        {
            tsm.position +=new Vector3(-0.5f,0,0);
        }
    }
}
