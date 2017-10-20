using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Windows.Forms;
using Commom.Dto;
using Common.Code;
using UnityEngine.SceneManagement;

public class LoadUserInfo : MonoBehaviour
{

    public Image HeadSculptureImage;
    public Text NickNameText;
    public Text GoldNumberText;

    public Sprite[] DefultSprites;
    void Start()
    {
        
    }

    public GameObject SettingWindow;
    public GameObject HeadSculptureWindow;
    public GameObject NickNameWindow;

    #region 头像选择

    public void OnHeadSculptureClick()
    {
        SettingWindow.SetActive(true);
        HeadSculptureWindow.SetActive(true);
    }


    public void OnSelectLocalTexture()
    {
        OpenFileDialog od = new OpenFileDialog();
        od.Title = "本地头像选择";
        od.Multiselect = false;
        od.Filter = "图片文件(*.jpg)|*.jpg";
        od.InitialDirectory = "D:\\";
        if (od.ShowDialog() == DialogResult.OK)
        {
            StartCoroutine(LoadPicture(od.FileName));
            HideSettingWindows();
        }
    }
    IEnumerator LoadPicture(string path)
    {
        //文件流读取
        FileStream fs = new FileStream(path, FileMode.Open);
        byte[] myByte = new byte[fs.Length];
        fs.Read(myByte, 0, myByte.Length);
        fs.Close();
        Texture2D texture = new Texture2D(128, 128);
        texture.LoadImage(myByte);
        yield return new WaitForSeconds(0.01f);
        //图片设置
        PhotonManager.Ins.HeadSculptureSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        //HeadSculptureImage.sprite = sprite;
        string picStr = Convert.ToBase64String(myByte);
        PhotonManager.Ins.userDetail.PictureStr = picStr;
        SendRequest((byte)DetailCode.UpdateInfo);
    }
    #endregion



    /// <summary>
    /// 取消头像、名称编辑
    /// </summary>
    public void OnCancelClick()
    {
        HideSettingWindows();
    }
    /// <summary>
    /// 返回登录界面
    /// </summary>
    public void OnBackToLoginBtn()
    {
        //SceneManager.LoadScene("01Main");
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = AccountCode.Logout;
        parameters[0] = LitJson.JsonMapper.ToJson( new AccountDto());
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Account, parameters);
    }
    /// <summary>
    /// 返回大厅
    /// </summary>
    public void OnBackToLobbyClick()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = MajongCode.Leave;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }

    public void OnBackToRoomClick()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = MajongCode.Stand;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }
    /// <summary>
    /// 游戏准备
    /// </summary>
    public void OnReadyClick()
    {
        Scene02Manager.Ins.mySeatDto.State = 1;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = MajongCode.Ready;
        parameters[0] = LitJson.JsonMapper.ToJson(Scene02Manager.Ins.mySeatDto);
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }
    /// <summary>
    /// 发送更改消息到服务器
    /// </summary>
    private void SendRequest(byte subCode)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[0] = LitJson.JsonMapper.ToJson(PhotonManager.Ins.userDetail);
        parameters[50] = subCode;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Detail, parameters);
    }

    private void HideSettingWindows()
    {
        SettingWindow.SetActive(false);
        HeadSculptureWindow.SetActive(false);
        NickNameWindow.SetActive(false);
    }
}
