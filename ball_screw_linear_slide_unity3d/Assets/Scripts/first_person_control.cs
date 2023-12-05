using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class first_person_control : MonoBehaviour
{
    public float speed;
    public float rot_speed;
    private Vector3 pos;
    private float x_rot, y_rot;
    // Start is called before the first frame update
    void Start()
    {
        pos = gameObject.transform.position;
        x_rot = 0;
        y_rot = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float forward = 0, right = 0;
        if (Input.GetKey(KeyCode.W))
            forward = 1f;
        if (Input.GetKey(KeyCode.S))
            forward = -1f;
        if (Input.GetKey(KeyCode.A))
            right = -1f;
        if (Input.GetKey(KeyCode.D))
            right = 1f;

        // rotate when the right mouse is pressed
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            x_rot += mouseX * rot_speed;
            y_rot -= mouseY * rot_speed;

            transform.eulerAngles = new Vector3(y_rot, x_rot, 0);
            transform.Translate(new Vector3(right, 0f, forward).normalized * speed * Time.deltaTime);
        }

    }
}
