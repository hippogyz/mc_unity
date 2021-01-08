using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButton : Button
{
    ItemPanel itemPanel;
    ItemLibrary itemLibrary;
    int itemID;
    int buttonOrder;

    Image selectedImage;
    public Image itemImage;

    Sprite emptyItemSprite;
    Sprite chosenItemSprite;
    Sprite selectedItemSprite;

    public void Initialize(ItemPanel itemPanel, ItemLibrary itemLibrary, int itemID, int buttonOrder)
    {
        this.itemPanel = itemPanel;
        this.itemLibrary = itemLibrary;
        this.itemID = itemID;
        this.buttonOrder = buttonOrder;

        itemImage = transform.Find("Image").gameObject.GetComponent<Image>();
        this.emptyItemSprite = itemPanel.emptyItemSprite;
        this.chosenItemSprite = itemPanel.chosenItemSprite;
        this.selectedItemSprite = itemPanel.selectedItemSprite;

        UpdateItemImage();

        onClick.AddListener(SelectItem);
        
    }

    void UpdateItemImage()
    {
        if (itemID == -1)
        {
            itemImage.sprite = emptyItemSprite;
        }
        else
        {
            Item tempItem = itemLibrary.GetItem(itemID);

            if (tempItem != null)
            {
                itemImage.sprite = tempItem.itemImage;
            }
            else
            {
                itemImage.sprite = itemLibrary.GetItem(0).itemImage;
            }
        }
    }

    public void UpdateItemID(int id)
    {
        if (id != itemID)
        {
            itemID = id;
            UpdateItemImage();
        }
    }

    void SelectItem()
    {
        itemPanel.SelectItem(buttonOrder);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        itemPanel.ChosenItem(buttonOrder);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        itemPanel.UnChosenItem(buttonOrder);
    }

    public void ChangeButtonState( ButtonChosenState buttonState )
    {
        switch (buttonState)
        {
            case ButtonChosenState.UNCHOSEN:
                GetComponent<Image>().sprite = emptyItemSprite;
                break;
            case ButtonChosenState.CHOSEN:
                GetComponent<Image>().sprite = chosenItemSprite;
                break;
            case ButtonChosenState.SELECTED:
                GetComponent<Image>().sprite = selectedItemSprite;
                break;
        }
    }
}

public enum ButtonChosenState
{ 
    UNCHOSEN,
    CHOSEN,
    SELECTED
}