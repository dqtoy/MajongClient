using System.Collections;
using System.Collections.Generic;
using Commom.Dto;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Common.Code;

public class MajongManager : MonoBehaviour
{
    public static MajongManager ins;

    private Transform majongLobby;
    private Image headSculptureImage;
    private Text nickNameText;
    private Text goldNumText;

    private Transform majongInterface;
    private Transform player0InfoPanel;
    private Transform player1InfoPanel;
    private Transform majongCardsPanel;
    private MajongCardsController majongCardsControllerScript;

    private DetailDto myInfoDto;
    private Dictionary<string, Sprite> nameHeadSpriteDict;
    private RoomDto myRoomDto;
    private Dictionary<int, int> tableIdStateDict;
    private int myTableId;
    private bool isPlaying = false;

    #region MonoBehaviour Method
    void Awake()
    {
        ins = this;
        tableIdStateDict = new Dictionary<int, int>();
        nameHeadSpriteDict = new Dictionary<string, Sprite>();
        nameHeadSpriteDict.Add("eg", GameManager.ins.EgSprite);

        for (int i = 0; i < 6; i++)
        {
            tableIdStateDict.Add(i, 0);
        }
    }
    #endregion

    #region Public Method
    /// <summary>
    /// 进入麻将房间
    /// </summary>
    /// <param name="response"></param>
    public void Excute4Enter(OperationResponse response)
    {

        if (response.ReturnCode == 0)
        {
            getHandler();
            myRoomDto = LitJson.JsonMapper.ToObject<RoomDto>(response.Parameters[0].ToString());
            initLobby(myRoomDto);
        }
        else if (response.ReturnCode == 1)
        {
            DetailDto otherDetailDto = LitJson.JsonMapper.ToObject<DetailDto>(response.Parameters[0].ToString());
            myRoomDto.NameDetailDict.Add(otherDetailDto.Name, otherDetailDto);
        }
    }

    public void Excute4Sit(OperationResponse response)
    {
        SeatDto seatDto = LitJson.JsonMapper.ToObject<SeatDto>(response.Parameters[0].ToString());
        SetPlayerHeadSculptureAtChair(seatDto, true);
        if (response.ReturnCode == 0)
        {
            //显示界面
            myTableId = seatDto.TableID;
            showMyMajongPlayingInterface(seatDto);
        }
        else
        {
            if (seatDto.TableID == myTableId)
            {
                showPlayer1Info(seatDto);
            }
        }
    }

    public void Excute4Stand(OperationResponse response)
    {
        myTableId = -1;
        SeatDto seatDto = LitJson.JsonMapper.ToObject<SeatDto>(response.Parameters[0].ToString());
        tableIdStateDict[seatDto.TableID] -= seatDto.TableID;
        string pathStr = "Majong/MajongSeatPanel/" + seatDto.TableID.ToString("00") + "/Seat" + seatDto.Id + "/HeadSculpture";
        Image smallHeadImg = majongLobby.Find(pathStr).GetComponent<Image>();
        smallHeadImg.sprite = GameManager.ins.EmptySprite;
        string nickNamePath = "Majong/MajongSeatPanel/" + seatDto.TableID.ToString("00") + "/Seat" + seatDto.Id + "/NickName";
        Text nickNameText = majongLobby.Find(nickNamePath).GetComponent<Text>();
        nickNameText.text = "";
        switch (response.ReturnCode)
        {
            case 0:
                Destroy(majongInterface.gameObject);
                break;
            case 1:
                player1InfoPanel.localScale = Vector3.zero;
                break;
            case 2:
                break;
            default:
                break;
        }
    }

    public void Excute4Ready(OperationResponse response)
    {
        SeatDto seatDto = LitJson.JsonMapper.ToObject<SeatDto>(response.Parameters[0].ToString());
        SetReadyStateAtTable(seatDto, true);
        //判断是否开始游戏
        tableIdStateDict[seatDto.TableID] += 1;
        if (tableIdStateDict[seatDto.TableID] == 2)
        {
            isPlaying = true;
            SetPlayingSpriteAtTable(seatDto.TableID, true);
        }
        switch (response.ReturnCode)
        {
            case 0:
                Transform player0ReadImg = majongInterface.Find("Play0InfoPanel/ReadyImg");
                player0ReadImg.localScale = Vector3.one;
                majongInterface.Find("ReadyBtn").localScale = Vector3.zero;

                if (isPlaying)
                {
                    Dictionary<byte, object> parameters = new Dictionary<byte, object>();
                    parameters[50] = MajongCode.Start;
                    PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
                }
                break;
            case 1:
                Transform player1ReadImg = majongInterface.Find("Play1InfoPanel/ReadyImg");
                player1ReadImg.localScale = Vector3.one;
                break;
            default:
                break;
        }
       
    }

    public void Excute4GameStart(OperationResponse response)
    {
        majongCardsControllerScript = majongInterface.Find("MajongCards").GetComponent<MajongCardsController>();
        majongCardsControllerScript.MyMjongCards = LitJson.JsonMapper.ToObject<List<int>>(response.Parameters[0].ToString());
        //隐藏准备按钮
        Transform player0ReadImg = majongInterface.Find("Play0InfoPanel/ReadyImg");
        player0ReadImg.localScale = Vector3.zero;
        Transform player1ReadImg = majongInterface.Find("Play1InfoPanel/ReadyImg");
        player1ReadImg.localScale = Vector3.zero;

        majongCardsPanel = majongInterface.Find("MajongCards");
        majongCardsPanel.localScale = Vector3.one;

        majongCardsControllerScript.ShowMajongCards();
    }

    public void Excute4GetCard(OperationResponse response)
    {
        int id = -1;
        if (response.ReturnCode == 0)
        {
            id = int.Parse(response.Parameters[0].ToString());
        }
        majongCardsControllerScript.ShowExtraCard(id);
    }

    public void Excute4DisCard(OperationResponse response)
    {
        int id = int.Parse(response.Parameters[0].ToString());
        Debug.Log("出牌：" + id);
        majongCardsControllerScript.ShowDiscard(response.ReturnCode,id);
    }

    public void Excute4Chow(OperationResponse response)
    {

    }

    public void Excute4Pong(OperationResponse response)
    {

    }

    public void Excute4Kong(OperationResponse response)
    {

    }

    public void Excute4Win(OperationResponse response)
    {

    }

    public void Excute4End(OperationResponse response)
    {

    }

    #endregion

    #region 外部获取
    /// <summary>
    /// 是否座位已有人
    /// </summary>
    /// <param name="tableId"></param>
    /// <param name="seatId"></param>
    /// <returns></returns>
    public bool IsSitted(int tableId, int seatId)
    {
        bool isSit = false;
        foreach (var item in myRoomDto.SeatedList)
        {
            if (item.TableID == tableId && item.Id == seatId)
            {
                isSit = true;
                break;
            }
        }
        return isSit;
    }
    /// <summary>
    /// 判断当前的游戏状态
    /// </summary>
    /// <returns></returns>
    public bool IsMajongPlaying()
    {
        return isPlaying;
    }

    public bool CanDiscard()
    {
        if (majongCardsControllerScript.MyMjongCards.Count%3 == 2)
            return true;
        else
            return false;
    }
    

    #endregion
    #region Private Method
    void getHandler()
    {
        //句柄获取
        GameObject obj = (GameObject)Resources.Load("Majong/MajongLobby");
        majongLobby = Instantiate(obj).transform;

        headSculptureImage = majongLobby.Find("Panel/UserInfo/HeadSculpture").GetComponent<Image>();
        nickNameText = majongLobby.Find("Panel/UserInfo/Nickname").GetComponent<Text>();
        goldNumText = majongLobby.Find("Panel/UserInfo/GoldNumber").GetComponent<Text>();

        myInfoDto = PhotonManager.Ins.userDetail;
    }

    void initLobby(RoomDto roomDto)
    {

        //头像赋值
        headSculptureImage.sprite = PhotonManager.Ins.HeadSculptureSprite;
        nickNameText.text = myInfoDto.Nickname;
        goldNumText.text = myInfoDto.Gold.ToString();
        //初始化房间信息
        RefreshRoomState(roomDto);
    }
   
    /// <summary>
    /// 初始化房间信息
    /// </summary>
    /// <param name="roomDto"></param>
    void RefreshRoomState(RoomDto roomDto)
    {
        //初始化房间
        foreach (var item in roomDto.SeatedList)
        {
            //设置头像
            SetPlayerHeadSculptureAtChair(item, true);
            //设置准备状态
            if (item.State == 1)
                SetReadyStateAtTable(item, true);
        }

        for (int i = 0; i < roomDto.SeatedList.Count; i++)
        {
            SeatDto seatDto = roomDto.SeatedList[i];
            tableIdStateDict[seatDto.TableID] += seatDto.State;

            if (tableIdStateDict[seatDto.TableID] == 2)
            {
                SetPlayingSpriteAtTable(seatDto.TableID, true);
            }
        }
    }
    /// <summary>
    /// 显示玩家头像
    /// </summary>
    /// <param name="seatDto"></param>
    /// <param name="setBool"></param>
    void SetPlayerHeadSculptureAtChair(SeatDto seatDto, bool setBool)
    {
        Sprite sprite = GameManager.ins.EmptySprite;
        string pathStr = "Majong/MajongSeatPanel/" + seatDto.TableID.ToString("00") + "/Seat" + seatDto.Id + "/HeadSculpture";
        Image smallHeadImg = majongLobby.Find(pathStr).GetComponent<Image>();
        string nickNamePath = "Majong/MajongSeatPanel/" + seatDto.TableID.ToString("00") + "/Seat" + seatDto.Id + "/NickName";
        Text nickNameText = majongLobby.Find(nickNamePath).GetComponent<Text>();

        if (setBool)
        {
            nickNameText.text = myRoomDto.NameDetailDict[seatDto.PlayName].Nickname;
            string imageName = myRoomDto.NameDetailDict[seatDto.PlayName].PictureStr;
            SetHeadSculptureAtChair(imageName, smallHeadImg);
        }
        else
        {
            smallHeadImg.sprite = sprite;
            nickNameText.text = "";
        }
    }

    void SetHeadSculptureAtChair(string imageName, Image smallHeadImage)
    {
        if (nameHeadSpriteDict.ContainsKey(imageName))
        {
            smallHeadImage.sprite = nameHeadSpriteDict[imageName];
        }
        else
        {
            StartCoroutine(DownLoadAndSetHeadSprite(imageName, smallHeadImage));
        }
    }
    IEnumerator DownLoadHeadSprite(string imageName)
    {
        string imgPath = Config.ServerPath + imageName + ".jpg";
        WWW www = new WWW(imgPath);
        yield return www;

        Texture2D tex2D = www.texture;
        Sprite curSprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f));
        nameHeadSpriteDict.Add(imageName, curSprite);
    }

    IEnumerator DownLoadAndSetHeadSprite(string imageName,Image smallHeadImage)
    {
        Debug.Log(imageName+"  "+ smallHeadImage.name);
        string imgPath = Config.ServerPath + imageName + ".jpg";
        WWW www = new WWW(imgPath);
        yield return www;

        Texture2D tex2D = www.texture;
        Sprite curSprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f));
        if (!nameHeadSpriteDict.ContainsKey(imageName))
        {
            nameHeadSpriteDict.Add(imageName, curSprite);
        }
        smallHeadImage.sprite = curSprite;
    }
    /// <summary>
    /// 显示准备状态
    /// </summary>
    /// <param name="seatDto"></param>
    /// <param name="setBool"></param>
    void SetReadyStateAtTable(SeatDto seatDto, bool setBool)
    {
        Sprite sprite = GameManager.ins.EmptySprite;
        string pathStr = "Majong/MajongSeatPanel/" + seatDto.TableID.ToString("00") + "/Table/State/Seat" + seatDto.Id + "State";
        Image stateImg = majongLobby.Find(pathStr).GetComponent<Image>();
        if (setBool)
            sprite = GameManager.ins.ReadySprite;
        stateImg.sprite = sprite;
    }
    /// <summary>
    /// 从服务器下载图片并且贴到Image上
    /// </summary>
    /// <param name="imageName"></param>
    /// <param name="image"></param>
    /// <returns></returns>
    

    void SetPlayingSpriteAtTable(int tableId, bool setBool)
    {
        Sprite sprite = GameManager.ins.EmptySprite;
        string pathStr = "Majong/MajongSeatPanel/" + tableId.ToString("00") + "/Table/State/Playing";
        Image playingImage = majongLobby.Find(pathStr).GetComponent<Image>();
        /*if (tableStateDict[tableId] == currentGamePlayingState)
        {
            sprite = PlayingSprite;
            //游戏开始
            Dictionary<byte, object> parameters = new Dictionary<byte, object>();
            parameters[50] = PlayMajongCode.Start;
            parameters[0] = tableId.ToString();
            PhotonManager.Ins.OnOperationRequest((byte)OpCode.PlayMajong, parameters);
        }*/
        if (setBool)
        {
            sprite = GameManager.ins.PlayingSprite;
        }
        playingImage.sprite = sprite;
    }

    void showMyMajongPlayingInterface(SeatDto seatDto)
    {
        GameObject obj = (GameObject)Resources.Load("Majong/MajongInterface");
        majongInterface = Instantiate(obj).transform;
        player0InfoPanel = majongInterface.Find("Play0InfoPanel");
        player0InfoPanel.Find("HeadImage").GetComponent<Image>().sprite = PhotonManager.Ins.HeadSculptureSprite;
        player0InfoPanel.Find("NameText").GetComponent<Text>().text = PhotonManager.Ins.userDetail.Nickname;
        player0InfoPanel.Find("GoldText").GetComponent<Text>().text = PhotonManager.Ins.userDetail.Gold.ToString();

        foreach (var item in myRoomDto.SeatedList)
        {
            if (item.TableID == seatDto.TableID)
            {
                Debug.Log("Show Player1Info");
                showPlayer1Info(item);
            }
        }
    }

    void showPlayer1Info(SeatDto seatDto)
    {
        player1InfoPanel = majongInterface.Find("Play1InfoPanel");
        DetailDto player1Info = myRoomDto.NameDetailDict[seatDto.PlayName];
        Image player1Head = player1InfoPanel.Find("HeadImage").GetComponent<Image>();
        SetHeadSculptureAtChair(player1Info.PictureStr, player1Head);
        player1InfoPanel.Find("NameText").GetComponent<Text>().text = player1Info.Nickname;
        player1InfoPanel.Find("GoldText").GetComponent<Text>().text = player1Info.Gold.ToString();
        if (seatDto.State == 1)
        {
            player1InfoPanel.Find("ReadyImg").localScale = Vector3.one;
        }
        player1InfoPanel.localScale = Vector3.one;
    }

    #endregion
}
