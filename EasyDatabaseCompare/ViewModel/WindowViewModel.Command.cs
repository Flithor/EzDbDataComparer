using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasyDatabaseCompare.ViewModel
{
    public class ActionCommand : ICommand
    {
        public ActionCommand(Action executeCommand)
        {
            ExecuteCommand = executeCommand;
        }
        public Action ExecuteCommand;
        public Func<object, bool> CanExecuteCommand = null;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return CanExecuteCommand == null || CanExecuteCommand(parameter);
        }

        public void Execute(object parameter)
        {
            try
            {
                ExecuteCommand?.Invoke();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}\r\n==========\r\n{ex.InnerException}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
    public class WithParameterCommand : ICommand
    {
        public WithParameterCommand(Action<object> executeCommand)
        {
            ExecuteCommand = executeCommand;
        }
        public Action<object> ExecuteCommand;
        public Func<object, bool> CanExecuteCommand = null;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return CanExecuteCommand == null || CanExecuteCommand(parameter);
        }

        public void Execute(object parameter)
        {
            try
            {
                
                ExecuteCommand?.Invoke(parameter);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}\r\n==========\r\n{ex.InnerException}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }

    public partial class WindowViewModel
    {
        public ICommand ResetCommand { get; }

        public ICommand CheckConnectionCommand { get; }
        public ICommand QuerySourceCommand { get; }
        public ICommand QueryTargetCommand { get; }
        public ICommand QueryTargetWithComparerCommand { get; }
        public ICommand StartComparerCommand { get; }
        public ICommand MoveTargetToSourceCommand { get; }
        public ICommand DisplayTargetDetailCommand { get; }
    }
}
