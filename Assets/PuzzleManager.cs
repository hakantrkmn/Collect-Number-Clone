using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

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
        
        UpdateCellNumbers();
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

        // Set the collected indexes to 0
        foreach (var index in indexesToClear)
        {
            matrix[index.Item1, index.Item2].number = 0;
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
