using BL.Domain;
using DAL.EF;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Implementations;

public class ProjectRepository : Repository, IProjectRepository
{
    private readonly PhygitalDbContext _context;

    public ProjectRepository(PhygitalDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Flow>> GetFlowsByProjectIdAsync(int projectId)
    {
        return await _context.Set<Flow>().Where(f => f.ProjectId == projectId).ToListAsync();
    }

    public async Task<IEnumerable<Flow>> GetParentFlowsByProjectIdAsync(int projectId)
    {
        return await _context.Set<Flow>().Where(f => f.ProjectId == projectId && f.ParentFlowId == null).ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetProjectsByAdminIdAsync(string adminId)
    {
        return await _context.Set<Project>().Where(p => p.AdminId == adminId).ToListAsync();
    }

    public ValueTask<Project> FindByIdAsync(int id)
    {
        return _context.Set<Project>().FindAsync(id);
    }

    public async Task<IEnumerable<Flow>> FindAvailableFlowsByProjectIdAsync(int projectId, DateTime date)
    {
        return await _context.Set<Flow>().Where(f => f.ProjectId == projectId &&
                                                     f.ParentFlowId == null &&
                                                     f.StartDate <= date &&
                                                     f.EndDate >= date).ToListAsync();
    }

    public IEnumerable<Project> GetProjectsByFacilitatorId(string userId)
    {
        return _context.Set<ProjectFacilitator>()
            .Where(pf => pf.FacilitatorId == userId)
            .Select(pf => pf.Project)
            .ToList();
    }

    public IEnumerable<IdentityUser> GetSearchedFacilitators(string searchTerm)
    {
        return _context.Set<IdentityUser>()
            .Where(u => u.UserName.Contains(searchTerm))
            .ToList();
    }

    public IEnumerable<IdentityUser> GetFacilitatorsByProjectId(int projectId)
    {
        return _context.Set<ProjectFacilitator>()
            .Where(pf => pf.ProjectId == projectId)
            .Select(pf => pf.Facilitator)
            .ToList();
    }

    public bool AddFacilitatorToProject(string userId, int projectId)
    {
        var projectFacilitator = new ProjectFacilitator
        {
            ProjectId = projectId,
            FacilitatorId = userId
        };

        _context.Set<ProjectFacilitator>().Add(projectFacilitator);
        return _context.SaveChanges() > 0;
    }

    public bool RemoveFacilitatorFromProject(string userId, int projectId)
    {
        var projectFacilitator = _context.Set<ProjectFacilitator>()
            .FirstOrDefault(pf => pf.ProjectId == projectId && pf.FacilitatorId == userId);

        if (projectFacilitator == null) return false;

        _context.Set<ProjectFacilitator>().Remove(projectFacilitator);
        return _context.SaveChanges() > 0;
    }
}