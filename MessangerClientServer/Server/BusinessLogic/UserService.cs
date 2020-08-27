using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Server.DataTransferObject;
using Server.Interface;
using Server.Model;
using Server.Repository;

namespace Server.BusinessLogic
{
    class UserService : IService
    {
        private EFUnitOfWork _efUnitOfWork;

        public UserService()
        {
            _efUnitOfWork = new EFUnitOfWork();
        }

        public bool IsUniqueData(string login, string name)
        {
            IEnumerable<User> users = _efUnitOfWork.Users.GetAll();
            if (users == null)
            {
                return true;
            }

            if ((from user in users
                where user.Login == login || user.Name == name
                select user).ToList().Count > 0)
            {
                return false;
            }

            return true;
        }

        public void AddUser(string login, string password, string gender, string name)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Login = login,
                Password = password,
                Gender = gender,
                Name = name,
                PastOnline = null
            };

            _efUnitOfWork.Users.Create(user);

            SaveAsync();
        }

        public UserDTO ValidationData(string login, string password, ref Guid id)
        {
            IEnumerable<User> users = _efUnitOfWork.Users.GetAll();

            if (users == null)
            {
                return null;
            }

            List<User> selectUsers = (from user in users
                where user.Login == login && user.Password == password
                select user).ToList();

            if (selectUsers.Count == 1)
            {
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>()).CreateMapper();
                id = selectUsers[0].Id;
                return mapper.Map<List<User>, List<UserDTO>>(selectUsers)[0];
            }

            return null;
        }

        public EFUnitOfWork GetUnitOfWork()
        {
            return _efUnitOfWork;
        }

        public async void SaveAsync()
        {
            await Task.Run(_efUnitOfWork.Save);
        }
    }
}