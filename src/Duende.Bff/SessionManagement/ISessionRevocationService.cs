// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System.Threading.Tasks;

namespace Duende.Bff
{
    /// <summary>
    /// Session revocation service
    /// </summary>
    public interface ISessionRevocationService
    {
        /// <summary>
        /// Deletes a user session
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task DeleteUserSessionsAsync(UserSessionsFilter filter);
    }
}