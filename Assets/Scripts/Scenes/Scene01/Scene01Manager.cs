using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Commom.Dto;
using Common.Code;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene01Manager : MonoBehaviour
{
    public static Scene01Manager Ins;
    //IndexPanel
    private GameObject IndexPanel;
    private Text Text4LoginInfo;
    private Text Text4RegisterInfo;
    //GameSelectPanel
    
    private GameObject GameSelectPanel;
    private Image HeadSculptureImage;
    private Text NicknameText;
    private Text GoldNumberText;
    //SetUserInfoWindow
    private Transform setUserInfoWindow;
    void Awake()
    {
        Ins = this;

        IndexPanel = GameObject.Find("IndexPanel");
        Text4LoginInfo = GameObject.Find("Text-Login-Info").transform.GetComponent<Text>();
        Text4RegisterInfo = GameObject.Find("Text-Register-Info").transform.GetComponent<Text>();
        
        GameSelectPanel = GameObject.Find("GameSelectPanel");
        HeadSculptureImage = GameObject.Find("HeadSculpture").transform.GetComponent<Image>();
        NicknameText = GameObject.Find("NickName").transform.GetComponent<Text>();
        GoldNumberText = GameObject.Find("GoldNumber").transform.GetComponent<Text>();

        setUserInfoWindow = GameObject.Find("SetUserInfoWindow").transform;
    }

    void Start()
    {
        
    }
    #region ExcuteAccountInfo
    /// <summary>
    /// 登录处理
    /// </summary>
    public void Excute4Login(OperationResponse response)
    {
        if (response.ReturnCode < 0)
        {
            ShowLoginInfo(response.DebugMessage);
        }
        else
        {
            DetailDto dto = LitJson.JsonMapper.ToObject<DetailDto>(response.Parameters[0].ToString());
            SetUserInfoAndSaveGlobalValue(dto);
            GameSelectPanel.transform.localScale = Vector3.one;
            IndexPanel.transform.localScale = Vector3.zero;
        }
    }
    /// <summary>
    /// 初始化登录信息，设置全局变量
    /// </summary>
    void SetUserInfoAndSaveGlobalValue(DetailDto dto)
    {
        
        NicknameText.text = dto.Nickname;
        GoldNumberText.text = dto.Gold.ToString();
//        
//        Texture2D tex2D = new Texture2D(64, 64);
//        byte[] myByte = Convert.FromBase64String(dto.PictureStr);
//        tex2D.LoadImage(myByte);
//        Sprite sprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f));
//        PhotonManager.Ins.HeadSculptureSprite = sprite;
        Sprite img = GetHeadImage(dto.PictureStr);
        HeadSculptureImage.sprite = img;

        PhotonManager.Ins.userDetail = dto;
        PhotonManager.Ins.HeadSculptureSprite = img;
    }

    Sprite GetHeadImage(string spriteName)
    {
        Sprite img = GameManager.ins.EgSprite;
        if (spriteName != "eg")
            StartCoroutine(loadHeadImage(spriteName));
        return img;
    }

    IEnumerator loadHeadImage(string pictureName)
    {
        string imgPath = Config.ServerPath + pictureName + ".jpg";
        Debug.Log(imgPath);
        WWW www = new WWW(imgPath);
        yield return www;

        Texture2D tex2d = www.texture;
        Sprite sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0.5f, 0.5f));
        HeadSculptureImage.sprite = sprite;
        PhotonManager.Ins.HeadSculptureSprite = sprite;

    }

    public void ShowLoginInfo(string info)
    {
        Text4LoginInfo.text = info;
    }

    /// <summary>
    /// 显示注册信息
    /// </summary>
    public void ShowRegisterInfo(string info)
    {
        Text4RegisterInfo.text = info;
    }
    /// <summary>
    /// 返回登录界面
    /// </summary>
    public void Excute4BackToLoginPanel()
    {
        IndexPanel.transform.localScale = Vector3.one;
        GameSelectPanel.transform.localScale = Vector3.zero;
    }
    #endregion

    #region ExcuteDetailInfo

    public void UpdateHeadAndNickName(DetailDto dto)
    {
        PhotonManager.Ins.userDetail = dto;
        Sprite sprite = setUserInfoWindow.Find("SettingBG/HeadSculpture").GetComponent<Image>().sprite;
        PhotonManager.Ins.HeadSculptureSprite = sprite;

        NicknameText.text = dto.Nickname;
        HeadSculptureImage.sprite = sprite;
        setUserInfoWindow.localScale = Vector3.zero;
    }

    #endregion



    public void ShowLog(object obj)
    {
        Debug.Log(obj.ToString());
    }
}
