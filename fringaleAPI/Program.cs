using Microsoft.EntityFrameworkCore;
using fringaleAPI;
using Microsoft.AspNetCore.Components.Forms;
using System.Formats.Tar;
using System;
using System.Text.Json;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FringaleAPIDb>(opt => opt.UseSqlite("Data Source=FringaleAPI.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("V1", new OpenApiInfo
    {
        Title = "Todo API",
        Version = "V1",
        Description = "Une API pour g�rer une liste de t�ches",
        Contact = new OpenApiContact
        {
            Name = "Julie",
            Email = "dbt.julie@gmail.com",
            Url = new Uri("https://votre-site.com"),
        }
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/V1/swagger.json", "Todo API V1");
        c.RoutePrefix = "";
    });
}

// Initialiser la base de donn�es avec des donn�es de test
using (var scope = app.Services.CreateScope())
{
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<FringaleAPIDb>();
        DbInitializer.Seed(dbContext);
    }
}




// ROUTES

    // R�cup�rer tous les clients
    app.MapGet("/clients", async (FringaleAPIDb db) =>
        await db.Clients.ToListAsync());

    // R�cup�rer un client par son ID
    app.MapGet("/Clients/{id}", async (int id, FringaleAPIDb db) =>
        await db.Clients.FindAsync(id)
            is Client client
                ? Results.Ok(client)
                : Results.NotFound());

    // Cr�er un client 
    app.MapPost("/clients", async (Client client, FringaleAPIDb db) =>
        {
            db.Clients.Add(client);
            await db.SaveChangesAsync();

            return Results.Created($"/clients/{client.Id_cl}", client);
        });

    // Modifier un client par son ID
    app.MapPut("/clients/{id}", async (int id, Client inputClient, FringaleAPIDb db) =>
    {
        var client = await db.Clients.FindAsync(id);

        if (client is null) return Results.NotFound();

        client.Nom_cl = inputClient.Nom_cl;
        client.Prenom_cl = inputClient.Prenom_cl;
        client.Adresse_cl = inputClient.Adresse_cl;
        client.Telephone_cl = inputClient.Telephone_cl;

        await db.SaveChangesAsync();

        return Results.NoContent();
    });

    // Supprimer un client par son ID
    app.MapDelete("/clients/{id}", async (int id, FringaleAPIDb db) =>
    {
        if (await db.Clients.FindAsync(id) is Client client)
        {
            db.Clients.Remove(client);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }
        return Results.NotFound();
    });

    // R�cup�rer toutes les commandes
    app.MapGet("/commandes", async (FringaleAPIDb db) =>
        await db.Commandes.ToListAsync());

    // R�cup�rer une commande par son ID
    app.MapGet("/commandes/{id}", async (int id, FringaleAPIDb db) =>
        await db.Commandes.FindAsync(id)
            is Commande commande
                ? Results.Ok(commande)
                : Results.NotFound());

    // Cr�er une commande 
    app.MapPost("/commandes", async (Commande commande, FringaleAPIDb db) =>
    {
        db.Commandes.Add(commande);
        await db.SaveChangesAsync();

        return Results.Created($"/clients/{commande.Id_co}", commande);
    });

    // Modifier une commande
    app.MapPut("/commandes/{id}", async (int id, Commande inputCommande, FringaleAPIDb db) =>
    {
        var commande = await db.Commandes.FindAsync(id);

        if (commande is null) return Results.NotFound();

        commande.Montant_co = inputCommande.Montant_co;

        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    );

    // Supprimer une commande par son ID
    app.MapDelete("/commandes/{id}", async (int id, FringaleAPIDb db) =>
    {
        if (await db.Commandes.FindAsync(id) is Commande commande)
        {
            db.Commandes.Remove(commande);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }
        return Results.NotFound();
    });

    // R�cup�rer toutes les commandes d'un client 
    app.MapGet("/Clients/{id}/Commandes", async (int id, DateTime date, FringaleAPIDb db) =>
    {
        var client = await db.Clients.FindAsync(id);
        if (client == null) return Results.NotFound();

        var commandes = await db.Commandes
                                .Where(o => o.Id_cl == id)
                                .ToListAsync(); // requ�te LINQ 

        return Results.Ok(commandes);
    });

    // R�cup�rer les plats 
    app.MapGet("/Plats", async (FringaleAPIDb db) =>
        await db.Plats.ToListAsync());

    // R�cup�rer les plats par ID 
    app.MapGet("/Plats/{id_pl}", async (int id_pl, FringaleAPIDb db) =>
        await db.Plats.FindAsync(id_pl)
            is Plat plat
                ? Results.Ok(plat)
                : Results.NotFound());

    // Cr�er un plat 
    app.MapPost("/Plats", async (Plat plat, FringaleAPIDb db) =>
    {
        db.Plats.Add(plat);
        await db.SaveChangesAsync();

        return Results.Created($"/Plats/{plat.Id_pl}", plat);
    });

    // Modifier un plat 
    app.MapPut("/Plats/{id_pl}", async (int id_pl, Plat inputPlat, FringaleAPIDb db) =>
    {
        var plat = await db.Plats.FindAsync(id_pl);

        if (plat is null) return Results.NotFound();

        plat.Nom_pl = inputPlat.Nom_pl;
        plat.Prix_pl = inputPlat.Prix_pl;
        plat.Categorie_pl = inputPlat.Categorie_pl;

        await db.SaveChangesAsync();

        return Results.NoContent();
    });

    // Supprimer un plat par ID 
    app.MapDelete("/Plats/{id}", async (int id_pl, FringaleAPIDb db) =>
    {
        if (await db.Plats.FindAsync(id_pl) is Plat plat)
        {
            db.Plats.Remove(plat);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }

        return Results.NotFound();
    });

    app.Run();