using Zefir.Core.Entity;

namespace Zefir.BL.Contracts;

public record PublicBasketData(int UserId, List<Product> Products);
