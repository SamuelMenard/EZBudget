using EZBudget.DataModels;
using EZBudget.Entity;
using EZBudget.PopupForms;
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
using LiveCharts;
using LiveCharts.Wpf;

namespace EZBudget.ViewModel.SidebarViewModels
{
    class ChartsViewModel : BaseViewModel
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

        #region Charts properties

        // properties for the past months chart ******** //
        public SeriesCollection PastMonths_SeriesCollection { get; set; }
        public string[] PastMonths_Labels { get; set; }
        public Func<double, string> PastMonths_YFormatter { get; set; }
        // **************************************** //

        #endregion

        #region Commands 

        public ICommand DeleteExpenseClick { get; set; }
        public ICommand EditExpenseClick { get; set; }
        public ICommand AddExpenseClick { get; set; }

        #endregion

        #region Constructor

        public ChartsViewModel(string username)
        {
            // Set LogedInUsername
            LogedInUsername = username;

            // Set commands
            DeleteExpenseClick = new RelayParameterizedCommand(parameter => DeleteExpense_Click(parameter));
            EditExpenseClick = new RelayParameterizedCommand(parameter => EditExpense_Click(parameter));
            AddExpenseClick = new RelayCommand(() => AddExpense_Click());

            // Populates the view model
            PopulateViewModel();

            Init_PastMonthsExpenses_Chart(6);
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
            int expenseID = (int)parameter;

            // Get expense
            var expense = EZBudgetDB.GetExpense(expenseID);

            if (expense == null)
                return;

            // Clone expense
            var clonedExpense = new Expense()
            {
                ExpenseName = expense.ExpenseName,
                ExpenseDescription = expense.ExpenseDescription,
                ExpenseBillImageUrl = expense.ExpenseBillImageUrl,
                Amount = expense.Amount
            };

            // Open temporary edit expense form
            EditExpenseWindow form = new EditExpenseWindow(LogedInUserID, clonedExpense);
            form.ShowDialog();

            // If expense is valid, edit and push changes to DB
            if (form._mViewModel.IsExpenseValid)
            {
                EZBudgetDB.EditExpense(expense, form._mViewModel.ExpenseName, form._mViewModel.ExpenseDescription,
                    Math.Round(decimal.Parse(form._mViewModel.ExpenseAmount), 2), form._mViewModel.ReceiptUrl,
                    form._mViewModel.selectedCategoryMonthlyId);

                // TO BE CHANGED
                PopulateViewModel();
            }
        }

        private void AddExpense_Click()
        {
            // Open temporary add expense form
            AddExpenseWindow form = new AddExpenseWindow(LogedInUserID);
            form.ShowDialog();

            // If expense is valid, add it to the DB
            if (form._mViewModel.IsExpenseValid)
            {
                // Create expense
                EZBudgetDB.CreateExpense(form._mViewModel.ExpenseName, form._mViewModel.ExpenseDescription,
                    Math.Round(decimal.Parse(form._mViewModel.ExpenseAmount), 2), form._mViewModel.selectedCategoryMonthlyId,
                    form._mViewModel.ReceiptUrl);

                // TO BE CHANGED
                PopulateViewModel();
            }
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
                Categories = EZBudgetDB.GetCurrentMonthBudgetCategories(LogedInUserID);
            }

            // Get and set "Total monthly expenses"
            TotalExpenses = Math.Round(EZBudgetDB.GetCurrentMonthTotalExpenses(LogedInUserID), 2);

            // Get and set "Total monthly budgeted"
            TotalBudgeted = Math.Round(EZBudgetDB.GetTotalMonthlyBudgeted(LogedInUserID), 2);
        }

        private void Init_PastMonthsExpenses_Chart(int nbPastMonths)
        {
            // Get and set the "Past months"
            var pastMonths = EZBudgetDB.GetPastMonthsExpenses_WithRange(LogedInUserID, nbPastMonths);

            var values = new ChartValues<double>();
            var labels = new List<string>();
            pastMonths.ForEach(x => {
                values.Add(decimal.ToDouble(x.TotalExpenses));
                labels.Add($"{x.Month} {x.Year}");
            });

            PastMonths_SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Past months expenses",
                    Values =  values,
                    LineSmoothness = 1 //0: straight lines, 1: really smooth lines
                },
            };

            PastMonths_Labels = labels.ToArray();
            PastMonths_YFormatter = value => value.ToString("C");
        }

        #endregion
    }
}
