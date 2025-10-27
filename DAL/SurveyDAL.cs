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

        public async Task<IEnumerable<SurveyShowViewModel>> SurveyShowAsync(int userId)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            try
            {
                string selectSurvey = @$"SELECT DISTINCT T0.Id, [name] as Name, [Description], UniqueLink,CONVERT(DATE,T0.CreatedAt) AS CreatedAt , COUNT(T1.Id) OVER(PARTITION BY T0.Id) AS Responses
                                        FROM Surveys T0
                                        LEFT JOIN Responses T1 ON T0.Id = T1.SurveyId
                                        WHERE T0.UserId = {userId} AND T0.Deleted = 'N'";

                return await db.QueryAsync<SurveyShowViewModel>(selectSurvey, commandType: CommandType.Text);
            }

            catch (SqlException ex)
            {
                throw;
            }
        }

        public async Task<int> DeleteSurveryAsync(int id)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_connectionString);
                string query = "UPDATE Surveys SET Deleted = 'Y' WHERE Id = @Id";
                return await db.ExecuteAsync(query, new { Id = id }, commandType: CommandType.Text);
            }
            catch
            {
                throw;
            }
        }
        public async Task<SurveyFillViewModel?> GetSurveyWithFieldsByLinkAsync(string link)
        {
            using var db = new SqlConnection(_connectionString);
            const string query = @"
                                SELECT Id, [Name], Description 
                                FROM Surveys 
                                WHERE UniqueLink = @Link AND Deleted = 'N';

                                SELECT Id, FieldName, FieldTitle, FieldType, IsRequired 
                                FROM SurveyFields 
                                WHERE SurveyId = (SELECT Id FROM Surveys WHERE UniqueLink = @Link);";

            using var multi = await db.QueryMultipleAsync(query, new { Link = link });
            var survey = await multi.ReadFirstOrDefaultAsync<SurveyFillViewModel>();
            if (survey != null)
                survey.Fields = (await multi.ReadAsync<SurveyFieldViewModel>()).ToList();

            return survey;
        }

        public async Task SaveSurveyResponseAsync(SurveyResponseViewModel model)
        {
            using var db = new SqlConnection(_connectionString);
            await db.OpenAsync();
            using var tx = db.BeginTransaction();

            try
            {
                string insertResponse = @"
                                        INSERT INTO Responses (SurveyId)
                                        VALUES (@SurveyId);
                                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int responseId = await db.ExecuteScalarAsync<int>(
                    insertResponse,
                    new { model.SurveyId },
                    tx
                );

                string insertDetail = @"
                                        INSERT INTO ResponseDetails (ResponseId, FieldId, [Value])
                                        VALUES (@ResponseId, @FieldId, @Value);";

                foreach (var answer in model.Answers)
                {
                    await db.ExecuteAsync(insertDetail, new
                    {
                        ResponseId = responseId,
                        FieldId = answer.FieldId,
                        Value = answer.Value
                    }, tx);
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task<SurveyEditViewModel?> GetSurveyWithFieldsAsync(int id)
        {
            using var db = new SqlConnection(_connectionString);

            const string query = @"
                                    SELECT Id, [Name], Description
                                    FROM Surveys
                                    WHERE Id = @Id AND Deleted = 'N';

                                    SELECT Id, FieldName, FieldTitle, FieldType, IsRequired
                                    FROM SurveyFields
                                    WHERE SurveyId = @Id;";

            using var multi = await db.QueryMultipleAsync(query, new { Id = id });

            var survey = await multi.ReadFirstOrDefaultAsync<SurveyEditViewModel>();
            if (survey != null)
                survey.Fields = (await multi.ReadAsync<SurveyFieldEditViewModel>()).ToList();

            return survey;
        }

        public async Task UpdateSurveyAsync(SurveyEditViewModel survey)
        {
            using var db = new SqlConnection(_connectionString);
            await db.OpenAsync();
            using var tx = db.BeginTransaction();

            try
            {
                const string updateSurvey = @"
                                            UPDATE Surveys
                                            SET [Name] = @Name,
                                                Description = @Description
                                            WHERE Id = @Id;";

                await db.ExecuteAsync(updateSurvey, survey, tx);

                foreach (var field in survey.Fields)
                {
                    if (field.Id == 0)
                    {
                        // Nuevo campo
                        const string insert = @"
                                                INSERT INTO SurveyFields (SurveyId, FieldName, FieldTitle, FieldType, IsRequired)
                                                VALUES (@SurveyId, @FieldName, @FieldTitle, @FieldType, @IsRequired);";

                        await db.ExecuteAsync(insert, new
                        {
                            SurveyId = survey.Id,
                            field.FieldName,
                            field.FieldTitle,
                            field.FieldType,
                            field.IsRequired
                        }, tx);
                    }
                    else
                    {
                        const string update = @"
                                                UPDATE SurveyFields
                                                SET FieldName = @FieldName,
                                                    FieldTitle = @FieldTitle,
                                                    FieldType = @FieldType,
                                                    IsRequired = @IsRequired
                                                WHERE Id = @Id;";

                        await db.ExecuteAsync(update, field, tx);
                    }
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }


    }
}
