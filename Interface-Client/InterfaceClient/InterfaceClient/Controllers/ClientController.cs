using InterfaceClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace ProductsApp.Controllers
{
    public class ProductsController : ApiController
    {
        ChansonClient[] products = new ChansonClient[]
        {
            new ChansonClient { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
            new ChansonClient { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
            new ChansonClient { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
        };

        public IEnumerable<ChansonClient> GetAllProducts()
        {
            return products;
        }

        public IHttpActionResult GetProduct(int id)
        {
            var product = products.FirstOrDefault((p) => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}