using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
    
public class ItemPanel : MonoBehaviour
{
    UIManager mManager;
    GameObject player;
    ItemLibrary itemLibrary;
    ItemBox itemBox;
    int itemNum;
    public ItemButton[] buttonList;
    public int selectedItem;
    int chosenItem;

    string emptyImagePath = "Prefab/Item/Image/EmptyItem";
    string chosenImagePath = "Prefab/Item/Image/chosen";
    string selectedImagePath = "Prefab/Item/Image/selected";
    public Sprite emptyItemSprite;
    public Sprite chosenItemSprite;
    public Sprite selectedItemSprite;

    public void Initialize(UIManager uiManager, GameObject player)
    {
        mManager = uiManager;
        this.player = player;
        this.itemLibrary = uiManager.allManager.itemLibrary;

        gameObject.SetActive(false);

        this.itemBox = player.GetComponent<ItemBox>();
        itemNum = itemBox.maxItemNum;
        buttonList = new ItemButton[itemNum];
        selectedItem = -1;
        chosenItem = -1;

        emptyItemSprite = Resources.Load<Sprite>(emptyImagePath);
        chosenItemSprite = Resources.Load<Sprite>(chosenImagePath);
        selectedItemSprite = Resources.Load<Sprite>(selectedImagePath);

        string buttonName = "itemButton ";
        for (int i = 0; i < itemNum; i++)
        {
            buttonList[i] = transform.Find(buttonName + "(" + i + ")").gameObject.GetComponent<ItemButton>();

            if (buttonList[i] != null)
            {
                buttonList[i].Initialize(this, itemLibrary, itemBox.itemList[i], i);
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < itemNum; ++i)
        {
            buttonList[i].UpdateItemID(itemBox.itemList[i]);
        }
    }

    public void SpecificActive()
    {
        UnChosenItem(chosenItem);
    }

    public void SelectItem(int buttonOrder)
    {
        if (buttonOrder != -1 && itemBox.itemList[buttonOrder] != -1)
        {
            if( selectedItem != -1 )
            {
                buttonList[selectedItem].ChangeButtonState(ButtonChosenState.UNCHOSEN);
            }

            selectedItem = buttonOrder;
            buttonList[selectedItem].ChangeButtonState(ButtonChosenState.SELECTED);

            player.GetComponent<Character>().HoldingItem(buttonOrder);
        }
    }

    public void UnSelectItem()
    {
        if (selectedItem != -1)
        {
            buttonList[selectedItem].ChangeButtonState(ButtonChosenState.UNCHOSEN);
            selectedItem = -1;
        }
    }

    public void ChosenItem(int buttonOrder)
    {
        if (buttonOrder != selectedItem && buttonOrder != -1)
        {
            chosenItem = buttonOrder;
            buttonList[buttonOrder].ChangeButtonState(ButtonChosenState.CHOSEN);
        }
    }
    public void UnChosenItem(int buttonOrder)
    {
        if (buttonOrder != selectedItem && buttonOrder != -1)
        {
            buttonList[buttonOrder].ChangeButtonState(ButtonChosenState.UNCHOSEN);
        }
    }
}