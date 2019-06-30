using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Anima2D;
using DG.Tweening;

[Serializable]
public class BattleSlot : MonoBehaviour
{
    BattleController battleController;
    private BaseCharacter character;
    public BaseCharacter Character
    {
        get { return character; }
        set
        {
            character = value;
            AddListeners();
        }
    }
    public Text HpText;
    public Text MpText;
    public Text DamageText;

    [HideInInspector]
    public bool IsFrontLine;

    GameObject animaChar;
    Animator charAnimator;
    SpriteMeshInstance[] spriteMeshes;

    public Button button;

    public float CalculatedDamage { get; set; }

    void Awake()
    {
        button = GetComponent<Button>();

        animaChar = (GameObject)Instantiate(Resources.Load("Prefabs/UnityChan"));

        charAnimator = animaChar.GetComponent<Animator>();
        spriteMeshes = animaChar.GetComponentsInChildren<SpriteMeshInstance>();
    }

    void Start()
    {
        battleController = GameObject.Find("BattleController").GetComponent<BattleController>();

        UpdateHpText();
        UpdateMpText();
        DamageText.text = "";
        ResetDamageTaken();

        animaChar.transform.SetParent(transform);
        animaChar.transform.position = transform.position * 0.0065f + new Vector3(-6.5f, -5.2f, 0);

        float charS = 0.25f;
        var isAlly = battleController.BattleModel.AlliesSlots.Any(i => i == this);
        if (isAlly)
            animaChar.transform.localScale = new Vector2(-charS, charS);
        else
            animaChar.transform.localScale = new Vector2(charS, charS);
    }

    public void PlayIdleAnimation()
    {
        charAnimator.Play("Idle");
    }

    public void PlayAttackAnimation()
    {
        charAnimator.SetTrigger("Attack");
    }

    public void MoveToAttackAnimation()
    {
        animaChar.transform.DOMove(Vector3.zero, 1.0f);

        StartCoroutine("CameraZoomIn");

        var posX = gameObject.transform.position.x > 1000 ? -1.2f : 1.2f;
        Camera.main.transform.DOMove(new Vector3(posX, 0, -10), 1.0f);
    }

    public void MoveToInitialPositionAnimation()
    {
        animaChar.transform.DOMove(transform.position * 0.0065f + new Vector3(-6.5f, -5.2f, 0), 1.0f);

        Camera.main.transform.DOMove(new Vector3(0, 0, -10), 1.0f);
        StartCoroutine("CameraZoomOut");
    }

    public IEnumerator CameraZoomIn()
    {
        float increment = 0;
        while (increment < 1)
        {
            increment += Time.deltaTime;
            if (increment > 1)
            {
                increment = 1;
            }

            Camera.main.orthographicSize = 5 - increment;

            yield return null;
        }
    }

    public IEnumerator CameraZoomOut()
    {
        float increment = 0;
        while (increment < 1)
        {
            increment += Time.deltaTime;
            if (increment > 1)
            {
                increment = 1;
            }

            Camera.main.orthographicSize = 4 + increment;

            yield return null;
        }
    }

    public void PlayDefenseAnimation()
    {
        charAnimator.Play("Defend");
        var sparkParticle = (GameObject)Instantiate(Resources.Load("Prefabs/Particles/Spark/Prefabs/ElectricalSparksEffect"), transform);
        sparkParticle.transform.position = transform.position * 0.0065f + new Vector3(-6.5f, -5.2f, 0);
    }

    public void ResetDamageTaken()
    {
        CalculatedDamage = 0;
        DamageText.DOFade(0, 1);
    }

    public void ApplyDamage(int damage, int mana)
    {
        CalculatedDamage += damage;
        DamageText.text = "" + Mathf.Abs(CalculatedDamage);
        DamageText.color = CalculatedDamage >= 0 ? Color.red : Color.green;

        // TODO : Mana Text

        if (damage != 0) Character.CurrentHp -= damage;
        if (mana != 0) Character.CurrentMp -= mana;
    }

    public void AddListeners()
    {
        character.OnCurrentHpChanged += UpdateHpText;
        character.OnCurrentMpChanged += UpdateMpText;
        character.OnStatusChanged += UpdateStatusIcons;
    }

    public void RemoveListeners()
    {
        character.OnCurrentHpChanged -= UpdateHpText;
        character.OnCurrentMpChanged -= UpdateMpText;
        character.OnStatusChanged -= UpdateStatusIcons;
    }

    public void UpdateHpText()
    {
        HpText.text = Character.CurrentHp + "/" + Character.MaxHp;
    }

    public void UpdateMpText()
    {
        MpText.text = "" + Character.CurrentMp;
    }

    void UpdateStatusIcons()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "StatusIcon(Clone)")
                GameObject.Destroy(child.gameObject);
        }

        Vector2[] positions = {
            new Vector2(-85, 85), new Vector2(0, 85), new Vector2(85, 85)
        };

        var index = 0;
        foreach (var status in character.Status)
        {
            var statusIcon = (GameObject)Instantiate(Resources.Load("Prefabs/StatusIcon"));
            statusIcon.transform.SetParent(transform);
            statusIcon.transform.localPosition = positions[index++];

            statusIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Status/Images/" + status.Id);
            statusIcon.GetComponentInChildren<Text>().text = "" + status.Turns;
        }
    }

    public void Selected()
    {
        if (!Character.IsAlive())
            return;

        List<BattleSlot> slots = new List<BattleSlot> { this };
        if (battleController.BattleModel.CurrentSkill.Target == kTarget.AllEnemies)
            slots = battleController.BattleModel.EnemiesSlots;
        else if (battleController.BattleModel.CurrentSkill.Target == kTarget.AllAllies)
            slots = battleController.BattleModel.AlliesSlots;

        battleController.UseSkill(slots);
    }

    public void SetTargetable(bool targetable)
    {
        button.interactable = targetable;
        LightUpCharacter(targetable);
    }

    public void LightUpCharacter(bool selectable)
    {
        float newColor = selectable ? 1.0f : 0.3f;

        foreach (var mesh in spriteMeshes)
        {
            mesh.color = new Color(newColor, newColor, newColor);
        }
    }

    private IEnumerator DeathAnimation()
    {
        float increment = 0;
        while (increment < 1)
        {

            increment += (2.5f) * Time.deltaTime;
            if (increment > 1)
            {
                increment = 1;
            }

            float light = 1 - (0.75f * increment);

            foreach (var mesh in spriteMeshes)
            {
                mesh.color = new Color(light, light, light);
            }
            // Icon.color = new Color(light, light, light);

            yield return null;
        }
    }

    public IEnumerator ReviveAnimation()
    {
        float increment = 0;
        while (increment < 1)
        {

            increment += (2.5f) * Time.deltaTime;
            if (increment > 1)
            {
                increment = 1;
            }

            float light = (0.75f * increment);

            foreach (var mesh in spriteMeshes)
            {
                mesh.color = new Color(light, light, light);
            }

            yield return null;
        }
    }

    private IEnumerator DamageTextFadeOut()
    {
        float increment = 0;
        while (increment < 1)
        {
            increment += Time.deltaTime;
            if (increment > 1)
            {
                increment = 1;
            }

            float alpha = (1.0f - increment);

            Color newColor = DamageText.color;
            newColor.a = alpha;
            DamageText.color = newColor;

            yield return null;
        }

        DamageText.text = "";
    }
}