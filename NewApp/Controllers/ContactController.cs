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
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Contacts()
        {
            int vip = 0;
            List<Contacts> contList=new List<Contacts>();
            DB db = new DB();
            string TempId = Request.Cookies["accountId"].Value;
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT *FROM conctacts WHERE idAccC=@usId ORDER BY contSurname", db.getConnection());
            command.Parameters.Add("@usId", MySqlDbType.VarChar).Value = Convert.ToInt16(TempId);
            adapter.SelectCommand = command;
            adapter.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                Contacts temCont = new Contacts();
                temCont.ContId = Convert.ToInt16(r["idCont"]);
                temCont.contName = Convert.ToString(r["contName"]);
                temCont.contSurname = Convert.ToString(r["contSurname"]);
                temCont.phNumber = Convert.ToString(r["phNumber"]);
                contList.Add(temCont); }

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
        
        public ActionResult listCont()
        {
            List<Contacts> contList = new List<Contacts>();
            DB db = new DB();
            string TempId = Request.Cookies["accountId"].Value;
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT *FROM conctacts WHERE idAccC=@usId ORDER BY contSurname", db.getConnection());
            command.Parameters.Add("@usId", MySqlDbType.VarChar).Value = Convert.ToInt16(TempId);
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

            return PartialView( "listCont",contList);
        }

        public ActionResult searchCont(string searchResult)

        {
            List<Contacts> contList = new List<Contacts>();
            List<Contacts> result = new List<Contacts>();
            DB db = new DB();
            string TempId = Request.Cookies["accountId"].Value;
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT *FROM conctacts WHERE idAccC=@usId ORDER BY contSurname", db.getConnection());
            command.Parameters.Add("@usId", MySqlDbType.VarChar).Value = Convert.ToInt16(TempId);
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

            //сравнение
           
            foreach (Contacts r in contList)
            {
                string s = r.contName +" "+ r.contSurname+" "+r.phNumber;

                if (s.Contains(searchResult))
                {
                    result.Add(r);
                }
                else if (Convert.ToString(r.contSurname+ " " +r.contName + " "+r.phNumber).Contains(searchResult))
                { result.Add(r); }
                else if (Convert.ToString(r.contSurname + " " +  r.phNumber+ " "+ r.contName ).Contains(searchResult))
                { result.Add(r); }
                else if (Convert.ToString( r.phNumber+ " " +r.contSurname + " " + r.contName ).Contains(searchResult))
                { result.Add(r); }
                else if (Convert.ToString(r.contName+" "+ r.phNumber+ " " +r.contSurname  ).Contains(searchResult))
                { result.Add(r); }
                else if (Convert.ToString(r.phNumber +" " + r.contName+ " " + r.contSurname).Contains(searchResult))
                { result.Add(r); }

            }
            return PartialView("listCont", result);
        }

        public ActionResult AddCont(Contacts cont)
        {
            DB db = new DB();
            MySqlCommand command= new MySqlCommand("INSERT INTO `contacts`  (`idAccC`,`idCont`,`contName`, `contSurname`, `phNumber`) VALUES (@uUs,NULL,  @uNam, @uSurn, @uPhoneNum)", db.getConnection());
            command.Parameters.Add("@uUs", MySqlDbType.VarChar).Value = Request.Cookies["accountId"].Value;
            command.Parameters.Add("@uNam", MySqlDbType.VarChar).Value = cont.contName;
            command.Parameters.Add("@uSurn", MySqlDbType.VarChar).Value = cont.contSurname;
            command.Parameters.Add("@uPhoneNum", MySqlDbType.VarChar).Value = cont.phNumber;
            return RedirectToAction("Contacts");
        }


        public JsonResult getContact(int ContId)
        {

            List<Contacts> contList = new List<Contacts>();
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT *FROM conctacts WHERE idCont=@uIdC ", db.getConnection());
            command.Parameters.Add("@uIdC", MySqlDbType.VarChar).Value = ContId;
            adapter.SelectCommand = command;
            adapter.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                Contacts temCont = new Contacts();

                temCont.contName = Convert.ToString(r["contName"]);
                temCont.contSurname = Convert.ToString(r["contSurname"]);
                temCont.phNumber = Convert.ToString(r["phNumber"]);

                contList.Add(temCont);
            }
            db.closeConnection();

            return Json(contList.AsEnumerable().Select(e => new {
               
                name = e.contName,
                surname = e.contSurname,
                phNumber = e.phNumber,
            }
                ).ToList(), JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult SaveContact(Contacts e)
        {
            bool status = false;
            e.idAccC = Convert.ToInt16(Request.Cookies["accountId"].Value);
        
            if (e.ContId == 0)
            {
                DB db = new DB();
                DataTable table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                MySqlCommand command = new MySqlCommand("INSERT INTO `conctacts` (`idAccC`,`idCont`, `contName`,`contSurname`,`phNumber`) VALUES (@uIdU,NULL,@uNam,  @uSur, @uPhNum)", db.getConnection());
                command.Parameters.Add("@uIdU", MySqlDbType.VarChar).Value = e.idAccC;
                command.Parameters.Add("@uNam", MySqlDbType.VarChar).Value = e.contName;
                command.Parameters.Add("@uSur", MySqlDbType.VarChar).Value = e.contSurname;
                command.Parameters.Add("@uPhNum", MySqlDbType.VarChar).Value = e.phNumber;
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
                MySqlCommand command = new MySqlCommand("UPDATE `conctacts` SET  idAccC=@uIdU , contName=@uNam, contSurname=@uSur, phNumber=@uPhNum WHERE idCont=@uIdC ", db.getConnection());
                command.Parameters.Add("@uIdU", MySqlDbType.VarChar).Value = e.idAccC;
                command.Parameters.Add("@uIdC", MySqlDbType.VarChar).Value = e.ContId;
                command.Parameters.Add("@uNam", MySqlDbType.VarChar).Value = e.contName;
                command.Parameters.Add("@uSur", MySqlDbType.VarChar).Value = e.contSurname;
                command.Parameters.Add("@uPhNum", MySqlDbType.VarChar).Value = e.phNumber;

                db.openConnection();
                if (command.ExecuteNonQuery() == 1)
                {
                    db.closeConnection();
                    status = true;
                }
            }

            return new JsonResult { Data = new { status = status } };

        }

        public JsonResult DeleteContact( int ContId)
        {
            bool status = false;
            int contId = Convert.ToInt16(Response.Cookies["idContact"].Value);
            List<Contacts> contList = new List<Contacts>();
            DB db = new DB();
            MySqlCommand command = new MySqlCommand("DELETE FROM conctacts WHERE idCont=@uIdC ", db.getConnection());
            command.Parameters.Add("@uIdC", MySqlDbType.VarChar).Value = ContId;
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