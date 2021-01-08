using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public UIManager mManager;
    public Image[] healthPic;
    public Sprite[] healthTexture; // full 0, half 1, empty 2
    const int healthNum = 10;
    int healthPoint;
    float divHealth;
    Basic playerBasic;
    public void Initialize(UIManager uiManager, GameObject playerObject)
    {
        mManager = uiManager;

        playerBasic = playerObject.GetComponent<Basic>();
        divHealth = ((float)playerBasic.maxHealth) / ((float)healthNum);

        healthPoint = healthNum;

        DrawHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        DrawHealthBar();
    }

    void DrawHealthBar()
    {
        healthPoint = (int)(((float)playerBasic.health) / divHealth);

        for (int i = 0; i < healthPoint; i++)
        {
            healthPic[i].sprite = healthTexture[0];
        }

        for (int i = healthPoint; i < healthNum; i++)
        {
            healthPic[i].sprite = healthTexture[2];
        }

        if ((((float)playerBasic.health) / divHealth) - healthPoint > 0.2f)
        {
            healthPic[healthPoint].sprite = healthTexture[1];
        }
    }
}
