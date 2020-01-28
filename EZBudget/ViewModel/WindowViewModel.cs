using EZBudget.DataModels;
using EZBudget.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EZBudget.ViewModel
{
    public class WindowViewModel : BaseViewModel
    {
        #region Private properties

        private Window mWindow;

        #endregion

        #region Public properties

        /// <summary>
        /// The current page displayed in window
        /// </summary>
        public ApplicationPageEnum CurrentPage { get; set; } = ApplicationPageEnum.Login;

        /// <summary>
        /// The current loged in user username
        /// </summary>
        public string LogedInUsername { get; set; }

        /// <summary>
        /// The time the user loged in at
        /// </summary>
        public DateTime LogedInTime { get; set; }

        #endregion

        public WindowViewModel(Window mWindow)
        {
            this.mWindow = mWindow;
        }
    }
}
