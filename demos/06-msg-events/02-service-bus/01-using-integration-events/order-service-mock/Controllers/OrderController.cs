using System;
using FoodApp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodApp.OrderService;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{    
    FoodOrderDBContext ctx;
    EventBus eb;

    public OrderController(FoodOrderDBContext dbcontext, EventBus eventBus)
    {
        ctx = dbcontext;
        eb = eventBus;
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
        var @event = new IntegrationEvent(order);
        eb.Publish(@event);
        return order;
    }
}