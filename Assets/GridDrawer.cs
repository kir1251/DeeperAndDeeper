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

        for (int i = -6; i <= 10; i++)
        {
            CreateChunk(10, i);
        }

    }
    

    ///Ширина шахты
    [SerializeField]
    int Width;
    
    ///Количество разных ресурсов
    [SerializeField]
    int ResourceCount;



    ///Какие ресурсы лежат в клетках
    List<byte[]> grid = new List<byte[]>();
    
    ///Раскопаны ли клетки
    List<bool[]> gridMask = new List<bool[]>();

    

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

    [SerializeField]
    int averageVein = 20;
    [SerializeField]
    float veinDensity = .3f;
    [SerializeField]
    int veinSigma = 10;
    [SerializeField]
    float RANDOM_SIGMA = 6;
    int GetResourceFromIndex(float index)
	{
        return Mathf.RoundToInt(GaussRandom(index, RANDOM_SIGMA, 1, ResourceCount));
	}

        

    void CreateChunk(int height, int medResource)
	{
        for (int i = 0; i < height; i++)
		{
            byte[] row = new byte[Width];
            bool[] maskRow = new bool[Width];
            grid.Add(row);
            gridMask.Add(maskRow);
            currentDepth++;
            for (int j = 0; j < Width; j++)
            {
                row[j] = 255;
                maskRow[j] = true;
            }
            for (int j = 0; j < Width; j++)
            {                 
                int pos = Random.Range(0, Width);
                while (row[pos] != 255) pos = (pos + 1) % Width;


                //понять, стоит ли приделать эту клетку к одной из соседних жил
                bool isResource = false;
                int veinSize = 0;
                byte resource = 0;
                bool first = true;
                HashSet<int> visited = new HashSet<int>();
                Queue<int> queue = new Queue<int>();
                queue.Enqueue(pos + (grid.Count - 1) * Width);
                while (queue.Count > 0)
				{
                    int p = queue.Dequeue();
                    if (visited.Contains(p)) continue;
                    visited.Add(p);
                    int yIndex = p / Width;
                    int xIndex = p % Width;
                    byte cellRes = GetResource(xIndex, yIndex);
                    if (cellRes > 0 && cellRes != 255 && resource > 0 && resource != 255 && cellRes != resource)
                    {
                        isResource = false;
                        veinSize = -1;
                        break;
                    } 
                    if (cellRes > 0 && cellRes != 255 || first)
					{

                        if (!first)
                        {
                            veinSize++;
                            resource = cellRes;
                        }
                        if (xIndex > 0) queue.Enqueue(xIndex - 1 + yIndex * Width);
                        if (xIndex < Width - 1) queue.Enqueue(xIndex + 1 + yIndex * Width);
                        if (yIndex > 0) queue.Enqueue(xIndex + (yIndex - 1)* Width);
                        if (yIndex < grid.Count - 1) queue.Enqueue(xIndex + (yIndex + 1) * Width);
                    }
                    first = false;

                }
                float variatedSize = GaussRandom(averageVein, veinSigma);
                if (variatedSize > veinSize)
                {
                    isResource = veinSize >= 0;
                }

                if (veinSize == 0)
				{
                    isResource = Random.value < veinDensity / averageVein;
                }
                if (resource == 0) resource = (byte)GetResourceFromIndex(medResource);
                row[j] = isResource ? resource : (byte)0;
			}
		}

	}

    public bool CheckCell(int x, int y)
	{
        return y >= 0 && y < currentDepth && x >= 0 && x < Width && gridMask[y][x];
	}

    public byte GetResource(int x, int y)
    {
        return (y >= 0 && y < currentDepth && x >= 0 && x < Width) ? grid[y][x] : (byte)0;
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
