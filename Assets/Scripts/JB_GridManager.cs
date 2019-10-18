using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JB_GridManager : MonoBehaviour
{
    public int rows = 12;
    public int columns = 12;
    
    public GameObject tile;
    
    [SerializeField]
    private float length = 8.77f;
    [SerializeField]
    private float width = 8.77f;

    public GameObject[,] gridArray;
    public int startX = 0;
    public int startY = 0;
    public int endX = 2;
    public int endY = 2;

    public List<GameObject> path = new List<GameObject>();

    private void Awake()
    {
        gridArray = new GameObject[columns, rows];
    }

    // Start is called before the first frame update
    void Start()
    {

        for (int x = 0; x < columns; ++x)
        {
            length += 9f;

            width = 9f; // resetting value to it's original number

            for (int y = 0; y < rows; ++y)
            {
                // spawn grid cell and initialising variables
                Vector3 spawnPoint = new Vector3(transform.position.x + length, transform.position.y + width, 89f);
                GameObject newTile = Instantiate(tile, spawnPoint, Quaternion.identity);
                newTile.transform.parent = gameObject.transform;
                
                // assigning variables on each tiles
                newTile.GetComponent<JB_Tile>().tilePosition = newTile.transform.position;
                newTile.GetComponent<JB_Tile>().x = x;
                newTile.GetComponent<JB_Tile>().y = y;

                gridArray[x, y] = newTile;
                width += 9f;
            }
        }


    }
    
    // reset all visited tiles to -1, starting at new x and y location
    private void InitialSetup()
    {
        foreach(GameObject obj in gridArray)
        {
            obj.GetComponent<JB_Tile>().visited = -1;
        }
        gridArray[startX, startY].GetComponent<JB_Tile>().visited = 0;
    }
    
    private bool TestDirection(int x, int y, int step, int direction)
    {
        // direction tells which case to use 1 is up, 2 is right, 3 is down, 4 is left
        switch (direction)
        {
            case 1:
                if (y + 1 < rows && gridArray[x,y+1] && gridArray[x, y + 1].GetComponent<JB_Tile>().visited == step)
                    return true;
                else
                    return false;
            case 2:
                if (x + 1 < columns && gridArray[x+1, y] && gridArray[x +1, y].GetComponent<JB_Tile>().visited == step)
                    return true;
                else
                    return false;
            case 3:
                if (y - 1 >-1 && gridArray[x, y - 1] && gridArray[x, y - 1].GetComponent<JB_Tile>().visited == step)
                    return true;
                else
                    return false;
            case 4:
                if (x - 1 >-1 && gridArray[x -1, y] && gridArray[x-1, y].GetComponent<JB_Tile>().visited == step)
                    return true;
                else
                    return false;
        }

        return false;
    }

    void SetVisited(int x, int y, int step)
    {
        if (gridArray[x, y])
            gridArray[x, y].GetComponent<JB_Tile>().visited = step;
    }
    
    void SetDistance()
    {
        InitialSetup();
        int x = startX;
        int y = startY;

        int[] testArray = new int[(rows * columns)];

        for(int step = 1; step < (rows * columns); step++)
        {
            foreach(GameObject obj in gridArray)
            {
                if (obj && obj.GetComponent<JB_Tile>().visited == (step - 1))
                    TestFourDirections(obj.GetComponent<JB_Tile>().x, obj.GetComponent<JB_Tile>().y, step);
            }
        }
    }

    void TestFourDirections(int x, int y, int step)
    {
        if (TestDirection(x, y, -1, 1))
        {
            SetVisited(x, y + 1, step);
        }
        if(TestDirection(x, y, -1, 2))
        {
            SetVisited(x + 1, y, step);
        }
        if (TestDirection(x, y, -1, 3))
        {
            SetVisited(x , y-1, step);
        }
        if (TestDirection(x, y, -1, 4))
        {
            SetVisited(x - 1, y, step);
        }
    }

    void SetPath()
    {
        int step;
        int x = endX;
        int y = endY;

        List<GameObject> tempList = new List<GameObject>();

        path.Clear();

        if(gridArray[endX, endY] && gridArray[endX, endY].GetComponent<JB_Tile>().visited > 0)
        {
            path.Add(gridArray[x, y]);
            step = gridArray[x, y].GetComponent<JB_Tile>().visited -1;
        }
        else
        {
            Debug.Log("Cant reach desired location");
            return;
        }

        for(int i = step; step > -1; step--)
        {
            if (TestDirection(x, y, step, 1))
            {
                tempList.Add(gridArray[x, y + 1]);
            }
            if (TestDirection(x, y, step, 2))
            {
                tempList.Add(gridArray[x+1, y]);
            }
            if (TestDirection(x, y, step, 3))
            {
                tempList.Add(gridArray[x, y - 1]);
            }
            if (TestDirection(x, y, step, 4))
            {
                tempList.Add(gridArray[x-1, y]);
            }

            GameObject tempObj = FindClosest(gridArray[endX, endY].transform, tempList);
            path.Add(tempObj);
            x = tempObj.GetComponent<JB_Tile>().x;
            y = tempObj.GetComponent<JB_Tile>().y;

            tempList.Clear();
        }

    }

    public int FindClosestShip()
    {
        SetDistance();
        SetPath();

        return path.Count;

    }


    GameObject FindClosest(Transform targetlocation, List<GameObject> list)
    {
        float currentDistance = rows * columns;
        int indexNumber = 0;

        for(int i = 0; i<list.Count; i++)
        {
            if(Vector3.Distance(targetlocation.position, list[i].transform.position)< currentDistance)
            {
                currentDistance = Vector3.Distance(targetlocation.position, list[i].transform.position);
                indexNumber = i;
            }
        }

        return list[indexNumber];
    }
}
