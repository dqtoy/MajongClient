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

    void Init()
    {
        //创建头像文件夹
        if (!Directory.Exists(Config.ImagePath))
        {
            Directory.CreateDirectory(Config.ImagePath);
        }else
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Config.ImagePath);
            foreach (var file in dirInfo.GetFiles())
            {
                string endStr = file.Name.Substring(file.Name.Length - 4);
                if (endStr == ".jpg")
                {
                    StartCoroutine(loadLoaclHeadImages(file.Name));
                }
            }
        }
    }

    IEnumerator loadLoaclHeadImages(string fileName)
    {
        string imgPath = "file://" + Path.Combine(Config.ImagePath, fileName);
        WWW www = new WWW(imgPath);
        yield return www;

        Texture2D tex2D = www.texture;
        Sprite sprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f));
        string nameStr = fileName.Substring(0, fileName.Length - 4);
    }
}
