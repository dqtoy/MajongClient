using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Style : MonoBehaviour {

    
	// Use this for initialization
	void Start () {
        ShowCards();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowCards()
    {
        int n = 0;
        foreach (Transform child in transform)
        {
            child.position += new Vector3(0.3f * n, 0, 0);
            n++;
        }
    }
}
