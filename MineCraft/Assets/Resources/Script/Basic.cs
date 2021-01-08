using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObjectManager mManager;

    public int maxHealth = 10;
    public int health = 10;
    public float height;
    float temphealth;

    public AttackComponent attackCom;
    public AttackType attackType = AttackType.physics;
    public int atkpoint = 0;
    public int dfdpoint = 0;

    void Start()
    {
        health = maxHealth;
        temphealth = health;
        attackCom = new AttackComponent(gameObject, attackType, atkpoint, dfdpoint);
    }

    // Update is called once per frame
    void Update()
    {
        //temphealth -= Time.deltaTime;
       // health = (int)temphealth;
    }

    public void Initialize(GameObjectManager manager, int x, int y, int z)
    {
        mManager = manager;

        GetComponent<Transform>().transform.position = new Vector3(x, y, z);
    }
    public void Initialize(GameObjectManager manager, float x, float y, float z)
    {
        mManager = manager;

        GetComponent<Transform>().transform.position = new Vector3(x, y, z);
    }

    public void ChangeHealth(int deltaHealth)
    {
        health += deltaHealth;

        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
}
