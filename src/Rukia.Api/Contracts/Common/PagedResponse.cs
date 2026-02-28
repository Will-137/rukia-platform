using System;
using System.Collections.Generic;

namespace Rukia.Api.Contracts.Common
{
    public sealed class PagedResponse<T>
    {
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int Total { get; init; }
    }
}