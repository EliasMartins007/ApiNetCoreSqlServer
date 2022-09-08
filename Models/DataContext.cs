using Microsoft.EntityFrameworkCore;//original using Microsoft.EntityFrameworkCore;
using System.Data;

namespace herois.Models
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {

        }
        //public DbSet<SuperHero> Superheros { get; set; }
        public DbSet<SuperHero> Superheros => Set<SuperHero>();//novo 08/09/2022
    }
}
