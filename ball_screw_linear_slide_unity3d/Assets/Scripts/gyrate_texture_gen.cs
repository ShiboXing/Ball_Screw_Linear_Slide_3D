 using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class build_gyrate_texture : MonoBehaviour
{
    public RawImage raw_img;
    public float rot_speed = 50f;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("UI cam");
        var size = gameObject.GetComponent<MeshFilter>().mesh.bounds.size;
        var width = Mathf.Max(size.x, size.y, size.z) * 100 * 1.2f;
        var texture = new RenderTexture(255, 255, 0);

        var cam_obj = new GameObject("Main Camera");
        cam = cam_obj.AddComponent<Camera>();
        cam.transform.position = gameObject.transform.position + Vector3.forward * Mathf.Max(width, 5);
        cam.transform.LookAt(gameObject.transform);
        cam.cullingMask = 1 << gameObject.layer;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.clear;
        cam.targetDisplay = 1;
        cam.targetTexture = texture;
        cam.Render();

        raw_img.texture = texture;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(0, Time.deltaTime * rot_speed, 0);
        cam.Render();
    }
}
