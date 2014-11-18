using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InlineConstraintsWebSite.Controllers
{
    [Route("[controller]/[action]")]
    public class ProductsController : Controller
    {
        public IDictionary<string, object> Index()
        {
            return ActionContext.RouteData.Values;
        }

        [HttpGet("{id:int?}")]
        public IDictionary<string, object> GetProductById(int id)
        {
            return ActionContext.RouteData.Values;
        }

        [HttpGet("{name:alpha}")]
        public IDictionary<string, object> GetProductByName(string name)
        {
            return ActionContext.RouteData.Values;
        }

        [HttpGet("{dateTime:datetime}")]
        public IDictionary<string, object> GetProductByManufacturingDate(DateTime dateTime)
        {
            return ActionContext.RouteData.Values;
        }

        [HttpGet("{name:length(1,20)?}")]
        public IDictionary<string, object> GetProductByCategoryName(string name)
        {
            return ActionContext.RouteData.Values;
        }

        [HttpGet("{catId:int:range(10, 100)}")]
        public IDictionary<string, object> GetProductByCategoryId(int catId)
        {
            return ActionContext.RouteData.Values;
        }

        [HttpGet("{price:float?}")]
        public IDictionary<string, object> GetProductByPrice(float price)
        {
            return ActionContext.RouteData.Values;
        }
        
        [HttpGet("{manId:int:min(10)?}")]
        public IDictionary<string, object> GetProductByManufacturarId(int manId)
        {
            return ActionContext.RouteData.Values;
        }
    }
}