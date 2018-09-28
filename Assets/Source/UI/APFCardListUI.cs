using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APFCardListUI : MonoBehaviour
{
    [SerializeField]
    private UIGrid CardListGrid;

    [SerializeField]
    public UILabel Title_UILabel;


    public void OnClick_CloseWindow()
    {
        gameObject.SetActive(false);
    }

    public void UpdateUI_CardList(List<int> cardList)
    {
        PFUtil.DestroyChild(CardListGrid.gameObject, false);

        if (cardList == null)
        {
            return;
        }

        for (int i = 0; i < cardList.Count; ++i)
        {
            APFCardUI pfCard = PFUtil.Instantiate<APFCardUI>(PFPrefabPath.Card, CardListGrid.gameObject, Vector3.zero);
            pfCard.Init(PFTable.GetCard(cardList[i]));
        }

        CardListGrid.repositionNow = true;
    }
}
