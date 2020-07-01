using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Repositories.RefreshToken;
using Persistence.Repository.RefreshToken;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Base
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private IRefreshTokenRepository _refreshTokenRepository;

        public IRefreshTokenRepository RefreshTokenRepository
        {
            get
            {
                if (_refreshTokenRepository == null)
                    _refreshTokenRepository = new RefreshTokenRepository();

                return _refreshTokenRepository;
            }
        }
    }
}
