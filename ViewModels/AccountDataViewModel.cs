using PropertyChanged;
using System.ComponentModel;
using System.Windows.Input;

namespace PasswordHints
{
    [AddINotifyPropertyChangedInterface]
    public class AccountDataViewModel
    {
        private string website, email, username, passwordHint;

        public string Website
        {
            get { return website; }
            set { website = value; CommandManager.InvalidateRequerySuggested(); }
        }
        public string Email
        {
            get { return email; }
            set { email = value; CommandManager.InvalidateRequerySuggested(); }
        }
        public string Username
        {
            get { return username; }
            set { username = value; CommandManager.InvalidateRequerySuggested(); }
        }
        public string PasswordHint
        {
            get { return passwordHint; }
            set { passwordHint = value; CommandManager.InvalidateRequerySuggested(); }
        }

        public double BorderHeight { get; set; }
        public double Height { get; set; }

        public double BorderWidth { get; set; }
        public double Width { get; set; }

        public ICommand ClearFieldsCommand { get; set; }

        public AccountDataViewModel()
        {
            ClearFieldsCommand = new RelayCommand(ClearFields);

            Website = Email = Username = PasswordHint = "";
        }

        public AccountDataViewModel(string website, string email, string username, string passwordhint)
        {
            ClearFieldsCommand = new RelayCommand(ClearFields);

            Website = website;
            Email = email;
            Username = username;
            PasswordHint = passwordhint;
        }
        
        private void ClearFields(object o)
        {
            Website = Email = Username = PasswordHint = "";
        }
    }
}
