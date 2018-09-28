using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PFCardEffectTable
{
    public static PFTable.CardEffect.EType GetCardEffectType(int cardEffectId)
    {
        PFTable.CardEffect cardEffect = PFTable.GetCardEffect(cardEffectId);
        if (cardEffect != null)
        {
            return cardEffect.CardEffectType;
        }

        return PFTable.CardEffect.EType.Invalid;
    }
}
