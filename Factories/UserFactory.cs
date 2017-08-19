using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using blackbelt.Models;
using System.Linq; 

namespace blackbelt.Factory
{
    public class UserFactory : IFactory<User>
    {
        private readonly IOptions<MySqlOptions> MySqlConfig;
 
        public UserFactory(IOptions<MySqlOptions> config)
        {
            MySqlConfig = config;
        }
        internal IDbConnection Connection 
        {
            get {
                return new MySqlConnection(MySqlConfig.Value.ConnectionString);
            }
        }
        public void CreateUser(User newUser)
        {
            using(IDbConnection dbConnection = Connection)
            {
                string query = @"INSERT INTO users (FirstName, LastName, Password, UserName, Wallet, CreatedAt, UpdatedAt) VALUES (@FirstName, @LastName, @Password, @UserName, 1000.00, NOW(), NOW())";
                dbConnection.Open();
                dbConnection.Execute(query, newUser);
            }
        }
        public List<User> TestUser(User testUser)
        {
            using(IDbConnection dbConnection = Connection)
            {

                string query = @"SELECT * FROM users WHERE (UserName = @UserName)";
                dbConnection.Open();
                return dbConnection.Query<User>(query, testUser).ToList();
                
            }
        }

        public User LoginUser(UserTest test)
        {
            using(IDbConnection dbConnection = Connection)
            {
                string query = @"SELECT * FROM users WHERE (UserName = @UserName && Password = @Password)";
                dbConnection.Open();
                return dbConnection.Query<User>(query, test).SingleOrDefault();              
            }
        }
        public User GetUserById(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string query = $"SELECT id, FirstName, LastName, UserName, Wallet From users WHERE (id={id})";
                dbConnection.Open();
                return dbConnection.Query<User>(query).SingleOrDefault();
            }
        }
    }

}