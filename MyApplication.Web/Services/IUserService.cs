using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace MyApplication.Web.Services
{
    public interface IUserService
    {
        Task<ExternalLoginInfo> GetExternalLoginInfoAsync();
    }
}
