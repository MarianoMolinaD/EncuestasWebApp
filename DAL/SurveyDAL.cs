using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;
using Dapper;

namespace DAL
{
    public class SurveyDAL
    {
        private readonly string _connectionString;

        public SurveyDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> CreateSurveyAsync(SurveyCreateViewModel survey)
        {
            using var db = new SqlConnection(_connectionString);
            db.OpenAsync();
            using var transaction = db.BeginTransaction();

            try
            {

                string insertSurvey = @"
                INSERT INTO Surveys ([Name], Description,UniqueLink,CreatedAt,UserId)
                VALUES (@Name, @Description,@UniqueLink, GETDATE(),@UserId);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int surveyId = await db.ExecuteScalarAsync<int>(
                    insertSurvey,
                    new { survey.Name, survey.Description, survey.UserId, survey.UniqueLink },
                    transaction
                );

                string insertField = @$"
                INSERT INTO SurveyFields (SurveyId, FieldName, FieldType,IsRequired,FieldTitle)
                VALUES ({surveyId}, @FieldName,@FieldType,@IsRequired,@FieldTitle);";

                foreach (var field in survey.Fields)
                {
                    await db.ExecuteAsync(insertField, new
                    {
                        SurveyId = surveyId,
                        field.FieldTitle,
                        field.FieldName,
                        field.FieldType,
                        field.IsRequired
                    }, transaction);
                }

                transaction.Commit();

                return surveyId;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<SurveyShowViewModel>> SurveyShow(int userId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            try
            {
                string selectSurvey = @$"SELECT T0.Id, [name] as Name, [Description], UniqueLink,CONVERT(DATE,T0.CreatedAt) AS CreatedAt , COUNT(T1.Id) OVER(PARTITION BY T0.Id) AS Responses
                                        FROM Surveys T0
                                        LEFT JOIN Responses T1 ON T0.Id = T1.SurveyId
                                        WHERE T0.UserId = {userId}";

                return await db.QueryAsync<SurveyShowViewModel>(selectSurvey, commandType: CommandType.Text);
            }

            catch (SqlException ex)
            {
                throw;
            }
        }
    }
}
