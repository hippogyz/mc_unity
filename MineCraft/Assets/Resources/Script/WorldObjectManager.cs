using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectManager : GameObjectManager
{
    // grass for 0, wood for 1, earth for 2, rock for 3

    public WorldObjectManager(AllManager am, GameObject[] templateList, GameObject pool, int poolSize = 500) : base(am, templateList, pool, poolSize)
    {
        InitializeManager();
    }

    protected void InitializeManager()
    {
        TestMap();
        //InitializeMap();
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

    //function for creating an object
    public void CreateWorldObject(GameObject instance, int x, int y, int z)
    {
        GameObject createdObject = CreateGameObject(instance, x, y, z);

        if (createdObject != null)
        {
            createdObject.GetComponent<Basic>().Initialize(this, x, y, z);

            ItemBox itemBox;
            if (itemBox = createdObject.GetComponent<ItemBox>())
            {
                itemBox.InitializeItemBox(1);
            }
        }
    }

    public bool CreateWorldObejct(int instanceIndex, int x, int y, int z)
    {
        if (instanceIndex >= 0 && instanceIndex < objectTemplate.Length)
        {
            CreateWorldObject(objectTemplate[instanceIndex], x, y, z);
            return true;
        }
        else
        {
            return false;
        }
    }

    //private function for initializing map 
    void TestMap()
    {
        for (int i = 0; i < 2 * xScale; i++)
        {
            for (int j = 0; j < 2 * zScale; j++)
            {
                // grass for 0, wood for 1, earth for 2, rock for 3
                CreateWorldObject(objectTemplate[3], i - xScale, 0, j - zScale);
                CreateWorldObject(objectTemplate[3], i - xScale, 1, j - zScale);
                CreateWorldObject(objectTemplate[2], i - xScale, 2, j - zScale);
                CreateWorldObject(objectTemplate[2], i - xScale, 3, j - zScale);
                CreateWorldObject(objectTemplate[0], i - xScale, 4, j - zScale);
                //CreateWorldObject(objectTemplate[0], i - xScale, 5, j - zScale);
            }
        }
    }

    void InitializeMap()
    {
        int horizon = (int)(0.4 * yScale);

        int[,] height = new int[xScale * 2 + 1, zScale * 2 + 1];
        int sumHeight = 0;


        //generate height matrix
        for (int j = 0; j < 2 * zScale + 1; j++)
        {
            height[0, j] = horizon;
            sumHeight += height[0, j];
        }

        for (int i = 1; i < 2 * xScale + 1; i++)
        {
            height[i, 0] = horizon;
            sumHeight += height[i, 0];

            float random = Random.Range(-0.5f, 1.0f);

            for (int j = 1; j < 2 * zScale + 1; j++)
            {
                random = (j % 3 == 0) ? Random.Range(-0.5f, 1.0f) : random;
                int tempHeight = (int)((random * 3.5 + height[i, j - 1] + height[i - 1, j]) / 2);

                tempHeight = (tempHeight <= 1) ? 2 : tempHeight;
                tempHeight = (tempHeight > yScale - 3) ? yScale - 3 : tempHeight;

                height[i, j] = tempHeight;
                sumHeight += height[i, j];
            }
        }

        //limit the total number of game object
        while (sumHeight > (int)(0.7 * objectPoolSize))
        {
            for (int i = 0; i < 2 * xScale + 1; i++)
            {
                for (int j = 0; j < 2 * zScale + 1; j++)
                {
                    if (height[i, j] > 2)
                    {
                        sumHeight--;
                        height[i, j]--;
                    }
                }
            }
        }


        //generate map
        for (int i = 0; i < 2 * xScale + 1; i++)
        {
            for (int j = 0; j < 2 * zScale + 1; j++)
            {
                float rockHeight = Random.Range(0.4f, 1.0f);

                for (int k = 0; k <= (int)(rockHeight * height[i, j]); k++)
                {
                    // grass for 0, wood for 1, earth for 2, rock for 3
                    CreateWorldObject(objectTemplate[3], i - xScale, k, j - zScale);
                }

                for (int k = (int)(rockHeight * height[i, j]) + 1; k <= height[i, j] - 1; k++)
                {
                    // grass for 0, wood for 1, earth for 2, rock for 3
                    CreateWorldObject(objectTemplate[2], i - xScale, k, j - zScale);
                }

                if (rockHeight < 0.75f)
                {
                    // grass for 0, wood for 1, earth for 2, rock for 3
                    CreateWorldObject(objectTemplate[0], i - xScale, height[i, j], j - zScale);
                }
                else
                {
                    // grass for 0, wood for 1, earth for 2, rock for 3
                    CreateWorldObject(objectTemplate[2], i - xScale, height[i, j], j - zScale);
                }
            }
        }

        //Debug.Log("Total Object Number: " + sumHeight.ToString());
    }


}