using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GridDrawer : MonoBehaviour
{
    [SerializeField]
    GameObject TilePrefab;

    public static InstancePointer<GridDrawer> Instance = new InstancePointer<GridDrawer>();

    // Start is called before the first frame update
    void Start()
    { 
        CreateChunk(10, -10);
        CreateChunk(10, -7);
        CreateChunk(10, -5);
        CreateChunk(10, -2);
    }
    


    [SerializeField]
    int Width;

    List<byte[]> grid = new List<byte[]>();
    List<bool[]> gridMask = new List<bool[]>();

    [SerializeField]
    int ResourceCount;

    /// <summary>
    /// Three-sigma gauss random
    /// </summary>
    /// <param name="mean"></param>
    /// <param name="sigma"></param>
    /// <returns></returns>
    float GaussRandom(float mean, float sigma, float minValue = float.MinValue, float maxValue = float.MaxValue)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }


    int GetResourceFromIndex(float index)
	{
        const float SIGMA = 10;
        return Mathf.RoundToInt(GaussRandom(index, SIGMA, 0, ResourceCount));

	}
    void CreateChunk(int height, int medResource)
	{
        for (int i = 0; i < height; i++)
		{
            byte[] row = new byte[Width];
            bool[] maskRow = new bool[Width];
            grid.Add(row);
            gridMask.Add(maskRow);
            for (int j = 0; j < Width; j++)
			{
                row[j] = (byte)GetResourceFromIndex(medResource);
                maskRow[j] = true;
			}
		}
        currentDepth += height;
	}

    public bool CheckCell(int x, int y)
	{
        return y >= 0 && y < currentDepth && x >= 0 && x < Width && gridMask[y][x];
	}

    public byte CheckNeighbours(int x, int y)
	{
        byte val = 0;
        if (CheckCell(x + 1, y)) val += 1 << 0;
        if (CheckCell(x, y + 1)) val += 1 << 1;
        if (CheckCell(x - 1, y)) val += 1 << 2;
        if (CheckCell(x, y - 1)) val += 1 << 3;
        return val;
	}

    int currentDepth = 0;
    int lastDepth = 0;
    // Update is called once per frame
    void Update()
    {
        if (currentDepth != lastDepth)
		{
            

            for (int i = lastDepth; i < currentDepth; i++)
			{
                for (int j = 0; j < Width; j++) {
                    var tile = GameObject.Instantiate(TilePrefab);
                    tile.transform.position = new Vector3(j, -i, 0);
                    tile.GetComponent<GridCell>().InitData(j, i, grid[i][j]);
                }
			}


            lastDepth = currentDepth;
        }
    }
}
