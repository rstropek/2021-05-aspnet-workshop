using MathematicsHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloAspNet.Controllers
{
    //public interface ICalculator
    //{
    //    int Add(int x, int y);
    //}

    //public class Calculator : ICalculator
    //{
    //    public int Add(int x, int y) => x + y;
    //}

    [Route("api/[controller]")]
    [ApiController]
    public class MathController : ControllerBase
    {
        private readonly ICalculator calc;

        public MathController(ICalculator calc)
        {
            this.calc = calc;
        }

        // https://localhost:5001/api/math?x=21&y=21

        [HttpGet]
        public int Add([FromQuery] int x, [FromQuery] int y)
            => calc.Add(x, y);
    }
}
