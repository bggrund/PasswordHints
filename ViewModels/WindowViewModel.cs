using PropertyChanged;

namespace PasswordHints
{
    /// <summary>
    /// Window view model containing view models for contents
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class WindowViewModel
    {
        public AccountDataCollectionViewModel AccountDataCollectionViewModel { get; set; }
        public AccountDataViewModel NewAccountDataViewModel { get; set; }
        public AccountDataViewModel SelectedAccountDataViewModel { get; set; }

        public WindowViewModel()
        {
            AccountDataCollectionViewModel = new AccountDataCollectionViewModel();
            NewAccountDataViewModel = new AccountDataViewModel();
            SelectedAccountDataViewModel = new AccountDataViewModel();
        }
    }
}
