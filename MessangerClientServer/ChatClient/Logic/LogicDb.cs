﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using ChatClient.Model;
using SQLite;

namespace ChatClient.Logic
{
    class LogicDb
    {
        private const string NAME_DATA_BASE = "Chat.db";
        private SQLiteConnection dataBase;
        private List<User> _users;
        private int _index;

        public LogicDb()
        {
            try
            {
                dataBase = new SQLiteConnection(NAME_DATA_BASE, SQLiteOpenFlags.ReadWrite, true);
                _users = dataBase.Table<User>().ToList();
            }
            catch (Exception)
            {
                dataBase = new SQLiteConnection(NAME_DATA_BASE, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, true);
                dataBase.CreateTable<User>();
                _users = new List<User>();
            }

            _index = 0;
        }

        public void AddNewUser(string login, string password)
        {
            if ((from u in dataBase.Table<User>()
                where u.Login == login
                select u).ToList().Count == 0)
            {
                User user = new User();
                user.Login = login;
                user.Password = password;
                dataBase.Insert(user);
            }
        }

        public int GetCountSaveUsers()
        {
            return _users.Count;
        }

        public (string, string) GetSaveData()
        {
            if (_index < _users.Count)
            {
                var user = _users[_index];
                _index++;
                return (user.Login, user.Password);
            }
            _index = 0;
            return GetSaveData();
        }
    }
}