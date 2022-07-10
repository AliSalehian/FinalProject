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
using jf;
using System.Runtime.CompilerServices;
using jf_FinalProject.Logic;
using System.Collections;

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
        private bool _fileHasError = false;
        private List<string> _errorMessages = new List<string>();
        private int _errorCount = 0;
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
            lineIndex.AppendText($"{++_rtbIndex}");
            Logger.Logger.Log("arman dog");
            Logger.Logger.Log("arman dog");
            Logger.Logger.Log("arman dog");
            Logger.Logger.Log(GetCurrentLine(), $"{this.GetType().Name}.cs", "arman ridi dash");
            Logger.Logger.Log(GetCurrentLine(), $"{this.GetType().Name}.cs", "arman ridi dash");
            Logger.Logger.Log(GetCurrentLine(), $"{this.GetType().Name}.cs", "arman ridi dash");
        }

        private static int GetCurrentLine([CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber;
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
            lineIndex.Document.Blocks.Clear();
            _rtbIndex = 0;
            lineIndex.AppendText($"{++_rtbIndex}");
            _numberOfSelectedFile = 0;
            selectedCode.IsReadOnly = false;
            codePrintedFileName.Content = "untitled.jf";
            codePrintedLogLabel.Content = "JF Code Errors";
            jfErrorsContainer.Children.Clear();
            IEnumerator t = jfErrorsContainer.Children.GetEnumerator();
            while (t.MoveNext())
            {
                jfErrorsContainer.Children.Remove((UIElement)t.Current);
            }
        }

        private void DeleteSelectedFileButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = selectedFileListBox.Items.IndexOf(selectedFileListBox.SelectedItem);
            selectedFileListBox.Items.RemoveAt(selectedIndex);
            selectedCode.Document.Blocks.Clear();
            lineIndex.Document.Blocks.Clear();
            _rtbIndex = 0;
            lineIndex.AppendText($"{++_rtbIndex}");
            selectedCode.IsReadOnly = false;
            codePrintedFileName.Content = "untitled.jf";
            codePrintedLogLabel.Content = "Log of ' untitled.jf '";
            _numberOfSelectedFile--;
            _selectedFilePath.RemoveAt(selectedIndex);
            int index = 0;
            for (int i = 0; i < selectedFileListBox.Items.Count; i++)
            {
                string oldValue = selectedFileListBox.Items[i].ToString();
                oldValue = Regex.Replace(oldValue, @"[\d-]", string.Empty).Trim();
                selectedFileListBox.Items[i] = $"{++index}\t{oldValue}";
            }
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
            selectedCode.Document.Blocks.Clear();
            lineIndex.Document.Blocks.Clear();
            _rtbIndex = 0;
            selectedCode.IsReadOnly = true;

            FlowDocument numberOfLineFlowDoc = new FlowDocument();
            FlowDocument lineFlowDoc = new FlowDocument();
            foreach (string line in lines)
            {
                Paragraph lineNumberParagraph = new Paragraph();
                lineNumberParagraph.Inlines.Add($"{++_rtbIndex}");
                numberOfLineFlowDoc.Blocks.Add(lineNumberParagraph);
                lineIndex.Document = numberOfLineFlowDoc;

                Paragraph lineParagraph = new Paragraph();
                lineParagraph.Inlines.Add($"{line}");
                lineFlowDoc.Blocks.Add(lineParagraph);
                selectedCode.Document = lineFlowDoc;
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
                fileStream = saveFileDialog.OpenFile();
                //string rawData = ExtractRawDataFromRichTextBox();
                //MemoryStream userInput = new MemoryStream();
                //userInput.Position = 0;
                //userInput.WriteTo(fileStream);
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
                FlowDocument flow = lineIndex.Document;
                Paragraph p = new Paragraph();
                p.Inlines.Add($"{++_rtbIndex}");
                p.Name = $"line{_rtbIndex}";
                flow.Blocks.Add(p);
                lineIndex.Document = flow;
            }//else if (e.Key == Key.Back)
            //{
            //    TextPointer currentPosition = selectedCode.CaretPosition;
            //    TextPointer nextStart = currentPosition.GetLineStartPosition(1);
            //    TextPointer lineEnd = (nextStart != null ? nextStart : currentPosition.DocumentEnd).GetInsertionPosition(LogicalDirection.Backward);
            //    TextRange r = new TextRange(nextStart, lineEnd);
            //    MessageBox.Show(r.Text);
            //}
        }

        private void RunAllButton_Click(object sender, EventArgs e)
        {

            if (_selectedFilePath.Count >= 0)
            {
                #region Push Notification
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
                #endregion

                #region Start Run
                Logger.Logger logger = new Logger.Logger();
                foreach (string path in _selectedFilePath)
                {
                    _errorCount = 0;
                    _errorMessages.Clear();
                    Compiler compiler = new Compiler();
                    compiler.compile(path);
                    SensorHandler sensor = new SensorHandler();
                    Runner runner = new Runner(compiler, sensor);
                    runner.RichTextNeedUpdate += OnRichTextNeedUpdate;
                    runner.NewLog += OnNewLog;
                    _fileHasError = runner.Run();
                    string selectedFileName = Regex.Replace(path, @"[\d-]", string.Empty);
                    TextBlock textBlock = new TextBlock
                    {
                        Foreground = new SolidColorBrush(Color.FromRgb(102, 255, 102)),
                        Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_darkBackGroundValue),
                        IsEnabled = false,
                        Padding = new Thickness(5),
                        Margin = new Thickness(5),
                        TextWrapping = TextWrapping.Wrap
                    };
                    if (_fileHasError)
                    {
                        textBlock.Inlines.Add(new Run 
                        {
                            Text = $"there is no error in {path}"
                        });
                    }
                    else
                    {
                        textBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 102, 102));
                        textBlock.Inlines.Add(new Run
                        {
                            Text = $"there are {_errorCount} errors in {path}:\n\n"
                        });
                        foreach(string error in _errorMessages)
                        {
                            textBlock.Inlines.Add(new Run
                            {
                                Text = $"{error}\n\n"
                            });
                        }
                    }
                    jfErrorsContainer.Children.Add(textBlock);
                }
                #endregion
            }

        }

        private void TextChangedEvent(object sender, EventArgs e)
        {
            
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            lineIndex.ScrollToVerticalOffset(e.VerticalOffset);
        }

        private void OnRichTextNeedUpdate(object sender, CommandEventArgs e)
        {
            //TextRange range = new TextRange(selectedCode.Document.ContentStart, selectedCode.Document.ContentEnd);
            //MessageBox.Show($"type is {e.Type} and line number is {e.LineNumber}");
        }

        private void OnNewLog(object sender, LogEventArgs e)
        {
            _errorCount++;
            //_errorMessages += $"Error in {e.CallerName} at line {e.LineNumber + 1}:\t{e.ErrorMessage}";
            _errorMessages.Add($"{e.CallerName} Error at line {e.LineNumber + 1}: {e.ErrorMessage}");
        }
    }
}
