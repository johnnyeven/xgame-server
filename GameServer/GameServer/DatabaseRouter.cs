using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace com.xgame.common.database
{
    class DatabaseRouter
    {
        private static DatabaseRouter _instance;
        private static Boolean _allowInstance = false;

        private MySqlConnection _platformDb;
        private MySqlConnection _gameDb;

        public DatabaseRouter()
        {
            if (!_allowInstance)
            {
                return;
            }

            String connectionString = "Data Source=localhost;Initial Catalog=pulse_db_platform;User ID=root;Password=84@41%%wi96^4";
            _platformDb = new MySqlConnection(connectionString);
            _platformDb.Open();

            connectionString = "Data Source=localhost;Initial Catalog=pulse_db_game;User ID=root;Password=84@41%%wi96^4";
            _gameDb = new MySqlConnection(connectionString);
            _gameDb.Open();
        }

        public static DatabaseRouter instance()
        {
            if (_instance == null)
            {
                _allowInstance = true;
                _instance = new DatabaseRouter();
                _allowInstance = false;
            }
            return _instance;
        }

        public MySqlConnection platformDb()
        {
            return _platformDb;
        }

        public MySqlConnection gameDb()
        {
            return _gameDb;
        }
    }
}
