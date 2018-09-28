using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PFConst
{
    public const string StrRemainCard = "남은 카드";
    public const string StrUsedCard = "사용한 카드";
    public const string StrDestroyedCard = "파괴된 카드";

    public const string StrGameResultVictory = "승리했습니다.";
    public const string StrGameResultDefeat = "패배했습니다.";

    public const float TurnInfoNotiTime = 1.5f;

    public const float MonsterTurnTime = 1.2f;

    public const int HP_Min = 0;

    public const int KeepGoing_StatusKeepTurnCount = 1000; //"계속유지" 하는 상태지속턴수

    public const float Scale_ClickedCardOnDeck = 1.3f;

    public const float CharacterCreatePosY = 220.0f;

    public const int MonsterCardIdBegin = 9001;
    public const int MonsterCardIdEnd = 9009;

    public const int DefaultPlayerMaxCost = 3;

    public const int InitDeckCardCount = 5;

    public const int WidthResolution = 1920;
}
