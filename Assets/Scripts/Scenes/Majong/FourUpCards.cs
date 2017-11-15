using System.Collections;
using System.Collections.Generic;
using Common.Code;
using UnityEngine;
using UnityEngine.UI;

public class FourUpCards :MonoBehaviour
{
    private int myTile;
    private byte myOpCode;

    private Transform card1;
    private Transform card2;
    private Transform card3;
    private Transform card4;
    private Image tile1;
    private Image tile2;
    private Image tile3;
    private Image tile4;

    private void Start()
    {
        card1 = transform.Find("UpCard0");
        tile1 = card1.Find("Tile").GetComponent<Image>();

        card2 = transform.Find("UpCard1");
        tile2 = card2.Find("Tile").GetComponent<Image>();

        card3 = transform.Find("UpCard2");
        tile3 = card3.Find("Tile").GetComponent<Image>();

        card4 = transform.Find("UpCard3");
        tile4 = card4.Find("Tile").GetComponent<Image>();
    }
    /// <summary>
    /// 根据麻将牌 和 操作类型显示卡牌
    /// </summary>
    /// <param name="id"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public int ShowCards(int id,byte code)
    {
        transform.gameObject.SetActive(true);
        myTile = id;
        myOpCode = code;

        int soundId = -1;
        switch (code)
        {
            case (byte)MajongOpCode.Chow_L:
                tile1.sprite = MajongCardsController.ins.getSpriteByTileId(id - 2);
                tile2.sprite = MajongCardsController.ins.getSpriteByTileId(id - 1);
                tile3.sprite = MajongCardsController.ins.getSpriteByTileId(id);
                card4.localScale = Vector3.zero;
                soundId = 101;
                break;
            case (byte)MajongOpCode.Chow_M:
                tile1.sprite = MajongCardsController.ins.getSpriteByTileId(id - 1);
                tile2.sprite = MajongCardsController.ins.getSpriteByTileId(id);
                tile3.sprite = MajongCardsController.ins.getSpriteByTileId(id + 1);
                card4.localScale = Vector3.zero;
                soundId = 101;
                break;
            case (byte)MajongOpCode.Chow_R:
                tile1.sprite = MajongCardsController.ins.getSpriteByTileId(id);
                tile2.sprite = MajongCardsController.ins.getSpriteByTileId(id + 1);
                tile3.sprite = MajongCardsController.ins.getSpriteByTileId(id + 2);
                card4.localScale = Vector3.zero;
                soundId = 101;
                break;
            case (byte)MajongOpCode.Pong:
                Sprite pongSprite = MajongCardsController.ins.getSpriteByTileId(id);
                tile1.sprite = tile2.sprite = tile3.sprite = pongSprite;
                card4.localScale = Vector3.zero;
                soundId = 102;
                break;
            case (byte)MajongOpCode.Kong:
                Sprite kongSprite = MajongCardsController.ins.getSpriteByTileId(id);
                tile1.sprite = tile2.sprite = tile3.sprite = tile4.sprite = kongSprite;
                card4.localScale = Vector3.one;
                soundId = 103;
                break;
        }
        return soundId;
    }
    /// <summary>
    /// 触发吃碰杠操作
    /// </summary>
    public void SelfClick()
    {
        Debug.Log("SelfClick");
        Dictionary<byte,object> parameters = new Dictionary<byte, object>();
        parameters[0] = myTile;
        parameters[1] = myOpCode;
        parameters[50] = MajongCode.Operate;
        PhotonManager.Ins.OnOperationRequest((byte) OpCode.Majong, parameters);
    }
    /// <summary>
    /// 外部调用，碰变成杠
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool PongToKong(int id)
    {
        if (id == myTile && myOpCode == (byte)MajongOpCode.Pong)
        {
            Sprite kongSprite = MajongCardsController.ins.getSpriteByTileId(id);
            tile4.sprite = kongSprite;
            card4.localScale = Vector3.one;
            return true;
        }
        return false;
    }
    
}
