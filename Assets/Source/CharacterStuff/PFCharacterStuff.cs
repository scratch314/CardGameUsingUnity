using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PFCharacterStuff
{
    public static void Active_CharSelectIconUI(List<APFCharacter> charList, bool bActive)
    {
        if (charList != null)
        {
            for (int i = 0; i < charList.Count; ++i)
            {
                charList[i].SelectIcon_UI.SetActive(bActive);
            }
        }
    }

    public static bool IsEnemy(APFCharacter char1, APFCharacter char2)
    {
        if (char1 != null && 
            char2 != null)
        {
            PFTable.CharacterType charType_char1 = PFCharacterTable.GetCharacterType(char1.CharacterTableId);
            PFTable.CharacterType charType_char2 = PFCharacterTable.GetCharacterType(char2.CharacterTableId);

            bool bResult = (charType_char1 != charType_char2);
            return bResult;
        }

        return false;
    }

    public static int CalcMaxHP(int charTableId)
    {
        int tableMaxHp = PFCharacterTable.GetMaxHP(charTableId);
        float tableMaxHpRand = PFCharacterTable.GetMaxHPRand(charTableId);

        float randMin = 1.0f;
        if (tableMaxHpRand < randMin)
        {
            tableMaxHpRand = randMin;
        }

        float rand = Random.Range(randMin, tableMaxHpRand);
        float newMaxHp = tableMaxHp * rand;

        //소수점이 발생하면 무조건 올림
        float newMaxHp_Ceil = Mathf.Ceil(newMaxHp);

        int maxHP = (int)newMaxHp_Ceil;

        return maxHP;
    }
}
