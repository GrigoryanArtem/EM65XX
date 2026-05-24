using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static EM65XX.Desktop.ViewModel.ObservableRam;

namespace EM65XX.Desktop.AttachedBehaviors;

public static class MemoryGridBehavior
{
    public static readonly DependencyProperty GenerateMemoryColumnsProperty =
        DependencyProperty.RegisterAttached(
            "GenerateMemoryColumns",
            typeof(bool),
            typeof(MemoryGridBehavior),
            new PropertyMetadata(false, OnGenerateMemoryColumnsChanged));

    public static void SetGenerateMemoryColumns(
        DependencyObject element,
        bool value)
    {
        element.SetValue(GenerateMemoryColumnsProperty, value);
    }

    public static bool GetGenerateMemoryColumns(
        DependencyObject element)
    {
        return (bool)element.GetValue(GenerateMemoryColumnsProperty);
    }

    private static void OnGenerateMemoryColumnsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not DataGrid grid)
            return;

        if ((bool)e.NewValue == false)
            return;

        GenerateColumns(grid);
    }

    private static void GenerateColumns(DataGrid grid)
    {
        grid.Columns.Clear();
        
        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "",

            Binding = new Binding(nameof(MemoryRow.Address))
            {
                StringFormat = "X4"
            },

            IsReadOnly = true
        });

        for (int i = 0; i < 16; i++)
        {
            grid.Columns.Add(new DataGridTextColumn
            {
                Header = i.ToString("X2"),
                FontFamily = new FontFamily("Consolas"),
                
                Binding = new Binding($"[{i}].Value")
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
                    StringFormat = "X2"
                }
            });
        }
    }
}