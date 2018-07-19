using RemoteLoggingService.BL.Inerfaces;
using RemoteLoggingService.Services.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace RemoteLoggingService.BL
{
    public class SqlLogProvider : ILogProvider
    {
        private string connectionString;
        public SqlLogProvider()
        {
            var settingsText = File.ReadAllText(@"appsettings.json");
            connectionString = JsonConvert.DeserializeObject<AppSettings>(settingsText).ConnectionStrings.DefaultConnection;
        }

        public string GetUserRole(string userGuid)
        {
            var query =
                "SELECT UR.Name as RoleName " +
                "FROM dbo.Users as US " +
                "INNER JOIN dbo.UserRoles AS UR ON UR.Id = US.RoleId " +
                "WHERE US.UserId = @UserId";
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userGuid);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    return reader["RoleName"].ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        public bool CheckIfDeveloperAssignedToClient(string clientGuid, string developerGuid)
        {
            var query =
                "SELECT ClientId " +
                "FROM dbo.Clients " +
                "WHERE ClientId = @ClientId and DeveloperId = @DeveloperId";

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClientId", clientGuid);
                command.Parameters.AddWithValue("@DeveloperId", developerGuid);
                connection.Open();
                var reader = command.ExecuteReader();
                if(reader.HasRows)
                {
                    return true;
                }
                return false;

            }
        }

        public List<LogMessage> GetLogMessages(string clientGuid, LogMessage.LogStatus status)
        {
            var query =
                "SELECT ClientGuid, Time, Status, Message " +
                "FROM dbo.Logs " +
                "WHERE ClientGuid = @clientGuid";
            if(status != LogMessage.LogStatus.Any)
            {
                query += " AND Status = @Status";
            }
            var logMessages = new List<LogMessage>();
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@clientGuid", clientGuid);
                command.Parameters.AddWithValue("@Status", status.ToString());
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    
                    logMessages.Add(new LogMessage
                        (
                            clientGuid: reader["ClientGuid"].ToString(),
                            message: reader["Message"].ToString(),
                            time: (DateTime)reader["Time"],
                            status: reader["Status"].ToString(),
                            metadata: reader["Metadata"].ToString()
                        ));
                }

                return logMessages;
            }
        }

        public List<LogMessage> GetLogMessages(string clientGuid, LogMessage.LogStatus status, DateTime fromDate)
        {
            throw new NotImplementedException();
        }

        public List<LogMessage> GetLogMessages(string clientGuid, LogMessage.LogStatus status, DateTime fromDate, DateTime ToDate)
        {
            throw new NotImplementedException();
        }       

        public void PostLogMessages(List<LogMessage> logMessages)
        {
            var query =
                "INSERT INTO dbo.Logs " +
                "(ClientGuid, Time, Message, Status, Metadata) VALUES " +
                "(@ClientGuid, @Time, @Message, @Status, @Metadata)";
               
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(query, connection);
                command.Parameters.Add("@ClientGuid", SqlDbType.NVarChar,50);
                command.Parameters.Add("@Time", SqlDbType.DateTime);
                command.Parameters.Add("@Message", SqlDbType.NVarChar,-1);
                command.Parameters.Add("@Status", SqlDbType.NVarChar, 10);
                command.Parameters.Add("@Metadata", SqlDbType.NVarChar, -1);
                

                foreach (var logMessage in logMessages)
                {                    
                    command.Parameters["@ClientGuid"].Value = logMessage.ClientGuid;
                    command.Parameters["@Time"].Value = logMessage.Time;
                    command.Parameters["@Message"].Value = logMessage.Message;
                    command.Parameters["@Status"].Value = logMessage.Status.ToString();
                    command.Parameters["@Metadata"].Value = logMessage.Metadata;
                    
                    command.ExecuteNonQuery();
                }          
            }
        }
    }
}
