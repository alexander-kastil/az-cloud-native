using System;
using FoodApp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DiagnosticAdapter.Internal;

namespace FoodApp.OrderService;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{    
    FoodOrderDBContext ctx;
    ServiceBusProxy proxy;

    public OrderController(FoodOrderDBContext dbcontext, ServiceBusProxy sbproxy)
    {
        ctx = dbcontext;
        proxy = sbproxy;
    }

    [HttpGet]
    public async Task<IEnumerable<FoodOrder>> Get()
    {
        return await ctx.Orders.ToListAsync<FoodOrder>();
    }

    [HttpPost()]
    public async Task<FoodOrder> AddOrder(FoodOrder order){
        await ctx.Orders.AddAsync(order);
        await ctx.SaveChangesAsync();
        var evt = new IntegrationEvent<FoodOrder>(order);
        proxy.AddEvent(evt);
        return order;
    }
}