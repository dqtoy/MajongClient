using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public Transform C;
    int n = 0;
    private void OnGUI()
    {
        if(GUILayout.Button("SetIndex"))
        {
            C.SetSiblingIndex(n);
            n++;
        }
    }
}
