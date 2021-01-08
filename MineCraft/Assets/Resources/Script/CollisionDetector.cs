using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public GameObject detector;

    public GameObject[] otherCollider;
    public int colliderNum;
    public bool isCollision;

    // Start is called before the first frame update
    void Start()
    {
        detector = gameObject;
        otherCollider = new GameObject[7];
        colliderNum = 0;
        isCollision = false;
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("trigger stay call");
        if (colliderNum < 7)
        {
            otherCollider[colliderNum] = other.gameObject;
            colliderNum++;
            isCollision = true;
        }
    }

    public void ResetDetector() // call it after modifying position
    {
        colliderNum = 0;
        isCollision = false;
    }
}
