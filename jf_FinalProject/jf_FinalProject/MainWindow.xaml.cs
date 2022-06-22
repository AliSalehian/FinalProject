using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Uwp.Notifications;

namespace jf_FinalProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int _numberOfSelectedFile = 0;
        private List<string> _selectedFilePath = new List<string>();
        private int _rtbIndex = 0;
        private string _darkBackGroundValue = "#292F34";
        private string _lightBackGroundValue = "#3A4149";
        private bool IsMenuOpen { get; set; }

        public bool IsManual { get; set; }

        public List<string> SelectedFilePath 
        { 
            get { return _selectedFilePath; }
            set { _selectedFilePath = value; }
        }
        public MainWindow()
        {
            InitializeComponent();
            IsMenuOpen = false;
            IsManual = true;
        }

        private void ShutDownButton_Click(object sender, EventArgs e) => Application.Current.Shutdown();

        private void ShutDownButton_MouseEnter(object sender, EventArgs e)
        {
            #if DEBUG
            shutdownBackgroundImage.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\shutdown_mo.png")));
            #else
            shutdownBackgroundImage.Source = new BitmapImage(new Uri(Path.GetFullPath(@"Assets\shutdown_mo.png")));
            #endif
        }

        private void ShutDownButton_MouseLeave(object sender, EventArgs e)
        {
            #if DEBUG
            shutdownBackgroundImage.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\shutdown_def.png")));
            #else
            shutdownBackgroundImage.Source = new BitmapImage(new Uri(Path.GetFullPath(@"Assets\shutdown_def.png")));
            #endif
        }

        private void MaxMinButton_MouseEnter(object sender, EventArgs e)
        {
            #if DEBUG
            maxMinButton.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\max_mo.png")));
            #else
            maxMinButton.Source = new BitmapImage(new Uri(Path.GetFullPath(@"Assets\max_mo.png")));
            #endif
        }

        private void MaxMinButton_MouseLeave(object sender, EventArgs e)
        {
            #if DEBUG
            maxMinButton.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\max_def.png")));
            #else
                        maxMinButton.Source = new BitmapImage(new Uri(Path.GetFullPath(@"Assets\max_def.png")));
            #endif
        }

        private void HamburgerButton_MouseEnter(object sender, EventArgs e)
        {
            #if DEBUG
            hamburgerBackgroundImage.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\hamburger_mo.png")));
            #else
            hamburgerBackgroundImage.Source = new BitmapImage(new Uri(Path.GetFullPath(@"Assets\hamburger_mo.png")));
            #endif
        }

        private void HamburgerButton_MouseLeave(object sender, EventArgs e)
        {
            #if DEBUG
            hamburgerBackgroundImage.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\hamburger_icon.png")));
            #else
            hamburgerBackgroundImage.Source = new BitmapImage(new Uri(Path.GetFullPath(@"Assets\hamburger_icon.png")));
            #endif
        }

        private void AutomaticButton_Click(object sender, EventArgs e)
        {
            mainBottomBorderManual.Visibility = Visibility.Hidden;
            mainBottomBorderAtomatic.Visibility = Visibility.Visible;

            manualContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_lightBackGroundValue);
            AutomaticContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_darkBackGroundValue);
            IsManual = false;
        }

        private void AddFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JF Code (*.jf)|*.jf";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    this._selectedFilePath.Add(filename);
                    selectedFileListBox.Items.Add($"{++_numberOfSelectedFile}\t{Path.GetFileName(filename)}");
                }
            }
        }

        private void DeleteAllFileButton_Click(object sender, EventArgs e)
        {
            selectedFileListBox.Items.Clear();
            selectedCode.Document.Blocks.Clear();
            _numberOfSelectedFile = 0;
            selectedCode.IsReadOnly = false;
            codePrintedFileName.Content = "untitled.jf";
        }

        private void DeleteSelectedFileButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = selectedFileListBox.Items.IndexOf(selectedFileListBox.SelectedItem);
            selectedFileListBox.Items.RemoveAt(selectedIndex);
            selectedCode.Document.Blocks.Clear();
            selectedCode.IsReadOnly = false;
            codePrintedFileName.Content = "untitled.jf";
            _numberOfSelectedFile--;
            _selectedFilePath.RemoveAt(selectedIndex);
            int index = 0;
            for (int i=0; i< selectedFileListBox.Items.Count; i++)
            {
                string oldValue = selectedFileListBox.Items[i].ToString();
                oldValue = Regex.Replace(oldValue, @"[\d-]", string.Empty).Trim();
                selectedFileListBox.Items[i] = $"{++index}\t{oldValue}";
            }
        }

        private void RunAllButton_Click(object sender, EventArgs e)
        {
            #if DEBUG
            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText("JF Message")
                .AddText("Start Run All Codes")
                .AddAppLogoOverride(new Uri(Path.GetFullPath(@"..\..\Assets\ToklanToosLogo.png")), ToastGenericAppLogoCrop.Circle)
                .Show();
            #else
            new ToastContentBuilder()
                            .AddArgument("action", "viewConversation")
                            .AddArgument("conversationId", 9813)
                            .AddText("JF Message")
                            .AddText("Start Run All Codes")
                            .AddAppLogoOverride(new Uri(Path.GetFullPath(@"Assets\ToklanToosLogo.png")), ToastGenericAppLogoCrop.Circle)
                            .Show();
                            
            #endif
        }

        private void FileListBox_SelectionChanged(object sender, EventArgs e)
        {
            string selectedFileName = selectedFileListBox.SelectedItem as string;
            if (selectedFileListBox.SelectedIndex < 0) return;
            if (selectedFileListBox.SelectedIndex > _selectedFilePath.Count) return;
            string selectedFilePath = _selectedFilePath[selectedFileListBox.SelectedIndex];
            selectedFileName = Regex.Replace(selectedFileName, @"[\d-]", string.Empty);
            codePrintedFileName.Content = selectedFileName.Trim();
            string[] lines = File.ReadAllLines(selectedFilePath);
            _rtbIndex = 0;
            selectedCode.Document.Blocks.Clear();
            selectedCode.IsReadOnly = true;

            FlowDocument myFlowDoc = new FlowDocument();
            foreach (string line in lines)
            {
                Run CurrentNumberOfLine = new Run($"{++_rtbIndex}");
                CurrentNumberOfLine.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFDD00");
                Run currentLine = new Run($"\t{line}");
                currentLine.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffffff");
                Paragraph myParagraph = new Paragraph();
                myParagraph.Inlines.Add(CurrentNumberOfLine);
                myParagraph.Inlines.Add(currentLine);
                myFlowDoc.Blocks.Add(myParagraph);
                selectedCode.Document = myFlowDoc;
            }
        }

        private void ManualButton_Click(object sender, EventArgs e)
        {
            mainBottomBorderManual.Visibility = Visibility.Visible;
            mainBottomBorderAtomatic.Visibility = Visibility.Hidden;

            manualContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_darkBackGroundValue); 
            AutomaticContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_lightBackGroundValue);
            IsManual = true;
        }
        private void MinMaxButton_Click(object sender, EventArgs e)
        {
            if (main.WindowState == WindowState.Normal)
            {
                main.WindowState = WindowState.Maximized;
            }
            else if (main.WindowState == WindowState.Maximized)
            {
                main.WindowState = WindowState.Normal;
            }
        }

        private void SaveFileButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.FileName = "myCode";
            saveFileDialog.DefaultExt = "jf";
            saveFileDialog.Filter = "JF Code (*.jf)|*.jf";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            bool? result = saveFileDialog.ShowDialog();
            Stream fileStream;

            if (result == true)
            {
                // Open the file, copy the contents of memoryStream to fileStream,
                // and close fileStream. Set the memoryStream.Position value to 0 
                // to copy the entire stream. 
                fileStream = saveFileDialog.OpenFile();
                MemoryStream userInput = new MemoryStream();
                userInput.Position = 0;
                userInput.WriteTo(fileStream);
                fileStream.Close();
                codePrintedFileName.Content = Path.GetFileName(saveFileDialog.FileName);
            }
        }

        private void HamburgerButton_Click(object sender, EventArgs e)
        {
            DoubleAnimation widthAnimation = new DoubleAnimation();
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            ColorAnimation borderColor = new ColorAnimation();
            double duration = 0.5;
            if (IsMenuOpen)
            {
                borderColor.From = (Color)ColorConverter.ConvertFromString(_lightBackGroundValue);
                borderColor.To = (Color)ColorConverter.ConvertFromString(_darkBackGroundValue);
                widthAnimation.From = 200;
                widthAnimation.To = 0;
                opacityAnimation.From = 1.0;
                opacityAnimation.To = 0.0;
                _ = ChangeGridOfmainContainers(0);
            }
            else
            {
                borderColor.From = (Color)ColorConverter.ConvertFromString(_darkBackGroundValue);
                borderColor.To = (Color)ColorConverter.ConvertFromString(_lightBackGroundValue);
                widthAnimation.From = 0;
                widthAnimation.To = 200;
                opacityAnimation.From = 0.0;
                opacityAnimation.To = 1.0;
                Grid.SetColumn(mainTopBorder, 1);
                Grid.SetColumn(mainBottomBorderManual, 1);
                Grid.SetColumn(mainBottomBorderAtomatic, 1);
            }
            borderColor.Duration = TimeSpan.FromSeconds(duration);
            widthAnimation.Duration = TimeSpan.FromSeconds(duration);
            opacityAnimation.Duration = TimeSpan.FromSeconds(duration);
            Storyboard sb = new Storyboard();

            sb.Children.Add(borderColor);
            sb.Children.Add(widthAnimation);
            sb.Children.Add(opacityAnimation);

            Storyboard.SetTarget(borderColor, menuBorder);
            Storyboard.SetTarget(widthAnimation, menuContainer);
            Storyboard.SetTarget(opacityAnimation, menuBorder);

            Storyboard.SetTargetProperty(borderColor, new PropertyPath("(Border.BorderBrush).(SolidColorBrush.Color)"));
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(StackPanel.WidthProperty));
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Border.OpacityProperty));

            sb.Begin();


            IsMenuOpen = !IsMenuOpen;
        }

        private async Task ChangeGridOfmainContainers(int columnNumber)
        {
            await Task.Delay(700);
            Grid.SetColumn(mainTopBorder, columnNumber);
            Grid.SetColumn(mainBottomBorderManual, columnNumber);
            Grid.SetColumn(mainBottomBorderAtomatic, columnNumber);
        }

        private void SelectedCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (!codePrintedFileName.Content.ToString().Contains("*"))
                codePrintedFileName.Content += "*";
        }

        private void SelectedCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                ////FlowDocument myFlowDoc = new FlowDocument();
                //Run CurrentNumberOfLine = new Run($"{++_rtbIndex}");
                //CurrentNumberOfLine.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFDD00");
                //Run currentLine = new Run($"\t ");
                //currentLine.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#ffffff");
                //Paragraph myParagraph = new Paragraph();
                //myParagraph.Inlines.Add(CurrentNumberOfLine);
                //myParagraph.Inlines.Add(currentLine);
                ////myFlowDoc.Blocks.Add(myParagraph);
                ////selectedCode.Document = myFlowDoc;
                //selectedCode.Document.Blocks.Add(myParagraph);
                //e.Handled = true;
            }
        }
    }
}
