using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : MonoBehaviour
{
    
    Camera cam;

    [SerializeField] private float power = 1;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Mine(power);
        }
        if (Input.GetMouseButtonDown(1)) {
            Mine(-power);
        }

    }

    void Mine(float pow) {
        Debug.Log("Mine");
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
            hit.collider.GetComponent<DeformSurface>().Deform(hit.point, -Vector3.Normalize(transform.position - hit.point), pow);
        }
    }

}
