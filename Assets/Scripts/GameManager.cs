using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager ins;
    public Sprite EgSprite;
    public Sprite EmptySprite;
    public Sprite ReadySprite;
    public Sprite PlayingSprite;
     
    void Awake()
    {
        ins = this;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


}
