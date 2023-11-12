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
    private Vector3 init_pos;
    private Transform new_obj;
    private int drag_mask;
    private MeshCollider new_obj_col;
    public void OnBeginDrag(PointerEventData eventData)
    {
        var obj = gameObject.transform.Find("real_obj");
        init_pos = gameObject.transform.position;
        new_obj = Instantiate(obj.transform);
        new_obj.SetParent(null);
        new_obj.localPosition = Vector3.zero;
        new_obj.transform.localScale = Vector3.one * 100;
        new_obj.gameObject.layer = LayerMask.NameToLayer("on_drag");
        drag_mask = 1 << new_obj.gameObject.layer;

        // for pivot pointer deviation calculation
        new_obj_col = new_obj.GetComponent<MeshCollider>();
        
    }

    public void OnDrag(PointerEventData eventData)
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~drag_mask))
        {
            new_obj.transform.position = hit.point + new_obj_col.bounds.extents.y * Vector3.one - (new_obj_col.bounds.center - new_obj.transform.position).y * Vector3.one;
            transform.position = init_pos;
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
    }
}