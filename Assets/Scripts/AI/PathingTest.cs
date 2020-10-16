using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathingTest : MonoBehaviour
{
    [SerializeField] GameObject Marker;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        PathNode pathTest = GetComponent<Pathing>().GetPath(new Vector3(5, 5), new Vector3(64.0f, 64.0f), 500);
        PathNode node = pathTest;
        while (node.parentNode != null)
        {
            Instantiate(Marker, node.coords + new Vector3(0.5f , 0.5f), Quaternion.identity);
            node = node.parentNode;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
