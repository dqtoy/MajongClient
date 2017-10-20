using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card {
    public int ID;
    public Suit suit;
    public Weight weight;

    public Card(int id,Suit s,Weight w)
    {
        this.ID = id;
        this.suit = s;
        this.weight = w;
    }
}
