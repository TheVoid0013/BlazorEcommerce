using BlazorEcommerce.Server.Services.AuthService;
using BlazorEcommerce.Server.Services.CartService;
using ZiggyCreatures.Caching.Fusion;

namespace BlazorEcommerce.Server.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _context;
        private readonly ICartService _cartService;
        private readonly IAuthService _authService;
        private readonly IFusionCache _fusionCache;

        public OrderService(DataContext context, ICartService cartService, IAuthService authService, IFusionCache fusionCache)
        {
            _context = context;
            _cartService = cartService;
            _authService = authService;
            _fusionCache = fusionCache;
        }

        public async Task<ServiceResponse<bool>> PlaceOrder()
        {
            var products = (await _cartService.GetDbCartProducts()).Data;
            decimal totalPrice = 0;
            products.ForEach(product => totalPrice += product.Price * product.Quantity);

            var orderItems = new List<OrderItem>();
            products.ForEach(product => orderItems.Add(new OrderItem
            {
                ProductId = product.ProductId,
                ProductTypeId = product.ProductTypeId,
                Quantity = product.Quantity,
                TotalPrice = product.Price * product.Quantity
            }));

            var order = new Order
            {
                UserId = _authService.GetUserId(),
                OrderDate = DateTime.Now,
                TotalPrice = totalPrice,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(_context.CartItems.Where(ci => ci.UserId == _authService.GetUserId()));

            await _context.SaveChangesAsync();

            // Clear cart cache after placing order
            await _fusionCache.RemoveAsync($"cart_products_{_authService.GetUserId()}");
            await _fusionCache.RemoveAsync($"cart_count_{_authService.GetUserId()}");

            return new ServiceResponse<bool> { Data = true };
        }
    }
}