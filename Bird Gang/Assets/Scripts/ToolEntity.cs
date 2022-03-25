using UnityEngine;

public class ToolEntity : MonoBehaviour
{
    private void Awake()
    {
        MeshRenderer r = GetComponent<MeshRenderer>();
        if (r)
            r.enabled = false;
    }
}
