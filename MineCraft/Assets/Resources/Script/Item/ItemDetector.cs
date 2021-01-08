using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetector : MonoBehaviour
{
    ItemMarker itemMarker;

    private void Start()
    {
        itemMarker = GetComponentInParent<ItemMarker>();
    }

    private void OnTriggerStay(Collider other)
    {
        itemMarker.OwnerUpdate(other.gameObject);
    }
}
