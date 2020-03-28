﻿using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TheraLang.BLL.Services;
using TheraLang.DAL.Entities;
using TheraLang.DAL.Repository;
using TheraLang.DAL.UnitOfWork;
using TheraLang.Tests.DataBuilders.ResourcesBuilders;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Common.Configurations;
using Common.Helpers.PasswordHelper;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace TheraLang.Tests.Services
{
   public class ConfirmationServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ConfirmationService _confirmationService;
        private readonly List<User> _testUsersDb;
        private readonly List<Role> _testRolesDb;
        private readonly List<UserConfirmation> _testUserConfirmationDb;
        private readonly List<UserDetails> _testUserDetailsDb;
        private readonly Mock<ISendGridClient> _sendGridMock;

        public ConfirmationServiceTests()
        {
            _testRolesDb = GetRolesTestData().ToList();
            _testUsersDb = GetUsersTestData().ToList();
            _testUserConfirmationDb = GetUserConfirmationTestData().ToList();
            _testUserDetailsDb = GetUserDetailsTestData().ToList();

            var userRepoMock = new Mock<IRepository<User>>();
            var roleRepoMock = new Mock<IRepository<Role>>();
            var confirmationRepoMock = new Mock<IRepository<UserConfirmation>>();
            var userDetailsRepoMock = new Mock<IRepository<UserDetails>>();

            userRepoMock.Setup(r => r.GetAll()).Returns(_testUsersDb.AsQueryable().BuildMock().Object);
            userRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync((Expression<Func<User, bool>> predicate) => _testUsersDb
                    .AsQueryable()
                    .Where(predicate)
                    .FirstOrDefault());
            userRepoMock.Setup(r => r.Add(It.IsAny<User>()))
                .Callback((User user) =>
                {
                    user.Id = new Guid();
                    _testUsersDb.Add(user);
                });

            roleRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Role, bool>>>()))
            .ReturnsAsync((Expression<Func<Role, bool>> predicate) => _testRolesDb
            .AsQueryable()
            .Where(predicate)
            .FirstOrDefault());

            confirmationRepoMock.Setup(r => r.Add(It.IsAny<UserConfirmation>()))
            .Callback((UserConfirmation confirmation) =>
            {
                _testUserConfirmationDb.Add(confirmation);
            });

            confirmationRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<UserConfirmation, bool>>>()))
               .ReturnsAsync((Expression<Func<UserConfirmation, bool>> predicate) => _testUserConfirmationDb
               .AsQueryable()
               .Where(predicate)
               .FirstOrDefault());

            userDetailsRepoMock.Setup(r => r.GetAll()).Returns(_testUserDetailsDb.AsQueryable().BuildMock().Object);
            userDetailsRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<UserDetails, bool>>>()))
                .ReturnsAsync((Expression<Func<UserDetails, bool>> predicate) => _testUserDetailsDb
                .AsQueryable()
                .Where(predicate)
                .FirstOrDefault());
            userDetailsRepoMock.Setup(r => r.Add(It.IsAny<UserDetails>()))
                .Callback((UserDetails details) =>
                {
                    _testUserDetailsDb.Add(details);

                });

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(u => u.Repository<User>()).Returns(userRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<UserDetails>()).Returns(userDetailsRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<Role>()).Returns(roleRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<UserConfirmation>()).Returns(confirmationRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Verifiable();
            var mockEnvironment = new Mock<IHostingEnvironment>();
            mockEnvironment.SetupAllProperties();
            mockEnvironment.Setup(m => m.ContentRootPath).Returns("");
            mockEnvironment.Setup(m => m.EnvironmentName).Returns("TEST");
            var _emailSettings = Options.Create(new EmailSettings());
            _sendGridMock = new Mock<ISendGridClient>();
            _sendGridMock.SetupAllProperties();
            var response = new Response(System.Net.HttpStatusCode.OK, null, null);
            _sendGridMock.Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), CancellationToken.None)).Returns(Task.FromResult(response));

            _confirmationService = new ConfirmationService(mockEnvironment.Object, _unitOfWorkMock.Object, _emailSettings, _sendGridMock.Object);
        }

        [Fact]
        public async Task SendEmail_ShouldSendEmail()
        {
            var user = new User
            {
                Id = DefaultValues.Guid,
                Email = "andriana@gmail.com"
            };

            _testUsersDb.Add(user);

            var userDetails = new UserDetails
            {
                UserDetailsId = DefaultValues.Guid,
                User = user
            };

            _testUserDetailsDb.Add(userDetails);

            await _confirmationService.SendEmail(UserDefaultValues.FakeConfirmNum, UserDefaultValues.FakeEmail, UserDefaultValues.PathTo);
            _sendGridMock.Verify(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task ConfirmUser_ShouldChangeRole()
        {
            var user = new User { 
                Id = DefaultValues.Guid,
                Email = UserDefaultValues.ConfirmUser.Email
            };

            var confUser = new UserConfirmation
            {
                Id = DefaultValues.Guid,
                ConfDateTime = DateTime.Now,
                Number = Int32.Parse(UserDefaultValues.ConfirmUser.ConfirmationNumber)
            };

            _testUsersDb.Add(user);
            _testUserConfirmationDb.Add(confUser);

            await _confirmationService.ConfirmUser(UserDefaultValues.ConfirmUser);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ConfirmUser_Exception()
        {
            Func<Task> result = async () => await _confirmationService.ConfirmUser(UserDefaultValues.ConfirmUser);
            await result.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task ConfirmPassword_ShouldChangePassword()
        {
            var user = new User
            {
                Id = DefaultValues.Guid,
                Email = UserDefaultValues.confirmPasswordChanging.Email,
                PasswordHash = PasswordHasher.HashPassword("oldpassword")
            };

            var confUser = new UserConfirmation
            {
                Id = DefaultValues.Guid,
                Number = Int32.Parse(UserDefaultValues.confirmPasswordChanging.ConfirmationNumber),
                ConfDateTime = DateTime.Now
            };

            _testUserConfirmationDb.Add(confUser);
            _testUsersDb.Add(user);

            await _confirmationService.ConfirmPassword(UserDefaultValues.confirmPasswordChanging);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        private IEnumerable<User> GetUsersTestData()
        {
            var data = new List<User>();

            for (int i = 0; i < DefaultValues.ListSize; i++)
            {
                var dataBuilder = new UserTestBuilder();
                data.Add(dataBuilder
                    .WithId(new Guid())
                    .WithDefault()
                    .Build());
            }

            return data.AsEnumerable();
        }

        private IEnumerable<Role> GetRolesTestData()
        {
            var data = new List<Role>();

            data.Add(new Role()
            {
                Id = Guid.NewGuid(),
                Name = "Unconfirmed",
            });

            data.Add(new Role()
            {
                Id = Guid.NewGuid(),
                Name = "Guest"
            });


            return data.AsEnumerable();
        }

        private IEnumerable<UserConfirmation> GetUserConfirmationTestData()
        {
            var data = new List<UserConfirmation>();
            return data.AsEnumerable();
        }

        private IEnumerable<UserDetails> GetUserDetailsTestData()
        {
            var data = new List<UserDetails>();
            return data.AsEnumerable();
        }
    }
}
