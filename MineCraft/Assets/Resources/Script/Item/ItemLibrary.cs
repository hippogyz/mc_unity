using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using JetBrains.Annotations;

public class ItemLibrary : MonoBehaviour
{
    Hashtable itemHash;
    int registeredID;
    const int maxRegisterNumber = System.Int16.MaxValue;

    public string jsonFilePath;

    public GameObject[] testlist;

    public void Initialize()
    {
        itemHash = new Hashtable();

        //StreamReader streamReader = new StreamReader(Application.dataPath + jsonFilePath);
        //JsonReader jsonReader = new JsonReader(streamReader);

        TextAsset s = Resources.Load<TextAsset>(jsonFilePath);
        string tmp = s.text;

        ReadItemList readList = JsonMapper.ToObject<ReadItemList>(tmp);
        int listLength = readList.ItemList.Count;
        int listIndex = 0;

        registeredID = 0;

        
        while (listIndex < listLength)
        {
            if (!RegisterItem(readList.ItemList[listIndex]))
            {
                break;
            }
            listIndex++;
        }

        //jsonReader.Close();
        //streamReader.Close();

        testlist = new GameObject[itemHash.Count];
        for (int i = 0; i < itemHash.Count; i++)
        {
            testlist[i] = GetItem(i).itemPrefab;
        }
    }

    public bool RegisterItem(Item item)
    {
        int id = item.itemID;

        if (!itemHash.ContainsKey(id) && id != -1)
        {
            itemHash.Add(id, item);
            return true;
        }

        return false;
    }

    bool RegisterItem(ReadItem readItem) // only called while initializing
    {
        while (itemHash.ContainsKey(registeredID) && registeredID < maxRegisterNumber)
        {
            registeredID++;
        }

        if (registeredID == maxRegisterNumber)
        {
            return false;
        }
        else
        {
            string itemName = readItem.Name;
            GameObject itemPrefab = Resources.Load<GameObject>(readItem.PrefabPath);
            Sprite itemImage = Resources.Load<Sprite>(readItem.ImagePath);
            int usageOrder = readItem.UsageOrder;
            int usagePara = readItem.UsagePara;

            Item newItem = new Item(this , registeredID, itemName, itemPrefab,  itemImage, usageOrder, usagePara);

            itemHash.Add(registeredID, newItem);
            return true;
        }
    }

    public Item GetItem(int id)
    {
        if (itemHash.ContainsKey(id))
        {
            return (Item) itemHash[id];
        }
        else
        {
            return null;
        }
    }

    public bool ItemExsit(int id)
    {
        return itemHash.ContainsKey(id);
    }

    public bool UseItem( int usageOrder, int usagePara, GameObject user, GameObject target )
    {
        switch (usageOrder)
        {
            case 0:
                return PlaceItem(usagePara, user, target);
            default:
                return false;
        }
    }

    bool PlaceItem(int usagePara, GameObject user, GameObject target)
    {
        Character userChara = user.GetComponent<Character>();

        if (target != null && userChara != null)
        {
            if (target.layer == 9)
            {
                Vector3 coordinate = target.transform.position + userChara.itemTargetHitInfo.normal;


                if (GetComponent<AllManager>().worldObjectManager.
                    CreateWorldObejct(usagePara, (int)coordinate.x, (int)coordinate.y, (int)coordinate.z))
                {
                    return true;
                }
            }
        }


        return false;
    }
}

public class Item
{
    ItemLibrary itemLibrary;

    readonly public int itemID;
    readonly public GameObject itemPrefab;
    readonly public String itemName;
    readonly public Sprite itemImage;
    readonly int usageOrder;
    readonly int usagePara;

    static GameObject defaultPrefab;
    static Sprite defaultImage;


    static Item()
    {
        defaultPrefab = Resources.Load<GameObject>("Prefab/Item/Model/Default");
        defaultImage = Resources.Load<Sprite>("Prefab/Item/Image/Default");
    }

    public Item(ItemLibrary itemLibrary, 
        int id, String name = "   ", GameObject prefab = null, Sprite image = null, int usageOrder = -1, int usagePara = 0)
    {
        this.itemLibrary = itemLibrary;

        itemID = id;
        itemName = name;

        if (prefab == null)
        {
            itemPrefab = defaultPrefab;
        }
        else
        {
            itemPrefab = prefab;
        }

        if (image == null)
        {
            itemImage = defaultImage;
        }
        else
        {
            itemImage = image;
        }

        this.usageOrder = usageOrder;
        this.usagePara = usagePara;
    }

    public GameObject GetModel()
    {
        return UnityEngine.Object.Instantiate(itemPrefab.transform.Find("Model").gameObject);
    }

    public bool UseItem(GameObject user, GameObject target)
    {
        return itemLibrary.UseItem(usageOrder, usagePara, user, target);
    }
}

public class ReadItemList
{
    public List<ReadItem> ItemList { get; set; }
}

public class ReadItem
{ 
    public string Name { get; set; }
    public string PrefabPath { get; set; }
    public string ImagePath { get; set; }
    public int UsageOrder { get; set; }
    public int UsagePara{ get; set; }
}