using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EZBudget.DataModels
{
    public static class Validators
    {
        /// <summary>
        /// Validates an email format
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool EmailValidator(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return (addr.Address == email);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates a password format
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool PasswordValidator(string password)
        {
            if (String.IsNullOrEmpty(password))
                return false;

            try
            {
                return new Regex(@"^\S{8,18}$").IsMatch(password);
            }
            catch
            {
                return false;
            }
            
        }

        /// <summary>
        /// Validates a username format
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static bool UserNameValidator(string username)
        {
            if (String.IsNullOrEmpty(username))
                return false;

            try
            {
                return new Regex(@"^\S{5,18}$").IsMatch(username);
            }
            catch
            {
                return false;
            }
        }
    }
}
