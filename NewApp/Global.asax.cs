using MySql.Data.MySqlClient;
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
using System.Web.Optimization;
using System.Web.Routing;


namespace NewApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        static Timer timer;
        long interval = 60000; //1 Минута
        static object synclock = new object();
        static bool sent = false;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            timer = new Timer(new TimerCallback(setAccounts), null, 0, interval);
        }
        public void setAccounts(object obj)
        {
            lock (synclock)
            {
                DB db = new DB();
                List<int> acc = new List<int>();

                DataTable table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                MySqlCommand command = new MySqlCommand("SELECT *FROM user WHERE statusVIP=1 AND pushState=1", db.getConnection());

                adapter.SelectCommand = command;
                adapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    foreach (DataRow r in table.Rows)
                    {
                        acc.Add(Convert.ToInt16(r["accountId"]));
                    }
                    SendMessage(acc);
                }
            }
        }

        public void SendMessage(List<int> accs)
        {//SELECT * FROM `meeting` WHERE idAccM=@usId AND notif=1 AND TO_DAYS(CURDATE()-TO_DAYS(`dateM`)<1
            //SELECT * FROM `meeting` WHERE `dateM`=DATE_ADD(@uCurD, INTERVAL 120 MINUTE)
            lock (synclock)
            {
                string email = "";
                string message = "";
                string curDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                List<List<Meeting>> meetList = new List<List<Meeting>>();
                List<VIP_User> listSt = new List<VIP_User>();
                int iterator = 0;
                DB db = new DB();
                DataTable table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                MySqlCommand command;
                foreach (int r in accs)
                {
                    table = new DataTable();
                    adapter = new MySqlDataAdapter();
                    command = new MySqlCommand("SELECT * FROM settingsmeet WHERE accId = @usId", db.getConnection());
                    command.Parameters.Add("@usId", MySqlDbType.VarChar).Value = Convert.ToInt16(r);
                    adapter.SelectCommand = command;
                    adapter.Fill(table);
                    if (table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            VIP_User sett = new VIP_User();
                            sett.accId = Convert.ToInt16(row["accId"]);
                            sett.timer = Convert.ToInt16(row["timer"]);
                            sett.allM = Convert.ToInt32(row["allM"]);
                            sett.email = Convert.ToString(row["email"]);
                            listSt.Add(sett);
                        }

                        if (listSt[iterator].allM == 1)
                        {
                            table = new DataTable();
                            adapter = new MySqlDataAdapter();
                            command = new MySqlCommand("SELECT * FROM `meeting` WHERE `dateM`=DATE_ADD(@uCurD, INTERVAL @uTime MINUTE) AND idAccM=@uId", db.getConnection());
                            command.Parameters.Add("@uId", MySqlDbType.VarChar).Value = Convert.ToInt16(r);
                            command.Parameters.Add("@uCurD", MySqlDbType.VarChar).Value = curDate;
                            command.Parameters.Add("@uTime", MySqlDbType.VarChar).Value = listSt[iterator].timer;
                            adapter.SelectCommand = command;
                            adapter.Fill(table);
                            if (table.Rows.Count > 0)
                            {
                                foreach (DataRow ro in table.Rows)
                                {
                                    Meeting temMeet = new Meeting();
                                    temMeet.MeetId = Convert.ToInt16(ro["idMeeting"]);
                                    temMeet.title = Convert.ToString(ro["title"]);
                                    temMeet.dateM = Convert.ToDateTime(Convert.ToString(ro["dateM"]));
                                    temMeet.dateM = Convert.ToDateTime(temMeet.dateM.ToString("dd-MM-yyyy HH:mm"));
                                    temMeet.place = Convert.ToString(ro["place"]);
                                    temMeet.participants = Convert.ToString(ro["participants"]);
                                    temMeet.notes = Convert.ToString(ro["notes"]);

                                    message = "Нагадуємо, що сьогодні " + temMeet.dateM.ToString("dd-MM-yyyy HH:mm") + " *ВІДБУДЕТЬСЯ* " + temMeet.title + "\n" + "УЧАСНИКИ " + temMeet.participants + "\n" + " МІСЦЕ " + temMeet.place + "\n" + " Нотатки:" + temMeet.notes;
                                    email = listSt[iterator].email;
                                    SendEmail(message, email);
                                }
                            }

                        }
                        else
                        {
                            table = new DataTable();
                            adapter = new MySqlDataAdapter();
                            command = new MySqlCommand("SELECT * FROM `meeting` WHERE `dateM`=DATE_ADD(@uCurD, INTERVAL @uTime MINUTE) AND notif=1 AND idAccM=@uId ", db.getConnection());
                            command.Parameters.Add("@uId", MySqlDbType.VarChar).Value = Convert.ToInt16(r);
                            command.Parameters.Add("@uCurD", MySqlDbType.VarChar).Value = curDate;
                            command.Parameters.Add("@uTime", MySqlDbType.VarChar).Value = listSt[iterator].timer;
                            adapter.SelectCommand = command;
                            adapter.Fill(table);
                            if (table.Rows.Count > 0)
                            {
                                foreach (DataRow ro in table.Rows)
                                {
                                    Meeting temMeet = new Meeting();
                                    temMeet.MeetId = Convert.ToInt16(ro["idMeeting"]);
                                    temMeet.title = Convert.ToString(ro["title"]);
                                    temMeet.dateM = Convert.ToDateTime(Convert.ToString(ro["dateM"]));
                                    temMeet.dateM = Convert.ToDateTime(temMeet.dateM.ToString("dd-MM-yyyy HH:mm"));
                                    temMeet.place = Convert.ToString(ro["place"]);
                                    temMeet.participants = Convert.ToString(ro["participants"]);
                                    temMeet.notes = Convert.ToString(ro["notes"]);


                                    message = "Нагадуємо, що сьогодні " + temMeet.dateM.ToString("dd-MM-yyyy HH:mm") + " *ВІДБУДЕТЬСЯ* " + temMeet.title + "\n" + "УЧАСНИКИ " + temMeet.participants + "\n" + " МІСЦЕ " + temMeet.place + "\n" + " Нотатки:" + temMeet.notes;
                                    email = listSt[iterator].email;
                                    SendEmail(message, email);
                                }
                            }

                        }
                        iterator += 1;
                    }
                }

            }
        }
        public async void SendEmail(string message, string email)
        {
            EmailService emailService = new EmailService();
            await emailService.SendEmailAsync(email, "Нагадування", message);

        }
    }
}