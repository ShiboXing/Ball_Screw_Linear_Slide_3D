using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

public class schieber_manager : MonoBehaviour
{
    private bool is_schieber = false;
    private bool fine_tuning = false;

    private Vector3 cam_pos;
    private Vector3 og_pos;
    // Start is called before the first frame update
    void Start()
    {
        if (name != "导轨MR12MN") 
            return;
        is_schieber = true;
        return;
    }

    // start schieber measurement, switch camera placement
    public bool start_schieber()
    {
        if (!is_schieber) return true;
        //if (fine_tuning) return false;
        fine_tuning = true;
        Bounds bds = GetComponent<Renderer>().bounds;
        og_pos = global_manager.main_cam.transform.position;
        cam_pos = transform.position + new Vector3(3 * bds.size.x, 3 * bds.size.y, 0);
        return false;
    }


    // Update is called once per frame
    void Update()
    {
        if (fine_tuning)
        {
            global_manager.main_cam.transform.position = Vector3.Lerp(og_pos, cam_pos, Time.deltaTime);
        }
    }
    
}
