using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APFMonsterController : APFCharacterController
{
    APFMonsterCharacter PFMonsterCharacter;

    PFTable.Card CardToUseForMyTurn = null;


    public override void Init(int charTableId, Vector3 vPos)
    {
        base.Init(charTableId, vPos);

        PFMonsterCharacter = PFCharacter as APFMonsterCharacter;
    }

    public override void OnCharacterDead()
    {
        base.OnCharacterDead();
    }

    public override void StartGamePlay()
    {
        base.StartGamePlay();

        Update_CardToUseForMyTurn();
    }

    private void Update_CardToUseForMyTurn()
    {
        CardToUseForMyTurn = PFMonsterStuff.GetRandomCard();

        UnityEngine.Sprite skillImg_sprite = Resources.Load<UnityEngine.Sprite>(CardToUseForMyTurn.skillImgPath);
        PFMonsterCharacter.SkillImage.sprite2D = skillImg_sprite;

        int skillDamage = PFSkill.GetDamage(this, CardToUseForMyTurn.uid);
        string strSkillDamage = (skillDamage == 0) ? null : skillDamage.ToString();
        PFMonsterCharacter.SkillDamage_UILabel.text = strSkillDamage;

        PFMonsterCharacter.SkillDetailInfo.SetActive(false);

        PFMonsterCharacter.SkillDetailInfo_UILabel.text = CardToUseForMyTurn.displayDesc;
    }

    IEnumerator TurnBegin()
    {
        yield return new WaitForSeconds(PFConst.MonsterTurnTime);

        APFGameMode gameMode = APFGameMode.GetInstance();
        APFPlayerController plrController = gameMode.GetPlayer();
        UseCard(CardToUseForMyTurn, plrController.GetCharacter());
    }

    public override void OnTurnBegin()
    {
        base.OnTurnBegin();

        StartCoroutine(this.TurnBegin());
    }

    public override void OnTurnEnd()
    {
        base.OnTurnEnd();

        Update_CardToUseForMyTurn();
    }
}
