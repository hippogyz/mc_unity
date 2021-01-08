using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    public AllManager allManager;

    HealthBar healthBar;
    ItemPanel itemPanel;
    GameObject exhibitingSystemPanel;

    public UIManager(AllManager manager)
    {
        allManager = manager;
        healthBar = manager.healthBar;
        itemPanel = manager.itemPanel;

        healthBar.Initialize(this, manager.player);
        itemPanel.Initialize(this, manager.player);
    }

    public void ProcessSystemOrder(SystemOrder systemOrder)
    {
        if ((int)(systemOrder & SystemOrder.OTHER) != 0)
        { }
        else if ((int)(systemOrder & SystemOrder.SYSTEM_MENU) != 0)
        { }
        else if ((int)(systemOrder & SystemOrder.ITEMBOX) != 0)
        {
            SetPanelActive(itemPanel.gameObject);
            itemPanel.SpecificActive();
        }
        else if (systemOrder == SystemOrder.NOTHING)
        {
            ClearPanelActive();
        }
    }

    void SetPanelActive(GameObject panel)
    {
        if (exhibitingSystemPanel != null)
        {
            exhibitingSystemPanel.SetActive(false);
        }

        panel.SetActive(true);
        exhibitingSystemPanel = panel;
    }

    void ClearPanelActive()
    {
        if (exhibitingSystemPanel != null)
        {
            exhibitingSystemPanel.SetActive(false);
            exhibitingSystemPanel = null;
        }
    }
}
