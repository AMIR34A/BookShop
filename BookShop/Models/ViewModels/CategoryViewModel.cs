namespace BookShop.Models.ViewModels
{
    public class CategoryViewModel
    {
        public class TreeViewCategory
        {
            public TreeViewCategory()
            {
                subs = new List<TreeViewCategory>();
            }

            public int id { get; set; }
            public string title { get; set; }
            public List<TreeViewCategory> subs { get; set; }
        }
    }
}
