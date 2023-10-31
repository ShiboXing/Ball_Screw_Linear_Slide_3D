using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class item_menu_manager : MonoBehaviour
{
    public List<GameObject> objs;
    public Transform parentTransform;
    public MonoScript texture_script;

    private List<RawImage> imgs;
    // Start is called before the first frame update
    void Start()
    {
        imgs = new List<RawImage>();
        float x_offset = 0;
        for (int i=0; i<objs.Count; i++) 
        {
            // wrap the obj (centering the rotational pivot) 
            //var obj_wrap = new GameObject("obj_wrap_" + i);
            //objs[i].transform.parent = obj_wrap.transform;
            //objs[i] = obj_wrap;

            // construct the raw image
            var inner_im = new GameObject("raw_im");
            inner_im.transform.SetParent(gameObject.transform);
            var im = inner_im.AddComponent<RawImage>();
            Texture2D text = Texture2D.whiteTexture;
            im.texture = text;
            RectTransform rect_t= inner_im.GetComponent<RectTransform>();
            rect_t.localPosition = Vector3.right * i * 110;

            // get the width of the obj
            var max_bound = Mathf.Max(5f, find_max_bound(objs[i]));

            // position the objs apart from each other (and zigzag on y axis)
            x_offset += max_bound / 2;
            objs[i].transform.position = new Vector3(x_offset, 100 + (i & 1) * 100, 0);
            x_offset += max_bound / 2;


            // attach the texture script to game object
            var script = objs[i].AddComponent(texture_script.GetClass());
            FieldInfo raw_img_field = script.GetType().GetField("raw_img");
            FieldInfo max_bound_field = script.GetType().GetField("max_bound");
            raw_img_field.SetValue(script, im);
            max_bound_field.SetValue(script, max_bound);
            
            // store the raw images
            imgs.Add(im);
            
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // dfs to find mesh filter in a 3d object
    public float find_max_bound(GameObject obj)
    {
        float max_bound = 0;
        var filter = obj.GetComponent<MeshFilter>();
        if (filter)
        {
            var size = filter.mesh.bounds.size;
            max_bound = Mathf.Max(size.x, size.y, size.z) * 100 * 1.2f;
        }

        foreach (Transform child in obj.transform)
            max_bound = Mathf.Max(max_bound, find_max_bound(child.gameObject));

        return max_bound;
    }
}
