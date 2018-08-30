using DependencyInjectionJson.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace DependencyInjectionJson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : Controller
    {
        private readonly ITestService _test;

        public TestController(ITestService test)
        {
            this._test = test;
        }
        // GET: api/Test
        [HttpGet]
        public string Get()
        {
            return _test.DoSomething("[Get()]");
        }

        // GET: api/Test/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return _test.DoSomething("[Get(" + id + ")]");
        }
    }
}
