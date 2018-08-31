using Microsoft.AspNetCore.Mvc;
using DependencyInjectionJson.IServices;

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
            return _test.DoSomething("[TestController][Get()]");
        }

        // GET: api/Test/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return _test.DoSomething("[TestController][Get(" + id + ")]");
        }
    }
}
