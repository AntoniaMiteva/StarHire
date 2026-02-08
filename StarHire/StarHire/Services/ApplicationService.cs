using Microsoft.EntityFrameworkCore;
using StarHire.Data;
using StarHire.Models;

namespace StarHire.Services;

public class ApplicationService
{
    private readonly ApplicationDbContext db;

    public ApplicationService(ApplicationDbContext db)
    {
        this.db = db;
    }

    public async Task ApplyAsync(int jobId, string userId, string message)
    {
        var alreadyApplied = await db.Applications
            .AnyAsync(a => a.JobId == jobId && a.AlienId == userId);

        if (alreadyApplied)
        {
            throw new InvalidOperationException("Вече си кандидатствал за тази обява.");
        }

        var application = new Application
        {
            JobId = jobId,
            AlienId = userId,
            Status = ApplicationStatus.Pending
        };

        db.Applications.Add(application);
        await db.SaveChangesAsync();
    }

    public async Task<List<Application>> GetMyApplicationsAsync(string userId)
    {
        return await db.Applications
            .Include(a => a.Job)
            .Where(a => a.AlienId == userId)
            .ToListAsync();
    }
}
