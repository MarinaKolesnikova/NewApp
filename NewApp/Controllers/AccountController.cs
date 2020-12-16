using MySql.Data.MySqlClient;
using NewApp.Models;
using NewApp.Models.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace NewApp.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Registration()
        {

            return View();
        }

        public ActionResult Verific(Account account)
        {
            DB db = new DB();
            try
            {
                DataTable table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                MySqlCommand command = new MySqlCommand("SELECT *FROM user WHERE login=@uLog AND password=@uPs", db.getConnection());
                command.Parameters.Add("@uLog", MySqlDbType.VarChar).Value = account.login;
                command.Parameters.Add("@uPs", MySqlDbType.VarChar).Value = account.password;
                adapter.SelectCommand = command;
                adapter.Fill(table);
                if (table.Rows.Count == 0)
                {
                    command = new MySqlCommand("INSERT INTO `user` (`login`, `password`, `statusVIP`, `pushState`) VALUES (@ulog, @upass, @uVip, @uPush)", db.getConnection());
                    command.Parameters.Add("@ulog", MySqlDbType.VarChar).Value = account.login;
                    command.Parameters.Add("@upass", MySqlDbType.VarChar).Value = account.password;
                    command.Parameters.Add("@uVip", MySqlDbType.VarChar).Value = account.statusVIP;
                    command.Parameters.Add("@uPush", MySqlDbType.VarChar).Value = account.pushState;
                    db.openConnection();
                    if (command.ExecuteNonQuery() == 1)
                    {

                        table = new DataTable();
                        adapter = new MySqlDataAdapter();
                        command = new MySqlCommand("SELECT *FROM user WHERE login=@uLog AND password=@uPass ", db.getConnection());
                        command.Parameters.Add("@uLog", MySqlDbType.VarChar).Value = account.login;
                        command.Parameters.Add("@uPass", MySqlDbType.VarChar).Value = account.password;
                        adapter.SelectCommand = command;
                        adapter.Fill(table);
                        if (table.Rows.Count == 1)
                        {
                            HttpContext.Response.Cookies["accountId"].Value = Convert.ToString(table.Rows[0]["accountId"]);

                        }
                        db.closeConnection();

                        return RedirectToAction("Index", "Home");
                        //return $"вас зареєстровано успішно"; ;
                    }

                    else
                    {
                        db.closeConnection();
                        ViewData["Message"] = "Проблеми з сервером, спробуйте ще раз";
                        return View("~/Views/Account/Registration.cshtml");
                    }
                }
                else
                {
                    ViewData["Message"] = "Користувач з таким логіном вже існує";
                    return View("~/Views/Account/Registration.cshtml");
                }
            }
            catch
            {
                ViewData["Message"] = "Проблеми з сервером, спробуйте ще раз";
                return View("~/Views/Account/Registration.cshtml");
            }
        }

        public ActionResult Authorization()
        {

            return View();
        }

        public ActionResult LogIn(Account account)
        {
            DB db = new DB();
            try
            {
                DataTable table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                MySqlCommand command = new MySqlCommand("SELECT *FROM user WHERE login=@uLog AND password=@uPs", db.getConnection());
                command.Parameters.Add("@uLog", MySqlDbType.VarChar).Value = account.login;
                command.Parameters.Add("@uPs", MySqlDbType.VarChar).Value = account.password;
                adapter.SelectCommand = command;
                adapter.Fill(table);
                if (table.Rows.Count == 1)
                {
                    Response.Cookies["accountId"].Value = Convert.ToString(table.Rows[0]["accountId"]);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["Message"] = "Неправильний логін або пароль";
                    return View("~/Views/Account/Authorization.cshtml");
                }
            }
            catch
            {
                ViewData["Message"] = "Проблеми з сервером, спробуйте ще раз";
                return View("~/Views/Account/Authorization.cshtml");
            }
        }
    }
}