using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMarker : MonoBehaviour
{
    public GameObject owner;
    public int itemID;
    Rigid rigid;
    static readonly public float obtainRange = 0.5f;
    static readonly public float throwingVelocity = 5.0f;
    float protectTime;
    bool onGround = false;

    void Update()
    {
        if (protectTime > 0)
        {
            protectTime -= Time.deltaTime;
        }

        if (owner != null)
        {

        }
        else if (rigid.velocity != Vector3.zero && rigid.OnGround())
        {
            if (!onGround)
            {
                onGround = true;
            }

            rigid.velocity = Vector3.zero;
        }
    }

    public void InitializeItemMarker(int id)
    {
        itemID = id;
        owner = null;
        rigid = GetComponent<Rigid>();
        protectTime = 1.0f;
    }

    public void OwnerUpdate(GameObject collider)
    {
        if (owner == null && protectTime <= 0)
        {
            ItemBox box = collider.GetComponentInParent<ItemBox>();

            if ( box != null && !box.IsFull())
            {
                owner = collider;
            }
        }
    }

    public bool IsObtained()
    {
        bool isObtained = false;

        if (owner != null)
        {
            ItemBox box = owner.GetComponentInParent<ItemBox>();

            if (!box.IsFull())
            {
                box.ObtainItem(itemID);
                isObtained = true;
            }
            else
            {
                owner = null;
            }
        }

        return isObtained;
    }
}
