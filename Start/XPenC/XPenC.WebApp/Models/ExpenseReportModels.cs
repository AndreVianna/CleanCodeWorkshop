using System;

namespace XPenC.WebApp.Models
{
    public class ERDetails
    {
        public int Id { get; set; }

        public string Client { get; set; }

        public DateTime CreateDate { get; set; }
        
        public DateTime ChangeDate { get; set; }

        public Item[] Items { get; set; }
        
        public decimal Meal { get; set; }

        public decimal SumIts { get; set; }

        public class Item
        {
            public int? ItemId { get; set; }

            public DateTime? Date { get; set; }

            public string ExpenseType { get; set; }

            public string Description { get; set; }

            public decimal? Value { get; set; }
        }
    }

    public class ERListItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Created { get; set; }

        public DateTime Changed { get; set; }
    }

    public class ERUpdate
    {
        public ERUpdate()
        {
            Items = Array.Empty<Item>();
            NewItem = new Item();
        }

        public int Id { get; set; }

        public string ClientText { get; set; }

        public Item[] Items { get; set; }

        public Item NewItem { get; set; }

        public class Item
        {
            public Item()
            {
                ItemDate = DateTime.Now;
                Price = 0;
            }

            public int? Number { get; set; }

            public DateTime? ItemDate { get; set; }

            public string Type { get; set; }

            public string Description { get; set; }

            public decimal? Price { get; set; }
        }
    }
}
