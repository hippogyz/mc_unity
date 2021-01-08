using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGround : MonoBehaviour
{
    public int onGroundNumber = 0;
    public bool onGround = false;


    void FixedUpdate()
    {
        Vector3 corner1 = new Vector3(transform.position.x + (transform.localScale.x / 2 - 0.01f),
                                                        transform.position.y - (transform.localScale.y / 2 - 0.01f),
                                                        transform.position.z + (transform.localScale.z / 2 - 0.01f));
        Vector3 corner2 = new Vector3(transform.position.x - (transform.localScale.x / 2 - 0.01f),
                                                        transform.position.y - (transform.localScale.y / 2 - 0.01f),
                                                        transform.position.z + (transform.localScale.z / 2 - 0.01f));
        Vector3 corner3 = new Vector3(transform.position.x + (transform.localScale.x / 2 - 0.01f),
                                                        transform.position.y - (transform.localScale.y / 2 - 0.01f),
                                                        transform.position.z - (transform.localScale.z / 2 - 0.01f));
        Vector3 corner4 = new Vector3(transform.position.x - (transform.localScale.x / 2 - 0.01f),
                                                        transform.position.y - (transform.localScale.y / 2 - 0.01f),
                                                        transform.position.z - (transform.localScale.z / 2 - 0.01f));

        onGround = Physics.Raycast(corner1, Vector3.down, 0.025f) ||
                          Physics.Raycast(corner2, Vector3.down, 0.025f) ||
                          Physics.Raycast(corner3, Vector3.down, 0.025f) ||
                          Physics.Raycast(corner4, Vector3.down, 0.025f);
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (!onGround)
        {
            onGroundNumber = 1;
        }
        else
        {
            onGroundNumber++;
        }

        onGround = true;
    }

    private void OnTriggerExit(Collider other)
    {
        onGroundNumber--;
        if (onGroundNumber == 0)
        {
            onGround = false;
        }
    }
    */
    public bool OnGround()
    {
        return onGround;
    }
}
