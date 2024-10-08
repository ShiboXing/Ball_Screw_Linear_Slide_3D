﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class drag_and_drop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{


    // for stick testing
    private GameObject parent_obj;
    public Transform sticky_obj_save;
    public Transform sticky_obj;
    public string sticky_og_name;
    public string sticky_new_name;
    private Vector3 init_pos;
    private int drag_mask;
    private MeshCollider sticky_collider;
    private Color orig_color;
    private LinkedList<GameObject> del_queue;

    // advanced sticky testing
    schieber_manager sch_man;
    collider_manager col_man;

    // Start is called before the first frame update
    void Start()
    {
        del_queue = new LinkedList<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        // remove objs from deque when their schieber session finishes
        while (del_queue.Count > 0 && !del_queue.First.Value.GetComponent<schieber_manager>().fine_tuning)
        {
            Destroy(del_queue.First.Value);
            del_queue.RemoveFirst();
        }


        // PLACE OBJ IN GAME FIRST THEN CHECK HERE FOR BOUND COORDS
        if (!sticky_obj || !parent_obj) return;

        var bds = sticky_obj.GetComponent<Renderer>().bounds;
        var parent_bds = parent_obj.GetComponent<Renderer>().bounds;

        var x_perc = (bds.min.x - parent_bds.min.x) / parent_bds.size.x;
        var y_perc = (bds.min.y - parent_bds.min.y) / parent_bds.size.y;
        var z_perc = (bds.min.z - parent_bds.min.z) / parent_bds.size.z;
        var w_perc = bds.size.x / parent_bds.size.x;
        var h_perc = bds.size.y / parent_bds.size.y;
        var l_perc = bds.size.z / parent_bds.size.z;

        return;
    }

public void OnBeginDrag(PointerEventData eventData)
    {
        init_pos = gameObject.transform.position;
        sticky_obj = Instantiate(sticky_obj_save);
        sticky_obj.name = sticky_obj_save.name;
        sticky_obj.SetParent(null);
        sticky_obj.localPosition = Vector3.zero;
        sticky_obj.transform.localScale = Vector3.one * 100;
        sticky_obj.gameObject.layer = LayerMask.NameToLayer("on_drag");
        drag_mask = 1 << sticky_obj.gameObject.layer;
        orig_color = sticky_obj.GetComponent<Renderer>().material.color;

        // for pivot pointer deviation calculation
        sticky_collider = sticky_obj.GetComponent<MeshCollider>();
        sticky_og_name = sticky_obj.name;

        col_man = sticky_collider.GetComponent<collider_manager>();
        sch_man = sticky_obj.GetComponent<schieber_manager>();
    }

    public void OnDrag(PointerEventData eventData)
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // user solves the schieber puzzle
        if (sch_man.fine_tuning) {
            /** user's mouse ray intersects with the XZ plane, solve for z
             P_intersect(x, z) = P0 + t*Ray
             z and t are unknown */
            float x = sticky_obj.position.x;
            float t = (x - ray.origin.x) / ray.direction.normalized.x;
            float z = ray.origin.z + t * ray.direction.normalized.z;
            z = Math.Min(Math.Max(z, sch_man.min_bound().z), sch_man.max_bound().z);
            sticky_obj.position = new Vector3(sticky_obj.position.x, sticky_obj.position.y, z);
            
            // check if the schiber puzzle is solved
            if (sch_man.check_target() && !col_man.check_duplicated())
                sticky_obj.GetComponent<Renderer>().material.color = Color.green;
            else
                sticky_obj.GetComponent<Renderer>().material.color = Color.red;

            // send obj's position to schieber to set up the ruler
            sch_man.set_schieber();

        } else if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~drag_mask)) {
            // re-center the new_obj (counter the offset between Renderer and Collider)
            var collider_offset = sticky_collider.bounds.extents + sticky_obj.transform.position - sticky_collider.bounds.center;
            var x_offset = collider_offset.x;
            var y_offset = collider_offset.y;
            var z_offset = collider_offset.z;
            if (ray.direction.x > 0) x_offset = -x_offset;
            if (ray.direction.y > 0) y_offset = -y_offset;
            if (ray.direction.z > 0) z_offset = -z_offset;
            sticky_obj.transform.position = hit.point + new Vector3(x_offset, y_offset, z_offset);
            transform.position = init_pos;

            var parent_name = hit.transform.gameObject.name;

       
            // check if sticky obj is in bound of the specified area
            parent_obj = hit.transform.gameObject;
            var p_bds = hit.transform.gameObject.GetComponent<Renderer>().bounds;
            // attached to the correct parent object, check if it's in bound
            if (sticky_manager.in_bound(sticky_obj, p_bds, parent_name, sticky_obj.name, ref sticky_new_name)
                && !col_man.check_duplicated()
                && !sch_man.start_schieber())
                //|| sticky_obj.name.Contains("螺丝")
                //|| sticky_obj.name.Contains("面板"))
            {
                sticky_obj.GetComponent<Renderer>().material.color = Color.green;
            }
            else
                sticky_obj.GetComponent<Renderer>().material.color = Color.red;
            
        } else {
            transform.position = eventData.position;
            sticky_obj.transform.position = Vector3.zero;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = init_pos;
        var rdn = sticky_obj.GetComponent<Renderer>();

        // end any schieber session
        sch_man.end_schiber();

        if (rdn.material.color == Color.red || sticky_obj.transform.position == Vector3.zero)
            del_queue.AddLast(sticky_obj.gameObject);
        else
            sticky_obj.name = sticky_new_name == null ? sticky_og_name : sticky_new_name;

        sticky_obj.gameObject.layer = LayerMask.NameToLayer("Default");
        rdn.material.color = orig_color;


    }
}