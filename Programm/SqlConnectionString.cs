using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace VkrProgramm
{
    class MySqlConnectionString
    {
        public static string GetConnectionMySql()
        {
            return "Server=localhost;Database=vkr;User Id=root;Password=13052003ujl;";
        }
    }
}
