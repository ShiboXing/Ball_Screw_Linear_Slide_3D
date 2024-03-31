using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class view_button_manager : MonoBehaviour
{
    public GameObject main_cam, board;
    public Button up_btn, left_btn, right_btn, front_btn, back_btn, schieber_btn1, schieber_btn2;
    // Start is called before the first frame update
    void Start()
    {
        up_btn.onClick.AddListener(clicked_up);
        right_btn.onClick.AddListener(clicked_right);
        left_btn.onClick.AddListener(clicked_left);
        front_btn.onClick.AddListener(clicked_front);
        back_btn.onClick.AddListener(clicked_back);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clicked_up()
    {
        main_cam.transform.position = board.transform.position + Vector3.up * 17;
        main_cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
    public void clicked_right()
    {
        main_cam.transform.position = board.transform.position + Vector3.back * 20 + Vector3.up * 7;
        main_cam.transform.rotation = Quaternion.Euler(15f, 0f, 0f);
    }
    public void clicked_left()
    {
        main_cam.transform.position = board.transform.position + Vector3.back * -20 + Vector3.up * 7;
        main_cam.transform.rotation = Quaternion.Euler(15f, 180f, 0f);
    }
    public void clicked_front()
    {
        main_cam.transform.position = board.transform.position + Vector3.right * 30 + Vector3.up * 7;
        main_cam.transform.rotation = Quaternion.Euler(15f, -90f, 0f);
    }
    public void clicked_back()
    {
        main_cam.transform.position = board.transform.position + Vector3.left * 30 + Vector3.up * 7;
        main_cam.transform.rotation = Quaternion.Euler(15f, 90f, 0f);
    }

}
