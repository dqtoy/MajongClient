using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIEffection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

    void Start()
    { }
    void Update()
    { }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(1.2f, 0.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1.0f, 0.3f);
    }
}
