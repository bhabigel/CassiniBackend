using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CassiniConnect.Core.Models.UserCore;
using CassiniConnect.Core.Persistance;
using CassiniConnect.Core.Utilities.Config;
using CassiniConnect.Application.Mapping;
using CassiniConnect.Application.Models.RoleManagement;
using CassiniConnect.Application.Models.TeacherManagement.Subjects;
using CassiniConnect.Application.Models.TeacherManagement.Teachers;
using CassiniConnect.Application.Models.UserManagement;
using CassiniConnect.Application.Utilities;
using CassiniConnect.Application.Models.LanguageManagement;
using CassiniConnect.Application.Models.PresentationManagement;
using CassiniConnect.Application.Models.EventManagement;
using System.Security.Claims;

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


    /// <summary>
    /// WebApplicationBuilder bekonfigurálása, kapcsolatot teremt adatbázissal, 
    // egyéb belső és potenciális külső szolgáltatásokkal, minden dependency injection itt történik
    /// </summary>
    /// <param name="args">Potenciális, a parancssorról beküldött argumentumok</param>
    /// <returns>Használatra kész WebApplicationBuilder instance</returns>
    private static WebApplicationBuilder BuildApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Cassini API", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer {token}' (without quotes). Example: Bearer eyJhbGciOiJI..."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new List<string>()
                }
            });
        });

        //Cors-policy, milyen címekről érhető el az API - külső támadások elleni védelem
        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy("CorsPolicy", policy =>
            {
                policy.WithOrigins("https://cassini-org.info", "http://localhost:3000", "http://localhost:5201", "http://localhost:5173/", "https://cassini-app.fly.dev")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        #region DbContext
        //Adatkontextus - összeköttetés adatbázissal, dependency injection
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
                connstring = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
            }
            opt.UseNpgsql(connstring);
        });
        #endregion

        #region FileStorage
        var storageSettings = new StorageSettings();
        builder.Configuration.GetSection("FileStorage").Bind(storageSettings);
        if (string.IsNullOrEmpty(storageSettings.StorageMount) ||
            string.IsNullOrEmpty(storageSettings.TeacherImageFolder) ||
            string.IsNullOrEmpty(storageSettings.PresenterImageFolder))
        {
            throw new Exception("StorageSettings incomplete or unavailable");
        }
        builder.Services.AddSingleton<IFileService>(new FileService(storageSettings));
        #endregion

        #region JWT Configuration
        var jwtSettings = new JwtSettings();
        var env = builder.Environment.EnvironmentName; // Get environment name

        if (env == "Development")
        {
            var jwtSection = builder.Configuration.GetSection("JwtSettings");
            if (jwtSection.Exists())
            {
                jwtSection.Bind(jwtSettings);
            }
            else
            {
                Console.WriteLine("Warning: JwtSettings section is missing from appsettings.json.");
            }
        }
        else
        {
            var envKey = Environment.GetEnvironmentVariable("JwtSettings__SecretKey");
            var envIss = Environment.GetEnvironmentVariable("JwtSettings__Issuer");
            var envAud = Environment.GetEnvironmentVariable("JwtSettings__Audience");

            if (envKey == null || envIss == null || envAud == null)
            {
                throw new InvalidOperationException("Critical error: JWT settings not found in env file!.");
            }

            jwtSettings.SecretKey = envKey;
            jwtSettings.Issuer = envIss;
            jwtSettings.Audience = envAud;
        }

        if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey) ||
            string.IsNullOrWhiteSpace(jwtSettings.Issuer) ||
            string.IsNullOrWhiteSpace(jwtSettings.Audience))
        {
            throw new InvalidOperationException("Critical error: JWT settings are missing or incomplete.");
        }

        builder.Services.AddSingleton(jwtSettings);
        #endregion


        #region Authentication
        //Auth szolgáltatások, Identity csomaggal kapcsolatos szolgáltatás, biztonságos regisztrációhoz, felhasználók bejelentkeztetéséhez
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                RoleClaimType = ClaimTypes.Role
            };

        }).AddCookie(IdentityConstants.ApplicationScheme); ;


        builder.Services.AddIdentityCore<User>(opt =>
        {
            opt.User.RequireUniqueEmail = true;
        })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<DataContext>()
        .AddSignInManager<SignInManager<User>>()
        .AddUserManager<UserManager<User>>()
        .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(opt =>
        {
            opt.LoginPath = "/api/User/login";
            opt.AccessDeniedPath = "/api/User/AccessDenied";
            opt.Cookie.HttpOnly = true;
            opt.Cookie.Name = "Identity.Application";
            opt.SlidingExpiration = true;
        });

        builder.Services.AddAuthorization(opt =>
        {
            opt.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
        });
        #endregion

        builder.Services.AddAutoMapper(
            typeof(EventMappingProfile),
            typeof(PresenterMappingProfile),
            typeof(PresenterDetailMappingProfile),
            typeof(RoleMappingProfile),
            typeof(SubjectMappingProfile),
            typeof(SubjectNameMappingProfile),
            typeof(TeacherMappingProfile),
            typeof(TeacherDescriptionMappingProfile),
            typeof(UserMappingProfile)
        );

        builder.Services.AddMediatR(
        #region EventManagement
            typeof(AddEvent.AddEventCommandHandler).Assembly,
            typeof(DeleteEvent.DeleteEventCommandHandler).Assembly,
            typeof(ListEvents.ListEventsHandler).Assembly,
        #endregion
        #region LanguageManagement
            typeof(AddLanguage.AddLanguageCommandHandler).Assembly,
            typeof(DeleteLanguage.DeleteLanguageCommandHandler).Assembly,
            typeof(ListLanguages.ListLanguagesRequestHandler).Assembly,
        #endregion
        #region PresenterManagement
            typeof(AddPresenter.AddPresenterCommandHandler).Assembly,
        #endregion
        #region TeacherManagement.Teacher
            typeof(AddSubjectToTeacher.AddSubjectToTeacherCommandHandler).Assembly,
            typeof(AddTeacher.AddTeacherCommandHandler).Assembly,
            typeof(DeleteTeacher.DeleteTeacherCommandHandler).Assembly,
            typeof(GetTeacherId.GetTeacherIdRequestHandler).Assembly,
            typeof(ListTeachers.ListTeacherCommandHandler).Assembly,
            typeof(FilterTeachers.FilterTeachersRequestHandler).Assembly,
            typeof(UpdateTeacher.UpdateTeacherCommandHandler).Assembly,
            typeof(UpdateTeacherDescription.UpdateTeacherDescriptionCommandHandler).Assembly,
        #endregion
        #region TeacherManagement.Subject
            typeof(AddSubject.AddSubjectCommandHandler).Assembly,
            typeof(DeleteSubject.DeleteSubjectCommandHandler).Assembly,
            typeof(GetSubject.GetSubjectRequestHandler).Assembly,
            typeof(ListSubjects.ListSubjectsRequestHandler).Assembly,
            typeof(ListSubjectCodes.ListSubjectCodesRequestHandler).Assembly,
        #endregion
        #region UserManagement
            typeof(LoginUser.LoginUserCommandHandler).Assembly,
            typeof(RegisterUser.RegisterUserCommandHandler).Assembly,
            typeof(GetUserId.GetUserIdRequestHandler).Assembly,
        #endregion
        #region RoleManagement
            typeof(AddRole.AddRoleCommandHandler).Assembly,
            typeof(DeleteRole.DeleteRoleCommandHandler).Assembly,
            typeof(GetRoleId.GetRoleIdRequestHandler).Assembly,
            typeof(ListRoles.ListRolesRequestHandler).Assembly
        #endregion
        );

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
        //app.UseHttpsRedirection();
        app.UseAuthentication();
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
            logger.LogError(ex, "Error during migration");
        }

        app.Run();
    }
}






