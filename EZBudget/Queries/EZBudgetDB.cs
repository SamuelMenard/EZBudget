using EZBudget.DataModels;
using EZBudget.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZBudget.Queries
{
    public static class EZBudgetDB
    {
        private static EZBudget_DBEntities1 context { get; } = new EZBudget_DBEntities1();

        public static async Task<ErrorCodesEnum> TryLogingIn(string username, string password)
        {
            // Validate credentials format
            if (Validators.UserNameValidator(username) && Validators.PasswordValidator(password))
            {
                // Validates with DB that credentials are valid
                bool IsCredentialsValid = false;
                /*await Task.Run(() =>
                {
                    IsCredentialsValid = ((from user in context.Users
                                                where user.UserName == username && user.UserPassword == password
                                                select user).Count()) > 0;
                });*/

                var lst = await ((from user in context.Users
                                       where user.UserName == username && user.UserPassword == password
                                       select user).ToArrayAsync());
                IsCredentialsValid = (lst.Length > 0) ? true : false;



                if (IsCredentialsValid)
                    return ErrorCodesEnum.ConnectionOK;
                else
                    return ErrorCodesEnum.UsernameOrPasswordInvalid;
            }
            else
            {
                // Finds which element format is wrong
                if (!Validators.UserNameValidator(username) && !Validators.PasswordValidator(password))
                    return ErrorCodesEnum.UsernameAndPasswordWrongFormat;
                else if (!Validators.UserNameValidator(username))
                    return ErrorCodesEnum.UsernameWrongFormat;
                else if (!Validators.PasswordValidator(password))
                    return ErrorCodesEnum.PasswordWrongFormat;
                else
                    return ErrorCodesEnum.UnknownError;
            }
        }

        public static int GetUserID(string username)
        {
            return (int)(from user in context.Users
                    where user.UserName == username
                    select user.UserID).First();
        }

        public static decimal GetTotalMonthlyBudgeted(int userID)
        {
            // Sums the current month budget categories amounts
            var totalBudgeted = (from user in context.Users
                                where user.UserID == userID
                                select user.Budgets.FirstOrDefault()
                                           .Category_Global
                                           .Where(category => category.IsActive == true)
                                           .Sum(categoryGlobal => (categoryGlobal.Category_Monthly
                                                                    .Where(categoryMonthly => categoryMonthly.CreationDate.Year == DateTime.Now.Year 
                                                                                                && categoryMonthly.CreationDate.Month == DateTime.Now.Month)
                                                                    .FirstOrDefault().Amount))).First();

            return totalBudgeted;
        }

        public static ObservableCollection<Category> GetCurrentMonthBudgetCategories(int userID)
        {
            ObservableCollection<Category> Categories = new ObservableCollection<Category>();

            try
            {
                var activeGlobalCategories = (from user in context.Users
                                              where user.UserID == userID
                                              select user.Budgets.FirstOrDefault()
                                                           .Category_Global.Where(categoryGlobal => categoryGlobal.IsActive == true));

                foreach (var categoryGlobal in activeGlobalCategories.First())
                {
                    var globalCategory = (Category_Global)categoryGlobal;
                    // Get the current month category
                    Category_Monthly currentMonthCategory = globalCategory.Category_Monthly.Where(categoryMonthly => categoryMonthly.CreationDate.Year == DateTime.Now.Year
                                                                                              && categoryMonthly.CreationDate.Month == DateTime.Now.Month)
                                                                    .FirstOrDefault();

                    // Add category to Collection
                    if (currentMonthCategory != null)
                    {
                        Categories.Add(new Category()
                        {
                            ID = currentMonthCategory.CategoryMonthlyID,
                            GlobalID = globalCategory.CategoryGlobalID,
                            Name = globalCategory.CategoryName,
                            Description = globalCategory.CategoryDescription,
                            ColorTag = globalCategory.ColorTag,
                            Amount = Math.Round(currentMonthCategory.Amount, 2),
                            Expenses = new ObservableCollection<Expense>()
                        });
                    }
                }
                return Categories;
            }
            catch
            {
                return Categories;
            }
        }

        public static List<PastMonth> GetPastMonthsExpenses(int userID)
        {
            try
            {
                var globalCategories = (from user in context.Users
                                              where user.UserID == userID
                                              select user).FirstOrDefault().Budgets.FirstOrDefault()
                                              .Category_Global;

                return globalCategories
                    .Select(x => x.Category_Monthly)
                    .SelectMany(y => y)
                    .Select(x => x.Expenses)
                    .SelectMany(y => y)
                    .GroupBy(
                    expense => new { Month = expense.CreationDate.Month, Year = expense.CreationDate.Year},
                    (key, expenses) => new PastMonth
                    {
                        Date = expenses.FirstOrDefault().CreationDate,
                        TotalExpenses = Math.Round(expenses.Sum(x => x.Amount), 2)
                    }
                    ).ToList();

            }
            catch
            {
                return new List<PastMonth>();
            }
        }

        public static bool DeleteCategory(int categoryID)
        {
            try
            {
                var monthlyCategory = (from categoryMonthly in context.Category_Monthly
                                       where categoryMonthly.CategoryMonthlyID == categoryID
                                       select categoryMonthly).First();

                var globalCategory = (from categoryGlobal in context.Category_Global
                                      where categoryGlobal.CategoryGlobalID == monthlyCategory.CategoryGlobalID
                                      select categoryGlobal).First();

                var nbOfMonthlyCategories = (int)globalCategory.Category_Monthly.Count();

                // If there is more than one monthly category, change global category status to "Not active", if not delete global category
                if (nbOfMonthlyCategories > 1)
                {
                    // Move category expenses to "Others" category


                    // Delete monthly category
                    context.Category_Monthly.Remove(monthlyCategory);

                    // Change global category status
                    globalCategory.IsActive = false;
                }
                else
                {
                    // Move category expenses to "Others" category


                    // Delete monthly category and global
                    context.Category_Monthly.Remove(monthlyCategory);
                    context.Category_Global.Remove(globalCategory);
                }

                // Apply changes
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CreateCategory(int userID, string catName, string catDescription, string catColorTag, decimal catAmount)
        {
            try
            {
                // Get user
                var logedInUser = (User)(from user in context.Users
                                   where user.UserID == userID
                                   select user).First();

                // Add global category
                var globalCategory = new Category_Global()
                {
                    CategoryName = catName,
                    CategoryDescription = catDescription,
                    ColorTag = catColorTag,
                    Category_Monthly = new HashSet<Category_Monthly>(),
                    IsActive = true
                };

                globalCategory.Category_Monthly.Add(new Category_Monthly()
                {
                    Amount = catAmount,
                    CreationDate = DateTime.Now,
                    LastModifDate = DateTime.Now
                });

                logedInUser.Budgets.FirstOrDefault().Category_Global.Add(globalCategory);

                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CreateExpense(string expenseName, string expenseDescription, decimal expenseAmount, int catID)
        {
            try
            {
                // Add new expense
                context.Expenses.Add(new Expense()
                {
                    ExpenseName = expenseName,
                    ExpenseDescription = expenseDescription,
                    Amount = expenseAmount,
                    CreationDate = DateTime.Now,
                    LastModifDate = DateTime.Now,
                    CategoryMonthlyID = catID
                });

                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Category_Global GetCategoryGlobal(int categoryID)
        {
            try
            {
                return (from category in context.Category_Global
                        where category.CategoryGlobalID == categoryID
                        select category).First();
            }
            catch
            {
                return null;
            }
        }

        public static Category_Monthly GetCategoryMonthly(int categoryID)
        {
            try
            {
                return (from category in context.Category_Monthly
                        where category.CategoryMonthlyID == categoryID
                        select category).First();
            }
            catch
            {
                return null;
            }
        }

        public static ObservableCollection<Category> GetDisabledCategpries(int userID)
        {
            ObservableCollection<Category> lstDisabledCategories = new ObservableCollection<Category>();

            var disabledGlobalCategories = (from user in context.Users
                                          where user.UserID == userID
                                          select user.Budgets.FirstOrDefault()
                                                       .Category_Global.Where(categoryGlobal => categoryGlobal.IsActive == false)).First();

            foreach (var category in disabledGlobalCategories)
            {
                lstDisabledCategories.Add(new DataModels.Category()
                {
                    GlobalID = category.CategoryGlobalID,
                    Name = category.CategoryName,
                    Description = category.CategoryDescription,
                    ColorTag = category.ColorTag
                });
            }

            return lstDisabledCategories;
        }

        public static bool RestoreCategory(int globalCategoryID)
        {
            try
            {
                // Activate the category
                var category = (from categoryGlobal in context.Category_Global
                                where categoryGlobal.CategoryGlobalID == globalCategoryID
                                select categoryGlobal).FirstOrDefault();
                category.IsActive = true;

                // Create new monthly category
                category.Category_Monthly.Add(new Category_Monthly()
                {
                    Amount = new decimal(0),
                    CreationDate = DateTime.Now,
                    LastModifDate = DateTime.Now
                });

                // save changes
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool NewMonthCategoriesProcedure(int userID)
        {
            try
            {
                var activeGlobalCategories = (from user in context.Users
                                              where user.UserID == userID
                                              select user.Budgets.FirstOrDefault()
                                                           .Category_Global.Where(categoryGlobal => categoryGlobal.IsActive == true)).First();

                foreach (var globalCategory in activeGlobalCategories)
                {
                    var lastMonthlyCategory = (globalCategory.Category_Monthly
                                                .OrderByDescending(monthlyCategory => monthlyCategory.CreationDate).FirstOrDefault());
                    globalCategory.Category_Monthly.Add(new Category_Monthly()
                    {
                        Amount = lastMonthlyCategory.Amount,
                        CreationDate = DateTime.Now,
                        LastModifDate = DateTime.Now,
                        Expenses = new HashSet<Expense>()
                    });
                }

                // save changes
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SaveChanges()
        {
            try
            {
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
            
        }

    }
}
