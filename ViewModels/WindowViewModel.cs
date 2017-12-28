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
        public AccountDataViewModel AccountDataViewModel { get; set; }

        public WindowViewModel()
        {
            AccountDataCollectionViewModel = new AccountDataCollectionViewModel();
            AccountDataViewModel = new AccountDataViewModel();
        }
    }
}
