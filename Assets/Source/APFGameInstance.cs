using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APFGameInstance : MonoBehaviour
{
    [SerializeField]
    private UIRoot PFUIRoot;


    public static APFGameMode PFGameMode;


    private void Awake()
    {
        PFTable.LoadTables();

        CreateGameMode();
    }

    private void CreateGameMode()
    {
        APFGameMode gamemode_original = Resources.Load<APFGameMode>(PFPrefabPath.GameMode);

        PFGameMode = PFUtil.Instantiate(gamemode_original, PFUIRoot.gameObject);
    }

    private void Start()
    {
        PFGameMode.Init();
    }
}
