using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


//public enum InputOrder    see Character.cs
//public enum SystemOrder
abstract public class CharacterIdentity
{
    public bool isPlayer = false;
    abstract public Vector3 GetWalkingDirection();
    abstract public InputOrder GetInputOrder();
    abstract public Vector3 GetSightRotation( GameObject owner = null, GameObject target = null );
    abstract public int GetAttackOrder();
    abstract public bool GetThrowOrder();
    abstract public SystemOrder GetSystemOrder();
    protected int systemOrderProtectedCount = 0;
}

public enum SystemOrder // binary
{ 
    NOTHING = 0,
    SYSTEM_MENU = 1,
    ITEMBOX = 2,
    OTHER = 4 // this term should never be called
}

public class PlayerCharacterIdentity : CharacterIdentity 
{
    public PlayerCharacterIdentity()
    {
        isPlayer = true;
    }
    public override Vector3 GetWalkingDirection()
    {
        float vx = 0;
        float vy = 0;
        float vz = 0;
        if (Input.GetKey(KeyCode.W))
            vx += 1;

        if (Input.GetKey(KeyCode.S))
            vx += -1;

        if (Input.GetKey(KeyCode.D))
            vz += -1;

        if (Input.GetKey(KeyCode.A))
            vz += 1;

        Vector3 v3 = new Vector3(vx, vy, vz);

        return v3.normalized;
    }

    public override InputOrder GetInputOrder()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            return InputOrder.jump;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            return InputOrder.squat;
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            return InputOrder.walk;
        }
        else
        {
            return InputOrder.stand;
        }
    }

    public override Vector3 GetSightRotation(GameObject owner = null, GameObject target = null)
    {
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");

        return new Vector3(0, h, v);
    }

    public override int GetAttackOrder()
    {
        if (Input.GetMouseButton(1)) // right click
        {
            return -2; // use item order
        }

        if (Input.GetMouseButton(0))// left click
        {
            return 0;
        }

        for (int i = 1; i < 10 && i < AttackComponent.skillListSize; i++)
        {
            if (Input.GetKey(KeyCode.Alpha0 + i))
            {
                return i;
            }
        }

        return -1;
    }

    public override bool GetThrowOrder()
    {
        return Input.GetKey(KeyCode.G);
    }

    public override SystemOrder GetSystemOrder()
    {
        --systemOrderProtectedCount;

        if (systemOrderProtectedCount > 0 )
        {
            return SystemOrder.NOTHING;
        }
     //   else if (Input.GetKey(KeyCode.Escape))
     //   {
     //       systemOrderProtectedCount = 30;
     //       return SystemOrder.SYSTEM_MENU;
     //   }
        else if (Input.GetKey(KeyCode.B))
        {
            systemOrderProtectedCount = 30;
            return SystemOrder.ITEMBOX;
        }
        else
        {
            return SystemOrder.NOTHING;
        }
    }
}

public class BOTCharacterIdentity : CharacterIdentity
{
    override public Vector3 GetWalkingDirection()
    {
        return Vector3.zero;
    }
    override public InputOrder GetInputOrder()
    {
        return InputOrder.stand;
    }
    override public Vector3 GetSightRotation(GameObject owner, GameObject target = null)
    {
        if (target == null)
        {
            float v = 0;
            float verticalAngle = owner.GetComponent<Character>().head.transform.eulerAngles.z;

            if (Math.Sin(verticalAngle * Math.PI / 180f) > 0.05f)
            {
                v = -0.5f;
            }
            else if (Math.Sin(verticalAngle * Math.PI / 180f) < -0.05f)
            {
                v = 0.5f;
            }

            return new Vector3(0, 0, v);
        }
        else
        {
            Vector3 distance = new Vector3(
                target.transform.position.x - owner.transform.position.x, 
                target.transform.position.y - owner.transform.position.y,
                target.transform.position.z - owner.transform.position.z);

            float h = 0;
            float v = 0;

            if (Vector3.Cross(distance, owner.GetComponent<Character>().model.transform.right).y > 0.05f)
            {
                h = -0.5f;
            }
            else if (Vector3.Cross(distance, owner.GetComponent<Character>().model.transform.right).y < -0.05f)
            {
                h = 0.5f;
            }

            Vector3 verticalSight = owner.GetComponent<Character>().head.transform.eulerAngles;
            
            if (distance.normalized.y > (float)Math.Sin(verticalSight.z * Math.PI / 180f) + 0.05f)
            {
                v = 0.5f;
            }
            else if (distance.normalized.y < (float)Math.Sin(verticalSight.z * Math.PI / 180f) - 0.05f)
            {
                v = -0.5f;
            }
            
            return new Vector3(0, h, v);
        }
    }

    public override int GetAttackOrder()
    {
        return -1;
    }
    public override bool GetThrowOrder()
    {
        return false;
    }
    public override SystemOrder GetSystemOrder()
    {
        return SystemOrder.NOTHING;
    }
}