using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace EasyDatabaseCompare.ViewModel
{
    class DataTableComparer : ICommand
    {
        #region Fields
        private Action execute;
        private Func<bool> canExecute;
        #endregion

        #region Implement interface
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if(canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if(canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }
        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute();
        }
        public void Execute(object parameter)
        {
            execute();
        }
        #endregion
    }
}
