using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class drag_and_drop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Start is called before the first frame update
    void Start()
    {
        return;
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    public void OnDrag(PointerEventData eventData)
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~drag_mask))
        {
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

            // attached to the wrong parent object
            if (!sticky_manager.is_dep(sticky_obj.name, parent_name))
            {
                sticky_obj.GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                // check if sticky obj is in bound of the specified area
                parent_obj = hit.transform.gameObject;
                var p_bds = hit.transform.gameObject.GetComponent<Renderer>().bounds;
                // attached to the correct parent object, check if it's in bound
                if (sticky_manager.in_bound(sticky_obj, p_bds, parent_name, sticky_obj.name)
                    && !sticky_collider.GetComponent<collider_manager>().check_duplicated())
                    //|| sticky_obj.name.Contains("非标"))
                {
                    sticky_obj.GetComponent<Renderer>().material.color = Color.green;
                    sticky_new_name = sticky_obj.name;
                    sticky_obj.name = sticky_og_name;
                }
                else
                {
                    sticky_obj.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
        else
        {
            transform.position = eventData.position;
            sticky_obj.transform.position = Vector3.zero;
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = init_pos;
        var rdn = sticky_obj.GetComponent<Renderer>();
        if (rdn.material.color == Color.red || sticky_obj.transform.position == Vector3.zero)
            Destroy(sticky_obj.gameObject);
        else
            sticky_obj.name = sticky_new_name;
        sticky_obj.gameObject.layer = LayerMask.NameToLayer("Default");
        rdn.material.color = orig_color;
    }
}