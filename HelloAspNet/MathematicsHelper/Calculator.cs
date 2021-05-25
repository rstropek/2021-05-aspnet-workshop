using Microsoft.Extensions.DependencyInjection;
using System;

namespace MathematicsHelper
{
    public interface ICalculator
    {
        int Add(int x, int y);
    }

    public class Calculator : ICalculator
    {
        public int Add(int x, int y) => x + y;
    }

    public static class AspNetCoreCalculatorExtensions
    {
        public static void AddMathematicsHelpers(this IServiceCollection services)
        {
            services.AddSingleton<ICalculator, Calculator>();
            // ...
        }
    }
}
