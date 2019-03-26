using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformHeadTrack : MonoBehaviour {
    public GameObject manusHead;
    public float offsetX = 0;
    public float offsetY = 0;
    public float offsetZ = 0;

    // Update is called once per frame
    void Update () {
        Vector3 newPosition = manusHead.transform.position;
        newPosition.x += offsetX;
        newPosition.y += offsetY;
        newPosition.z += offsetZ;
        transform.position = newPosition;
	}
}
