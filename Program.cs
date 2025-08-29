using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Enuns;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.Services;
using minimal_api.Domain.ViewModel;
using minimal_api.Infra.Db;
using System.Text;

#region App

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option => 
{
    var key = builder.Configuration.GetSection("Jwt").ToString();

    option.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT aqui"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {   
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, 
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.UseHttpsRedirection();

#endregion

#region Login

app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) => 
{
    var admin = adminService.Login(loginDTO);

    if (admin != null)
    {
        var key = builder.Configuration.GetSection("Jwt").ToString();
        string token = adminService.GerarTokenJwt(admin, key);

        var adminLogado = new AdminLogado()
        {
            Email = admin.Email,
            Role = admin.Role,
            Token = token
        };

        return Results.Ok(adminLogado);
    }
    return Results.Unauthorized();
}).WithTags("Admin");

#endregion

#region Admin

app.MapGet("/admins", ([FromQuery] int? page, IAdminService adminService) =>
{
    return Results.Ok(adminService.GetAll(page ?? 1));
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" }).WithTags("Admin");

app.MapGet("admin/{id}", ([FromRoute] int id, IAdminService adminService) =>
{
    var admin = adminService.FindById(id);

    if (admin == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(admin);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" }).WithTags("Admin");

app.MapPost("/admin", ([FromBody] AdminDTO adminDTO, IAdminService adminService) =>
{
    var validacao = new ErrosDeValidacao();

    if (string.IsNullOrEmpty(adminDTO.Email))
    {
        validacao.Menssagens.Add("Email nao pode ser vazio!");
    }

    if(string.IsNullOrEmpty(adminDTO.Senha))
    {
        validacao.Menssagens.Add("Senha nao pode ser vazia!");
    }

    if(adminDTO.Role == null)
    {
        validacao.Menssagens.Add("Role nao pode ser vazio!");
    }

    if (validacao.Menssagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var admin = new Admin()
    {
        Email = adminDTO.Email,
        Senha = adminDTO.Senha,
        Role = adminDTO.Role.ToString()
    };
    adminService.Create(admin);

    return Results.Created($"/admin/{admin.Id}", admin);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" }).WithTags("Admin");

#endregion

#region Veiculos

app.MapGet("/veiculos", ([FromQuery] int? page, [FromQuery] string? nome, [FromQuery] string? marca, IVeiculoService veiculoService) =>
{
    var veiculos = veiculoService.ListAll(page ?? 1, nome, marca);
    return Results.Ok(veiculos);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Editor" }).WithTags("Veiculo");

app.MapPost("/veiculo", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoService veiculoService) =>
{
    var validacao = VeiculoService.ValidaVeiculoDto(veiculoDTO);

    if (validacao.Menssagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var veiculo = new minimal_api.Domain.Entities.Veiculo
    {
        Nome = veiculoDTO.Nome,
        Ano = veiculoDTO.Ano,
        Marca = veiculoDTO.Marca
    };

    veiculoService.Create(veiculo);
    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Editor" }).WithTags("Veiculo");

app.MapGet("veiculo/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.FindById(id);

    if (veiculo == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(veiculo);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Editor" }).WithTags("Veiculo");

app.MapPut("/veiculo/{id}", ( [FromRoute] int id, VeiculoDTO veiculoDTO ,IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.FindById(id);
    if (veiculo == null)
    {
        return Results.NotFound();
    }

    var validacao = VeiculoService.ValidaVeiculoDto(veiculoDTO);
    if (validacao.Menssagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    veiculo.Ano = veiculoDTO.Ano;
    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;

    veiculoService.Update(veiculo);

    return Results.Ok(veiculo);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" }).WithTags("Veiculo");

app.MapDelete("/veiculo/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.FindById(id);
    if (veiculo == null)
    {
        return Results.NotFound();
    }

    veiculoService.Delete(veiculo);
    return Results.NoContent();
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" }).WithTags("Veiculo");

#endregion

app.Run();