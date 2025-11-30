using UnityEngine;
using DG.Tweening;

public class Sudoku3DManager : MonoBehaviour
{
    [Header("Prefab & Container")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Transform gridContainer;

    [Header("Grid Settings")]
    [SerializeField] private float cubeSpacing = 1.2f;
    [SerializeField] private float heightMultiplier = 0.5f; // M?i giá tr? = 0.5 ??n v? chi?u cao
    [SerializeField] private Vector3 gridOffset = Vector3.zero;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private Ease animationEase = Ease.OutBack;

    [Header("Visual Settings")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material fixedMaterial;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material errorMaterial;

    [Header("Subgrid Lines")]
    [SerializeField] private float subgridGap = 0.3f; // Kho?ng cách thêm gi?a các subgrid 3x3

    private Sudoku3DCube[,] cubes;
    private SudokuGrid sudokuGrid;
    private int selectedRow = -1;
    private int selectedCol = -1;

    private void Awake()
    {
        if (gridContainer == null)
            gridContainer = transform;
    }

    /// <summary>
    /// Kh?i t?o grid 3D t? SudokuGrid
    /// </summary>
    public void InitializeGrid(SudokuGrid grid)
    {
        sudokuGrid = grid;
        ClearGrid();
        GenerateCubes();
    }

    /// <summary>
    /// Xóa grid hi?n t?i
    /// </summary>
    private void ClearGrid()
    {
        if (gridContainer == null) return;

        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }

        cubes = null;
    }

    /// <summary>
    /// Sinh ra các cube cho grid 9x9
    /// </summary>
    private void GenerateCubes()
    {
        if (cubePrefab == null || sudokuGrid == null)
        {
            Debug.LogError("[Sudoku 3d manager] Sudoku cube prefab not found");
            return;
        }

        cubes = new Sudoku3DCube[SudokuGrid.GRID_SIZE, SudokuGrid.GRID_SIZE];
        Debug.Log("[Sudoku 3D Manager] Start generate cubes");
        for (int row = 0; row < SudokuGrid.GRID_SIZE; row++)
        {
            for (int col = 0; col < SudokuGrid.GRID_SIZE; col++)
            {
                Vector3 position = CalculatePosition(row, col);
                GameObject cubeObj = Instantiate(cubePrefab, gridContainer);
                cubeObj.transform.localPosition = position;
                cubeObj.name = $"Cube_{row}_{col}";

                Sudoku3DCube cube = cubeObj.GetComponent<Sudoku3DCube>();
                if (cube == null)
                    cube = cubeObj.AddComponent<Sudoku3DCube>();

                SudokuCell cell = sudokuGrid.GetCell(row, col);
                cube.Initialize(cell, this);

                // Set material
                SetCubeMaterial(cube, cell);

                cubes[row, col] = cube;

                // Animate vào t? d??i lên
                AnimateCubeSpawn(cube, 0.05f * (row + col));
                Debug.Log($"[Sudoku 3D Manager] Generated cube at ({row}, {col}) with value {cell.value}");
            }
        }
        Debug.Log("[Sudoku 3D Manager] Completed generate cubes");
    }

    /// <summary>
    /// Tính toán v? trí 3D c?a cube
    /// Có kho?ng cách thêm gi?a các subgrid 3x3
    /// </summary>
    private Vector3 CalculatePosition(int row, int col)
    {
        float x = col * cubeSpacing;
        float z = row * cubeSpacing;

        // Thêm kho?ng cách cho subgrid
        x += (col / 3) * subgridGap;
        z += (row / 3) * subgridGap;

        // Center grid
        float gridWidth = (SudokuGrid.GRID_SIZE - 1) * cubeSpacing + 2 * subgridGap;
        x -= gridWidth / 2f;
        z -= gridWidth / 2f;

        SudokuCell cell = sudokuGrid.GetCell(row, col);
        float y = CalculateHeight(cell.value);

        return new Vector3(x, y, z) + gridOffset;
    }

    /// <summary>
    /// Tính toán chi?u cao d?a trên giá tr?
    /// </summary>
    private float CalculateHeight(int value)
    {
        return value * heightMultiplier;
    }

    /// <summary>
    /// C?p nh?t giá tr? và animate cube
    /// </summary>
    public void UpdateCubeValue(int row, int col)
    {
        if (cubes == null || cubes[row, col] == null) return;

        Sudoku3DCube cube = cubes[row, col];
        SudokuCell cell = sudokuGrid.GetCell(row, col);

        // Animate height change
        float targetHeight = CalculateHeight(cell.value);
        Vector3 currentPos = cube.transform.position;
        Vector3 targetPos = new Vector3(currentPos.x, targetHeight, currentPos.z);

        cube.transform.DOMove(targetPos, animationDuration)
            .SetEase(animationEase);

        // Update material
        SetCubeMaterial(cube, cell);

        // Pulse effect
        cube.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 5, 1f);
    }

    /// <summary>
    /// Set selection
    /// </summary>
    public void SetSelection(int row, int col)
    {
        // Deselect previous
        if (selectedRow >= 0 && selectedCol >= 0 && cubes != null)
        {
            var prevCube = cubes[selectedRow, selectedCol];
            if (prevCube != null)
            {
                var cell = sudokuGrid.GetCell(selectedRow, selectedCol);
                SetCubeMaterial(prevCube, cell);
            }
        }

        selectedRow = row;
        selectedCol = col;

        // Select new
        if (row >= 0 && col >= 0 && cubes != null)
        {
            var cube = cubes[row, col];
            if (cube != null && selectedMaterial != null)
            {
                cube.SetMaterial(selectedMaterial);

                // Bounce effect
                cube.transform.DOPunchPosition(Vector3.up * 0.3f, 0.3f, 5, 1f);
            }
        }
    }

    /// <summary>
    /// ??t material cho cube d?a trên tr?ng thái
    /// </summary>
    private void SetCubeMaterial(Sudoku3DCube cube, SudokuCell cell)
    {
        if (cube == null || cell == null) return;

        if (cell.row == selectedRow && cell.col == selectedCol && selectedMaterial != null)
        {
            cube.SetMaterial(selectedMaterial);
        }
        else if (!cell.IsEmpty && !cell.IsValid && errorMaterial != null)
        {
            cube.SetMaterial(errorMaterial);
        }
        else if (cell.isFixed && fixedMaterial != null)
        {
            cube.SetMaterial(fixedMaterial);
        }
        else if (defaultMaterial != null)
        {
            cube.SetMaterial(defaultMaterial);
        }
    }

    /// <summary>
    /// Animation khi spawn cube
    /// </summary>
    private void AnimateCubeSpawn(Sudoku3DCube cube, float delay)
    {
        Vector3 originalPos = cube.transform.position;
        cube.transform.position = new Vector3(originalPos.x, -5f, originalPos.z);
        cube.transform.localScale = Vector3.zero;

        cube.transform.DOMove(originalPos, 0.5f)
            .SetDelay(delay)
            .SetEase(Ease.OutBounce);

        cube.transform.DOScale(Vector3.one, 0.5f)
            .SetDelay(delay)
            .SetEase(Ease.OutBack);
    }

    /// <summary>
    /// Refresh toàn b? grid
    /// </summary>
    public void RefreshGrid()
    {
        if (cubes == null || sudokuGrid == null) return;

        for (int row = 0; row < SudokuGrid.GRID_SIZE; row++)
        {
            for (int col = 0; col < SudokuGrid.GRID_SIZE; col++)
            {
                UpdateCubeValue(row, col);
            }
        }
    }

    /// <summary>
    /// Hi?u ?ng khi hoàn thành puzzle
    /// </summary>
    public void PlayCompleteAnimation()
    {
        if (cubes == null) return;

        for (int row = 0; row < SudokuGrid.GRID_SIZE; row++)
        {
            for (int col = 0; col < SudokuGrid.GRID_SIZE; col++)
            {
                float delay = (row + col) * 0.05f;
                cubes[row, col]?.transform.DOPunchScale(Vector3.one * 0.3f, 0.5f, 5, 1f)
                    .SetDelay(delay);
            }
        }
    }

    private void OnDestroy()
    {
        // Kill all tweens
        DOTween.Kill(transform);
        if (cubes != null)
        {
            foreach (var cube in cubes)
            {
                if (cube != null)
                    DOTween.Kill(cube.transform);
            }
        }
    }
}