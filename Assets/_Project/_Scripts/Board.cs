using System;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Transform emptySpritePrefab;
    [SerializeField] private Transform boardSpaces;
    [SerializeField] private byte boardHeight = 30, boardWidth = 10;
    [SerializeField] private byte header = 8;

    public int CompletedRows { get; private set; }

    private Transform[,] grid;

    [SerializeField] private ParticlePlayer[] clearRowFX = new ParticlePlayer[4];

    private void Awake()
    {
        grid = new Transform[boardWidth, boardHeight];
        DrawEmptyCells();
    }

    private void Start()
    {
        GameController.OnStoreShapeInGrid += StoreShapeInGrid;
    }

    private void DrawEmptyCells()
    {
        if (emptySpritePrefab)
        {
            for (int y = 0; y < boardHeight - header; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    Transform clone;
                    clone = Instantiate(emptySpritePrefab, new Vector3(x, y, 0), Quaternion.identity);
                    clone.name = $"Board Space ( x = {x} - y = {y})";
                    clone.transform.parent = boardSpaces;
                }
            }
        }
        else
        {
            Debug.LogError("No empty sprite transform!!");
        }
    }

    private bool IsWithinBoard(int x, int y)
    {
        return x >= 0 && x < boardWidth && y >= 0;
    }

    private bool IsOccupied(int x, int y, Shape shape)
    {
        return grid[x, y] != null && grid[x, y].parent != shape.transform;
    }

    public bool IsValidPosition(Shape shape)
    {
        foreach (Transform child in shape.transform)
        {
            Vector2 pos = VectorF.Round(child.position);

            if (!IsWithinBoard((int)pos.x, (int)pos.y))
            {
                return false;
            }

            if (IsOccupied((int)pos.x, (int)pos.y, shape))
            {
                return false;
            }
        }

        return true;
    }

    private void StoreShapeInGrid(Shape shape)
    {
        if (!shape) return;

        foreach (Transform child in shape.transform)
        {
            Vector2 pos = VectorF.Round(child.position);
            grid[(int)pos.x, (int)pos.y] = child;
        }
    }

    private bool IsRowComplete(int y)
    {
        for (int x = 0; x < boardWidth; ++x)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }

    private void ClearRow(int y)
    {
        for (int x = 0; x < boardWidth; ++x)
        {
            if (grid[x, y] != null)
            {
                Destroy(grid[x, y].gameObject);
            }
            grid[x, y] = null;
        }
    }

    public void ResetBoard()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (grid[x, y] != null)
                {
                    Destroy(grid[x, y].gameObject);
                }
            }
        }
    }

    private void ShiftOneRowDown(int y)
    {
        for (int x = 0; x < boardWidth; ++x)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    private void ShiftMultipleRowsDown(int startY)
    {
        for (int i = startY; i < boardHeight; ++i)
        {
            ShiftOneRowDown(i);
        }
    }

    public IEnumerator ClearAllRows(WaitForSeconds clearRowTimer)//Public??????????
    {
        CompletedRows = 0;

        for (int y = 0; y < boardHeight; ++y)
        {
            if (IsRowComplete(y))
            {
                ClearRowFX(CompletedRows, y);
                CompletedRows++;
            }
        }

        yield return new WaitForSeconds(1);

        for (int y = 0; y < boardHeight; ++y)
        {
            if (IsRowComplete(y))
            {
                ClearRow(y);
                ShiftMultipleRowsDown(y + 1);
                yield return clearRowTimer;
                y--;
            }
        }
    }

    public bool IsOverLimit(Shape shape)
    {
        foreach (Transform child in shape.transform)
        {
            if (child.transform.position.y >= (boardHeight - header - 1))
            {
                return true;
            }
        }
        return false;
    }

    private void ClearRowFX(int index, int y)
    {
        if (clearRowFX[index])
        {
            clearRowFX[index].transform.position = new Vector3(0, y, -2f);
            clearRowFX[index].PlayLandSquareParticles();
        }
    }
}
