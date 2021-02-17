using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    public float speed = 10.0f;

    // Update is called once per frame
    void Update()
    {
        float verticalTranslation = Input.GetAxis("Vertical") * speed;

        // Make it move 10 meters per second instead of 10 meters per frame...
        verticalTranslation *= Time.deltaTime;

        // Move translation along the object's z-axis
        transform.Translate(0, verticalTranslation, 0);

    }
}
