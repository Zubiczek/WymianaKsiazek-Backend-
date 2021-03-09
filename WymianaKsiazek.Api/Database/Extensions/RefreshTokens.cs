using System;
using WymianaKsiazek.Api.Database.Entities;

namespace WymianaKsiazek.Api.Database.Extensions
{
    public static class RefreshTokens
    {
        public static bool IsActive(this RefreshTokenEntity refreshToken, DateTime dateTimeNow)
        {
            return refreshToken.ExpiresOn > dateTimeNow;
        }
    }
}
