using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public string characterName { get; private set; }
    public int maxHP { get; private set; }
    public int currentHP { get; private set; }
    public int attack { get; private set; }
    public int defense { get; private set; }
    public int speed { get; private set; }

    // 角色的行动方法，可以在派生类中实现具体逻辑
    public virtual void PerformAction()
    {
        // 实现角色行动的逻辑
    }

    public Character(string characterName, int maxHP, int attack, int defense)
    {
        this.characterName = characterName;
        this.maxHP = maxHP;
        currentHP = maxHP;
        this.attack = attack;
        this.defense = defense;
    }
}
public class PlayerCharacter : Character
{
    // 玩家角色的特有属性或方法

    // 通过构造函数调用基类构造函数
    public PlayerCharacter(string characterName, int maxHP, int attack, int defense)
        : base(characterName, maxHP, attack, defense)
    {
        // 玩家角色特有的初始化逻辑
    }
}

public class EnemyCharacter : Character
{
    // 敌方角色的特有属性或方法

    // 通过构造函数调用基类构造函数
    public EnemyCharacter(string characterName, int maxHP, int attack, int defense)
        : base(characterName, maxHP, attack, defense)
    {
        // 敌方角色特有的初始化逻辑
    }
}