﻿using UnityEngine;

public class schieber_manager : MonoBehaviour
{
    public bool fine_tuning = false;
    private bool ending = false;
    private bool is_schieber = false;
    private Vector3 cam_pos;
    private Vector3 og_pos;
    private Vector3 target_pos;
    private GameObject schiene; // = "schiene";
    private GameObject schieber;
    private Vector3 schiene_pos_copy;
    private Vector3 schiene_rot_copy;
    private Bounds obj_bds;
    //private GameObject schieber; // = "schieber";

    // for smooth camera animation
    private float dist;
    
    // Start is called before the first frame update
    void Start()
    {
        if (name != "导轨MR12MN") 
            return;
        is_schieber = true;
        obj_bds = gameObject.GetComponent<Renderer>().bounds;

        // create and assign the schiber object
        schiene = GameObject.Find("schiene_wrapper");
        schieber = schiene.transform.Find("schieber_wrapper").gameObject;
        schiene_pos_copy = schiene.transform.position;
        schiene_rot_copy = schiene.transform.eulerAngles;

        return;
    }

    // start schieber measurement, switch camera placement
    public bool start_schieber()
    {
        if (!is_schieber) return false;
        fine_tuning = true;
        
        // calculate the start and end points of the camera movement
        Bounds bds = GetComponent<Renderer>().bounds;
        og_pos = global_manager.main_cam.transform.position;
        cam_pos = transform.position + new Vector3(0.8f * bds.size.x, 1.5f * bds.size.y, 0);
        dist = (cam_pos - og_pos).magnitude;

        // record the target position of the object
        target_pos = transform.position;

        return true;
    }
    
    // stop schieber session and reset the camera
    public void end_schiber()
    {
        if (!fine_tuning) return;

        // re-assign the start and end points of the camera movement
        transform.position = target_pos;
        var tmp = og_pos;
        og_pos = cam_pos;
        cam_pos = tmp;

        // set the ending animation marker
        ending = true;

        // destroy the schieber object
        schiene.transform.position = schiene_pos_copy;
    }

    // fit the schiber near the obj
    public void set_schieber()
    {
        // align the schiene to the x-edge of the target position
        var sc_pos = new Vector3(
            obj_bds.max.x - obj_bds.size.x * 0.16f,
            transform.position.y + obj_bds.extents.y,
            target_pos.z 
        );

        var sb_pos = new Vector3(
            obj_bds.max.x - obj_bds.size.x * 0.16f,
            transform.position.y + obj_bds.extents.y,
            transform.position.z 
        );


        if (transform.position.z >= target_pos.z)
        {
            schiene.transform.eulerAngles = new Vector3(
                schiene_rot_copy.x,
                schiene_rot_copy.y,
                schiene_rot_copy.z
            );
            schiene.transform.position = new Vector3(
                sc_pos.x, sc_pos.y, sc_pos.z - obj_bds.extents.z
            );
            schieber.transform.position = new Vector3(
                sb_pos.x, sb_pos.y, sb_pos.z + obj_bds.extents.z
            );
        } 
        else
        {
            schiene.transform.eulerAngles = new Vector3(
                schiene_rot_copy.x, 
                schiene_rot_copy.y + 180,
                schiene_rot_copy.z
            );
            schiene.transform.position = new Vector3(
                sc_pos.x, sc_pos.y, sc_pos.z + obj_bds.extents.z
            );
            schieber.transform.position = new Vector3(
                sb_pos.x, sb_pos.y, sb_pos.z - obj_bds.extents.z
            );

        }
        

    }

    // give the min bound of the mouse drag in puzzle
    public Vector3 min_bound()
    {
        Bounds bds = GetComponent<Renderer>().bounds;
        return new Vector3(
            target_pos.x - bds.size.x * 5f,
            target_pos.y - bds.size.y * 5f,
            target_pos.z - bds.size.z * 5f
        );
    }

    // give the max bound of the mouse drag in puzzle
    public Vector3 max_bound()
    {
        Bounds bds = GetComponent<Renderer>().bounds;
        return new Vector3(
            target_pos.x + bds.size.x * 5f,
            target_pos.y + bds.size.y * 5f,
            target_pos.z + bds.size.z * 5f
        );
    }

    // check if schiber puzzle is complete
    public bool check_target()
    {
        if ((target_pos - transform.position).magnitude < 0.1f)
        {
            transform.position = target_pos;
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (fine_tuning)
        {
            /** CAMERA CHANGE **/
            if (global_manager.main_cam.transform.position == cam_pos)
            {
                if (ending)
                {
                    ending = false;
                    fine_tuning = false;
                }
            } else {
                float frac = (global_manager.main_cam.transform.position - og_pos).magnitude / dist;

                // smooth the animation using shrinking steps
                float _step = 0.1f;
                float step = Mathf.Max(_step * 0.1f, _step * (1f - frac));

                global_manager.main_cam.transform.position = Vector3.Lerp(og_pos, cam_pos, frac + step);
                global_manager.main_cam.transform.LookAt(target_pos);

            }

        }
    }
    
}
