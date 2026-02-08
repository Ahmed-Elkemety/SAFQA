using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFQA.BLL.Managers.AccountManager.Auth;

namespace SAFQA.BLL.Managers.AccountManager.OAuth
{
    public interface IOAuth
    {
        Task<AuthResult> GoogleLoginAsync(string idToken, string deviceId);
        Task<AuthResult> FacebookLoginAsync(string accessToken, string deviceId);
    }
}
