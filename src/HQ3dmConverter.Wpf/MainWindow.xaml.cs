using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using HQ3dmConverter.Core;

namespace HQ3dmConverter.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly ThreeDmConverter _converter = new();
        private string? _selectedInputPath;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseInputButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Rhino 8+ .3dm File",
                Filter = "Rhino 3DM Files (*.3dm)|*.3dm|All Files (*.*)|*.*",
                CheckFileExists = true
            };

            if (dialog.ShowDialog() == true)
            {
                _selectedInputPath = dialog.FileName;
                InputPathTextBox.Text = _selectedInputPath;
                ConvertButton.IsEnabled = true;
                SetResults("File selected. Ready to convert.", false);
            }
        }

        private void BrowseOutputButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save Converted Rhino 7 File",
                Filter = "Rhino 3DM Files (*.3dm)|*.3dm|All Files (*.*)|*.*",
                DefaultExt = ".3dm",
                AddExtension = true
            };

            if (!string.IsNullOrEmpty(_selectedInputPath))
            {
                var dir = Path.GetDirectoryName(_selectedInputPath)!;
                var stem = Path.GetFileNameWithoutExtension(_selectedInputPath);
                dialog.InitialDirectory = dir;
                dialog.FileName = $"{stem}_R7.3dm";
            }

            if (dialog.ShowDialog() == true)
            {
                OutputPathTextBox.Text = dialog.FileName;
            }
        }

        private async void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedInputPath))
                return;

            var outputPath = string.IsNullOrWhiteSpace(OutputPathTextBox.Text) 
                ? null 
                : OutputPathTextBox.Text.Trim();

            ConvertButton.IsEnabled = false;
            BrowseInputButton.IsEnabled = false;
            BrowseOutputButton.IsEnabled = false;
            ProgressBar.Visibility = Visibility.Visible;
            SetResults("Converting...", false);

            try
            {
                var result = await Task.Run(() => _converter.ConvertToRhino7(_selectedInputPath, outputPath));

                if (result.Status == ConversionStatus.Success)
                {
                    SetResults($"✅ Conversion successful!\n\nOutput: {result.OutputPath}\n\nSource: v{result.SourceSummary?.ArchiveVersion / 10} → Output: v{result.OutputSummary?.ArchiveVersion / 10}\nObjects: {result.SourceSummary?.ObjectCount} → {result.OutputSummary?.ObjectCount}\nLayers: {result.SourceSummary?.LayerCount} → {result.OutputSummary?.LayerCount}", false);
                }
                else if (result.Status == ConversionStatus.SuccessWithWarnings)
                {
                    var warnings = string.Join("\n", result.Warnings);
                    SetResults($"⚠️ Conversion succeeded with warnings:\n\n{warnings}\n\nOutput: {result.OutputPath}", false);
                }
                else if (result.Status == ConversionStatus.Skipped)
                {
                    SetResults($"ℹ️ Skipped: {result.Warnings.FirstOrDefault()}", false);
                }
                else
                {
                    var errors = string.Join("\n", result.Errors);
                    SetResults($"❌ Conversion failed:\n\n{errors}", true);
                }
            }
            catch (Exception ex)
            {
                SetResults($"❌ Unexpected error:\n\n{ex.Message}", true);
            }
            finally
            {
                ConvertButton.IsEnabled = true;
                BrowseInputButton.IsEnabled = true;
                BrowseOutputButton.IsEnabled = true;
                ProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void SetResults(string text, bool isError)
        {
            ResultsTextBlock.Text = text;
            ResultsTextBlock.Foreground = isError ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xCC, 0x00, 0x00)) 
                                                   : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x33, 0x33, 0x33));
        }
    }
}