using EZBudget.DataModels;
using EZBudget.Queries;
using EZBudget.ViewModel.Base;
using EZBudget.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace EZBudget.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {

        #region Private properties

        private WindowViewModel windowViewModel { get; } = ((WindowViewModel)((MainWindow)App.Current.MainWindow).DataContext);

        #endregion

        #region Public properties

        public string Username { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsLoginRunning { get; set; }

        #endregion

        #region Commands

        public ICommand LoginClick { get; set; }

        #endregion

        public LoginViewModel()
        {
            this.LoginClick = new RelayParameterizedCommand(async (parameter) => await Login_Click(parameter));
        }

        public async Task Login_Click(Object parameter)
        {
            var passwordBox = (PasswordBox)parameter;

            //TODO: Validate username and password
            switch (await EZBudgetDB.TryLogingIn(Username, passwordBox.Password))
            {
                case ErrorCodesEnum.ConnectionOK:
                    ErrorMessage = "";
                    SetWindowViewModelBaseVariables(Username);
                    ChangePage(ApplicationPageEnum.Main);
                    break;
                case ErrorCodesEnum.PasswordWrongFormat:
                    ErrorMessage = "Incorrect password format";
                    break;
                case ErrorCodesEnum.UsernameWrongFormat:
                    ErrorMessage = "Incorrect username format";
                    break;
                case ErrorCodesEnum.UsernameAndPasswordWrongFormat:
                    ErrorMessage = "Incorrect username and password format";
                    break;
                case ErrorCodesEnum.UsernameOrPasswordInvalid:
                    ErrorMessage = "Invalid username or password";
                    break;
                case ErrorCodesEnum.UnknownError:
                    ErrorMessage = "Unknown error";
                    break;
            }
        }

        private void ChangePage(ApplicationPageEnum page)
        {
            windowViewModel.CurrentPage = page;
        }

        private void SetWindowViewModelBaseVariables(string username)
        {
            windowViewModel.LogedInUsername = username;
            windowViewModel.LogedInTime = DateTime.Now;
        }
    }
}
