using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public static class PFCardPool
{
    public const int DeckCard      = 0;//덱에 있는 카드
    public const int RemainCard    = 1;//남은 카드
    public const int UsedCard      = 2;//사용된 카드
    public const int DestroyedCard = 3;//파괴된 카드
    public const int Count         = 4;
    public const int Invalid       = 5;
}

public class APFPlayerController : APFCharacterController
{
    [SerializeField]
    private UILabel Cost_UILabel;

    [SerializeField]
    private GameObject DeckCardParent;

    [SerializeField]
    private GameObject TurnEndBtn;

    [SerializeField]
    private APFShowCardListBtn ShowRemainCardBtn;

    [SerializeField]
    private APFShowCardListBtn ShowUsedCardBtn;

    [SerializeField]
    private APFShowCardListBtn ShowDestroyedCardBtn;


    private APFPlayerCharacter PFPlayerCharacter;

    private APFCardUI SelectedCard_OnDeck = null;

    private List<int>[] CardPool = new List<int>[PFCardPool.Count]
    {
        new List<int>(),
        new List<int>(),
        new List<int>(),
        new List<int>()
    };

    private List<APFCardUI> DeckCardList;

    private APFCardListUI CardListUI_Remain = null;

    private APFCardListUI CardListUI_Used = null;

    private APFCardListUI CardListUI_Destroyed = null;


    public Action OnClickTurnEndBtn_Dele;


    public override void Init(int charTableId, Vector3 vPos)
    {
        base.Init(charTableId, vPos);

        CardListUI_Remain = PFUtil.Instantiate<APFCardListUI>(PFPrefabPath.CardListUI, this.gameObject, Vector3.zero);
        CardListUI_Remain.gameObject.SetActive(false);
        CardListUI_Remain.Title_UILabel.text = PFConst.StrRemainCard;

        CardListUI_Used = PFUtil.Instantiate<APFCardListUI>(PFPrefabPath.CardListUI, this.gameObject, Vector3.zero);
        CardListUI_Used.gameObject.SetActive(false);
        CardListUI_Used.Title_UILabel.text = PFConst.StrUsedCard;

        CardListUI_Destroyed = PFUtil.Instantiate<APFCardListUI>(PFPrefabPath.CardListUI, this.gameObject, Vector3.zero);
        CardListUI_Destroyed.gameObject.SetActive(false);
        CardListUI_Destroyed.Title_UILabel.text = PFConst.StrDestroyedCard;

        PFPlayerCharacter = PFCharacter as APFPlayerCharacter;

        List<int> totalCardList = PFPlayerStuff.GetInitTotalCardList();

        CardPool[PFCardPool.RemainCard] = totalCardList;

        APFGameMode gameMode = APFGameMode.GetInstance();
        gameMode.OnClick_Character_Dele += OnClick_Character;
    }

    private void RemoveCard_InDeckUI(APFCardUI cardUI)
    {
        int findIdx_DeckUI = DeckCardList.FindIndex(comp =>
        {
            return comp == cardUI;
        });

        if (findIdx_DeckUI >= 0)
        {
            GameObject.Destroy(DeckCardList[findIdx_DeckUI].gameObject);
            DeckCardList.RemoveAt(findIdx_DeckUI);
        }
    }

    private void RemoveAllCard_InDeckUI()
    {
        if (DeckCardList != null)
        {
            for (int i = DeckCardList.Count - 1; i >= 0; --i)
            {
                RemoveCard_InDeckUI(DeckCardList[i]);
            }
        }
    }

    protected void UseCard(APFCardUI cardUI, APFCharacter target_clickedChar)
    {
        base.UseCard(cardUI.GetTable(), target_clickedChar);

        DeselectDeckCard(SelectedCard_OnDeck);

        RemoveCard_InDeckUI(cardUI);

        int findIdx_DeckCardPool = CardPool[PFCardPool.DeckCard].FindIndex(comp =>
        {
            if (cardUI != null)
            {
                return comp == cardUI.CardTableId;
            }

            return false;
        });

        if (findIdx_DeckCardPool >= 0)
        {
            CardPool[PFCardPool.DeckCard].RemoveAt(findIdx_DeckCardPool);

            if (cardUI.GetTable().cardPoolType == PFTable.ECardPoolType.ToUsedPool_AfterAction)
            {
                AddComponent_ToCardPool(PFCardPool.UsedCard, cardUI.CardTableId);
            }
            else if (cardUI.GetTable().cardPoolType == PFTable.ECardPoolType.ToDestroyedPool_AfterAction)
            {
                AddComponent_ToCardPool(PFCardPool.DestroyedCard, cardUI.CardTableId);
            }
        }
    }

    private void MoveCard_InCardPool(int src_cardPoolType, int src_idx, int dst_cardPoolType)
    {
    }

    private void AddComponent_ToCardPool(int cardPoolType, int cardId)
    {
        CardPool[cardPoolType].Add(cardId);

        if (cardPoolType == PFCardPool.UsedCard)
        {
            ShowUsedCardBtn.CardCount_UILabel.text = CardPool[cardPoolType].Count.ToString();
            CardListUI_Used.UpdateUI_CardList(CardPool[cardPoolType]);
        }
        else if (cardPoolType == PFCardPool.DestroyedCard)
        {
            ShowDestroyedCardBtn.CardCount_UILabel.text = CardPool[cardPoolType].Count.ToString();
            CardListUI_Destroyed.UpdateUI_CardList(CardPool[cardPoolType]);
        }
    }

    public override void OnCharacterDead()
    {
        base.OnCharacterDead();
    }

    void Enable_DeckCard_OnClickEvent(bool bEnable)
    {
        if (DeckCardList != null)
        {
            for (int i = 0; i < DeckCardList.Count; ++i)
            {
                if (bEnable)
                {
                    DeckCardList[i].OnClickThis_Dele += OnClick_DeckCard;
                }
                else
                {
                    DeckCardList[i].OnClickThis_Dele -= OnClick_DeckCard;
                }
            }
        }
    }

    public override void OnTurnBegin()
    {
        base.OnTurnBegin();

        TurnEndBtn.SetActive(true);

        SetCost(PFConst.DefaultPlayerMaxCost);

        RemoveAllCard_InDeckUI();

        //덱카드 갱신
        CardPool[PFCardPool.DeckCard].Clear();

        if (CardPool[PFCardPool.RemainCard].Count < PFConst.InitDeckCardCount)
        {
            CardPool[PFCardPool.RemainCard].AddRange(CardPool[PFCardPool.UsedCard]);

            CardPool[PFCardPool.UsedCard].Clear();
            ShowUsedCardBtn.CardCount_UILabel.text = CardPool[PFCardPool.UsedCard].Count.ToString();
            CardListUI_Used.UpdateUI_CardList(CardPool[PFCardPool.UsedCard]);
        }

        CardPool[PFCardPool.DeckCard] = PFUtil.GetRandomList(CardPool[PFCardPool.RemainCard], PFConst.InitDeckCardCount, true);

        DeckCardList = PFPlayerStuff.Create_DeckCards(DeckCardParent, CardPool[PFCardPool.DeckCard], PFConst.WidthResolution);

        //남은카드 갱신
        PFPlayerStuff.RemoveTargetComponent(CardPool[PFCardPool.RemainCard], CardPool[PFCardPool.DeckCard]);

        CardListUI_Remain.UpdateUI_CardList(CardPool[PFCardPool.RemainCard]);

        ShowRemainCardBtn.CardCount_UILabel.text = CardPool[PFCardPool.RemainCard].Count.ToString();

        Enable_DeckCard_OnClickEvent(true);
    }

    public override void OnTurnEnd()
    {
        base.OnTurnEnd();

        TurnEndBtn.SetActive(false);

        Enable_DeckCard_OnClickEvent(false);

        RemoveAllCard_InDeckUI();

        //'사용한 카드'로 옮겨짐
        List<int> deckCardList = CardPool[PFCardPool.DeckCard];
        for (int i = deckCardList.Count - 1; i >= 0; --i)
        {
            PFTable.ECardPoolType cardType = PFCardTable.GetCardType(deckCardList[i]);

            if (cardType == PFTable.ECardPoolType.ToUsedPool_AfterAction)
            {
                AddComponent_ToCardPool(PFCardPool.UsedCard, deckCardList[i]);
            }
            else if (cardType == PFTable.ECardPoolType.ToDestroyedPool_AfterAction)
            {
                AddComponent_ToCardPool(PFCardPool.DestroyedCard, deckCardList[i]);
            }

            deckCardList.RemoveAt(i);
        }
    }

    public void OnClick_RemainCardBtn()
    {
        CardListUI_Remain.gameObject.SetActive(true);
    }

    public void OnClick_UsedCardBtn()
    {
        CardListUI_Used.gameObject.SetActive(true);
    }

    public void OnClick_DestroyedCardBtn()
    {
        CardListUI_Destroyed.gameObject.SetActive(true);
    }

    public void OnClick_TurnEndBtn()
    {
        OnClickTurnEndBtn_Dele();
    }

    private void OnClick_Character(APFCharacter clickedChar)
    {
        bool clickedChar_Is_Enemy = PFPlayerCharacter.IsEnemy(clickedChar);

        if (clickedChar_Is_Enemy)
        {
            if (SelectedCard_OnDeck != null)
            {
                UseCard(SelectedCard_OnDeck, clickedChar);
            }
        }
    }

    private void OnClick_DeckCard(APFCardUI clickedCard)
    {
        bool isClicked_SameCard = (SelectedCard_OnDeck == clickedCard);

        if (clickedCard != null)
        {
            if (isClicked_SameCard)
            {
                DeselectDeckCard(clickedCard);
            }
            else
            {
                int remainCost = GetCost();
                int cardCost = clickedCard.GetCost();

                if (cardCost <= remainCost)
                {
                    SelectDeckCard(clickedCard);
                }
            }
        }
    }

    private void SelectDeckCard(APFCardUI selectCard)
    {
        DeselectDeckCard(SelectedCard_OnDeck);

        if (selectCard != null)
        {
            SelectedCard_OnDeck = selectCard;

            SelectedCard_OnDeck.gameObject.transform.localScale = new Vector3(PFConst.Scale_ClickedCardOnDeck, PFConst.Scale_ClickedCardOnDeck, 1.0f);

            PFTable.ECardApplyTarget applyTarget = PFCardTable.GetApplyTarget(SelectedCard_OnDeck.CardTableId);

            if (applyTarget == PFTable.ECardApplyTarget.EnemyAll)
            {
                List<APFCharacter> aliveMonsterList = APFGameMode.GetInstance().GetAliveCharList(PFTable.CharacterType.Monster);
                PFCharacterStuff.Active_CharSelectIconUI(aliveMonsterList, true);
            }
        }
    }

    private void DeselectDeckCard(APFCardUI deselectCard)
    {
        List<APFCharacter> aliveCharList = APFGameMode.GetInstance().GetAliveCharList();
        PFCharacterStuff.Active_CharSelectIconUI(aliveCharList, false);

        if (deselectCard != null)
        {
            float fDefaultScale = 1.0f;
            deselectCard.gameObject.transform.localScale = new Vector3(fDefaultScale, fDefaultScale, fDefaultScale);
        }

        SelectedCard_OnDeck = null;
    }

    protected override void SetCost(int newCost)
    {
        base.SetCost(newCost);

        StringBuilder costUILabel = new StringBuilder("");
        costUILabel.AppendFormat("{0}/{1}", newCost, PFConst.DefaultPlayerMaxCost);
        Cost_UILabel.text = costUILabel.ToString();
    }
}
