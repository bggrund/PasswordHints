﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Windows.Media.Animation;

namespace PasswordHints
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*private string placeholderText = "Search here...";
        private string accountDataFilePath = AppDomain.CurrentDomain.BaseDirectory + "AccountData.xml";
        private XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<AccountData>));
        private ICollectionView source;
        private ObservableCollection<AccountData> accountDataList = new ObservableCollection<AccountData>();
        private AccountData newAccountData = new AccountData();
        private BackgroundWorker bwUpdateFilter = new BackgroundWorker();
        private int delayFilterUpdate = 0;*/
        
        public MainWindow()
        {
            DataContext = new WindowViewModel();

            InitializeComponent();
            /*searchBox.GotFocus += (s, e) => searchBox.Text = searchBox.Text == placeholderText ? "" : searchBox.Text;
            searchBox.LostFocus += (s, e) => searchBox.Text = string.IsNullOrWhiteSpace(searchBox.Text) ? placeholderText : searchBox.Text;

            newAccountBox.DataContext = newAccountData;

            if (!File.Exists(accountDataFilePath))
            {
                saveAccountData();
            }

            loadAccountData();

            source = CollectionViewSource.GetDefaultView(accountDataList);
            source.SortDescriptions.Add(new SortDescription("Website", ListSortDirection.Ascending));
            source.Filter = filter;
            credentialItemsControl.ItemsSource = source;

            bwUpdateFilter.WorkerSupportsCancellation = true;
            bwUpdateFilter.DoWork += BwUpdateFilter_DoWork;
            bwUpdateFilter.RunWorkerCompleted += BwUpdateFilter_RunWorkerCompleted;

            searchBox.Focus();*/
        }

        private void btnClearText_Click(object sender, RoutedEventArgs e)
        {
            SearchTermTextBox.Text = string.Empty;
        }

        private void CollapseButton_Click(object sender, RoutedEventArgs e)
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

        /*
private bool filter(object obj)
{
   if (string.IsNullOrWhiteSpace(searchBox.Text) || searchBox.Text == placeholderText)
   {
       return true;
   }

   AccountData item = (AccountData)obj;
   if (cbWebsite.IsChecked == true && item.Website.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
   {
       return true;
   }
   else if (cbEmail.IsChecked == true && item.Email.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
   {
       return true;
   }
   else if (cbUsername.IsChecked == true && item.Username.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
   {
       return true;
   }
   else if (cbPasswordHint.IsChecked == true && item.PasswordHint.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
   {
       return true;
   }
   return false;
}

private void SearchBox_KeyUp(object sender, KeyEventArgs e)
{
   if (!searchBox.IsLoaded)
   {
       return;
   }

   updateFilterDelayed();
}

private void BwUpdateFilter_DoWork(object sender, DoWorkEventArgs e)
{
   Interlocked.Exchange(ref delayFilterUpdate, 0);
   Thread.Sleep(500);

   if (delayFilterUpdate == 1)
   {
       return;
   }

   Dispatcher.Invoke(updateFilter);
}

private void BwUpdateFilter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
{
   if (delayFilterUpdate == 1)
   {
       bwUpdateFilter.RunWorkerAsync();
   }
}

private void updateFilter()
{
   using (source.DeferRefresh())
   {
       source.Filter = filter;
   }
}

private void updateFilterDelayed()
{
   Interlocked.Exchange(ref delayFilterUpdate, 1);
   if (!bwUpdateFilter.IsBusy)
   {
       bwUpdateFilter.RunWorkerAsync();
   }
}

private void btnAdd_Click(object sender, RoutedEventArgs e)
{
   accountDataList.Add(new AccountData(txtInputWebsite.Text, txtInputEmail.Text, txtInputUsername.Text, txtInputPasswordHint.Text));

   updateFilterDelayed();

   saveAccountData();
}

private void saveAccountData()
{
   using (FileStream f = new FileStream(accountDataFilePath, FileMode.Create))
   {
       serializer.Serialize(f, accountDataList);
   }
}

private void loadAccountData()
{
   using (FileStream f = new FileStream(accountDataFilePath, FileMode.Open))
   {
       accountDataList = (ObservableCollection<AccountData>)serializer.Deserialize(f);
   }
}

private void btnClear_Click(object sender, RoutedEventArgs e)
{
   txtInputWebsite.Text = "";
   txtInputEmail.Text = "";
   txtInputUsername.Text = "";
   txtInputPasswordHint.Text = "";
}

private void btnRemove_Click(object sender, RoutedEventArgs e)
{
   accountDataList.Remove((AccountData)((FrameworkElement)sender).DataContext);

   updateFilterDelayed();

   saveAccountData();
}

private void searchField_CheckedChanged(object sender, RoutedEventArgs e)
{
   if (!((FrameworkElement)sender).IsLoaded)
   {
       return;
   }

   updateFilterDelayed();
}

private void btnClearText_Click(object sender, RoutedEventArgs e)
{
   searchBox.Text = string.Empty;

   updateFilterDelayed();

   searchBox.Focus();
}

private void txtWebsite_TextChanged(object sender, TextChangedEventArgs e)
{
   AccountData account = (AccountData)((FrameworkElement)sender).DataContext;
   account.Website = ((TextBox)sender).Text;

   updateFilterDelayed();

   saveAccountData();
}

private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
{
   AccountData account = (AccountData)((FrameworkElement)sender).DataContext;
   account.Email = ((TextBox)sender).Text;

   updateFilterDelayed();

   saveAccountData();
}

private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
{
   AccountData account = (AccountData)((FrameworkElement)sender).DataContext;
   account.Username = ((TextBox)sender).Text;

   updateFilterDelayed();

   saveAccountData();
}

private void txtPasswordHint_TextChanged(object sender, TextChangedEventArgs e)
{
   AccountData account = (AccountData)((FrameworkElement)sender).DataContext;
   account.PasswordHint = ((TextBox)sender).Text;

   updateFilterDelayed();

   saveAccountData();
}
*/
    }
}
