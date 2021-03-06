﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TheraLang.BLL.DataTransferObjects.NewsDtos;
using TheraLang.BLL.Interfaces;
using TheraLang.DAL.Entities;
using TheraLang.DAL.UnitOfWork;
using TheraLang.DAL.Entities.ManyToMany;
using Common;
using AutoMapper.QueryableExtensions;
using Common.Exceptions;

namespace TheraLang.BLL.Services
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IAuthenticateService _authenticateService;

        public NewsService(IUnitOfWork unitOfWork, IFileService fileService, IAuthenticateService authenticateService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _authenticateService = authenticateService;
        }

        public async Task<int> GetNewsCount()
        {
            return await _unitOfWork.Repository<News>().GetAll().CountAsync();
        }

        public async Task<IEnumerable<NewsPreviewDto>> GetAllNews()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<News, NewsPreviewDto>()
                    .ForMember(m => m.AuthorName,
                    opt => opt.MapFrom(sm => $"{sm.Author.Details.FirstName} {sm.Author.Details.LastName}"));
            });

            var newsDtos = await _unitOfWork.Repository<News>().GetAll()
                .OrderByDescending(e => e.CreatedDateUtc)
                .ProjectTo<NewsPreviewDto>(mapper)
                .ToListAsync();

            if (!newsDtos.Any())
            {
                throw new NotFoundException("News");
            }

            return newsDtos;
        }

        public async Task<IEnumerable<NewsPreviewDto>> GetNewsPage(PaginationParams paginationParams)
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<News, NewsPreviewDto>()
                    .ForMember(m => m.AuthorName,
                        opt => opt.MapFrom(sm => $"{sm.Author.Details.FirstName} {sm.Author.Details.LastName}"));
            });

            var newsDtos = await _unitOfWork.Repository<News>().GetAll()
                .OrderByDescending(e => e.CreatedDateUtc)
                .Skip(paginationParams.Skip)
                .Take(paginationParams.Take)
                .ProjectTo<NewsPreviewDto>(mapper)
                .ToListAsync();

            if (!newsDtos.Any())
            {
                throw new NotFoundException($"News page {paginationParams.PageNumber}");
            }

            return newsDtos;
        }

        public async Task<NewsDetailsDto> GetNewsById(int id)
        {
            var currentUser = await _authenticateService.TryGetAuthUser();

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<News, NewsDetailsDto>()
                    .ForMember(m => m.ContentImageUrls,
                        opt => opt.MapFrom(sm => sm.UploadedContentImages.Select(i => i.Url)))
                    .ForMember(m => m.AuthorName,
                        opt => opt.MapFrom(sm => $"{sm.Author.Details.FirstName} {sm.Author.Details.LastName}"))
                    .ForMember(m => m.LikesCount, opt => opt.MapFrom(sm => sm.Likes.Count))
                    .ForMember(m => m.IsLikedByCurrentUser,
                        opt =>
                        {
                            opt.Condition(sm => currentUser != null);
                            opt.MapFrom(sm => sm.Likes.Select(u => u.UserThatLikedId).Contains(currentUser.Id));
                        });
            }).CreateMapper();

            var news = await _unitOfWork.Repository<News>().GetAll()
                .Where(n => n.Id == id)
                .Include(n => n.Author)
                .ThenInclude(a => a.Details)
                .Include(n => n.UploadedContentImages)
                .Include(n => n.Likes)
                .SingleOrDefaultAsync();

            // Cannot use ProjectTo With conditional mapping
            var newsDto = mapper.Map<NewsDetailsDto>(news);

            if (newsDto == null)
            {
                throw new NotFoundException($"News with id {id}");
            }

            return newsDto;
        }

        public async Task AddNews(NewsCreateDto newsDto)
        {
            var user = await _authenticateService.GetAuthUser();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<NewsCreateDto, News>()
                    .ForMember(m => m.CreatedById, opt => opt.MapFrom(sm => user.Id)))
                .CreateMapper();

            var news = mapper.Map<NewsCreateDto, News>(newsDto);

            news.MainImageUrl = (await UploadImage(newsDto.MainImage)).Url;
            news.UploadedContentImages = new List<UploadedNewsContentImage>();
            foreach (var image in newsDto.ContentImages)
            {
                news.UploadedContentImages.Add(await UploadImage(image));
            }

            _unitOfWork.Repository<News>().Add(news);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveNews(int id)
        {
            var news = await _unitOfWork.Repository<News>().Get(i => i.Id == id);
            if (news == null)
            {
                throw new NotFoundException($"News with id {id}");
            }

            _unitOfWork.Repository<News>().Remove(news);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateNews(int id, NewsEditDto newsDto)
        {
            var newsToUpdate = await _unitOfWork.Repository<News>().GetAll()
                .Include(e => e.UploadedContentImages)
                .SingleOrDefaultAsync(n => n.Id == id);

            if (newsToUpdate == null)
            {
                throw new NotFoundException($"News with id {id} not found!");
            }

            var user = await _authenticateService.GetAuthUser();

            newsToUpdate.UpdatedById = user.Id;
            newsToUpdate.Title = newsDto.Title;
            newsToUpdate.Text = newsDto.Text;

            //update main image if need
            if (newsDto.NewMainImage != null)
            {
                //await DeleteImageFile(newsToUpdate.MainImageUrl);
                newsToUpdate.MainImageUrl = (await UploadImage(newsDto.NewMainImage)).Url;
            }

            //delete doesn't used images
            foreach (var uploadedImage in newsToUpdate.UploadedContentImages)
            {
                if (!newsDto.NotDeletedContentImageUrls.Contains(uploadedImage.Url))
                {
                    await DeleteImage(uploadedImage);
                }
            }

            //upload new images
            foreach (var image in newsDto.AddedContentImages)
            {
                newsToUpdate.UploadedContentImages.Add(await UploadImage(image));
            }

            _unitOfWork.Repository<News>().Update(newsToUpdate);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Like(int id)
        {
            var user = await _authenticateService.GetAuthUser();
            var existedLikeFromUser = (await _unitOfWork.Repository<NewsLike>()
                .GetAllAsync(e => e.NewsId == id && e.UserThatLikedId == user.Id))
                .SingleOrDefault();

            //remove like if user already liked
            if (existedLikeFromUser != null)
            {
                _unitOfWork.Repository<NewsLike>().Remove(existedLikeFromUser);
            }
            else
            {
                var newLikeFromUser = new NewsLike() { NewsId = id, UserThatLikedId = user.Id };
                _unitOfWork.Repository<NewsLike>().Add(newLikeFromUser);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<UploadedNewsContentImage> UploadImage(IFormFile image)
        {
            if (image == null) throw new ArgumentNullException();

            using (var fileStream = image.OpenReadStream())
            {
                var imageExtension = Path.GetExtension(image.FileName);
                var imageUrl = await _fileService.SaveFile(fileStream, imageExtension);
                var uploadedImage = new UploadedNewsContentImage() { Url = imageUrl.ToString() };
                return uploadedImage;
            }
        }

        private async Task DeleteImage(UploadedNewsContentImage image)
        {
            await DeleteImageFile(image.Url);
            _unitOfWork.Repository<UploadedNewsContentImage>().Remove(image);
        }

        private async Task DeleteImageFile(string url)
        {
            await _fileService.RemoveFile(url);
        }
    }
}