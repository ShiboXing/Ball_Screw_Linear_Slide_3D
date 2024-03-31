using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class view_button_manager : MonoBehaviour
{
    public GameObject main_cam;
    public Button btn;
    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Button clicked");
        main_cam.transform.position = new Vector3(-0.28f, 16.53f, 200.98f);
        main_cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    public void OnClick()
    {
        Debug.Log("Button clicked");
        main_cam.transform.position = new Vector3(-0.28f, 16.53f, 200.98f);
        main_cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

}
