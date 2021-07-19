using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Junto.Infra.Data;
using JuntoSeguros.Business;
using Junto.Dominio;
using Junto.Infra.Security;
using Microsoft.OpenApi.Models;
using Junto.Infra;

namespace JuntoSeguros
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // Configurando o acesso a dados
            services.AddDbContext<BancoDbContext>(options =>
                options.UseInMemoryDatabase("InMemoryDatabase"));
            services.AddScoped<SegurosService>();

            // Configurando o acesso às tabelas do ASP.NET Identity Core
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("InMemoryDatabase"));

            #region configuração do token e acesso
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<AccessManager>();

            var signingConfigurations = new SigningConfigurations();
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenConfigurations();
            new ConfigureFromConfigurationOptions<TokenConfigurations>(
                Configuration.GetSection("TokenConfigurations"))
                    .Configure(tokenConfigurations);
            services.AddSingleton(tokenConfigurations);

            services.AddJwtSecurity(signingConfigurations, tokenConfigurations);
            #endregion

            services.AddCors(op =>
            {
                op.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials().Build());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Junto Seguros API",
                    Version = "v1",
                    Description = "API_1 Cadastro Usuarios",
                });
            });

            services.AddControllers();

            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //criação do banco default
            new IdentityInitializer(context, userManager, roleManager).Initialize();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "JUNTO SEGUROS API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
