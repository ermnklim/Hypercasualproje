using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hypercasualkameratakip : MonoBehaviour
{
    public Transform target; 
    public Vector3 offset; 
    public float followSpeed = 5f; 

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        transform.LookAt(target);
    }
}