using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


    private IEnumerator DropElementsAndFillCoroutine()
    {
        var width = gridCreator.puzzleSettings.width;
        var height = gridCreator.puzzleSettings.height;
        Sequence mainSequence = DOTween.Sequence();

        //tüm satırları kontrol et
        for (int col = 0; col < width; col++)
        {
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
                ZeroNumbers[i].number = EventManager.GetPuzzleSettings()
                    .possibleNumbers[Random.Range(0, EventManager.GetPuzzleSettings().possibleNumbers.Count)];
                ZeroNumbers[i].UpdateText();
            }

            //sıfır olanları sıfır olmayanların üstüne taşı bu sayede sıralı bir şekilde yerleşmiş olacaklar
            nonZeroNumbers.AddRange(ZeroNumbers);

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


        yield return StartCoroutine(UpdateMatrixAfterChangeCoroutine());
    }

    private IEnumerator UpdateMatrixAfterChangeCoroutine(bool isClick = false, int row = 0, int col = 0)
    {
        var width = gridCreator.puzzleSettings.width;
        var height = gridCreator.puzzleSettings.height;
        GameManager.Instance.gameState = GameStates.Drop;

        List<(int, int)> indexesToClear = new List<(int, int)>();

        if (isClick)
        {
            // tıklanan cellin etrafını kontrol et
            CheckForBlast(row, col, width, indexesToClear, height);
        }
        else
        {
            // bütün matrixi kontrol et
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    CheckForBlast(i, j, width, indexesToClear, height);
                }
            }
        }


        Sequence sequence = DOTween.Sequence();

        // eşleşenlern animasyonunu oynat
        foreach (var index in indexesToClear)
        {
            GameManager.Instance.ColorPopped(matrix[index.Item1, index.Item2].number);
            matrix[index.Item1, index.Item2].number = 0;
            matrix[index.Item1, index.Item2].numberText.text = "";
            sequence.Join(matrix[index.Item1, index.Item2].Animation());
        }

        yield return sequence.WaitForCompletion();

        // match varsa düşür ve doldur
        if (indexesToClear.Count > 0)
        {
            yield return StartCoroutine(DropElementsAndFillCoroutine());
        }
        else
        {
            GameManager.Instance.gameState = GameStates.Idle;
        }
    }

    private void CheckForBlast(int i, int j, int width, List<(int, int)> indexesToClear, int height)
    {
        int value = matrix[i, j].number;

        // Check horizontal
        List<(int, int)> horizontalIndexes = new List<(int, int)>();
        for (int k = j - 1; k >= 0 && matrix[i, k].number == value; k--)
        {
            horizontalIndexes.Add((i, k));
        }

        for (int k = j + 1; k < width && matrix[i, k].number == value; k++)
        {
            horizontalIndexes.Add((i, k));
        }

        if (horizontalIndexes.Count >= 2)
        {
            horizontalIndexes.Add((i, j));
            indexesToClear.AddRange(horizontalIndexes);
        }

        // Check vertical
        List<(int, int)> verticalIndexes = new List<(int, int)>();
        for (int k = i - 1; k >= 0 && matrix[k, j].number == value; k--)
        {
            verticalIndexes.Add((k, j));
        }

        for (int k = i + 1; k < height && matrix[k, j].number == value; k++)
        {
            verticalIndexes.Add((k, j));
        }

        if (verticalIndexes.Count >= 2)
        {
            verticalIndexes.Add((i, j));
            indexesToClear.AddRange(verticalIndexes);
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
        matrix = gridCreator.matrix;

        StartCoroutine(UpdateMatrixAfterChangeCoroutine(true, (int)obj.cellIndex.y, (int)obj.cellIndex.x));
    }
}