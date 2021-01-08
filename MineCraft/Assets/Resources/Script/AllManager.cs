using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllManager : MonoBehaviour
{
    [Header("Player Information")]
    public GameObject mainCamera;
    [HideInInspector]
    public GameObject player;

    [Header("Item Manager")]
    public GameObject dropItemPool;
    public int dropItemPoolSize;
    [HideInInspector]
    public ItemManager itemManager;
    public ItemLibrary itemLibrary;

    [Header("World Object Manager")]
    public GameObject worldObjectPool;
    public int worldObjectPoolSize;
    public GameObject[] worldObjectTemplateList;    // grass for 0, wood for 1, earth for 2, rock for 3
    [HideInInspector]
    public WorldObjectManager worldObjectManager;

    [Header("Character Object Manager")]
    public GameObject characterObjectPool;
    public int characterObjectPoolSize;
    public GameObject[] characterObjectTemplateList; // human for 0
    [HideInInspector]
    public CharacterObjectManager characterObjectManager;

    [Header("UI Manager")]
    public HealthBar healthBar;
    public ItemPanel itemPanel;
    [HideInInspector]
    public UIManager uiManager;


    void Start()
    {
        itemLibrary = GetComponent<ItemLibrary>();
        itemLibrary.Initialize();

        itemManager = new ItemManager(this, dropItemPool, dropItemPoolSize);
        worldObjectManager = new WorldObjectManager(this, worldObjectTemplateList, worldObjectPool, worldObjectPoolSize);
        characterObjectManager = new CharacterObjectManager(this, characterObjectTemplateList, characterObjectPool, characterObjectPoolSize);

        uiManager = new UIManager(this);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        itemManager.Update();
        worldObjectManager.Update();
        characterObjectManager.Update();
    }

    public void SetCamera(GameObject playerObject)
    {
        player = playerObject;

        mainCamera.transform.parent = player.GetComponent<Character>().head.transform;
        mainCamera.transform.localPosition = Vector3.zero;
        mainCamera.transform.eulerAngles = Vector3.up * 90;
    }

}
