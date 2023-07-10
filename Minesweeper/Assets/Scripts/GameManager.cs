using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Cell CellPrefab;
    public int size = 9;
    public int mineCount = 10;
    public float gap = 0.1f;
    public GameOverUI gameOverUI;
    public bool isFirstReveal = true;

    private Cell[,] _cells;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeCells();
        PlaceMines();
        CalculateMinesAroundCells();
    }

    private void InitializeCells()
    {
        GameObject cellContainer = new GameObject("CellContainer");
        _cells = new Cell[size, size];
        
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                CreateCell(row, column, cellContainer);
            }
        }
    }

    private void CreateCell(int row, int column, GameObject cellContainer)
    {
        var cell = Instantiate(CellPrefab, CalculateCellPosition(row, column), Quaternion.identity);
        cell.x = row;
        cell.y = column;
        _cells[row, column] = cell;
        cell.transform.SetParent(cellContainer.transform);
    }

    private Vector3 CalculateCellPosition(int row, int column)
    {
        return new Vector3(row * (1 + gap), column * (1 + gap), 0);
    }

    private void PlaceMines()
    {
        for (int i = 0; i < mineCount; i++)
        {
            var (x, y) = GetNextMineCoordinates();
            _cells[x, y].isMine = true;
        }
    }

    private (int x, int y) GetNextMineCoordinates()
    {
        int x, y;
        do
        {
            x = Random.Range(0, size);
            y = Random.Range(0, size);
        } while (_cells[x, y].isMine);

        return (x, y);
    }

    private void CalculateMinesAroundCells()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                SetNumberOfMinesAround(x, y);
            }
        }
    }

    private void CalculateMinesAroundCellsDoOnCell() => DoAllCells(SetNumberOfMinesAround);
    
    private void DoAllCells(Action<int, int> doOnCell)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                doOnCell(x, y);
            }
        }
    }

    private void SetNumberOfMinesAround(int x, int y)
    {
        if (_cells[x, y].isMine) return;
        
        _cells[x, y].minesAround = CountMinesAroundCell(x, y);
    }

    private int CountMinesAroundCell(int x, int y)
    {
        int count = 0;

        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && i < size && j >= 0 && j < size && _cells[i, j].isMine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public void ShowGameOver()
    {
        gameOverUI.ShowGameOverUI();
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RevealAllCells()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (!_cells[x, y].isRevealed)
                {
                    _cells[x, y].StartCoroutine(_cells[x, y].RevealCell());
                }
            }
        }
    }

    public void CheckWinCondition()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)    
            {
                if (!_cells[x, y].isMine && !_cells[x, y].isRevealed)
                {
                    // There is at least one non-mine cell that hasn't been revealed, so the player hasn't won yet.
                    return;
                }
            }
        }

        // If we get to this point, that means all non-mine cells have been revealed. The player has won!
        gameOverUI.ShowVictoryUI();
    }

    public List<Cell> GetNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();

        for (int i = cell.x - 1; i <= cell.x + 1; i++)
        {
            for (int j = cell.y - 1; j <= cell.y + 1; j++)
            {
                if (i >= 0 && i < size && j >= 0 && j < size)
                {
                    neighbors.Add(_cells[i, j]);
                }
            }
        }

        return neighbors;
    }

    public void PlaceMines(Cell firstCell)
    {
        int placedMines = 0;
        while (placedMines < mineCount)
        {
            int x = Random.Range(0, size);
            int y = Random.Range(0, size);
            // We shouldn't place a mine in the first cell that was revealed
            if (!(firstCell.x == x && firstCell.y == y) && !_cells[x, y].isMine)
            {
                _cells[x, y].isMine = true;
                placedMines++;
            }
        }

        CalculateMinesAroundCells();
    }
}
