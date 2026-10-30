using KronoGeo_Api.Models.Infrastructure.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceHttpManagerClientAssembly
    {
        public Task<ResponseApiManager> GetListUsersAsync();
    }
}
