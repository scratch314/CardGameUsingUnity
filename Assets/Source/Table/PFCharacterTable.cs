using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PFCharacterTable
{
    public static PFTable.CharacterType GetCharacterType(int characterTableId)
    {
        PFTable.Character charTable = PFTable.GetCharacter(characterTableId);
        if (charTable != null)
        {
            return charTable.characterType;
        }

        return PFTable.CharacterType.Invalid;
    }

    public static string GetCharacterPrefabPath(int characterTableId)
    {
        PFTable.Character charTable = PFTable.GetCharacter(characterTableId);
        if (charTable != null)
        {
            string path = charTable.CharacterPrefabPath;
            return path;
        }

        return null;
    }

    public static int GetMaxHP(int characterTableId)
    {
        PFTable.Character charTable = PFTable.GetCharacter(characterTableId);
        if (charTable != null)
        {
            int maxHp = charTable.maxHp;
            return maxHp;
        }

        return 0;
    }

    public static float GetMaxHPRand(int characterTableId)
    {
        PFTable.Character charTable = PFTable.GetCharacter(characterTableId);
        if (charTable != null)
        {
            float maxHpRand = charTable.maxHpRand;
            return maxHpRand;
        }

        return 0.0f;
    }
}
