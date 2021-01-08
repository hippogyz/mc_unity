using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class ItemBox : MonoBehaviour
{
    public int[] itemList; // record id
    int defaultEmpty;
    int itemNum;
    public int maxItemNum;
    ItemLibrary itemLibrary;
    ItemManager itemManager;
    public int[] defaultIniHold;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void InitializeItemBox(int maxNumber) // be called in respective manager, e.g. WorldObjectManager / CharacterObjectManager
    {
        maxItemNum = maxNumber;
        itemList = new int[maxItemNum];
        defaultEmpty = 0;
        itemLibrary = GetComponent<Basic>().mManager.allManager.itemLibrary;
        itemManager = GetComponent<Basic>().mManager.allManager.itemManager;

        int tempIndex = (defaultIniHold.Length < maxItemNum) ? defaultIniHold.Length : maxItemNum;
        itemNum = 0;

        for (int i = 0; i < maxItemNum; i++)
        {
            itemList[i] = -1;
        }

        for (int i = 0; i < defaultIniHold.Length && itemNum < maxNumber; i++)
        {
            ObtainItem(defaultIniHold[i]);
        }
    }

    public void InitializeItemBox(int maxNumber, int[] iniHold) // be called in the respective manager, e.g. WorldObjectManager / CharacterObjectManager
    {
        maxItemNum = maxNumber;
        itemList = new int[maxItemNum];
        defaultEmpty = 0;
        itemLibrary = GetComponent<Basic>().mManager.allManager.itemLibrary;
        itemManager = GetComponent<Basic>().mManager.allManager.itemManager;

        int tempIndex = (iniHold.Length < maxItemNum) ? iniHold.Length : maxItemNum;
        itemNum = 0;

        for (int i = 0; i < maxItemNum; i++)
        {
            itemList[i] = -1;
        }

        for (int i = 0; i < iniHold.Length && itemNum < maxNumber; i++)
        {
            ObtainItem( iniHold[i] );
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsFull()
    {
        if (itemNum == maxItemNum)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool ObtainItem(int id)
    {
        if (itemLibrary.ItemExsit(id) && !IsFull())
        {
            itemList[defaultEmpty] = id;
            itemNum++;

            while (defaultEmpty < maxItemNum && itemList[defaultEmpty] != -1)
            {
                defaultEmpty++;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool UseItem(int index,  GameObject target)
    {
        Item usingItem = itemLibrary.GetItem(itemList[index]);

        if (usingItem != null)
        {
            bool isConsumed = usingItem.UseItem( gameObject, target);

            if (isConsumed)
            {
                itemList[index] = -1;
                itemNum--;

                if (index < defaultEmpty)
                {
                    defaultEmpty = index;
                }
            }

            return isConsumed;
        }
        else
        {
            return false;
        }
    }

    public bool ThrowItem(int index, Vector3 direction, float throwStrength = 1.0f)
    {
        Item throwingItem = itemLibrary.GetItem(itemList[index]);

        if (throwingItem != null)
        {
            Vector3 position = GetComponent<Transform>().transform.position;
            
            itemManager.CreateItemObject(itemList[index], position.x, position.y, position.z, direction.normalized * throwStrength);

            itemList[index] = -1;
            itemNum--;

            if (index < defaultEmpty)
            {
                defaultEmpty = index;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public void ThrowAll()
    {
        int id;
        Vector3 direction = Vector3.up;

        for (int i = 0; i < maxItemNum; i++)
        {
            id = itemList[i]; 
            if (id != -1)
            {
                float rand = UnityEngine.Random.Range(0f, (float)maxItemNum);
                direction += Vector3.left * (float)Math.Cos(rand / maxItemNum * Math.PI)
                    + Vector3.forward * (float)Math.Sin(rand / maxItemNum * Math.PI);
                ThrowItem(i, direction, 0.1f);
            }
        }
    }

    public Item AccessItem(int index)
    {
        return itemLibrary.GetItem(itemList[index]);
    }
}

