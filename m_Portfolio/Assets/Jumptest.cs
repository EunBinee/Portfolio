using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumptest : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody rigid;
    void Start()
    {
        rigid.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Vector3 jumpVelocity = Vector3.up * 5f;
            rigid.AddForce(jumpVelocity, ForceMode.Impulse);
        }
    }
}
