using System;
using System.Linq;
using PDFSharpQR.Services;
using PDFSharpQR.Models;
using Xunit;

namespace PDFSharpQRTests
{
    public class EventTicketServiceTests
    {
        private readonly EventTicketService _service;

        public EventTicketServiceTests()
        {
            _service = new EventTicketService();
        }

        [Fact]
        public void GetProducers_ShouldReturnNonEmptyList()
        {
            // Act
            var producers = _service.GetProducers();

            // Assert
            Assert.NotNull(producers);
            Assert.NotEmpty(producers);
            Assert.True(producers.Count >= 5);
        }

        [Fact]
        public void GetVenues_ShouldReturnNonEmptyList()
        {
            // Act
            var venues = _service.GetVenues();

            // Assert
            Assert.NotNull(venues);
            Assert.NotEmpty(venues);
            Assert.True(venues.Count >= 5);
        }

        [Fact]
        public void GetPerformances_ShouldReturnNonEmptyList()
        {
            // Act
            var performances = _service.GetPerformances();

            // Assert
            Assert.NotNull(performances);
            Assert.NotEmpty(performances);
            Assert.True(performances.Count >= 6);
        }

        [Fact]
        public void GetTickets_ShouldReturnNonEmptyList()
        {
            // Act
            var tickets = _service.GetTickets();

            // Assert
            Assert.NotNull(tickets);
            Assert.NotEmpty(tickets);
            Assert.All(tickets, ticket => Assert.NotNull(ticket.TicketCode));
        }

        [Fact]
        public void GetProducer_WithValidId_ShouldReturnProducer()
        {
            // Act
            var producer = _service.GetProducer(1);

            // Assert
            Assert.NotNull(producer);
            Assert.Equal(1, producer.Id);
            Assert.NotEmpty(producer.Name);
        }

        [Fact]
        public void GetProducer_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var producer = _service.GetProducer(999);

            // Assert
            Assert.Null(producer);
        }

        [Fact]
        public void GetVenue_WithValidId_ShouldReturnVenue()
        {
            // Act
            var venue = _service.GetVenue(1);

            // Assert
            Assert.NotNull(venue);
            Assert.Equal(1, venue.Id);
            Assert.NotEmpty(venue.Name);
        }

        [Fact]
        public void GetPerformancesByVenue_WithValidVenueId_ShouldReturnPerformances()
        {
            // Act
            var performances = _service.GetPerformancesByVenue(1);

            // Assert
            Assert.NotNull(performances);
            Assert.All(performances, p => Assert.Equal(1, p.VenueId));
        }

        [Fact]
        public void GetPerformancesByProducer_WithValidProducerId_ShouldReturnPerformances()
        {
            // Act
            var performances = _service.GetPerformancesByProducer(1);

            // Assert
            Assert.NotNull(performances);
            Assert.All(performances, p => Assert.Equal(1, p.ProducerId));
        }

        [Fact]
        public void GetTicketsByPerformance_WithValidPerformanceId_ShouldReturnTickets()
        {
            // Act
            var tickets = _service.GetTicketsByPerformance(1);

            // Assert
            Assert.NotNull(tickets);
            Assert.All(tickets, t => Assert.Equal(1, t.PerformanceId));
        }

        [Fact]
        public void Ticket_GetQRCodeContent_ShouldReturnValidFormat()
        {
            // Arrange
            var tickets = _service.GetTickets();
            var ticket = tickets.First();

            // Act
            var qrContent = ticket.GetQRCodeContent();

            // Assert
            Assert.NotNull(qrContent);
            Assert.Contains("TICKET:", qrContent);
            Assert.Contains("PERF:", qrContent);
            Assert.Contains("SEAT:", qrContent);
            Assert.Contains("CUSTOMER:", qrContent);
            Assert.Contains("VALID:", qrContent);
        }

        [Fact]
        public void AllTickets_ShouldHaveUniqueTicketCodes()
        {
            // Act
            var tickets = _service.GetTickets();
            var ticketCodes = tickets.Select(t => t.TicketCode).ToList();

            // Assert
            Assert.Equal(ticketCodes.Count, ticketCodes.Distinct().Count());
        }

        [Fact]
        public void AllPerformances_ShouldHaveFutureDates()
        {
            // Act
            var performances = _service.GetPerformances();

            // Assert
            Assert.All(performances, p => Assert.True(p.Date >= DateTime.Now.Date));
        }

        [Fact]
        public void AllTickets_ShouldHaveValidPrices()
        {
            // Act
            var tickets = _service.GetTickets();

            // Assert
            Assert.All(tickets, t => Assert.True(t.Price > 0));
        }
    }
}