using EZBudget.DataModels;
using EZBudget.ViewModel.Base;
using EZBudget.ViewModel.Commands;
using EZBudget.ViewModel.SidebarViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EZBudget.ViewModel
{
    public class MainPageViewModel : BaseViewModel
    {

        #region Private properties

        private WindowViewModel windowViewModel { get; } = ((WindowViewModel)((MainWindow)App.Current.MainWindow).DataContext);

        #endregion

        #region Public properties

        public BaseViewModel CurrentViewModel { get; set; }

        public string LogedInUsername { get; set; }

        public DateTime CurrentDate { get; set; }

        #endregion

        #region Commands

        public ICommand DashboardClick { get; set; }
        public ICommand MyBudgetClick { get; set; }
        public ICommand MyExpensesClick { get; set; }
        public ICommand SignOutClick { get; set; }

        #endregion

        #region Constructor

        public MainPageViewModel()
        {
            // Set loged in user username from windowViewModel
            LogedInUsername = windowViewModel.LogedInUsername;

            // Set current date
            CurrentDate = DateTime.Now;

            // Set default view (Dashboard)
            CurrentViewModel = new DashboardViewModel(LogedInUsername);

            DashboardClick = new RelayCommand(() => Dashboard_Click());
            MyBudgetClick = new RelayCommand(() => MyBudget_Click());
            MyExpensesClick = new RelayCommand(() => MyExpenses_Click());
            SignOutClick = new RelayCommand(() => SignOut_Click());
        }

        #endregion

        #region Private methods

        private void ChangePage(ApplicationPageEnum page)
        {
            windowViewModel.CurrentPage = page;
        }

        #endregion

        #region Action methods

        private void Dashboard_Click()
        {
            CurrentViewModel = new DashboardViewModel(LogedInUsername);
        }

        private void MyBudget_Click()
        {
            CurrentViewModel = new MyBudgetViewModel(LogedInUsername);
        }

        private void MyExpenses_Click()
        {
            CurrentViewModel = new MyExpensesViewModel(LogedInUsername);
        }

        private void SignOut_Click()
        {
            // Go back to login
            ChangePage(ApplicationPageEnum.Login);
        }

        #endregion
    }
}
