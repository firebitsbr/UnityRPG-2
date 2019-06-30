using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillsPanel : MonoBehaviour
{
    public GameObject IconsPanel;
    public Image TargetIcon;
    public Image CostIcon;
    public Image ElementIcon;
    public Image RangedIcon;
    public Image CastIcon;
    public Text NameText;
    public Text DescriptionText;

    List<GameObject> SkillButtons = new List<GameObject>();
    BattleController battleController;

    void Awake()
    {
        battleController = GameObject.Find("BattleController").GetComponent<BattleController>();

        for (int index = 0; index < 10; index++)
        {
            var skillButton = (GameObject)Instantiate(Resources.Load("Prefabs/SkillButton"));
            skillButton.transform.SetParent(IconsPanel.transform);
            SkillButtons.Add(skillButton);
        }
    }

    public void DisplaySkills(BaseCharacter character)
    {
        gameObject.SetActive(true);

        int index = 0;
        foreach (var skill in character.Skills)
        {
            var skillButton = SkillButtons[index++];

            Button button = skillButton.GetComponent<Button>();
            button.interactable = skill.HasCostRequired(character);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate ()
            {
                battleController.SelectSkill(skill);
            });

            skillButton.GetComponent<Image>().sprite = skill.Icon;
        }

        while (index < 10)
            ResetButton(SkillButtons[index++]);

        battleController.SelectSkill(character.Skills[0]);
    }

    void ResetButton(GameObject skillButton)
    {
        Button button = skillButton.GetComponent<Button>();
        button.interactable = false;

        skillButton.GetComponent<Image>().sprite = null;
        //        skillButton.GetComponentInChildren<Text>().text = "";
    }

    public void DisableButtons()
    {
        foreach (var skillButton in SkillButtons)
        {
            Button button = skillButton.GetComponent<Button>();
            button.interactable = false;
        }
    }

    public void DisplaySkillInfo(BaseSkill currentSkill)
    {
        Debug.Log("DisplaySkillInfo");
        LoadSkillInfoIcon(TargetIcon, true, "Skills/Images/" + currentSkill.Target);
        LoadSkillInfoIcon(CostIcon, currentSkill.CostValue > 0, "Skills/Images/" + currentSkill.CostType);
        LoadSkillInfoIcon(ElementIcon, currentSkill.Element != kElements.None, "Skills/Images/" + currentSkill.Element);
        LoadSkillInfoIcon(RangedIcon, currentSkill.IsRanged, "Skills/Images/Ranged");
        LoadSkillInfoIcon(CastIcon, currentSkill.CastTime > 0, "Skills/Images/Cast");

        TextMeshProUGUI costValueText = CostIcon.GetComponentInChildren<TextMeshProUGUI>();
        costValueText.SetText("" + currentSkill.CostValue);
        TextMeshProUGUI castValueText = CastIcon.GetComponentInChildren<TextMeshProUGUI>();
        castValueText.SetText("" + currentSkill.CastTime);

        NameText.text = currentSkill.Name;
        DescriptionText.text = currentSkill.Description;
    }

    void LoadSkillInfoIcon(Image image, bool active, string path)
    {
        image.gameObject.SetActive(active);
        if (active)
            image.sprite = Resources.Load<Sprite>(path);
    }
}
