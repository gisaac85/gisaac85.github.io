namespace Core.Entities.ProductModels
{
    public class Product : ModelBase
    {
        public Product(int id, string name,string description,decimal price,string pictureUrl,int productTypeId,int productBrnadId, ProductBrand productBrand,ProductType productType)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            PictureUrl=pictureUrl;
            ProductBrandId = productBrnadId ;
            ProductTypeId = productTypeId;
            ProductBrand = productBrand;
            ProductType = productType;
        }
        public Product() { }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PictureUrl { get; set; }
        public ProductType ProductType { get; set; }
        public int ProductTypeId { get; set; }
        public ProductBrand ProductBrand { get; set; }
        public int ProductBrandId { get; set; }
    }
}