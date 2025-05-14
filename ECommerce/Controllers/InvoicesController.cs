using ECommerce.Core;
using ECommerce.Errors;
using ECommerce.Repo.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ECommerce.Core.Models.Order;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using ECommerce.Core.Specifications;
using ECommerce.Service.Emails;
using ECommerce.DTO.Request;
using ECommerce.DTO.Response;

namespace ECommerce.Controllers
{
    public class InvoicesController : ApiBaseController
    {
        private readonly IUnitWork _repos;
        private readonly IMapper _mapper;
        private readonly StoreContext _context;
        private readonly IMailServices _mailServices;

        public InvoicesController(IUnitWork repos, IMapper mapper, StoreContext context, IMailServices mailServices)
        {
            _repos = repos;
            _mapper = mapper;
            this._context = context;
            this._mailServices = mailServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<InvoiceResponse>), 200)]
        public async Task<ActionResult<IEnumerable<InvoiceResponse>>> GetAllInvoices()
        {
            var invoice = await _repos.Repo<Invoice>().GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<InvoiceResponse>>(invoice));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InvoiceResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<InvoiceResponse>> GetInvoiceById(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400));

            var spec = new InvoiceSpec(id);
            var invoice = await _repos.Repo<Invoice>().GetByIdAsync(spec);
            if (invoice is null)
                return NotFound(new ApiResponse(404));

            var mapped = _mapper.Map<InvoiceResponse>(invoice);
            return Ok(mapped);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(InvoiceResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<ActionResult<InvoiceResponse>> UpdateInvoice(int id, [FromBody] InvoiceRequest request)
        {
            if (id <= 0 || request.totalAmount < 0) return BadRequest(new ApiResponse(400));

            var spec = new InvoiceSpec(id);
            var exist = await _repos.Repo<Invoice>().GetByIdAsync(spec);
            if (exist == null)
                return NotFound(new ApiResponse(404));

            exist.TotalAmount = request.totalAmount;
            exist.BillingAddress = request.address;
            exist.PaymentMethod = request.paymentMethod;

            try
            {
                _repos.Repo<Invoice>().Update(exist);
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500));
            }

            var mapped = _mapper.Map<Invoice, InvoiceResponse>(exist);
            return Ok(mapped);
        }

        [HttpPost("{orderId}")]
        [Authorize("Sanctum")]
        [ProducesResponseType(typeof(InvoiceResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        public async Task<ActionResult<InvoiceResponse>> CreateInvoice(int orderId)
        {
            if (orderId <= 0) return BadRequest(new ApiResponse(400));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401));

            var order = _context.Orders.Where(o => o.Id == orderId).FirstOrDefault();
            if (order == null)
                return BadRequest(new ApiResponse(400, "Order not found"));

            if (order.Status != "paid")
                return BadRequest(new ApiResponse(400, "Order isn't paid"));

            var invoice = new Invoice(int.Parse(userId), order.Id, true, order.Total, order.Address.ToString() ?? "", order.TransactionId, DateTime.Now, null);
            await _repos.Repo<Invoice>().AddAsync(invoice);
            await _repos.CompleteAsync();

            var mapped = _mapper.Map<InvoiceResponse>(invoice);
            return Ok(mapped);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ApiResponse>> DeleteInvoice(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400));

            var spec = new InvoiceSpec(id);
            var invoice = await _repos.Repo<Invoice>().GetByIdAsync(spec);
            if (invoice is null)
                return NotFound(new ApiResponse(404));

            try
            {
                _repos.Repo<Invoice>().Delete(invoice);
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500));
            }
            return Ok(new ApiResponse(200, "Invoice deleted successfully"));
        }

        [HttpGet("user")]
        [Authorize("Sanctum")]
        [ProducesResponseType(typeof(IEnumerable<InvoiceResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<IEnumerable<InvoiceResponse>>> GetUserInvoices()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401));

            var spec = new InvoiceSpec(int.Parse(userId), default);
            var invoices = await _repos.Repo<Invoice>().GetAllAsync(spec);
            if (!invoices.Any() || invoices is null)
                return NotFound(new ApiResponse(404));

            var mapped = _mapper.Map<IEnumerable<InvoiceResponse>>(invoices);
            return Ok(mapped);
        }

        [HttpGet("pdf")]
        [Authorize("Sanctum")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult> GetInvoicesPdf()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401));

            var spec = new InvoiceSpec(int.Parse(userId), default);
            var invoices = await _repos.Repo<Invoice>().GetAllAsync(spec);
            if (!invoices.Any() || invoices is null)
                return NotFound(new ApiResponse(404));

            var mapped = _mapper.Map<IEnumerable<InvoiceResponse>>(invoices);
            using var stream = new MemoryStream();
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var yPoint = 40;

            //Action, Func, Predicate
            Action<string, int?> drawText = (text, size) => 
            {
                gfx.DrawString($"{text}", new XFont("Arial", size ?? 12), XBrushes.Black, new XRect(20, yPoint, 20, 20), XStringFormats.TopLeft);
                yPoint += 20;
            };

            drawText($"Invoices for UserID: {userId}", 20);
            foreach (var invoice in mapped)
            {
                drawText($"Invoice Number: {invoice.InvoiceNumber}", null);
                drawText($"Invoice Order: {invoice.Order}", null);
                drawText($"Date: {invoice.InvoiceDate}", null);
                drawText($"Total Amount: {invoice.TotalAmount}$", null);
                drawText($"Paid: {(invoice.IsPaid ? "Paid" : "Not Paid Yet")}", null);
                drawText($"Billing Address: {invoice.BillingAddress}", null);
                drawText($"Payment Method: {invoice.PaymentMethod}", null);
                drawText($"Payment Date: {invoice.PaymentDate}", null);
                drawText($"Transaction ID: {invoice.TransactionId}", null);
                yPoint += 20;
            }

            document.Save(stream, false);
            stream.Position = 0;
            var fileName = $"Invoices_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var file = new FormFile(stream, 0, stream.Length, "Invoices", fileName);
            var attachments = new List<IFormFile> { file };

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            await _mailServices.SendEmailAsync(email, "Invoice", "Please Check You Invoice", attachments);
            return Ok(new ApiResponse(200, "Please Check Email and Check Your Invoice"));
        }
    }
}