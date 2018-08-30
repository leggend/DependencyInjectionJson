﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionJson.Repositories
{
    public class TestAlternativoRepository : ITestRepository
    {
        public string GetRepositoryInfo(string serviceName)
        {
            return serviceName + "[TestAlternativoRepository] - Hola desde el repositorio alternativo.";
        }
    }

}
