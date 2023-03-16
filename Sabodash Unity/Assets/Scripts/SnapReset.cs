using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapReset : MonoBehaviour
{

    Transform tf;
    Vector3 initPos;

    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        initPos = tf.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Resetting behaviour
        if (!GameState.gameStarted)
        {
            tf.position = initPos;
        }
    }
}
