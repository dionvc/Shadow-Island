using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallUpdater : MonoBehaviour
{
    [SerializeField] Sprite[] wallSprites; //order single -> left -> center -> right -> left with below -> center with below -> right with below -> single with below
    bool markedDestroy = false;
    [SerializeField] int wallCode = 0;

    private void Start()
    {
        UpdateWallNeighbors();
    }

    public void UpdateWallNeighbors()
    {
        SpriteRenderer srenderer = this.GetComponent<SpriteRenderer>();
        //Requirements - update neighbor walls but
        Collider2D[] results = new Collider2D[12];
        ContactFilter2D filter = new ContactFilter2D();
        bool hasLeft = false;
        bool hasRight = false;
        bool hasBelow = false;
        filter.useLayerMask = true;
        filter.layerMask = 1 << LayerMask.NameToLayer("Damageable");
        int count = Physics2D.OverlapBox(this.transform.position, new Vector2(2, 2), 0, filter, results);
        for (int i = 0; i < count; i++)
        {
            WallUpdater updater;
            if (results[i].gameObject.TryGetComponent<WallUpdater>(out updater) && updater.markedDestroy == false)
            {
                if (updater.wallCode != this.wallCode) continue;
                float diffX = results[i].transform.position.x - this.transform.position.x;
                float diffY = Mathf.Abs(results[i].transform.position.y - this.transform.position.y);
                bool below = results[i].transform.position.y < this.transform.position.y;
                if (below && Mathf.Abs(diffX) < 0.2f)
                {
                    hasBelow = true;
                }
                if (diffY < 0.2f && diffX < 0)
                {
                    hasLeft = true;
                }
                if (diffY < 0.2f && diffX > 0)
                {
                    hasRight = true;
                }
                updater.UpdateWall();
            }
        }
        if(markedDestroy)
        {
            return;
        }

        if (hasRight == false && hasLeft == false && hasBelow == false)
        {
            srenderer.sprite = wallSprites[0];
        }
        else if (hasRight == false && hasLeft == false && hasBelow == true)
        {
            srenderer.sprite = wallSprites[7];
        }
        else if (hasRight == true && hasLeft == false && hasBelow == false)
        {
            srenderer.sprite = wallSprites[1];
        }
        else if (hasRight == true && hasLeft == true && hasBelow == false)
        {
            srenderer.sprite = wallSprites[2];
        }
        else if (hasRight == false && hasLeft == true && hasBelow == false)
        {
            srenderer.sprite = wallSprites[3];
        }
        else if (hasRight == true && hasLeft == false && hasBelow == true)
        {
            srenderer.sprite = wallSprites[4];
        }
        else if (hasRight == true && hasLeft == true && hasBelow == true)
        {
            srenderer.sprite = wallSprites[5];
        }
        else if (hasRight == false && hasLeft == true && hasBelow == true)
        {
            srenderer.sprite = wallSprites[6];
        }
    }

    public void UpdateWall()
    {
        SpriteRenderer srenderer = this.GetComponent<SpriteRenderer>();
        //Requirements - update neighbor walls but
        Collider2D[] results = new Collider2D[12];
        ContactFilter2D filter = new ContactFilter2D();
        bool hasLeft = false;
        bool hasRight = false;
        bool hasBelow = false;
        filter.useLayerMask = true;
        filter.layerMask = 1 << LayerMask.NameToLayer("Damageable");
        int count = Physics2D.OverlapBox(this.transform.position, new Vector2(2, 2), 0, filter, results);
        for (int i = 0; i < count; i++)
        {
            WallUpdater updater;
            if (results[i].gameObject.TryGetComponent<WallUpdater>(out updater))
            {
                if (updater.wallCode != this.wallCode) continue;
                float diffX = results[i].transform.position.x - this.transform.position.x;
                float diffY = Mathf.Abs(results[i].transform.position.y - this.transform.position.y);
                bool below = results[i].transform.position.y < this.transform.position.y;
                if (below && Mathf.Abs(diffX) < 0.2f)
                {
                    hasBelow = true;
                }
                if (diffY < 0.2f && diffX < 0)
                {
                    hasLeft = true;
                }
                if (diffY < 0.2f && diffX > 0)
                {
                    hasRight = true;
                }
            }
        }

        if (hasRight == false && hasLeft == false && hasBelow == false)
        {
            srenderer.sprite = wallSprites[0];
        }
        else if (hasRight == false && hasLeft == false && hasBelow == true)
        {
            srenderer.sprite = wallSprites[7];
        }
        else if (hasRight == true && hasLeft == false && hasBelow == false)
        {
            srenderer.sprite = wallSprites[1];
        }
        else if (hasRight == true && hasLeft == true && hasBelow == false)
        {
            srenderer.sprite = wallSprites[2];
        }
        else if (hasRight == false && hasLeft == true && hasBelow == false)
        {
            srenderer.sprite = wallSprites[3];
        }
        else if (hasRight == true && hasLeft == false && hasBelow == true)
        {
            srenderer.sprite = wallSprites[4];
        }
        else if (hasRight == true && hasLeft == true && hasBelow == true)
        {
            srenderer.sprite = wallSprites[5];
        }
        else if (hasRight == false && hasLeft == true && hasBelow == true)
        {
            srenderer.sprite = wallSprites[6];
        }
    }

    void OnDestroy()
    {
        markedDestroy = true;
        UpdateWallNeighbors();
    }
}
