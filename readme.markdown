Fluent BLToolkit now is part of [BLToolkit](http://bltoolkit.net/)
## Getting started
### BLToolkit attribute way

    [TableName("Categories")]
    public class Category
    {
        [PrimaryKey, Identity] public int    CategoryID;
                               public string CategoryName;
        [Nullable]             public string Description;
        [Nullable]             public Binary Picture;

        [Association(ThisKey="CategoryID", OtherKey="CategoryID")]
        public List<Product> Products;
    }

### Fluent way

Data model class

    public class Category
    {
        public int    CategoryID;
        public string CategoryName;
        public string Description;
        public Binary Picture;

        public List<Product> Products;
    }

Data mapping settings

    public class CategoryMap : FluentMap<Category>
    {
        public CategoryMap()
        {
            TableName("Categories");
			PrimaryKey(_ => _.CategoryID).Identity();
			Nullable(_ => _.Description);
			Nullable(_ => _.Picture);
			Association(_ => _.Products,_ => _.CategoryID).ToMany((Product _) => _.CategoryID);
        }
    }

Apply settings

	public static void Main()
	{
	    FluentConfig
		    .Configure(Map.DefaultSchema)
			    .MapingFromAssemblyOf<Category>();
	}

## License
Fluent BLToolkit is licensed under [MIT License](http://en.wikipedia.org/wiki/MIT_License)