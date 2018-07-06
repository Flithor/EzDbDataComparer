using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataModelsLib.Model;

namespace DataModelsLib
{
    public class Command : ICommand
    {
        public Action<object> ExecuteCommand;
        public Func<object, bool> CanExecuteCommand = null;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return CanExecuteCommand == null || CanExecuteCommand(parameter);
        }

        public void Execute(object parameter)
        {
            ExecuteCommand?.Invoke(parameter);
        }
    }
    public interface IWindowViewModel
    {

    }
    public class WindowViewModel : IWindowViewModel
    {
        public Command QuerySourceCommand { get; }
        public DataCacheModel DataCache { get; set; }

        public WindowViewModel()
        {

        }
    }
}
