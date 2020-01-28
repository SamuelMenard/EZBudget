using EZBudget.DataModels;
using EZBudget.Entity;
using EZBudget.PopupForms;
using EZBudget.Queries;
using EZBudget.ViewModel.Base;
using EZBudget.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EZBudget.ViewModel.SidebarViewModels
{
    public class MyBudgetViewModel : BaseViewModel
    {
        #region Private properties

        private int LogedInUserID { get; set; }

        #endregion

        #region Public properties

        public string LogedInUsername { get; set; }
        public decimal TotalBudgeted { get; set; }

        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Category> DisabledCategories { get; set; }

        #endregion

        #region Commands

        public ICommand DeleteCategoryClick { get; set; }
        public ICommand EditCategoryClick { get; set; }
        public ICommand AddCategoryClick { get; set; }
        public ICommand RestoreCategoryClick { get; set; }

        #endregion

        #region Constructor

        public MyBudgetViewModel(string username)
        {
            // Set LogedInUsername
            LogedInUsername = username;

            // Set commands
            DeleteCategoryClick = new RelayParameterizedCommand(parameter => DeleteCategory_Click(parameter));
            EditCategoryClick = new RelayParameterizedCommand(parameter => EditCategory_Click(parameter));
            AddCategoryClick = new RelayCommand(() => AddCategory_Click());
            RestoreCategoryClick = new RelayParameterizedCommand(parameter => RestoreCategory_Click(parameter));

            // Populates the view model
            PopulateViewModel();
        }

        #endregion

        #region Commands methods

        private void DeleteCategory_Click(Object parameter)
        {
            int categoryID = (int)parameter;
            EZBudgetDB.DeleteCategory(categoryID);

            // TO BE CHANGED
            PopulateViewModel();
        }

        private void EditCategory_Click(Object parameter)
        {
            int categoryID = (int)parameter;
            EditCategoryWindow form = new EditCategoryWindow(categoryID);
            form.ShowDialog();

            if (form.MViewModel.IsCategoryValid)
            {
                form.editingCategoryGlobal.CategoryName = form.MViewModel.CategoryName;
                form.editingCategoryGlobal.CategoryDescription = form.MViewModel.CategoryDescription;
                form.editingCategoryGlobal.ColorTag = form.MViewModel.CategoryColorTag;
                form.editingCategoryMonthly.Amount = Math.Round(decimal.Parse(form.MViewModel.CategoryAmount), 2);
                form.editingCategoryMonthly.LastModifDate = DateTime.Now;

                EZBudgetDB.SaveChanges();

                // TO BE CHANGED
                PopulateViewModel();
            }
        }

        private void AddCategory_Click()
        {
            // Open temporary add category form
            AddCategoryWindow form = new AddCategoryWindow();
            form.ShowDialog();

            // If category is valid, at it to the DB
            if (form.MViewModel.IsCategoryValid)
            {
                // Create category
                EZBudgetDB.CreateCategory(LogedInUserID, form.MViewModel.CategoryName, 
                    form.MViewModel.CategoryDescription, form.MViewModel.CategoryColorTag, 
                    Math.Round(decimal.Parse(form.MViewModel.CategoryAmount), 2));

                // TO BE CHANGED
                PopulateViewModel();
            }
        }

        private void RestoreCategory_Click(Object parameter)
        {
            int categoryID = (int)parameter;
            EZBudgetDB.RestoreCategory(categoryID);

            // TO BE CHANGED
            PopulateViewModel();
        }

        #endregion

        #region Private methods

        private void PopulateViewModel()
        {
            // Get loged in user ID
            LogedInUserID = EZBudgetDB.GetUserID(LogedInUsername);

            // Get and set the budget "Categories"
            Categories = EZBudgetDB.GetCurrentMonthBudgetCategories(LogedInUserID);
            
            // If it is a new month
            if (Categories.Count == 0)
            {
                // Recreate all the categories from last month
                EZBudgetDB.NewMonthCategoriesProcedure(LogedInUserID);

                // Get and set the budget "Categories"
                Categories = EZBudgetDB.GetCurrentMonthBudgetCategories(LogedInUserID);
            }

            // Get and set "Total monthly budgeted"
            TotalBudgeted = Math.Round(EZBudgetDB.GetTotalMonthlyBudgeted(LogedInUserID), 2);

            // Get and set the budget "DisabledCategories"
            DisabledCategories = EZBudgetDB.GetDisabledCategpries(LogedInUserID);
        }

        #endregion
    }
}
