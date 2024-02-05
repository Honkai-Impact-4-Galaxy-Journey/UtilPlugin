//Copyright 2023 Silver Wolf,All Rights Reserved.
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilPlugin
{
    public class BadgeDatabase
    {
        public static string connectstring = UtilPlugin.Instance.Config.MysqlConnectstring;
        public static List<Badge> badges = new List<Badge>();
        public static void Update(string connectstring = null)
        {
            using (MySqlConnection connection = new MySqlConnection(connectstring != null ? connectstring : BadgeDatabase.connectstring))
            {
                connection.Open();
                badges.Clear();
                using (MySqlCommand command = new MySqlCommand("select * from `badge`", connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) {
                            Badge badge = new Badge
                            {
                                text = reader.GetString("text"),
                                color = reader.GetString("color"),
                                reverseslot = reader.GetString("reserveslot"),
                                adminrank = reader.GetString("admin"),
                                userid = reader.GetString("userid"),
                                cover = reader.GetBoolean("cover")
                            };
                            badges.Add(badge);
                        }
                    }
                }
            }
        }
        public static Badge GetBadge(string userid)
        {
            return badges.Find(x => x.userid == userid);
        }
    }
    public class Badge
    {
        public string text;
        public string color;
        public string reverseslot;
        public string adminrank;
        public string userid;
        public bool cover;
        public override string ToString()
        {
            return $"text:{text}, color:{color}, adminrank: {adminrank}";
        }
    }
}
