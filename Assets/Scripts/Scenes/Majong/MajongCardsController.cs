using Common.Code;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MajongCardsController : MonoBehaviour
{
    public static MajongCardsController ins;
    public List<int> MyMajongCards;

    //显示手牌
    public Transform ExtraCard;
    public Transform ExtraCardBack;
    public Transform[] Cards;
    public Transform[] CardsBack;

    //显示出牌
    public Transform MyFirstDiscard;//己方第一个出牌的位置
    public Transform ItFirstDiscard;//对方第一个出牌的位置

    private MajongDetect majongDetect = new MajongDetect();

    private float xOffset = 30.0f;
    private float yOffset = 42.0f;
    private int nMyDiscard ,nItDiscard= 0;//出牌数量
    private GameObject lastDiscard;
    private int myOpCount, itOpCount = 0;

    private Transform operatedCards;
    private Transform basicOperation;
    private Transform huTransform;
    private Transform huMask;
    private Transform huBtn;
    private Transform winMsg;

    private List<GameObject> tempGos = new List<GameObject>();
    private List<GameObject> tempWinGos = new List<GameObject>();

    private List<int> pongList = new List<int>();
    private bool isKong = false;
    private void Awake()
    {
        ins = this;


        operatedCards = transform.Find("OperatedCards");
        basicOperation = transform.Find("BasicOperation");
        huTransform = transform.Find("Hu");
        huMask = huTransform.Find("HuMask");
        huBtn = huTransform.Find("HuBtn");
        winMsg = huTransform.Find("WinMsg");
        
    }

    private void Start()
    {
        
    }
    #region public fuction

    public void SetMyMajongCards(List<int> cards)
    {
        MyMajongCards = new List<int>(cards);
        debugMajongCards();
    }
    /// <summary>
    /// 显示手牌
    /// </summary>
    /// <param name="cards"></param>
    public void ShowMajongCards()
    {
        MyMajongCards.Sort((x, y) => -x.CompareTo(y));
        
        for (int i = 0; i < 13; i++)
        {
            GameObject obj = transform.Find("MyMajongCards/FrontCard"+i).gameObject;
            if (i < MyMajongCards.Count)
            {
                Sprite tileSprite = getSpriteByTileId(MyMajongCards[i]);
                obj.transform.localScale = Vector3.one;
                obj.transform.Find("Tile").GetComponent<Image>().sprite = tileSprite;
            }
            else
            {
                obj.transform.localScale = Vector3.zero;
            }
        }
    }
    /// <summary>
    /// 显示摸的牌
    /// </summary>
    /// <param name="tileId"></param>
    public void ShowExtraCard(int tileId)
    {
        if (tileId == -1)
        {
            ExtraCardBack.localScale = Vector3.one;
        }
        else
        {
            ExtraCard.Find("Tile").GetComponent<Image>().sprite = getSpriteByTileId(tileId);
            ExtraCard.localScale = Vector3.one;
            if (majongDetect.isHuWithoutJoker(MyMajongCards, tileId))
            {
                huMask.localScale = Vector3.one;
                huBtn.localScale = Vector3.one;
                return;
            }
            MyMajongCards.Add(tileId);
            List<int> kongList = majongDetect.isKongWhenGetCard(MyMajongCards, pongList);
            if (kongList.Count != 0)
            {
                showMajongKongUI(kongList);
                isKong = true;
            }
        }
    }
    
    public void ShowDiscard(int reCode,int id)
    {
        
        if (reCode == 0)
        {
            Vector3 myDiscardPos = MyFirstDiscard.position + new Vector3(xOffset*(nMyDiscard%20), -yOffset*(int)(nMyDiscard/20), 0);
            lastDiscard = Instantiate(Resources.Load("Majong/UpCard"), myDiscardPos, Quaternion.identity,MyFirstDiscard.parent) as GameObject;
            lastDiscard.transform.Find("Tile").GetComponent<Image>().sprite = getSpriteByTileId(id);
            tempGos.Add(lastDiscard);
            //lastDiscard.transform.position = MyFirstDiscard.position+new Vector3(xOffset * (int)(nMyDiscard%20),-yOffset*(int)(nMyDiscard / 20),0);
            ExtraCard.localScale = Vector3.zero;
            MajongSoundController.ins.PlayMajongSound("b_" + id);

            MyMajongCards.Remove(id);
            ShowMajongCards();
            nMyDiscard ++;

            if (isKong)
            {
                hideMajongOperationUI();
                isKong = false;
            }
                
        }
        else
        {
            Vector3 itDiscardPos = ItFirstDiscard.position + new Vector3(-xOffset*(nItDiscard%20), yOffset*(int)(nItDiscard/20), 0);
            lastDiscard = Instantiate(Resources.Load("Majong/UpCard"), itDiscardPos, Quaternion.identity,ItFirstDiscard.parent) as GameObject;
            lastDiscard.transform.SetSiblingIndex(0);
            lastDiscard.transform.Find("Tile").GetComponent<Image>().sprite = getSpriteByTileId(id);
            tempGos.Add(lastDiscard);
            MajongSoundController.ins.PlayMajongSound("g_" + id);

            int reNum = 13 - itOpCount * 3;
            updateItMajongCards(reNum);

            nItDiscard ++;
            //吃碰杠胡检测
            if (majongDetect.isHuWithoutJoker(MyMajongCards, id))
            {
                huMask.localScale = Vector3.one;
                huBtn.localScale = Vector3.one;
                return;
            }
            List<byte> opList = majongDetect.Detect(MyMajongCards,id);
            if (opList.Count == 0)
            {
                getCard();
            }
            else
            {
                //显示可操作UI
                showMajongOperationUI(id, opList);
            }
            
        }
    }

    public void ShowOperatedCards(int reCode,int id, byte code)
    {
        bool isSingleKong = false;
        if (code == (byte) MajongOpCode.Kong)
        {
            if (pongList.Contains(id))
            {
                isSingleKong = true;
            }
        }
        if (code == (byte) MajongOpCode.Pong)
        {
            pongList.Add(id);
        }
        int soundId = 0;
        if (reCode == 0)
        {
            if (isSingleKong)
            {
                for (int i = 0; i < myOpCount; i++)
                {
                    Transform myTs = transform.Find("OperatedCards/myOpCards" + i);
                    if (myTs.GetComponent<FourUpCards>().PongToKong(id))
                    {
                        break;
                    }
                }
                soundId = 103;
                myOpCount--;
            }
            else
            {
                Transform myTs = transform.Find("OperatedCards/myOpCards" + myOpCount);
                soundId = myTs.GetComponent<FourUpCards>().ShowCards(id, code);
                myTs.localScale = 0.8f * Vector3.one;
            }
            
            MajongSoundController.ins.PlayMajongSound("b_" + soundId);

            myOpCount++;
            //删除对手出牌
            nItDiscard--;
            tempGos.Remove(lastDiscard);
            Destroy(lastDiscard);

            hideMajongOperationUI();
            updateMyMajiongCards(id, code);
            if (code == (byte) MajongOpCode.Kong)
            {
                Dictionary<byte, object> parameters = new Dictionary<byte, object>();
                parameters[50] = MajongCode.Getcard;
                PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
            }
        }
        else
        {
            if (isSingleKong)
            {
                for (int i = 0; i < itOpCount; i++)
                {
                    Transform itTs = transform.Find("OperatedCards/itOpCards" + i);
                    if (itTs.GetComponent<FourUpCards>().PongToKong(id))
                    {
                        break;
                    }
                }

                soundId = 103;
                itOpCount--;
            }
            else
            {
                Transform itTs = transform.Find("OperatedCards/itOpCards" + itOpCount);
                soundId = itTs.GetComponent<FourUpCards>().ShowCards(id, code);
                itTs.localScale = 0.8f * Vector3.one;
            }
            MajongSoundController.ins.PlayMajongSound("g_" + soundId);
            itOpCount++;

            int reNum = 13 - 3 * itOpCount;
            if (code == (byte) MajongOpCode.Kong)
            {
                reNum -= 3;
            }
            else
            {
                reNum -= 2;
            }

            nMyDiscard--;
            tempGos.Remove(lastDiscard);
            Destroy(lastDiscard);
            updateItMajongCards(reNum);
            
        }
    }


    public void ShowGameOverUI(int reCode,List<int> winMajiongCards)
    {
        huBtn.localScale = Vector3.zero;
        winMsg.localScale = Vector3.one;
        Transform pos0 = winMsg.Find("msg/pos0");
        if (reCode == 0)
        {
            MajongSoundController.ins.PlayMajongSound("b_" + 104);
            if (myOpCount != 0)
            {
                for (int i = 0; i < myOpCount; i++)
                {
                    GameObject myOpCard = operatedCards.Find("myOpCards" + i).gameObject;
                    Vector3 pos = pos0.position + new Vector3(xOffset * (3 * i + 1), 0, 0);
                    GameObject go = Instantiate(myOpCard, pos, Quaternion.identity,pos0.parent);
                    go.transform.localScale = 0.6f * Vector3.one;
                    tempWinGos.Add(go);
                }
            }

            for (int j = 0; j < winMajiongCards.Count; j++)
            {
                Vector3 singlePos = pos0.position + new Vector3(xOffset * (3 * myOpCount + j), 0, 0);
                GameObject singleGo = getUpCardById(winMajiongCards[j]);
                singleGo.transform.position = singlePos;
                singleGo.transform.parent = pos0.parent;
                tempWinGos.Add(singleGo);
            }

            winMsg.Find("banner/info").GetComponent<Image>().sprite = getSpriteByPath("Majong/Texture/win_str");

        }
        else
        {
            MajongSoundController.ins.PlayMajongSound("g_" + 104);
            if (itOpCount != 0)
            {
                for (int k = 0; k < itOpCount; k++)
                {
                    GameObject itOpCard = operatedCards.Find("itOpCards" + k).gameObject;
                    Vector3 pos = pos0.position + new Vector3(xOffset * (3 * k + 1), 0, 0);
                    GameObject go = Instantiate(itOpCard, pos, Quaternion.identity, pos0.parent);
                    go.transform.localScale = 0.6f * Vector3.one;
                    tempWinGos.Add(go);
                }
            }

            
            for (int l = 0; l < winMajiongCards.Count; l++)
            {
                Vector3 singlePos = pos0.position + new Vector3(xOffset * (3 * itOpCount + l), 0, 0);
                GameObject singleGo = getUpCardById(winMajiongCards[l]);
                singleGo.transform.position = singlePos;
                singleGo.transform.parent = pos0.parent;
                tempWinGos.Add(singleGo);
            }
            winMsg.Find("banner/info").GetComponent<Image>().sprite = getSpriteByPath("Majong/Texture/failed_str");
        }
    }
    /// <summary>
    /// 根据id获取麻将图片
    /// </summary>
    /// <param name="tileId"></param>
    /// <returns></returns>
    public Sprite getSpriteByTileId(int tileId)
    {
        //加载图片转化为sprite
        Texture2D tex = (Texture2D)Resources.Load("Majong/MajongTile/" + tileId.ToString("00"));
        Sprite tileSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        tileSprite.name = tileId.ToString();
        return tileSprite;
    }

    public void Pass()
    {
        hideMajongOperationUI();
        getCard();
    }
    public void Hu()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[0] = LitJson.JsonMapper.ToJson(MyMajongCards);
        parameters[1] = MajongManager.ins.GetTableId().ToString();
        parameters[50] = MajongCode.End;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }

    /// <summary>
    /// 游戏重新开始 数据恢复
    /// </summary>
    public void ResetUnPlayMajong()
    {
        huMask.localScale = Vector3.zero;
        winMsg.localScale = Vector3.zero;
        foreach (var item in tempWinGos)
        {
            Destroy(item);
        }
        foreach (var item2 in tempGos)
        {
            Destroy(item2);
        }
        for (int i = 0; i < myOpCount; i++)
        {
            transform.Find("OperatedCards/myOpCards" + i).localScale = Vector3.zero;
        }
        for (int j = 0; j < itOpCount; j++)
        {
            transform.Find("OperatedCards/itOpCards" + j).localScale = Vector3.zero;
        }
        updateItMajongCards(13);
        tempGos = new List<GameObject>();
        tempWinGos = new List<GameObject>();
        pongList = new List<int>();
        nItDiscard = 0;
        nMyDiscard = 0;
        myOpCount = 0;
        itOpCount = 0;
        transform.localScale = Vector3.zero;
        transform.parent.Find("ReadyBtn").localScale = Vector3.one;
    }
    #endregion

    #region private fuction

    void showMajongOperationUI(int id,List<byte> opList)
    {
        basicOperation.Find("Pass").localScale = Vector3.one;
        for (int i = 0; i < opList.Count; i++)
        {
            Transform ts = basicOperation.Find("Op" + (i + 1));
            ts.GetComponent<FourUpCards>().ShowCards(id, opList[i]);
            ts.localScale = 0.5f*Vector3.one;
        }
    }

    void showMajongKongUI(List<int> kongList)
    {
        for (int i = 0; i < kongList.Count; i++)
        {
            Transform ts = basicOperation.Find("Op" + (i + 1));
            ts.GetComponent<FourUpCards>().ShowCards(kongList[i], (byte)MajongOpCode.Kong);
            ts.localScale = 0.5f * Vector3.one;
        }
    }

    void hideMajongOperationUI()
    {
        basicOperation.Find("Pass").localScale = Vector3.zero;
        for (int i = 0; i < 5; i++)
        {
            Transform ts = basicOperation.Find("Op" + i);
            ts.localScale = Vector3.zero;
        }
    }

    void updateMyMajiongCards(int id, byte code)
    {
        switch (code)
        {
            case (byte)MajongOpCode.Chow_L:
                MyMajongCards.Remove(id - 2);
                MyMajongCards.Remove(id - 1);
                break;
            case (byte)MajongOpCode.Chow_M:
                MyMajongCards.Remove(id - 1);
                MyMajongCards.Remove(id + 1);
                break;
            case (byte)MajongOpCode.Chow_R:
                MyMajongCards.Remove(id + 1);
                MyMajongCards.Remove(id + 2);
                break;
            case (byte)MajongOpCode.Pong:
                MyMajongCards.Remove(id);
                MyMajongCards.Remove(id);
                break;
            case (byte)MajongOpCode.Kong:
                MyMajongCards.Remove(id);
                MyMajongCards.Remove(id);
                MyMajongCards.Remove(id);
                break;
        }
        ShowMajongCards();
    }

    void updateItMajongCards(int reNum)
    {
        for (int i = 0; i < CardsBack.Length; i++)
        {
            if (i < reNum)
            {
                CardsBack[i].localScale = Vector3.one;
            }
            else
            {
                CardsBack[i].localScale = Vector3.zero;
            }
        }

        ExtraCardBack.localScale = Vector3.zero;
    }


    

 
    /// <summary>
    /// 向服务器发送摸牌消息
    /// </summary>
    void getCard()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = MajongCode.Getcard;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }

    
    GameObject getUpCardById(int id)
    {
        GameObject go = Instantiate(Resources.Load("Majong/UpCard")) as GameObject;
        go.transform.Find("Tile").GetComponent<Image>().sprite = getSpriteByTileId(id);
        return go;
    }

    Sprite getSpriteByPath(string path)
    {
        Texture2D tex = Resources.Load(path) as Texture2D;
        Sprite reSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), 0.5f * Vector2.one);
        return reSprite;

    }

    void debugMajongCards()
    {
        List<int> ls = new List<int>(MyMajongCards);
        ls.Sort();
        string str = "";
        foreach (var item in MyMajongCards)
        {
            str += item+"  ";
        }
        Debug.Log(str);
    }
    #endregion
}
