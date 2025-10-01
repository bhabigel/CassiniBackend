using System.Text;
using CassiniConnect.Core.Persistance;
using CassiniConnect.Core.Utilities.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public class CassiniApp()
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = BuildApp(args);
            RunApp(builder);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static WebApplicationBuilder BuildApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy("CorsPolicy", policy =>
            {
                policy.WithOrigins("https://cassini-org.info", "http://localhost:5201", "http://localhost:5173/", "https://cassini-app.fly.dev")
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        builder.Services.AddDbContext<DataContext>(opt =>
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string? connstring;
            Console.WriteLine(env);
            if (env == "Development")
            {
                builder.Configuration.AddUserSecrets<CassiniApp>();
                connstring = builder.Configuration.GetConnectionString("DefaultConnection");
            }
            else
            {
                throw new Exception("Program csak development verzióban működik jelenleg!");
            }
            opt.UseNpgsql(connstring);
        });

        var jwtSettings = new JwtSettings();
        builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);


        if (string.IsNullOrEmpty(jwtSettings.Issuer) ||
            string.IsNullOrEmpty(jwtSettings.Audience) ||
            string.IsNullOrEmpty(jwtSettings.SecretKey))
        {
            throw new Exception("JwtSettings beállítás hiányos vagy nem elérhető!");
        }

        builder.Services.AddSingleton(jwtSettings);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            };
        });


        return builder;
    }

    private static void RunApp(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        app.UseCors("CorsPolicy");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();
        app.MapFallbackToFile("index.html");

        try
        {
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                context.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<CassiniApp>>();
            logger.LogError(ex, "Hiba migráció közben");
        }

        app.Run();
    }
}






