
using UnityEngine;

public class first_person_control : MonoBehaviour
{
    public float speed;
    public float rot_speed;
    private Vector3 pos;
    private float x_rot, y_rot, z_rot;
    // Start is called before the first frame update
    void Start()
    {
        pos = gameObject.transform.position;
        // X, Y axes are inverted 
        x_rot = transform.eulerAngles.y;
        y_rot = transform.eulerAngles.x;
        z_rot = transform.eulerAngles.z;
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

            transform.eulerAngles = new Vector3(y_rot, x_rot, z_rot);
            transform.Translate(new Vector3(right, 0f, forward).normalized * speed * Time.deltaTime);
        }
    }
}
