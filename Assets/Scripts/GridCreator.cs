using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class GridCreator : MonoBehaviour
{
    public Cell[,] matrix;
    
    public List<FixedValue> matrixValues;
    public List<int> possibleNumbers;
    
    public GameObject cellPrefab;
    public int width;
    public int height;
    public float spacing;

    public Transform topPoint;
    
    public List<Cell> cells;

    [HideInInspector]public float cellGap;

    private void Start()
    {
        matrix = new Cell[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                matrix[i, j] = cells[i * width + j];
                cells[i * width + j].cellIndex = new Vector2(j, i);
            }
        }
    }

    void CreateGrid()
    {
        ClearGrid();
        var rectTransform = GetComponent<RectTransform>();
        var cellSize = Mathf.Min(
            (rectTransform.rect.width - (width - 1) * spacing - 2 * spacing) / width,
            (rectTransform.rect.height - (height - 1) * spacing - 2 * spacing) / height
        );
        var totalWidth = (cellSize + spacing) * width - spacing;
        var totalHeight = (cellSize + spacing) * height - spacing;
        var startX = -totalWidth / 2 + cellSize / 2;
        var startY = totalHeight / 2 - cellSize / 2;
        
        topPoint.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        topPoint.position += new Vector3(0, cellSize/2, 0);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 gridPosition = new Vector3(startX + x * (cellSize + spacing),
                    startY + y * -(cellSize + spacing), 0);

                var grid =  PrefabUtility.InstantiatePrefab(cellPrefab).GameObject();
                grid.transform.SetParent(transform);
                grid.transform.localPosition = gridPosition;
                grid.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize, cellSize);

                cells.Add(grid.GetComponent<Cell>());
            }
        }
        
        cellGap = cells[0].transform.position.y- cells[width].transform.position.y;
    }
    
    

    private void ClearGrid()
    {
        foreach (var grid in cells)
        {
            DestroyImmediate(grid.gameObject);
        }
        
        cells.Clear();
    }
    
    [Button]
    void GenerateGrid()
    {
        CreateGrid();
        
        matrix = new Cell[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Debug.Log(i * width + j);

                matrix[i, j] = cells[i * width + j];
                cells[i * width + j].cellIndex = new Vector2(j, i);
            }
        }
        
        System.Random rand = new System.Random();

        // Sabit değerleri doldur
        foreach (var fixedValue in matrixValues)
        {
            int row = fixedValue.row;
            int col = fixedValue.col;
            int value = fixedValue.value;

            if (row >= 0 && row < height && col >= 0 && col < width)
            {
                matrix[row, col].number = value;
                matrix[row, col].UpdateText();
            }
        }

        // Sabit değerler dışındaki hücreleri doldur
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (matrix[i, j].number != 0) // Sabit değerleri atla
                {
                    continue;
                }

                List<int> possibleValues = new List<int>(possibleNumbers);
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

                if (!validValueFound)
                {
                    matrix[i, j].number = 1;
                    matrix[i, j].UpdateText();
                }
            }
        }
    }

    private bool IsValueValid(int row, int col, int value)
    {
        // Yatay kontrol
        int sameCount = 0;
        for (int i = 0; i < width; i++)
        {
            if (matrix[row, i].number == value)
            {
                sameCount++;
                if (sameCount >= 2)
                {
                    return false;
                }
            }
            else
            {
                sameCount = 0;
            }
        }

        // Dikey kontrol
        sameCount = 0;
        for (int i = 0; i < height; i++)
        {
            if (matrix[i, col].number == value)
            {
                sameCount++;
                if (sameCount >= 2)
                {
                    return false;
                }
            }
            else
            {
                sameCount = 0;
            }
        }

        return true;
    }
}