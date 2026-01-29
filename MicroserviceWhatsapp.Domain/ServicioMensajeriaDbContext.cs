using MicroserviceWhatsapp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceWhatsapp.Domain
{
    public class ServicioMensajeriaDbContext : DbContext
    {
        public ServicioMensajeriaDbContext(DbContextOptions<ServicioMensajeriaDbContext> options) : base(options)
        { }
        public DbSet<UserLogin> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
       

    }


}
