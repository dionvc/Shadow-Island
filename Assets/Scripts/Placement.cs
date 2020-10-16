using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placement : MonoBehaviour
{
    [SerializeField] GameObject[] placeItems;
    [SerializeField] GameObject[] placeSprites;
    [SerializeField] Grid placeGrid;
    int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 place = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 placeRounded = new Vector3(Mathf.Round(place.x), Mathf.Round(place.y), 0);
        Vector3 placeVector = placeGrid.WorldToCell(placeRounded);
        Vector3 size = placeItems[index].GetComponent<BoxCollider2D>().size;
        size = new Vector3(Mathf.Ceil(size.x), Mathf.Ceil(size.y));
        Vector3 offset = new Vector3(0, 0);
        if(Mathf.Round(size.x)%2 != 0)
        {
            offset.x = 0.5f;
        }
        if(Mathf.Round(size.y)%2 != 0)
        {
            offset.y = 0.5f;
        }
        placeSprites[index].transform.position = placeVector - offset;
        if (Input.GetMouseButton(0))
        {
            if (Physics2D.OverlapBoxAll(placeVector - offset, size/2.0f, 0.0f).Length == 0) {
                GameObject building = Instantiate(placeItems[index], placeVector - offset, Quaternion.identity);
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            index = 0;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            index = 1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            index = 2;
        }
    }
}
