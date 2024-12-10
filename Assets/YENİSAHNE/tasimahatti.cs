using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tasimahatti : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    public MeshRenderer renderer;
    private float yoffset;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = rb.position;
        rb.position -= transform.forward * Time.fixedDeltaTime * speed;
        rb.MovePosition(pos);
        yoffset += Time.fixedDeltaTime;
        renderer.material.mainTextureOffset = new Vector2(0, yoffset);
    }
}
