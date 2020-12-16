using MySql.Data.MySqlClient;
using NewApp.Models;
using NewApp.Models.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<MyTask> contList = new List<MyTask>();
            DB db = new DB();
            int vip = 0;
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT *FROM task WHERE idAccT=@uIdU ", db.getConnection());
            command.Parameters.Add("@uIdU", MySqlDbType.VarChar).Value = Convert.ToInt16(Request.Cookies["accountId"].Value);
            adapter.SelectCommand = command;
            adapter.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                MyTask temCont = new MyTask();
                temCont.TaskId = Convert.ToInt16(r["idTask"]);
                temCont.title = Convert.ToString(r["title"]);
                temCont.startDateT = Convert.ToDateTime(Convert.ToString(r["startDateT"]));
                temCont.endDateT = Convert.ToDateTime(Convert.ToString(r["endDateT"]));
                temCont.descriptionT = Convert.ToString(r["descriptionT"]);

                contList.Add(temCont);
            }
            db.closeConnection();
            table = new DataTable();
            command = new MySqlCommand("SELECT *FROM user WHERE accountId=@uIdU", db.getConnection());
            command.Parameters.Add("@uIdU", MySqlDbType.VarChar).Value = Convert.ToInt16(Request.Cookies["accountId"].Value);
            adapter.SelectCommand = command;
            adapter.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                vip = Convert.ToInt32(r["statusVIP"]);
            }
            ViewBag.VIP = vip;
            return View(contList);

        }
        public JsonResult getJSData()
        {
            List<MyTask> contList = new List<MyTask>();
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT *FROM task WHERE idAccT=@uIdU ", db.getConnection());
            command.Parameters.Add("@uIdU", MySqlDbType.VarChar).Value = Convert.ToInt16(Request.Cookies["accountId"].Value);
            adapter.SelectCommand = command;
            adapter.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                MyTask temCont = new MyTask();
                temCont.TaskId = Convert.ToInt16(r["idTask"]);
                temCont.title = Convert.ToString(r["title"]);
                temCont.startDateT = Convert.ToDateTime(Convert.ToString(r["startDateT"]));
             
                temCont.endDateT = Convert.ToDateTime(Convert.ToString(r["endDateT"]));
                temCont.descriptionT = Convert.ToString(r["descriptionT"]);
                contList.Add(temCont);
            }
            db.closeConnection();
 
            return Json(contList.AsEnumerable().Select(e => new {
                id = e.TaskId,
                start = e.startDateT.ToString("yyyy-MM-dd HH:mm"),
                end = e.endDateT.ToString("yyyy-MM-dd HH:mm"),
                title = e.title,
                description = e.descriptionT,
             

        }
                ).ToList(), JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult SaveEvent(MyTask e)
        {
          
            e.AccId = Convert.ToInt16(Request.Cookies["accountId"].Value);
            if (e.TaskId == 0)
            {
                DB db = new DB();
                DataTable table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                 MySqlCommand command = new MySqlCommand("INSERT INTO `task` (`idAccT`,`idTask`, `title`,`startDateT`,`endDateT`, `descriptionT`) VALUES (@uIdU,NULL,@uTit,  @uStaT, @uEndT,@uDes)", db.getConnection());
                command.Parameters.Add("@uIdU", MySqlDbType.VarChar).Value = Convert.ToInt16(Request.Cookies["accountId"].Value);
                command.Parameters.Add("@uTit", MySqlDbType.VarChar).Value = e.title;
                command.Parameters.Add("@uStaT", MySqlDbType.VarChar).Value = e.startDateT.ToString("yyyy-MM-dd HH:mm:ss");
                command.Parameters.Add("@uEndT", MySqlDbType.VarChar).Value = e.endDateT.ToString("yyyy-MM-dd HH:mm:ss"); ;
                command.Parameters.Add("@uDes", MySqlDbType.VarChar).Value = e.descriptionT;
                db.openConnection();
                if (command.ExecuteNonQuery() == 1)
                {
                    table = new DataTable();
                    adapter = new MySqlDataAdapter();
                    command = new MySqlCommand("SELECT *FROM tasks WHERE idUser=@uIdU AND idTask=@uNam AND title=@uTit AND description=@uDes AND startDateT= @uDate ", db.getConnection());
                    db.closeConnection();
                }
                db.closeConnection();
                var status = true;
                return new JsonResult { Data = new { status = status } };
            }
            else
            {
                DB db = new DB();
                MySqlCommand command = new MySqlCommand("UPDATE `task` SET  idAccT=@uIdU, title=@uTit , startDateT=@uStaT,endDateT=@uEndT, descriptionT=@uDes WHERE idTask=@uIdT ", db.getConnection());
                command.Parameters.Add("@uIdU", MySqlDbType.VarChar).Value = Convert.ToInt16(Request.Cookies["accountId"].Value);
                command.Parameters.Add("@uIdT", MySqlDbType.VarChar).Value = e.TaskId;
                command.Parameters.Add("@uTit", MySqlDbType.VarChar).Value = e.title;
                command.Parameters.Add("@uStaT", MySqlDbType.VarChar).Value = e.startDateT.ToString("yyyy-MM-dd HH:mm:ss");
                command.Parameters.Add("@uEndT", MySqlDbType.VarChar).Value = e.endDateT.ToString("yyyy-MM-dd HH:mm:ss"); 
                command.Parameters.Add("@uDes", MySqlDbType.VarChar).Value = Convert.ToString(e.descriptionT);

                db.openConnection();
                if (command.ExecuteNonQuery() == 1)
                {
                    db.closeConnection();
                 
                }

                var status = true;
                return new JsonResult { Data = new { status = status } };

            }
            
        }
        
        public JsonResult getContList()
        {
            List<Contacts> contList = new List<Contacts>();
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT *FROM conctacts WHERE idAccC=@uIdU ", db.getConnection());
            command.Parameters.Add("@uIdU", MySqlDbType.VarChar).Value = Convert.ToInt16(Request.Cookies["accountId"].Value);
            adapter.SelectCommand = command;
            adapter.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                Contacts temCont = new Contacts();
                temCont.ContId = Convert.ToInt16(r["idCont"]);
                temCont.contName = Convert.ToString(r["contName"]);
                temCont.contSurname = Convert.ToString(r["contSurname"]);
                temCont.phNumber = Convert.ToString(r["phNumber"]);
                contList.Add(temCont);
            }
            db.closeConnection();

            return Json(contList.AsEnumerable().Select(e => new {
                ContId = e.ContId,
                surname = Convert.ToString(e.contSurname+" " + e.contName+" " + e.phNumber)
            }
               ).ToList(), JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult getSelected( )
        {
            List<string> contList = new List<string>();
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT conctacts.idCont,conctacts.contName,conctacts.contSurname,conctacts.phNumber FROM conctacts INNER JOIN contfortask ON conctacts.idCont=contfortask.idContT WHERE ((contfortask.idTaskCont)=@uIdT) ", db.getConnection());
            command.Parameters.Add("@uIdT", MySqlDbType.VarChar).Value = Convert.ToInt16(Request.Cookies["TaskId"].Value);
            adapter.SelectCommand = command;
            adapter.Fill(table);
            if (table.Rows.Count >0)
            { foreach(DataRow r in table.Rows)
                {
                   string temCont = Convert.ToString(r["idCont"]);
                    contList.Add(temCont);
                }

            }
            return Json(contList.AsEnumerable().Select(e => new {
                ContId = e,  
            }
               ).ToList(), JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult SaveContacts(UserS Cont)
        {
            var status = false;
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            int TaskId;
            MySqlCommand command;
            if (Request.Cookies["TaskId"].Value == "0") //"ContactId"
            {
                 command = new MySqlCommand("SELECT* FROM task ORDER BY idTask DESC LIMIT 1", db.getConnection());

                adapter.SelectCommand = command;
                adapter.Fill(table);
                 TaskId = Convert.ToInt16(table.Rows[0]["idTask"]);
            }
            else
            {
                TaskId = Convert.ToInt16( Request.Cookies["TaskId"].Value);
            }
            if (Cont.forCont != "-1")
            {


                List<int> contId = new List<int>();
                string[] idContacts = Cont.forCont.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string r in idContacts)
                {
                    contId.Add(Convert.ToInt32(r));
                }

                List<int> exist = new List<int>();
                List<int> add = new List<int>();
                List<Contacts> contList = new List<Contacts>();
                db = new DB();
                table = new DataTable();
                adapter = new MySqlDataAdapter();
                command = new MySqlCommand("SELECT *FROM contfortask WHERE idTaskCont=@uIdC ", db.getConnection());
                command.Parameters.Add("@uIdC", MySqlDbType.VarChar).Value = TaskId;
                adapter.SelectCommand = command;
                adapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    foreach (DataRow r in table.Rows)
                    {
                        exist.Add(Convert.ToInt16(r["idContT"]));
                    }
                    foreach (int r in contId)
                    {
                        if (!exist.Exists(item => item == r))
                        {
                            add.Add(r);
                        }
                        else
                        {
                            exist.Remove(r);
                        }
                    }
                    foreach (int r in add)
                    {
                        db = new DB();
                        command = new MySqlCommand("INSERT INTO `contfortask` (`idLinkCT`,`idTaskCont`, `idContT`) VALUES( NULL, @uidT, @uidC)", db.getConnection());
                        command.Parameters.Add("@uidT", MySqlDbType.VarChar).Value = TaskId;
                        command.Parameters.Add("@uidC", MySqlDbType.VarChar).Value = r;
                        db.openConnection();
                        if (command.ExecuteNonQuery() == 1)
                        {
                            status = true;
                            db.closeConnection();
                        }
                        else { db.closeConnection(); status = false ; }
                    }
                    foreach (int r in exist)
                    {
                        db = new DB();
                        command = new MySqlCommand("DELETE FROM contfortask WHERE idTaskCont=@uIdT AND idContT=@uIdC", db.getConnection());
                        command.Parameters.Add("@uIdT", MySqlDbType.VarChar).Value = TaskId;
                        command.Parameters.Add("@uIdC", MySqlDbType.VarChar).Value = r;
                        db.openConnection();
                        if (command.ExecuteNonQuery() == 1)
                        {
                            status = true;
                            db.closeConnection();
                        }
                        else { db.closeConnection(); status = false ; }
                    }

                }
                else
                {
                    foreach (int r in contId)
                    {
                        db = new DB();
                        command = new MySqlCommand("INSERT INTO `contfortask` (`idLinkCT`,`idTaskCont`, `idContT`) VALUES( NULL, @uidT, @uidC)", db.getConnection());
                        command.Parameters.Add("@uIdT", MySqlDbType.VarChar).Value = TaskId;
                        command.Parameters.Add("@uIdC", MySqlDbType.VarChar).Value = r;
                        db.openConnection();
                        if (command.ExecuteNonQuery() == 1)
                        {
                            db.closeConnection();
                            status = true;
                        }
                        else { db.closeConnection(); status = false; }
                    }

                }
            }
            else
            {
                db = new DB();
                command = new MySqlCommand("DELETE FROM contfortask WHERE idTaskCont=@uIdT", db.getConnection());
                command.Parameters.Add("@uIdT", MySqlDbType.VarChar).Value = TaskId;
                db.openConnection();
                if (command.ExecuteNonQuery() == 1)
                {
                    db.closeConnection();
                  status = true;
                }
                else { db.closeConnection(); }
            }
        
            
            return new JsonResult { Data = new { status = status } };

        }

        [HttpPost]
        public JsonResult DeleteEvent(int TaskId)
        {
            var status = false;
            
            DB db = new DB();
            MySqlCommand command = new MySqlCommand("DELETE FROM task WHERE idTask=@uIdT", db.getConnection());
            command.Parameters.Add("@uIdT", MySqlDbType.VarChar).Value = TaskId;
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