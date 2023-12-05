using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionDetection : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject root;
    void Start()
    {
        root = GameObject.Find("eSeries_UR3e");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        bool future = (gameObject.transform.root.name == "FutureArm");
        root.GetComponent<InverseMapControll>().CollisionDetected(gameObject, collision.gameObject, future);
    }
}
