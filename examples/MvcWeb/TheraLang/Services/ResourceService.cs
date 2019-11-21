﻿using MvcWeb.TheraLang.Entities;
using MvcWeb.TheraLang.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcWeb.TheraLang.Services
{
    public class ResourceService : IResourceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ResourceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Resource GetResourceById(int id)
        {
            try
            {
                Resource resource = _unitOfWork.Repository<Resource>().Get().SingleOrDefault(i => i.Id == id);
                return resource;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when geting resource by {nameof(id)}={id}: ", ex);
            }
        }

        public async Task AddResource(Resource resource)
        {
            try
            {
                resource.CreatedDateUtc = DateTime.UtcNow;
                await _unitOfWork.Repository<Resource>().Add(resource);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when adding the {nameof(resource)}: {resource.Id} ", ex);
            }
        }

        public async Task UpdateResource(Resource resource, int updatetById)
        {
            try
            {
                resource.UpdatedDateUtc = DateTime.UtcNow;
                resource.UpdatedById = updatetById;

                _unitOfWork.Repository<Resource>().Update(resource);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when updating the {nameof(resource)}: {resource.Id}: ", ex);
            }
        }

        public async Task RemoveResource(int id)
        {
            try
            {
                Resource resource = _unitOfWork.Repository<Resource>().Get().SingleOrDefault(i => i.Id == id);
                _unitOfWork.Repository<Resource>().Remove(resource);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when remove resource by {nameof(id)}: {id}: ", ex);
            }
        }

        public IEnumerable<Resource> GetResourcesByCategoryId(int categoryId, int pageNumber, int recordsPerPage)
        {
            try
            {
                var resources = _unitOfWork.Repository<Resource>().Get().Where(x => x.CategoryId == categoryId);
                var joinedResources = (from res in resources
                                       select new Resource
                                       {
                                           Id = res.Id,
                                           User = res.User,
                                           Name = res.Name,
                                           Description = res.Description,
                                           Url = res.Url,
                                           File = res.File,
                                           CategoryId = res.CategoryId,
                                           ResourceCategory = res.ResourceCategory,
                                           ResourceToProjects = res.ResourceToProjects,
                                           UpdatedById = res.UpdatedById,
                                           CreatedDateUtc = res.CreatedDateUtc,
                                           UpdatedDateUtc = res.UpdatedDateUtc,
                                       });

                var resourcesPerPages = joinedResources.Skip((pageNumber - 1) * recordsPerPage)
                    .Take(recordsPerPage).ToList();
                return resourcesPerPages;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when get all resources: ", ex);
            }
        }

        public int CountResourcesByCategoryId(int categoryId)
        {
            try
            {
                var resourcesCount = _unitOfWork.Repository<Resource>().Get().Count(x => x.CategoryId == categoryId);
                return resourcesCount;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when get count resources by category id: ", ex);
            }
        }

        public IEnumerable<ResourceCategory> GetResourcesCategories(bool withAssignedResources)
        {
            try
            {
                var query = _unitOfWork.Repository<ResourceCategory>().Get();

                if (withAssignedResources)
                {
                    query = query.Where(x => x.Resources.Any());
                }

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when get all ResourceCategories: ", ex);
            }
        }

        public IEnumerable<Resource> GetAllResourcesByProjectId(int projectId)
        {
            try
            {
                var resources = _unitOfWork.Repository<Resource>().Get().Where(x => x.ResourceToProjects.Any(c => c.ProjectId == projectId));
                var joinedResources = (from res in resources
                                       select new Resource
                                       {
                                           Id = res.Id,
                                           User = res.User,
                                           Name = res.Name,
                                           Description = res.Description,
                                           Url = res.Url,
                                           File = res.File,
                                           CategoryId = res.CategoryId,
                                           ResourceCategory = res.ResourceCategory,
                                           ResourceToProjects = res.ResourceToProjects,
                                           UpdatedById = res.UpdatedById,
                                           CreatedDateUtc = res.CreatedDateUtc,
                                           UpdatedDateUtc = res.UpdatedDateUtc,
                                       });
                return joinedResources.ToList();
            }
            catch (Exception ex)
            {

                throw new Exception($"Error when get all resources by project id: ", ex);
            }
        }
    }
}