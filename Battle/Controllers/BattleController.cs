using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public GameObject canvas;
    public TurnBar turnBar;
    public SkillsPanel skillsPanel;
    public FramesPanel framesPanel;

    public GameObject skillNamePanel;

    [HideInInspector]
    public BattleModel BattleModel = new BattleModel();

    void Start()
    {
        Debug.Log("BattleController Start");

        CreateAlliesBattleSlots();
        CreateEnemyBattleSlots();

        BattleModel.CurrentChar = turnBar.UpdateOrder(BattleModel.AllCharacters);
        BattleModel.CurrentSlot = BattleModel.AllSlots.Where(s => s.Character == BattleModel.CurrentChar).FirstOrDefault();
        Invoke("ContinueTurn", 1);
    }

    void CreateAlliesBattleSlots()
    {
        Vector2[] positions = {
            new Vector2(400, 300), new Vector2(450, 70), new Vector2(500, -200),
            new Vector2(800, -20),  new Vector2(800, -100)
        };

        var allies = PartyManager.Instance.Party;

        for (int index = 0; index < allies.Count; index++)
        {
            var pos = positions[index];
            var character = allies[index];

            character.ResetValues();
            BattleSlot battleSlot = CreateBattleSlot(character, pos);
            battleSlot.IsFrontLine = index < 3;
            BattleModel.AlliesSlots.Add(battleSlot);
        }
    }
    void CreateEnemyBattleSlots()
    {
        foreach (var enemy in PartyManager.Instance.Encounter.Enemies)
        {
            enemy.Enemy.Awake();
            BattleSlot battleSlot = CreateBattleSlot(enemy.Enemy, enemy.Position);
            battleSlot.IsFrontLine = enemy.IsFrontLine;
            BattleModel.EnemiesSlots.Add(battleSlot);
        }
    }

    private BattleSlot CreateBattleSlot(BaseCharacter character, Vector2 pos)
    {
        var battleSlotPrefab = (GameObject)Instantiate(Resources.Load("Prefabs/BattleSlot"));
        var battleSlot = battleSlotPrefab.GetComponent<BattleSlot>();
        battleSlot.Character = character;
        battleSlot.transform.SetParent(canvas.transform);
        battleSlot.transform.localPosition = pos;

        BattleModel.AllCharacters.Add(character);
        BattleModel.AllSlots.Add(battleSlot);

        return battleSlot;
    }

    public void NextChar()
    {
        BattleModel.CurrentChar = turnBar.GetNextCharacter(BattleModel.AllCharacters);
        BattleModel.CurrentSlot = BattleModel.AllSlots.Where(s => s.Character == BattleModel.CurrentChar).FirstOrDefault();

        Invoke("ContinueTurn", 1);
    }

    private void ContinueTurn()
    {
        Debug.Log("<color=green>Next Character: " + BattleModel.CurrentChar.Name + "</color>");

        if (!OnTurnStart())
            return;

        BattleModel.CurrentChar.UpdateStatus();

        if (BattleModel.CurrentChar.IsCasting)
        {
            BattleModel.CurrentSkill = BattleModel.CurrentChar.castingInfo.Skill;
            UseSkill(BattleModel.CurrentChar.castingInfo.Targets);
            return;
        }

        if (BattleModel.CurrentChar.isAlly)
            skillsPanel.DisplaySkills(BattleModel.CurrentChar);
        else
            EnemyTurn();
    }

    private bool OnTurnStart()
    {
        if (BattleModel.CurrentChar.HasStatus(kStatus.Regen))
        {
            BattleModel.CurrentSlot.ApplyDamage((int)(BattleModel.CurrentChar.MaxHp * -0.05f), (int)(BattleModel.CurrentChar.MaxHp * -0.05f));
            BattleModel.CurrentSlot.ResetDamageTaken();
        }

        if (BattleModel.CurrentChar.HasStatus(kStatus.Dot))
        {
            BattleModel.CurrentSlot.ApplyDamage((int)(BattleModel.CurrentChar.MaxHp * 0.10f), 0);

            if (AfterTakeDamage(new List<BattleSlot>() { BattleModel.CurrentSlot }))
                return false;
        }

        if (BattleModel.CurrentChar.HasStatus(kStatus.Stun) || !BattleModel.CurrentChar.IsAlive())
        {
            BattleModel.CurrentChar.UpdateStatus();
            NextChar();
            return false;
        }

        return true;
    }

    public void EnemyTurn()
    {
        BattleModel.CurrentSkill = BattleModel.CurrentChar.Skills.OrderBy(n => UnityEngine.Random.value).FirstOrDefault();
        UseSkill(EnemyController.GetTargetsForEnemySkill(BattleModel));
    }

    public void SelectSkill(BaseSkill skill)
    {
        BattleModel.CurrentSkill = skill;

        BattleTargetController.EnableTargetableCharacterSlots(BattleModel);
        skillsPanel.DisplaySkillInfo(BattleModel.CurrentSkill);
    }

    public void UseSkill(List<BattleSlot> targetSlots)
    {
        if (BattleModel.CurrentSkill.CastTime > 0)
        {
            if (!BattleModel.CurrentChar.IsCasting)
            {
                BattleModel.CurrentChar.StartCasting(BattleModel.CurrentSkill, targetSlots);
                skillsPanel.gameObject.SetActive(false);
                skillNamePanel.GetComponentInChildren<Text>().text = "Casting: " + BattleModel.CurrentSkill.Name;
                skillNamePanel.SetActive(true);
                Invoke("EndAction", 1);
                return;
            }

            BattleModel.CurrentChar.IsCasting = false;
            targetSlots.RemoveAll(e => !e.Character.IsAlive());
            if (targetSlots.Count == 0)
            {
                skillNamePanel.GetComponentInChildren<Text>().text = "Cast Failed";
                skillNamePanel.SetActive(true);
                Invoke("EndAction", 1);
                return;
            }
        }

        Debug.Log("<color=red>Skill: " + BattleModel.CurrentSkill.Name + "</color>");

        skillNamePanel.GetComponentInChildren<Text>().text = BattleModel.CurrentSkill.Name;
        skillNamePanel.SetActive(true);

        BattleModel.TargetSlots = targetSlots;

        var aliveSlots = BattleModel.AllSlots.Where(e => e.Character.IsAlive());
        foreach (var slot in aliveSlots)
        {
            slot.button.interactable = false;
            slot.LightUpCharacter(true);
        }

        BattleModel.CurrentSkill.TakeCost(BattleModel.CurrentChar);
        StatusController.ApplyStatus(targetSlots, BattleModel.CurrentSkill.Status);
        BuffsController.ApplyBuffs(targetSlots, BattleModel.CurrentSkill.Buffs);

        var isAlly = BattleModel.CurrentChar.isAlly;
        if (isAlly)
        {
            if (BattleModel.CurrentSkill.Target == kTarget.AllAllies
            || BattleModel.CurrentSkill.Target == kTarget.SingleAlly
            || BattleModel.CurrentSkill.Target == kTarget.Self)
                isAlly = !isAlly;
        }

        BattleModel.CurrentSlot.MoveToAttackAnimation();

        framesPanel.Init(BattleModel.CurrentSkill, isAlly);
    }

    public void ApplyDamage(float precisionMultiplier)
    {
        var currentSlot = BattleModel.AllSlots.Where(e => e.Character == BattleModel.CurrentChar).FirstOrDefault();
        currentSlot.PlayAttackAnimation();

        foreach (var slot in BattleModel.TargetSlots)
        {
            float preDefDamage = (precisionMultiplier * BattleModel.CurrentSkill.CalculateTotalHp(BattleModel.CurrentChar, slot.Character));
            //Debug.Log("PreDefDamage " + preDefDamage);
            int totalHp = (int)slot.Character.CalculateDefense(preDefDamage, BattleModel.CurrentSkill.Element);
            int totalMp = (int)(precisionMultiplier * BattleModel.CurrentSkill.CalculateTotalMp(BattleModel.CurrentChar, slot.Character));
            slot.ApplyDamage(totalHp, totalMp);

            slot.PlayDefenseAnimation();
        }
    }

    private bool AfterTakeDamage(List<BattleSlot> damageTakers)
    {
        var revivingCharactersSlots = damageTakers.Where(e => !e.Character.IsAlive() && e.Character.HasStatus(kStatus.Reraise));
        foreach (var slot in revivingCharactersSlots)
        {
            slot.Character.CurrentHp = (int)(slot.Character.MaxHp * 0.2f);
            slot.StartCoroutine("ReviveAnimation");
        }

        foreach (var slot in damageTakers)
        {
            if (!slot.Character.IsAlive())
                slot.StartCoroutine("DeathAnimation");
            else
                slot.PlayIdleAnimation();

            slot.ResetDamageTaken();
        }

        return CheckBattleEnd();
    }

    public void EndAttack()
    {
        var currentSlot = BattleModel.AllSlots.Where(e => e.Character == BattleModel.CurrentChar).FirstOrDefault();
        currentSlot.MoveToInitialPositionAnimation();
        currentSlot.PlayIdleAnimation();

        if (AfterTakeDamage(BattleModel.TargetSlots))
            return;

        turnBar.UpdateOrder(BattleModel.AllCharacters);
        turnBar.UpdateDelay(BattleModel.CurrentChar);

        EndAction();
    }

    public void EndAction()
    {
        BattleModel.CurrentSkill = null;

        skillNamePanel.SetActive(false);

        Invoke("NextChar", 1);
    }

    bool CheckBattleEnd()
    {
        if (!GroupAlive(BattleModel.AlliesSlots))
        {
            Invoke("BattleLose", 1);
            return true;
        }

        if (!GroupAlive(BattleModel.EnemiesSlots))
        {
            Invoke("BattleWin", 1);
            return true;
        }

        return false;
    }

    bool GroupAlive(List<BattleSlot> group)
    {
        foreach (var slot in group)
        {
            if (slot.Character.IsAlive())
                return true;
        }

        return false;
    }

    private void BattleLose()
    {
        throw new NotImplementedException();
    }

    private void BattleWin()
    {
        int totalExp = 0;
        foreach (var slot in BattleModel.EnemiesSlots)
            totalExp += slot.Character.Exp;

        var aliveAllies = BattleModel.AlliesSlots.Where(e => e.Character.IsAlive());
        foreach (var slot in aliveAllies)
            slot.Character.AddExp(totalExp);

        foreach (var slot in BattleModel.AlliesSlots)
            slot.RemoveListeners();

        var currentScene = SceneManager.GetSceneByName("BattleScene");
        SceneManager.UnloadSceneAsync(currentScene);
        SceneManager.sceneUnloaded += LoadMap;
    }

    public void LoadMap(Scene scene)
    {
        var mapScene = SceneManager.GetSceneAt(0);
        foreach (var obj in mapScene.GetRootGameObjects())
        {
            obj.SetActive(true);
        }
    }

    public void CreateLabel(string text, float posX)
    {
        var obj = new GameObject();

        var image = obj.AddComponent<Image>();
        image.sprite = Resources.Load<Sprite>("FrameBar/" + text + "Label");
        image.preserveAspect = true;
        image.transform.SetParent(canvas.transform);
        image.transform.localPosition = new Vector2(posX, -300);
        image.SetNativeSize();

        image.MoveBy(1, new Vector2(0, 100));
        image.RemoveSelf(1);
    }
}

