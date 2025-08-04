using System;
using System.Collections.Generic;
using System.Linq;
using PDFSharpQR.Models;

namespace PDFSharpQR.Services
{
    public class EventTicketService
    {
        private readonly List<Producer> _producers;
        private readonly List<Venue> _venues;
        private readonly List<Performance> _performances;
        private readonly List<Ticket> _tickets;

        public EventTicketService()
        {
            _producers = InitializeProducers();
            _venues = InitializeVenues();
            _performances = InitializePerformances();
            _tickets = InitializeTickets();
        }

        public List<Producer> GetProducers() => _producers.ToList();
        public List<Venue> GetVenues() => _venues.ToList();
        public List<Performance> GetPerformances() => _performances.ToList();
        public List<Ticket> GetTickets() => _tickets.ToList();

        public Producer? GetProducer(int id) => _producers.FirstOrDefault(p => p.Id == id);
        public Venue? GetVenue(int id) => _venues.FirstOrDefault(v => v.Id == id);
        public Performance? GetPerformance(int id) => _performances.FirstOrDefault(p => p.Id == id);
        public Ticket? GetTicket(int id) => _tickets.FirstOrDefault(t => t.Id == id);

        public List<Performance> GetPerformancesByVenue(int venueId) => 
            _performances.Where(p => p.VenueId == venueId).ToList();

        public List<Performance> GetPerformancesByProducer(int producerId) => 
            _performances.Where(p => p.ProducerId == producerId).ToList();

        public List<Ticket> GetTicketsByPerformance(int performanceId) => 
            _tickets.Where(t => t.PerformanceId == performanceId).ToList();

        private static List<Producer> InitializeProducers()
        {
            return new List<Producer>
            {
                new Producer { Id = 1, Name = "Broadway Productions LLC", Description = "Premier theater production company specializing in musical theater", ContactEmail = "contact@broadwayprod.com", Website = "www.broadwayprod.com" },
                new Producer { Id = 2, Name = "Symphony Entertainment", Description = "Classical music and orchestral performances", ContactEmail = "info@symphonyent.com", Website = "www.symphonyent.com" },
                new Producer { Id = 3, Name = "Rock Nation Concerts", Description = "Rock, pop, and alternative music concerts", ContactEmail = "booking@rocknation.com", Website = "www.rocknation.com" },
                new Producer { Id = 4, Name = "Comedy Central Live", Description = "Stand-up comedy and entertainment shows", ContactEmail = "shows@comedycentral.com", Website = "www.comedycentral.com" },
                new Producer { Id = 5, Name = "Dance Theater Company", Description = "Contemporary and classical dance performances", ContactEmail = "info@dancetheater.com", Website = "www.dancetheater.com" }
            };
        }

        private static List<Venue> InitializeVenues()
        {
            return new List<Venue>
            {
                new Venue { Id = 1, Name = "Grand Opera House", Address = "123 Main Street", City = "New York", State = "NY", ZipCode = "10001", Capacity = 2500, Type = "Opera House", ContactPhone = "(555) 123-4567" },
                new Venue { Id = 2, Name = "City Concert Hall", Address = "456 Music Avenue", City = "Los Angeles", State = "CA", ZipCode = "90210", Capacity = 1800, Type = "Concert Hall", ContactPhone = "(555) 234-5678" },
                new Venue { Id = 3, Name = "Downtown Theater", Address = "789 Theater Row", City = "Chicago", State = "IL", ZipCode = "60601", Capacity = 1200, Type = "Theater", ContactPhone = "(555) 345-6789" },
                new Venue { Id = 4, Name = "Memorial Arena", Address = "321 Sports Drive", City = "Houston", State = "TX", ZipCode = "77001", Capacity = 8000, Type = "Arena", ContactPhone = "(555) 456-7890" },
                new Venue { Id = 5, Name = "Riverside Amphitheater", Address = "654 River Road", City = "Seattle", State = "WA", ZipCode = "98101", Capacity = 5000, Type = "Amphitheater", ContactPhone = "(555) 567-8901" }
            };
        }

        private static List<Performance> InitializePerformances()
        {
            var baseDate = DateTime.Now.Date.AddDays(7); // Start a week from now
            return new List<Performance>
            {
                new Performance { Id = 1, Name = "The Phantom of the Opera", Description = "Classic musical theater masterpiece", Date = baseDate, Duration = TimeSpan.FromHours(2.5), VenueId = 1, ProducerId = 1, Genre = "Musical Theater", BasePrice = 85.00m },
                new Performance { Id = 2, Name = "Symphony No. 9 in D minor", Description = "Beethoven's final symphony performed by the city orchestra", Date = baseDate.AddDays(1), Duration = TimeSpan.FromHours(1.5), VenueId = 2, ProducerId = 2, Genre = "Classical Music", BasePrice = 65.00m },
                new Performance { Id = 3, Name = "Thunder and Lightning Tour", Description = "High-energy rock concert experience", Date = baseDate.AddDays(3), Duration = TimeSpan.FromHours(3), VenueId = 4, ProducerId = 3, Genre = "Rock Concert", BasePrice = 95.00m },
                new Performance { Id = 4, Name = "Comedy Night Spectacular", Description = "Stand-up comedy featuring three headlining comedians", Date = baseDate.AddDays(5), Duration = TimeSpan.FromHours(2), VenueId = 3, ProducerId = 4, Genre = "Comedy", BasePrice = 45.00m },
                new Performance { Id = 5, Name = "Swan Lake", Description = "Classic ballet performance", Date = baseDate.AddDays(7), Duration = TimeSpan.FromHours(2), VenueId = 1, ProducerId = 5, Genre = "Dance", BasePrice = 75.00m },
                new Performance { Id = 6, Name = "Summer Music Festival", Description = "Outdoor concert series", Date = baseDate.AddDays(10), Duration = TimeSpan.FromHours(6), VenueId = 5, ProducerId = 3, Genre = "Music Festival", BasePrice = 120.00m }
            };
        }

        private static List<Ticket> InitializeTickets()
        {
            var tickets = new List<Ticket>();
            var random = new Random(42); // Seed for consistent results
            var ticketId = 1;

            // Generate tickets for each performance
            for (int perfId = 1; perfId <= 6; perfId++)
            {
                var ticketCount = random.Next(15, 25); // 15-25 tickets per performance
                
                for (int i = 0; i < ticketCount; i++)
                {
                    var section = $"SEC{random.Next(1, 4)}";
                    var row = $"{(char)('A' + random.Next(0, 10))}";
                    var seatNum = random.Next(1, 31).ToString();
                    var ticketTypes = new[] { "General", "VIP", "Student", "Senior" };
                    var ticketType = ticketTypes[random.Next(ticketTypes.Length)];
                    var customerNames = new[] { "John Smith", "Jane Doe", "Mike Johnson", "Sarah Wilson", "David Brown", "Lisa Davis", "Chris Taylor", "Amanda White", "Kevin Lee", "Maria Garcia" };
                    var customerName = customerNames[random.Next(customerNames.Length)];
                    
                    var basePrice = GetBasePriceForPerformance(perfId);
                    var priceMultiplier = ticketType == "VIP" ? 1.5m : ticketType == "Student" ? 0.8m : ticketType == "Senior" ? 0.9m : 1.0m;
                    var price = Math.Round(basePrice * priceMultiplier, 2);

                    tickets.Add(new Ticket
                    {
                        Id = ticketId++,
                        PerformanceId = perfId,
                        SeatNumber = seatNum,
                        Section = section,
                        Row = row,
                        Price = price,
                        TicketType = ticketType,
                        PurchaseDate = DateTime.Now.AddDays(-random.Next(1, 30)),
                        CustomerName = customerName,
                        CustomerEmail = $"{customerName.Replace(" ", ".").ToLower()}@email.com",
                        TicketCode = $"TKT{perfId:D3}{ticketId:D6}",
                        IsValid = true,
                        IsUsed = false
                    });
                }
            }

            return tickets;
        }

        private static decimal GetBasePriceForPerformance(int performanceId)
        {
            return performanceId switch
            {
                1 => 85.00m,
                2 => 65.00m,
                3 => 95.00m,
                4 => 45.00m,
                5 => 75.00m,
                6 => 120.00m,
                _ => 50.00m
            };
        }
    }
}