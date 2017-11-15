using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MajongDetect  {
    public bool isHu()
    {
        return false;
    }
    /// <summary>
    /// 吃碰杠的监测
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="singleCard"></param>
    /// <returns></returns>
    public List<byte> Detect(List<int> cards, int singleCard)
    {
        List<byte> opList = new List<byte>();

        if (singleCard < 30)
        {
            
            
            if (cards.Contains(singleCard + 1) && cards.Contains(singleCard + 2))
                opList.Add((byte)MajongOpCode.Chow_R);
            if (cards.Contains(singleCard - 1) && cards.Contains(singleCard + 1))
                opList.Add((byte)MajongOpCode.Chow_M);
            if (cards.Contains(singleCard - 2) && cards.Contains(singleCard - 1))
                opList.Add((byte) MajongOpCode.Chow_L);
            if (cards.FindAll(x => x == singleCard).Count == 2)
            {
                opList.Add((byte)MajongOpCode.Pong);
            }
            if (cards.FindAll(x => x == singleCard).Count == 3)
            {
                opList.Add((byte)MajongOpCode.Pong);
                opList.Add((byte)MajongOpCode.Kong);
            }
        }
        return opList;
    }
    /// <summary>
    /// 没有百搭的胡牌规则
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool isHuWithoutJoker(List<int> cards, int id)
    {
        List<int> mCards = new List<int>(cards);
        mCards.Add(id);
        mCards.Sort();

        for (int i = 0; i < mCards.Count; i++)
        {
            List<int> tmp = new List<int>(mCards);

            List<int> fs = mCards.FindAll(delegate (int a)
            {
                return mCards[i] == a;
            });

            if (fs.Count >= 2)
            {
                tmp.Remove(mCards[i]);
                tmp.Remove(mCards[i]);
            }
            i += fs.Count - 1;

            if (isCanHu(tmp))
            {
                cards.Add(id);
                return true;
            }
        }
        return false;
    }


    public List<int> isKongWhenGetCard(List<int> cards,List<int> pongList)
    {
        List<int> kongList = new List<int>();
        List<int> restCards = new List<int>(cards);
        for (int i = 0; i < restCards.Count; i++)
        {
            List<int> ls = restCards.FindAll(x => x == restCards[i]);

            if (ls.Count == 1)
            {
                for (int j = 0; j < pongList.Count; j++)
                {
                    if (restCards[i] == pongList[j])
                    {
                        kongList.Add(restCards[i]);
                        break;
                    }
                }
            }else if (ls.Count == 4)
            {
                kongList.Add(restCards[i]);
            }

            i += ls.Count - 1;
        }
        return kongList;
    } 

    bool isCanHu(List<int> mCards)
    {
        if (mCards.Count == 0)
        {
            return true;
        }

        List<int> fs = mCards.FindAll(t => t.Equals(mCards[0]));
        if (fs.Count >= 3)
        {
            mCards.RemoveRange(0, 3);
            return isCanHu(mCards);
        }
        else
        {
            if (mCards.Contains(mCards[0] + 1) && mCards.Contains(mCards[0] + 2))
            {
                mCards.Remove(mCards[0] + 2);
                mCards.Remove(mCards[0] + 1);
                mCards.Remove(mCards[0]);
                return isCanHu(mCards);
            }
        }
        return false;
    }
}
