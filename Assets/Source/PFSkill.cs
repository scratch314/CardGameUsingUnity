using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PFSkill
{
    public static int GetDamage(APFCharacterController attackerCharCtrl, int cardTableId)
    {
        if (attackerCharCtrl != null)
        {
            int characterTableId = attackerCharCtrl.CharacterTableId;

            PFTable.Character charTable = PFTable.GetCharacter(characterTableId);
            PFTable.Card cardTable = PFTable.GetCard(cardTableId);

            if (charTable != null &&
                cardTable != null)
            {
                float atkRateSum = attackerCharCtrl.GetCardEffectSum(ECardEffect.AtkRate);

                float atkRatio = charTable.atkRatio + atkRateSum;

                float fDamage = atkRatio * cardTable.damage;
                int iDamage = (int)fDamage;//소수점이 발생하면 버린다.
                return iDamage;
            }
        }

        return 0;
    }
}
