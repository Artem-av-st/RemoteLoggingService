using Microsoft.EntityFrameworkCore;
using RemoteLoggingService.Services;
using RemoteLoggingService.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteLoggingService.Models
{
    public class MainAppRepo : IRepository
    {      
        private AppDbContext _db;

        public MainAppRepo()
        {            
            _db = new AppDbContext();
        }

        #region User
        public async Task<User> GetUserByEmail(string email)
        {
            return (await GetAllUsers()).FirstOrDefault(x => x.Email == email
);
        }

        public async Task<IEnumerable<Client>> GetUserClients(Guid? userId)
        {
            var user = await GetUserById(userId);
            
            if (user.UserRole.Name == "Admin")
            {
                return await GetAllClients();
            }
            else
            {
                return (await GetAllClients()).Where(x => x.Developer.Id == user.Id);
            }
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _db.Users.Include(u => u.UserRole).ToListAsync();
        }

        public async Task<User> GetUserById(Guid? id)
        {
            return await _db.Users.Include(u => u.UserRole).SingleOrDefaultAsync(_ => _.Id == id);
        }

        public async Task<bool> UserExists(Guid? userId)
        {
            return await _db.Users.AnyAsync(_ => _.Id == userId);
        }
        #endregion User

        #region Common
        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<TEntity> Update<TEntity>(TEntity entity) where TEntity : class
        {
            _db.Entry(entity).State = EntityState.Modified;
            return (TEntity) await _db.FindAsync(entity.GetType(), entity);
        }

        public async Task<TEntity> UpdateAndSave<TEntity>(TEntity entity) where TEntity: class
        {
            var updatedEntity = await Update(entity);
            await Save();

            return updatedEntity;
        }

        public async Task<IEnumerable<TEntity>> UpdateRangeAndSave<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _db.Entry(entities).State = EntityState.Modified;
            await Save();
            var resultEntities = new List<TEntity>();
            foreach (var entity in entities)
            {
                resultEntities.Add((TEntity)await _db.FindAsync(entity.GetType(), entity));
            }
            return resultEntities;
        }               

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            _db.Entry(entity).State = EntityState.Deleted;
        }

        public async Task DeleteAndSave<TEntity>(TEntity entity) where TEntity : class
        {
            Delete(entity);
            await Save();
        }

        public async Task DeleteRangeAndSave<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _db.Entry(entities).State = EntityState.Deleted;
            await Save();
        }

        public async Task<TEntity> Add<TEntity>(TEntity entity) where TEntity : class
        {
            _db.Entry(entity).State = EntityState.Added;
            var entityType = entity.GetType();
            var idProp = entityType.GetProperty("Id") ?? entityType.GetProperty($"{entityType.Name}Id");

            return (TEntity) await _db.FindAsync(entity.GetType(), idProp.GetValue(entity));
        }

        public async Task<TEntity> AddAndSave<TEntity>(TEntity entity) where TEntity : class
        {
            var newEntity = await Add(entity);
            await Save();
            return newEntity;
        }

        public async Task<IEnumerable<TEntity>> AddRangeAndSave<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _db.Entry(entities).State = EntityState.Added;
            await Save();
            var resultEntities = new List<TEntity>();
            foreach (var entity in entities)
            {
                resultEntities.Add((TEntity) await _db.FindAsync(entity.GetType(), entity));
            }
            return resultEntities;
        }
        #endregion Common

        #region UserRole
        public async Task<IEnumerable<UserRole>> GetAllUserRoles()
        {
            return await _db.UserRoles.Include(ur => ur.Users).ToListAsync();
        }

        public async Task<UserRole> GetUserRoleByName(string name)
        {
            return await _db.UserRoles.Include(ur => ur.Users).SingleOrDefaultAsync(_ => _.Name == name);
        }
        #endregion UserRole

        #region Client
        public async Task<IEnumerable<Client>> GetAllClients()
        {
            return await _db.Clients
                .Include(c => c.Developer).ThenInclude(u => u.UserRole)
                .Include(u => u.UserRole)
                .ToListAsync();
        }

        public async Task<Client> GetClientById(Guid? id)
        {
            return await _db.Clients
                .Include(c => c.Developer).ThenInclude(u => u.UserRole)
                .Include(u => u.UserRole)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Client> CreateClient(Client client)
        {
            var clientGuidParam = new SqlParameter("@ClientGuid", client.Id);
            var clientNameParam = new SqlParameter("@ClientName", client.Name);
            var developerGuidParam = new SqlParameter("@DeveloperGuid", client.Developer.Id);
            var resultParam = new SqlParameter
            {
                ParameterName = "@Result",
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Direction = System.Data.ParameterDirection.Output,
                Size = 100
            };

            // Execute stored procedure
            await _db.Database.ExecuteSqlCommandAsync(
                "AddNewClient @ClientGuid, @ClientName, @DeveloperGuid, @Result OUT",
                clientGuidParam, clientNameParam, developerGuidParam, resultParam);

            // Check result
            if (!String.IsNullOrEmpty(resultParam.Value.ToString()))
            {
                throw new Exception("Add new user exception: " + resultParam.Value.ToString());
            }

            return client;
        }

        public async Task<bool> ClientExists(Guid? clientId)
        {
            return await _db.Clients.AnyAsync(x => x.Id == clientId);
        }
        #endregion Client

        #region Log
        public async Task<IEnumerable<Log>> GetLogs(GetLogsModel model)
        {
            var logs = new List<Log>();
            if (model.LogType == "Any")
            {
                logs = await _db.Logs.Where(log =>
                    log.User.Name == model.ClientName &&
                    log.Time >= model.FromDate &&
                    log.Time <= model.ToDate &&
                    log.Message.CustomContains(model.FindText))
                    .OrderBy(x => x.Time)
                    .Include(l => l.User).ThenInclude(u => u.UserRole)
                    .ToListAsync();
            }
            else
            {
                logs = await _db.Logs.Where(log =>
                    log.User.Name == model.ClientName &&
                    log.Time >= model.FromDate &&
                    log.Time <= model.ToDate &&
                    log.Status == model.LogType &&
                    log.Message.CustomContains(model.FindText))
                    .OrderBy(x => x.Time)
                    .Include(l => l.User).ThenInclude(u => u.UserRole)
                    .ToListAsync();
            }

            return logs;
        }
        
        public async Task<IEnumerable<Log>> GetLogs(Guid? userId)
        {
            return await _db.Logs.Include(l => l.User).ThenInclude(u => u.UserRole).Where(x => x.ClientId == userId).ToListAsync();
        }
        #endregion Log
        
    }
}
