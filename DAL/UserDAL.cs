using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Entity;

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
            using IDbConnection db = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
            string query = "SELECT * FROM Users WHERE UserName = @UserName";
            return await db.QueryFirstOrDefaultAsync<User>(query, new { UserName = userName });
        }
        public async Task<int> CreateAsync(User user)
        {
            using IDbConnection db = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
            string sql = @"INSERT INTO Users (UserName, PasswordHash)
                           VALUES (@UserName, @PasswordHash);
                           SELECT CAST(SCOPE_IDENTITY() as int);";
            return await db.ExecuteScalarAsync<int>(sql, user);
        }
    }
}
