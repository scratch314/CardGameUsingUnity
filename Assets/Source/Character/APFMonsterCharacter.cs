using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class APFMonsterCharacter : APFCharacter
{
    [SerializeField]
    public UI2DSprite SkillImage;

    [SerializeField]
    public UILabel SkillDamage_UILabel;

    [SerializeField]
    public GameObject SkillDetailInfo;

    [SerializeField]
    public UILabel SkillDetailInfo_UILabel;

    [SerializeField]
    public GameObject HitEffect;

    [SerializeField]
    public GameObject CharacterImage;


    public override void Init(APFCharacterController controller)
    {
        base.Init(controller);

        UIEventTrigger uiEventTrigger = gameObject.AddComponent<UIEventTrigger>();
        EventDelegate.Add(uiEventTrigger.onClick, OnClick_This);
        EventDelegate.Add(uiEventTrigger.onHoverOver, OnHoverOver_This);
        EventDelegate.Add(uiEventTrigger.onHoverOut, OnHoverOut_This);

        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            string charPrefabPath = PFCharacterTable.GetCharacterPrefabPath(CharacterTableId);

            StringBuilder sbLog = new StringBuilder("");
            sbLog.AppendFormat("{0}.prefab  =>  BoxCollider 셋팅필요", charPrefabPath);
            Debug.LogError(sbLog.ToString());
        }
    }

    public override void StartGamePlay()
    {
        base.StartGamePlay();
    }

    public override void OnDead()
    {
        base.OnDead();
    }

    public override void OnTurnBegin()
    {
        base.OnTurnBegin();

        SkillDetailInfo.SetActive(true);
    }

    public override void OnTurnEnd()
    {
        base.OnTurnEnd();

        SkillDetailInfo.SetActive(false);
    }

    public override void OnBeAttacked(int dmgAmount)
    {
        base.OnBeAttacked(dmgAmount);

        HitEffect.SetActive(true);
    }

    protected override void OnClick_This()
    {
        base.OnClick_This();
    }

    private void OnHoverOver_This()
    {
        if (!SkillDetailInfo.activeSelf)
        {
            SkillDetailInfo.SetActive(true);
        }
    }

    private void OnHoverOut_This()
    {
        SkillDetailInfo.SetActive(false);
    }
}
