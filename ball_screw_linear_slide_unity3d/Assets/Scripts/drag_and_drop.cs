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
    public float sticky_x_perc = 0.7f;
    public float sticky_z_perc = 0.3f;
    public float sticky_w_perc = 0.3f;
    public float sticky_h_perc = 0.3f;
    public GameObject parent_obj;
    private float y_hi, x_lo, z_lo;

    public Transform sticky_obj_save;
    public Transform sticky_obj;
    private Vector3 init_pos;
    private int drag_mask;
    private MeshCollider new_obj_col;
    private Color orig_color;


    public void OnBeginDrag(PointerEventData eventData)
    {
        init_pos = gameObject.transform.position;
        sticky_obj = Instantiate(sticky_obj_save);
        sticky_obj.SetParent(null);
        sticky_obj.localPosition = Vector3.zero;
        sticky_obj.transform.localScale = Vector3.one * 100;
        sticky_obj.gameObject.layer = LayerMask.NameToLayer("on_drag");
        drag_mask = 1 << sticky_obj.gameObject.layer;
        orig_color = sticky_obj.GetComponent<Renderer>().material.color;

        // for pivot pointer deviation calculation
        new_obj_col = sticky_obj.GetComponent<MeshCollider>();
        
    }

    public void OnDrag(PointerEventData eventData)
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~drag_mask))
        {
            // re-center the new_obj
            sticky_obj.transform.position = hit.point + new_obj_col.bounds.extents.y * Vector3.one - (new_obj_col.bounds.center - sticky_obj.transform.position).y * Vector3.one;
            transform.position = init_pos;

            // check if sticky obj is in bound of the specified area
            var p_bds = hit.transform.gameObject.GetComponent<Renderer>().bounds;
            var x_lo = p_bds.min.x;
            var z_lo = p_bds.min.z;
            var x_hi = p_bds.max.x;
            var z_hi = p_bds.max.z;
            
            var w = (x_hi - x_lo) * sticky_w_perc;
            var h = (z_hi - z_lo) * sticky_h_perc;
            var x = x_lo + (x_hi - x_lo) * sticky_x_perc;
            var z = z_lo + (z_hi - z_lo) * sticky_z_perc;

            if (x+w > hit.point.x && hit.point.x > x && z+h > hit.point.z && hit.point.z > z)
                sticky_obj.GetComponent<Renderer>().material.color = Color.green;
            else
                sticky_obj.GetComponent<Renderer>().material.color = Color.red;
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

        if (rdn.material.color == Color.red && sticky_obj.transform.position == Vector3.zero)
            Destroy(sticky_obj.gameObject);
        rdn.material.color = orig_color;
    }
}