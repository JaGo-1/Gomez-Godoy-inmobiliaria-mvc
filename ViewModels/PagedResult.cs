namespace inmobiliaria_mvc.ViewModels
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

        //Casteo para utilizar en _Pagination.cshtml
        public PagedResult<object> ToObjectPagedResult()
        {
            return new PagedResult<object>
            {
                Items = Items.Cast<object>().ToList(),
                TotalItems = TotalItems,
                PageNumber = PageNumber,
                PageSize = PageSize
            };
        }
    }

}