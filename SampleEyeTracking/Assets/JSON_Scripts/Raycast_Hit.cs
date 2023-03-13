using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast_Hit : MonoBehaviour
{
    public GetTimeValue script;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            GameObject obj = hit.transform.gameObject;
            if (obj.GetComponent<Object_Tag>())
            {
                script.UpdateObj(obj.GetComponent<Object_Tag>().name);
            }
            else
            {
                script.UpdateObj("Default");
            }

        } else
        {
            script.UpdateObj("Default");
        }

    }
}
