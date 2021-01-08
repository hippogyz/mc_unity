using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Posture
{
    GameObject head;
    GameObject wholebody;
    GameObject body;
    GameObject leftHand;
    GameObject rightHand;
    GameObject leftLeg;
    GameObject rightLeg;

    public RotateObject headObject;
    public RotateObject bodyObject;
    public RotateObject leftHandObject;
    public RotateObject rightHandObject;
    public RotateObject leftLegObject;
    public RotateObject rightLegObject;

    public Posture(Character character)
    {
        head = character.head;
        wholebody = character.wholebody;
        body = character.body;
        leftHand = character.leftHand;
        rightHand = character.rightHand;
        leftLeg = character.leftLeg;
        rightLeg = character.rightLeg;

        headObject = new RotateHead(head);
        bodyObject = new RotateBody(wholebody);
        leftHandObject = new RotateLeftHand(leftHand);
        rightHandObject = new RotateRightHand(rightHand);
        leftLegObject = new RotateRightHand(leftLeg); // same as right hand
        rightLegObject = new RotateLeftHand(rightLeg); // same as left leg
    }

    public void BecomeInvisible()
    {
        body.layer = 8;//8 is invisible to the camera
        leftHand.layer = 8;
        leftLeg.layer = 8;
        rightHand.layer = 8;
        rightLeg.layer = 8;
    }
    public void HandVisible()
    {
        rightHand.layer = 0;

        foreach (Transform tran in rightHand.GetComponentInChildren<Transform>())
        {
            tran.gameObject.layer = 0;
        }
    }

    public void HandInvisible()
    {
        rightHand.layer = 8;

        foreach (Transform tran in rightHand.GetComponentInChildren<Transform>())
        {
            tran.gameObject.layer = 8;
        }
    }
    public void RotateAngle(RotateObject sth, float angle)
    {
        sth.RotateAngle(angle);
    }
    public void RotateTo(RotateObject sth, float targetAngle)
    {
        sth.RotateTo(targetAngle);
    }

    public float GetPhase(RotateObject sth)
    {
        return sth.GetPhase();
    }
}

abstract public class RotateObject
{
    protected GameObject target;
    protected float phase;
    protected float phaseLimit;

    public RotateObject(GameObject targetObject)
    {
        target = targetObject;
        phase = 0;
    }
    public abstract void RotateAngle(float angle);

    public void RotateTo(float angle)
    {
        RotateAngle(angle - phase);
    }

    protected float GetActualRotateAngle(float angle)
    {
        if (phase + angle > phaseLimit)
        {
            angle = phaseLimit - phase;
        }
        else if (phase + angle < -phaseLimit)
        {
            angle = -phaseLimit - phase;
        }

        return angle;
    }

    public float GetPhase() { return phase; }
}

public class RotateHead : RotateObject
{
    public RotateHead(GameObject targetObject):base(targetObject)
    {
        phaseLimit = 60;
    }

    public override void RotateAngle(float angle)
    {
        angle = GetActualRotateAngle(angle);
        phase += angle;

        target.GetComponent<Transform>().Rotate(Vector3.forward * angle);
    }
}

public class RotateBody : RotateObject
{
    public RotateBody(GameObject targetObject):base(targetObject)
    {
        phaseLimit = 45;
    }

    public override void RotateAngle(float angle)
    {
        angle = GetActualRotateAngle(angle);
        phase += angle;

        target.GetComponent<Transform>().Rotate(Vector3.up * angle);
    }
}

public class RotateLeftHand : RotateObject
{
    float length;
    public RotateLeftHand(GameObject targetObject) : base(targetObject)
    {
        phaseLimit = 120;
        length = target.GetComponent<Transform>().localScale.y / 2;
    }
    public override void RotateAngle(float angle)
    {
        angle = GetActualRotateAngle(angle);

        target.GetComponent<Transform>().Rotate(Vector3.forward * phase); // reversal direction

        target.GetComponent<Transform>().localPosition -= new Vector3( - length * (float)Math.Sin(phase / 180 * Math.PI) ,
                                                                  length - length * (float)Math.Cos( phase / 180 * Math.PI ) ,
                                                                  0 );

        phase += angle;

        target.GetComponent<Transform>().localPosition += new Vector3(-length * (float)Math.Sin(phase / 180 * Math.PI),
                                                                  length - length * (float)Math.Cos(phase / 180 * Math.PI),
                                                                  0);

        target.GetComponent<Transform>().Rotate(Vector3.forward * -phase);
    }
}

public class RotateRightHand : RotateObject
{
    float length;
    public RotateRightHand(GameObject targetObject) : base(targetObject)
    {
        phaseLimit = 120;
        length = target.GetComponent<Transform>().localScale.y / 2;
    }
    public override void RotateAngle(float angle)
    {
        angle = GetActualRotateAngle(angle);

        target.GetComponent<Transform>().Rotate( Vector3.forward * -phase);

        target.GetComponent<Transform>().localPosition -= new Vector3( length * (float)Math.Sin(phase / 180 * Math.PI),
                                                                  length - length * (float)Math.Cos(phase / 180 * Math.PI),
                                                                  0);

        phase += angle;

        target.GetComponent<Transform>().localPosition += new Vector3( length * (float)Math.Sin(phase / 180 * Math.PI),
                                                                  length - length * (float)Math.Cos(phase / 180 * Math.PI),
                                                                  0);

        target.GetComponent<Transform>().Rotate(Vector3.forward * phase);
    }
}