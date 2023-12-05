using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


// 技能数据模型
[Serializable]
public class Skill
{
    public string skillName;
    public int manaCost;
    public int damage;
    // 可以添加其他属性，如冷却时间、技能类型等
}
[System.Serializable]
public class SkillConfig
{
    public List<Skill> skills;
}

// 技能管理器
public class SkillManager
{
    private Dictionary<string, Skill> skillDatabase = new Dictionary<string, Skill>();

    public void AddSkill(string skillName, Skill skill)
    {
        skillDatabase[skillName] = skill;
    }

public void LoadSkillsFromJson(string jsonPath)
{
    try
    {
        SkillConfig skillConfig = JsonReader.LoadJsonFile<SkillConfig>(jsonPath);

        if (skillConfig != null && skillConfig.skills != null)
        {
            foreach (var skill in skillConfig.skills)
            {
                AddSkill(skill.skillName, skill);
            }

            Debug.Log("技能加载成功");
        }
        else
        {
            Debug.LogError("无法加载或解析技能配置");
        }
    }
    catch (Exception e)
    {
        Debug.LogError($"加载技能配置时发生错误: {e.Message}");
    }
}
}

// 角色技能系统
public class CharacterSkillSystem
{
    private List<Skill> equippedSkills = new List<Skill>();

    public void EquipSkill(Skill skill)
    {
        equippedSkills.Add(skill);
    }

    public void UseSkill(string skillName)
    {
        Skill skill = equippedSkills.Find(s => s.skillName == skillName);

        if (skill != null)
        {
            // 在这里执行技能效果，例如伤害敌人、治疗己方等
            Console.WriteLine($"Using skill: {skill.skillName}, Damage: {skill.damage}");
        }
        else
        {
            Console.WriteLine($"Skill not equipped: {skillName}");
        }
    }
}


