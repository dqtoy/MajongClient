using System.Collections.Generic;
using Commom.Dto;
using Common.Code;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Account4LoginRegister : MonoBehaviour
{
    public InputField IF4LoginAccount;
    public InputField IF4LoginPassword;
    public void OnLogin()
    {
        if (IF4LoginAccount.text == "" || IF4LoginPassword.text == "")
        {
            Scene01Manager.Ins.ShowLoginInfo("用户名或密码不能为空！");
            return;
        }

        AccountDto dto = new AccountDto();
        dto.Account = IF4LoginAccount.text;
        dto.Password = IF4LoginPassword.text;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[0] = LitJson.JsonMapper.ToJson(dto);
        parameters[50] = AccountCode.Login;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Account, parameters);
    }

    public InputField IF4RegisterAccount;
    public InputField IF4Registerpassword0;
    public InputField IF4Registerpassword1;
    public void OnRegister()
    {
        if (IF4RegisterAccount.text == "eg0" || IF4RegisterAccount.text == "eg1")
        {
            Scene01Manager.Ins.ShowRegisterInfo("用户名已存在！");
            return;
        }
        if (IF4RegisterAccount.text == "" || IF4Registerpassword0.text == "" || IF4Registerpassword1.text == "")
        {
            Scene01Manager.Ins.ShowRegisterInfo("用户名或密码不能为空！");
            return;
        }
        if (IF4Registerpassword0.text != IF4Registerpassword1.text)
        {
            Scene01Manager.Ins.ShowRegisterInfo("两次密码不一致！");
            return;
        }
        AccountDto dto = new AccountDto();
        dto.Account = IF4RegisterAccount.text;
        dto.Password = IF4Registerpassword0.text;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[0] = LitJson.JsonMapper.ToJson(dto);
        parameters[50] = AccountCode.Register;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Account, parameters);
    }

    public GameObject LoginWindow;
    public GameObject RegisterWindow;
    public void ToRegister()
    {
        LoginWindow.transform.localScale = Vector3.zero;
        RegisterWindow.transform.localScale = Vector3.one;
    }
    public void ToLogin()
    {
        IF4RegisterAccount.text = "";
        IF4Registerpassword0.text = "";
        IF4Registerpassword1.text = "";
        Scene01Manager.Ins.ShowRegisterInfo("");
        RegisterWindow.transform.localScale = Vector3.zero;
        LoginWindow.transform.localScale = Vector3.one;
    }

    public GameObject GameSelectPanel; 
    public void BackToLoginFromGameSelectPanel()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = AccountCode.Logout;
        parameters[0] = LitJson.JsonMapper.ToJson(new AccountDto());
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Account, parameters);
    }

    public void OnDouDiZhuClick()
    {
        SceneManager.LoadScene("");
    }

    public void OnMajongClick()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = MajongCode.Enter;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }
}
