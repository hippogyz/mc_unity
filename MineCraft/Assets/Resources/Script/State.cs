using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputOrder
{
    stand, jump, squat, walk
}

//CharacterState Part
abstract public class CharacterState
{
    abstract public CharacterState UpdataState(Character character, InputOrder input);
    abstract public void DoSomething(Character character, float deltaTime);
}

public class IdleState : CharacterState
{
    public override CharacterState UpdataState(Character character, InputOrder input)
    {
        CharacterState tempState = this;

        switch (input)
        {
            case InputOrder.jump:
                character.DoJump();
                tempState = new JumpState();
                break;
            case InputOrder.walk:
                tempState = new WalkState();
                break;
            case InputOrder.squat:
                tempState = new SquatState();
                break;
        }

        return tempState;
    }

    public override void DoSomething(Character character, float deltaTime)
    {
        // Debug.Log("Idle State Now");
        character.ResetPosture();
        character.Walk(0, 0.01f);
    }
}
public class JumpState : CharacterState
{
    public override CharacterState UpdataState(Character character, InputOrder input)
    {
        CharacterState tempState = this;

        if (character.OnGround())
        {
            //Debug.Log("On Ground Now.");
            switch (input)
            {
                case InputOrder.stand:
                    tempState = new IdleState();
                    break;
                case InputOrder.walk:
                    tempState = new WalkState();
                    break;
                case InputOrder.squat:
                    tempState = new SquatState();
                    break;
                default:
                    tempState = new IdleState();
                    break;
            }
        }

        return tempState;
    }
    public override void DoSomething(Character character, float deltaTime)
    {
        // Debug.Log("Jump State Now");
        character.Walk(1.0f, deltaTime);
        character.WalkPosture(deltaTime);
        //freeze posture
    }
}
public class WalkState : CharacterState
{
    public override CharacterState UpdataState(Character character, InputOrder input)
    {
        CharacterState tempState = this;

        if (!character.OnGround())
        {
            tempState = new JumpState();
        }
        else
        {
            switch (input)
            {
                case InputOrder.stand:
                    tempState = new IdleState();
                    break;
                case InputOrder.jump:
                    character.DoJump();
                    tempState = new JumpState();
                    break;
                case InputOrder.squat:
                    tempState = new SquatState();
                    break;
            }
        }

        return tempState;
    }
    public override void DoSomething(Character character, float deltaTime)
    {
        // Debug.Log("Walk State Now");
        character.Walk(1.0f, deltaTime);
        character.WalkPosture(deltaTime);
        //walk posture
    }
}
public class SquatState : CharacterState
{
    public override CharacterState UpdataState(Character character, InputOrder input)
    {
        CharacterState tempState = this;

        if (!character.OnGround())
        {
            tempState = new JumpState();
        }
        else
        {
            switch (input)
            {
                case InputOrder.stand:
                    tempState = new IdleState();
                    break;
                case InputOrder.jump:
                    character.DoJump();
                    tempState = new JumpState();
                    break;
                case InputOrder.walk:
                    tempState = new WalkState();
                    break;
            }
        }

        return tempState;
    }

    public override void DoSomething(Character character, float deltaTime)
    {
        // Debug.Log("Squat State Now");
        character.Walk(0.5f, deltaTime);
        character.DoSquat();
        //freeze posture
    }
}

//parallel state
abstract public class DelayState // state with cooldown motion
{
    protected float cdTime;
    abstract public DelayState UpdateState(Character character, int attackOrder);
    abstract public void DoSomething(Character character, float deltaTime);
    abstract public void PreDoSomething(Character character, float deltaTime); // call before UpdateState
}

public class IdleDelayState : DelayState
{
    override public DelayState UpdateState(Character character, int attackOrder)
    {
        if (attackOrder == -2)
        {
            return new UseItemState(); 
        }
        else if ( AttackComponent.SkillExisted(character.gameObject, attackOrder) )
        {
            return new AttackState(character, attackOrder);
        }
        else
        {
            return this;
        }
    }

    override public void DoSomething(Character character, float deltaTime)
    {
        // nothing to do
    }

    override public void PreDoSomething(Character character, float deltaTime)
    {
        //nothing to predo
    }
}

public class AttackState : DelayState
{
    GameObject defender;
    int attackOrder;
    bool attacked;

    public AttackState(Character character, int attackOrder)
    {
        this.attackOrder = attackOrder;
        attacked = false;
        cdTime = AttackComponent.GetSkillTime(character.gameObject, attackOrder);
    }

    override public DelayState UpdateState(Character character, int attackOrder)
    {
        DelayState tempState = this;

        if (cdTime <= 0)
        {
            if (attackOrder == -2)
            {
                return new UseItemState();
            }
            else if (AttackComponent.SkillExisted(character.gameObject, attackOrder))
            {
                tempState = new AttackState(character, attackOrder);
            }
            else
            {
                tempState = new IdleDelayState();
                character.ExitAttackPosture();
            }
        }

        return tempState;
    }

    override public void DoSomething(Character character, float deltaTime)
    {
        character.AttackPosture(deltaTime);

        if ( !attacked )
        {
            defender = character.FindDefender(this.attackOrder);
        }
    }

    override public void PreDoSomething(Character character, float deltaTime)
    {
        if ( !attacked )
        {
            character.DoAttack(defender, attackOrder);
            attacked = true;
        }

        cdTime -= deltaTime;
    }
}

public class UseItemState : DelayState // while attack order = -2, it is triggered by right click, and it means using item
{
    GameObject target;
    bool used;
    public UseItemState()
    {
        cdTime = 0.25f;
        used = false;
        target = null;
    }
    public override DelayState UpdateState(Character character, int attackOrder)
    {
        DelayState tempState = this;

        if (cdTime <= 0)
        {
            if ( attackOrder == -2 )
            {
                tempState = new UseItemState();
            }
            else if (AttackComponent.SkillExisted(character.gameObject, attackOrder))
            {
                tempState = new AttackState(character, attackOrder);
            }
            else
            {
                tempState = new IdleDelayState();
                character.ExitAttackPosture();
            }
        }

        return tempState;
    }
    override public void DoSomething(Character character, float deltaTime)
    {
        character.AttackPosture(deltaTime);

        if (!used)
        {
            target = character.FindItemTarget();
        }
    }

    override public void PreDoSomething(Character character, float deltaTime)
    {
        if (!used)
        {
            character.UseHoldingItem(target);
            used = true;
        }

        cdTime -= deltaTime;
    }
}
