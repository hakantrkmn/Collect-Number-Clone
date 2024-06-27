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

    private Sequence tween;

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
                gridCreator.cells[i * gridCreator.width + j].cellIndex = new Vector2(j, i);
            }
        }


        GenerateMatrix();
    }

    public List<Cell> nonZeroNumbers = new List<Cell>();
    public List<Cell> ZeroNumbers = new List<Cell>();

    void DropElementsAndFill()
    {
        tween = DOTween.Sequence();

        UnityEngine.Debug.Log(tween.IsActive());
        var changedCells = new List<Cell>();
        // Her sütun için işlem yap
        for (int col = 0; col < gridCreator.width; col++)
        {
            bool hasZero = false;
            for (int row = 0; row < gridCreator.height; row++)
            {
                if (matrix[row, col].number == 0)
                {
                    hasZero = true;
                    break;
                }
            }

            // Eğer sütunda sıfır yoksa, bu sütunu atla
            if (!hasZero) continue;
            
            UnityEngine.Debug.Log("sıfır var");


            nonZeroNumbers.Clear();
            ZeroNumbers.Clear();


            for (int row = gridCreator.height - 1; row >= 0; row--)
            {
                if (matrix[row, col].number != 0)
                {
                    nonZeroNumbers.Add(matrix[row, col]);
                }
                else
                {
                    ZeroNumbers.Add(matrix[row, col]);
                }
            }

            List<Vector3> gridPositions = new List<Vector3>();

            for (int row = gridCreator.height - 1; row >= 0; row--)
            {
                gridPositions.Add(matrix[row, col].transform.position);
            }


            for (int i = 0; i < ZeroNumbers.Count; i++)
            {
                ZeroNumbers[i].transform.position += new Vector3(0, 800, 0);
                ZeroNumbers[i].number = Random.Range(1, 5);
                ZeroNumbers[i].UpdateText();
            }

            nonZeroNumbers.AddRange(ZeroNumbers);
            changedCells.AddRange(nonZeroNumbers);

            for (int i = 0; i < nonZeroNumbers.Count; i++)
            {
                if (nonZeroNumbers[i].transform.position != gridPositions[i])
                {
                    tween.Join(nonZeroNumbers[i].transform.DOMove(gridPositions[i],
                        Vector3.Distance(gridPositions[i], nonZeroNumbers[i].transform.position) / 400));
                }
                
            }

            for (int j = 0; j < nonZeroNumbers.Count; j++)
            {
                matrix[j, col] = nonZeroNumbers[nonZeroNumbers.Count - 1 - j];
                matrix[j, col].cellIndex = new Vector2(col, j);
            }
        }


        foreach (var cell in changedCells)
        {
            tween.AppendCallback(()=>
            {
                UpdateMatrixAfterChange((int)cell.cellIndex.y, (int)cell.cellIndex.x);
            });
        }
        changedCells.Clear();

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
        UpdateMatrixAfterChange((int)obj.cellIndex.y, (int)obj.cellIndex.x);
    }

    void UpdateMatrixAfterChange(int row, int col)
    {
        tween = DOTween.Sequence();

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
            matrix[index.Item1, index.Item2].UpdateText();
            tween.Join(matrix[index.Item1, index.Item2].Animation());
        }

        //tween.AppendInterval(1);
        tween.AppendCallback(DropElementsAndFill);
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
                        matrix[i, j].UpdateText();
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
                    matrix[i, j].number = 1;
                    matrix[i, j].UpdateText();
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