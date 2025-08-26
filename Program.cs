 using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.Services;
using minimal_api.Domain.ViewModel;
using minimal_api.Infra.Db;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) => 
{
    if (adminService.Login(loginDTO) != null)
    {
        return Results.Ok("Login com sucesso");
    }
    return Results.Unauthorized();
}).WithTags("Admin");

app.MapGet("/veiculos", ([FromQuery] int? page, [FromQuery] string? nome, [FromQuery] string? marca, IVeiculoService veiculoService) =>
{
    var veiculos = veiculoService.ListAll(page ?? 1, nome, marca);
    return Results.Ok(veiculos);
}).WithTags("Veiculo");

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
}).WithTags("Veiculo");

app.MapGet("veiculo/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.FindById(id);

    if (veiculo == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(veiculo);
}).WithTags("Veiculo");

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
}).WithTags("Veiculo");

app.MapDelete("/veiculo/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
{
    var veiculo = veiculoService.FindById(id);
    if (veiculo == null)
    {
        return Results.NotFound();
    }

    veiculoService.Delete(veiculo);
    return Results.NoContent();
}).WithTags("Veiculo");

app.Run();
 