﻿using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TheraLang.BLL.Services;
using TheraLang.DAL.Entities;
using TheraLang.DAL.Repository;
using TheraLang.DAL.UnitOfWork;
using Xunit;
using FluentAssertions;
using TheraLang.BLL.DataTransferObjects;
using Common.Exceptions;

namespace TheraLang.Tests.Services
{
    public class MemberFeeServiceTests
    {
        private List<MemberFee> fakeMemberFees;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<IRepository<MemberFee>> mockRepo;

        public MemberFeeServiceTests()
        {
            fakeMemberFees = new List<MemberFee>
            {
                 new MemberFee {Id = 1,FeeDate = new DateTime(2020,1,1),FeeAmount = 200 },
                 new MemberFee {Id = 2,FeeDate = new DateTime(2020,2,1),FeeAmount = 300 },
                 new MemberFee {Id = 3,FeeDate = new DateTime(2020,3,1),FeeAmount = 400 }
            };

            mockRepo = new Mock<IRepository<MemberFee>>();
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(x => x.Repository<MemberFee>()).Returns(mockRepo.Object);
        }

        [Fact]
        public void GetMemberFeesAsync_ShouldReturnAllFees()
        {
            mockRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MemberFee, bool>>>()))
                .ReturnsAsync((Expression<Func<MemberFee, bool>> expression) => fakeMemberFees);

            MemberFeeService memberFeeService = new MemberFeeService(mockUnitOfWork.Object);
            var result = memberFeeService.GetMemberFeesAsync();
            
            result.Should().NotBeNull();
            result.Result.Count().Should().Be(3);
        }

        [Fact]
        public void DeleteAsync_ShouldDeleteOneFee()
        {
            mockUnitOfWork.Setup(x => x.SaveChangesAsync()).Verifiable();
            mockRepo.Setup(x => x.Get(It.IsAny<Expression<Func<MemberFee, bool>>>()))
                .ReturnsAsync((Expression<Func<MemberFee, bool>> expression) => fakeMemberFees
                .AsQueryable()
                .Where(expression)
                .FirstOrDefault());

            mockRepo.Setup(x => x.Remove(It.IsAny<MemberFee>())).Verifiable();
            MemberFeeService memberFeeService = new MemberFeeService(mockUnitOfWork.Object);

            var result = memberFeeService.DeleteAsync(1);

            mockRepo.Verify(x => x.Remove(It.IsAny<MemberFee>()), Times.Once());
            mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public void DeleteAsync_ShouldThrowArgumentNullException()
        {            
            mockRepo.Setup(x => x.Get(It.IsAny<Expression<Func<MemberFee, bool>>>())).ReturnsAsync(() => null);
            MemberFeeService memberFeeService = new MemberFeeService(mockUnitOfWork.Object);
            
            int id = 4;
            Func<Task> act = async () => await memberFeeService.DeleteAsync(id);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddAsync_ShouldAddMemberFee()
        {            
            mockUnitOfWork.Setup(x => x.SaveChangesAsync()).Verifiable();
            mockRepo.Setup(x => x.Add(It.IsAny<MemberFee>())).Verifiable();
            mockRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<MemberFee, bool>>>()))
                 .ReturnsAsync((Expression<Func<MemberFee, bool>> expression) => fakeMemberFees);

            MemberFeeService memberFeeService = new MemberFeeService(mockUnitOfWork.Object);

            var result = memberFeeService.AddAsync(new MemberFeeDto() 
                { FeeDate = new DateTime(2022,7,20),FeeAmount = 500});

            mockRepo.Verify(x => x.Add(It.IsAny<MemberFee>()), Times.Once());
            mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once());
        }

        [Fact]
        public void AddAsync_ShouldThrowNullReferenceException()
        {
            mockUnitOfWork.Setup(x => x.SaveChangesAsync()).Verifiable();
            mockRepo.Setup(x => x.Add(It.IsAny<MemberFee>())).Verifiable();

            MemberFeeService memberFeeService = new MemberFeeService(mockUnitOfWork.Object);

            Func<Task> act = async () => await memberFeeService.AddAsync(null);

            mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never());
            mockRepo.Verify(x => x.Add(It.IsAny<MemberFee>()), Times.Never());
            act.Should().Throw<NullReferenceException>();
        }
        [Fact]
        public void AddAsync_ShouldThrowInvalidArgumentException()
        {
            mockUnitOfWork.Setup(x => x.SaveChangesAsync()).Verifiable();
            mockRepo.Setup(x => x.Add(It.IsAny<MemberFee>())).Verifiable();

            MemberFeeService memberFeeService = new MemberFeeService(mockUnitOfWork.Object);

            Func<Task> act = async () => await memberFeeService.AddAsync(new MemberFeeDto()
            { FeeDate = new DateTime(2020, 2, 20), FeeAmount = 500 });

            mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never());
            mockRepo.Verify(x => x.Add(It.IsAny<MemberFee>()), Times.Never());
            act.Should().Throw<InvalidArgumentException>();
        }
    }
}
