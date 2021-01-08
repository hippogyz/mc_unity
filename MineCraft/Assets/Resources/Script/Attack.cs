using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
public enum AttackType { nothing = 0, physics = 1 }; // binary enum

public delegate AttackInfo AttackSkill();
public delegate void SkillEffect();
public struct AttackInfo
{
    public AttackType attackType;
    public int attackPoint;

    public AttackInfo(AttackType atktype, int atkpoint)
    {
        attackType = atktype;
        attackPoint = atkpoint;
    }
}

public class AttackComponent
{
    GameObject mGameObject;
    AttackType attackType;
    int attackPoint;

    int defendPoint;
    AttackType existedResistence = (AttackType) 0;
    Hashtable resistence = new Hashtable();

    public AttackSkill[] attackSkillList;
    public SkillEffect[] attackSkillEffectList;
    public bool[] skillListRegistered;
    public float[] skillTime;
    public static readonly int skillListSize = 5;
    int counterIndex = -1;

    public AttackComponent(GameObject gameObject, AttackType atktype = AttackType.physics, int atkpoint = 0, int dfdpoint = 0)
    {
        mGameObject = gameObject;

        attackType = atktype;
        attackPoint = atkpoint;

        defendPoint = dfdpoint;
        SetResistance(AttackType.physics, 0);

        attackSkillList = new AttackSkill[skillListSize];
        attackSkillEffectList = new SkillEffect[skillListSize];
        skillListRegistered = new bool[skillListSize];
        skillTime = new float[skillListSize];
        for (int i = 0; i < skillListSize; i++)
        {
            skillListRegistered[i] = false;
            skillTime[i] = 0;
        }

        RegisterSkill(BasicAttack, 0);   //  same as RegisterSkill(BasicAttack, EmptyEffect, 0);
    }

    static public void DoAttack(GameObject attacker, GameObject defender, int skillIndex )
    {
        AttackComponent attackerCom = attacker.GetComponent<Basic>().attackCom;

        if (defender == null || (skillIndex >= skillListSize && skillIndex < 0))
        {
            return;
        }

        AttackComponent defenderCom = defender.GetComponent<Basic>().attackCom;

        if ( attackerCom.skillListRegistered[skillIndex])
        {
            AttackInfo attackInfo = attackerCom.attackSkillList[skillIndex]();
            attackerCom.attackSkillEffectList[skillIndex]();

            float resist = defenderCom.GetResistance(attackInfo.attackType);
            float tempDamage = (1.0f - resist) * (float)attackInfo.attackPoint;

            if ((int)(attackInfo.attackType & AttackType.physics) != 0)
            {
                int dfdpoint = defenderCom.GetDefend();
                tempDamage = (tempDamage > dfdpoint) ? tempDamage - dfdpoint : 0;
            }

            //Debug.Log("Damage:" + (int)tempDamage);
            defender.GetComponent<Basic>().ChangeHealth( - (int)tempDamage );
        }
    }

    static public void DoCounter(GameObject attacker, GameObject defender)
    {
        if ( defender.GetComponent<Basic>().health > 0)
        {
            int counterIndex = defender.GetComponent<Basic>().attackCom.CounterIndex();
            DoAttack(defender, attacker, counterIndex);
        }
    }

    static public bool SkillExisted(GameObject attacker, int skillindex)
    {
        if (skillindex < 0 || skillindex >= skillListSize)
        {
            return false;
        }
        else
        {
            return attacker.GetComponent<Basic>().attackCom.skillListRegistered[skillindex];
        }
    }

    static public float GetSkillTime(GameObject attacker, int skillindex)
    {
        return attacker.GetComponent<Basic>().attackCom.skillTime[skillindex];
    }

    public AttackInfo BasicAttack() 
    {
        if (attackPoint != 0)
        {
            return new AttackInfo(attackType, attackPoint);
        }
        else
        {
            return new AttackInfo(AttackType.physics, 0);
        }
    }
    public static void EmptyEffect()
    {
        return;
    }
    public void RegisterSkill(AttackSkill atkskill, int registerNumber = -1, float skilltime = 0.5f)
    {
        int index = registerNumber;

        if (registerNumber == -1)
        {
            index = 0;
            while (index < skillListSize && skillListRegistered[ index ])
            {
                index++;
            }
        }

        if( index < skillListSize && !skillListRegistered[index] )
        {
            attackSkillList[index] = atkskill;
            attackSkillEffectList[index] = EmptyEffect;
            skillListRegistered[index] = true;
            skillTime[index] = skilltime;
        }
    }

    public void RegisterSkill(AttackSkill atkskill, SkillEffect skleffect, int registerNumber = -1, float skilltime = 0.5f)
    {
        int index = registerNumber;

        if (registerNumber == -1)
        {
            index = 0;
            while (index < skillListSize && skillListRegistered[index])
            {
                index++;
            }
        }

        if (index < skillListSize && !skillListRegistered[index])
        {
            attackSkillList[index] = atkskill;
            attackSkillEffectList[index] = skleffect;
            skillListRegistered[index] = true;
            skillTime[index] = skilltime;
        }
    }

    public void SetCounterIndex(int index)
    {
        if (index < skillListSize && index >= 0 && skillListRegistered[index])
        {
            counterIndex = index;
        }
    }

    public int CounterIndex()
    {
        return counterIndex;
    }
    
    public void SetDefend(int dfdpoint)
    {
        defendPoint = dfdpoint;
    }

    public void AddDefend(int dfdpoint)
    {
        defendPoint += dfdpoint;
    }

    int GetDefend()
    {
        return defendPoint;
    }

    public void SetResistance(AttackType atktype, float resist)
    {
        if ((int)(existedResistence & atktype) != 0)
        {
            resistence[atktype] = resist;
        }
        else
        {
            existedResistence |= atktype;
            resistence.Add(atktype, resist);
        }
    }

    public void AddResistance(AttackType atktype, float resist)
    {
        if ((int)(existedResistence & atktype) != 0)
        {
            resistence[atktype] = (float)resistence[atktype] + resist;
        }
        else
        {
            existedResistence |= atktype;
            resistence.Add(atktype, resist);
        }
    }

    float GetResistance(AttackType atktype)
    {
        if ((int)(existedResistence & atktype) != 0)
        {
            return (float)resistence[atktype];
        }
        else
        {
            return 0;
        }
    }
}
