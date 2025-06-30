using CarServiceSystem.Data;
using CarServiceSystem.Models;
using CarServiceSystem.Services;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;



namespace CarServiceSystem.Controllers
{
    public class ClientsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public ClientsController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;

        }

        public async Task<IActionResult> SendTestEmail(int clientId)
        {
            var client = await _context.Clients.FindAsync(clientId);
            if (client == null) return NotFound();

            await _emailService.SendEmailAsync(
                client.Email,
                "Test Email from CarService",
                "This is a test email body."
            );

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SendRepairHistory(int clientId)
        {
            try
            {
                Console.WriteLine($"Początek SendRepairHistory dla clientId: {clientId}");

                var client = await _context.Clients
                    .Include(c => c.Orders)
                    .FirstOrDefaultAsync(c => c.Id == clientId);

                if (client == null)
                {
                    Console.WriteLine("Klient nie znaleziony");
                    TempData["Error"] = "Klient nie znaleziony";
                    return RedirectToAction(nameof(Index));
                }

                if (string.IsNullOrEmpty(client.Email))
                {
                    TempData["Error"] = "Klient nie ma podanego adresu email";
                    return RedirectToAction(nameof(Index));
                }

                Console.WriteLine($"Znaleziono klienta: {client.FullName}, zamówień: {client.Orders?.Count ?? 0}");

                var history = new System.Text.StringBuilder();
                history.AppendLine($"Historia zamówień dla {client.FullName}");
                history.AppendLine($"Data: {DateTime.Now:dd.MM.yyyy HH:mm}");
                history.AppendLine("----------------------------");

                if (client.Orders != null && client.Orders.Any())
                {
                    foreach (var order in client.Orders)
                    {
                        history.AppendLine($"\nZamówienie #{order.Id}");
                        history.AppendLine($"Auto: {order.CarBrand} ({order.CarLicensePlate})");
                        history.AppendLine($"Opis: {order.Description}");
                        history.AppendLine($"Status: {order.Status}");
                    }
                }
                else
                {
                    history.AppendLine("\nKlient nie ma zamówień");
                }

                Console.WriteLine($"Formowany tekst emaila:\n{history}");

                await _emailService.SendEmailAsync(
                    client.Email,  
                    $"Historia zamówień {client.FullName}",
                    history.ToString()
                );

                Console.WriteLine("Email wysłany pomyślnie");
                TempData["Message"] = $"Historię wysłano na {client.Email}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w SendRepairHistory: {ex.ToString()}");
                TempData["Error"] = $"Błąd: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clients.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,Phone")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Phone")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
