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
    public void OnBeginDrag(PointerEventData eventData)
    {
        var obj = gameObject.transform.Find("real_obj");
        init_pos = gameObject.transform.position;
        new_obj = Instantiate(obj.transform);
        new_obj.SetParent(null);
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = init_pos;
        new_obj.position = eventData.position;
    }
}
