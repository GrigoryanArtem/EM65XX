using EM65XX.Desktop.ViewModel;
using System.Windows.Controls;

namespace EM65XX.Desktop.View.Pages;
/// <summary>
/// Interaction logic for CodePage.xaml
/// </summary>
public partial class CodePage : Page
{
    public CodePage()
    {
        InitializeComponent();
        DataContext = new CodePageViewModel();
    }
}
