using NewApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewApp.Models;
using NewApp.Models.Data;
using System.Data;
using MySql.Data.MySqlClient;

namespace NewApp.Controllers
{
    public class MeetingController : Controller
    {
        // GET: Meeting
        public ActionResult Meeting()
        {
            List<Account> meetList = new List<Account>();
            DB db = new DB();
            string TempId = Request.Cookies["accountId"].Value;
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT *FROM user WHERE accountId=@usId", db.getConnection());
            command.Parameters.Add("@usId", MySqlDbType.VarChar).Value = Convert.ToInt16(TempId);
            adapter.SelectCommand = command;
            adapter.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                ViewBag.Vip = Convert.ToInt32(r["statusVIP"]);
            }
            return View();
        }


        public ActionResult listMeet()
        {
            List<Meeting> meetList = new List<Meeting>();
            DB db = new DB();
            string TempId = Request.Cookies["accountId"].Value;
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT *FROM meeting WHERE idAccM=@usId ORDER BY dateM", db.getConnection());
            command.Parameters.Add("@usId", MySqlDbType.VarChar).Value = Convert.ToInt16(TempId);
            adapter.SelectCommand = command;
            adapter.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                Meeting temMeet = new Meeting();
                temMeet.MeetId = Convert.ToInt16(r["idMeeting"]);
                temMeet.title = Convert.ToString(r["title"]);
                temMeet.dateM = Convert.ToDateTime(Convert.ToString(r["dateM"]));
                temMeet.dateM = Convert.ToDateTime(temMeet.dateM.ToString("dd-MM-yyyy HH:mm"));
                temMeet.place = Convert.ToString(r["place"]);
                temMeet.participants = Convert.ToString(r["participants"]);
                temMeet.notes = Convert.ToString(r["notes"]);

                meetList.Add(temMeet);
            }
            return PartialView("listMeet",meetList);
        }

        public JsonResult getMeeting(int MeetId)
        {
            int meetId = MeetId;
            List<Meeting> meetList = new List<Meeting>();
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT *FROM meeting WHERE idMeeting=@uIdM ", db.getConnection());
            command.Parameters.Add("@uIdM", MySqlDbType.VarChar).Value = MeetId;
            adapter.SelectCommand = command;
            adapter.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                Meeting temMeet = new Meeting();
                temMeet.MeetId = Convert.ToInt16(r["idMeeting"]);
                temMeet.title = Convert.ToString(r["title"]);
                temMeet.dateM = Convert.ToDateTime(r["dateM"]);
               
                temMeet.place = Convert.ToString(r["place"]);
                temMeet.participants = Convert.ToString(r["participants"]);
                temMeet.notes = Convert.ToString(r["notes"]);
                temMeet.notif = Convert.ToInt16(r["notif"]);
                meetList.Add(temMeet);
            }
            db.closeConnection();

            return Json(meetList.AsEnumerable().Select(e => new {

                title = e.title,
                dateM = e.dateM.ToString("dd-MM-yyyy HH:mm"),
               
                place = e.place,
                participants = e.participants,
                notes = e.notes,
                notif=e.notif
            }
                ).ToList(), JsonRequestBehavior.AllowGet);

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
                surname = Convert.ToString(e.contSurname + " " + e.contName + " " + e.phNumber)
            }
               ).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult getSelected(int MeetId)
        {
            List<string> contList = new List<string>();
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT conctacts.idCont,conctacts.contName,conctacts.contSurname,conctacts.phNumber FROM conctacts INNER JOIN contformeetings ON conctacts.idCont=contformeetings.idContM WHERE ((contformeetings.idMeetC)=@uIdT) ", db.getConnection());
            command.Parameters.Add("@uIdT", MySqlDbType.VarChar).Value = MeetId;
            adapter.SelectCommand = command;
            adapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                foreach (DataRow r in table.Rows)
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
        public ActionResult SaveMeeting(Meeting e)
        {
            bool status = false;
            e.AccId = Convert.ToInt16(Request.Cookies["accountId"].Value);

            if (e.MeetId == 0)
            {
                DB db = new DB();
                DataTable table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                MySqlCommand command = new MySqlCommand("INSERT INTO `meeting` (`idAccM`,`idMeeting`, `title`,`dateM`,`place`,`participants`,`notes`,`notif`) VALUES (@uIdU,NULL, @uTit,  @uDaM,@uPl, @uPrt, @uNot,@uNtf)", db.getConnection());
              
                command.Parameters.Add("@uIdU", MySqlDbType.VarChar).Value = e.AccId;
                command.Parameters.Add("@uDaM", MySqlDbType.VarChar).Value = e.dateM.ToString("yyyy-MM-dd HH:mm:ss"); 
                command.Parameters.Add("@uTit", MySqlDbType.VarChar).Value = e.title;
                command.Parameters.Add("@uPl", MySqlDbType.VarChar).Value = e.place;
                command.Parameters.Add("@uPrt", MySqlDbType.VarChar).Value = e.participants;
                command.Parameters.Add("@uNot", MySqlDbType.VarChar).Value = e.notes;
                command.Parameters.Add("@uNtf", MySqlDbType.VarChar).Value = Convert.ToInt16(e.notif);

                db.openConnection();
                if (command.ExecuteNonQuery() == 1)
                {
                    status = true;
                }
                db.closeConnection();
            }
            else
            {
                DB db = new DB();
                MySqlCommand command = new MySqlCommand("UPDATE `meeting` SET  title=@uTit, dateM=@uDaM, place=@uPl , participants=@uPrt, notes=@uNot, notif=@uNtf  WHERE idMeeting=@uIdM ", db.getConnection());
                command.Parameters.Add("@uIdM", MySqlDbType.VarChar).Value = e.MeetId;
                command.Parameters.Add("@uDaM", MySqlDbType.VarChar).Value = e.dateM.ToString("yyyy-MM-dd HH:mm:ss"); 
                command.Parameters.Add("@uTit", MySqlDbType.VarChar).Value = e.title;
                command.Parameters.Add("@uPl", MySqlDbType.VarChar).Value = e.place;
                command.Parameters.Add("@uPrt", MySqlDbType.VarChar).Value = e.participants;
                command.Parameters.Add("@uNot", MySqlDbType.VarChar).Value = e.notes;
                command.Parameters.Add("@uNtf", MySqlDbType.VarChar).Value = Convert.ToInt16(e.notif);
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
        public JsonResult DeleteMeeting(int MeetId)
        {
            bool status = false;
            int meetId = MeetId;
            List<Meeting> meetList = new List<Meeting>();
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            
            MySqlCommand command = new MySqlCommand("DELETE FROM meeting WHERE idMeeting=@uIdM ", db.getConnection());
            command.Parameters.Add("@uIdM", MySqlDbType.VarChar).Value = meetId;
            adapter.SelectCommand = command;
            adapter.Fill(table);
            db.openConnection();
            if (command.ExecuteNonQuery() == 1)
            {
                db.closeConnection();
                status = true;
            }
            else { db.closeConnection(); }

            return new JsonResult { Data = new { status = status } };
        }


        [HttpPost]
        public ActionResult SaveContacts(UserS Cont)
        {
            var status = false;
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            int MeetId;
            MySqlCommand command;
            if (Request.Cookies["MeetId"].Value == "0")
            {
                command = new MySqlCommand("SELECT* FROM meeting ORDER BY idMeeting DESC LIMIT 1", db.getConnection());

                adapter.SelectCommand = command;
                adapter.Fill(table);
                MeetId = Convert.ToInt16(table.Rows[0]["idMeeting"]);
            }
            else
            {
                MeetId = Convert.ToInt16(Request.Cookies["MeetId"].Value);
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
                command = new MySqlCommand("SELECT *FROM contformeetings WHERE idMeetC=@uIdC ", db.getConnection());
                command.Parameters.Add("@uIdC", MySqlDbType.VarChar).Value = MeetId;
                adapter.SelectCommand = command;
                adapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    foreach (DataRow r in table.Rows)
                    {
                        exist.Add(Convert.ToInt16(r["idContM"]));
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
                        command = new MySqlCommand("INSERT INTO `contformeetings` (`idLinkCM`,`idContM`,`idMeetC` ) VALUES( NULL, @uidC, @uidM)", db.getConnection());
                        command.Parameters.Add("@uidM", MySqlDbType.VarChar).Value = MeetId;
                        command.Parameters.Add("@uidC", MySqlDbType.VarChar).Value = r;
                        db.openConnection();
                        if (command.ExecuteNonQuery() == 1)
                        {
                            status = true;
                            db.closeConnection();
                        }
                        else { db.closeConnection(); status = false; }
                    }
                    foreach (int r in exist)
                    {
                        db = new DB();
                        command = new MySqlCommand("DELETE FROM contformeetings WHERE idMeetC=@uIdM AND idContM=@uIdC", db.getConnection());
                        command.Parameters.Add("@uIdM", MySqlDbType.VarChar).Value = MeetId;
                        command.Parameters.Add("@uIdC", MySqlDbType.VarChar).Value = r;
                        db.openConnection();
                        if (command.ExecuteNonQuery() == 1)
                        {
                            status = true;
                            db.closeConnection();
                        }
                        else { db.closeConnection(); status = false; }
                    }

                }
                else
                {
                    foreach (int r in contId)
                    {
                        db = new DB();
                        command = new MySqlCommand("INSERT INTO `contformeetings` (`idLinkCM`,`idContM`,`idMeetC` ) VALUES( NULL, @uidC, @uidM)", db.getConnection());
                        command.Parameters.Add("@uIdM", MySqlDbType.VarChar).Value = MeetId;
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
                command = new MySqlCommand("DELETE FROM contformeetings WHERE idMeetC=@uIdM", db.getConnection());
                command.Parameters.Add("@uIdM", MySqlDbType.VarChar).Value = MeetId;
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

    public ActionResult searchMeeting( string searchResult)
        {
            List<Meeting> meetList = new List<Meeting>();
            List<Meeting> result = new List<Meeting>();
            DB db = new DB();
            string TempId = Request.Cookies["accountId"].Value;
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT *FROM meeting WHERE idAccM=@usId ORDER BY dateM", db.getConnection());
            command.Parameters.Add("@usId", MySqlDbType.VarChar).Value = Convert.ToInt16(TempId);
            adapter.SelectCommand = command;
            adapter.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                Meeting temMeet = new Meeting();
                temMeet.MeetId = Convert.ToInt16(r["idMeeting"]);
                temMeet.title = Convert.ToString(r["title"]);
                temMeet.dateM = Convert.ToDateTime(Convert.ToString(r["dateM"]));
                temMeet.dateM = Convert.ToDateTime(temMeet.dateM.ToString("dd-MM-yyyy HH:mm"));
                temMeet.place = Convert.ToString(r["place"]);
                temMeet.participants = Convert.ToString(r["participants"]);
                temMeet.notes = Convert.ToString(r["notes"]);
                meetList.Add(temMeet);
            }

            //сравнение

            foreach (Meeting r in meetList)
            {
                string s = r.title + " " + Convert.ToString(r.dateM) + " " + r.place+" "+r.notes+" "+r.participants;

                if (s.Contains(searchResult))
                {
                    result.Add(r);
                }

            }
            return PartialView("listMeet", result);
        }


    }
}