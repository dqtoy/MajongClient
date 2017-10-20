using System;
using System.Collections;
using System.Collections.Generic;
using Commom.Dto;
using Common.Code;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene02Manager : MonoBehaviour
{
    public static Scene02Manager Ins;
    private MajongOperation mjOperation = new MajongOperation();
    
    void Awake()
    {
        Ins = this;
    }

    void Start()
    {
        tableStateDict = new Dictionary<int, int>();
        for (int i =0;i<tableNumber;i++)
        {
            tableStateDict.Add(i, 0);
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            SceneManager.LoadScene("01Main");
        }
    }
    #region 头像设置
    /// <summary>
    /// 设置头像或者昵称
    /// </summary>
    public Image HeadSculptureImage;
    public Text NickNameText;
    public void SetUserDetailInformation()
    {
        HeadSculptureImage.sprite = PhotonManager.Ins.HeadSculptureSprite;
        NickNameText.text = PhotonManager.Ins.userDetail.Nickname;
    }
    #endregion

    #region 初始化房间状态

    public Sprite[] DefaultSprites;
    public Sprite ReadySprite;
    public Sprite PlayingSprite;
    public Sprite EmptySprite;
    public RoomDto MyRoomDto;
    public Dictionary<int, int> tableStateDict;


    private GameObject gameKindWindow;
    private GameObject currentGameWindow;
    private GameObject currentGameInterface;

    private UnityEngine.Object player0;
    private UnityEngine.Object player1;

    private int currentGamePlayingState;
    private string currentGameName;

    private int tableNumber = 6;

    public void InitEnterValues(string GameName,int playingState)
    {
        gameKindWindow = GameObject.Find("GameKind");
        GameObject root = GameObject.Find("BGPanel");
        currentGameWindow = root.transform.Find(GameName).gameObject;
        currentGameInterface = GameObject.Find(GameName+"Interface");

        currentGamePlayingState = playingState;
        currentGameName = GameName;
    }

    public void RefreshRoomState(RoomDto roomDto)
    {
        //全局赋值
        MyRoomDto = roomDto;
        
        //初始化房间
        foreach (var item in MyRoomDto.SeatedList)
        {
            //设置头像
            SetHeadSculptureAtChair(item,true);
            //设置准备状态
            if(item.State == 1)
                SetReadyStateAtTable(item,true);
        }

        
        for (int i = 0; i < roomDto.SeatedList.Count; i++)
        {
            int currentState = 0;
            for (int j = 0; j < roomDto.SeatedList.Count; j++)
            {
                if (roomDto.SeatedList[i].TableID == roomDto.SeatedList[j].TableID)
                {
                    currentState += roomDto.SeatedList[j].State;
                }
            }
            if (currentState == currentGamePlayingState)
            {
                SetPlayingSpriteAtTable(roomDto.SeatedList[i].TableID, true);
            }
        }
    }

    public void SetPlayingSpriteAtTable(int  tableId,bool setBool)
    {
        Sprite sprite = EmptySprite;
        if (tableStateDict[tableId] == currentGamePlayingState)
        {
            sprite = PlayingSprite;
            //游戏开始
            Dictionary<byte, object> parameters = new Dictionary<byte, object>();
            parameters[50] = MajongCode.Start;
            parameters[0] = tableId.ToString();
            PhotonManager.Ins.OnOperationRequest((byte) OpCode.Majong, parameters);
        }
        string pathStr = currentGameName +"SeatPanel/Table" + tableId + "/Table/State/Playing";
        Image playingImg = currentGameWindow.transform.Find(pathStr).GetComponent<Image>();
        playingImg.sprite = sprite;
    }

    /// <summary>
    /// 设置房间内座位显示头像
    /// </summary>
    /// <param name="seatDto"></param>
    /// <param name="setBool">是否设置头像</param>
    public void SetHeadSculptureAtChair(SeatDto seatDto,bool setBool)
    {
        Sprite sprite = EmptySprite;
        if (setBool)
        {
            string picStr = MyRoomDto.NameDetailDict[seatDto.PlayName].PictureStr;
            sprite = GetSpriteFromStr(picStr);
        }
        string pathStr = currentGameName + "SeatPanel/Table" + seatDto.TableID + "/Seat" + seatDto.Id + "/HeadSculpture";
        Image smallHeadImg = currentGameWindow.transform.Find(pathStr).GetComponent<Image>();
        smallHeadImg.sprite = sprite;
    }

    /// <summary>
    ///设置玩家准备状态
    /// </summary>
    /// <param name="seatDto"></param>
    public void SetReadyStateAtTable(SeatDto seatDto,bool setBool)
    {
        Sprite sprite = EmptySprite;
        if (setBool)
            sprite = ReadySprite;
        string pathStr = currentGameName + "SeatPanel/Table" + seatDto.TableID + "/Table/State/Seat" + seatDto.Id + "State";
        Image stateImg = currentGameWindow.transform.Find(pathStr).GetComponent<Image>();
        stateImg.sprite = sprite;
    }


    public void ShowRoom()
    {
        currentGameWindow.SetActive(true);
        gameKindWindow.SetActive(false);
    }
    #endregion

    #region 进入游戏界面
    public SeatDto mySeatDto = new SeatDto();
    private GameObject player0Panel;
    private GameObject player1Panel;
    /// <summary>
    /// 选择座位
    /// </summary>
    /// <param name="seatDto"></param>
    public void EnterGameInterface(SeatDto seatDto)
    {
        currentGameInterface.transform.localScale = Vector3.one;
        //
        //player0Panel = Instantiate(Resources.Load("Majong/Play0InfoPanel", typeof(GameObject))) as GameObject;
        //player0Panel.transform.SetParent(currentGameInterface.transform);
        player0Panel = currentGameInterface.transform.Find("Play0InfoPanel").gameObject;
        player0Panel.transform.localScale = Vector3.one;

        //显示玩家信息
        Image headImg0 = player0Panel.transform.Find("HeadImage").GetComponent<Image>();
        Text nameText0 = player0Panel.transform.Find("NameText").GetComponent<Text>();
        Text goldText0 = player0Panel.transform.Find("GoldText").GetComponent<Text>();
        headImg0.sprite = GetSpriteFromStr(PhotonManager.Ins.userDetail.PictureStr);
        nameText0.text = PhotonManager.Ins.userDetail.Nickname;
        goldText0.text = PhotonManager.Ins.userDetail.Gold.ToString();
        //显示对手信息
        foreach (var item in MyRoomDto.SeatedList)
        {
            if (seatDto.TableID == item.TableID && seatDto.Id != item.Id)
            {
                DetailDto opponentDetailDto = MyRoomDto.NameDetailDict[item.PlayName];
                ShowOpponent(opponentDetailDto, item.State);
                return;
            }
        }
    }
    /// <summary>
    /// 更新新进来的对手
    /// </summary>
    /// <param name="seatDto"></param>
    public void UpdateOpponent(SeatDto seatDto)
    {
        if (seatDto.TableID != mySeatDto.TableID) return;

        DetailDto opponentDetailDto = MyRoomDto.NameDetailDict[seatDto.PlayName];
        ShowOpponent(opponentDetailDto,seatDto.State);
    }

    private void ShowOpponent(DetailDto opponentDetailDto,int state)
    {
        //player1Panel = Instantiate(Resources.Load("Majong/Play1InfoPanel", typeof(GameObject)),new Vector3(Screen.width-10,Screen.height-10,0),Quaternion.identity) as GameObject;
        //player1Panel.transform.SetParent(currentGameInterface.transform);
        player1Panel = currentGameInterface.transform.Find("Play1InfoPanel").gameObject;
        player1Panel.transform.localScale = Vector3.one;
        Image headImg1 = player1Panel.transform.Find("HeadImage").GetComponent<Image>();
        Text nameText1 = player1Panel.transform.Find("NameText").GetComponent<Text>();
        Text goldText1 = player1Panel.transform.Find("GoldText").GetComponent<Text>();
        
        headImg1.sprite = GetSpriteFromStr(opponentDetailDto.PictureStr);
        nameText1.text = opponentDetailDto.Nickname;
        goldText1.text = opponentDetailDto.Gold.ToString();
        if (state == 1)
        {
            player1Panel.transform.Find("ReadyImg").localScale = Vector3.one;
        }
    }

    public void OpponentLeave()
    {
        Destroy(player1Panel);
    }

    public void ShowReadyState(int reCode)
    {
        switch (reCode)
        {
            case 0:
                player0Panel.transform.Find("ReadyImg").localScale = Vector3.one;
                break;
            case 1:
                player1Panel.transform.Find("ReadyImg").localScale = Vector3.one;
                break;

        }
    }

    public void ReadyForGame(SeatDto seatDto)
    {
        UpdataRoomDtoSeatList(seatDto);
        SetReadyStateAtTable(seatDto,true);
    }

    #endregion


    #region 返回大厅
    public void BackToLobby()
    {
        gameKindWindow.SetActive(true);
        currentGameWindow.SetActive(false);
    }
    #endregion
    #region 返回游戏房间

    public void RemoveHeahScrulptureAtChair(SeatDto seatDto)
    {
        SetHeadSculptureAtChair(seatDto,false);
        SetReadyStateAtTable(seatDto,false);
    }

    public void BackToRoomFromGameInterface(int reCode)
    {
        Destroy(player1Panel);
        if (reCode == 0)
        {
            Destroy(player0Panel);
            currentGameInterface.transform.localScale = Vector3.zero;
        }
    }
    #endregion

    #region 实现麻将UI

    public List<int> myCardsList;

    private GameObject majongUIPosition;
    private GameObject myFourteenCard;
    private GameObject itFourteenCard;
    private GameObject myOpUI;
    private GameObject itOpUI;
    public void GameStart(List<int> cardsList)
    {
        myCardsList = cardsList;

        player0Panel.transform.Find("ReadyImg").localScale = Vector3.zero;
        player1Panel.transform.Find("ReadyImg").localScale = Vector3.zero;
        //加载麻将牌
        majongUIPosition = currentGameInterface.transform.Find("MajongUIPosition").gameObject;
//        majongUIPosition = Instantiate(Resources.Load("Majong/MajongUIPosition"),Vector3.zero,Quaternion.identity) as GameObject;
//        majongUIPosition.transform.SetParent(currentGameInterface.transform);
        majongUIPosition.transform.localScale = Vector3.one;
        myFourteenCard = majongUIPosition.transform.Find("MyMajongCards/FrontCard13").gameObject;
        itFourteenCard = majongUIPosition.transform.Find("ItMajongCards/BackCard13").gameObject;
        myOpUI = majongUIPosition.transform.Find("MyOpUI").gameObject;
        itOpUI = majongUIPosition.transform.Find("ItOpUI").gameObject;

        myOpUIList = new List<GameObject>();
        itOpUIList = new List<GameObject>();
        //显示麻将花色
        ShowMyMajongCards();
    }

    

    public void Getcard(int reCode, int tileId)
    {
        switch (reCode)
        {
            case 0:
                myCardsList.Add(tileId);
                Sprite tileSprite = getSpriteByTileId(tileId);
                myFourteenCard.transform.localScale = Vector3.one;
                myFourteenCard.transform.Find("Tile").GetComponent<Image>().sprite = tileSprite;
                isMyGetcard = true;
                break;
            case 1:
                isItGetcard = true;
                itFourteenCard.transform.localScale = Vector3.one;
                break;
        }
    }

    private List<int> myDiscardList = new List<int>();
    private List<int> itDiscardList = new List<int>();
    private bool isMyGetcard = true;
    private bool isItGetcard = true;
    public void Discard(int reCode,int tileId)
    {
        Sprite sprite = getSpriteByTileId(tileId);
        string targetDiscardPath="";
        switch (reCode)
        {
            case 0:
                myDiscardList.Add(tileId);
                targetDiscardPath = "MyDiscards/Discard" + (myDiscardList.Count - 1).ToString() + "/Tile";
                //UI显示
                GameObject targetDiscard0 = majongUIPosition.transform.Find(targetDiscardPath).gameObject;
                targetDiscard0.transform.GetComponent<Image>().sprite = sprite;
                targetDiscard0.transform.parent.localScale = Vector3.one;
                //隐藏摸的牌
                myFourteenCard.transform.localScale = Vector3.zero;
                myCardsList.Remove(tileId);
                ShowMyMajongCards();
                break;
            case 1:
                itDiscardList.Add(tileId);
                targetDiscardPath = "ItDiscards/Discard" + (itDiscardList.Count - 1).ToString() + "/Tile";
                GameObject targetDiscard1 = majongUIPosition.transform.Find(targetDiscardPath).gameObject;
                targetDiscard1.transform.GetComponent<Image>().sprite = sprite;
                targetDiscard1.transform.parent.localScale = Vector3.one;
                //隐藏对手摸的牌
                if (isItGetcard)
                {
                    itFourteenCard.transform.localScale = Vector3.zero;
                }
                else
                {
                    string backCardPath = "ItMajongCards/BackCard" + (12 - 3 * itOpUIList.Count + 1).ToString();
                    majongUIPosition.transform.Find(backCardPath).localScale = Vector3.zero;
                }
                    
                //吃碰杠检测
                int[] chowCode = mjOperation.Chow(tileId, myCardsList);
                int samecards = mjOperation.PongOrKong(tileId, myCardsList);
                showChowOrPongOrKong(tileId,chowCode, samecards);
                break;
        }
    }

    private int opTileId;
    private List<int> opList;
    /// <summary>
    /// 判断可进行的操作
    /// </summary>
    /// <param name="tileId">对方打出的牌</param>
    /// <param name="chowCode">吃牌的可能性</param>
    /// <param name="samecards">手牌中相同牌的归属</param>
    void showChowOrPongOrKong(int tileId,int[] chowCode,int samecards)
    {
        //初始化
        opTileId = tileId;
        opList = new List<int>();

        int n = 0;
        int opId = 99;
        if (chowCode[2] == 1)
        {
            opId = 1;
            Debug.Log("操作id：" + opId);
            showOpUi(opId,n,tileId);
            opList.Add(opId);
            n++;
        }
        if (chowCode[1] == 1)
        {

            opId = 0;
            Debug.Log("操作id：" + opId);
            showOpUi(opId, n, tileId);
            opList.Add(opId);
            n++;
        }
        if (chowCode[0] == 1)
        {
            opId = -1;
            Debug.Log("操作id：" + opId);
            showOpUi(opId, n, tileId);
            opList.Add(opId);
            n++;
        }
        switch (samecards)
        {
            case 3:
                opId = 2;
                showOpUi(opId, n, tileId);
                opList.Add(opId);
                n++;
                opId = 3;
                showOpUi(opId, n, tileId);
                opList.Add(opId);
                n++;
                break;
            case 2:
                opId = 2;
                showOpUi(opId, n, tileId);
                opList.Add(opId);
                n++;
                break;
            default:
                break;
        }
        if (opList.Count == 0)
        {
            //通知服务器摸牌
            request4getcard();
        }
        else
        {
            showHuOrGuo(false);
        }
    }

    void request4getcard()
    {
        isMyGetcard = true;

        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = MajongCode.Getcard;
        PhotonManager.Ins.OnOperationRequest((byte) OpCode.Majong, parameters);
    }

    private List<GameObject> opUIList = new List<GameObject>();
    void showOpUi(int opId,int posId,int tileId)
    {
        string posPath = "ChowPongKong/Pos" + posId.ToString();
        Transform targetPosTransform = majongUIPosition.transform.Find(posPath);
        string pfbPath = "Majong/Kong";
        GameObject obj = Instantiate(Resources.Load(pfbPath), targetPosTransform) as GameObject;
        obj.transform.GetComponent<Button>().enabled = true;
        Transform card0 = obj.transform.Find("UpCard0").transform;
        Transform card1 = obj.transform.Find("UpCard1").transform;
        Transform card2 = obj.transform.Find("UpCard2").transform;
        Transform card3 = obj.transform.Find("UpCard3").transform;
        Sprite sprite0, sprite1, sprite2;
        switch (opId)
        {
            case -1:
                sprite0 = getSpriteByTileId(tileId - 2);
                sprite1= getSpriteByTileId(tileId - 1);
                sprite2 = getSpriteByTileId(tileId);
                card0.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card1.Find("Tile").GetComponent<Image>().sprite = sprite1;
                card2.Find("Tile").GetComponent<Image>().sprite = sprite2;
                card3.transform.localScale = Vector3.zero;
                opUIList.Add(obj);
                break;
            case 0:
                sprite0 = getSpriteByTileId(tileId - 1);
                sprite1 = getSpriteByTileId(tileId );
                sprite2 = getSpriteByTileId(tileId + 1);
                card0.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card1.Find("Tile").GetComponent<Image>().sprite = sprite1;
                card2.Find("Tile").GetComponent<Image>().sprite = sprite2;
                card3.transform.localScale = Vector3.zero;
                opUIList.Add(obj);
                break;
            case 1:
                sprite0 = getSpriteByTileId(tileId);
                sprite1 = getSpriteByTileId(tileId + 1);
                sprite2 = getSpriteByTileId(tileId + 2);
                card0.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card1.Find("Tile").GetComponent<Image>().sprite = sprite1;
                card2.Find("Tile").GetComponent<Image>().sprite = sprite2;
                card3.transform.localScale = Vector3.zero;
                opUIList.Add(obj);
                break;
            case 2:
                sprite0 = getSpriteByTileId(tileId);
                card0.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card1.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card2.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card3.transform.localScale = Vector3.zero;
                opUIList.Add(obj);
                break;
            case 3:
                sprite0 = getSpriteByTileId(tileId);
                card0.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card1.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card2.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card3.Find("Tile").GetComponent<Image>().sprite = sprite0;
                opUIList.Add(obj);
                break;
        }
    }

    void showHuOrGuo(bool isHu)
    {
        string path = "Majong/MajongTile/";
        if (isHu)
        {
            path += "Hu";
        }
        else
        {
            path += "Guo";
        }
        Texture2D tex = (Texture2D)Resources.Load(path);
        Sprite pic = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        Transform tsm = majongUIPosition.transform.Find("HuOrGuo");
        tsm.localScale = Vector3.one;
        tsm.Find("WordPic").GetComponent<Image>().sprite = pic;
    }

    
    void ShowMyMajongCards()
    {
        //MyMajongCardsSort();
        myCardsList.Sort((x, y) => -x.CompareTo(y));
        for (int i = 0; i < 13; i++)
        {
            string targetMajongPath = "MyMajongCards/FrontCard" + i.ToString();
            GameObject obj = majongUIPosition.transform.Find(targetMajongPath).gameObject;
            if (i < myCardsList.Count)
            {
                Sprite tileSprite = getSpriteByTileId(myCardsList[i]);
                obj.transform.localScale = Vector3.one;
                obj.transform.Find("Tile").GetComponent<Image>().sprite = tileSprite;
            }
            else
            {
                obj.transform.localScale = Vector3.zero;
            }
            
        }
    }

    void MyMajongCardsSort()
    {
        //区分白皮和普通牌
        List<int> reList = new List<int>();
        List<int> baipiList = new List<int>();
        foreach (var it in myCardsList)
        {
            if (it == 37)
            {
                baipiList.Add(it);
                continue;
            }
            reList.Add(it);
        }

        //排序并且添加白皮
        reList.Sort((x, y) => -x.CompareTo(y));
        foreach (var it1 in baipiList)
        {
            reList.Add(it1);
        }

        myCardsList = reList;
    }

    Sprite getSpriteByTileId(int tileId)
    {
        string tileName = "";
        int m = tileId / 10;
        switch (m)
        {
            case 0:
                tileName += "wan";
                break;
            case 1:
                tileName += "tiao";
                break;
            case 2:
                tileName += "tong";
                break;
            case 3:
                tileName += "old";
                break;
        }
        int n = tileId % 10;
        tileName += n.ToString();
        //加载图片转化为sprite
        Debug.Log("tileName:" + tileName);
        Texture2D tex = (Texture2D)Resources.Load("Majong/MajongTile/" + tileName);
        Sprite tileSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        return tileSprite;
    }

    #endregion

    #region Chow Pong Kong

    private List<GameObject> myOpUIList;
    private List<GameObject> itOpUIList;

    private int opId;
    private int posId;
    /// <summary>
    /// OpUI点击，通知服务器
    /// </summary>
    /// <param name="posId"></param>
    public void KongClick(int posId)
    {
        opId = opList[posId];
        this.posId = posId;
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[0] = opTileId;
        parameters[1] = opId;
        parameters[50] = MajongCode.Operate; 
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }
    /// <summary>
    /// 响应操作：吃
    /// </summary>
    public void Operate(int reCode,int tileId,int operationId)
    {
        switch (reCode)
        {
            case 0:
                UI_KongClick();
                break;
            case 1:
                UI_KongClick2(tileId , operationId);
                break;
        }
    }

    private void UI_KongClick()
    {
        switch (opId)
        {
            case -1:
                myCardsList.Remove(opTileId - 2);
                myCardsList.Remove(opTileId - 1);
                break;
            case 0:
                myCardsList.Remove(opTileId - 1);
                myCardsList.Remove(opTileId + 1);
                break;
            case 1:
                myCardsList.Remove(opTileId + 1);
                myCardsList.Remove(opTileId + 2);
                break;
            case 2:
                myCardsList.Remove(opTileId);
                myCardsList.Remove(opTileId);
                break;
            case 3:
                myCardsList.Remove(opTileId);
                myCardsList.Remove(opTileId);
                myCardsList.Remove(opTileId);
                break;
        }
        //手牌减少
        int myCardsLenth = myCardsList.Count;
        for (int i = 0; i < 13; i++)
        {
            string targetPath = "MyMajongCards/FrontCard" + i.ToString();
            Transform tsm = majongUIPosition.transform.Find(targetPath);
            if (i < myCardsLenth)
            {
                Sprite tileSprite = getSpriteByTileId(myCardsList[i]);
                tsm.Find("Tile").GetComponent<Image>().sprite = tileSprite;
            }
            else
            {
                tsm.localScale = Vector3.zero;
            }
        }
        //销毁OpUIList
        for (int i = 0; i < opUIList.Count; i++)
        {
            if (i == posId)
            {
                string targetPath = "MyMajongCards/FrontCard" + (12-3*myOpUIList.Count).ToString();
                Transform tsm = majongUIPosition.transform.Find(targetPath);
                opUIList[i].transform.position = tsm.position;
                opUIList[i].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                opUIList[i].transform.parent = myOpUI.transform;
                myOpUIList.Add(opUIList[i]);
            }
            else
            {
                Destroy(opUIList[i]);
            }
        }
        opUIList.Clear();

        //出牌列表销毁
        string targetDiscardPath = "ItDiscards/Discard" + (itDiscardList.Count - 1).ToString();
        GameObject targetDiscard = majongUIPosition.transform.Find(targetDiscardPath).gameObject;
        targetDiscard.transform.localScale = Vector3.zero;
        itDiscardList.RemoveAt(itDiscardList.Count - 1);
        //隐藏按钮：过
        Transform guoTransform = majongUIPosition.transform.Find("Guo");
        guoTransform.localScale = Vector3.zero;
    }

    /// <summary>
    /// 次位基本操作UI显示
    /// </summary>
    private void UI_KongClick2(int tileId,int operationId)
    {
        string pfbPath = "Majong/Kong";
        GameObject obj = Instantiate(Resources.Load(pfbPath), itOpUI.transform) as GameObject;
        obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        Transform card0 = obj.transform.Find("UpCard0").transform;
        Transform card1 = obj.transform.Find("UpCard1").transform;
        Transform card2 = obj.transform.Find("UpCard2").transform;
        Transform card3 = obj.transform.Find("UpCard3").transform;
        Sprite sprite0, sprite1, sprite2;

        int reduceCardsNum = 2;//减少手牌的量
        isItGetcard = false;
        switch (operationId)
        {
            case -1:
                sprite0 = getSpriteByTileId(tileId - 2);
                sprite1 = getSpriteByTileId(tileId - 1);
                sprite2 = getSpriteByTileId(tileId);
                card0.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card1.Find("Tile").GetComponent<Image>().sprite = sprite1;
                card2.Find("Tile").GetComponent<Image>().sprite = sprite2;
                card3.transform.localScale = Vector3.zero;
                break;
            case 0:
                sprite0 = getSpriteByTileId(tileId - 1);
                sprite1 = getSpriteByTileId(tileId);
                sprite2 = getSpriteByTileId(tileId + 1);
                card0.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card1.Find("Tile").GetComponent<Image>().sprite = sprite1;
                card2.Find("Tile").GetComponent<Image>().sprite = sprite2;
                card3.transform.localScale = Vector3.zero;
                break;
            case 1:
                sprite0 = getSpriteByTileId(tileId);
                sprite1 = getSpriteByTileId(tileId + 1);
                sprite2 = getSpriteByTileId(tileId + 2);
                card0.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card1.Find("Tile").GetComponent<Image>().sprite = sprite1;
                card2.Find("Tile").GetComponent<Image>().sprite = sprite2;
                card3.transform.localScale = Vector3.zero;
                break;
            case 2:
                sprite0 = getSpriteByTileId(tileId);
                card0.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card1.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card2.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card3.transform.localScale = Vector3.zero;
                break;
            case 3:
                sprite0 = getSpriteByTileId(tileId);
                card0.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card1.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card2.Find("Tile").GetComponent<Image>().sprite = sprite0;
                card3.Find("Tile").GetComponent<Image>().sprite = sprite0;
                reduceCardsNum = 3;
                break;
        }
        //
        Debug.Log("隐藏");
        for (int i = 0; i < reduceCardsNum; i++)
        {
            string backCardPath = "ItMajongCards/BackCard" + (12 - 3*itOpUIList.Count - i).ToString();
            majongUIPosition.transform.Find(backCardPath).localScale = Vector3.zero;
        }
        //更新位置
        string targetPath = "ItMajongCards/BackCard" + (12 - 3*itOpUIList.Count).ToString();
        Transform tsm = majongUIPosition.transform.Find(targetPath);
        obj.transform.position = tsm.position;
        itOpUIList.Add(obj);

        //销毁出牌列表
        string targetDiscardPath = "MyDiscards/Discard" + (myDiscardList.Count - 1).ToString();
        GameObject targetDiscard = majongUIPosition.transform.Find(targetDiscardPath).gameObject;
        targetDiscard.transform.localScale = Vector3.zero;
        myDiscardList.RemoveAt(myDiscardList.Count - 1);
    }

    #endregion
    public Sprite GetSpriteFromStr(string picStr)
    {
        foreach (var item in DefaultSprites)
        {
            if (item.name == picStr)
            {
                return item;
            }
        }
        byte[] myByte = Convert.FromBase64String(picStr);
        Texture2D tex2D = new Texture2D(64, 64);
        tex2D.LoadImage(myByte);
        Sprite sprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }

    public void UpdataRoomDtoSeatList(SeatDto seatDto)
    {
        foreach (var item in MyRoomDto.SeatedList)
        {
            if (item.TableID == seatDto.Id && item.Id == seatDto.Id)
            {
                item.PlayName = seatDto.PlayName;
                item.State = seatDto.State;
                break;
            }
        }
    }

    public void Log(string str)
    {
        Debug.Log(str);
    }

    public void ShowRoomDtoSeatList()
    {
        foreach (var it in MyRoomDto.SeatedList)
        {
            Debug.Log(it.PlayName);
        }
    }
}
