using UnityEngine;
using UnityEngine.UI;

public class build_gyrate_texture : MonoBehaviour
{
    public RawImage raw_img;
    public float rot_speed = 50f;
    public float max_bound = 0f;

    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("UI cam");
        apply_layer_mask_all(gameObject.layer, gameObject);
        var texture = new RenderTexture(255, 255, 0);

        var cam_obj = new GameObject("Main Camera");
        cam = cam_obj.AddComponent<Camera>();
        //cam.transform.SetParent(gameObject.transform, false);
        cam.transform.position = gameObject.transform.position + Vector3.forward * max_bound;
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

        if (gameObject.transform.eulerAngles.x != 0)
            gameObject.transform.Rotate(0, 0, Time.deltaTime * rot_speed);
        else
            gameObject.transform.Rotate(0, Time.deltaTime * rot_speed, 0);
        cam.Render();
    } 

    void apply_layer_mask_all(LayerMask mask, GameObject obj)
    {
        obj.layer = mask;
        
        foreach (Transform child in obj.transform)
        {
            apply_layer_mask_all(mask, child.gameObject);
        }
    }
}
