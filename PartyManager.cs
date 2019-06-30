using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    #region Singleton

    public static PartyManager Instance;

    void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }

    #endregion

    public List<BaseCharacter> Party;
    public BattleEncounter Encounter;

    void Start()
    {
        Debug.Log("PartyManager Start");
        Party = new List<BaseCharacter>();

        var c1 = Instantiate(Resources.Load<BaseCharacter>("Characters/Data/Wolf"));


        var sword = Resources.Load<Equipment>("Items/Equipments/Magic Sword");
        EquipmentController.EquipItem(c1, sword);

        Party.Add(c1);
        // Party.Add(Instantiate(Resources.Load<BaseCharacter>("Characters/Data/Cat")));
        // Party.Add(Instantiate(Resources.Load<BaseCharacter>("Characters/Data/Dog")));
        Party.Add(Instantiate(Resources.Load<BaseCharacter>("Characters/Data/Lion")));

        Party.ForEach(c => c.isAlly = true);
    }
}
