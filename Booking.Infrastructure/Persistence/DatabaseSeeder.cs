using Booking.Domain.Bookings;
using Booking.Domain.Hotels;
using Booking.Domain.Reviews;
using Booking.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(BookingDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);

        if (await dbContext.Hotels.AnyAsync(cancellationToken))
        {
            return;
        }

        var hotels = CreateHotels();
        var users = CreateUsers();
        var reviews = CreateReviews();
        var bookings = CreateBookings();

        dbContext.Hotels.AddRange(hotels);
        dbContext.Users.AddRange(users);
        dbContext.Reviews.AddRange(reviews);
        dbContext.Bookings.AddRange(bookings);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static List<Hotel> CreateHotels()
    {
        return
        [
            new Hotel
            {
                Id = "hot_kyiv_river",
                Name = "Dnipro River Boutique",
                City = "Kyiv",
                Country = "Ukraine",
                PricePerNight = 130,
                Rating = 4.8,
                ReviewCount = 612,
                DistanceToCenterKm = 0.2m,
                Tags = ["Top rated", "City center", "Breakfast included"],
                Amenities = ["Wi-Fi", "Airport transfer", "Gym", "Parking", "Pet friendly"],
                Description = "A central riverside hotel with modern rooms, skyline views, and fast access to business and historic districts.",
                Images =
                [
                    "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1522798514-97ceb8c4f1c8?auto=format&fit=crop&w=1400&q=80"
                ],
                Facilities = DefaultFacilities(),
                Rooms =
                [
                    new Room
                    {
                        Id = "room_river_suite",
                        HotelId = "hot_kyiv_river",
                        Name = "River Suite",
                        Beds = "Queen bed 1",
                        Price = 184,
                        FreeCancellation = true
                    },
                    new Room
                    {
                        Id = "room_river_standard",
                        HotelId = "hot_kyiv_river",
                        Name = "Standard City",
                        Beds = "Double bed 1",
                        Price = 130,
                        FreeCancellation = true
                    }
                ]
            },

            new Hotel
            {
                Id = "hot_lviv_oldtown",
                Name = "Old Town Residence",
                City = "Lviv",
                Country = "Ukraine",
                PricePerNight = 98,
                Rating = 4.6,
                ReviewCount = 438,
                DistanceToCenterKm = 0.4m,
                Tags = ["Historic area", "Family favorite"],
                Amenities = ["Wi-Fi", "Breakfast", "Parking", "Laundry"],
                Description = "Warm interiors, walkable old city routes, and flexible family rooms designed for short city breaks.",
                Images =
                [
                    "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1566665797739-1674de7a421a?auto=format&fit=crop&w=1400&q=80"
                ],
                Facilities = DefaultFacilities(),
                Rooms =
                [
                    new Room
                    {
                        Id = "room_oldtown_classic",
                        HotelId = "hot_lviv_oldtown",
                        Name = "Classic Room",
                        Beds = "Double bed 1",
                        Price = 98,
                        FreeCancellation = true
                    },
                    new Room
                    {
                        Id = "room_oldtown_family",
                        HotelId = "hot_lviv_oldtown",
                        Name = "Family Studio",
                        Beds = "Queen bed 1 | Sofa bed 1",
                        Price = 142,
                        FreeCancellation = false
                    }
                ]
            },

            new Hotel
            {
                Id = "hot_warsaw_hub",
                Name = "Warsaw Skyline Hub",
                City = "Warsaw",
                Country = "Poland",
                PricePerNight = 142,
                Rating = 4.5,
                ReviewCount = 721,
                DistanceToCenterKm = 1.1m,
                Tags = ["Business", "Late check-in"],
                Amenities = ["Wi-Fi", "Coworking", "Gym", "Spa"],
                Description = "Business-ready hotel with quiet work lounges, conference corners, and easy airport train access.",
                Images =
                [
                    "https://images.unsplash.com/photo-1455587734955-081b22074882?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1501117716987-c8e1ecb21079?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1445019980597-93fa8acb246c?auto=format&fit=crop&w=1400&q=80"
                ],
                Facilities = DefaultFacilities(),
                Rooms =
                [
                    new Room
                    {
                        Id = "room_warsaw_exec",
                        HotelId = "hot_warsaw_hub",
                        Name = "Executive Room",
                        Beds = "King bed 1",
                        Price = 176,
                        FreeCancellation = true
                    },
                    new Room
                    {
                        Id = "room_warsaw_standard",
                        HotelId = "hot_warsaw_hub",
                        Name = "Standard Room",
                        Beds = "Double bed 1",
                        Price = 142,
                        FreeCancellation = true
                    }
                ]
            },

            new Hotel
            {
                Id = "hot_berlin_north",
                Name = "North Berlin Loft",
                City = "Berlin",
                Country = "Germany",
                PricePerNight = 167,
                Rating = 4.7,
                ReviewCount = 529,
                DistanceToCenterKm = 0.8m,
                Tags = ["Modern", "Design hotel"],
                Amenities = ["Wi-Fi", "Rooftop", "Breakfast", "Gym"],
                Description = "Contemporary loft-style rooms with rooftop city views and rapid metro links across Berlin.",
                Images =
                [
                    "https://images.unsplash.com/photo-1512918728675-ed5a9ecdebfd?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1590490360182-c33d57733427?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1468824357306-a439d58ccb1c?auto=format&fit=crop&w=1400&q=80"
                ],
                Facilities = DefaultFacilities(),
                Rooms =
                [
                    new Room
                    {
                        Id = "room_berlin_loft",
                        HotelId = "hot_berlin_north",
                        Name = "Loft Deluxe",
                        Beds = "King bed 1",
                        Price = 201,
                        FreeCancellation = false
                    },
                    new Room
                    {
                        Id = "room_berlin_comfort",
                        HotelId = "hot_berlin_north",
                        Name = "Comfort Room",
                        Beds = "Double bed 1",
                        Price = 167,
                        FreeCancellation = true
                    }
                ]
            },

            new Hotel
            {
                Id = "hot_rome_aurora",
                Name = "Aurora Roma Suites",
                City = "Rome",
                Country = "Italy",
                PricePerNight = 155,
                Rating = 4.4,
                ReviewCount = 304,
                DistanceToCenterKm = 1.5m,
                Tags = ["Romantic", "Near landmarks"],
                Amenities = ["Wi-Fi", "Breakfast", "Concierge", "Transfer"],
                Description = "Elegant suites near cultural landmarks with concierge service and curated local experiences.",
                Images =
                [
                    "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1496417263034-38ec4f0b665a?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1611892440504-42a792e24d32?auto=format&fit=crop&w=1400&q=80"
                ],
                Facilities = DefaultFacilities(),
                Rooms =
                [
                    new Room
                    {
                        Id = "room_rome_suite",
                        HotelId = "hot_rome_aurora",
                        Name = "City Suite",
                        Beds = "Queen bed 1",
                        Price = 182,
                        FreeCancellation = true
                    },
                    new Room
                    {
                        Id = "room_rome_compact",
                        HotelId = "hot_rome_aurora",
                        Name = "Compact Room",
                        Beds = "Double bed 1",
                        Price = 155,
                        FreeCancellation = true
                    }
                ]
            },

            new Hotel
            {
                Id = "hot_paris_lumiere",
                Name = "Lumiere Paris Central",
                City = "Paris",
                Country = "France",
                Address = "Rue de la Paix, Paris, France",
                PricePerNight = 214,
                Rating = 4.9,
                ReviewCount = 955,
                DistanceToCenterKm = 0.3m,
                Tags = ["Luxury", "Top rated"],
                Amenities = ["Wi-Fi", "Spa", "Restaurant", "Valet parking"],
                Description = "Five-star city retreat with curated dining and premium service in the center of Paris.",
                Images =
                [
                    "https://images.unsplash.com/photo-1564501049412-61c2a3083791?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1455587734955-081b22074882?auto=format&fit=crop&w=1400&q=80"
                ],
                ScoreItems =
                [
                    new ScoreItem { Label = "Facilities", Value = 9.8 },
                    new ScoreItem { Label = "Staff", Value = 7.9 },
                    new ScoreItem { Label = "Cleanliness", Value = 9.2 },
                    new ScoreItem { Label = "Comfort", Value = 9.5 },
                    new ScoreItem { Label = "Location", Value = 9.2 },
                    new ScoreItem { Label = "Value for money", Value = 9.8 }
                ],
                Facilities = DefaultFacilities(),
                Rooms =
                [
                    new Room
                    {
                        Id = "room_paris_signature",
                        HotelId = "hot_paris_lumiere",
                        Image = DefaultRoomImage(),
                        Name = "Signature Suite",
                        Beds = "King bed 1",
                        Price = 296,
                        FreeCancellation = false
                    },
                    new Room
                    {
                        Id = "room_paris_deluxe",
                        HotelId = "hot_paris_lumiere",
                        Image = DefaultRoomImage(),
                        Name = "Deluxe Room",
                        Beds = "Queen bed 1",
                        Price = 214,
                        FreeCancellation = true
                    }
                ]
            },

            new Hotel
            {
                Id = "hot_london_tower",
                Name = "Tower Bridge Stay",
                City = "London",
                Country = "United Kingdom",
                PricePerNight = 198,
                Rating = 4.3,
                ReviewCount = 441,
                DistanceToCenterKm = 1m,
                Tags = ["Weekend", "Shopping district"],
                Amenities = ["Wi-Fi", "Breakfast", "Gym", "Room service"],
                Description = "Comfort-first city stay with easy access to iconic landmarks and key transport stations.",
                Images =
                [
                    "https://images.unsplash.com/photo-1551632436-cbf8dd35adfa?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1551776235-dde6d4829808?auto=format&fit=crop&w=1400&q=80"
                ],
                Facilities = DefaultFacilities(),
                Rooms =
                [
                    new Room
                    {
                        Id = "room_london_city",
                        HotelId = "hot_london_tower",
                        Name = "City View",
                        Beds = "Double bed 1",
                        Price = 198,
                        FreeCancellation = true
                    },
                    new Room
                    {
                        Id = "room_london_family",
                        HotelId = "hot_london_tower",
                        Name = "Family Room",
                        Beds = "Queen bed 1 | Sofa bed 1",
                        Price = 242,
                        FreeCancellation = true
                    }
                ]
            },

            new Hotel
            {
                Id = "hot_barcelona_breeze",
                Name = "Barcelona Breeze",
                City = "Barcelona",
                Country = "Spain",
                PricePerNight = 172,
                Rating = 4.4,
                ReviewCount = 388,
                DistanceToCenterKm = 1.4m,
                Tags = ["Sea side", "Flexible check-in"],
                Amenities = ["Wi-Fi", "Pool", "Breakfast", "Bike rental"],
                Description = "Sunlit rooms close to the coast, ideal for mixed work-leisure stays and short city trips.",
                Images =
                [
                    "https://images.unsplash.com/photo-1505691938895-1758d7feb511?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=1400&q=80",
                    "https://images.unsplash.com/photo-1552901464-6f8adf6f2b8c?auto=format&fit=crop&w=1400&q=80"
                ],
                Facilities = DefaultFacilities(),
                Rooms =
                [
                    new Room
                    {
                        Id = "room_barcelona_ocean",
                        HotelId = "hot_barcelona_breeze",
                        Name = "Ocean Room",
                        Beds = "Queen bed 1",
                        Price = 172,
                        FreeCancellation = true
                    },
                    new Room
                    {
                        Id = "room_barcelona_sun",
                        HotelId = "hot_barcelona_breeze",
                        Name = "Sunset Suite",
                        Beds = "King bed 1",
                        Price = 224,
                        FreeCancellation = false
                    }
                ]
            }
        ];
    }

    private static List<FacilityGroup> DefaultFacilities()
    {
        return
        [
            new FacilityGroup
            {
                Title = "General",
                Icon = "general",
                Items =
                [
                    "Shuttle service",
                    "Additional charge",
                    "Grocery deliveries"
                ]
            },
            new FacilityGroup
            {
                Title = "Parking",
                Icon = "parking",
                Items =
                [
                    "Parking garage"
                ]
            }
        ];
    }

    private static string DefaultRoomImage()
    {
        return "https://plus.unsplash.com/premium_photo-1676823553207-758c7a66e9bb?fm=jpg&q=60&w=3000&auto=format&fit=crop";
    }

    private static List<AppUser> CreateUsers()
    {
        var demoUser = new AppUser
        {
            Id = "usr_demo",
            FullName = "Your Name",
            Email = "demo@hotel4you.app",
            Verified = true,
            Phone = "+380991112233",
            Country = "Ukraine",
            PreferredCurrency = "USD",
            Birthday = new DateOnly(1996, 8, 20),
            Favorites = ["hot_kyiv_river", "hot_paris_lumiere"],
            Role = "User",
            IsBlocked = false
        };

        var adminUser = new AppUser
        {
            Id = "usr_admin",
            FullName = "Admin User",
            Email = "admin@hotel4you.app",
            Verified = true,
            Phone = "+380990000000",
            Country = "Ukraine",
            PreferredCurrency = "USD",
            Birthday = new DateOnly(1995, 1, 1),
            Favorites = [],
            Role = "Admin",
            IsBlocked = false
        };

        var hasher = new PasswordHasher<AppUser>();

        demoUser.PasswordHash = hasher.HashPassword(demoUser, "DemoPass123!");
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "AdminPass123!");

        return [demoUser, adminUser];
    }

    private static List<Review> CreateReviews()
    {
        return
        [
            new Review
            {
                Id = "rev_1001",
                Author = "Olivia",
                HotelId = "hot_kyiv_river",
                Rating = 5,
                Text = "Perfect location and very clean rooms. Booking process was quick and smooth.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-12)
            },
            new Review
            {
                Id = "rev_1002",
                Author = "Artem",
                HotelId = "hot_kyiv_river",
                Rating = 4,
                Text = "Helpful staff and good breakfast. I would stay again for business trips.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-19)
            },
            new Review
            {
                Id = "rev_1101",
                Author = "Nora",
                HotelId = "hot_lviv_oldtown",
                Rating = 4,
                Text = "Warm atmosphere, close to everything in the old town.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-8)
            },
            new Review
            {
                Id = "rev_1201",
                Author = "Ethan",
                HotelId = "hot_paris_lumiere",
                Rating = 5,
                Text = "Outstanding service and one of the best city views I have had in Paris.",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-3)
            }
        ];
    }

    private static List<HotelBooking> CreateBookings()
    {
        return
        [
            new HotelBooking
            {
                Id = "bok_demo_1",
                UserId = "usr_demo",
                HotelId = "hot_kyiv_river",
                RoomId = "room_river_standard",
                CheckIn = new DateOnly(2026, 3, 20),
                CheckOut = new DateOnly(2026, 3, 24),
                Guests = 2,
                Status = "confirmed",
                TotalPrice = 520,
                Currency = "USD"
            },
            new HotelBooking
            {
                Id = "bok_demo_2",
                UserId = "usr_demo",
                HotelId = "hot_lviv_oldtown",
                RoomId = "room_oldtown_family",
                CheckIn = new DateOnly(2025, 12, 11),
                CheckOut = new DateOnly(2025, 12, 13),
                Guests = 3,
                Status = "completed",
                TotalPrice = 284,
                Currency = "USD"
            }
        ];
    }
}