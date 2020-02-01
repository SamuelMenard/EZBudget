using EZBudget.DataModels;
using EZBudget.Queries;
using EZBudget.ViewModel.Base;
using EZBudget.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EZBudget.ViewModel.SidebarViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        #region Private properties

        private int LogedInUserID { get; set; }

        #endregion

        #region Public properties

        public string LogedInUsername { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalBudgeted { get; set; }

        public ObservableCollection<Category> Categories { get; set; }

        #endregion

        #region Commands 

        public ICommand DeleteExpenseClick { get; set; }
        public ICommand EditExpenseClick { get; set; }
        public ICommand AddExpenseClick { get; set; }

        #endregion

        #region Constructor

        public DashboardViewModel(string username)
        {
            // Set LogedInUsername
            LogedInUsername = username;

            // Set commands
            DeleteExpenseClick = new RelayParameterizedCommand(parameter => DeleteExpense_Click(parameter));
            EditExpenseClick = new RelayParameterizedCommand(parameter => EditExpense_Click(parameter));
            AddExpenseClick = new RelayCommand(() => AddExpense_Click());

            // Populates the view model
            PopulateViewModel();
        }

        #endregion

        #region Commands methods

        private void DeleteExpense_Click(Object parameter)
        {
            int expenseID = (int)parameter;
            EZBudgetDB.DeleteExpense(expenseID);
            PopulateViewModel();
        }

        private void EditExpense_Click(Object parameter)
        {
            
        }

        private void AddExpense_Click()
        {
            
        }

        #endregion

        #region Private methods

        private void PopulateViewModel()
        {
            // Get loged in user ID
            LogedInUserID = EZBudgetDB.GetUserID(LogedInUsername);

            // Get and set the "Categories"
            Categories = EZBudgetDB.GetCurrentMonthCategoriesWithExpenses(LogedInUserID);

            // If it is a new month
            if (Categories.Count == 0)
            {
                // Recreate all the categories from last month
                EZBudgetDB.NewMonthCategoriesProcedure(LogedInUserID);

                // Get and set the budget "Categories"
                Categories = EZBudgetDB.GetCurrentMonthCategoriesWithExpenses(LogedInUserID);
            }

            // Get and set "Total monthly expenses"
            TotalExpenses = Math.Round(EZBudgetDB.GetCurrentMonthTotalExpenses(LogedInUserID), 2);

            // Get and set "Total monthly budgeted"
            TotalBudgeted = Math.Round(EZBudgetDB.GetTotalMonthlyBudgeted(LogedInUserID), 2);
        }

        #endregion
    }
}
