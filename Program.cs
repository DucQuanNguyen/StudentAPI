using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StudentAPI.Model;
using StudentAPI.Service;
using StudentAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();
// Đăng ký các mapper và parameter adder
builder.Services.AddScoped<IDataMapper<SinhVien>, SinhVienMapper>();
builder.Services.AddScoped<IParameterAdder<SinhVien>, SinhVienParameterAdder>();
builder.Services.AddScoped<IDataMapper<LopHoc>, LopHocMapper>();
builder.Services.AddScoped<IParameterAdder<LopHoc>, LopHocParameterAdder>();

// Đăng ký BaseService 
builder.Services.AddScoped<BaseService, BaseService>();

// Đăng ký GenericRepository cho SinhVien
builder.Services.AddScoped<IRepository<SinhVien, string>>(provider =>
    new GenericRepository<SinhVien, string>(
        provider.GetRequiredService<BaseService>(),
        "GETSinhVien",
        "GETSinhVienById",
        "AddSinhVien",
        "UpdateSinhVien",
        "DeleteSinhVien",
        "@StudentID",
        provider.GetRequiredService<IDataMapper<SinhVien>>(),
        provider.GetRequiredService<IParameterAdder<SinhVien>>(),
        "GETSinhVienPaged",
        "@StudentID"        
    ));

// Đăng ký GenericRepository cho LopHoc
builder.Services.AddScoped<IRepository<LopHoc, int>>(provider =>
    new GenericRepository<LopHoc, int>(
        provider.GetRequiredService<BaseService>(),
        "GETLopHoc",
        "GETLopHocbyId",
        "AddLopHoc",
        "UpdateLopHoc",
        "DeleteLopHoc",
        "@Id",
        provider.GetRequiredService<IDataMapper<LopHoc>>(),
        provider.GetRequiredService<IParameterAdder<LopHoc>>(),
        "GETLopHocPaged", 
        "@Id"             
    ));

// Đăng ký các service sử dụng repository
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Đăng ký các service khác 
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();