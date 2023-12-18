using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]
public sealed class sticky_manager : MonoBehaviour
{
    // (parent obj, stick obj) -> list of bounds specified by multiples of percentage values (x, y, z, w, h, length)
    public static Dictionary<string, Dictionary<string, List<float>>> sticky_bounds;
    // parent obj -> sticky objs
    public static Dictionary<string, HashSet<string>> sticky_map;

    public static bool is_dep(string child, string parent)
    {
        return sticky_map.ContainsKey(parent)
            && sticky_map[parent].Contains(child);
    }

    // check if the user has placed the sticky obj in the correct bound
    public static bool in_bound(Transform sticky_trans, Bounds bd, string parent, string child)
    {  
        // check if the obj is attached to the right parent obj
        if (!sticky_bounds.ContainsKey(parent) || !sticky_bounds[parent].ContainsKey(child))
            return false;
        
        // get ANSWER BOUNDS and check them one by one
        var bds = sticky_bounds[parent][child];
        for (int i = 0; i < bds.Count; i += 6)
        {

            float x_perc = bds[i];
            float y_perc = bds[i + 1];
            float z_perc = bds[i + 2];
            float w_perc = bds[i + 3];
            float h_perc = bds[i + 4];
            float l_perc = bds[i + 5];


            var x_base = bd.min.x + bd.size.x * x_perc;
            var y_base = bd.min.y + bd.size.y * y_perc;
            var z_base = bd.min.z + bd.size.z * z_perc;
            var w = bd.size.x * w_perc;
            var h = bd.size.y * h_perc;
            var l = bd.size.z * l_perc;

            var x_hit = sticky_trans.position.x;
            var y_hit = sticky_trans.position.y;
            var z_hit = sticky_trans.position.z;

            // check if the sticky object is IN BOUND
            if (x_base + w >= x_hit && x_hit >= x_base
                && y_base + h >= y_hit && y_hit >= y_base
                && z_base + l >= z_hit && z_hit >= z_base)
            {
                // RE-ADJUST for the OFFSET between the Renderer's BOUND and TRANSFORM
                var sticky_bds = sticky_trans.gameObject.GetComponent<Renderer>().bounds;
                var offset = sticky_trans.position - sticky_bds.center;

                // reset it to the CENTER of the bound
                var x_mid = x_base + offset.x + w / 2;
                var y_mid = y_base + offset.y + h / 2;
                var z_mid = z_base + offset.z + l / 2;
                sticky_trans.position = new Vector3(x_mid, y_mid, z_mid);
                return true;
            }

        }

        return false;
    }


    // Start is called before the first frame update
    void Start()
    {
        // instantiate the dependency map
        sticky_map = new Dictionary<string, HashSet<string>>();
        sticky_map["底板"] = new HashSet<string> { "GSX60C-H0前端座" };
        sticky_map["GSX60C-H0前端座"] = new HashSet<string>
        {
            "SGSX60C000000003-轴承端盖",
            "SGSX060CM00670015-GSX60C丝杆",
            "SGSX60C000000004-圆螺母",
        };

        sticky_map["SGSX060CM00670015-GSX60C丝杆"] = new HashSet<string> {
            "SFC-26V",
            "面板螺母固定块" 
        };


        // instantiate the bound coordinates mapping
        sticky_bounds = new Dictionary<string, Dictionary<string, List<float>>>();
        foreach (var kv in sticky_map)
        {
            if (!sticky_bounds.ContainsKey(kv.Key))
                sticky_bounds[kv.Key] = new Dictionary<string, List<float>>();
            foreach (var val in sticky_map[kv.Key])
            {
                if (!sticky_bounds[kv.Key].ContainsKey(val))
                    sticky_bounds[kv.Key][val] = new List<float>();
            }
        }
        // when width is 2 times the x-offset, the sticky obj is centered in the x axis of the parent object. Same applies to other axes
        sticky_bounds["底板"]["GSX60C-H0前端座"].AddRange(new float[] { 0.74f, 1f, 0.32f, 0.07f, 4f, 0.35f });
        sticky_bounds["GSX60C-H0前端座"]["SGSX60C000000003-轴承端盖"].AddRange(new float[] { -0.1f, 0.27f, 0.21f, 0.13f, 0.48f, 0.61f });
        sticky_bounds["GSX60C-H0前端座"]["SGSX060CM00670015-GSX60C丝杆"].AddRange(new float[] { -10.64312f, 0.3883308f, 0.3909872f, 11.63333f, 0.25f, 0.2142864f });
        sticky_bounds["GSX60C-H0前端座"]["SGSX60C000000004-圆螺母"].AddRange(new float[] { 0.426317f, 0.35774518f, 0.3602898f, 0.2666667f, 0.3274215f, 0.280647f });
        sticky_bounds["SGSX060CM00670015-GSX60C丝杆"]["SFC-26V"].AddRange(new float[] { 0.9734778f, -0.5685194f, -0.5830744f, 0.06733526f, 2.166667f, 2.166659f });
        sticky_bounds["SGSX060CM00670015-GSX60C丝杆"]["面板螺母固定块"].AddRange(new float[] { 0.07297214f, -0.5963218f - 3 * 2.191667f, -0.9945007f - 3 * 2.99999f, 0.1432665f, 2.191667f * 7, 2.99999f * 7 }); 
    }

    // Update is called once per frame
    void Update()
    {

    }


}
