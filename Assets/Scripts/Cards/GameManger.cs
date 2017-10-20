using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour {
    public List<Card> neatCards = new List<Card>();
    public List<Card> mixedCards = new List<Card>();

    public List<Card> player1Cards = new List<Card>();
    public List<Card> player2Cards = new List<Card>();
    public List<Card> player3Cards = new List<Card>();

    void Start() {
        CreateCards();
        WashCards();
        DealCards();
        test();
    }

    void test()
    {
        string str = "";
        foreach(Card cd in player1Cards)
        {
            str += cd.ID + "\t";
        }
        print(str);
    }
    void Update() {

    }
    /// <summary>
    /// 构造一幅新牌
    /// </summary>
    private void CreateCards()
    {
        int n = 0;
        for (int col = 0; col < 4; col++)
        {
            for (int wt = 0; wt < 13; wt++)
            {
                Suit s = (Suit)col;
                Weight w = (Weight)wt;
                Card card = new Card(n, s, w);
                neatCards.Add(card);
                n++;
            }
        }
        neatCards.Add(new Card(n + 1, Suit.None, Weight.SJoke));
        neatCards.Add(new Card(n + 2, Suit.None, Weight.LJoke));
    }
    /// <summary>
    /// 洗牌
    /// </summary>
    System.Random random = new System.Random();
    private void WashCards()
    {
        mixedCards.Clear();
        foreach(Card card in neatCards)
        {
            mixedCards.Insert(random.Next(mixedCards.Count + 1), card);
        }
    }

    private void DealCards()
    {
        if (mixedCards.Count == 0)
            return;
        Debug.Log(mixedCards.Count);
        for (int i =0;i<3;i++)
        {
            List<Card> list = new List<Card>();
            switch(i)
            {
                case 0:
                    list = player1Cards;
                    break;
                case 1:
                    list = player2Cards;
                    break;
                case 2:
                    list = player3Cards;
                    break;
            }
            Debug.Log(list);

            for (int j = 0;j<17;j++)
            {
                Card card = mixedCards[mixedCards.Count-1];
                list.Add(card);
                mixedCards.Remove(card);
            }
        }
    }
}
