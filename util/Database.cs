using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilPlugin
{
    public class Database
    {
        public static List<Badge> badges = new List<Badge>();
        public static void Update(string connectstring)
        {
            using (MySqlConnection connection = new MySqlConnection(connectstring))
            {
                connection.Open();
                badges.Clear();
                using (MySqlCommand command = new MySqlCommand("select * from `badge`"))
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
                                userid = reader.GetString("userid")
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
    }
}
