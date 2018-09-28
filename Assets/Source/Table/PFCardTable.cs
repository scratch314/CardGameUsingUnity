using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public static class PFCardTable
{
    public static PFTable.ECardApplyTarget GetApplyTarget(int cardId)
    {
        PFTable.Card cardTable = PFTable.GetCard(cardId);
        if (cardTable != null)
        {
            return cardTable.applyTarget;
        }

        return PFTable.ECardApplyTarget.Invalid;
    }

    public static PFTable.ECardActiveType GetCardActiveType(int cardId)
    {
        PFTable.Card cardTable = PFTable.GetCard(cardId);
        if (cardTable != null)
        {
            return cardTable.activeType;
        }

        return PFTable.ECardActiveType.Invalid;
    }

    public static PFTable.ECardPoolType GetCardType(int cardId)
    {
        PFTable.Card cardTable = PFTable.GetCard(cardId);
        if (cardTable != null)
        {
            return cardTable.cardPoolType;
        }

        return PFTable.ECardPoolType.Invalid;
    }

    public static string GetStatusIconPath(PFTable.Card.StatusIcon statusIconType)
    {
        if (statusIconType != PFTable.Card.StatusIcon.None)
        {
            StringBuilder sb = new StringBuilder("");
            sb.AppendFormat("StatusIcon/{0}", statusIconType.ToString());
            string strResult = sb.ToString();
            return strResult;
        }

        return null;
    }

    public static int GetCost(int cardTableId)
    {
        PFTable.Card cardTable = PFTable.GetCard(cardTableId);
        if (cardTable != null)
        {
            return cardTable.cost;
        }

        return PFNumber.InvalidCost;
    }
}
