using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace XPenC.DataAccess.EntityFrameworkCore
{
	[ExcludeFromCodeCoverage]
	public class XPenCDbContextFactory : IDesignTimeDbContextFactory<XPenCDbContext>
	{
		public XPenCDbContext CreateDbContext(string[] args)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
			var config = builder.Build();

			var optionsBuilder = new DbContextOptionsBuilder<XPenCDbContext>();
			optionsBuilder.UseSqlServer(config.GetConnectionString("DataContext"));
			return new XPenCDbContext(optionsBuilder.Options);
		}
	}
}
