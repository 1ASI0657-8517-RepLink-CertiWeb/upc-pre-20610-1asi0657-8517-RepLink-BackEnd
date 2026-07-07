using Bogus;
using CertiWeb.API.IAM.Domain.Model.Aggregates;
using CertiWeb.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using CertiWeb.API.Users.Application.Internal.OutboundServices;
using CertiWeb.API.Users.Domain.Model.Aggregates;
using CertiWeb.API.Users.Domain.Model.Commands;
using CertiWeb.API.Vehicles.Domain.Model.Aggregates;
using CertiWeb.API.Vehicles.Domain.Model.Commands;
using Microsoft.EntityFrameworkCore;
using ReservationEntity = CertiWeb.API.Reservation.Domain.Model.Aggregates.Reservation;

namespace CertiWeb.API.Shared.Infrastructure;

/// <summary>
/// Seeds randomized development-only data (users, cars, reservations) using Bogus.
/// Caller must guard invocation with an environment check (Development only) — this
/// class does not check the environment itself, mirroring how BrandSeeder is just
/// plain data with no environment awareness of its own.
/// </summary>
public static class DevDataSeeder
{
    private static readonly string[] Plans = { "basic", "premium", "pro" };

    public static async Task SeedAsync(AppDbContext context, IHashingService hashingService)
    {
        Console.WriteLine("[DevDataSeeder] Running (Development environment only)...");
        var users = await SeedUsersAsync(context, hashingService);
        await SeedCarsAsync(context);
        await SeedReservationsAsync(context, users);
        await SeedAdminUserAsync(context, hashingService);
        Console.WriteLine("[DevDataSeeder] Done.");
    }

    /// <summary>
    /// Ensures at least one admin account exists for local testing. Falls back to this
    /// when the admin_users table came from a database created before AdminUser's
    /// EF Core HasData seed existed (EnsureCreated only applies HasData on first creation).
    /// </summary>
    private static async Task SeedAdminUserAsync(AppDbContext context, IHashingService hashingService)
    {
        if (await context.AdminUsers.AnyAsync())
        {
            Console.WriteLine("[DevDataSeeder] AdminUsers table already has row(s), skipping.");
            return;
        }

        var admin = new AdminUser
        {
            Name = "Dev Admin",
            Email = "devadmin@repllink.local",
            Password = hashingService.HashPassword("DevAdmin123!")
        };

        await context.AdminUsers.AddAsync(admin);
        await context.SaveChangesAsync();
        Console.WriteLine($"[DevDataSeeder] Inserted fallback admin user ({admin.Email}).");
    }

    private static async Task<List<User>> SeedUsersAsync(AppDbContext context, IHashingService hashingService)
    {
        if (await context.Users.AnyAsync())
        {
            var existing = await context.Users.ToListAsync();
            Console.WriteLine($"[DevDataSeeder] Users table already has {existing.Count} row(s), skipping.");
            return existing;
        }

        var passwordHash = hashingService.HashPassword("Passw0rd!");
        var faker = new Faker<User>()
            .CustomInstantiator(f => new User(new CreateUserCommand(
                f.Name.FullName(),
                f.Internet.Email().ToLowerInvariant(),
                passwordHash,
                f.PickRandom(Plans)
            )));

        var users = faker.Generate(5);
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
        Console.WriteLine($"[DevDataSeeder] Inserted {users.Count} fake users.");
        return users;
    }

    private static async Task SeedCarsAsync(AppDbContext context)
    {
        if (await context.Cars.AnyAsync())
        {
            var existingCount = await context.Cars.CountAsync();
            Console.WriteLine($"[DevDataSeeder] Cars table already has {existingCount} row(s), skipping.");
            return;
        }

        var brandIds = await context.Brands.Select(b => b.Id).ToListAsync();
        if (brandIds.Count == 0)
        {
            Console.WriteLine("[DevDataSeeder] No brands found yet, skipping car seeding.");
            return;
        }

        var faker = new Faker();
        var usedPlates = new HashSet<string>();
        var cars = new List<Car>();

        for (var i = 0; i < 8; i++)
        {
            string plate;
            do
            {
                plate = $"{faker.Random.String2(3, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")}-{faker.Random.Number(100, 999)}";
            } while (!usedPlates.Add(plate));

            var pdfPlaceholder = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes("%PDF-1.4 dev-seed-placeholder"));

            var command = new CreateCarCommand(
                Title: $"Certificación {faker.Vehicle.Model()}",
                Owner: faker.Name.FullName(),
                OwnerEmail: faker.Internet.Email().ToLowerInvariant(),
                Year: faker.Random.Int(2008, DateTime.Now.Year),
                BrandId: faker.PickRandom(brandIds),
                Model: faker.Vehicle.Model(),
                Description: faker.Lorem.Sentence(8),
                PdfCertification: pdfPlaceholder,
                ImageUrl: $"https://picsum.photos/seed/{faker.Random.AlphaNumeric(8)}/640/480",
                Price: Math.Round(faker.Random.Decimal(150, 450), 2),
                LicensePlate: plate,
                OriginalReservationId: 900001 + i
            );

            cars.Add(new Car(command));
        }

        await context.Cars.AddRangeAsync(cars);
        await context.SaveChangesAsync();
        Console.WriteLine($"[DevDataSeeder] Inserted {cars.Count} fake certified cars.");
    }

    private static async Task SeedReservationsAsync(AppDbContext context, List<User> users)
    {
        var reservations = context.Set<ReservationEntity>();
        if (await reservations.AnyAsync())
        {
            var existingCount = await reservations.CountAsync();
            Console.WriteLine($"[DevDataSeeder] Reservations table already has {existingCount} row(s), skipping.");
            return;
        }

        if (users.Count == 0)
            users = await context.Users.ToListAsync();
        if (users.Count == 0)
        {
            Console.WriteLine("[DevDataSeeder] No users available, skipping reservation seeding.");
            return;
        }

        var brandNames = await context.Brands.Select(b => b.Name).ToListAsync();
        var faker = new Faker();
        var pendingReservations = new List<ReservationEntity>();

        for (var i = 0; i < 3; i++)
        {
            var user = faker.PickRandom(users);
            var plate = $"{faker.Random.String2(3, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")}-{faker.Random.Number(100, 999)}";
            var inspectionHour = faker.PickRandom(9, 11, 13, 15, 17);

            pendingReservations.Add(new ReservationEntity
            {
                UserId = user.Id,
                ReservationName = user.name,
                ReservationEmail = user.email,
                ImageUrl = $"https://picsum.photos/seed/{faker.Random.AlphaNumeric(8)}/640/480",
                Brand = brandNames.Count > 0 ? faker.PickRandom(brandNames) : "Toyota",
                Model = faker.Vehicle.Model(),
                LicensePlate = plate,
                InspectionDateTime = faker.Date.Soon(10).Date.AddHours(inspectionHour),
                Price = Math.Round(faker.Random.Decimal(80, 200), 2).ToString("0.00"),
                Status = "pending"
            });
        }

        await reservations.AddRangeAsync(pendingReservations);
        await context.SaveChangesAsync();
        Console.WriteLine($"[DevDataSeeder] Inserted {pendingReservations.Count} fake pending reservations.");
    }
}
