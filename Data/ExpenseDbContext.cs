using ExpenseTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerApi.Data;

public class ExpenseDbContext : DbContext
{
    public ExpenseDbContext(
        DbContextOptions<ExpenseDbContext> options
    ) : base(options)
    {
        
    }

    public DbSet<Expenses> Expenses => Set<Expenses>();
}