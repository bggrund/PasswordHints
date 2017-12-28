using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PasswordHints
{
    class AddItemCommand : ICommand
    {
        private Action<object> action;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            if(parameter == null)
            {
                return false;
            }

            AccountDataViewModel item = parameter as AccountDataViewModel;
            
            return !(
                string.IsNullOrWhiteSpace(item.Website) &&
                string.IsNullOrWhiteSpace(item.Email) &&
                string.IsNullOrWhiteSpace(item.Username) &&
                string.IsNullOrWhiteSpace(item.PasswordHint)
                );
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }

        public AddItemCommand(Action<object> a)
        {
            action = a;
        }
    }
}
