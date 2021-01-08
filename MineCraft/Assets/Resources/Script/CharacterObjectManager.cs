using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObjectManager : GameObjectManager
{
    public CharacterObjectManager(AllManager am, GameObject[] templateList, GameObject pool, int poolSize = 500) : base(am, templateList, pool, poolSize)
    {
        InitializeManager();
    }

    protected void InitializeManager()
    {
        TestChara();
    }

    override protected void CheckAlive()
    {
        GameObjectNode nowNode = headNode;
        while (nowNode != null && objectPoolCount != 0)
        {
            if (nowNode.mObject.GetComponent<Basic>().health <= 0)
            {
                DestoryGameObject(nowNode);
            }

            nowNode = nowNode.next;
        }
    }

    override protected void SpecificUpdate()
    {
    }

    public void CreateCharacterObject(GameObject instance, CharacterIdentity ci, int x, int y, int z)
    {
        GameObject createdObject = CreateGameObject(instance, x, y, z);

        if (createdObject != null)
        {
            createdObject.GetComponent<Basic>().Initialize(this, x, y, z);
            createdObject.GetComponent<Character>().InitializeCharacter(ci);

            ItemBox itemBox;
            if (itemBox = createdObject.GetComponent<ItemBox>())
            {
                itemBox.InitializeItemBox(54);
            }
        }
    }

    void TestChara()
    {
        CreateCharacterObject(objectTemplate[0], new PlayerCharacterIdentity(), 2, 15, 2);
        CreateCharacterObject(objectTemplate[0], new BOTCharacterIdentity(), -2, 7, 1);
    }
}
