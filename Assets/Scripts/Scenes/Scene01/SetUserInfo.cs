using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Common.Code;
using UnityEngine;
using UnityEngine.UI;

public class SetUserInfo : MonoBehaviour
{

    private Transform userInfoSettingWindow;
    private Image HeadSprite4SetWindow;
    private InputField NickNameInputField4SetWindow;
    private Sprite HeadSprite4GameSelectPanel;
    private InputField NickNameInputField4GameSelectPanel;

    private byte[] tempPicBytes;

    void Awake()
    {
        userInfoSettingWindow = GameObject.Find("SetUserInfoWindow").transform;
        HeadSprite4SetWindow = userInfoSettingWindow.Find("SettingBG/HeadSculpture").GetComponent<Image>();
        NickNameInputField4SetWindow = userInfoSettingWindow.Find("SettingBG/NickNameInputField").GetComponent<InputField>();


    }

    public void ActOpenSetUserInfoWindow()
    {
        tempPicBytes = null;

        HeadSprite4SetWindow.sprite = PhotonManager.Ins.HeadSculptureSprite;
        NickNameInputField4SetWindow.text = PhotonManager.Ins.userDetail.Nickname;
        userInfoSettingWindow.localScale = Vector3.one;
    }

    public void ActLoadLocalImage()
    {
        OpenFileDialog od = new OpenFileDialog();
        od.Title = "本地头像选择";
        od.Multiselect = false;
        od.Filter = "图片文件(*.jpg)|*.jpg";
        od.InitialDirectory = "D:\\";
        if (od.ShowDialog() == DialogResult.OK)
        {
            FileInfo fi = new FileInfo(od.FileName);
            if (fi.Length > 25*1024)
            {
                return;
            }
            StartCoroutine(LoadPicture(od.FileName));
        }
    }

    public void ActOKBtnClick()
    {
        if (NickNameInputField4SetWindow.text == PhotonManager.Ins.userDetail.Nickname && tempPicBytes == null)
            return;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[0] = NickNameInputField4SetWindow.text;
        parameters[1] = tempPicBytes;
        parameters[50] = DetailCode.UpdateInfo;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Detail, parameters);
    }

    public void ActCancelBtnClick()
    {
        userInfoSettingWindow.localScale = Vector3.zero;
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
        HeadSprite4SetWindow.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        tempPicBytes = myByte;
    }
}
