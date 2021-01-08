using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rigid : MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;
    public bool onGround;

    Transform trans;
    CollisionDetector collisionDetector;
    TestGround testGround;

    float innerFixedTime;

    static float verticalVelocityLimit = 20f;
    static float gravity = 10f;

    //for test
    //public bool isCollision;
    //public GameObject[] otherCollider;

    void Start()
    {
        //velocity = Vector3.zero;
        trans = GetComponent<Transform>();
        collisionDetector = GetComponentInChildren<CollisionDetector>();
        testGround = GetComponentInChildren<TestGround>();
        onGround = OnGround();
        innerFixedTime = 0;
    }

    void FixedUpdate()
    {
        innerFixedTime += Time.fixedDeltaTime;
        //gravity
        if (!OnGround()  && velocity.y > - verticalVelocityLimit )
        {
            velocity -= Vector3.up * gravity * Time.fixedDeltaTime;
        }

        trans.position += velocity * Time.fixedDeltaTime;
    }

    void Update()
    {
        //for test
        //isCollision = collisionDetector.isCollision;
        //otherCollider = collisionDetector.otherCollider;


        if (collisionDetector.isCollision)
        {
            //Debug.Log(collisionDetector.colliderNum);
            ModifyPhysics();
        }

        collisionDetector.ResetDetector();
        innerFixedTime = 0;
    }

    public bool OnGround()
    {
        bool result = testGround.OnGround();
        result = result && !(velocity.y > 0.1f);

        return result;
    }
    void ModifyPhysics()
    {
        CollisionDeltaTime[] deltaTimeList = new CollisionDeltaTime[collisionDetector.colliderNum];

        for (int i = 0; i < collisionDetector.colliderNum; i++)
        {
            deltaTimeList[i] = CalDeltaTime(collisionDetector.detector, collisionDetector.otherCollider[i]);
        }

        int listLength = collisionDetector.colliderNum;
        int maxIndex;

        for (int i = 0; i < 3 && listLength > 0; i++)
        {
            maxIndex = FindMaxDeltaTime(ref deltaTimeList, listLength);

            if (maxIndex != -1)
            {
                CollisionDeltaTime maxCDT = new CollisionDeltaTime(deltaTimeList[maxIndex]);
                ModifyDirection(maxCDT);
                listLength = OrderDeltaTimeList(ref deltaTimeList, listLength, maxCDT);
            }
            else
            {
                listLength = 0;
            }
        }
    }
    void ModifyDirection(CollisionDeltaTime CDT)//modify one direction
    {

        switch (CDT.resumeDirection)
        {
            case ResumeDirection.x:
                trans.position -= Vector3.right * velocity.x * (CDT.dt[0] + Mathf.Abs(0.01f/ velocity.x));
                velocity.x = 0;
                break;
            case ResumeDirection.y:
                trans.position -= Vector3.up * velocity.y * (CDT.dt[0] + Mathf.Abs(0.01f / velocity.y));
                velocity.y = 0;
                break;
            case ResumeDirection.z:
                trans.position -= Vector3.forward * velocity.z * (CDT.dt[0] + Mathf.Abs(0.01f / velocity.z));
                velocity.z = 0;
                break;
            default:
                break;
        }
    }
    CollisionDeltaTime CalDeltaTime(GameObject detector, GameObject collider)//calculate deltatime with one collider
    {
        CollisionDeltaTime result = new CollisionDeltaTime();
        result.dt = new float[4];

        Transform transDe = detector.transform;
        Transform transCo = collider.transform;

        if (velocity.x > 0)
        {
            result.dt[1] = ((transDe.position.x + transDe.localScale.x / 2) - (transCo.position.x - transCo.localScale.x / 2)) / velocity.x;
        }
        else if (velocity.x < 0)
        {
            result.dt[1] = ((transDe.position.x - transDe.localScale.x / 2) - (transCo.position.x + transCo.localScale.x / 2)) / velocity.x;
        }
        else
        {
            result.dt[1] = 0;
        }

        if (velocity.y > 0)
        {
            result.dt[2] = ((transDe.position.y + transDe.localScale.y / 2) - (transCo.position.y - transCo.localScale.y / 2)) / velocity.y;
        }
        else if (velocity.y < 0)
        {
            result.dt[2] = ((transDe.position.y - transDe.localScale.y / 2) - (transCo.position.y + transCo.localScale.y / 2)) / velocity.y;
        }
        else
        {
            result.dt[2] = 0;
        }

        if (velocity.z > 0)
        {
            result.dt[3] = ((transDe.position.z + transDe.localScale.z / 2) - (transCo.position.z - transCo.localScale.z / 2)) / velocity.z;
        }
        else if (velocity.z < 0)
        {
            result.dt[3] = ((transDe.position.z - transDe.localScale.z / 2) - (transCo.position.z + transCo.localScale.z / 2)) / velocity.z;
        }
        else
        {
            result.dt[3] = 0;
        }

        float dttemp = 100;
        result.resumeDirection = ResumeDirection.n;

        for (int i = 1; i < 4; i++)
        {
            if (result.dt[i] > 0 && result.dt[i] < dttemp)
            {
                dttemp = result.dt[i];
                result.resumeDirection = (ResumeDirection)i;
            }
        }

        result.dt[0] = dttemp;

        return result;
    }
    int FindMaxDeltaTime(ref CollisionDeltaTime[] deltaTimeList, int listLength) // return the index of the max dt
    {
        if (listLength == 0)
        {
            return -1;
        }

        int index = 0;
        float maxdt = deltaTimeList[0].dt[0];

        for (int i = 1; i < listLength; i++)
        {
            if (deltaTimeList[i].dt[0] > maxdt)
            {
                index = i;
                maxdt = deltaTimeList[i].dt[0];
            }
        }

        if (maxdt == 100)
        {
            return -1;
        }

        return index;
    }
    int OrderDeltaTimeList(ref CollisionDeltaTime[] deltaTimeList, int listLength, CollisionDeltaTime maxCDT)    // return valid length of arranged time list
    // replace invalid element to the tail of the list
    {
        if (listLength == 0)
        {
            return -1;
        }

        int nowIndex = 0;
        int tailIndex = listLength - 1;
        float judgeTime = maxCDT.dt[0];
        int judgeDirection = (int)maxCDT.resumeDirection;

        while (nowIndex <= tailIndex)
        {
            if (deltaTimeList[nowIndex].dt[judgeDirection] <= judgeTime)
            {
                CollisionDeltaTime tempCDT = new CollisionDeltaTime(deltaTimeList[nowIndex]);
                deltaTimeList[nowIndex].DeepCopy(deltaTimeList[tailIndex]);
                deltaTimeList[tailIndex].DeepCopy(tempCDT);
                tailIndex--;
            }
            else
            {
                nowIndex++;
            }
        }

        return nowIndex;
    }
}
public class CollisionDeltaTime
{
    public float[] dt;
    public ResumeDirection resumeDirection;

    public CollisionDeltaTime()
    { }

    public CollisionDeltaTime(CollisionDeltaTime CDT)
    {
        dt = new float[4];

        for (int i = 0; i < 4; i++)
        {
            dt[i] = CDT.dt[i];
        }

        resumeDirection = CDT.resumeDirection;
    }

    public void DeepCopy(CollisionDeltaTime CDT) 
    {
        if (dt.Length == 0)
        {
            dt = new float[4];
        }

        for (int i = 0; i < 4; i++)
        {
            dt[i] = CDT.dt[i];
        }

        resumeDirection = CDT.resumeDirection;
    }
}
public enum ResumeDirection { n = 0, x = 1, y = 2, z = 3 };
