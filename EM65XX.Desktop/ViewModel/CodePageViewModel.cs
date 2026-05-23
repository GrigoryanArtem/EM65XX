using CommunityToolkit.Mvvm.ComponentModel;

namespace EM65XX.Desktop.ViewModel;

public class CodePageViewModel : ObservableObject
{
    public ObservableRam Ram { get; } = new();
}
