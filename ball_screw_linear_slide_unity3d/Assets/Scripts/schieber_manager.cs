using System;
using Unity.VisualScripting;
using UnityEngine;

public class schieber_manager : MonoBehaviour
{
    public bool fine_tuning = false;
    private bool ending = false;
    private bool is_schieber = false;
    private Vector3 cam_pos;
    private Vector3 og_pos;
    
    // for smooth camera animation
    private float dist;

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
        fine_tuning = true;
        
        // calculate the start and end points of the camera movement
        Bounds bds = GetComponent<Renderer>().bounds;
        og_pos = global_manager.main_cam.transform.position;
        cam_pos = transform.position + new Vector3(1.005f * bds.size.x, 1.5f * bds.size.y, 0);
        dist = (cam_pos - og_pos).magnitude;
        
        return false;
    }
    
    // stop schieber session and reset the camera
    public void end_schiber()
    {
        if (!fine_tuning) return;
        // re-assign the start and end points of the camera movement
        var tmp = og_pos;
        og_pos = cam_pos;
        cam_pos = tmp;
        
        // set the ending animation marker
        ending = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (fine_tuning)
        {
            float frac = (global_manager.main_cam.transform.position - og_pos).magnitude / dist;

            // smooth the animation using shrinking steps
            float _step = 0.1f;
            float step = Mathf.Max(_step*0.1f, _step * (1f - frac));
            
            global_manager.main_cam.transform.position = Vector3.Lerp(og_pos, cam_pos, frac + step);
            global_manager.main_cam.transform.LookAt(transform);

            if (ending && global_manager.main_cam.transform.position == cam_pos)
            {
                ending = false;
                fine_tuning = false;
            }
        }
    }
    
}
