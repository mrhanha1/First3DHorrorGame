using UnityEngine;

public class Sudoku3DCube : MonoBehaviour
{
    private SudokuCell cell;
    private Sudoku3DManager manager;
    private Renderer cubeRenderer;

    public void Initialize(SudokuCell cellData, Sudoku3DManager gridManager)
    {
        cell = cellData;
        manager = gridManager;
        cubeRenderer = GetComponent<Renderer>();
    }

    public void SetMaterial(Material material)
    {
        if (cubeRenderer != null && material != null)
        {
            cubeRenderer.material = material;
        }
    }

    public SudokuCell GetCell() => cell;
}