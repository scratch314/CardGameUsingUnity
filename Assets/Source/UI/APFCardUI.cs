using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class APFCardUI : MonoBehaviour
{
    [SerializeField]
    private UILabel SkillName;

    [SerializeField]
    private UILabel Cost;

    [SerializeField]
    private UILabel Description;

    [SerializeField]
    private UI2DSprite SkillImage;


    public Action<APFCardUI> OnClickThis_Dele;


    private PFTable.Card CardTable;


    public int GetCardWidth()
    {
        return 400;
    }

    public int GetCardHeight()
    {
        return 570;
    }

    public int GetCost()
    {
        if (CardTable != null)
        {
            return CardTable.cost;
        }

        return PFNumber.InvalidCost;
    }

    public void OnClickThis()
    {
        OnClickThis_Dele(this);
    }

    public PFTable.Card GetTable()
    {
        return CardTable;
    }

    public void Init(PFTable.Card cardTable)
    {
        if (cardTable == null)
        {
            return;
        }

        CardTable = cardTable;

        UnityEngine.Sprite skillImg_sprite = Resources.Load<UnityEngine.Sprite>(CardTable.skillImgPath);

        SkillName.text = CardTable.displayName_KR;
        Description.text = CardTable.displayDesc;
        Cost.text = CardTable.cost.ToString();
        SkillImage.sprite2D = skillImg_sprite;
    }

    public int CardTableId
    {
        get
        {
            return CardTable.uid;
        }
    }
}
