using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class PuzzleManager : MonoBehaviour
{
    public GridCreator gridCreator;

    private List<Cell> nonZeroNumbers = new List<Cell>();
    private List<Cell> ZeroNumbers = new List<Cell>();

    private Cell[,] matrix;


    private void Start()
    {
        matrix = gridCreator.matrix;
    }


    private IEnumerator DropElementsAndFillCoroutine()
    {
        var width = gridCreator.puzzleSettings.width;
        var height = gridCreator.puzzleSettings.height;
        var changedCells = new List<Cell>();
        Sequence mainSequence = DOTween.Sequence();

        //tüm satırları kontrol et
        for (int col = 0; col < width; col++)
        {
            bool hasZero = false;
            for (int row = 0; row < height; row++)
            {
                if (matrix[row, col].number == 0)
                {
                    hasZero = true;
                    break;
                }
            }
            //sıfır yoksa devam et
            if (!hasZero) continue;
            
            nonZeroNumbers.Clear();
            ZeroNumbers.Clear();

            //sıfır olmayanları ve sıfırları ayır
            for (int row = height - 1; row >= 0; row--)
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
            //gidecekleri pozisyonları listede sakla
            List<Vector3> gridPositions = new List<Vector3>();

            for (int row = height - 1; row >= 0; row--)
            {
                gridPositions.Add(matrix[row, col].transform.position);
            }

            //sıfır olan cellleri ekranın üstüne taşı ve yeni değerlerini ata
            for (int i = 0; i < ZeroNumbers.Count; i++)
            {
                ZeroNumbers[i].transform.position = new Vector3(ZeroNumbers[i].transform.position.x,
                    gridCreator.topPoint.position.y + (i * gridCreator.cellGap), ZeroNumbers[i].transform.position.z);
                ZeroNumbers[i].number = Random.Range(1, 5);
                ZeroNumbers[i].UpdateText();
            }

            //sıfır olanları sıfır olmayanların üstüne taşı bu sayede sıralı bir şekilde yerleşmiş olacaklar
            nonZeroNumbers.AddRange(ZeroNumbers);
            changedCells.AddRange(nonZeroNumbers);

            Sequence columnSequence = DOTween.Sequence();

            //tüm celleri yeni pozisyonlarına taşı
            for (int i = 0; i < nonZeroNumbers.Count; i++)
            {
                if (nonZeroNumbers[i].transform.position != gridPositions[i])
                {
                    columnSequence.Join(nonZeroNumbers[i].transform.DOMove(gridPositions[i],
                        Vector3.Distance(gridPositions[i], nonZeroNumbers[i].transform.position) / 800));
                }
            }

            mainSequence.Join(columnSequence);

            //matrixi güncelle
            for (int j = 0; j < nonZeroNumbers.Count; j++)
            {
                matrix[j, col] = nonZeroNumbers[nonZeroNumbers.Count - 1 - j];
                matrix[j, col].cellIndex = new Vector2(col, j);
            }
        }

        yield return mainSequence.WaitForCompletion();

        //değişen celler için kontrol yap
        foreach (var cell in changedCells)
        {
            yield return StartCoroutine(UpdateMatrixAfterChangeCoroutine((int)cell.cellIndex.y, (int)cell.cellIndex.x));
        }

        changedCells.Clear();
    }

    private IEnumerator UpdateMatrixAfterChangeCoroutine(int row, int col)
    {
        var width = gridCreator.puzzleSettings.width;
        var height = gridCreator.puzzleSettings.height;
        GameManager.Instance.gameState = GameStates.Drop;

        int value = matrix[row, col].number;
        List<(int, int)> indexesToClear = new List<(int, int)>();

        // Check horizontal
        List<(int, int)> horizontalIndexes = new List<(int, int)>();
        for (int k = col - 1; k >= 0 && matrix[row, k].number == value; k--)
        {
            horizontalIndexes.Add((row, k));
        }

        for (int k = col + 1; k < width && matrix[row, k].number == value; k++)
        {
            horizontalIndexes.Add((row, k));
        }

        if (horizontalIndexes.Count >= 2)
        {
            horizontalIndexes.Add((row, col));
            indexesToClear.AddRange(horizontalIndexes);
        }

        // Check vertical
        List<(int, int)> verticalIndexes = new List<(int, int)>();
        for (int k = row - 1; k >= 0 && matrix[k, col].number == value; k--)
        {
            verticalIndexes.Add((k, col));
        }

        for (int k = row + 1; k < height && matrix[k, col].number == value; k++)
        {
            verticalIndexes.Add((k, col));
        }

        if (verticalIndexes.Count >= 2)
        {
            verticalIndexes.Add((row, col));
            indexesToClear.AddRange(verticalIndexes);
        }

        Sequence sequence = DOTween.Sequence();

        //koşul sağlandıysa animasyonu oynat 
        foreach (var index in indexesToClear)
        {
            GameManager.Instance.ColorPopped(matrix[index.Item1, index.Item2].number);
            matrix[index.Item1, index.Item2].number = 0;
            matrix[index.Item1, index.Item2].numberText.text = "";
            sequence.Join(matrix[index.Item1, index.Item2].Animation());
        }

        yield return sequence.WaitForCompletion();

        //eğer koşul sağlandıysa düşme işlemini başlat
        if (indexesToClear.Count > 0)
        {
            yield return StartCoroutine(DropElementsAndFillCoroutine());
        }
        else
        {
            GameManager.Instance.gameState = GameStates.Idle;
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
        StartCoroutine(UpdateMatrixAfterChangeCoroutine((int)obj.cellIndex.y, (int)obj.cellIndex.x));
    }
}