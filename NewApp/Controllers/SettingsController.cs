using MySql.Data.MySqlClient;
using NewApp.Hubs;
using NewApp.Models;
using NewApp.Models.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace NewApp.Controllers
{
    public class SettingsController : Controller
    {
        public ActionResult Settings()
        {
            int vip = 0;
            DB db = new DB();
            string TempId = Request.Cookies["accountId"].Value;
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT *FROM user WHERE accountId=@usId AND statusVIP=1", db.getConnection());
            command.Parameters.Add("@usId", MySqlDbType.VarChar).Value = Convert.ToInt16(TempId);
            adapter.SelectCommand = command;
            adapter.Fill(table);
            if (table.Rows.Count == 1)
            {
                vip = 1;
            }
            ViewBag.VIP = vip;

            return View();
        }


        public ActionResult getSettings()
        {
            string TempId = Request.Cookies["accountId"].Value;
            List<VIP_User> contList = new List<VIP_User>();
            DB db = new DB();

            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT *FROM settingsmeet WHERE accId=@usId ", db.getConnection());
            command.Parameters.Add("@usId", MySqlDbType.VarChar).Value = Convert.ToInt16(TempId);
            adapter.SelectCommand = command;
            adapter.Fill(table);
            if (table.Rows.Count == 1)
            {
                foreach (DataRow r in table.Rows)
                {
                    VIP_User temCont = new VIP_User();
                    temCont.accId = Convert.ToInt16(r["accId"]);
                    temCont.timer = Convert.ToInt16(r["timer"]);
                    temCont.allM = Convert.ToInt32(r["allM"]);
                    temCont.email = Convert.ToString(r["email"]);
                    contList.Add(temCont);

                }
            }

            return Json(contList.AsEnumerable().Select(e => new {

                hour = e.timer/60,
                minute = e.timer%60,
                allM = e.allM,
                email = e.email,
            }
               ).ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult setSettings(VIP_User e)
        {
            bool status = false;
            e.accId = Convert.ToInt16(Request.Cookies["accountId"].Value);
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT *FROM settingsmeet WHERE accId=@uId", db.getConnection());
            command.Parameters.Add("@uId", MySqlDbType.VarChar).Value = e.accId;
          
            adapter.SelectCommand = command;
            adapter.Fill(table);
            if (table.Rows.Count == 0)
            {
                db = new DB();
                table = new DataTable();
                adapter = new MySqlDataAdapter();
                command = new MySqlCommand("INSERT INTO `settingsmeet` (`accId`,`timer`, `allM`,`email`) VALUES (@uIdU,@uTim,  @uAll, @uEm)", db.getConnection());
                command.Parameters.Add("@uIdU", MySqlDbType.VarChar).Value = e.accId;
                command.Parameters.Add("@uTim", MySqlDbType.VarChar).Value = e.timer;
                command.Parameters.Add("@uAll", MySqlDbType.VarChar).Value = e.allM;
                command.Parameters.Add("@uEm", MySqlDbType.VarChar).Value = e.email;
                db.openConnection();
                if (command.ExecuteNonQuery() == 1)
                {
                    status = true;
                }
                db.closeConnection();
            }
            else
            {
                db = new DB();
                command = new MySqlCommand("UPDATE `settingsmeet` SET  timer=@uTim, allM=@uAll, email=@uEm WHERE  accId=@uIdU  ", db.getConnection());
                command.Parameters.Add("@uIdU", MySqlDbType.VarChar).Value = e.accId;
                command.Parameters.Add("@uTim", MySqlDbType.VarChar).Value = e.timer ;
                command.Parameters.Add("@uAll", MySqlDbType.VarChar).Value = e.allM;
                command.Parameters.Add("@uEm", MySqlDbType.VarChar).Value = e.email;
                db.openConnection();
                if (command.ExecuteNonQuery() == 1)
                {
                    db.closeConnection();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };

        }

        [HttpPost]
        public ActionResult setPush(int e)
        {
            bool status = false;
            int accId = Convert.ToInt16(Request.Cookies["accountId"].Value);
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("UPDATE `user` SET  pushState=@uPu WHERE  accountId=@uIdU  ", db.getConnection());
            command.Parameters.Add("@uIdU", MySqlDbType.VarChar).Value = accId;
            command.Parameters.Add("@uPu", MySqlDbType.VarChar).Value = e;

            db.openConnection();
            if (command.ExecuteNonQuery() == 1)
            {
                status = true;
            }
            db.closeConnection();
            return new JsonResult { Data = new { status = status } };

        }
        public ActionResult getPush()
        {
            int pushState = 0;
           
            int accId = Convert.ToInt16(Request.Cookies["accountId"].Value);
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT *FROM user WHERE accountId=@usId AND statusVIP=1", db.getConnection());
            command.Parameters.Add("@usId", MySqlDbType.VarChar).Value = Convert.ToInt16(accId);
            adapter.SelectCommand = command;
            adapter.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                pushState = Convert.ToInt32(r["pushState"]);
            }


            return Json(new {
                pushState = pushState,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult deleteAcc()
        {

            bool status = false;
            int accId = Convert.ToInt16(HttpContext.Request.Cookies["accountId"].Value);
            List<Contacts> contList = new List<Contacts>();
            DB db = new DB();
            MySqlCommand command = new MySqlCommand("DELETE FROM user WHERE accountId=@uIdC ", db.getConnection());
            command.Parameters.Add("@uIdC", MySqlDbType.VarChar).Value = accId;
            db.openConnection();
            if (command.ExecuteNonQuery() == 1)
            {
                db.closeConnection();
                status = true;
               
            }
            else { db.closeConnection(); }
            return new JsonResult { Data = new { status = status } };

        }
        }
    }