using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPickUp : MonoBehaviour
{
    [SerializeField] private GameObject item;
    [SerializeField] private GameObject tempHold;
    [SerializeField] private bool isHolding = false;
    [SerializeField] private float throwForce;

    private float distance;
    private Vector3 objectPos;

    void Update()
    {
        Rigidbody body = item.GetComponent<Rigidbody>();

        distance = Vector3.Distance(item.transform.position, tempHold.transform.position);

        //checks distance of player from object
        if (distance >= 3.5)
        {
            isHolding = false;
        }

        //check if player is holding object
        if (isHolding == true)
        {
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            item.transform.SetParent(tempHold.transform);

            //throws object
            if (Input.GetMouseButtonDown(1))
            {
                body.AddForce(tempHold.transform.forward * throwForce);
                isHolding = false;
            }
        }
        else
        {
            objectPos = item.transform.position;
            item.transform.SetParent(null);
            item.transform.position = objectPos;
            body.useGravity = true;
        }
    }

    void OnMouseDown()
    {
        Rigidbody body = item.GetComponent<Rigidbody>();

        if (distance <= 3.5)
        {
            isHolding = true;
            body.useGravity = false;
            body.detectCollisions = true;
        }
    }
    void OnMouseUp()
    {
        isHolding = false;
    }
}
