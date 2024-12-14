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
        [Description("Scrapes the content of a web page using an external online service.")]
        public async Task<string> ScrapingPageAsync(
            [Description("The URL to scrape. Required parameter.")] 
            string url,
            [Description("Formats to include in the output. " +
                        "Available options: markdown, html, rawHtml, links, screenshot, extract, screenshot@fullPage. " +
                        "Default is markdown.")]
            string[]? formats = null,
            [Description("Only return the main content of the page excluding headers, navs, footers, etc. Default is true.")]
            bool onlyMainContent = true,
            [Description("Tags to include in the output.")]
            string[] includeTags = null,
            [Description("Tags to exclude from the output.")]
            string[] excludeTags = null,
            //[Description("Headers to send with the request. Can be used to send cookies, user-agent, etc.")]
            //object headers = null,
            [Description("Specify a delay in milliseconds before fetching the content, allowing the page sufficient time to load. Default is 0.")]
            int waitFor = 0,
            [Description("Set to true if you want to emulate scraping from a mobile device. Useful for testing responsive pages and taking mobile screenshots. Default is false.")]
            bool mobile = false,
            [Description("Skip TLS certificate verification when making requests. Default is false.")]
            bool skipTlsVerification = false,
            [Description("Timeout in milliseconds for the request. Default is 30000.")]
            int timeout = 30000
            //[Description("Extract object containing extraction parameters.")]
            //object extract = null,
            //[Description("Actions to perform on the page before grabbing the content.")]
            //object[] actions = null
        )
        {
            // Define the supported formats
            var supportedFormats = new[] { "markdown", "html", "rawHtml", "links", "screenshot", "extract", "screenshot@fullPage" };

            // Check if the requested formats are supported
            if (formats != null && formats.Any(format => !supportedFormats.Contains(format)))
            {
                return "The requested format(s) are not supported for scraping. Supported formats are: " +
                    string.Join(", ", supportedFormats);
            }

            // Call the updated ScrapingPageAsync method with the provided parameters
            var response = await webSpracping.ScrapingPageAsync(
                url,
                formats,
                onlyMainContent,
                includeTags,
                excludeTags,
                //headers,
                waitFor,
                mobile,
                skipTlsVerification,
                timeout
                //extract,
                //actions
            );

            return response;
        }

        [KernelFunction("map_we_page")]
        [Description("Map a website url and get all the urls on the website (extremely fast).")]
        public async Task<string> MapPageAsync(
            [Description("Url/Link/Uri page to scrap")] string url,
            [Description("Filter for search")] string? search = null,
            [Description("Ignore the website sitemap when crawling. Default is false.")] bool ignoreSitemap = false,
            [Description("Only return links found in the website sitemap. Default is false.")] bool sitemapOnly = false,
            [Description("Include subdomains of the website. Default is false.")] bool includeSubdomains = false,
            [Description("Maximum number of links to return. Required range: x < 5000. Default is 5000.")] int limit = 5000
            )
        {
            return await webSpracping.MapPageAsync(url, search, ignoreSitemap, sitemapOnly, includeSubdomains, limit);
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
