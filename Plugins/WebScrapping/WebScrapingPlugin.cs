using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelPersonalized.Plugins.WebScrapping
{
    public class WebScrapingPlugin(
        IWebScraping webSpracping
    //IUserContext userContext,
    //IPaymentService paymentService
    )
    {
        [KernelFunction("scraping_we_page")]
        [Description("Scrapes the content of a web page using external an online service.")]
        public async Task<string> ScrapingPageAsync(
            [Description("Url/Link/Uri page to scrap")] string url
            )
        {
            return await webSpracping.ScrapingPageAsync(url);
        }

        [KernelFunction("map_we_page")]
        [Description("Map a website url and get all the urls on the website (extremely fast).")]
        public async Task<string> MapPageAsync(
            [Description("Url/Link/Uri page to scrap")] string url,
            [Description("Filter for search")] string? search = null
            )
        {
            return await webSpracping.MapPageAsync(url, search);
        }



        //[KernelFunction("add_pizza_to_cart")]
        //[Description("Add a pizza to the user's cart; returns the new item and updated cart")]
        //public async Task<CartDelta> AddPizzaToCart(
        //    PizzaSize size,
        //    List<PizzaToppings> toppings,
        //    int quantity = 1,
        //    string specialInstructions = ""
        //)
        //{
        //    Guid cartId = userContext.GetCartId();
        //    return await pizzaService.AddPizzaToCart(
        //        cartId: cartId,
        //        size: size,
        //        toppings: toppings,
        //        quantity: quantity,
        //        specialInstructions: specialInstructions);
        //}

        //[KernelFunction("remove_pizza_from_cart")]
        //public async Task<RemovePizzaResponse> RemovePizzaFromCart(int pizzaId)
        //{
        //    Guid cartId = userContext.GetCartId();
        //    return await pizzaService.RemovePizzaFromCart(cartId, pizzaId);
        //}

        //[KernelFunction("get_pizza_from_cart")]
        //[Description("Returns the specific details of a pizza in the user's cart; use this instead of relying on previous messages since the cart may have changed since then.")]
        //public async Task<Pizza> GetPizzaFromCart(int pizzaId)
        //{
        //    Guid cartId = await userContext.GetCartIdAsync();
        //    return await pizzaService.GetPizzaFromCart(cartId, pizzaId);
        //}

        //[KernelFunction("get_cart")]
        //[Description("Returns the user's current cart, including the total price and items in the cart.")]
        //public async Task<Cart> GetCart()
        //{
        //    Guid cartId = await userContext.GetCartIdAsync();
        //    return await pizzaService.GetCart(cartId);
        //}

        //[KernelFunction("checkout")]
        //[Description("Checkouts the user's cart; this function will retrieve the payment from the user and complete the order.")]
        //public async Task<CheckoutResponse> Checkout()
        //{
        //    Guid cartId = await userContext.GetCartIdAsync();
        //    Guid paymentId = await paymentService.RequestPaymentFromUserAsync(cartId);

        //    return await pizzaService.Checkout(cartId, paymentId);
        //}
    }
}
