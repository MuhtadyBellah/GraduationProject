using ECommerce.Core;
using ECommerce.Errors;
using ECommerce.Repo.Data;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ECommerce.Core.Models.Order;
using ECommerce.DTO;
using ECommerce.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using ECommerce.Core.Models.Laravel;
using ECommerce.Core.Specifications.OrderSpec;

namespace ECommerce.Controllers
{
    public class InvoicesController : ApiBaseController
    {
        private readonly IUnitWork _repos;
        private readonly IMapper _mapper;
        private readonly StoreContext _context;

        public InvoicesController(IUnitWork repos, IMapper mapper, StoreContext context)
        {
            _repos = repos;
            _mapper = mapper;
            this._context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InvoiceDTO>), 200)]
        public async Task<ActionResult<IEnumerable<InvoiceDTO>>> GetAllInvoices()
        {
            var invoice = await _repos.Repo<Invoice>().GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<Invoice>, IEnumerable<InvoiceDTO>>(invoice));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InvoiceDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<InvoiceDTO>> GetInvoiceById(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400));

            var spec = new InvoiceSpec(id);
            var invoice = await _repos.Repo<Invoice>().GetByIdAsync(spec);
            if(invoice is null) 
                return NotFound(new ApiResponse(404));

            var mapped = _mapper.Map<Invoice, InvoiceDTO>(invoice);
            return Ok(mapped);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(InvoiceDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<ActionResult<InvoiceDTO>> UpdateInvoice(int id, [FromQuery] decimal totalAmount, [FromQuery] string address, [FromQuery] string paymentMethod)
        {
            if (id <= 0 || totalAmount < 0) return BadRequest(new ApiResponse(400));

            var spec = new InvoiceSpec(id);
            var exist = await _repos.Repo<Invoice>().GetByIdAsync(spec);
            if (exist == null)
                return NotFound(new ApiResponse(404));

            exist.TotalAmount = totalAmount;
            exist.BillingAddress = address;
            exist.PaymentMethod = paymentMethod;

            try
            {
                _repos.Repo<Invoice>().Update(exist);
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500, $"An error occurred while saving the product: {ex.Message}, StackTrace: {ex.StackTrace}"));
            }

            var mapped = _mapper.Map<Invoice, InvoiceDTO>(exist);
            return Ok(mapped);
        }

        [HttpPost("{orderId}")]
        [Authorize(AuthenticationSchemes = "Sanctum")]
        [ProducesResponseType(typeof(InvoiceDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<InvoiceDTO>> CreateInvoice(int orderId)
        {
            if (orderId <= 0) return BadRequest(new ApiResponse(400));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new ApiResponse(400, "User ID is null or empty"));

            var order = _context.Orders.Where(o => o.Id == orderId).FirstOrDefault();
            if (order == null)
                return NotFound(new ApiResponse(404, "Order not found"));

            if (order.Status != "complete")
                return BadRequest(new ApiResponse(400, "Order isn't paid"));

            var invoice = new Invoice(int.Parse(userId), order.Id, true, order.Total, order.Address, order.PaymentMethod ?? string.Empty, order.PaidAt, order.TransactionId);
            await _repos.Repo<Invoice>().AddAsync(invoice);
            await _repos.CompleteAsync();

            var mapped = _mapper.Map<Invoice, InvoiceDTO>(invoice);
            return Ok(mapped);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ApiResponse>> DeleteInvoice(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400));

            var invoice = await _repos.Repo<Invoice>().GetByIdAsync(id);
            if (invoice == null)
                return NotFound(new ApiResponse(404));

            try
            {
                _repos.Repo<Invoice>().Delete(invoice);
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500, $"An error occurred while saving the product: {ex.Message}, StackTrace: {ex.StackTrace}"));
            }
            return Ok(new ApiResponse(200, "Invoice deleted successfully"));
        }

        [HttpGet("user")]
        [Authorize(AuthenticationSchemes = "Sanctum")]
        [ProducesResponseType(typeof(IEnumerable<Invoice>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetUserInvoices()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new ApiResponse(400, "User ID is null or empty"));

            var spec = new InvoiceSpec(userId);
            var invoices = await _repos.Repo<Invoice>().GetAllAsync(spec);
            if (!invoices.Any() || invoices is null)
                return NotFound(new ApiResponse(404, "No invoices found"));

            var mapped = _mapper.Map<IEnumerable<Invoice>, IEnumerable<InvoiceDTO>>(invoices);
            return Ok(mapped);
        }

        [HttpGet("pdf")]
        [Authorize(AuthenticationSchemes = "Sanctum")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult> GetInvoicesPdf()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new ApiResponse(400, "User ID is null or empty"));

            var spec = new InvoiceSpec(userId);
            var invoices = await _repos.Repo<Invoice>().GetAllAsync(spec);
            if (!invoices.Any() || invoices is null)
                return NotFound(new ApiResponse(404, "No invoices found"));

            var mapped = _mapper.Map<IEnumerable<Invoice>, IEnumerable<InvoiceDTO>>(invoices);
            using var stream = new MemoryStream();
            using (var document = new Document())
            {
                PdfWriter.GetInstance(document, stream);
                document.Open();

                document.Add(new Paragraph($"Invoices for UserID: {userId}"));
                document.Add(new Paragraph(" "));

                foreach (var invoice in mapped)
                {
                    document.Add(new Paragraph($"Invoice Number: {invoice.InvoiceNumber}"));
                    document.Add(new Paragraph($"Invoice Order: {invoice.Order}"));
                    document.Add(new Paragraph($"Date: {invoice.InvoiceDate}"));
                    document.Add(new Paragraph($"Total Amount: {invoice.TotalAmount}$"));
                    document.Add(new Paragraph($"Paid: {(invoice.IsPaid ? "Paid" : "Not Paid Yet")}"));
                    document.Add(new Paragraph($"Billing Address: {invoice.BillingAddress}"));
                    document.Add(new Paragraph($"PaymentMethod: {invoice.PaymentMethod}"));
                    document.Add(new Paragraph($"Payment Date: {invoice.PaymentDate}"));
                    document.Add(new Paragraph($"Transaction ID: {invoice.TransactionId}"));
                    document.Add(new Paragraph("---------------------------------"));
                }

                document.Close();
            }
            return File(stream.ToArray(), "application/pdf", "Invoices.pdf");
        }
    }
}