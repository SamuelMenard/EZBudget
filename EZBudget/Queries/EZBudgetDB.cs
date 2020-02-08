using EZBudget.DataModels;
using EZBudget.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZBudget.Queries
{
    public static class EZBudgetDB
    {
        private static EZBudget_DBEntities context { get; } = new EZBudget_DBEntities();

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
            try
            {
                // Sums the current month budget categories amounts
                return (from user in context.Users
                        where user.UserID == userID
                        select user.Budgets.FirstOrDefault()
                                   .Category_Global
                                   .Where(category => category.IsActive == true)
                                   .Sum(categoryGlobal => (categoryGlobal.Category_Monthly
                                                            .Where(categoryMonthly => categoryMonthly.CreationDate.Year == DateTime.Now.Year
                                                                                        && categoryMonthly.CreationDate.Month == DateTime.Now.Month)
                                                            .FirstOrDefault().Amount))).First();
            }
            catch
            {
                return new decimal(0);
            }
        }

        public static decimal GetCurrentMonthTotalExpenses(int userID)
        {
            try
            {
                return (from user in context.Users
                        where user.UserID == userID
                        select user).FirstOrDefault().Budgets.FirstOrDefault()
                    .Category_Global.Select(x => x.Category_Monthly).SelectMany(x => x)
                    .Select(x => x.Expenses).SelectMany(x => x)
                    .Where(x => x.CreationDate.Month == DateTime.Now.Month && x.CreationDate.Year == DateTime.Now.Year).Sum(x => x.Amount);
            }
            catch
            {
                return new decimal(0);
            }
            
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
                            Expenses = new List<Expense>()
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
                    .SelectMany(y => y).Where(x => ((x.CreationDate.Year == DateTime.Now.Year) ? x.CreationDate.Month < DateTime.Now.Month : true))
                    .GroupBy(
                    expense => new { Month = expense.CreationDate.Month, Year = expense.CreationDate.Year},
                    (key, expenses) => new PastMonth
                    {
                        Date = expenses.FirstOrDefault().CreationDate,
                        TotalExpenses = Math.Round(expenses.Sum(x => x.Amount), 2)
                    }
                    ).OrderByDescending(x => x.Date).ToList();

            }
            catch
            {
                return new List<PastMonth>();
            }
        }

        public static List<PastMonth> GetPastMonthsExpenses_WithRange(int userID, int nbOfMonths)
        {
            try
            {
                var minDate = DateTime.Now.AddMonths(-nbOfMonths);
                minDate = new DateTime(minDate.Year, minDate.Month, 1);

                var maxDate = DateTime.Now;
                maxDate = new DateTime(maxDate.Year, maxDate.Month, 1);

                var globalCategories = (from user in context.Users
                                        where user.UserID == userID
                                        select user).FirstOrDefault().Budgets.FirstOrDefault()
                                              .Category_Global;

                var pastMonthsValues = globalCategories
                    .Select(x => x.Category_Monthly)
                    .SelectMany(y => y)
                    .Select(x => x.Expenses)
                    .SelectMany(y => y).Where(x => x.CreationDate >= minDate && x.CreationDate < maxDate)
                    .GroupBy(
                    expense => new { Month = expense.CreationDate.Month, Year = expense.CreationDate.Year },
                    (key, expenses) => new PastMonth
                    {
                        Date = expenses.FirstOrDefault().CreationDate,
                        TotalExpenses = Math.Round(expenses.Sum(x => x.Amount), 2)
                    }
                    ).ToList();

                var pastMonths = new List<PastMonth>();
                var pastDates = new List<DateTime>();
                for (var i = 1; i <= nbOfMonths; i++)
                    pastDates.Add(DateTime.Now.AddMonths(-i));

                pastDates.ForEach(date =>
                {
                    var pastMonth = pastMonthsValues.FirstOrDefault(x => (x.Date.Year == date.Year) && (x.Date.Month == date.Month));
                    if (pastMonth == null)
                        pastMonths.Add(new PastMonth { Date = date, TotalExpenses = 0 });
                    else
                        pastMonths.Add(pastMonth);


                });

                return pastMonths.OrderBy(x => x.Date).ToList();
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

        public static bool DeleteExpense(int expenseID)
        {
            try
            {
                var expenseToBeDeleted = (from expense in context.Expenses
                                          where expense.ExpenseID == expenseID
                                          select expense).FirstOrDefault();

                if (expenseToBeDeleted == null)
                    return false;

                if (expenseToBeDeleted.ExpenseBillImageUrl != "")
                    DeleteImage(expenseToBeDeleted.ExpenseBillImageUrl);

                context.Expenses.Remove(expenseToBeDeleted);
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

        public static bool CreateExpense(string expenseName, string expenseDescription, decimal expenseAmount, int catID, string receiptURL)
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
                    CategoryMonthlyID = catID,
                    ExpenseBillImageUrl = CopyImage(receiptURL)
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

        public static Expense GetExpense(int expenseID)
        {
            return (from expense in context.Expenses
                    where expense.ExpenseID == expenseID
                    select expense).FirstOrDefault();
        }

        public static bool EditExpense(Expense expense, string name, string description, decimal amount, string url, int catID)
        {
            try
            {
                expense.ExpenseName = name;
                expense.ExpenseDescription = description;
                expense.Amount = amount;
                expense.ExpenseBillImageUrl = CopyImage(url);
                expense.CategoryMonthlyID = catID;
                expense.LastModifDate = DateTime.Now;

                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
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

        public static ObservableCollection<Category> GetCurrentMonthCategoriesWithExpenses(int userID)
        {
            ObservableCollection<Category> categories = new ObservableCollection<Category>();

            var activeGlobalCategories = (from user in context.Users
                                    where user.UserID == userID
                                    select user).FirstOrDefault().Budgets.First()
                       .Category_Global.Where(x => x.IsActive);

            foreach(var globalCategory in activeGlobalCategories)
            {
                var monthlyCategory = globalCategory.Category_Monthly
                    .FirstOrDefault(x => x.CreationDate.Month == DateTime.Now.Month && x.CreationDate.Year == DateTime.Now.Year);

                if (monthlyCategory == null)
                    continue;

                var expenses = monthlyCategory.Expenses.ToList();
                expenses.ForEach(x => x.Amount = Math.Round(x.Amount, 2));

                categories.Add(new Category()
                {
                    GlobalID = globalCategory.CategoryGlobalID,
                    ID = monthlyCategory.CategoryMonthlyID,
                    Name = globalCategory.CategoryName,
                    Description = globalCategory.CategoryDescription,
                    Amount = Math.Round(monthlyCategory.Amount, 2),
                    ColorTag = globalCategory.ColorTag,
                    Expenses = expenses
                });
            }

            return categories;
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

                    if (lastMonthlyCategory != null)
                    {
                        globalCategory.Category_Monthly.Add(new Category_Monthly()
                        {
                            Amount = lastMonthlyCategory.Amount,
                            CreationDate = DateTime.Now,
                            LastModifDate = DateTime.Now,
                            Expenses = new HashSet<Expense>()
                        });
                    }
                    else
                    {
                        globalCategory.Category_Monthly.Add(new Category_Monthly()
                        {
                            Amount = new decimal(0),
                            CreationDate = DateTime.Now,
                            LastModifDate = DateTime.Now,
                            Expenses = new HashSet<Expense>()
                        });
                    }
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

        private static string CopyImage(string imageURL)
        {
            var targetPath = @"C:\Users\Samuel Ménard\repos\EZBudget\EZBudget\Images\Bills";

            try
            {
                var fileName = Path.GetFileName(imageURL);

                if (fileName == "")
                    return "";

                fileName = $"{DateTime.Now.Ticks.ToString()}_{fileName}";

                string destFile = Path.Combine(targetPath, fileName);
                Directory.CreateDirectory(targetPath);
                File.Copy(imageURL, destFile, true);
                return destFile;
            }
            catch
            {
                return "";
            }
            
        }

        private static bool DeleteImage(string imageURL) {
            try
            {
                if (!File.Exists(imageURL))
                    return false;

                File.Delete(imageURL);
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
