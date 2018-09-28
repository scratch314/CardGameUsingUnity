using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APFStatusIconUI : MonoBehaviour
{
    [SerializeField]
    UILabel StatusKeepTurnCount_UILabel; //상태 지속 턴수

    [SerializeField]
    public UI2DSprite StatusIconImage;


    [HideInInspector]
    public PFTable.Card.StatusIcon StatusIconType = PFTable.Card.StatusIcon.None;

    int StatusKeepTurnCount = 0;


    public void SetStatusKeepTurnCount(int count)
    {
        StatusKeepTurnCount = count;

        if (StatusKeepTurnCount_UILabel != null)
        {
            if (StatusKeepTurnCount == 0)
            {
                StatusKeepTurnCount_UILabel.text = null;
            }
            else
            {
                StatusKeepTurnCount_UILabel.text = StatusKeepTurnCount.ToString();
            }
        }
    }

    public int GetStatusKeepTurnCount()
    {
        return StatusKeepTurnCount;
    }
}
