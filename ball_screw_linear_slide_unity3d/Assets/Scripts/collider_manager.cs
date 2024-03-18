using UnityEngine;

public class collider_manager : MonoBehaviour
{
    private bool duplicated = false;
    // Start is called before the first frame update
    void Start()
    { 
    }


    private void OnTriggerEnter (Collider col)
    {
        if (sticky_manager.same_obj(col.gameObject.name, gameObject.name))
            duplicated = true;
    }

    private void OnTriggerExit(Collider col)
    {
        if (sticky_manager.same_obj(col.gameObject.name, gameObject.name))
            duplicated = false;
    }

    public bool check_duplicated()
    {
        return duplicated;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
