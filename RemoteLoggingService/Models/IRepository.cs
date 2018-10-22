using RemoteLoggingService.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemoteLoggingService.Models
{
    public interface IRepository
    {       
        #region User
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetUserById(Guid? userId);
        Task<User> GetUserByEmail(string email);
        Task<bool> UserExists(Guid? userId);
        Task<IEnumerable<Client>> GetUserClients(Guid? userId);
        #endregion User

        #region UserRole
        Task<IEnumerable<UserRole>> GetAllUserRoles();
        Task<UserRole> GetUserRoleByName(string name);
        #endregion UserRole

        #region Client
        Task<IEnumerable<Client>> GetAllClients();
        Task<Client> GetClientById(Guid? cliendId);
        Task<Client> CreateClient(Client client);
        Task<bool> ClientExists(Guid? clientId);
        #endregion Client

        #region Logs
        Task<IEnumerable<Log>> GetLogs(GetLogsModel model);
        Task<IEnumerable<Log>> GetLogs(Guid? clientId);
        #endregion Logs

        #region Common
        Task<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;
        Task<TEntity> UpdateAndSave<TEntity>(TEntity entity) where TEntity: class;
        Task<IEnumerable<TEntity>> UpdateRangeAndSave<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        void Delete<TEntity>(TEntity entity) where TEntity : class;
        Task DeleteAndSave<TEntity>(TEntity entity) where TEntity : class;
        Task DeleteRangeAndSave<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        Task<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;
        Task<TEntity> AddAndSave<TEntity>(TEntity entity) where TEntity : class;
        Task<IEnumerable<TEntity>> AddRangeAndSave<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        Task Save();
        #endregion Common
    }
}
