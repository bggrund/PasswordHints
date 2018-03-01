using Microsoft.Win32;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace PasswordHints
{
    // TODO:    Add item notification
    //          Let user choose account data file path when the one in AccountDataFilePathFilePath isn't found
    [AddINotifyPropertyChangedInterface]
    public class AccountDataCollectionViewModel
    {
        #region Private Members

        private string searchText, placeholderText = "Search here...";
        private bool searchWebsite, searchEmail, searchUsername, searchPasswordHint;
        private BackgroundWorker bwUpdateFilter = new BackgroundWorker();
        private int delayFilterUpdate = 0;

        private string mAccountDataFilePath;
        private string mTxtAccountDataFilePath;

        #endregion

        #region Static Members

        public static readonly string DefaultAccountDataFilePath = AppDomain.CurrentDomain.BaseDirectory + "AccountData.xml";
        public static readonly string AccountDataFilePathFilePath = AppDomain.CurrentDomain.BaseDirectory + "AccountDataFilePath.txt";

        #endregion

        #region Public Properties

        public string AccountDataFilePath { get { return mAccountDataFilePath; } set { mAccountDataFilePath = value; mTxtAccountDataFilePath = value.Replace(".xml", ".txt"); } }

        public bool ItemAdded { get; set; }

        /// <summary>
        /// List of account data items
        /// </summary>
        public ObservableCollection<AccountDataViewModel> Items { get; set; }

        /// <summary>
        /// Collection view for sorting and filtering <see cref="Items"/>
        /// </summary>
        public ICollectionView CollectionView { get; set; }

        /// <summary>
        /// Command that removes account data item from list
        /// </summary>
        public ICommand RemoveItemCommand { get; set; }

        /// <summary>
        /// Command that adds account data item from list
        /// </summary>
        public ICommand AddItemCommand { get; set; }

        /// <summary>
        /// Command that calls <see cref="ChooseAccountDataFile"/>
        /// </summary>
        public ICommand ChooseAccountDataFileCommand { get; set; }

        /// <summary>
        /// Command that calls <see cref="OpenAccountDataFileLocation"/>
        /// </summary>
        public ICommand OpenAccountDataFileLocationCommand { get; set; }

        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                searchText = value;
                updateFilterDelayed();
            }
        }

        public bool SearchWebsite
        {
            get
            {
                return searchWebsite;
            }
            set
            {
                searchWebsite = value;
                updateFilterDelayed();
            }
        }

        public bool SearchEmail
        {
            get
            {
                return searchEmail;
            }
            set
            {
                searchEmail = value;
                updateFilterDelayed();
            }
        }

        public bool SearchUsername
        {
            get
            {
                return searchUsername;
            }
            set
            {
                searchUsername = value;
                updateFilterDelayed();
            }
        }

        public bool SearchPasswordHint
        {
            get
            {
                return searchPasswordHint;
            }
            set
            {
                searchPasswordHint = value;
                updateFilterDelayed();
            }
        }

        public int RemoveItemDelayMs { get; set; } = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public AccountDataCollectionViewModel()
        {
            // Initialize account data file path
            InitializeAccountDataFilePath();

            // Initialize account data items
            InitializeAccountDataItems();

            // Initialze commands
            RemoveItemCommand = new RelayCommand(RemoveItem);
            AddItemCommand = new AddItemCommand(AddItem);
            ChooseAccountDataFileCommand = new RelayCommand(ChooseAccountDataFile);
            OpenAccountDataFileLocationCommand = new RelayCommand(OpenAccountDataFileLocation);

            // Initialize Properties
            SearchWebsite = SearchEmail = SearchUsername = SearchPasswordHint = true;

            // Initialize background worker for filtering
            bwUpdateFilter.DoWork += BwUpdateFilter_DoWork;
            bwUpdateFilter.RunWorkerCompleted += BwUpdateFilter_RunWorkerCompleted;
        }

        #endregion

        #region Commands and Helpers

        /// <summary>
        /// Removes account data item from <see cref="Items"/>
        /// </summary>
        /// <param name="item"><see cref="AccountDataViewModel"/> to remove</param>
        private async void RemoveItem(object item)
        {
            await Task.Delay(RemoveItemDelayMs);

            Items.Remove(item as AccountDataViewModel);

            SaveAccountData();
        }

        /// <summary>
        /// Adds account data item to <see cref="Items"/>
        /// </summary>
        /// <param name="item"><see cref="AccountDataViewModel"/> to add</param>
        private void AddItem(object item)
        {
            ItemAdded = false;

            try
            {
                AccountDataViewModel accountData = item as AccountDataViewModel;
                Items.Add(new AccountDataViewModel(accountData.Website, accountData.Email, accountData.Username, accountData.PasswordHint));

                SaveAccountData();

                ItemAdded = true;
            }
            catch { }
        }

        /// <summary>
        /// Saves account data as sorted by <see cref="CollectionView"/>
        /// </summary>
        public void SaveAccountData()
        {
            List<AccountData> sortedAccountData = new List<AccountData>(Items.Select(i => new AccountData(i.Website, i.Email, i.Username, i.PasswordHint)));
            sortedAccountData.Sort();

            AccountDataCollection.SaveAccountData(AccountDataFilePath, sortedAccountData);

            List<string> lines = new List<string>();

            foreach(AccountData data in sortedAccountData)
            {
                lines.Add(data.Website);
                lines.Add(data.Email);
                lines.Add(data.Username);
                lines.Add(data.PasswordHint);
                lines.Add("----");
            }

            File.WriteAllLines(mTxtAccountDataFilePath, lines);
            /*
            for(int i = 0; i < lines.Count; i++)
            {
                lines[i] = "<p>" + lines[i] + "</p>";
            }

            File.WriteAllText(mHtmlAccountDataFilePath, "<html><body>");
            File.AppendAllLines(mHtmlAccountDataFilePath, lines);
            File.AppendAllText(mHtmlAccountDataFilePath, "</body></html>");
            */
        }

        /// <summary>
        /// Presents file dialog for user to choose the account data file from. If the user selects a valid file, this method updates <see cref="AccountDataFilePath"/> with selected file, writes the selected file's path to <see cref="AccountDataFilePathFilePath"/>, and reinitializes the <see cref="Items"/> collection and its <see cref="CollectionView"/>.
        /// </summary>
        /// <param name="parameter">null parameter</param>
        private void ChooseAccountDataFile(object parameter)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.AddExtension = true;
            ofd.DefaultExt = ".xml";
            ofd.FileName = "AccountData.xml";
            ofd.Filter = "Xml Files (*.xml)|*.xml";
            ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == true)
            {
                AccountDataFilePath = ofd.FileName;
                File.WriteAllText(AccountDataFilePathFilePath, ofd.FileName);

                InitializeAccountDataItems();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter">null parameter</param>
        private void OpenAccountDataFileLocation(object parameter)
        {
            Process.Start((new FileInfo(AccountDataFilePath)).Directory.FullName);
        }

        /// <summary>
        /// Attempts to read account data file path from AccountDataFilePath.txt and sets <see cref="AccountDataFilePath"/> to this path if it is valid or to a path in the base directory if it is invalid
        /// </summary>
        private void InitializeAccountDataFilePath()
        {
            // Get file used to store account data file path for the user
            string accountDataFilePathFilePath = AccountDataFilePathFilePath;

            // Create the file if it doesn't exist
            if (!File.Exists(accountDataFilePathFilePath))
            {
                File.Create(accountDataFilePathFilePath);
            }

            // Verify the read-in file's directory exists and that the file is an xml file, otherwise set path to base directory
            try
            {
                // Read path from file
                AccountDataFilePath = File.ReadAllText(accountDataFilePathFilePath);
                FileInfo f = new FileInfo(AccountDataFilePath);
                if (!Directory.Exists(f.Directory.FullName))
                {
                    throw new Exception();
                }
                if (string.Compare(f.Extension, ".xml", true) != 0)
                {
                    throw new Exception();
                }
            }
            // Exception caught means read-in file's directory does not exist or that the file is not an xml file, so set AccountDataFilePath to an xml file in the base directory
            catch (Exception ex)
            {
                AccountDataFilePath = DefaultAccountDataFilePath;
            }
        }

        /// <summary>
        /// Reads items from <see cref="AccountDataFilePath"/> and initializes the <see cref="Items"/> collection and its <see cref="CollectionView"/>
        /// </summary>
        private void InitializeAccountDataItems()
        {
            // Initialize items
            Items = new ObservableCollection<AccountDataViewModel>(AccountDataCollection.GetAccountData(AccountDataFilePath).Select(item => new AccountDataViewModel(item.Website, item.Email, item.Username, item.PasswordHint)));

            // Initialize collection view
            CollectionView = CollectionViewSource.GetDefaultView(Items);
            CollectionView.SortDescriptions.Add(new SortDescription("Website", ListSortDirection.Ascending));
            CollectionView.Filter = Filter;
        }

        #endregion

        #region CollectionView Filtering

        private bool Filter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText) || SearchText == placeholderText)
            {
                return true;
            }

            AccountDataViewModel item = (AccountDataViewModel)obj;
            if (SearchWebsite && item.Website.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
            else if (SearchEmail && item.Email.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
            else if (SearchUsername && item.Username.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
            else if (SearchPasswordHint && item.PasswordHint.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
            return false;
        }

        private void BwUpdateFilter_DoWork(object sender, DoWorkEventArgs e)
        {
            Interlocked.Exchange(ref delayFilterUpdate, 0);
            Thread.Sleep(500);

            if (delayFilterUpdate == 1)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(updateFilter);
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
            using (CollectionView.DeferRefresh())
            {
                CollectionView.Filter = Filter;
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

        #endregion
    }
}
