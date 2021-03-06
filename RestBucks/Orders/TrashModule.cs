namespace RestBucks.Orders
{
  using System.Linq;
  using Data;
  using Domain;
  using Infrastructure;
  using Infrastructure.Linking;
  using Nancy;
  using Nancy.Routing;
  using Representations;

  public class TrashModule : NancyModule
  {
    private readonly IRepository<Order> orderRepository;
    private readonly ResourceLinker linker;
    private readonly IRouteCacheProvider routeCacheProvider;
    public const string GetCancelledPath = "/order/{orderId}";
    public const string path = "/trash";

    public TrashModule(IRepository<Order> orderRepository, ResourceLinker linker)
      : base(path)
    {
      this.orderRepository = orderRepository;
      this.linker = linker;
      this.routeCacheProvider = routeCacheProvider;

      Get["ReadCancelledOrder", GetCancelledPath] = parameters => GetCanceled((int) parameters.orderId);
    }

    private object GetCanceled(int orderId)
    {
      var order = orderRepository.Retrieve(o => o.Id == orderId && o.Status == OrderStatus.Canceled)
        .FirstOrDefault();

      if (order == null)
      {
          return HttpStatusCode.NotFound;
      }

      return 
        Negotiate
        .WithModel(OrderRepresentationMapper.Map(order, linker, Context))
        .WithCacheHeaders(order);
    }
  }
}