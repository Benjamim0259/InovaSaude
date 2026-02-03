namespace InovaSaude.Blazor.Services;

public class ToastService
{
    private static ToastNotification? _toastComponent;

    public static void Initialize(ToastNotification toastComponent)
    {
        _toastComponent = toastComponent;
    }

    public static void ShowSuccess(string message)
    {
        _toastComponent?.ShowSuccess(message);
 }

    public static void ShowError(string message)
    {
     _toastComponent?.ShowError(message);
    }

    public static void ShowWarning(string message)
  {
        _toastComponent?.ShowWarning(message);
    }

    public static void ShowInfo(string message)
    {
     _toastComponent?.ShowInfo(message);
    }
}
