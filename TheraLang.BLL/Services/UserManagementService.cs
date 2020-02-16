﻿using Common.Helpers.PasswordHelper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TheraLang.BLL.DataTransferObjects;
using TheraLang.BLL.Interfaces;
using TheraLang.DAL.Entities;
using TheraLang.DAL.UnitOfWork;
using TheraLang.BLL.DataTransferObjects;
using AutoMapper;

namespace TheraLang.BLL.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public UserManagementService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }


        public async Task<User> GetUser(string email, string password)
        {
            var user = await _unitOfWork.Repository<User>().GetAll().Include(x => x.Role)
                     .FirstOrDefaultAsync(u => u.Email == email);
            if (PasswordHasher.VerifyHashedPassword(user.PasswordHash, password))
            {
                return user;
            }
            else
            {
                throw new Exception($"Cannot get user with {nameof(email)}: {email}.");
            }

        }

        public async Task<User> GetUserById(Guid id)
        {
            try
            {
                var user = await _unitOfWork.Repository<User>().Get(u => u.Id == id);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot get project with {nameof(id)}: {id}.", ex);
            }
        }

        public async Task AddUser(UserAllDto NewUser)
        {
            try
            {
                if (NewUser != null)
                {
                    var mappertwo = new MapperConfiguration(cfg => cfg.CreateMap<UserAllDto, UserDetails>()).CreateMapper();
                    var userDetails = mappertwo.Map<UserAllDto, UserDetails>(NewUser);
                    if (NewUser.Image != null)
                    {
                        var imageUri = await _fileService.SaveFile(NewUser.Image.OpenReadStream(), Path.GetExtension(NewUser.Image.Name));
                        userDetails.ImageURl = imageUri.ToString();
                    }
                    _unitOfWork.Repository<UserDetails>().Add(userDetails);

                    var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserAllDto, User>()).CreateMapper();
                    var user = mapper.Map<UserAllDto, User>(NewUser);
                    user.RoleId = (await _unitOfWork.Repository<Role>().Get(r => r.Name == "Guest")).Id;
                    user.PasswordHash = PasswordHasher.HashPassword(NewUser.Password);
                    user.Details = userDetails;
                    _unitOfWork.Repository<User>().Add(user);

                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error when adding user ", ex);
            }
        }

    }
}