namespace InovaSaude.Blazor.Services;

public class ToastService
{
    private static object? _toastComponent;
    private static Action<string>? _showSuccess;
    private static Action<string>? _showError;
    private static Action<string>? _showWarning;
    private static Action<string>? _showInfo;

    public static void Initialize(object toastComponent, 
        Action<string> showSuccess,
        Action<string> showError, 
      Action<string> showWarning,
        Action<string> showInfo)
    {
        _toastComponent = toastComponent;
        _showSuccess = showSuccess;
        _showError = showError;
        _showWarning = showWarning;
     _showInfo = showInfo;
    }

    public static void ShowSuccess(string message)
    {
        _showSuccess?.Invoke(message);
 }

    public static void ShowError(string message)
    {
     _showError?.Invoke(message);
    }

    public static void ShowWarning(string message)
  {
        _showWarning?.Invoke(message);
    }

    public static void ShowInfo(string message)
    {
     _showInfo?.Invoke(message);
    }
}
