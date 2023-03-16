using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rainbow : MonoBehaviour
{

    public SpriteRenderer sprite;
    public float hue = 0f;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        hue += 0.01f;
        if (hue > 1f) hue = 0f;
        sprite.color = Color.HSVToRGB(hue, 1, 1);
    }
}
