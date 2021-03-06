﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Exceptions;
using Microsoft.Extensions.Options;
using TheraLang.BLL.DataTransferObjects.Donations;
using TheraLang.BLL.Interfaces;
using TheraLang.BLL.LiqPay;
using TheraLang.DAL.Entities;
using TheraLang.DAL.UnitOfWork;

namespace TheraLang.BLL.Services
{
    public class DonationService : IDonationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LiqPayKeys _liqPayKeys;

        public DonationService(IUnitOfWork unitOfWork, IOptions<LiqPayKeys> liqPayKeyOptions)
        {
            _unitOfWork = unitOfWork;
            _liqPayKeys = liqPayKeyOptions.Value;
        }

        public async Task<DonationDto> GetDonation(Guid donationId)
        {
            var donation = await _unitOfWork.Repository<Donation>().Get(d => d.Id == donationId) ??
                           throw new NotFoundException("Donations");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Donation, DonationDto>()).CreateMapper();
            var projectsDto = mapper.Map<Donation, DonationDto>(donation);

            return projectsDto;
        }

        public async Task<Guid> AddDonation(LiqPayCheckoutDto liqPayCheckoutDto)
        {
            var liqPayData = new LiqPayData(liqPayCheckoutDto.Data);
            var liqPaySignature = new LiqPaySignature(liqPayData, _liqPayKeys.PrivateKey);

            if (!await liqPaySignature.Validate(liqPayCheckoutDto.Signature))
            {
                throw new InvalidArgumentException(nameof(liqPayCheckoutDto.Signature),
                    "Invalid signature");
            } 
            
            var donation = liqPayData.Donation;
            
            var commissionModel = liqPayData.Commission;
            donation.Amount -= commissionModel.ReceiverCommission;
            
            donation.ProjectId = liqPayCheckoutDto.ProjectId;
            donation.SocietyId = liqPayCheckoutDto.SocietyId;
            donation.DonatorId = liqPayCheckoutDto.DonatorId;

            _unitOfWork.Repository<Donation>().Add(donation);
            await _unitOfWork.SaveChangesAsync();
            return donation.Id;
        }
    }
}