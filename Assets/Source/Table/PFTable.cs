using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public static class PFTable
{
    public enum CharacterType
    {
        Player,
        Monster,
        Boss,
        Count,
        Invalid,
    }

    //카드 타입은 어떤 식으로 사용되는 카드인지를 지정합니다(WhereaboutsType)
    public enum ECardPoolType
    {
        ToUsedPool_AfterAction, //사용: 사용된후, 사용한 카드Pool로 이동됨.
        //Volatility,   //휘발성: 사용하지 않으면 턴 종료와 함께 바로 사라짐
        ToDestroyedPool_AfterAction, //파괴: 사용된후, 파괴된 카드Pool로 이동됨 (이 Pool에 있는 카드는, 덱을 다시 섞을때, 대상에 포함되지 않음)
        //CantBeUsed,   //사용불가: 플레이어가 사용할 수 없고, 턴 종료와 함께 사용한 카드Pool로 이동됨.
        //Start,        //시작: 전투 시작시 무조건 첫번째 패에 포함됩니다.
        //Preservation, //보존: 턴종료가 되어도 남은 카드들이 사용한 카드 더미로 옮겨지지 않게 됨.
        Invalid,
    }

    //발동 타입
    public enum ECardActiveType
    {
        Instant,            //즉시: 사용하는 순간 효과 발동
        TurnEnd,            //턴종료: 턴이 종료될 때 효과 발동
        NextTurnBegin,      //다음턴시작때: 턴이 시작될 때 효과 발동
        //OnCharacterDamaged, //반사: 피해를 입었을 때 효과 발동
        //CardDestroyed,      //파괴: 카드가 파괴되었을 때 효과 발동
        //CharacterDead,      //죽음: 몬스터나 소환용으로 이 스킬을 가진 대상이 죽었을 때 효과 발동
        Invalid,
    }

    public enum ECardApplyTarget
    {
        EnemyAll,
        EnemySingle,
        Invalid,
    }

    public class TableBase
    {
        public int uid;
    }

    public class Character : TableBase
    {
        public string stringIndex;
        public string CharacterPrefabPath;
        public string displayName;
        public CharacterType characterType;
        public int maxHp;
        public float maxHpRand;
        public float atkRatio;
        public float defBase;
    }

    public class Card : TableBase
    {
        public enum StatusIcon
        {
            None,
            Atk_Up,
            Atk_Down,
            Defence_Down,
        }

        public ECardPoolType cardPoolType;
        public ECardApplyTarget applyTarget;
        public string displayName_KR;
        public int cost;
        public string displayDesc;
        public ECardActiveType activeType;
        public int damage;
        public int shield;
        public int heal;
        public int[] CardEffectIdList;
        public float atkRate;
        public float atkRateTarget;
        public float defRate;
        public float defRateTarget;
        public string skillImgPath;
        public int statusKeepTurnCount; //상태 지속 턴수
        public StatusIcon statusIcon;
        public StatusIcon statusIcon_target;
    }

    public class CardEffect : TableBase
    {
        public enum EType
        {
            DecreaseHP,
            Damage,
            Invalid,
        }

        public EType CardEffectType;
        public int Amount;
    }






    public class Effect : TableBase
    {
    }

    public enum ETargetCharacter
    {
        Self,
        EnemySingle,
        EnemyAll,
    }

    public enum EActuationTiming
    {
        Instant,
        TurnEnd,
        TurnBegin,
    }

    //주어부
    public enum ESubject
    {
        HP,
    }

    //술부(서술부)
    public enum EPredicate
    {
        Less,
    }

    public enum EDataType
    {
        Absolute,
        Percent,
    }

    //발동 조건
    public class APFActuationCondition
    {
        //주어부
        public ESubject Subject;
        //서술부
        public EPredicate Predicate;
        //비교값
        public int Data;
        //절대값, %, ... 인지
        public EDataType DataType;
    }

    public class Damage : Effect
    {
        //크기
        public int Amount;

        //적용대상
        public ETargetCharacter TargetType;

        //몇번 적용할지
        public int Times;

        //발동 타이밍
        public EActuationTiming ActuationTiming;
    }

    public enum EKeepGoingDuration
    {
        ActuationConditionIsTrue,
        Turn1,
        Turn2,
    }

    public class AddAtkRate : Effect
    {
        public float Amount;

        public ETargetCharacter TargetType;

        //발동 타이밍
        public EActuationTiming ActuationTiming;

        //발동 조건
        public APFActuationCondition ActuationCondition;

        //유지기간
        public EKeepGoingDuration KeepGoingDuration;
    }







    public static void LoadTables()
    {
        LoadTable<PFTable.Character>("Tables/CharacterTable", PFTable_Characters);
        LoadTable<PFTable.Card>("Tables/CardTable", PFTable_Cards);
        LoadTable<PFTable.CardEffect>("Tables/CardEffectTable", PFTable_CardEffects);

        LoadTable<PFTable.Damage>("Tables/Effect/Effect_DecreaseHP", PFTable_Damage);
        LoadTable<PFTable.AddAtkRate>("Tables/Effect/Effect_AddAtkRate", PFTable_AddAtkRate);
    }

    //----------------------------------------------------------------------------
    private static PFTable.Character CharacterSearchDummy = new PFTable.Character();
    private static UIDComparer<PFTable.Character> CharacterComparer = new UIDComparer<PFTable.Character>();
    public static PFTable.Character GetCharacter(int characterId)
    {
        CharacterSearchDummy.uid = characterId;

        int idx = PFTable_Characters.BinarySearch(CharacterSearchDummy, CharacterComparer);
        if (idx >= 0)
        {
            return PFTable_Characters[idx];
        }

        return null;
    }

    //----------------------------------------------------------------------------
    private static PFTable.Card CardSearchDummy = new PFTable.Card();
    private static UIDComparer<PFTable.Card> CardComparer = new UIDComparer<PFTable.Card>();
    public static PFTable.Card GetCard(int cardId)
    {
        CardSearchDummy.uid = cardId;

        int idx = PFTable_Cards.BinarySearch(CardSearchDummy, CardComparer);
        if (idx >= 0)
        {
            return PFTable_Cards[idx];
        }

        return null;
    }

    //----------------------------------------------------------------------------
    private static PFTable.CardEffect CardEffectSearchDummy = new PFTable.CardEffect();
    private static UIDComparer<PFTable.CardEffect> CardEffectComparer = new UIDComparer<PFTable.CardEffect>();
    public static PFTable.CardEffect GetCardEffect(int cardEffectId)
    {
        CardEffectSearchDummy.uid = cardEffectId;

        int idx = PFTable_CardEffects.BinarySearch(CardEffectSearchDummy, CardEffectComparer);
        if (idx >= 0)
        {
            return PFTable_CardEffects[idx];
        }

        return null;
    }

    //----------------------------------------------------------------------------
    private static PFTable.Damage DecreaseHPSearchDummy = new PFTable.Damage();
    private static UIDComparer<PFTable.Damage> DecreaseHPComparer = new UIDComparer<PFTable.Damage>();
    public static PFTable.Damage GetEffect_Damage(int damageEffectId)
    {
        DecreaseHPSearchDummy.uid = damageEffectId;

        int idx = PFTable_Damage.BinarySearch(DecreaseHPSearchDummy, DecreaseHPComparer);
        if (idx >= 0)
        {
            return PFTable_Damage[idx];
        }

        return null;
    }

    //----------------------------------------------------------------------------
    private static PFTable.AddAtkRate AddAtkRateSearchDummy = new PFTable.AddAtkRate();
    private static UIDComparer<PFTable.AddAtkRate> AddAtkRateComparer = new UIDComparer<PFTable.AddAtkRate>();
    public static PFTable.AddAtkRate GetAddAtkRate(int AddAtkRateId)
    {
        AddAtkRateSearchDummy.uid = AddAtkRateId;

        int idx = PFTable_AddAtkRate.BinarySearch(AddAtkRateSearchDummy, AddAtkRateComparer);
        if (idx >= 0)
        {
            return PFTable_AddAtkRate[idx];
        }

        return null;
    }

    //----------------------------------------------------------------------------
    public class UIDComparer<T> : IComparer<T> where T : TableBase
    {
        public int Compare(T a, T b)
        {
            return a.uid - b.uid;
        }
    }

    public static int[] ToIntArray(string value, char separator)
    {
        return Array.ConvertAll(value.Split(separator), s => int.Parse(s));
    }

    private static void LoadTable<T>(string tablePath, List<T> table_list) where T : TableBase, new()
    {
        List<Dictionary<string, object>> rawTable = PFCSVReader.Read(tablePath);

        for (int i = 0; i < rawTable.Count; ++i)
        {
            T table = new T();

            Type type = table.GetType();

            FieldInfo[] fields = type.GetFields();
            for (int j = 0; j < fields.Length; ++j)
            {
                FieldInfo fieldInfo = fields[j];
                object val = rawTable[i][fieldInfo.Name];

                Type fieldInfoType = fieldInfo.FieldType;
                if (fieldInfoType.IsEnum)
                {
                    var enumVal = Enum.Parse(fieldInfoType, val.ToString());
                    fieldInfo.SetValue(table, enumVal);
                }
                else if (fieldInfoType == typeof(bool))
                {
                    var boolVal = Boolean.Parse(val.ToString());
                    fieldInfo.SetValue(table, boolVal);
                }
                else if (fieldInfoType.IsArray)
                {
                    var toString = val.ToString();
                    int[] intArray = ToIntArray(toString, ',');

                    if (intArray.Length == 1 && intArray[0] == PFInvalid.CardEffectId)
                    {
                        //invalid 값을 가지고 있다면, null로 셋팅한다 - 18.06.24.kkw
                        fieldInfo.SetValue(table, null);
                    }
                    else
                    {
                        fieldInfo.SetValue(table, intArray);
                    }
                }
                else
                {
                    fieldInfo.SetValue(table, val);
                }
            }

            table_list.Add(table);
        }

        UIDComparer<T> uidComparer = new UIDComparer<T>();
        table_list.Sort(uidComparer);
    }

    private static List<PFTable.Character> PFTable_Characters = new List<PFTable.Character>();
    private static List<PFTable.Card> PFTable_Cards = new List<PFTable.Card>();
    private static List<PFTable.CardEffect> PFTable_CardEffects = new List<PFTable.CardEffect>();

    private static List<PFTable.Damage> PFTable_Damage = new List<PFTable.Damage>();
    private static List<PFTable.AddAtkRate> PFTable_AddAtkRate = new List<PFTable.AddAtkRate>();
}
