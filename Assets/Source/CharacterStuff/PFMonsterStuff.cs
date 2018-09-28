using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PFMonsterStuff
{
    public static PFTable.Card GetRandomCard()
    {
        int randCardId = UnityEngine.Random.Range(PFConst.MonsterCardIdBegin, PFConst.MonsterCardIdEnd);

        PFTable.Card cardTable = PFTable.GetCard(randCardId);

        return cardTable;
    }

    public static List<APFMonsterController> CreateMonsterControllers(List<List<int>> monsterGroupList, GameObject parent)
    {
        if (monsterGroupList == null)
        {
            return null;
        }

        List<int> monsterGroup = null;
        {
            int randIndex = UnityEngine.Random.Range(0, monsterGroupList.Count);
            monsterGroup = monsterGroupList[randIndex];
            monsterGroupList.RemoveAt(randIndex);
        }

        float xPos = 0.0f;
        float yPos = PFConst.CharacterCreatePosY;
        float xWidth = 300.0f;

        List<APFMonsterController> monsterControllerList = new List<APFMonsterController>();

        for (int i = 0; i < monsterGroup.Count; ++i)
        {
            int charTableId = monsterGroup[i];

            APFMonsterController monsterController = PFUtil.CreateGameObject<APFMonsterController>(parent);

            Vector3 vPos = new Vector3(xPos, yPos, 0.0f);
            monsterController.Init(charTableId, vPos);

            monsterControllerList.Add(monsterController);

            xPos += xWidth;
        }

        return monsterControllerList;
    }

    public static List<List<int>> GetMonsterGroupList()
    {
        List<List<int>> monsterGroupList = new List<List<int>>();

        //돈을 먹는자 x 3
        {
            List<int> monsterGroup = new List<int>();
            monsterGroup.Add(20001);
            monsterGroup.Add(20001);
            monsterGroup.Add(20001);

            monsterGroupList.Add(monsterGroup);
        }

        //비난을 먹는자 x 2
        {
            List<int> monsterGroup = new List<int>();
            monsterGroup.Add(20002);
            monsterGroup.Add(20002);

            monsterGroupList.Add(monsterGroup);
        }

        //맹신을 먹는자 x 2
        {
            List<int> monsterGroup = new List<int>();
            monsterGroup.Add(20003);
            monsterGroup.Add(20003);

            monsterGroupList.Add(monsterGroup);
        }

        //돈을 먹는자 x 1, 맹신을 먹는자 x 2
        {
            List<int> monsterGroup = new List<int>();
            monsterGroup.Add(20001);
            monsterGroup.Add(20003);
            monsterGroup.Add(20003);

            monsterGroupList.Add(monsterGroup);
        }

        //비난을 먹는자 x 1, 맹신을 먹는자 x 1
        {
            List<int> monsterGroup = new List<int>();
            monsterGroup.Add(20002);
            monsterGroup.Add(20003);

            monsterGroupList.Add(monsterGroup);
        }

        {
            List<int> monsterGroup = new List<int>();
            monsterGroup.Add(20005);
            monsterGroup.Add(20006);
            monsterGroup.Add(20007);

            monsterGroupList.Add(monsterGroup);
        }

        {
            List<int> monsterGroup = new List<int>();
            monsterGroup.Add(20008);
            monsterGroup.Add(20009);
            monsterGroup.Add(20010);

            monsterGroupList.Add(monsterGroup);
        }

        {
            List<int> monsterGroup = new List<int>();
            monsterGroup.Add(20011);
            monsterGroup.Add(20012);
            monsterGroup.Add(20013);

            monsterGroupList.Add(monsterGroup);
        }

        return monsterGroupList;
    }
}
