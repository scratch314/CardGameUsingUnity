using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

//18.04.23.kkw

public class APFGameMode : MonoBehaviour
{
    [SerializeField]
    private APFGameOverUI GameOverUI;

    [SerializeField]
    private GameObject TurnInfoUI_Player;

    [SerializeField]
    private GameObject TurnInfoUI_Monster;


    private List<APFCharacterController> AliveCharCtrlList = new List<APFCharacterController>();
    private List<APFCharacterController> DeadCharCtrlList = new List<APFCharacterController>();

    public Action<APFCharacter> OnClick_Character_Dele;

    private APFCharacterController TurnCharacter = null;


    public enum EGameResult
    {
        Victory,
        Defeat,
        InProgress,
    }


    public static APFGameMode GetInstance()
    {
        return APFGameInstance.PFGameMode;
    }

    private void Awake()
    {
    }

    public void Init()
    {
        Create_CharCtrlList();

        StartGamePlay();
    }

    public List<APFCharacter> GetAliveCharList(PFTable.CharacterType charType)
    {
        List<APFCharacter> aliveList = new List<APFCharacter>();

        for (int i = 0; i < AliveCharCtrlList.Count; ++i)
        {
            APFCharacter aliveChar = AliveCharCtrlList[i].GetCharacter();
            PFTable.CharacterType aliveCharType = PFCharacterTable.GetCharacterType(aliveChar.CharacterTableId);

            if (aliveCharType == charType)
            {
                aliveList.Add(aliveChar);
            }
        }

        return aliveList;
    }

    public List<APFCharacter> GetAliveCharList()
    {
        List<APFCharacter> aliveList = new List<APFCharacter>();

        for (int i = 0; i < AliveCharCtrlList.Count; ++i)
        {
            APFCharacter aliveChar = AliveCharCtrlList[i].GetCharacter();
            aliveList.Add(aliveChar);
        }

        return aliveList;
    }

    private void Destroy_CharCtrlList()
    {
        PFUtil.DestroyGameObject(AliveCharCtrlList);
        PFUtil.DestroyGameObject(DeadCharCtrlList);
    }

    private void Create_CharCtrlList()
    {
        APFPlayerController playerController = CreatePlayerController();
        AliveCharCtrlList.Add(playerController);

        playerController.OnClickTurnEndBtn_Dele += OnClick_PlayerTurnEndBtn;

        List<List<int>> monsterGroupList = PFMonsterStuff.GetMonsterGroupList();
        List<APFMonsterController> monsterControllerList = PFMonsterStuff.CreateMonsterControllers(monsterGroupList, this.gameObject);

        if (monsterControllerList != null)
        {
            for (int i = 0; i < monsterControllerList.Count; ++i)
            {
                AliveCharCtrlList.Add(monsterControllerList[i]);
            }
        }

        for (int i = 0; i < AliveCharCtrlList.Count; ++i)
        {
            AliveCharCtrlList[i].UseCard_Dele += OnUseCard;
        }
    }

    public void OnClick_TryAgainBtn()
    {
        Destroy_CharCtrlList();

        Create_CharCtrlList();

        StartGamePlay();
    }

    private void OnClick_PlayerTurnEndBtn()
    {
        TurnEnd(TurnCharacter);
    }

    public APFPlayerController GetPlayer()
    {
        APFPlayerController plrController = PFPlayerStuff.GetPlayerController(AliveCharCtrlList);
        return plrController;
    }

    private void StartGamePlay()
    {
        TurnCharacter = null;

        TurnInfoUI_Player.SetActive(false);
        TurnInfoUI_Monster.SetActive(false);

        GameOverUI.gameObject.SetActive(false);

        for (int i = 0; i < AliveCharCtrlList.Count; ++i)
        {
            AliveCharCtrlList[i].StartGamePlay();
        }

        APFPlayerController plrController = PFPlayerStuff.GetPlayerController(AliveCharCtrlList);
        if (plrController != null)
        {
            TurnBegin(plrController);
        }
    }

    private IEnumerator OnTurnBegin(APFCharacterController turnBeginChar)
    {
        if (turnBeginChar != null)
        {
            if (turnBeginChar.IsEnemy(TurnCharacter))
            {
                PFTable.CharacterType charType = PFCharacterTable.GetCharacterType(turnBeginChar.CharacterTableId);
                if (charType == PFTable.CharacterType.Player)
                {
                    TurnInfoUI_Player.SetActive(true);
                    yield return new WaitForSeconds(PFConst.TurnInfoNotiTime);
                    TurnInfoUI_Player.SetActive(false);
                }
                else if (charType == PFTable.CharacterType.Monster)
                {
                    TurnInfoUI_Monster.SetActive(true);
                    yield return new WaitForSeconds(PFConst.TurnInfoNotiTime);
                    TurnInfoUI_Monster.SetActive(false);
                }
            }

            TurnCharacter = turnBeginChar;

            TurnCharacter.OnTurnBegin();
        }
    }

    private void TurnBegin(APFCharacterController turnBeginChar)
    {
        StartCoroutine(this.OnTurnBegin(turnBeginChar));
    }

    private void TurnEnd(APFCharacterController turnEndChar)
    {
        if (turnEndChar != null)
        {
            turnEndChar.OnTurnEnd();

            APFCharacterController nextTurnChar = PFGameModeStuff.GetNextTurnCharacter(AliveCharCtrlList, TurnCharacter);

            TurnBegin(nextTurnChar);
        }
    }

    private List<APFCharacterController> GetTargetList(PFTable.Card srcCard, APFCharacterController specifiedTarget)
    {
        List<APFCharacterController> resultList = new List<APFCharacterController>();

        PFTable.ECardApplyTarget applyTarget = PFCardTable.GetApplyTarget(srcCard.uid);
        if (applyTarget == PFTable.ECardApplyTarget.EnemyAll)
        {
            List<APFCharacter> aliveMonsterList = APFGameMode.GetInstance().GetAliveCharList(PFTable.CharacterType.Monster);

            for (int i = 0; i < aliveMonsterList.Count; ++i)
            {
                APFCharacterController monCtrl = aliveMonsterList[i].GetCharacterController();
                resultList.Add(monCtrl);
            }
        }
        else
        {
            resultList.Add(specifiedTarget);
        }

        return resultList;
    }

    List<APFCharacterController> GetTargetList(APFCharacterController srcChar, PFTable.ETargetCharacter targetType, APFCharacterController target_clickedChar)
    {
        List<APFCharacterController> resultList = new List<APFCharacterController>();

        switch (targetType)
        {
            case PFTable.ETargetCharacter.Self:
            {
                if (srcChar != null)
                {
                    resultList.Add(srcChar);
                }
            }
            break;

            case PFTable.ETargetCharacter.EnemySingle:
            {
                if (target_clickedChar != null)
                {
                    resultList.Add(target_clickedChar);
                }
            }
            break;

            case PFTable.ETargetCharacter.EnemyAll:
            {
                List<APFCharacter> aliveMonsterList = APFGameMode.GetInstance().GetAliveCharList(PFTable.CharacterType.Monster);
                for (int i = 0; i < aliveMonsterList.Count; ++i)
                {
                    APFCharacterController monCtrl = aliveMonsterList[i].GetCharacterController();
                    resultList.Add(monCtrl);
                }
            }
            break;

            default:
            {
                StringBuilder sbLog = new StringBuilder("");
                sbLog.AppendFormat("GetTargetList  =>  switch - case 구현되지 않은 타입이 있음");
                Debug.LogError(sbLog.ToString());
            }
            break;
        }

        return resultList;
    }

    const float Damage_Times_Interval = 1.0f;

    public static void DoDamage(APFCharacterController attacker, int damage_EffectId, APFCharacterController target_clickedChar)
    //public static IEnumerator DoDamage(APFCharacterController attacker, int damage_EffectId, APFCharacterController target_clickedChar)
    {
        PFTable.Damage addHP_Effect = PFTable.GetEffect_Damage(damage_EffectId);

        APFGameMode pfGameMode = APFGameMode.GetInstance();

        if (attacker == null || addHP_Effect == null || pfGameMode == null)
        {
            //yield return null;
        }

        int attackerDamage = attacker.GetDamage(addHP_Effect.Amount);

        List<APFCharacterController> targetList = pfGameMode.GetTargetList(attacker, addHP_Effect.TargetType, target_clickedChar);

        for (int times = 0; times < addHP_Effect.Times; ++times)
        {
            for (int i = 0; i < targetList.Count; ++i)
            {
                targetList[i].OnBeAttacked(attackerDamage);

                if (targetList[i].GetHP() <= PFConst.HP_Min)
                {
                    pfGameMode.OnCharacterDead(targetList[i]);
                }
            }

            if (addHP_Effect.Times >= 2)
            {
                //yield return new WaitForSeconds(Damage_Times_Interval);
            }
        }
    }

    private void OnUseCard(APFCharacterController srcCharCtrl, PFTable.Card srcCard, APFCharacterController target_clickedChar)
    {
        if (srcCharCtrl != null && srcCard != null)
        {
            EGameResult gameResult = EGameResult.InProgress;

            PFTable.ECardActiveType cardActiveType = PFCardTable.GetCardActiveType(srcCard.uid);
            //if (cardActiveType == PFTable.ECardActiveType.Instant)
            {
                List<APFCharacterController> targetList = GetTargetList(srcCard, target_clickedChar);

                int damage = PFSkill.GetDamage(srcCharCtrl, srcCard.uid);

                for (int i = 0; i < targetList.Count; ++i)
                {
                    targetList[i].OnBeAttacked(damage);

                    srcCharCtrl.OnUseCard_AfterApplyDmg(srcCard, targetList[i]);

                    if (targetList[i].GetHP() <= PFConst.HP_Min)
                    {
                        gameResult = OnCharacterDead(targetList[i]);
                    }
                }
            }



            int[] cardEffectIdList = srcCard.CardEffectIdList;
            if (cardEffectIdList != null)
            {
                for (int i = 0; i < cardEffectIdList.Length; ++i)
                {
                    //PFTable.CardEffect.EType cardEffectType = PFCardEffectTable.GetCardEffectType(cardEffectIdList[i]);
                    //if (cardEffectType == PFTable.CardEffect.EType.DecreaseHP)
                    //{

                    //}

                    int effectId = cardEffectIdList[i];




                    PFTable.Damage effect_Damage = PFTable.GetEffect_Damage(effectId);
                    if (effect_Damage != null)
                    {
                        if (effect_Damage.ActuationTiming == PFTable.EActuationTiming.Instant)
                        {
                            DoDamage(srcCharCtrl, effectId, target_clickedChar);
                            //StartCoroutine(DoDamage(srcCharCtrl, effectId, target_clickedChar));

                            /*
                            if (targetList[j].GetHP() <= PFConst.HP_Min)
                            {
                                gameResult = OnCharacterDead(targetList[j]);
                            }
                            */
                        }
                        else
                        {
                            srcCharCtrl.PendingEffect_DamageList.Add(effectId);
                        }
                    }

                    PFTable.AddAtkRate addAtkRate = PFTable.GetAddAtkRate(effectId);
                    if (addAtkRate != null)
                    {
                        //addAtkRate.Amount
                        //addAtkRate.TargetType
                        if (addAtkRate.ActuationTiming == PFTable.EActuationTiming.Instant)
                        {
                        }
                    }




                    PFTable.CardEffect cardEffect = PFTable.GetCardEffect(effectId);
                    if (cardEffect != null)
                    {
                        if (cardEffect.CardEffectType == PFTable.CardEffect.EType.DecreaseHP)
                        {
                            //cardEffect.Amount
                        }
                    }
                }
            }




            if (gameResult == EGameResult.InProgress)
            {
                PFTable.CharacterType srcCharType = PFCharacterTable.GetCharacterType(srcCharCtrl.CharacterTableId);
                if (srcCharType == PFTable.CharacterType.Monster)
                {
                    int remainCost = srcCharCtrl.GetCost();
                    if (remainCost <= 0)
                    {
                        TurnEnd(srcCharCtrl);
                    }
                }
            }
            else if (gameResult == EGameResult.Victory)
            {
                GameOverUI.Title_UILabel.text = PFConst.StrGameResultVictory;
                GameOverUI.gameObject.SetActive(true);
            }
            else if (gameResult == EGameResult.Defeat)
            {
                GameOverUI.Title_UILabel.text = PFConst.StrGameResultDefeat;
                GameOverUI.gameObject.SetActive(true);
            }
        }
    }

    private EGameResult OnCharacterDead(APFCharacterController targetCtrl)
    {
        targetCtrl.OnCharacterDead();

        DeadCharCtrlList.Add(targetCtrl);
        AliveCharCtrlList.Remove(targetCtrl);

        APFPlayerController plrCtrl = PFPlayerStuff.GetPlayerController(AliveCharCtrlList);
        if (plrCtrl == null)
        {
            return EGameResult.Defeat;
        }
        else if (plrCtrl != null && AliveCharCtrlList.Count == 1)
        {
            return EGameResult.Victory;
        }

        return EGameResult.InProgress;
    }

    public void OnClick_Character(APFCharacter clickChar)
    {
        OnClick_Character_Dele(clickChar);
    }

    APFPlayerController CreatePlayerController()
    {
        APFPlayerController plrController_original = Resources.Load<APFPlayerController>(PFPrefabPath.PlayerController);

        APFPlayerController newPlrController = PFUtil.Instantiate(plrController_original, this.gameObject);

        Vector3 vPos = new Vector3(-480, PFConst.CharacterCreatePosY, 0.0f);

        int charTableId = 10001;
        newPlrController.Init(charTableId, vPos);

        return newPlrController;
    }

    private APFCharacterController GetNextTurnCharacter()
    {
        return null;
    }

    public void OnClick_TurnEnd()
    {
    }
}
