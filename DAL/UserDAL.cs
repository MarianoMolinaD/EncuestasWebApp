using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Entity;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class UserDAL
    {
        private readonly string _connectionString;

        public UserDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            try
            {
                using IDbConnection db = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
                string query = "SELECT * FROM Users WHERE UserName = @UserName";
                return await db.QueryFirstOrDefaultAsync<User>(query, new { UserName = userName });
            }catch
            {
                throw;
            }
            
        }
    }
}
