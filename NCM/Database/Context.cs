
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;

namespace NCM.Database
{
    public class Context : DbContext
    {
        public DbSet<Adapter> Adapters { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
    }
}
