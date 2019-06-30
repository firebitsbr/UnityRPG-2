using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EncounterEnemy
{
    public BaseCharacter Enemy;
    public bool IsFrontLine;
    public Vector2 Position;

    void OnEnable()
    {
        Debug.Log("EncounterEnemy OnEnable: ");

        Enemy = UnityEngine.Object.Instantiate(Enemy);
    }
}

[CreateAssetMenu(menuName = "RPG/Encounter")]
public class BattleEncounter : ScriptableObject
{
    public List<EncounterEnemy> Enemies;


}
