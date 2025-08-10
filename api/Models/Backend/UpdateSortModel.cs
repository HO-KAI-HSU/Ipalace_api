namespace npm.api.API.Models.Backend
{
    using System.Collections.Generic;

    public class UpdateSortModel<T>
    {
        public IEnumerable<T> Data { get; set; }
    }
}