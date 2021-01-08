using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : GameObjectManager
{
    ItemLibrary itemLibrary;

    public ItemManager(AllManager am, GameObject pool, int poolSize = 500) : base(am, pool, poolSize)
    {
        InitializeManager();
    }

    protected void InitializeManager()
    {
        itemLibrary = allManager.itemLibrary;
    }

    protected override void CheckAlive()
    {
        GameObjectNode nowNode = headNode;
        while (nowNode != null && objectPoolCount != 0)
        {
            if (nowNode.mObject.GetComponent<ItemMarker>().IsObtained())
            {
                DestoryGameObject(nowNode);
            }

            nowNode = nowNode.next;
        }
    }

    protected override void SpecificUpdate()
    {
    }

    public void CreateItemObject(int itemId, float x, float y, float z)
    {
        GameObject instance = itemLibrary.GetItem(itemId).itemPrefab;

        GameObject createdObject = CreateGameObject(instance, (int) x, (int) y, (int) z);

        if (createdObject != null)
        {
            createdObject.GetComponent<Basic>().Initialize(this, x, y, z);
            createdObject.GetComponent<ItemMarker>().InitializeItemMarker(itemId);
        }
    }

    public void CreateItemObject(int itemId, float x, float y, float z, Vector3 direction)
    {
        GameObject instance = itemLibrary.GetItem(itemId).itemPrefab;

        GameObject createdObject = CreateGameObject(instance, (int)x, (int)y, (int)z);

        if (createdObject != null)
        {
            Vector3 tempD = direction * (ItemMarker.obtainRange + 0.05f);
            createdObject.GetComponent<Basic>().Initialize(this, x + tempD.x, y + tempD.y, z + tempD.z);
            createdObject.GetComponent<Rigid>().velocity = direction * ItemMarker.throwingVelocity;
            createdObject.GetComponent<ItemMarker>().InitializeItemMarker(itemId);
        }
    }
}


