using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClubManagement.Repository.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ClubManagementContext _context;

    public DashboardRepository(ClubManagementContext context)
    {
        _context = context;
    }

    // ==========================================
    // OVERVIEW STATISTICS
    // ==========================================
    
    public async Task<int> GetTotalClubsAsync()
    {
        return await _context.Clubs.CountAsync();
    }

    public async Task<int> GetTotalMembersAsync()
    {
        return await _context.Users
            .Where(u => u.Role == "Student")
            .CountAsync();
    }

    public async Task<int> GetTotalActivitiesAsync()
    {
        return await _context.Activities.CountAsync();
    }

    public async Task<int> GetPendingJoinRequestsAsync()
    {
        return await _context.JoinRequests
            .Where(jr => jr.Status == "Pending")
            .CountAsync();
    }

    // ==========================================
    // CLUB STATISTICS
    // ==========================================
    
    public async Task<Dictionary<string, int>> GetTopClubsByMembersAsync(int topCount = 5)
    {
        return await _context.Clubs
            .Select(c => new
            {
                c.ClubName,
                MemberCount = c.Memberships.Count(m => m.Status == "Active")
            })
            .OrderByDescending(x => x.MemberCount)
            .Take(topCount)
            .ToDictionaryAsync(x => x.ClubName, x => x.MemberCount);
    }

    public async Task<Dictionary<string, int>> GetActivitiesByClubAsync()
    {
        return await _context.Clubs
            .Select(c => new
            {
                c.ClubName,
                ActivityCount = c.Activities.Count
            })
            .ToDictionaryAsync(x => x.ClubName, x => x.ActivityCount);
    }

    // ==========================================
    // FINANCIAL STATISTICS
    // ==========================================
    
    public async Task<decimal> GetTotalRevenueAsync()
    {
        return await _context.Payments
            .Where(p => p.Status == "Paid")
            .SumAsync(p => p.Amount);
    }

    public async Task<int> GetPaidPaymentsCountAsync()
    {
        return await _context.Payments
            .Where(p => p.Status == "Paid")
            .CountAsync();
    }

    public async Task<int> GetPendingPaymentsCountAsync()
    {
        return await _context.Payments
            .Where(p => p.Status == "Pending")
            .CountAsync();
    }

    public async Task<List<(string FullName, string ClubName, decimal Amount)>> GetUnpaidFeesAsync()
    {
        var unpaidFees = await _context.Payments
            .Where(p => p.Status == "Pending")
            .Include(p => p.User)
            .Include(p => p.Fee)
                .ThenInclude(f => f.Club)
            .Select(p => new
            {
                p.User.FullName,
                ClubName = p.Fee.Club.ClubName,
                p.Fee.Amount
            })
            .ToListAsync();

        return unpaidFees
            .Select(x => (x.FullName, x.ClubName, x.Amount))
            .ToList();
    }

    // ==========================================
    // ACTIVITY STATISTICS
    // ==========================================
    
    public async Task<List<(string ActivityName, DateTime StartDate, string ClubName)>> GetUpcomingActivitiesAsync(int days = 30)
    {
        var fromDate = DateTime.Now;
        var toDate = fromDate.AddDays(days);

        var upcomingActivities = await _context.Activities
            .Where(a => a.StartDate >= fromDate && a.StartDate <= toDate)
            .Include(a => a.Club)
            .OrderBy(a => a.StartDate)
            .Select(a => new
            {
                a.ActivityName,
                a.StartDate,
                a.Club.ClubName
            })
            .ToListAsync();

        return upcomingActivities
            .Select(x => (x.ActivityName, x.StartDate, x.ClubName))
            .ToList();
    }

    public async Task<Dictionary<string, int>> GetActivityParticipationStatsAsync()
    {
        var stats = await _context.ActivityParticipants
            .GroupBy(ap => ap.Status)
            .Select(g => new
            {
                Status = g.Key ?? "Unknown",
                Count = g.Count()
            })
            .ToDictionaryAsync(x => x.Status, x => x.Count);

        return stats;
    }

    // ==========================================
    // MEMBERSHIP STATISTICS
    // ==========================================
    
    public async Task<Dictionary<string, int>> GetMembershipsByStatusAsync()
    {
        return await _context.Memberships
            .GroupBy(m => m.Status)
            .Select(g => new
            {
                Status = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(x => x.Status, x => x.Count);
    }

    public async Task<Dictionary<int, int>> GetMonthlyJoinTrendsAsync(int months = 6)
    {
        var fromDate = DateTime.Now.AddMonths(-months);

        var trends = await _context.Memberships
            .Where(m => m.JoinedAt >= fromDate)
            .GroupBy(m => new { m.JoinedAt!.Value.Year, m.JoinedAt.Value.Month })
            .Select(g => new
            {
                YearMonth = g.Key.Year * 100 + g.Key.Month, // Format: 202412
                Count = g.Count()
            })
            .OrderBy(x => x.YearMonth)
            .ToDictionaryAsync(x => x.YearMonth, x => x.Count);

        return trends;
    }
}