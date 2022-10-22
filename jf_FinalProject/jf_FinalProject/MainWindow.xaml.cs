using Microsoft.Win32;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<string> _selectedFilePath = new ObservableCollection<string>();
        private int _rtbIndex = 0;
        private string _darkBackGroundValue = "#292F34";
        private string _lightBackGroundValue = "#3A4149";
        private string _green = "#00ff00";
        private string _gray = "#DCE0E4";
        private string _yellow = "#FFDD00";
        private string _darkYellow = "#8f7c27";
        private SolidColorBrush _greenColor;
        private bool _fileHasError = false;
        private List<string> _errorMessages = new List<string>();
        private int _errorCount = 0;
        private bool _hydraulicPressed = false;
        private bool _isMotorRunning = false;
        private string selectedSensor;
        private SensorHandler sensor = new SensorHandler();
        private Compiler compiler;
        private Runner runner;
        private SolidColorBrush _animatedBrush;
        private ObservableCollection<string> _selectedItemsInListBox;
        
        private bool IsMenuOpen { get; set; }

        public bool IsManual { get; set; }

        public ObservableCollection<string> SelectedFilePath
        {
            get { return _selectedFilePath; }
            set { _selectedFilePath = value; }
        }
        public MainWindow()
        {
            InitializeComponent();
            //HardwareInterface hardware = new HardwareInterface();
            _greenColor = (SolidColorBrush)new BrushConverter().ConvertFrom(_green);
            IsMenuOpen = false;
            IsManual = true;
            Label label = CreateLabel(Visibility.Hidden, $"l{_rtbIndex}", _greenColor);
            codePointer.Children.Add(label);
            lineIndex.AppendText($"{++_rtbIndex}");
            runner = new Runner(this.sensor);
            compiler = new Compiler();
            runner.RichTextNeedUpdate += OnRichTextNeedUpdate;
            runner.NewLog += OnNewLog;

            ColorAnimation calibrationColorAnim = new ColorAnimation();
            _animatedBrush = new SolidColorBrush();
            calibrationColorAnim.From = (Color)ColorConverter.ConvertFromString(_darkBackGroundValue);
            calibrationColorAnim.To = (Color)ColorConverter.ConvertFromString(_darkYellow);
            calibrationColorAnim.Duration = new Duration(TimeSpan.FromSeconds(2));
            calibrationColorAnim.RepeatBehavior = RepeatBehavior.Forever;
            calibrationColorAnim.AutoReverse = true;
            _animatedBrush.BeginAnimation(SolidColorBrush.ColorProperty, calibrationColorAnim);

            _selectedItemsInListBox = new ObservableCollection<string>();
        }

        private Label CreateLabel(Visibility visibility, string name, SolidColorBrush color)
        {
            Label label = new Label();
            label.Name = name;
            label.Foreground = color;
            label.Visibility = visibility;
            label.Content = ">";
            return label;
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

        private void selectTemp1(object sender, EventArgs e)
        {
            temp1Container.Background = this._animatedBrush;
            speedContainer.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp2Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp3Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            loadCell1Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            loadCell2Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            rpmContainer.Background = this.FindResource("darkBackGround") as SolidColorBrush;

            this.selectedSensor = "t1";
            selectedSensorName.Content = "temperature 1";
            Tuple<double, double> sensorCalibration = this.sensor.getCalibration("t1");
            gain.Text = Convert.ToString(sensorCalibration.Item1);
            arzAzMabda.Text = Convert.ToString(sensorCalibration.Item2);
        }

        private void selectSpeed(object sender, EventArgs e)
        {
            speedContainer.Background = this._animatedBrush;
            temp1Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp2Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp3Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            loadCell1Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            loadCell2Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            rpmContainer.Background = this.FindResource("darkBackGround") as SolidColorBrush;

            this.selectedSensor = "speed";
            selectedSensorName.Content = "speed";
            Tuple<double, double> sensorCalibration = this.sensor.getCalibration("n"); //TODO: in aya bayad hamin bashe? sensor n aya speed mide be ma? 
            gain.Text = Convert.ToString(sensorCalibration.Item1);
            arzAzMabda.Text = Convert.ToString(sensorCalibration.Item2);
        }

        private void selectTemp2(object sender, EventArgs e)
        {
            temp2Container.Background = this._animatedBrush;
            speedContainer.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp1Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp3Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            loadCell1Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            loadCell2Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            rpmContainer.Background = this.FindResource("darkBackGround") as SolidColorBrush;

            this.selectedSensor = "t2";
            selectedSensorName.Content = "temperature 2";
            Tuple<double, double> sensorCalibration = this.sensor.getCalibration("t2");
            gain.Text = Convert.ToString(sensorCalibration.Item1);
            arzAzMabda.Text = Convert.ToString(sensorCalibration.Item2);
        }

        private void selectTemp3(object sender, EventArgs e)
        {
            temp3Container.Background = this._animatedBrush;
            speedContainer.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp2Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp1Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            loadCell1Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            loadCell2Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            rpmContainer.Background = this.FindResource("darkBackGround") as SolidColorBrush;

            this.selectedSensor = "t3";
            selectedSensorName.Content = "temperature 3";
            Tuple<double, double> sensorCalibration = this.sensor.getCalibration("t3");
            gain.Text = Convert.ToString(sensorCalibration.Item1);
            arzAzMabda.Text = Convert.ToString(sensorCalibration.Item2);
        }

        private void selectLoadCell1(object sender, EventArgs e)
        {
            loadCell1Container.Background = this._animatedBrush;
            speedContainer.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp2Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp3Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp1Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            loadCell2Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            rpmContainer.Background = this.FindResource("darkBackGround") as SolidColorBrush;

            this.selectedSensor = "mleft";
            selectedSensorName.Content = "loadCell 1";
            Tuple<double, double> sensorCalibration = this.sensor.getCalibration("mleft");
            gain.Text = Convert.ToString(sensorCalibration.Item1);
            arzAzMabda.Text = Convert.ToString(sensorCalibration.Item2);
        }

        private void selectLoadCell2(object sender, EventArgs e)
        {
            loadCell2Container.Background = this._animatedBrush;
            speedContainer.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp2Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp3Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            loadCell1Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp1Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            rpmContainer.Background = this.FindResource("darkBackGround") as SolidColorBrush;

            this.selectedSensor = "mright";
            selectedSensorName.Content = "loadCell 2";
            Tuple<double, double> sensorCalibration = this.sensor.getCalibration("mright");
            gain.Text = Convert.ToString(sensorCalibration.Item1);
            arzAzMabda.Text = Convert.ToString(sensorCalibration.Item2);
        }

        private void selectRpm(object sender, EventArgs e)
        {
            rpmContainer.Background = this._animatedBrush;
            speedContainer.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp2Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp3Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            loadCell1Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            loadCell2Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;
            temp1Container.Background = this.FindResource("darkBackGround") as SolidColorBrush;

            this.selectedSensor = "p";
            selectedSensorName.Content = "RPM";
            Tuple<double, double> sensorCalibration = this.sensor.getCalibration("p");//TODO: RPM bayad beshe Presure
            gain.Text = Convert.ToString(sensorCalibration.Item1);
            arzAzMabda.Text = Convert.ToString(sensorCalibration.Item2);
        }

        private void AutomaticButton_Click(object sender, EventArgs e)
        {
            mainBottomBorderAtomatic.Visibility = Visibility.Visible;
            mainBottomBorderManual.Visibility = Visibility.Hidden;
            mainBottomBorderCalibration.Visibility = Visibility.Hidden;

            mainTopBorder.Visibility = Visibility.Visible;
            mainTopBorderCalibration.Visibility = Visibility.Hidden;

            manualContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_lightBackGroundValue);
            calibrationContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_lightBackGroundValue);
            AutomaticContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_darkBackGroundValue);
            IsManual = false;
        }

        private void AddFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text File (*.txt)|*.txt";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    _selectedFilePath.Add(filename);
                    //selectedFileListBox.Items.Add($"{++_numberOfSelectedFile}\t{Path.GetFileName(filename)}");
                    _selectedItemsInListBox.Add($"{++_numberOfSelectedFile}\t{Path.GetFileName(filename)}");
                }
                selectedFileListBox.ItemsSource = _selectedItemsInListBox;
                Style itemContainerStyle = new Style(typeof(ListBoxItem));
                itemContainerStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));
                itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(selectedFileListBox_PreviewMouseLeftButtonDown)));
                itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.DropEvent, new DragEventHandler(selectedFileListBox_Drop)));
                selectedFileListBox.ItemContainerStyle = itemContainerStyle;
            }
        }

        void selectedFileListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                draggedItem.IsSelected = true;
            }
        }
        void selectedFileListBox_Drop(object sender, DragEventArgs e)
        {
            string droppedData = e.Data.GetData(typeof(string)) as string;
            string target = ((ListBoxItem)(sender)).DataContext as string;

            int removedIdx = selectedFileListBox.Items.IndexOf(droppedData);
            int targetIdx = selectedFileListBox.Items.IndexOf(target);

            string droppedFullPath = _selectedFilePath[removedIdx];

            if (removedIdx < targetIdx)
            {
                _selectedItemsInListBox.Insert(targetIdx + 1, droppedData);
                _selectedItemsInListBox.RemoveAt(removedIdx);

                _selectedFilePath.Insert(targetIdx + 1, droppedFullPath);
                _selectedFilePath.RemoveAt(removedIdx);
            }
            else
            {
                int remIdx = removedIdx + 1;
                if (_selectedItemsInListBox.Count + 1 > remIdx)
                {
                    _selectedItemsInListBox.Insert(targetIdx, droppedData);
                    _selectedItemsInListBox.RemoveAt(remIdx);

                    _selectedFilePath.Insert(targetIdx, droppedFullPath);
                    _selectedFilePath.RemoveAt(remIdx);
                }
            }
            selectedFileListBox.SelectedIndex = targetIdx;
            FileListBox_SelectionChanged();
        }

        private void DeleteAllFileButton_Click(object sender, EventArgs e)
        {
            this._selectedFilePath.Clear();
            //selectedFileListBox.Items.Clear();
            _selectedItemsInListBox.Clear();
            selectedCode.Document.Blocks.Clear();
            lineIndex.Document.Blocks.Clear();
            codePointer.Children.Clear();
            _rtbIndex = 0;
            Label label = CreateLabel(Visibility.Hidden, $"l{_rtbIndex}", _greenColor);
            codePointer.Children.Add(label);
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
            if (selectedIndex < 0) return;
            //selectedFileListBox.Items.RemoveAt(selectedIndex);
            _selectedItemsInListBox.RemoveAt(selectedIndex);
            selectedCode.Document.Blocks.Clear();
            lineIndex.Document.Blocks.Clear();
            codePointer.Children.Clear();
            _rtbIndex = 0;
            Label label = CreateLabel(Visibility.Hidden, $"l{_rtbIndex}", _greenColor);
            codePointer.Children.Add(label);
            lineIndex.AppendText($"{++_rtbIndex}");
            selectedCode.IsReadOnly = false;
            codePrintedFileName.Content = "untitled.jf";
            codePrintedLogLabel.Content = "Log of ' untitled.jf '";
            _numberOfSelectedFile--;
            _selectedFilePath.RemoveAt(selectedIndex);
            //int index = 0;
            //for (int i = 0; i < selectedFileListBox.Items.Count; i++)
            //{
            //    string oldValue = selectedFileListBox.Items[i].ToString();
            //    oldValue = Regex.Replace(oldValue, @"[\d-]", string.Empty).Trim();
            //    //selectedFileListBox.Items[i] = $"{++index}\t{oldValue}";
            //}
        }

        #region Manual Hanlders

        private void BrakeButton_Click(object sender, EventArgs e)
        {
            BrakeType brakeType = _hydraulicPressed ? BrakeType.Pressure : BrakeType.Moment;
            Command.Brake(moment.Value, pressure.Value, brakeType);
            brake.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_gray);
            brake.IsEnabled = false;
        }

        private void HydraulicButton_Click(object sender, EventArgs e)
        {
            if (_hydraulicPressed)
                hydraulic.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_yellow);
            else
                hydraulic.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_gray);
            _hydraulicPressed = !_hydraulicPressed;
        }

        private void UnloadBrake_Click(object sender, EventArgs e)
        {
            Command.UnloadBrake();
            brake.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_yellow);
            brake.IsEnabled = true;
        }

        private void ManualRun_Click(object sender, EventArgs e)
        {
            _isMotorRunning = !_isMotorRunning;
            if (_isMotorRunning)
            {
                RunDirection direction = (bool)backCheckBox.IsChecked ? RunDirection.Backward : RunDirection.Forward;
                Command.Run(speed.Value, direction);
                manualRun.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_gray);
            }
            else
            {
                manualRun.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_yellow);
            }
        }

        #endregion

        private void FileListBox_SelectionChanged()
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
            codePointer.Children.Clear();
            _rtbIndex = 0;
            selectedCode.IsReadOnly = true;

            FlowDocument numberOfLineFlowDoc = new FlowDocument();
            FlowDocument lineFlowDoc = new FlowDocument();
            foreach (string line in lines)
            {
                Paragraph lineNumberParagraph = new Paragraph();
                lineNumberParagraph.Inlines.Add($"{++_rtbIndex}");
                Label label = CreateLabel(Visibility.Hidden, $"l{_rtbIndex}", _greenColor);
                codePointer.Children.Add(label);
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
            mainBottomBorderCalibration.Visibility = Visibility.Hidden;

            mainTopBorderCalibration.Visibility = Visibility.Hidden;
            mainTopBorder.Visibility = Visibility.Visible;


            manualContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_darkBackGroundValue);
            AutomaticContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_lightBackGroundValue);
            calibrationContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_lightBackGroundValue);
            IsManual = true;
        }

        private void CalibrationButton_Click(object sender, EventArgs e)
        {
            mainBottomBorderCalibration.Visibility = Visibility.Visible;
            mainBottomBorderManual.Visibility = Visibility.Hidden;
            mainBottomBorderAtomatic.Visibility = Visibility.Hidden;

            mainTopBorderCalibration.Visibility = Visibility.Visible;
            mainTopBorder.Visibility = Visibility.Hidden;

            calibrationContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_darkBackGroundValue);
            AutomaticContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_lightBackGroundValue);
            manualContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_lightBackGroundValue);
        }

        private void setCalibration_Clicked(object sender, EventArgs e)
        {
            if (this.selectedSensor != "")
            {
                double doubleGain, doubleArzAzMabda;
                try
                {
                    doubleGain = Convert.ToDouble(gain.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("gain value should be a float number");
                    return;
                }

                try
                {
                    doubleArzAzMabda = Convert.ToDouble(arzAzMabda.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("arz az mabda value should be a float number");
                    return;
                }
                this.sensor.setCalibration(this.selectedSensor, doubleGain, doubleArzAzMabda);
            }
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
                Grid.SetColumn(mainTopBorderCalibration, 1);
                Grid.SetColumn(mainBottomBorderManual, 1);
                Grid.SetColumn(mainBottomBorderAtomatic, 1);
                Grid.SetColumn(mainBottomBorderCalibration, 1);
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
            Grid.SetColumn(mainTopBorderCalibration, columnNumber);
            Grid.SetColumn(mainBottomBorderManual, columnNumber);
            Grid.SetColumn(mainBottomBorderAtomatic, columnNumber);
            Grid.SetColumn(mainBottomBorderCalibration, columnNumber);
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
                Label label = CreateLabel(Visibility.Hidden, $"l{_rtbIndex}", _greenColor);
                codePointer.Children.Add(label);
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

        private void StartRunThread(ObservableCollection<string> pathList)
        {

            //this.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    runner.RichTextNeedUpdate += OnRichTextNeedUpdate;
            //    runner.NewLog += OnNewLog;
            //}));
            foreach (string path in pathList)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _errorCount = 0;
                    _errorMessages.Clear();
                    addFileButton.IsEnabled = false;
                    deleteAllFileButton.IsEnabled = false;
                    deleteSelectedFileButton.IsEnabled = false;
                    runAllButton.IsEnabled = false;

                    addFileButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_gray);
                    deleteAllFileButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_gray);
                    deleteSelectedFileButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_gray);
                    runAllButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_gray);
                }));
                compiler.compile(path);
                runner._compiler = compiler;
                this._fileHasError = runner.Run();
                this.Dispatcher.BeginInvoke(new Action(() => {

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
                        foreach (string error in _errorMessages)
                        {
                            textBlock.Inlines.Add(new Run
                            {
                                Text = $"{error}\n\n"
                            });
                        }
                        _errorCount = 0;
                        _errorMessages.Clear();
                    }
                    jfErrorsContainer.Children.Add(textBlock);
                }));
            }
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                _errorCount = 0;
                _errorMessages.Clear();
                addFileButton.IsEnabled = true;
                deleteAllFileButton.IsEnabled = true;
                deleteSelectedFileButton.IsEnabled = true;
                runAllButton.IsEnabled = true;

                addFileButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_yellow);
                deleteAllFileButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_yellow);
                deleteSelectedFileButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_yellow);
                runAllButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(_yellow);
            }));
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
                Thread thread = new Thread(() => { StartRunThread(_selectedFilePath); });
                thread.Start();
                #endregion
            }

        }

        private void TextChangedEvent(object sender, EventArgs e)
        {
            
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            lineIndex.ScrollToVerticalOffset(e.VerticalOffset);
            codePointerScroll.ScrollToVerticalOffset(e.VerticalOffset);
            selectedCode.ScrollToVerticalOffset(e.VerticalOffset);
        }

        private void OnRichTextNeedUpdate(object sender, CommandEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                TextRange range = new TextRange(lineIndex.Document.ContentStart, lineIndex.Document.ContentEnd);
                TextPointer current = range.Start.GetInsertionPosition(LogicalDirection.Forward);
                int line = 0;
                while (current != null)
                {
                    if (line == e.LineNumber)
                    {
                        if (line != 0)
                        {
                            //codePointer.Children[line - 1].Visibility = Visibility.Hidden;
                        }
                        if (line >= codePointer.Children.Count) break;
                        codePointer.Children[line].Visibility = Visibility.Visible;
                        break;
                    }
                    current = current.GetNextContextPosition(LogicalDirection.Forward);
                    line++;
                }
            }));
        }

        private void OnNewLog(object sender, LogEventArgs e)
        {
            _errorCount++;
            //_errorMessages += $"Error in {e.CallerName} at line {e.LineNumber + 1}:\t{e.ErrorMessage}";
            _errorMessages.Add($"{e.CallerName} Error at line {e.LineNumber + 1}: {e.ErrorMessage}");
        }
    }
}
