using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APFPlayerCharacter : APFCharacter
{
    [SerializeField]
    private APFShieldUI ShieldUI;

    public override void OnUseCard(PFTable.Card card, APFCharacter target)
    {
        base.OnUseCard(card, target);
    }

    protected override void SetShieldValue(int val)
    {
        base.SetShieldValue(val);

        if (val > 0)
        {
            ShieldUI.gameObject.SetActive(true);

            ShieldUI.Shield_UILabel.text = val.ToString();
        }
        else
        {
            ShieldUI.gameObject.SetActive(false);
        }
    }
}
