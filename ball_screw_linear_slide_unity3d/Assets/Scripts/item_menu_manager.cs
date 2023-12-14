using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class item_menu_manager : MonoBehaviour
{
    public List<GameObject> objs;
    public Transform parentTransform;
    public MonoScript texture_script;
    public MonoScript drag_script;
    public float toggle_speed = 10f;
    public float margin = 20;

    private List<RawImage> imgs;
    private float item_width;

    // animation
    private bool moving = false;
    private bool moving_left = false;
    private float shift_amt;

    // Start is called before the first frame update
    void Start()
    {
        // UI coords
        imgs = new List<RawImage>();
        float x_offset = 0;
        float img_x_offset = 0;
        var menu_rect = gameObject.GetComponent<RectTransform>();
        float item_height = menu_rect.rect.height - margin;
        item_width = menu_rect.rect.width / 15f - margin;
        menu_rect.sizeDelta= new Vector2(item_width * 100, menu_rect.sizeDelta.y); // make it enough for 100 items

        // build menu images
        for (int i=0; i<objs.Count; i++) 
        {
            // construct the raw image
            var inner_im = new GameObject("raw_im");
            inner_im.transform.SetParent(gameObject.transform);
            var im = inner_im.AddComponent<RawImage>();
            Texture2D text = Texture2D.whiteTexture;
            im.texture = text;
            RectTransform rect_t= inner_im.GetComponent<RectTransform>();
            rect_t.localPosition = new Vector3(img_x_offset, item_height / 4, 0);
            rect_t.sizeDelta = new Vector2(item_width, item_height);

            // add object name label beneath the image
            var name = objs[i].name;
            var label_obj = new GameObject("label_text");
            label_obj.transform.SetParent(gameObject.transform);
            var label_text = label_obj.AddComponent<Text>();
            label_text.text = name;
            label_text.font = Resources.GetBuiltinResource(typeof(Font), "LegacyRuntime.ttf") as Font;
            label_text.color = Color.black;
            RectTransform label_rect = label_text.GetComponent<RectTransform>();
            label_rect.localPosition = new Vector3(img_x_offset, -item_height / 3, 0);
            label_rect.sizeDelta = new Vector2(item_width, item_height / 6);

            // update the horizontal offset
            img_x_offset += item_width + margin;

            // Attach the 3d obj under the raw image for drag - and - drop instantiating
            var real_obj = Instantiate(objs[i]);
            real_obj.name = name;
            var drag_script_ins = im.gameObject.AddComponent(drag_script.GetClass());
            FieldInfo new_obj_field = drag_script_ins.GetType().GetField("sticky_obj_save");
            new_obj_field.SetValue(drag_script_ins, real_obj.transform);

            // get the width of the obj
            var max_bound = Mathf.Max(5f, find_max_bound(objs[i]));

            // position the objs apart from each other (and zigzag on y axis)
            x_offset += max_bound / 2;
            objs[i].transform.position = new Vector3(x_offset, 100 + (i & 1) * 100, 0);
            x_offset += max_bound / 2;

            // attach the texture script to game object
            var texture_script_ins = objs[i].AddComponent(texture_script.GetClass());
            FieldInfo raw_img_field = texture_script_ins.GetType().GetField("raw_img");
            FieldInfo max_bound_field = texture_script_ins.GetType().GetField("max_bound");
            raw_img_field.SetValue(texture_script_ins, im);
            max_bound_field.SetValue(texture_script_ins, max_bound);
            
            // store the raw images
            imgs.Add(im);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!moving && Input.GetKeyDown(KeyCode.Q))
        {
            shift_amt = item_width + margin;
            moving = true;
            moving_left = false;
        } 
        else if (!moving && Input.GetKeyDown(KeyCode.E))
        {
            shift_amt = item_width + margin;
            moving = true;
            moving_left = true;
        }
        if (moving)
        {
            Vector3 offset = Vector3.right * Mathf.Min(toggle_speed, shift_amt);
            gameObject.transform.localPosition += moving_left ? -offset : offset;
            shift_amt -= offset.x;
            if (shift_amt <= 0) moving = false;
        }
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
