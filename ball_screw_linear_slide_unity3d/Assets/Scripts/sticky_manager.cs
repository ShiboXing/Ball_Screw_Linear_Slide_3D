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
    // (parent, sticky, index) -> (new sticky name)
    public static Dictionary<string, Dictionary<string, Dictionary<int, string>>> rename_map;

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
        int interval = 6;
        for (int i = 0; i < bds.Count; i += interval)
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

                // STICKY OBJ RENAMING PHASE
                if (rename_map[parent][sticky_trans.name].ContainsKey(i/interval))
                    sticky_trans.name = rename_map[parent][sticky_trans.name][i / interval];

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
        sticky_bounds = new Dictionary<string, Dictionary<string, List<float>>>();
        rename_map = new Dictionary<string, Dictionary<string, Dictionary<int, string>>>();

        sticky_map["底板"] = new HashSet<string> { 
            "GSX60C-H0前端座",
            "轴承支座",
            "电机座",
            "轴承支座",
            "AGETE13-295-25 米思米导轨高度垫块"
        };
        sticky_map["GSX60C-H0前端座"] = new HashSet<string>
        {
            "SGSX60C000000003-轴承端盖",
            "SGSX060CM00670015-GSX60C丝杆",
            "SGSX60C000000004-圆螺母",
            "大螺丝"
        };
        sticky_map["SGSX060CM00670015-GSX60C丝杆"] = new HashSet<string> {
            "面板螺母固定块",
            "628-6轴承"
        };
        sticky_map["SGSX60C000000004-圆螺母"] = new HashSet<string>
        {
            "SFC-26V"
        };
        sticky_map["电机座"] = new HashSet<string> {
            "42步进电机",
            "小螺丝",
            "大螺丝"
        };
        sticky_map["SGSX60C000000003-轴承端盖"] = new HashSet<string>
        {
            "小螺丝"
        };
        sticky_map["AGETE13-295-25 米思米导轨高度垫块"] = new HashSet<string>
        {
            "导轨MR12MN"
        };
        sticky_map["AGETE13-295-25 米思米导轨高度垫块_0"] = new HashSet<string>
        {
            "导轨MR12MN"
        };
        sticky_map["AGETE13-295-25 米思米导轨高度垫块_1"] = new HashSet<string>
        {
            "导轨MR12MN"
        };
        sticky_map["导轨MR12MN_0"] = new HashSet<string>
        {
            "MR12MN滑块"
        };
        sticky_map["导轨MR12MN_1"] = new HashSet<string>
        {
            "MR12MN滑块"
        };
        sticky_map["MR12MN滑块_0"] = new HashSet<string>
        {
            "丝杆结构面板"
        };
        sticky_map["MR12MN滑块_1"] = new HashSet<string>
        {
            "丝杆结构面板"
        };
        sticky_map["SFC-26V"] = new HashSet<string>
        {
            "中螺丝"
        };

        // instantiate the bound coordinates mapping
        foreach (var kv in sticky_map)
        {
            if (!sticky_bounds.ContainsKey(kv.Key))
            {
                sticky_bounds[kv.Key] = new Dictionary<string, List<float>>();
                rename_map[kv.Key] = new Dictionary<string, Dictionary<int, string>>();
            }
            foreach (var val in sticky_map[kv.Key])
            {
                if (!sticky_bounds[kv.Key].ContainsKey(val))
                {
                    sticky_bounds[kv.Key][val] = new List<float>();
                    rename_map[kv.Key][val] = new Dictionary<int, string>();
                }
            }
        }

        // BOUND BOXES SPECIFICATIONS
        // Can make the specified box n times wider/higher/lengthier to allow more leeway for user
        sticky_bounds["底板"]["GSX60C-H0前端座"].AddRange(new float[] { 0.7402945f, 0.9999999f, 0.3250647f, 0.06896553f, 4f, 0.3510973f });
        sticky_bounds["底板"]["电机座"].AddRange(new float[] { 0.8382539f, 0.9999999f, 0.3255995f, 0.05057471f, 4f, 0.3510972f });
        sticky_bounds["底板"]["轴承支座"].AddRange(new float[] { 0.004624289f, 0.9961535f, 0.3560415f, 0.0275862f, 4f, 0.2821316f});
        sticky_bounds["底板"]["AGETE13-295-25 米思米导轨高度垫块"].AddRange(new float[] { 0.05528285f - 3*0.6781608f, 0.9999996f, 0.8107246f - 5*0.07523511f, 0.6781608f * 7, 2.083333f, 0.07523511f * 11,
                                                                                       0.05528285f - 3*0.6781608f, 0.9999996f, 0.1142362f - 5*0.07523511f, 0.6781608f * 7, 2.083333f, 0.07523511f * 11});

        sticky_bounds["电机座"]["42步进电机"].AddRange(new float[] { -0.0787995f, 0.05959356f, 0.1031739f, 3.272728f, 0.8812502f, 0.7553574f });
        sticky_bounds["GSX60C-H0前端座"]["SGSX60C000000003-轴承端盖"].AddRange(new float[] { -0.1016667f, 0.2816641f, 0.1965304f, 0.1333334f, 0.4791667f, 0.6071427f });
        sticky_bounds["GSX60C-H0前端座"]["SGSX060CM00670015-GSX60C丝杆"].AddRange(new float[] { -10.64312f, 0.3883308f, 0.3909872f, 11.63333f, 0.25f, 0.2142864f });
        sticky_bounds["GSX60C-H0前端座"]["SGSX60C000000004-圆螺母"].AddRange(new float[] { 0.426317f, 0.35774518f, 0.3602898f, 0.2666667f, 0.3274215f, 0.280647f });
        sticky_bounds["SGSX60C000000004-圆螺母"]["SFC-26V"].AddRange(new float[] { 1.000002f, -0.405973f - 3*1.767344f, -0.3929593f - 3*1.767344f, 2.937499f, 7*1.767344f, 7*1.767344f });
        sticky_bounds["SGSX060CM00670015-GSX60C丝杆"]["面板螺母固定块"].AddRange(new float[] { 0.07297214f, -0.5963218f - 3 * 2.191667f, -0.9945007f - 3 * 2.99999f, 0.1432665f, 2.191667f * 7, 2.99999f * 7 });
        sticky_bounds["SGSX060CM00670015-GSX60C丝杆"]["628-6轴承"].AddRange(new float[] { 0f, 0.3883308f - 3 * 0.25f, 0.3909872f - 3 * 0.2142864f, 0.02432665f, 0.25f * 7, 0.2142864f * 7 });
        sticky_bounds["AGETE13-295-25 米思米导轨高度垫块_0"]["导轨MR12MN"].AddRange(new float[] { -0.0001975237f - 3*1f, 0.9999999f, -0.008544921f - 3*1f, 1f * 7, 0.3f, 1f*7 });
        sticky_bounds["AGETE13-295-25 米思米导轨高度垫块_1"]["导轨MR12MN"].AddRange(new float[] { -0.0001975237f - 3*1f, 0.9999999f, -0.008544921f - 3*1f, 1f*7, 0.3f, 1f *7});
        sticky_bounds["导轨MR12MN_0"]["MR12MN滑块"].AddRange(new float[] { 0.07455705f, 0.3966967f - 3 * 1.333333f, -0.6375248f - 3*2.25f, 0.1220339f, 7 * 1.333333f, 7*2.25f });
        sticky_bounds["导轨MR12MN_1"]["MR12MN滑块"].AddRange(new float[] { 0.07455705f, 0.3966967f - 3 * 1.333333f, -0.6375248f - 3*2.25f, 0.1220339f, 7 * 1.333333f, 7*2.25f });
        sticky_bounds["MR12MN滑块_1"]["丝杆结构面板"].AddRange(new float[] { -0.2622162f, 1f, -0.1252464f, 1.527778f, 1.200002f, 5.370371f });
        sticky_bounds["MR12MN滑块_0"]["丝杆结构面板"].AddRange(new float[] { -0.2622162f, 1f, -4.245984f, 1.527778f, 1.200002f, 5.370371f });

        // specs for screws (multiple boxese)
        sticky_bounds["电机座"]["小螺丝"].AddRange(new float[] { 0.5629686f, 0.782914f, 0.1724515f, 0.5909104f, 0.1145783f, 0.09812751f,
                                                               0.5629686f, 0.7899914f, 0.7260212f, 0.5909104f, 0.1145783f, 0.09812751f,
                                                               0.5629686f, 0.1433309f, 0.7279858f, 0.5909104f, 0.1145783f, 0.09812751f,
                                                               0.5629686f, 0.1433309f, 0.1724515f, 0.5909104f, 0.1145783f, 0.09812751f});
        sticky_bounds["电机座"]["大螺丝"].AddRange(new float[] { 0.07331847f, -0.184265f - 3*0.5208333f, 0.8309963f, 0.3861576f, 0.5208333f * 7, 0.1517203f,
                                                               0.07331847f, -0.184265f - 3*0.5208333f, 0.01311983f, 0.3861576f, 0.5208333f * 7, 0.1517203f});
        sticky_bounds["SGSX60C000000003-轴承端盖"]["小螺丝"].AddRange(new float[] { -0.9098019f, 0.6730435f, 0.7721215f, 3.250004f, 0.2391198f, 0.1616218f,
                                                                                  -0.9098019f, 0.6856521f, 0.06330152f, 3.250004f, 0.2391198f, 0.1616218f,
                                                                                  -0.9098019f, 0.07217385f, 0.7741814f, 3.250004f, 0.2391198f, 0.1616218f,
                                                                                  -0.9098019f, 0.07478257f, 0.06859273f, 3.250004f, 0.2391198f, 0.1616218f});
        sticky_bounds["GSX60C-H0前端座"]["大螺丝"].AddRange(new float[] { 0.1516163f, -0.01759844f - 3*0.5208333f, 0.8172576f, 0.2831822f, 0.5208333f * 7, 0.1517203f,
                                                                        0.6507257f, -0.1775984f - 3*0.5208333f, 0.04080634f, 0.2831822f, 0.5208333f * 7, 0.1517203f,
                                                                        0.6507257f, -0.1775984f - 3*0.5208333f, 0.8172576f, 0.2831822f, 0.5208333f * 7, 0.1517203f,
                                                                        0.1516163f, -0.01759844f - 3*0.5208333f, 0.03761562f, 0.2831822f, 0.5208333f * 7, 0.1517203f,});
        sticky_bounds["SFC-26V"]["中螺丝"].AddRange(new float[] { 0.7390078f, 0.3797785f - 3*0.5746338f, 0.6827561f, 0.2338643f, 7*0.5746338f, 0.2807588f});


        // RENAMING SPECIFICATIONS (FOR IDENTIFYING STICKY OBJ AT DIFFERENT PLACEMENT POSITIONS)
        rename_map["底板"]["AGETE13-295-25 米思米导轨高度垫块"][0] = "AGETE13-295-25 米思米导轨高度垫块_0";
        rename_map["底板"]["AGETE13-295-25 米思米导轨高度垫块"][1] = "AGETE13-295-25 米思米导轨高度垫块_1";
        rename_map["AGETE13-295-25 米思米导轨高度垫块_0"]["导轨MR12MN"][0] = "导轨MR12MN_0";
        rename_map["AGETE13-295-25 米思米导轨高度垫块_1"]["导轨MR12MN"][0] = "导轨MR12MN_1";
        rename_map["导轨MR12MN_0"]["MR12MN滑块"][0] = "MR12MN滑块_0";
        rename_map["导轨MR12MN_1"]["MR12MN滑块"][0] = "MR12MN滑块_1";

    }

    // Update is called once per frame
    void Update()
    {

    }


}
