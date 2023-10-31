using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class item_menu_manager : MonoBehaviour
{
    private List<RawImage> imgs;
    public Transform parentTransform;
    // Start is called before the first frame update
    void Start()
    {
        imgs = new List<RawImage>();
        for (int i = 0; i < 5; i++)
        {
            var inner_im = new GameObject("raw_im");
            inner_im.transform.SetParent(gameObject.transform);
            var im = inner_im.AddComponent<RawImage>();
            Texture2D text = Texture2D.whiteTexture;
            im.texture = text;

            RectTransform rect_t= inner_im.GetComponent<RectTransform>();
            rect_t.localPosition = Vector3.right * i * 110;
        }    
    }

    // Update is called once per frame
    void Update()
    {

    }
}
