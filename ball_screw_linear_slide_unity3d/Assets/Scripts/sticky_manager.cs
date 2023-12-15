using System.Collections.Generic;
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
                // reset it to the CENTER of the bound
                var x_mid = x_base + w / 2;
                var y_mid = y_base + h / 2;
                var z_mid = z_base + l / 2;
                if (Mathf.Abs(x_mid) == float.PositiveInfinity)
                    sticky_trans.position = new Vector3(x_hit, y_mid, z_mid);
                else if (Mathf.Abs(y_mid) == float.PositiveInfinity)
                    sticky_trans.position = new Vector3(x_mid, y_hit, z_mid);
                else if (Mathf.Abs(x_mid) == float.PositiveInfinity)
                    sticky_trans.position = new Vector3(x_mid, y_mid, z_hit);
                else
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
        sticky_map["GSX60C-H0前端座"] = new HashSet<string> { "SGSX60C000000003-轴承端盖" };

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
        sticky_bounds["底板"]["GSX60C-H0前端座"].AddRange(new float[] { 0.74f, 0f, 0.32f, 0.07f, float.PositiveInfinity, 0.35f });
        sticky_bounds["GSX60C-H0前端座"]["SGSX60C000000003-轴承端盖"].AddRange(new float[] { -0.1f, 0.27f, 0.21f, 0.13f, 0.48f, 0.61f });
    }

    // Update is called once per frame
    void Update()
    {

    }


}
