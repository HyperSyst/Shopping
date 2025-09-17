using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Shopping.Repository;
namespace Shopping.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "admin")]
public class OrderController : Controller
{
    private readonly DataContext _dataContext;
    public OrderController(DataContext context)
    {
        _dataContext = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync());
    }

    [HttpGet]
    //[Route("ViewOrder")]
    public async Task<IActionResult> ViewOrder(string ordercode)
    {
        var DetailsOrder = await _dataContext.OrderDetails.Include(od => od.Product)
            .Where(od => od.OrderCode == ordercode).ToListAsync();

        var Order = _dataContext.Orders.Where(o => o.OrderCode == ordercode).First();

        var order = _dataContext.Orders.FirstOrDefault(o => o.OrderCode == ordercode);
        ViewBag.Status = order?.Status;

        var shippingCost = _dataContext.Orders.Where(o => o.OrderCode == ordercode).First();
        ViewBag.ShippingCost = shippingCost.ShippingCost;

        //ViewBag.ShippingCost = Order.ShippingCost;
        ViewBag.Status = Order.Status;
        return View(DetailsOrder);
    }

    [HttpPost]
    //[Route("UpdateOrder")]
    public async Task<IActionResult> UpdateOrder(string ordercode, int status)
    {
        var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);

        if (order == null)
        {
            return NotFound();
        }

        order.Status = status;
        _dataContext.Update(order);

        if(status == 0)
        {
            var DetailsOrder = await _dataContext.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderCode == order.OrderCode)
                .Select(od => new
                {
                    od.Quantity,
                    od.Product.Price,
                    od.Product.CapitalPrice
                }).ToListAsync();

            var statisticalModel = await _dataContext.Statistical
                .FirstOrDefaultAsync(s => s.DateCreated.Date == order.CreatedDate.Date);

            if (statisticalModel != null)
            {
                foreach (var orderDetail in DetailsOrder)
                {
                    statisticalModel.Quantity += 1;
                    statisticalModel.Sold += orderDetail.Quantity;
                    statisticalModel.Revenue += orderDetail.Quantity * orderDetail.Price;
                    statisticalModel.Profit += orderDetail.Price - orderDetail.CapitalPrice;
                }
                _dataContext.Update(statisticalModel);
            }
            else
            {
                int new_quantity = 0;
                int new_sold = 0;
                decimal new_profit = 0;  
                foreach (var orderDetail in DetailsOrder)
                {
                    new_quantity += 1;
                    new_sold += orderDetail.Quantity;
                    new_profit += orderDetail.Price - orderDetail.CapitalPrice;

                    statisticalModel = new Models.StatisticalModel
                    {
                        DateCreated = order.CreatedDate,
                        Quantity = new_quantity,
                        Sold = new_sold,
                        Revenue = orderDetail.Quantity * orderDetail.Price,
                        Profit = new_profit
                    };
                }
                _dataContext.Add(statisticalModel);
            }
        }

        try
        {
            await _dataContext.SaveChangesAsync();
            return Ok(new { success = true, message = "Order status updated successfully" });
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while updating the order status.");
        }
    }

    //[Route("Delete")]
    public async Task<IActionResult> Delete(string ordercode)
    {
        var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
        var detailOrder = await _dataContext.OrderDetails.FirstOrDefaultAsync(o => o.OrderCode == ordercode);

        if (order == null)
        {
            return NotFound();
        }
        try
        {
            //delete order
            _dataContext.Orders.Remove(order);
            _dataContext.OrderDetails.Remove(detailOrder);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa đơn hàng thành công";
            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while deleting the order.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> PaymentVnpayInfo(string paymentID)
    {
        var VnpayInfo = await _dataContext.Vnpay.FirstOrDefaultAsync(v => v.PaymentId == paymentID);

        if (VnpayInfo == null)
        {
            return NotFound();
        }
        else
        {
            return View(VnpayInfo);
        }
    }
}