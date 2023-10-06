using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PaymentsTracker.Models;
using PaymentsTracker.Models.ViewModels;
using System.Data;

namespace PaymentsTracker.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IConfiguration _configuration;

        public PaymentController(ApplicationContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            string connectionString = _configuration.GetConnectionString("ApplicationContext");
            List<Payment> pagos = new List<Payment>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ObtenerPagos", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Payment pago = new Payment();
                        pago.Id = Convert.ToInt32(reader["Id"]);
                        pago.Description = reader["Description"].ToString();
                        pagos.Add(pago);
                    }
                }
            }

            return View(pagos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentViewModel paymentViewModel)
        {
            if (ModelState.IsValid)
            {
                var paymentExists = await _context.Payments.AnyAsync(p => p.Description == paymentViewModel.Description);
                if (paymentExists)
                {
                    ViewData["Error"] = "Ya existe un pago con el mismo nombre.";
                    return View(paymentViewModel);
                }

                string connectionString = _configuration.GetConnectionString("ApplicationContext");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("CrearPago", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Description", paymentViewModel.Description);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(paymentViewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound();

            var paymentViewModel = new PaymentViewModel
            {
                Id = payment.Id,
                Description = payment.Description
            };

            return View(paymentViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PaymentViewModel paymentViewModel)
        {
            if (id != paymentViewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var paymentExists = await _context.Payments.AnyAsync(p => p.Description == paymentViewModel.Description && p.Id != paymentViewModel.Id);
                    if (paymentExists)
                    {
                        ViewData["Error"] = "Ya existe un pago con el mismo nombre.";
                        return View(paymentViewModel);
                    }
                    string connectionString = _configuration.GetConnectionString("ApplicationContext");
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        using (SqlCommand cmd = new SqlCommand("EditarPago", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Id", paymentViewModel.Id);
                            cmd.Parameters.AddWithValue("@Description", paymentViewModel.Description);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(paymentViewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                var idsToDelete = new List<int> { id.Value };

                string connectionString = _configuration.GetConnectionString("ApplicationContext");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    DataTable idTable = new DataTable();
                    idTable.Columns.Add("Id", typeof(int));
                    foreach (var idToDelete in idsToDelete)
                    {
                        idTable.Rows.Add(idToDelete);
                    }

                    using (SqlCommand cmd = new SqlCommand("BorrarPagos", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter parameter = new SqlParameter("@Ids", SqlDbType.Structured);
                        parameter.Value = idTable;
                        parameter.TypeName = "dbo.IntArray";

                        cmd.Parameters.Add(parameter);

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BulkDelete(List<int> selectedItems)
        {
            if (selectedItems != null && selectedItems.Any())
            {
                string connectionString = _configuration.GetConnectionString("ApplicationContext");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    DataTable idTable = new DataTable();
                    idTable.Columns.Add("Id", typeof(int));
                    foreach (var id in selectedItems)
                    {
                        idTable.Rows.Add(id);
                    }

                    using (SqlCommand cmd = new SqlCommand("BorrarPagos", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter parameter = new SqlParameter("@Ids", SqlDbType.Structured);
                        parameter.Value = idTable;
                        parameter.TypeName = "dbo.IntArray";

                        cmd.Parameters.Add(parameter);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction(nameof(Index));

        }
    }
}
