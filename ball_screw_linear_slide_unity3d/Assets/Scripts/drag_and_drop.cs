using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public void OnBeginDrag(PointerEventData eventData)
    {
        var obj = gameObject.transform.Find("real_obj");
        init_pos = gameObject.transform.position;
        new_obj = Instantiate(obj.transform);
        new_obj.SetParent(null);
        new_obj.transform.localScale = Vector3.one * 100;
        new_obj.gameObject.layer = LayerMask.NameToLayer("on_drag");
        //drag_mask = 1 << new_obj.gameObject.layer;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            new_obj.transform.position = hit.point;
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
        new_obj.position = Vector3.zero;
        Destroy(new_obj.gameObject);
    }
}
