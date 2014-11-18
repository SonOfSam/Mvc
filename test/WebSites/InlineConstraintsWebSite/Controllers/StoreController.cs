using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;

namespace InlineConstraintsWebSite.Controllers
{
    public class StoreController : Controller
    {
        public IDictionary<string, object> GetStoreById(Guid id)
        {
            return ActionContext.RouteData.Values;
        }

        public IDictionary<string, object> GetStoreByLocation(string location)
        {
            return ActionContext.RouteData.Values;
        }
    }
}