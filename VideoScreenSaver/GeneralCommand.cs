using System;
using System.Windows.Input;

namespace VideoScreenSaver
{

    public class GeneralCommand : ICommand
    {

        private Action<object> mAction = null;

        private Func<object, bool> mCanExecute = null;

        public event EventHandler CanExecuteChanged;

        public GeneralCommand(Action<object> action, Func<object, bool> canExecute)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (canExecute == null) throw new ArgumentNullException("canExecute");

            mAction = action;
            mCanExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return mCanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            mAction(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

    }

}
