using Newtonsoft.Json;
using RemoteLoggingService.Models;
using RemoteLoggingService.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteLoggingServiceTest
{
    class FakeRepository : IRepository
    {
        IEnumerable<User> users;
        public FakeRepository()
        {
            users = JsonConvert.DeserializeObject<IEnumerable<User>>(System.IO.File.ReadAllText(@"C:\sources\RemoteLoggingService\RemoteLoggingServiceTest\FakeUsers.json"));
        }
        public Task<TEntity> Add<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> AddAndSave<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> AddRangeAndSave<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task<bool> ClientExists(Guid? clientId)
        {
            throw new NotImplementedException();
        }

        public Task<Client> CreateClient(Client client)
        {
            throw new NotImplementedException();
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task DeleteAndSave<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAndSave<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Client>> GetAllClients()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserRole>> GetAllUserRoles()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllUsers()
        {
            return Task.FromResult(users);
        }

        public Task<Client> GetClientById(Guid? cliendId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Log>> GetLogs(GetLogsModel model)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Log>> GetLogs(Guid? clientId)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserById(Guid? userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserRole> GetUserRoleByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task Save()
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> Update<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> UpdateAndSave<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> UpdateRangeAndSave<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExists(Guid? userId)
        {
            throw new NotImplementedException();
        }
    }
}
