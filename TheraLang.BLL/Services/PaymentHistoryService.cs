﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Exceptions;
using System.Threading.Tasks;
using TheraLang.BLL.DataTransferObjects;
using TheraLang.BLL.Interfaces;
using TheraLang.DAL.Entities;
using TheraLang.DAL.UnitOfWork;
using Common;

namespace TheraLang.BLL.Services
{
    public class PaymentHistoryService : IPaymentHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserManagementService _userManagementService;

        public PaymentHistoryService(IUnitOfWork unitOfWork, IUserManagementService userManagementService)
        {
            _unitOfWork = unitOfWork;
           _userManagementService = userManagementService;
        }

        public async Task<int> GetUserPaymentHistoryCount(Guid userId)
        {
            return await _unitOfWork.Repository<PaymentHistory>()
                .GetAll()
                .Where(ph => ph.UserId == userId)
                .CountAsync();
        }

        public async Task<int> GetAllPaymentHistoryCount()
        {
            return await _unitOfWork.Repository<PaymentHistory>()
                .GetAll()
                .CountAsync();
        }
        public async Task Add(PaymentHistoryDto paymentDto)
        {
            var currentBalance = (await _userManagementService.GetUserById(paymentDto.UserId)).Details.Balance;

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<PaymentHistoryDto, PaymentHistory>()
                .ForMember(p => p.CurrentBalance, opt => opt.MapFrom(n => currentBalance)))
                .CreateMapper();
            var payment = mapper.Map<PaymentHistoryDto, PaymentHistory>(paymentDto);

            _unitOfWork.Repository<PaymentHistory>().Add(payment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<PaymentHistoryDto>> GetAll()
        {

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<PaymentHistory,PaymentHistoryDto>()
                .ForMember(p => p.UserName, opt => opt.MapFrom(n => $"{n.Payer.Details.FirstName} {n.Payer.Details.LastName}")));

            var paymantsDto = await _unitOfWork.Repository<PaymentHistory>()
                .GetAll()
                .ProjectTo<PaymentHistoryDto>(mapper)
                .ToListAsync();
            
            if(!paymantsDto.Any())
            {
                throw new NotFoundException("Payments history not found!");
            }

            return paymantsDto;
        }

        public async Task<IEnumerable<PaymentHistoryDto>> GetByUserId(Guid userId)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<PaymentHistory, PaymentHistoryDto>()
                .ForMember(p => p.UserName, opt => opt.MapFrom(n => $"{n.Payer.Details.FirstName} {n.Payer.Details.LastName}")));

            var paymantsDto = await _unitOfWork.Repository<PaymentHistory>()
                .GetAll()
                .Where(p => p.UserId == userId)
                .ProjectTo<PaymentHistoryDto>(mapper)
                .ToListAsync();

            if (!paymantsDto.Any())
            {
                throw new NotFoundException("Payments history not found!");
            }

            return paymantsDto;
        }

        public async Task<IEnumerable<PaymentHistoryDto>> GetPageByUserId(Guid userId, PaginationParams pagingParameters)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<PaymentHistory, PaymentHistoryDto>()
                .ForMember(p => p.UserName, opt => opt.MapFrom(n => $"{n.Payer.Details.FirstName} {n.Payer.Details.LastName}")));

            var paymantsDto = await _unitOfWork.Repository<PaymentHistory>()
                .GetAll()
                .Where(p => p.UserId == userId)
                .Skip(pagingParameters.Skip)
                .Take(pagingParameters.Take)
                .ProjectTo<PaymentHistoryDto>(mapper)
                .ToListAsync();

            if (!paymantsDto.Any())
            {
                throw new NotFoundException("Payments history not found!");
            }

            return paymantsDto;
        }

        public async Task<IEnumerable<PaymentHistoryDto>> GetHistoryPage(PaginationParams pagingParameters)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<PaymentHistory, PaymentHistoryDto>()
                .ForMember(p => p.UserName, opt => opt.MapFrom(n => $"{n.Payer.Details.FirstName} {n.Payer.Details.LastName}")));

            var paymantsDto = await _unitOfWork.Repository<PaymentHistory>()
                .GetAll()
                .Skip(pagingParameters.Skip)
                .Take(pagingParameters.Take)
                .ProjectTo<PaymentHistoryDto>(mapper)
                .ToListAsync();

            if (!paymantsDto.Any())
            {
                throw new NotFoundException("Payments history not found!");
            }

            return paymantsDto;
        }
    }
}
