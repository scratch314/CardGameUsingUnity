using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PFGameModeStuff
{
    public static APFCharacterController GetNextTurnCharacter(List<APFCharacterController> targetList, APFCharacterController currTurnChar)
    {
        if (targetList != null && currTurnChar != null)
        {
            int findIdx = targetList.FindIndex(comp =>
            {
                return comp == currTurnChar;
            });

            if (findIdx != -1)
            {
                int nextIdx = findIdx + 1;
                nextIdx = nextIdx % targetList.Count;
                APFCharacterController nextTurnChar = targetList[nextIdx];

                return nextTurnChar;
            }
        }

        return null;
    }
}
