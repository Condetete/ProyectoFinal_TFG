using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tutoriales { 
public class pause : MonoBehaviour
{
        bool active;
        Canvas canvas;



    void Start()
    {
            canvas = GetComponent<Canvas>();
            canvas.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
            if (Input.GetKeyDown("m"))
            {
                active = !active;
                canvas.enabled = active;
            }
    }
}
}