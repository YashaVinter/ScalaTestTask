using ScalaTestTask.services.implementations;
using ScalaTestTask.services.interfaces;
using System;
namespace ScalaTestTask.extensions
{
    public static class BuilderExtensions
    {
        public static void AddDbContext(this IServiceCollection services)
        {
            services.AddSingleton<AppDBContext>();
        }
    }
}
