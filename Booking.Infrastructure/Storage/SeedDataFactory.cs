using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Booking.Domain.Bookings;
using Booking.Domain.Hotels;
using Booking.Domain.Reviews;
using Booking.Domain.Users;

namespace Booking.Infrastructure.Storage;

public static class SeedDataFactory
{
    public static InMemoryStore Create()
    {
        var store = new InMemoryStore();

        store.Hotels.AddRange(CreateHotels());
        store.Reviews.AddRange(CreateReviews());
        store.Users.AddRange(CreateUsers());
        store.Bookings.AddRange(CreateBookings());

        store.AccessSessions["token_access_demo"] = new AccessSession
        {
            UserId = "usr_demo",
            ExpiresAtUtc = DateTime.UtcNow.AddHours(6),
            RefreshToken = "token_refresh_demo"
        };

        store.RefreshSessions["token_refresh_demo"] = new RefreshSession
        {
            UserId = "usr_demo",
            ExpiresAtUtc = DateTime.UtcNow.AddDays(5)
        };

        return store;
    }

    private static IEnumerable<Hotel> CreateHotels()
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
                Rooms =
                [
                    new Room
                    {
                        Id = "room_river_suite",
                        Name = "River Suite",
                        Beds = "Queen bed 1",
                        Price = 184,
                        FreeCancellation = true
                    },
                    new Room
                    {
                        Id = "room_river_standard",
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
                Rooms =
                [
                    new Room
                    {
                        Id = "room_oldtown_classic",
                        Name = "Classic Room",
                        Beds = "Double bed 1",
                        Price = 98,
                        FreeCancellation = true
                    },
                    new Room
                    {
                        Id = "room_oldtown_family",
                        Name = "Family Studio",
                        Beds = "Queen bed 1 | Sofa bed 1",
                        Price = 142,
                        FreeCancellation = false
                    }
                ]
            }
        ];
    }

    private static IEnumerable<Review> CreateReviews()
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
            }
        ];
    }

    private static IEnumerable<AppUser> CreateUsers()
    {
        return
        [
            new AppUser
            {
                Id = "usr_demo",
                FullName = "Your Name",
                Email = "demo@hotel4you.app",
                Password = "DemoPass123!",
                Verified = true,
                Phone = "+380991112233",
                Country = "Ukraine",
                PreferredCurrency = "USD",
                Birthday = new DateOnly(1996, 8, 20),
                Favorites = ["hot_kyiv_river", "hot_paris_lumiere"]
            }
        ];
    }

    private static IEnumerable<HotelBooking> CreateBookings()
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