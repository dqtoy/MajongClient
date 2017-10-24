using System.Collections;
using System.Collections.Generic;
using Common.Code;
using UnityEngine;
using UnityEngine.UI;

public class MajongCardsController : MonoBehaviour
{
    public List<int> MyMjongCards;

    //显示手牌
    public Transform ExtraCard;
    public Transform ExtraCardBack;
    public Transform[] Cards;
    public Transform[] CardsBack;

    //显示出牌
    public Transform MyFirstDiscard;//己方第一个出牌的位置
    public Transform ItFirstDiscard;//对方第一个出牌的位置

    private float xOffset = 45.0f;
    private float yOffset = 55.0f;
    private int nMyDiscard ,nItDiscard= 0;//出牌数量
    private GameObject lastDiscard;

    #region public fuction
    /// <summary>
    /// 显示手牌
    /// </summary>
    /// <param name="cards"></param>
    public void ShowMajongCards()
    {
        MyMjongCards.Sort((x, y) => -x.CompareTo(y));
        for (int i = 0; i < 13; i++)
        {
            GameObject obj = transform.Find("MyMajongCards/FrontCard"+i).gameObject;
            if (i < MyMjongCards.Count)
            {
                Sprite tileSprite = getSpriteByTileId(MyMjongCards[i]);
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
            MyMjongCards.Add(tileId);
        }
    }

    public void ShowDiscard(int reCode,int id)
    {
        Debug.Log("出牌Code:" + reCode);
        if (reCode == 0)
        {
            Vector3 myDiscardPos = MyFirstDiscard.position + new Vector3(xOffset*(nMyDiscard%20), -yOffset*(int)(nMyDiscard/20), 0);
            lastDiscard = Instantiate(Resources.Load("Majong/UpCard"), myDiscardPos, Quaternion.identity,MyFirstDiscard.parent) as GameObject;
            lastDiscard.transform.Find("Tile").GetComponent<Image>().sprite = getSpriteByTileId(id);
            lastDiscard.transform.position = MyFirstDiscard.position+new Vector3(xOffset * (int)(nMyDiscard%20),-yOffset*(int)(nMyDiscard / 20),0);
            ExtraCard.localScale = Vector3.zero;

            MyMjongCards.Remove(id);
            ShowMajongCards();
            nMyDiscard ++;
        }
        else
        {
            Vector3 itDiscardPos = ItFirstDiscard.position + new Vector3(-xOffset*(nItDiscard%20), yOffset*(int)(nItDiscard/20), 0);
            lastDiscard = Instantiate(Resources.Load("Majong/UpCard"), itDiscardPos, Quaternion.identity,ItFirstDiscard.parent) as GameObject;
            lastDiscard.transform.SetSiblingIndex(0);
            lastDiscard.transform.Find("Tile").GetComponent<Image>().sprite = getSpriteByTileId(id);
            ExtraCardBack.localScale = Vector3.zero;

            nItDiscard ++;
            //吃碰杠胡检测
            Dictionary<byte, object> parameters = new Dictionary<byte, object>();
            parameters[50] = MajongCode.Getcard;
            PhotonManager.Ins.OnOperationRequest((byte) OpCode.Majong, parameters);
        }
    }
    #endregion

    #region private fuction
    /// <summary>
    /// 根据id获取麻将图片
    /// </summary>
    /// <param name="tileId"></param>
    /// <returns></returns>
    Sprite getSpriteByTileId(int tileId)
    {
        //加载图片转化为sprite
        Texture2D tex = (Texture2D)Resources.Load("Majong/MajongTile/" + tileId.ToString("00"));
        Sprite tileSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        tileSprite.name = tileId.ToString();
        return tileSprite;
    }
    #endregion
}
