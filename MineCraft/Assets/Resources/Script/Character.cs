using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.VR;

public class Character : MonoBehaviour
{
    //CharacterController characterController;

    public CharacterIdentity characterIdentity;
    GameObject targetCharacter;//for BOT to find player temporarily

    public GameObject characterCollider;

    Transform trans;
    Rigid rigid;

    CharacterState mState;
    DelayState mDelayState;

    public GameObject model;
    public GameObject head;
    public GameObject wholebody;
    public GameObject body;
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject leftLeg;
    public GameObject rightLeg;

    Posture posture;

    //for jump
    public float jumpStrength = 5.0f;
    //for walk
    public float speedLimit = 5.0f;
    public float accelerateRate = 10.0f;
    //for change sight
    public float sightSpeed = 120.0f;
    //for walk posture
    public float walkPhaseAmplitude = 45.0f;
    public float walkPhaseSpeed = 540.0f;
    float walkPhase;
    //for attack posture
    float attackPhase;
    //for squat posture
    public float squatPhase = 30f;
    //for holding item
    public int holdingItemIndex;
    GameObject holdingItem;
    public RaycastHit itemTargetHitInfo;

    public SystemOrder gameSystemState;
    InputOrder inputOrder;
    int attackOrder;
    Vector3 walkDirection;
    Vector3 sightRotation;

    float mDeltaTime = 0;

    int frame = 0;

    public void InitializeCharacter(CharacterIdentity ci)
    {
        characterIdentity = ci;
        trans = model.GetComponent<Transform>();
        rigid = GetComponent<Rigid>();

        mState = new IdleState();
        mDelayState = new IdleDelayState();

        gameSystemState = SystemOrder.NOTHING;
        inputOrder = InputOrder.stand;
        attackOrder = -1;
        walkDirection = new Vector3(0, 0, 0);
        sightRotation = new Vector3(0, 0, 0);

        posture = new Posture(this);
        walkPhase = 0;
        attackPhase = 0;

        holdingItemIndex = -1;

        if (characterIdentity.isPlayer)
        {
            GetComponent<Basic>().mManager.allManager.SetCamera(gameObject);
            posture.BecomeInvisible();

            targetCharacter = null;
        }
        else
        {
            targetCharacter = GetComponent<Basic>().mManager.allManager.player;
        }
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        mDeltaTime += Time.fixedDeltaTime;
    }

    void LateUpdate()
    {
        mDelayState.PreDoSomething(this, mDeltaTime);

        GetOrder();

        RotateSight(mDeltaTime);

        CharacterState tempState = mState.UpdataState(this, inputOrder);
        mState = tempState;
        mState.DoSomething(this, mDeltaTime);
        //dosomething for attack should always be put behind the CharacterState
        DelayState tempDelayState = mDelayState.UpdateState(this, attackOrder);
        mDelayState = tempDelayState;
        mDelayState.DoSomething(this, mDeltaTime);

        mDeltaTime = 0;
        frame++;
    }

    public void GetOrder()
    {
        //process system order first
        SystemOrder systemOder = characterIdentity.GetSystemOrder();
        gameSystemState ^= systemOder;
        if (systemOder != SystemOrder.NOTHING)
        {
            GetComponent<Basic>().mManager.allManager.uiManager.ProcessSystemOrder(gameSystemState);
        }

        //process gameplay order
        if (gameSystemState == SystemOrder.NOTHING)
        {
            GameObject target = FindTarget(); //woud be better put in lateupdate (after fixing physics)

            inputOrder = characterIdentity.GetInputOrder();
            walkDirection = characterIdentity.GetWalkingDirection();
            sightRotation = characterIdentity.GetSightRotation(gameObject, target);
            attackOrder = characterIdentity.GetAttackOrder();

            if (characterIdentity.GetThrowOrder())
            {
                ThrowHoldingItem();
            }
        }
        else
        {
            inputOrder = InputOrder.stand;
            walkDirection = Vector3.zero;
            sightRotation = Vector3.zero;
            attackOrder = -1;
        }
    }
    public bool OnGround()
    {
        TestGround testGround = characterCollider.GetComponent<TestGround>();

        bool result = testGround.OnGround();
        result = result && !(rigid.velocity.y > 0.1f);

        return result;
    }
    public void RotateSight(float deltaTime)
    {
        float horizonAngle = sightRotation.y * sightSpeed * deltaTime;
        trans.Rotate(Vector3.up * horizonAngle);
        posture.RotateAngle(posture.bodyObject, -horizonAngle);

        float verticalAngle = sightRotation.z * sightSpeed * deltaTime;
        posture.RotateAngle(posture.headObject, verticalAngle);
    }
    public void Walk(float speed, float deltaTime)
    {
        float directionAngle = -trans.eulerAngles.y * (float)Math.PI / 180f;
        float cos = (float)Math.Cos(directionAngle);
        float sin = (float)Math.Sin(directionAngle);

        Vector3 directionN = new Vector3(walkDirection.x * cos - walkDirection.z * sin, 0, walkDirection.z * cos + walkDirection.x * sin);
        //Vector3 directionT = new Vector3(walkDirection.z * cos + walkDirection.x * sin, 0, -walkDirection.x * cos + walkDirection.z * sin);

        float vn = Vector3.Dot(directionN, rigid.velocity);
        //float vt = Vector3.Dot(directionT, rigid.velocity);

        Vector3 tempVelocity = Vector3.Project(rigid.velocity, Vector3.up);

        if (vn < 0)
        {
            vn = 0;
        }

        if (vn < (speedLimit * speed - 0.1f))
        {
            vn += accelerateRate * deltaTime * speed;
        }

        if (vn >= speedLimit * speed)
        {
            vn = speedLimit * speed;
        }

        tempVelocity += vn * directionN;

        rigid.velocity = tempVelocity;
    }
    GameObject FindTarget()
    {
        if (targetCharacter != null && Vector3.Distance(targetCharacter.transform.position, transform.position) < 5f)
        {
            return targetCharacter;
        }
        else
        {
            return null;
        }
    }
    public GameObject FindDefender(int attackOrder)
    {
        GameObject defender = null;

        float attackRange = 2.5f;

        RaycastHit hitInfo;
        if (Physics.Raycast(characterCollider.transform.position + Vector3.up * characterCollider.transform.localScale.y * 0.4f,
                                        head.transform.right,
                                        out hitInfo,
                                        attackRange))
        {
            Basic basic = hitInfo.collider.GetComponentInParent<Basic>();
            if (basic != null && gameObject != basic.gameObject)
            {
                //UnityEngine.Debug.Log(hitInfo);
                //UnityEngine.Debug.Log("hit frame: " + frame);
                defender = basic.gameObject;
            }
        }

        return defender;
    }
    public void DoJump()
    {
        rigid.velocity += Vector3.up * (jumpStrength - rigid.velocity.y);
        //rigid.constraints = (rigid.constraints | RigidbodyConstraints.FreezePositionY) ^ RigidbodyConstraints.FreezePositionY;
        //rigid.AddForce(new Vector3(0, jumpStrength, 0), ForceMode.Impulse);
    }
    public void DoSquat()
    {
        //change posture to squat
        posture.RotateTo(posture.rightHandObject, -squatPhase);
        posture.RotateTo(posture.leftHandObject, squatPhase);
        posture.RotateTo(posture.rightLegObject, 0);
        posture.RotateTo(posture.leftLegObject, 0);
    }
    public void DoAttack(GameObject defender, int attackOrder)
    {
        if (defender != null)
        {
            //UnityEngine.Debug.Log("attacking " + defender);
            //UnityEngine.Debug.Log("attacking frame: " + frame);
            AttackComponent.DoAttack(gameObject, defender, attackOrder);
            //AttackComponent.DoCounter(gameObject, defender);
        }
    }
    public void ResetPosture()
    {
        walkPhase = 0;

        posture.RotateTo(posture.leftHandObject, 0);
        posture.RotateTo(posture.rightHandObject, 0);
        posture.RotateTo(posture.leftLegObject, 0);
        posture.RotateTo(posture.rightLegObject, 0);
    }
    public void WalkPosture(float deltaTime)
    {
        walkPhase += walkPhaseSpeed * deltaTime;
        while (walkPhase > 360)
        {
            walkPhase -= 360;
        }

        float angle = walkPhaseAmplitude * (float)Math.Sin(walkPhase * Math.PI / 180);

        posture.RotateTo(posture.leftHandObject, angle);
        posture.RotateTo(posture.rightHandObject, angle);
        posture.RotateTo(posture.leftLegObject, angle);
        posture.RotateTo(posture.rightLegObject, angle);

        float bodyPhase = posture.bodyObject.GetPhase();

        if (bodyPhase != 0)
        {
            float tempPhase = (bodyPhase > 0) ? bodyPhase - sightSpeed * 0.5f * deltaTime : bodyPhase + sightSpeed * 0.2f * deltaTime;

            if (bodyPhase * tempPhase <= 0)
            {
                posture.RotateTo(posture.bodyObject, 0);
            }
            else
            {
                posture.RotateTo(posture.bodyObject, tempPhase);
            }
        }
    }
    public void AttackPosture(float deltaTime)
    {
        posture.HandVisible();

        attackPhase += walkPhaseSpeed * deltaTime;
        while (attackPhase > 180)
        {
            attackPhase -= 180;
        }

        float angle = walkPhaseAmplitude * (1 + (float)Math.Cos(attackPhase * Math.PI / 180));

        posture.RotateTo(posture.rightHandObject, angle);
    }
    public void ExitAttackPosture()
    {
        attackPhase = 0;
        posture.HandInvisible();
    }

    public void HoldingItem(int index)
    {
        ItemBox itemBox = GetComponent<ItemBox>();
        Item item = itemBox.AccessItem(index);

        if (item != null)
        {
            if (holdingItemIndex != -1)
            {
                UnityEngine.Object.Destroy(holdingItem);
            }

            holdingItemIndex = index;
            holdingItem = item.GetModel();

            Vector3 itemScale = Vector3.zero + holdingItem.transform.localScale;
            holdingItem.transform.parent = rightHand.transform;
            holdingItem.layer = rightHand.layer;
            holdingItem.transform.localPosition = new Vector3(0.45f, -0.45f, -0.2f);
            holdingItem.transform.localEulerAngles = Vector3.zero;
            holdingItem.transform.localScale = new Vector3(
                itemScale.x / rightHand.transform.localScale.x,
                itemScale.y / rightHand.transform.localScale.y,
                itemScale.z / rightHand.transform.localScale.z);
            holdingItem.GetComponent<Animation>().Stop();
        }
    }

    public void ThrowHoldingItem()
    {
        if (holdingItemIndex != -1)
        {
            float directionAngle = -trans.eulerAngles.y * (float)Math.PI / 180f;
            float cos = (float)Math.Cos(directionAngle);
            float sin = (float)Math.Sin(directionAngle);
            Vector3 throwDirection = new Vector3(cos, 0, sin);

            if (GetComponent<ItemBox>().ThrowItem(holdingItemIndex, throwDirection))
            {
                HoldingItemConsumed();
            }
        }
    }

    public GameObject FindItemTarget()
    {
        GameObject target = null;

        float attackRange = 2.5f;

        RaycastHit hitInfo;
        if (Physics.Raycast(characterCollider.transform.position + Vector3.up * characterCollider.transform.localScale.y * 0.4f,
                                        head.transform.right,
                                        out hitInfo,
                                        attackRange))
        {
            Basic basic = hitInfo.collider.GetComponentInParent<Basic>();
            if (basic != null && gameObject != basic.gameObject)
            {
                //UnityEngine.Debug.Log(hitInfo);
                //UnityEngine.Debug.Log("hit frame: " + frame);
                itemTargetHitInfo = hitInfo;
                target = basic.gameObject;
            }
        }

        return target;
    }

    public void UseHoldingItem(GameObject target)
    {
        if (holdingItemIndex != -1)
        {
            if (GetComponent<ItemBox>().UseItem(holdingItemIndex, target))
            {
                HoldingItemConsumed();
            }
        }
    }

    void HoldingItemConsumed()
    {
        holdingItemIndex = -1;
        UnityEngine.Object.Destroy(holdingItem);

        if (characterIdentity.isPlayer)
        {
            GetComponent<Basic>().mManager.allManager.itemPanel.UnSelectItem();
        }
    }
}