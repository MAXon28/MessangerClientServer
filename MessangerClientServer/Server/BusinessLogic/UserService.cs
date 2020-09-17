using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ChatLibrary.DataTransferObject;
using Server.Domain;
using Server.Interface;
using Server.Model;
using Server.Repository;

namespace Server.BusinessLogic
{
    class UserService : IService
    {
        private EFUnitOfWork _efUnitOfWork;

        private const int UPDATE_NAME = 1;
        private const int UPDATE_GENDER = 2;
        private const int UPDATE_LOGIN = 3;
        private const int UPDATE_PASSWORD = 4;

        public UserService()
        {
            _efUnitOfWork = new EFUnitOfWork();
        }

        public bool IsUniqueData(string login = "", string name = "")
        {
            IEnumerable<User> users = _efUnitOfWork.UsersRepository.GetAll();
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

        public Guid AddUser(string login, string password, string gender, string name)
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

            _efUnitOfWork.UsersRepository.Create(user);

            return user.Id;
        }

        public async void UpdateUserAsync(int typeOfUpdate, Guid userId, string updateString)
        {
            switch (typeOfUpdate)
            {
                case UPDATE_NAME:
                    await Task.Run(() => _efUnitOfWork.Users.UpdateName(userId, updateString));
                    break;
                case UPDATE_GENDER:
                    await Task.Run(() => _efUnitOfWork.Users.UpdateGender(userId, updateString));
                    break;
                case UPDATE_LOGIN:
                    await Task.Run(() => _efUnitOfWork.Users.UpdateLogin(userId, updateString));
                    break;
                case UPDATE_PASSWORD:
                    await Task.Run(() => _efUnitOfWork.Users.UpdatePassword(userId, updateString));
                    break;
            }
        }

        public async void UpdatePastOnlineAsync(Guid userId, DateTime date)
        {
            await Task.Run(() => _efUnitOfWork.Users.UpdatePastOnline(userId, date));
        }

        public List<UserDTO> GetUsers()
        {
            List<User> users = (from u in _efUnitOfWork.UsersRepository.GetAll()
                where u.PastOnline != null
                select u).ToList();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>()).CreateMapper();
            return (mapper.Map<List<User>, List<UserDTO>>(users));
        }

        public UserDomain ValidationData(string login, string password, ref Guid id)
        {
            IEnumerable<User> users = _efUnitOfWork.UsersRepository.GetAll() != null ? _efUnitOfWork.UsersRepository.GetAll() : null;

            if (users == null)
            {
                return null;
            }

            List<User> selectUsers = (from user in users
                where user.Login == login && user.Password == password
                select user).ToList();

            if (selectUsers.Count == 1)
            {
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDomain>()).CreateMapper();
                id = selectUsers[0].Id;
                return mapper.Map<List<User>, List<UserDomain>>(selectUsers)[0];
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