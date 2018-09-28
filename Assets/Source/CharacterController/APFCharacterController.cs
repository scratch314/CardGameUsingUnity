using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public enum ECardEffect
{
    AtkRate,
    DefRate,
}

public class APFCharacterController : MonoBehaviour
{
    private int Cost = 0;

    protected APFCharacter PFCharacter = null;

    public Action<APFCharacterController, PFTable.Card, APFCharacterController> UseCard_Dele;

    public List<CardEffect.PowerRate> CardEffectList_AttackRate = new List<CardEffect.PowerRate>();

    public List<CardEffect.PowerRate> CardEffectList_DefenceRate = new List<CardEffect.PowerRate>();

    public List<int> PendingEffect_DamageList = new List<int>();


    public int CharacterTableId
    {
        get;
        set;
    }


    public virtual void Init(int charTableId, Vector3 vPos)
    {
        PFCharacter = CreateCharacter<APFCharacter>(charTableId, vPos);
        PFCharacter.Init(this);
    }

    public int GetDamage(int damageAmout)
    {
        PFTable.Character attackerCharTable = PFTable.GetCharacter(CharacterTableId);

        if (attackerCharTable != null)
        {
            float atkRateSum = GetCardEffectSum(ECardEffect.AtkRate);

            float atkRatio = attackerCharTable.atkRatio + atkRateSum;

            float fDamage = atkRatio * damageAmout;
            int iDamage = (int)fDamage;//소수점이 발생하면 버린다.
            return iDamage;
        }

        return 0;
    }

    public bool IsEnemy(APFCharacterController target)
    {
        if (target != null)
        {
            bool bResult = GetCharacter().IsEnemy(target.GetCharacter());
            return bResult;
        }

        return false;
    }

    public APFCharacter GetCharacter()
    {
        return PFCharacter;
    }

    public virtual void StartGamePlay()
    {
        PFCharacter.StartGamePlay();
    }

    static private void Update_Remove_CardEffect(List<CardEffect.PowerRate> effectList)
    {
        for (int i = effectList.Count - 1; i >= 0; --i)
        {
            CardEffect.PowerRate cardEffect_powerRate = effectList[i];

            if (cardEffect_powerRate.KeepTurnCount == PFConst.KeepGoing_StatusKeepTurnCount)
            {
                //"계속유지"
            }
            else
            {
                --cardEffect_powerRate.KeepTurnCount;
            }

            if (cardEffect_powerRate.KeepTurnCount == 0)
            {
                effectList.RemoveAt(i);
            }
        }
    }

    private void DoDamage_PendingList(PFTable.EActuationTiming actuationTiming)
    {
        for (int i = PendingEffect_DamageList.Count - 1; i >= 0; --i)
        {
            int effectId_damage = PendingEffect_DamageList[i];

            PFTable.Damage effect_Damage = PFTable.GetEffect_Damage(effectId_damage);

            if (effect_Damage != null &&
                effect_Damage.ActuationTiming == actuationTiming)
            {
                if (effect_Damage.TargetType == PFTable.ETargetCharacter.EnemyAll ||
                    effect_Damage.TargetType == PFTable.ETargetCharacter.Self)
                {
                    APFGameMode.DoDamage(this, effectId_damage, null);
                    PendingEffect_DamageList.RemoveAt(i);
                }
                else
                {
                    StringBuilder sbLog = new StringBuilder("");
                    sbLog.AppendFormat("효과Damage id {0} => {1}발동인데 TargetType이 EnemySingle임", effectId_damage, actuationTiming.ToString());
                    Debug.LogError(sbLog.ToString());
                }
            }
        }
    }

    public virtual void OnTurnBegin()
    {
        PFCharacter.OnTurnBegin();

        DoDamage_PendingList(PFTable.EActuationTiming.TurnBegin);
    }

    public virtual void OnTurnEnd()
    {
        PFCharacter.OnTurnEnd();

        Update_Remove_CardEffect(CardEffectList_AttackRate);
        Update_Remove_CardEffect(CardEffectList_DefenceRate);

        DoDamage_PendingList(PFTable.EActuationTiming.TurnEnd);
    }

    public class CardEffect
    {
        public class PowerRate
        {
            public float Rate;
            public int KeepTurnCount;
        }
    }

    public float GetCardEffectSum(ECardEffect cardEffectType)
    {
        float rateSum = 0.0f;

        if (cardEffectType == ECardEffect.AtkRate)
        {
            for (int i = 0; i < CardEffectList_AttackRate.Count; ++i)
            {
                rateSum += CardEffectList_AttackRate[i].Rate;
            }
        }
        else if (cardEffectType == ECardEffect.DefRate)
        {
            for (int i = 0; i < CardEffectList_DefenceRate.Count; ++i)
            {
                rateSum += CardEffectList_DefenceRate[i].Rate;
            }
        }

        return rateSum;
    }

    static private CardEffect.PowerRate Create_CardEffect_PowerRate(float rate, int keepTurnCount)
    {
        CardEffect.PowerRate attackRate = new CardEffect.PowerRate();

        attackRate.Rate = rate;
        attackRate.KeepTurnCount = keepTurnCount;

        return attackRate;
    }

    protected virtual void UseCard(PFTable.Card useCard, APFCharacter target_clickedChar)
    {
        if (useCard != null)
        {
            int cardCost = useCard.cost;
            AddCost(-cardCost);
        }

        if (PFCharacter != null)
        {
            PFCharacter.OnUseCard(useCard, target_clickedChar);
        }

        if (target_clickedChar != null)
        {
            UseCard_Dele(this, useCard, target_clickedChar.GetCharacterController());
        }
    }

    public void OnUseCard_AfterApplyDmg(PFTable.Card useCard, APFCharacterController targetCtrl)
    {
        if (useCard != null)
        {
            if (useCard.statusKeepTurnCount > 0)
            {
                if (useCard.atkRate != 0)
                {
                    CardEffect.PowerRate attackRate = Create_CardEffect_PowerRate(useCard.atkRate, useCard.statusKeepTurnCount);
                    CardEffectList_AttackRate.Add(attackRate);
                }

                if (useCard.atkRateTarget != 0)
                {
                    CardEffect.PowerRate attackRate = Create_CardEffect_PowerRate(useCard.atkRateTarget, useCard.statusKeepTurnCount);
                    targetCtrl.CardEffectList_AttackRate.Add(attackRate);
                }

                if (useCard.defRate != 0)
                {
                    CardEffect.PowerRate defenceRate = Create_CardEffect_PowerRate(useCard.defRate, useCard.statusKeepTurnCount);
                    CardEffectList_DefenceRate.Add(defenceRate);
                }

                if (useCard.defRateTarget != 0)
                {
                    CardEffect.PowerRate defenceRate = Create_CardEffect_PowerRate(useCard.defRateTarget, useCard.statusKeepTurnCount);
                    targetCtrl.CardEffectList_DefenceRate.Add(defenceRate);
                }
            }
        }
    }

    public int GetHP()
    {
        return PFCharacter.GetHP();
    }

    private float GetDefencePowerRatio()
    {
        float defPowerRatio = 0.0f;

        PFTable.Character charTable = PFTable.GetCharacter(CharacterTableId);
        if (charTable != null)
        {
            defPowerRatio += charTable.defBase;
        }

        float cardEffectSum = GetCardEffectSum(ECardEffect.DefRate);

        defPowerRatio += cardEffectSum;

        return defPowerRatio;
    }

    public void OnBeAttacked(int attackerDmgAmount)
    {
        float defencePowerRatio = GetDefencePowerRatio();

        float dmgRatio = (1.0f - defencePowerRatio);

        float dmgAmount = attackerDmgAmount * dmgRatio;
        int iDamage = (int)dmgAmount;//소수점이 발생하면 버린다.

        PFCharacter.OnBeAttacked(iDamage);
    }

    public virtual void OnCharacterDead()
    {
        PFCharacter.OnDead();

        gameObject.SetActive(false);
    }

    protected T CreateCharacter<T>(int charTableId, Vector3 pos) where T : APFCharacter
    {
        CharacterTableId = charTableId;

        string charPrefabPath = PFCharacterTable.GetCharacterPrefabPath(CharacterTableId);

        T character = PFUtil.Instantiate<T>(charPrefabPath, this.gameObject, pos);

        return character;
    }

    public int GetCost()
    {
        return Cost;
    }

    protected virtual void SetCost(int newCost)
    {
        Cost = newCost;
    }

    public void AddCost(int costAmount)
    {
        int newCost = Cost + costAmount;
        SetCost(newCost);
    }
}
