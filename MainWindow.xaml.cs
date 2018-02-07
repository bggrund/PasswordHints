using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace PasswordHints
{
    //TODO: add notification when added item using template from isostotxts

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        #region Constructor

        public MainWindow()
        {
            DataContext = new WindowViewModel();

            InitializeComponent();
        }

        #endregion

        #region Events

        public void btnClearText_Click(object sender, RoutedEventArgs e)
        {
            SearchTermTextBox.Text = string.Empty;
        }

        public void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            Image content = (Image)(((Button)sender).Content);
            FrameworkElement element = (FrameworkElement)FindName(((Button)sender).Tag.ToString());
            if (content.Source == (BitmapImage)FindResource("DownTriangle"))
            {
                Storyboard sb = new Storyboard();

                DoubleAnimation anim = new DoubleAnimation(element.MinHeight, new Duration(new TimeSpan(0, 0, 0, 0, 700)));
                Storyboard.SetTargetProperty(anim, new PropertyPath("Height"));
                anim.DecelerationRatio = 0.8;

                sb.Children.Add(anim);

                sb.Begin(element);

                //NewItemGroupBox.Height = NewItemGroupBox.MinHeight;
                content.Source = (BitmapImage)FindResource("RightTriangle");
            }
            else
            {
                Storyboard sb = new Storyboard();

                DoubleAnimation anim = new DoubleAnimation(element.MaxHeight, new Duration(new TimeSpan(0, 0, 0, 0, 700)));
                Storyboard.SetTargetProperty(anim, new PropertyPath("Height"));
                anim.DecelerationRatio = 0.8;

                sb.Children.Add(anim);

                sb.Begin(element);

                //NewItemGroupBox.Height = NewItemGroupBox.MaxHeight;
                content.Source = (BitmapImage)FindResource("DownTriangle");
            }
        }

        private void SaveAccountData(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)DataContext).AccountDataCollectionViewModel.SaveAccountData();
        }

        private void RemoveItemButtonClick(object sender, RoutedEventArgs e)
        {
            int animationDurationMs = 500;

            // Set RemoveItemDelayMs in AccountDataCollectionViewModel so that it knows how long to delay before removing item from collection
            ((WindowViewModel)DataContext).AccountDataCollectionViewModel.RemoveItemDelayMs = animationDurationMs;

            Border border = (Border)((Grid)((FrameworkElement)sender).Parent).Parent;

            Storyboard sb = new Storyboard();

            DoubleAnimation anim = new DoubleAnimation(0, new Duration(new TimeSpan(0, 0, 0, 0, animationDurationMs)));
            Storyboard.SetTargetProperty(anim, new PropertyPath("Opacity"));
            anim.DecelerationRatio = 0.8;

            sb.Children.Add(anim);

            sb.Begin(border);
        }

        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (((Image)btn.Content).Source == (BitmapImage)FindResource("Maximize"))
            {
                // Set main scroll viewer to collapsed
                MainScrollViewer.Visibility = Visibility.Collapsed;

                // Set maximized border to visible
                AccountDataItem.Visibility = Visibility.Visible;

                // Set data context to button's item
                AccountDataItem.DataContext = btn.DataContext;
            }

            if (((Image)btn.Content).Source == (BitmapImage)FindResource("Restore"))
            {
                // Set main scroll viewer to collapsed
                MainScrollViewer.Visibility = Visibility.Visible;

                // Set maximized border to visible
                AccountDataItem.Visibility = Visibility.Collapsed;
            }

            ((WindowViewModel)DataContext).AccountDataCollectionViewModel.SaveAccountData();
        }

        #endregion
    }
}
