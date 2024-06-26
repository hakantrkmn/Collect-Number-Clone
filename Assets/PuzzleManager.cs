using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class PuzzleManager : MonoBehaviour
{
    
    GridCreator gridCreator;
    
    private Cell[,] matrix;


    private void Start()
    {
        GenerateNumbers();
    }

    [Button]
    public void GenerateNumbers()
    {
        gridCreator = GetComponent<GridCreator>();
        gridCreator.CreateGrid();
        matrix = new Cell[gridCreator.width, gridCreator.height];

        for (int i = 0; i < gridCreator.height; i++)
        {
            for (int j = 0; j < gridCreator.width; j++)
            {
                matrix[i, j] = gridCreator.cells[i * gridCreator.width + j];
                gridCreator.cells[i * gridCreator.width + j].cellIndex = new Vector2(i, j);
            }
        }
        GenerateMatrix();
        PrintMatrix();
        
        for (int j = 0; j < gridCreator.height; j++)
        {
            for (int i = 0; i < gridCreator.width; i++)
            {
                gridCreator.cells[i * gridCreator.width + j].number = matrix[i, j].number;
                gridCreator.cells[i * gridCreator.width + j].numberText.text = matrix[i, j].ToString();
            }
        }
        
        UpdateCellNumbers();
       
    }

    void DropElementsAndFill()
    {
        var changedCells = new List<Cell>();
        // Her sütun için işlem yap
        for (int col = 0; col < gridCreator.width; col++)
        {
            int writeIndex = 0; // En üst satırdan başla

            // Yukarıdan aşağıya doğru ilerle
            for (int row = 0; row < gridCreator.height; row++)
            {
                if (matrix[row, col].number != 0)
                {
                    // Sıfır olmayan elemanı yukarı taşı
                    matrix[writeIndex, col].number = matrix[row, col].number;
                    matrix[writeIndex, col].numberText.text = matrix[row, col].number.ToString();
                    writeIndex++;
                }
            }

            // Kalan boş hücreleri -1 ile doldur
            while (writeIndex < gridCreator.height)
            {
                matrix[writeIndex, col].number = Random.Range(1,5);
                matrix[writeIndex, col].numberText.text = matrix[writeIndex, col].number.ToString();
                changedCells.Add(matrix[writeIndex, col]);
                writeIndex++;
            }
        }

        foreach (var cell in changedCells)
        {
            UpdateMatrixAfterChange((int)cell.cellIndex.x, (int)cell.cellIndex.y);
        }
        
    }

    public void UpdateCellNumbers()
    {
        for (int j = 0; j < gridCreator.height; j++)
        {
            for (int i = 0; i < gridCreator.width; i++)
            {
                gridCreator.cells[i * gridCreator.width + j].number = matrix[i, j].number;
                gridCreator.cells[i * gridCreator.width + j].numberText.text = matrix[i, j].number.ToString();
            }
        }
    }

    private void OnEnable()
    {
        EventManager.CellClicked += OnCellClicked;
    }

    private void OnDisable()
    {
        EventManager.CellClicked -= OnCellClicked;
    }

    private void OnCellClicked(Cell obj)
    {
        for (int i = 0; i < gridCreator.height; i++)
        {
            for (int j = 0; j < gridCreator.width; j++)
            {
                if (matrix[i,j] == obj)
                {
                    UpdateMatrixAfterChange(i,j);
                }
            }
        }
        //DropElementsAndFill();

       // UpdateCellNumbers();
    }

    void UpdateMatrixAfterChange(int row, int col)
    {
        int value = matrix[row, col].number;

        // List to keep track of indexes to be set to 0
        List<(int, int)> indexesToClear = new List<(int, int)>();

        // Check horizontal
        List<(int, int)> horizontalIndexes = new List<(int, int)>();
        for (int k = col - 1; k >= 0 && matrix[row, k].number == value; k--)
        {
            horizontalIndexes.Add((row, k));
        }
        for (int k = col + 1; k < gridCreator.width && matrix[row, k].number == value; k++)
        {
            horizontalIndexes.Add((row, k));
        }
        if (horizontalIndexes.Count >= 2)
        {
            horizontalIndexes.Add((row, col)); // Include the changed cell itself
            indexesToClear.AddRange(horizontalIndexes);
        }

        // Check vertical
        List<(int, int)> verticalIndexes = new List<(int, int)>();
        for (int k = row - 1; k >= 0 && matrix[k, col].number == value; k--)
        {
            verticalIndexes.Add((k, col));
        }
        for (int k = row + 1; k < gridCreator.height && matrix[k, col].number == value; k++)
        {
            verticalIndexes.Add((k, col));
        }
        if (verticalIndexes.Count >= 2)
        {
            verticalIndexes.Add((row, col)); // Include the changed cell itself
            indexesToClear.AddRange(verticalIndexes);
        }

        if (indexesToClear.Count!=0)
        {
            Debug.Log("oldu");

        }
        // Set the collected indexes to 0
        foreach (var index in indexesToClear)
        {
            DOVirtual.Int(matrix[index.Item1, index.Item2].number, 0, 2, x =>
            {
                matrix[index.Item1, index.Item2].number = x;
                matrix[index.Item1, index.Item2].numberText.text = x.ToString();
            }).OnComplete(() =>
            {
                matrix[index.Item1, index.Item2].number = 0;
    
                if (index == indexesToClear.Last())
                {
                    Debug.Log("hakan");
                    DropElementsAndFill();
                }
            });
        }

    }


    void GenerateMatrix()
    {
        System.Random rand = new System.Random();
        
        for (int i = 0; i < gridCreator.height; i++)
        {
            for (int j = 0; j < gridCreator.width; j++)
            {
                List<int> possibleValues = new List<int> { 1, 2, 3, 4 };
                bool validValueFound = false;

                while (!validValueFound && possibleValues.Count > 0)
                {
                    int value = possibleValues[rand.Next(possibleValues.Count)];

                    if (IsValueValid(i, j, value))
                    {
                        matrix[i, j].number = value;
                        validValueFound = true;
                    }
                    else
                    {
                        possibleValues.Remove(value);
                    }
                }

                // If no valid value is found (which shouldn't happen with the given constraints), fill with a default value
                if (!validValueFound)
                {
                    matrix[i, j].number = 1; // or any default value
                }
            }
        }
    }

    bool IsValueValid(int row, int col, int value)
    {
        // Check horizontal
        int horizontalCount = 1;
        for (int k = col - 1; k >= 0 && matrix[row, k].number == value; k--)
        {
            horizontalCount++;
        }
        for (int k = col + 1; k < gridCreator.height && matrix[row, k].number == value; k++)
        {
            horizontalCount++;
        }
        if (horizontalCount >= 3) return false;

        // Check vertical
        int verticalCount = 1;
        for (int k = row - 1; k >= 0 && matrix[k, col].number == value; k--)
        {
            verticalCount++;
        }
        for (int k = row + 1; k < gridCreator.width && matrix[k, col].number == value; k++)
        {
            verticalCount++;
        }
        if (verticalCount >= 3) return false;

        return true;
    }

    void PrintMatrix()
    {
        for (int i = 0; i < gridCreator.height; i++)
        {
            string rowString = "";
            for (int j = 0; j < gridCreator.height; j++)
            {
                rowString += matrix[i, j] + " ";
            }
            Debug.Log(rowString);
        }
    }
}
