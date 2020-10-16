using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testAI : MonoBehaviour
{
    [SerializeField] Grid tileGrid = null;
    [SerializeField] GameObject startMarker;
    [SerializeField] GameObject endMarker;
    Pathing pather = null;
    PathNode currentPath = null;
    Rigidbody2D rb = null;
    List<Object> pathMarkers = new List<Object>();
    [SerializeField] GameObject marker;
    // Start is called before the first frame update
    void Start()
    {
        pather = tileGrid.GetComponent<Pathing>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(currentPath == null)
        {
            Vector3 end = transform.position + new Vector3(Random.value * 10, Random.value * 10);
            Vector3 start = transform.position;

            currentPath = pather.GetPath(end, start, 500);
            startMarker.transform.position = new Vector3((int)start.x + 0.5f, (int)start.y - 0.5f, 0);
            endMarker.transform.position = new Vector3((int)end.x + 0.5f, (int)end.y - 0.5f, 0);
            PathNode node = currentPath;
            for(int i = 0; i < pathMarkers.Count; i++)
            {
                Object.Destroy(pathMarkers[i]);
            }
            pathMarkers.Clear();
            while (node.parentNode != null)
            {
                pathMarkers.Add(Instantiate(marker, node.coords + new Vector3(0.5f, 0.5f), Quaternion.identity));
                node = node.parentNode;
            }
        }
        else
        {
            rb.velocity = 2 * Vector3.Normalize(currentPath.coords - transform.position + new Vector3(0.5f, 0.5f));
            if(Vector3.Magnitude(currentPath.coords - transform.position + new Vector3(0.5f, 0.5f)) < 0.1f)
            {
                currentPath = currentPath.parentNode;
            }
        }
    }
}
