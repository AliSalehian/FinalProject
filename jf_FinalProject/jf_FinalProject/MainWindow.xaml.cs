using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace jf_FinalProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsMenuOpen { get; set; }

        public bool IsManual { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            IsMenuOpen = false;
            IsManual = true;
        }

        private void shutDownButton_Click(object sender, EventArgs e) { Application.Current.Shutdown(); }

        private void shutDownButton_MouseEnter(object sender, EventArgs e) { shutdownBackgroundImage.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\shutdown_mo.png"))); }

        private void shutDownButton_MouseLeave(object sender, EventArgs e) { shutdownBackgroundImage.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\shutdown_def.png"))); }

        private void maxMinButton_MouseEnter(object sender, EventArgs e) { maxMinButton.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\max_mo.png"))); }

        private void maxMinButton_MouseLeave(object sender, EventArgs e) { maxMinButton.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\max_def.png"))); }

        private void HamburgerButton_MouseEnter(object sender, EventArgs e) { hamburgerBackgroundImage.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\hamburger_mo.png"))); }

        private void HamburgerButton_MouseLeave(object sender, EventArgs e) { hamburgerBackgroundImage.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\hamburger_icon.png"))); }

        private void PressureUpButton_MouseEnter(object sender, EventArgs e) { pressureUpBtn.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\flesh_up_gray.png"))); }

        private void PressureUpButton_MouseLeave(object sender, EventArgs e) { pressureUpBtn.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\flesh_up_yellow.png"))); }

        private void PressureDownButton_MouseEnter(object sender, EventArgs e) { pressureDownBtn.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\flesh_down_gray.png"))); }

        private void PressureDownButton_MouseLeave(object sender, EventArgs e) { pressureDownBtn.Source = new BitmapImage(new Uri(Path.GetFullPath(@"..\..\Assets\flesh_down_yellow.png"))); }

        private void AutomaticButton_Click(object sender, EventArgs e)
        {
            mainBottomBorderManual.Visibility = Visibility.Hidden;
            mainBottomBorderAtomatic.Visibility = Visibility.Visible;

            manualContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#3A4149");
            AutomaticContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#292F34");
            IsManual = false;
        }

        private void ManualButton_Click(object sender, EventArgs e)
        {
            mainBottomBorderManual.Visibility = Visibility.Visible;
            mainBottomBorderAtomatic.Visibility = Visibility.Hidden;

            manualContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#292F34"); 
            AutomaticContainer.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#3A4149");
            IsManual = true;
        }
        private void minMaxButton_Click(object sender, EventArgs e)
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

        private void hamburgerButton_Click(object sender, EventArgs e)
        {
            DoubleAnimation widthAnimation = new DoubleAnimation();
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            ColorAnimation borderColor = new ColorAnimation();
            double duration = 0.5;
            if (IsMenuOpen)
            {
                borderColor.From = (Color)ColorConverter.ConvertFromString("#3A4149");
                borderColor.To = (Color)ColorConverter.ConvertFromString("#292F34");
                widthAnimation.From = 200;
                widthAnimation.To = 0;
                opacityAnimation.From = 1.0;
                opacityAnimation.To = 0.0;
                _ = changeGridOfmainContainers(0);
            }
            else
            {
                borderColor.From = (Color)ColorConverter.ConvertFromString("#292F34");
                borderColor.To = (Color)ColorConverter.ConvertFromString("#3A4149");
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

        private async Task changeGridOfmainContainers(int columnNumber)
        {
            await Task.Delay(700);
            Grid.SetColumn(mainTopBorder, columnNumber);
            Grid.SetColumn(mainBottomBorderManual, columnNumber);
            Grid.SetColumn(mainBottomBorderAtomatic, columnNumber);
        }
    }
}
