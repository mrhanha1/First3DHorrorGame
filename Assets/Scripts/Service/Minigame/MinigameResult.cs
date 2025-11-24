public class MinigameResult
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; }
    public MinigameResult(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }
}