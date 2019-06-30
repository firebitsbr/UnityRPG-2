using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using DG.Tweening;

public class TurnBar : MonoBehaviour
{
    public List<Image> icons = new List<Image>();

    public void UpdateDelay(BaseCharacter currentCharacter)
    {
        currentCharacter.CurrentDelay = currentCharacter.MaxDelay;
    }

    public BaseCharacter UpdateOrder(List<BaseCharacter> allCharacters)
    {
        var orderedTurns = UpdateTurnOrder(allCharacters);
        UpdateIcons(orderedTurns);
        return orderedTurns[0].Character;
    }

    public BaseCharacter GetNextCharacter(List<BaseCharacter> allCharacters)
    {
        var orderedTurns = UpdateTurnOrder(allCharacters);
        BaseCharacter nextChar = orderedTurns[0].Character;
        // nextChar.CurrentDelay = nextChar.MaxDelay;

        MoveIcons(orderedTurns);

        return nextChar;
    }

    private void MoveIcons(List<TurnOrder> orderedTurns)
    {
        for (int index = 0; index < 11; index++)
        {
            icons[index].transform.DOBlendableMoveBy(new Vector3(-184, 0, 0), 1).OnComplete(() => UpdateIcons(orderedTurns));
        }
    }

    List<TurnOrder> UpdateTurnOrder(List<BaseCharacter> allCharacters)
    {
        var turns = new List<TurnOrder>();

        var aliveCharacters = allCharacters.Where(e => e.IsAlive());

        foreach (var character in aliveCharacters)
        {
            // O décimo turno de um char pode ser mais rapido do que o primeiro turno de outro
            for (int index = 0; index < 11; index++)
            {
                character.MaxDelay = 1.0f / character.Speed;

                float time = character.CurrentDelay + (character.MaxDelay * index);
                // Debug.Log(character.Name + " / Speed: " + character.Speed + " / Time: " + time);

                turns.Add(new TurnOrder(character, time));
            }
        }

        turns = turns.OrderBy(e => e.Time).ToList();

        float smallestDelay = turns[0].Character.CurrentDelay;

        foreach (var character in allCharacters)
            character.CurrentDelay -= smallestDelay;

        return turns;
    }

    public void UpdateIcons(List<TurnOrder> orderedTurns)
    {
        for (int index = 0; index < 11; index++)
        {
            icons[index].sprite = orderedTurns[index].Character.Icon;

            icons[index].color = orderedTurns[index].Character.isAlly ? Color.white : Color.red;

            // https://forum.unity.com/threads/in-2019-1-changes-to-image-color-are-not-displayed-after-changing-its-sprite-via-c.665686/
            icons[index].DisableSpriteOptimizations();

            // TODO: Para resetar as posicoes dos icones
            GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(false);
            GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(true);
        }
    }

    public class TurnOrder
    {
        public BaseCharacter Character { get; set; }
        public float Time { get; set; }

        public TurnOrder(BaseCharacter character, float time)
        {
            Character = character;
            Time = time;
        }
    }
}
