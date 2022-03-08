using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CooperateMVC.Models;

namespace CooperateMVC.Data
{
    public class CooperateMVCContext : DbContext
    {
        public CooperateMVCContext (DbContextOptions<CooperateMVCContext> options)
            : base(options)
        {
        }

        public DbSet<CooperateMVC.Models.CreateKycRequest> CreateKycRequest { get; set; }

        public DbSet<CooperateMVC.Models.KycRequest> KycRequest { get; set; }

        public DbSet<CooperateMVC.Models.BankUser> BankUser { get; set; }
    }
}
