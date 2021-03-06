﻿using ServiceStack.CacheAccess;
using ServiceStack.Common;
using ServiceStack.Northwind.ServiceModel.Operations;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace ServiceStack.Northwind.ServiceInterface
{
	public class CachedCustomersService : RestServiceBase<CachedCustomers>
	{
		public ICacheClient CacheClient { get; set; }

		public override object OnGet(CachedCustomers request)
		{
			return base.RequestContext.ToOptimizedResultUsingCache(
				this.CacheClient, "urn:customers", () => {
					var service = this.ResolveService<CustomersService>();
					return (CustomersResponse) service.Get(new Customers());
				});
		}
	}

	public class CachedCustomerDetailsService : RestServiceBase<CachedCustomerDetails>
	{
		public ICacheClient CacheClient { get; set; }

		public override object OnGet(CachedCustomerDetails request)
		{
			var cacheKey = UrnId.Create<CustomerDetails>(request.Id);
			return base.RequestContext.ToOptimizedResultUsingCache(
				this.CacheClient, cacheKey, () =>
				{
					return (CustomerDetailsResponse)this.ResolveService<CustomerDetailsService>()
						.Get(new CustomerDetails { Id = request.Id });
				});
		}
	}

	public class CachedOrdersService : RestServiceBase<CachedOrders>
	{
		public ICacheClient CacheClient { get; set; }

		public override object OnGet(CachedOrders request)
		{
			var cacheKey = UrnId.Create<Orders>(request.CustomerId ?? "all", request.Page.GetValueOrDefault(0).ToString());
			return base.RequestContext.ToOptimizedResultUsingCache(this.CacheClient, cacheKey, () =>
				{
					return (OrdersResponse)this.ResolveService<OrdersService>()
						.Get(new Orders { CustomerId = request.CustomerId, Page = request.Page });
				});
		}
	}

}
