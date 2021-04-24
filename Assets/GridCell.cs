using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{

    int PosX, PosY;

    byte resource;

    [SerializeField]
    Sprite GroundTile;

    [SerializeField]
    Sprite[] Resources;

    [SerializeField]
    Sprite[] Decorations = new Sprite[16];
    public void InitData(int x, int y, byte resource)
	{
        PosX = x;
        PosY = y;
        if (resource > 0) resourceRender.sprite = Resources[resource - 1];
        UpdateDecoration();
	}

    [SerializeField]
    SpriteRenderer tileRender;
    [SerializeField]
    SpriteRenderer resourceRender;
    [SerializeField]
    SpriteRenderer decorRender;

    void UpdateDecoration()
	{
        var gridDrawer = GridDrawer.Instance.Get();
        var sprite = Decorations[gridDrawer.CheckNeighbours(PosX, PosY)];
        decorRender.sprite = sprite;
	}    

    // Update is called once per frame
    void Update()
    {
        UpdateDecoration();
    }
}
