using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    }

    // for stick testing
    public float sticky_x_perc = 0.5f;
    public float sticky_z_perc = 0.5f;
    public float sticky_w_perc = 0.5f;
    public float sticky_h_perc = 0.5f;
    public GameObject parent_obj;
    private float y_hi, x_lo, z_lo;

    public Transform new_obj;
    private Vector3 init_pos;
    private int drag_mask;
    private MeshCollider new_obj_col;

    public void OnBeginDrag(PointerEventData eventData)
    {
        init_pos = gameObject.transform.position;
        new_obj = Instantiate(new_obj);
        new_obj.SetParent(null);
        new_obj.localPosition = Vector3.zero;
        new_obj.transform.localScale = Vector3.one * 100;
        new_obj.gameObject.layer = LayerMask.NameToLayer("on_drag");
        drag_mask = 1 << new_obj.gameObject.layer;

        // for pivot pointer deviation calculation
        new_obj_col = new_obj.GetComponent<MeshCollider>();

        // calculate the y-axis surface bound coords
        
    }

    public void OnDrag(PointerEventData eventData)
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~drag_mask))
        {
            // re-center the new_obj
            new_obj.transform.position = hit.point + new_obj_col.bounds.extents.y * Vector3.one - (new_obj_col.bounds.center - new_obj.transform.position).y * Vector3.one;
            transform.position = init_pos;


            var p_bds = hit.transform.gameObject.GetComponent<Renderer>().bounds;
            y_hi = p_bds.max.y;
            x_lo = p_bds.min.x;
            z_lo = p_bds.min.z;

            // check if the obj is dragged within the bounds
            var x = hit.point.x;
            var z = hit.point.z;
            var i = 0;
        }
        else
        {
            transform.position = eventData.position;
            new_obj.transform.position = Vector3.zero;
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = init_pos;
        if (new_obj.transform.position == Vector3.zero)
            Destroy(new_obj.gameObject);
    }
}