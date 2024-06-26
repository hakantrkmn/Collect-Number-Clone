using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class GridCreator : MonoBehaviour
{
    public GameObject cellPrefab;
    public int width;
    public int height;
    public float spacing;
    public List<GameObject> cells;

    [Button]
    public void CreateGrid()
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
        var startY = -totalHeight / 2 + cellSize / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 gridPosition = new Vector3(startX + x * (cellSize + spacing),
                    startY + y * (cellSize + spacing), 0);

                var grid = Instantiate(cellPrefab, gridPosition, Quaternion.identity, transform);
                grid.transform.localPosition = gridPosition;
                grid.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize, cellSize);

                cells.Add(grid);
            }
        }
    }

    private void ClearGrid()
    {
        foreach (GameObject grid in cells)
        {
            DestroyImmediate(grid);
        }
        
        cells.Clear();
    }
}