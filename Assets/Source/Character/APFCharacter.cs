using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class APFCharacter : MonoBehaviour
{
    [SerializeField]
    private UISlider HP_UISlider;

    [SerializeField]
    private UILabel HP_UILabel;

    [SerializeField]
    public GameObject SelectIcon_UI;

    [SerializeField]
    public UIGrid StatusIcon_UIGrid;


    private List<APFStatusIconUI> StatusIconList = new List<APFStatusIconUI>();

    private int MaxHP = PFNumber.InvalidHP;

    private int CurrentHP = PFNumber.InvalidHP;

    protected int Shield = 0;

    private APFCharacterController CharacterController;


    public virtual void Init(APFCharacterController controller)
    {
        SetController(controller);

        MaxHP = PFCharacterStuff.CalcMaxHP(CharacterTableId);

        int maxpHp = GetMaxHP();
        SetHP(maxpHp);

        SetShieldValue(0);
    }

    protected virtual void SetShieldValue(int val)
    {
        Shield = val;
    }

    public virtual void StartGamePlay()
    {
    }

    public virtual void OnTurnBegin()
    {
        //방어도는 발동하고 내 턴이 돌아올 때 방어도가 남아있더라도 0으로 초기화됩니다 - 18.06.07.kkw
        SetShieldValue(0);
    }

    public virtual void OnTurnEnd()
    {
        DecreaseTurnCount_StatusIcon();
    }

    void DecreaseTurnCount_StatusIcon()
    {
        for (int i = StatusIconList.Count - 1; i >= 0; --i)
        {
            int statusKeepTurnCount = StatusIconList[i].GetStatusKeepTurnCount();

            if (PFConst.KeepGoing_StatusKeepTurnCount != statusKeepTurnCount)
            {
                int newStatusKeepTurnCount = statusKeepTurnCount - 1;

                StatusIconList[i].SetStatusKeepTurnCount(newStatusKeepTurnCount);

                if (newStatusKeepTurnCount == 0)
                {
                    GameObject.Destroy(StatusIconList[i].gameObject);
                    StatusIcon_UIGrid.repositionNow = true;//grid에 새 요소를 제거한후, 위치를 재정비한다.

                    StatusIconList.RemoveAt(i);
                }
            }
        }
    }

    private APFStatusIconUI AddStatusIcon(PFTable.Card.StatusIcon statusIconType, int statusKeepTurnCount)
    {
        APFStatusIconUI newStatusIcon = PFUtil.Instantiate<APFStatusIconUI>(PFPrefabPath.StatusIcon, StatusIcon_UIGrid.gameObject, Vector3.zero);
        StatusIcon_UIGrid.repositionNow = true; //grid에 새 요소를 추가한후, 위치를 재정비한다.

        string statusIconPath = PFCardTable.GetStatusIconPath(statusIconType);
        UnityEngine.Sprite sprite = Resources.Load<UnityEngine.Sprite>(statusIconPath);

        newStatusIcon.StatusIconType = statusIconType;
        newStatusIcon.SetStatusKeepTurnCount(statusKeepTurnCount);
        newStatusIcon.StatusIconImage.sprite2D = sprite;

        StatusIconList.Add(newStatusIcon);

        return newStatusIcon;
    }

    APFStatusIconUI GetStatusIcon(PFTable.Card.StatusIcon statusIconType)
    {
        if (StatusIconList != null)
        {
            APFStatusIconUI statusIcon = StatusIconList.Find(comp =>
            {
                return comp.StatusIconType == statusIconType;
            });

            return statusIcon;
        }

        return null;
    }

    public virtual void OnUseCard(PFTable.Card card, APFCharacter target)
    {
        if (card != null)
        {
            if (card.shield > 0)
            {
                int newShieldVal = Shield + card.shield;
                SetShieldValue(newShieldVal);
            }

            if (card.statusIcon != PFTable.Card.StatusIcon.None)
            {
                APFStatusIconUI statusIcon = GetStatusIcon(card.statusIcon);
                if (statusIcon == null)
                {
                    AddStatusIcon(card.statusIcon, card.statusKeepTurnCount);
                }
                else
                {
                    int newStatusKeepTurnCount = statusIcon.GetStatusKeepTurnCount() + card.statusKeepTurnCount;
                    statusIcon.SetStatusKeepTurnCount(newStatusKeepTurnCount);
                }
            }

            if (card.statusIcon_target != PFTable.Card.StatusIcon.None)
            {
                APFStatusIconUI statusIcon = target.GetStatusIcon(card.statusIcon_target);
                if (statusIcon == null)
                {
                    target.AddStatusIcon(card.statusIcon_target, card.statusKeepTurnCount);
                }
                else
                {
                    int newStatusKeepTurnCount = statusIcon.GetStatusKeepTurnCount() + card.statusKeepTurnCount;
                    statusIcon.SetStatusKeepTurnCount(newStatusKeepTurnCount);
                }
            }
        }
    }

    public virtual void OnBeAttacked(int dmgAmount)
    {
        if (Shield > 0)
        {
            if (Shield <= dmgAmount)
            {
                dmgAmount -= Shield;
                SetShieldValue(0);
            }
            else
            {
                SetShieldValue(Shield - dmgAmount);
                dmgAmount = 0;
            }
        }

        AddHP(-dmgAmount);
    }

    public APFCharacterController GetCharacterController()
    {
        return CharacterController;
    }

    public bool IsEnemy(APFCharacter target)
    {
        bool bResult = PFCharacterStuff.IsEnemy(this, target);
        return bResult;
    }

    protected virtual void OnClick_This()
    {
        APFGameMode gameMode = APFGameMode.GetInstance();
        gameMode.OnClick_Character(this);
    }

    private void SetHP(int hp)
    {
        CurrentHP = hp;

        int mapHp = GetMaxHP();
        CurrentHP = Mathf.Clamp(CurrentHP, PFConst.HP_Min, mapHp);

        float hpRatio = CurrentHP / (float)mapHp;
        HP_UISlider.value = hpRatio;

        StringBuilder hpUILabel = new StringBuilder("");
        hpUILabel.AppendFormat("{0}/{1}", CurrentHP, mapHp);
        HP_UILabel.text = hpUILabel.ToString();
    }

    public void AddHP(int damage)
    {
        int newHp = CurrentHP + damage;
        SetHP(newHp);
    }

    public int CharacterTableId
    {
        get
        {
            if (CharacterController != null)
            {
                int characterTableId = CharacterController.CharacterTableId;
                return characterTableId;
            }

            return PFNumber.InvalidCharTableId;
        }
    }

    public int GetHP()
    {
        return CurrentHP;
    }

    protected int GetMaxHP()
    {
        if (MaxHP == PFNumber.InvalidHP)
        {
            StringBuilder sbLog = new StringBuilder("");
            sbLog.AppendFormat("MaxHP 가 Invalid 함");
            Debug.LogError(sbLog.ToString());
        }

        return MaxHP;
    }

    public virtual void OnDead()
    {
    }

    private void SetController(APFCharacterController controller)
    {
        CharacterController = controller;
    }
}
