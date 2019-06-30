using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FramesPanel : MonoBehaviour
{

    public BattleController battleController;
    public SkillsPanel skillPanel;
    public GameObject actionBar;

    float perfectPrecision = .5f;//2
    float greatPrecision = 1.0f;//5
    float goodPrecision = 1.5f;

    List<FrameInfo> buttons = new List<FrameInfo>();

    public class FrameInfo
    {
        public Image Button;
        public SkillFrame Frame;
    };

    struct AttackInfo
    {
        public float Speed;
        public float FinalX;

        public float PerfectMultiplier;
        public float GreatMultiplier;
        public float GoodMultiplier;
        public float MissMultiplier;
    }
    AttackInfo attackInfo;

    public void Init(BaseSkill skill, bool isAlly)
    {
        gameObject.SetActive(true);
        skillPanel.gameObject.SetActive(false);

        CreateAttackInfo(isAlly);

        actionBar.transform.localPosition = new Vector2(attackInfo.FinalX, actionBar.transform.localPosition.y);

        foreach (var frame in skill.Frames)
        {
            GameObject obj = new GameObject();
            Image image = obj.AddComponent<Image>();
            image.sprite = Resources.Load<Sprite>("FrameBar/" + frame.Size + frame.Button);
            image.transform.SetParent(transform);
            image.name = frame.Button;
            image.transform.localPosition = new Vector2(attackInfo.FinalX + frame.Time * attackInfo.Speed, GetPosYForButton(frame.Button));
            image.SetNativeSize();

            FrameInfo frameInfo = new FrameInfo();
            frameInfo.Button = image;
            frameInfo.Frame = frame;
            buttons.Add(frameInfo);
        }
    }

    void CreateAttackInfo(bool isAlly)
    {
        attackInfo.Speed = isAlly ? 200 : -200;
        attackInfo.FinalX = isAlly ? -800 : 800;

        attackInfo.PerfectMultiplier = isAlly ? 1.0f : .2f;
        attackInfo.GreatMultiplier = isAlly ? .6f : .4f;
        attackInfo.GoodMultiplier = isAlly ? .4f : .6f;
        attackInfo.MissMultiplier = isAlly ? .2f : 1.0f;
    }

    float GetPosYForButton(string button)
    {
        if (button.Equals("A"))
            return 350;

        if (button.Equals("S"))
            return 250;

        if (button.Equals("D"))
            return 150;

        return 50;
    }

    void Update()
    {
        InputButtons();

        RemoveButtons();

        MoveButtons();

        if (buttons.Count <= 0)
            EndSkill();
    }

    void InputButtons()
    {
        var pressedButtons = GetPressedButtons();
        List<FrameInfo> removeList = new List<FrameInfo>();

        foreach (var pressedButton in pressedButtons)
        {
            var button = buttons.Where(e => e.Button.name == pressedButton).FirstOrDefault();
            if (button == null)
                continue;

            float posX = button.Button.transform.localPosition.x;
            float width = button.Button.rectTransform.rect.width;
            float halfWidth = width * .5f;
            float buttonArea = width * goodPrecision;

            if (posX - buttonArea <= attackInfo.FinalX && posX + buttonArea >= attackInfo.FinalX)
            {
                float precision = Mathf.Abs(posX - attackInfo.FinalX) / halfWidth;

                if (precision <= perfectPrecision)
                {
                    battleController.CreateLabel("Perfect", attackInfo.FinalX);
                    ApplyDamage(button.Frame.Multiplier * attackInfo.PerfectMultiplier);
                }
                else if (precision <= greatPrecision)
                {
                    battleController.CreateLabel("Great", attackInfo.FinalX);
                    ApplyDamage(button.Frame.Multiplier * attackInfo.GreatMultiplier);
                }
                else
                {
                    battleController.CreateLabel("Good", attackInfo.FinalX);
                    ApplyDamage(button.Frame.Multiplier * attackInfo.GoodMultiplier);
                }

                removeList.Add(button);
            }
        }

        foreach (var button in removeList)
        {
            Destroy(button.Button.gameObject);
            buttons.Remove(button);
        }
    }

    void ApplyDamage(float damage)
    {
        battleController.ApplyDamage(damage);
    }

    List<string> GetPressedButtons()
    {
        List<string> pressedButtons = new List<string>();

        if (Input.GetKeyDown(KeyCode.A))
            pressedButtons.Add("A");

        if (Input.GetKeyDown(KeyCode.S))
            pressedButtons.Add("S");

        if (Input.GetKeyDown(KeyCode.D))
            pressedButtons.Add("D");

        if (Input.GetKeyDown(KeyCode.F))
            pressedButtons.Add("F");

        return pressedButtons;
    }

    void RemoveButtons()
    {
        List<FrameInfo> removeList = new List<FrameInfo>();

        foreach (var button in buttons)
        {
            if (button.Button.transform.localPosition.x <= -900 || button.Button.transform.localPosition.x >= 900)
            {
                battleController.CreateLabel("Miss", attackInfo.FinalX);
                ApplyDamage(button.Frame.Multiplier * attackInfo.MissMultiplier);

                removeList.Add(button);
                Destroy(button.Button.gameObject);
            }
        }

        foreach (var button in removeList)
        {
            buttons.Remove(button);
        }
    }

    void MoveButtons()
    {
        foreach (var button in buttons)
        {
            float curX = button.Button.transform.position.x;
            float curY = button.Button.transform.position.y;
            button.Button.transform.position = new Vector2(curX - (Time.deltaTime * attackInfo.Speed), curY);
        }
    }

    void EndSkill()
    {
        //Debug.Log("Frames EndSkill");

        gameObject.SetActive(false);
        battleController.EndAttack();
    }
}