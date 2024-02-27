using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamLogic : MonoBehaviour
{
    public Transform playerTransform;

    public float fixedDistance;
    public float height;
    public float heightFactor;
    public float rotationFactor;

    private float startRotationAngle;
    private float endRotationAngle;
    private float finalRotationAngle;

    private float currentHeight;
    private float wantedHeight;

    // Update is called once per frame
    void LateUpdate()
    {
        currentHeight = this.transform.position.y;
        wantedHeight = playerTransform.position.y + height;
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightFactor * Time.deltaTime);
        
        startRotationAngle = this.transform.eulerAngles.y;
        endRotationAngle = playerTransform.eulerAngles.y;
        finalRotationAngle = Mathf.Lerp(startRotationAngle, endRotationAngle, Time.deltaTime * rotationFactor);

        Quaternion finalRotation = Quaternion.Euler(0, finalRotationAngle, 0);
        this.transform.position = playerTransform.position;
        this.transform.position -= finalRotation * Vector3.forward * fixedDistance;

        this.transform.position = new Vector3(this.transform.position.x, currentHeight, this.transform.position.z);
        this.transform.LookAt(playerTransform);
    }
}
