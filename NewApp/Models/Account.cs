using NewApp.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NewApp.Models
{
    
    public class Account
    {   public Account()
        {
            statusVIP = 0;
            pushState = 0;
        }
        //public Account(UserD row)
        //{
        //    id = row.id;
        //    login = row.login;
        //    password = row.password;
        //    statusVIP = row.statusVIP;
        //    pushState = row.pushState;
        //}
        public int id { get; set; }
        [Required(ErrorMessage = "Поле 'логін' обовязкове")]
        public string  login { get; set; }
        [Required(ErrorMessage = "Поле 'пароль' обов'язкове")]
        public string password { get; set; }
        
        [Compare("password", ErrorMessage = "Не співпадає з паролем")]
        public string passConfirm { get; set; }
        public int statusVIP { get; set; }
        public int pushState { get; set; }
    }
}