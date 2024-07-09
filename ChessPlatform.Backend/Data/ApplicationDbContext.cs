using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChessPlatform.Backend.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    
}
