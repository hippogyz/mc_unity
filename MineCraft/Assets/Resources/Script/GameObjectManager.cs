using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class GameObjectManager
{
    public AllManager allManager;

    public GameObject gameObjectPool;

    //public const int xScale = 20;
    //public const int yScale = 30;
    //public const int zScale = 20;

    public const int xScale = 10;
    public const int yScale = 20;
    public const int zScale = 10;

    public GameObject[] objectTemplate;
    protected int objectPoolSize;
    //protected int objectPoolSize = 30000;
    protected GameObjectNode headNode;
    protected GameObjectNode tailNode;
    protected int objectPoolCount;

    public GameObjectManager(AllManager am, GameObject[] templateList, GameObject pool, int poolSize = 500)
    {
        allManager = am;

        if (poolSize <= 0)
        {
            Debug.Log("Illegal pool size.");
            poolSize = 50;
        }

        objectTemplate = templateList;
        gameObjectPool = pool;
        objectPoolSize = poolSize;

        objectPoolCount = 0;

        //InitializeManager();
    }

    public GameObjectManager(AllManager am, GameObject pool, int poolSize = 500)
    {
        allManager = am;

        if (poolSize <= 0)
        {
            Debug.Log("Illegal pool size.");
            poolSize = 50;
        }

        //objectTemplate = templateList;
        gameObjectPool = pool;
        objectPoolSize = poolSize;

        objectPoolCount = 0;

        //InitializeManager();
    }

    //abstract protected void InitializeManager();
    abstract protected void CheckAlive();
    abstract protected void SpecificUpdate();

    public void Update()
    {
        CheckAlive();
        SpecificUpdate();
    }

    public GameObject CreateGameObject(GameObject instance, int x, int y, int z)
    {
        GameObject tempObject = null;

        if (x > xScale || x < -xScale || y > yScale || y < 0 || z > zScale || z < -zScale)
        {
            Debug.Log("Creat Coordinate oversteps the World Box. ");
        }
        else if (objectPoolCount >= objectPoolSize)
        {
            Debug.Log("Object Pool is Full.");
        }
        else
        {
            tempObject = Object.Instantiate(instance);
            tempObject.transform.parent = gameObjectPool.transform;

            if (objectPoolCount == 0)
            {
                headNode = new GameObjectNode(tempObject);
                tailNode = headNode;
                objectPoolCount++;
            }
            else
            {
                tailNode.next = new GameObjectNode(tempObject);
                tailNode.next.pre = tailNode;
                tailNode = tailNode.next;
                objectPoolCount++;
            }
        }

        return tempObject;
    }
    public void DestoryGameObject(GameObjectNode node)
    {
        //Debug.Log("Game Object " + node.mObject.name + "in Pool is going to be Destroyed.");

        ThrowAllItem(node.mObject); 

        Object.Destroy(node.mObject);

        if (node == headNode)
        {
            headNode = node.next;
        }

        if (node.pre != null)
        {
            node.pre.next = node.next;
        }

        if (node.next != null)
        {
            node.next.pre = node.pre;
        }

        if (node == tailNode)
        {
            tailNode = node.pre;
        }

        objectPoolCount--;
    }

    private void ThrowAllItem(GameObject mObject )
    {
        ItemBox itemBox = mObject.GetComponent<ItemBox>();

        if (itemBox != null)
        {
            itemBox.ThrowAll();
        }
    }
}

public class GameObjectNode
{
    public GameObject mObject;
    public GameObjectNode next;
    public GameObjectNode pre;

    public GameObjectNode(GameObject mGameObject)
    {
        mObject = mGameObject;
        next = null;
        pre = null;
    }

    /*
    static public void AddNode(GameObjectNode added, ref GameObjectNode head, ref GameObjectNode tail)
    {
        if (head == null)
        {
            head = added;
            tail = head;
        }
        else
        {
            added.pre = tail;
            tail.next = added;
            tail = tail.next;
        }
    }

    static public void AddNode(GameObject gameObject, ref GameObjectNode head, ref GameObjectNode tail)
    {
        AddNode(new GameObjectNode(gameObject), ref head, ref tail);
    } 
    */
}
