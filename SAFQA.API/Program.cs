using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SAFQA.API.Middleware;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.BLL.Managers.AccountManager.Email_Sender;
using SAFQA.BLL.Managers.AccountManager.OAuth;
using SAFQA.BLL.Managers.SellerAppManager;
using SAFQA.BLL.Managers.SellerAppManager.AuctionManager;
using SAFQA.BLL.Managers.SellerAppManager.AuctionService;
using SAFQA.BLL.Managers.SellerAppManager.BidService;
using SAFQA.BLL.Managers.SellerAppManager.ItemService;
using SAFQA.BLL.Managers.SellerAppManager.Notification;
using SAFQA.BLL.Managers.SellerAppManager.SellerManager;
using SAFQA.BLL.Managers.SellerAppManager.TransactionService;
using SAFQA.BLL.Managers.SellerAppManager.WalletServeice;
using SAFQA.BLL.Managers.SellerAppManager.WalletService;
using SAFQA.BLL.Managers.UserAppManager;
using SAFQA.DAL.Database;
using SAFQA.DAL.Models;
using SAFQA.DAL.Repository.Auction;
using SAFQA.DAL.Repository.Bids;
using SAFQA.DAL.Repository.Category;
using SAFQA.DAL.Repository.Items;
using SAFQA.DAL.Repository.Location;
using SAFQA.DAL.Repository.Notification;
using SAFQA.DAL.Repository.Seller;
using SAFQA.DAL.Repository.Transaction;
using SAFQA.DAL.Repository.Users;
using SAFQA.DAL.Repository.Wallet;
using System.Security.Claims;
using System.Text;



namespace SAFQA.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);




            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "Enter API Key",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Name = "x-api-key",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddDbContext<SAFQA_Context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("cs")));

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<SAFQA_Context>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IOAuth, Oauth>();
            builder.Services.AddScoped<IAuthUser, AuthUser>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IcategoryRepo, categoryRepo>();
            builder.Services.AddScoped<IAuctionManager, AuctionManager>();
            builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
            builder.Services.AddScoped<IAuctionManager, AuctionManager>();
            builder.Services.AddScoped<IitemsRepository, ItemRepository>();
            builder.Services.AddScoped<IItemManager, ItemManager>(); 
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
            builder.Services.AddScoped<ITransactionManager, TransactionManager>();
            builder.Services.AddScoped<IBidRepository, BidRepository>();
            builder.Services.AddScoped<IBidManager, BidManager>();
            builder.Services.AddScoped<IsellerManager, sellerManager>();
            builder.Services.AddScoped<IsellerRepo, sellerRepo>();
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<ICardRepo, CardRepo>();
            builder.Services.AddScoped<ICardService, CardService>();
            builder.Services.AddScoped<IWalletRepo, WalletRepo>();
            builder.Services.AddScoped<IWalletService, WalletService>();
            builder.Services.AddScoped<ILocationRepo, LocationRepo>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<INotificationManager, NotificationManager>();
            builder.Services.AddScoped<IAuctionService, AuctionService>();




            var jwtSettings = builder.Configuration.GetSection("JWT");
            builder.Services.Configure<ApiKeyOptions>(builder.Configuration.GetSection("ApiKey"));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtSettings["ValidIssuer"],
                    ValidAudience = jwtSettings["ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userManager = context.HttpContext.RequestServices
                            .GetRequiredService<UserManager<User>>();

                        var userId = context.Principal
                            .FindFirstValue(ClaimTypes.NameIdentifier);

                        if (string.IsNullOrEmpty(userId))
                        {
                            context.Fail("Unauthorized");
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userId);

                        if (user == null)
                        {
                            context.Fail("Unauthorized");
                            return;
                        }

                        var securityStamp = context.Principal
                            .FindFirstValue("SecurityStamp");

                        if (string.IsNullOrEmpty(securityStamp) ||
                            user.SecurityStamp != securityStamp)
                        {
                            context.Fail("Token expired due to security stamp change");
                        }
                    }
                };
            });

            builder.Services.AddDbContext<SAFQA_Context>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,      
                            maxRetryDelay: TimeSpan.FromSeconds(10), 
                            errorNumbersToAdd: null  
                        );
                    }
                )
            );

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            builder.Services.AddHostedService<ExpiredOtpCleanupService>();

            var app = builder.Build(); 

          


            // Configure the HTTP request pipeline.

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("AllowAll");

            app.UseMiddleware<ApiKeyMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            async Task SeedRolesAsync(WebApplication app)
            {
                using var scope = app.Services.CreateScope();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = { "ADMIN", "USER", "SELLER" };

                foreach (var roleName in roles)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
            }

            await SeedRolesAsync(app);

            await app.RunAsync();
        }
    }
}
