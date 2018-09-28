using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public static class PFPlayerStuff
{
    public static void RemoveTargetComponent(List<int> target, List<int> source)
    {
        if (target != null && source != null)
        {
            for (int i = 0; i < source.Count; ++i)
            {
                int deckCardId = source[i];

                int findIdx = target.FindIndex(comp =>
                {
                    return comp == deckCardId;
                });

                if (findIdx != -1)
                {
                    target.RemoveAt(findIdx);
                }
            }
        }
    }

    public static APFPlayerController GetPlayerController(List<APFCharacterController> charControllerList)
    {
        if (charControllerList != null)
        {
            for (int i = 0; i < charControllerList.Count; ++i)
            {
                PFTable.CharacterType charType = PFCharacterTable.GetCharacterType(charControllerList[i].CharacterTableId);
                if (charType == PFTable.CharacterType.Player)
                {
                    return charControllerList[i] as APFPlayerController;
                }
            }
        }

        return null;
    }

    public static List<APFCardUI> Create_DeckCards(GameObject cardParent, List<int> deckCardIdList, int bgFrameWidth)
    {
        APFCardUI card_original = Resources.Load<APFCardUI>(PFPrefabPath.Card);
        if (card_original == null)
        {
            StringBuilder sbLog = new StringBuilder("");
            sbLog.AppendFormat("{0} 경로에 카드프리팹이 없습니다.", PFPrefabPath.Card);
            Debug.LogError(sbLog.ToString());
            return null;
        }

        int cardWidth = card_original.GetCardWidth();
        int cardCount = deckCardIdList.Count;
        int cardTotalWidth = cardWidth * cardCount;

        float createPosX = (bgFrameWidth - cardTotalWidth) / 2;
        createPosX += cardWidth / 2;

        List<APFCardUI> newInstanceList = new List<APFCardUI>(cardCount);

        for (int i = 0; i < cardCount; ++i)
        {
            int cardId = deckCardIdList[i];
            PFTable.Card cardTable = PFTable.GetCard(cardId);

            if (cardTable != null)
            {
                Vector3 createPos = new Vector3(createPosX, 0.0f, 0.0f);

                APFCardUI newCardInstance = PFUtil.Instantiate(card_original, cardParent, createPos);

                newCardInstance.Init(cardTable);

                newInstanceList.Add(newCardInstance);

                createPosX += cardWidth;
            }
            else
            {
                StringBuilder sbLog = new StringBuilder("");
                sbLog.AppendFormat("카드테이블 id: {0} 카드정보가 없습니다", cardId);
                Debug.LogError(sbLog.ToString());
                return null;
            }
        }

        return newInstanceList;
    }

    public static List<int> GetInitTotalCardList()
    {
        List<int> totalCardList = new List<int>();

        totalCardList.Add(1001);
        totalCardList.Add(1001);
        totalCardList.Add(1001);
        totalCardList.Add(1001);
        totalCardList.Add(2001);

        /*
        //평타카드3장
        totalCardList.Add(1001);
        totalCardList.Add(1001);
        totalCardList.Add(1001);

        //방어카드5장
        totalCardList.Add(1004);
        totalCardList.Add(1004);
        totalCardList.Add(1004);
        totalCardList.Add(1004);
        totalCardList.Add(1004);

        //나머지카드 각 1장씩
        totalCardList.Add(1002);
        totalCardList.Add(1003);
        totalCardList.Add(1005);
        totalCardList.Add(1006);
        */

        return totalCardList;
    }
}
