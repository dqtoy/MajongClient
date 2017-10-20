using System.Collections.Generic;
using Common.Code;
using UnityEngine;

public  class MajongOperation{

    
    /// <summary>
    /// 向服务器发送摸牌请求
    /// </summary>
    public void Request4Getcard()
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[50] = MajongCode.Getcard;
        PhotonManager.Ins.OnOperationRequest((byte) OpCode.Majong, parameters);
    }
    /// <summary>
    /// 向服务发送出牌请求
    /// </summary>
    /// <param name="tileId"></param>
    public void Request4Discard(int tileId)
    {
        Dictionary<byte, object> parameters = new Dictionary<byte, object>();
        parameters[0] = tileId;
        parameters[50] = MajongCode.Getcard;
        PhotonManager.Ins.OnOperationRequest((byte)OpCode.Majong, parameters);
    }
    /// <summary>
    /// 吃牌检测
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="cardslist"></param>
    /// <returns></returns>
    public int[] Chow(int cardId, List<int> cardslist)
    {
        
        int[] i3 = { 0, 0, 0 };
        if (cardId > 30)
            return i3;
        if (cardslist.Contains(cardId - 2) && cardslist.Contains(cardId - 1))
            i3[0] = 1;
        if (cardslist.Contains(cardId - 1) && cardslist.Contains(cardId + 1))
            i3[1] = 1;
        if (cardslist.Contains(cardId + 1) && cardslist.Contains(cardId + 2))
            i3[2] = 1;
        return i3;
    }
    /// <summary>
    /// 对面出牌情况，判断碰 杠
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="cardslist"></param>
    /// <returns></returns>
    public int PongOrKong(int cardId, List<int> cardslist)
    {
        int sameCards = 0;
        foreach (var it in cardslist)
        {
            if (it == cardId)
                sameCards ++;
        }
        return sameCards;
    }
    /// <summary>
    /// 摸牌 判断有无杠
    /// </summary>
    /// <param name="cardsList"></param>
    /// <param name="pongList"></param>
    /// <returns></returns>
    public int KongOrAnKong(int cardId,List<int> cardsList,List<int> pongList)
    {
        int detectCode = 0;
        //遍历手牌
        int n = 0;
        foreach (var it1 in cardsList)
        {
            if (it1 == cardId)
                n++;
        }
        if (n == 3)
            detectCode = 2;
        // 遍历碰出列表
        foreach (var it2 in cardsList)
        {
            if (it2 == cardId)
            {
                detectCode = 1;
                break;
            }
        }
        return detectCode;
    }
    #region 胡牌算法
    private List<int> tmpCardsList;
    public int Hu(List<int> cardsList)
    {
        int reState = 0;

        tmpCardsList = cardsList;
        int jokerNum = getJokerNum();
        if (jokerNum == 0)
        {
            if (isHuWithoutJoker())
            {
                reState = 9;
            }
        }
        return reState;
        return 0;
    }
    /// <summary>
    /// 获取百搭数量
    /// </summary>
    /// <returns></returns>
    private int getJokerNum()
    {
        int num = 0;
        foreach (var it in tmpCardsList)
        {
            if (it == 39)
            {
                num++;
            }
        }
        tmpCardsList.RemoveRange(0, num);
        return num;
    }

    #region 无花自摸算法
    bool isHuWithoutJoker()
    {

        for (int i = 0; i < tmpCardsList.Count; i++)
        {
            List<int> tmp = new List<int>(tmpCardsList);

            List<int> fs = tmpCardsList.FindAll(delegate (int a)
            {
                return tmpCardsList[i] == a;
            });

            if (fs.Count >= 2)
            {
                tmp.Remove(tmpCardsList[i]);
                tmp.Remove(tmpCardsList[i]);
            }
            i += fs.Count - 1;

            if (isCanHu(tmp))
                return true;
        }
        return false;
    }

    bool isCanHu(List<int> cards)
    {
        if (cards.Count == 0)
        {
            return true;
        }

        List<int> fs = cards.FindAll(t => t.Equals(cards[0]));
        if (fs.Count >= 3)
        {
            cards.RemoveRange(0, 3);
            isCanHu(cards);
        }
        else
        {
            if (cards.Contains(cards[0] + 1) && cards.Contains(cards[0] + 2))
            {
                cards.Remove(cards[0] + 2);
                cards.Remove(cards[0] + 1);
                cards.Remove(cards[0]);
                return isCanHu(cards);
            }
        }
        return false;
    }
    #endregion
    #endregion
    public bool HuWithoutHun(List<int> cardList)
    {
        tmpCardsList = cardList;
        return isHuWithoutJoker();
    }
}
