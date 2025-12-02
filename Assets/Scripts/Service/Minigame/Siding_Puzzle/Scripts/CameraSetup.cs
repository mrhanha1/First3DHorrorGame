using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    public Transform puzzleBoard;
    
    
    public float distance = 10f;
    
   
    public float viewAngle = 15f;
    
    public bool autoSetupOnStart = true;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupCamera();
        }
    }
    
    void SetupCamera()
    {
        if (puzzleBoard == null)
        {
            GameObject board = GameObject.Find("PuzzleBoard");
            if (board != null)
            {
                puzzleBoard = board.transform;
            }
        }
        
        if (puzzleBoard == null)
        {
            Debug.LogWarning("Không tìm thấy PuzzleBoard!");
            return;
        }
        
        // Tính vị trí camera
        float angleRad = viewAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            0f,
            Mathf.Sin(angleRad) * distance,
            Mathf.Cos(angleRad) * distance
        );
        
        transform.position = puzzleBoard.position + offset;
        transform.LookAt(puzzleBoard.position);
        
        Debug.Log("✓ Camera đã được setup!");
    }
    
    [ContextMenu("Setup Camera")]
    public void ManualSetup()
    {
        SetupCamera();
    }
}