using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceApp.Models;

namespace PoliceApp.test
{
    public class TestBase
    {
        protected PoliceDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<PoliceDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            return new PoliceDbContext(options);
        }
    }

}
