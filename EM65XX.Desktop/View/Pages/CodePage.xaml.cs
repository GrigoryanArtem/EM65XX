using EM65XX.Desktop.ViewModel;
using System.Windows.Controls;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.IO;

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
        
        using var reader = new XmlTextReader("6502.xshd");

        _textEditor.SyntaxHighlighting =
            HighlightingLoader.Load(
                reader,
                HighlightingManager.Instance);
    }
}
