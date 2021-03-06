﻿using System;
using System.Collections.Generic;
using TheraLang.DAL.Entities;

namespace TheraLang.Tests.DataBuilders.ResourcesBuilders
{
    public class UserTestBuilder : IDataBuilder<User>
    {
        private User _user;
        public UserTestBuilder()
        {
            _user = new User();
        }

        public UserTestBuilder WithDefault()
        {
            _user.PasswordHash = "123456789";
            _user.Email = "zwichainyi@utmm.com";

            return this;
        }

        /// <summary>
        /// Creats deafultUserDetails only was created UserId.
        /// </summary>
        /// <returns></returns>
        public UserTestBuilder WithDefaultDetails()
        {
            _user.Details = new UserDetailsTestBuilder()
                .WithDefault()
                .WithUserDetailsId(_user.Id)
                .Build();

            return this;
        }

        public UserTestBuilder WithDefaultRole()
        {
            _user.Role = new RoleTestBuilder()
                 .RoleMember()
                 .Build();

            return this;
        }

        public UserTestBuilder WithId(Guid id)
        {
            _user.Id = id;
            return this;
        }
        public UserTestBuilder WithEmail(string email)
        {
            _user.Email = email;
            return this;
        }
        public UserTestBuilder WithPasswordHash(string password)
        {
            _user.PasswordHash = password;
            return this;
        }
        public UserTestBuilder WithRole(Role role)
        {
            _user.Role = role;
            return this;
        }
        public UserTestBuilder WithRoleId(Guid id)
        {
            _user.RoleId = id;
            return this;
        }

        public UserTestBuilder WithNews(ICollection<News> news)
        {
            _user.News = news;
            return this;
        }
        public UserTestBuilder WithResources(ICollection<Resource> resources)
        {
            _user.Resources = resources;
            return this;
        }

        public UserTestBuilder WithDetails(UserDetails details)
        {
            _user.Details = details;
            return this;
        }

        public User Build()
        {
            return _user;
        }
    }
}
