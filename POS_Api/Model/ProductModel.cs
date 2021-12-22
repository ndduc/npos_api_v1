using System.Collections.Generic;

namespace POS_Api.Model
{
    public class ProductModel
    {
        public string UId { get; set; }
        public string Description { get; set; }
        public string SecondDescription { get; set; }
        public string ThirdDescription { get; set; }
        public int Upc { get; set; }
        public int ItemCode { get; set; }
        public double Cost { get; set; }
        public double Price { get; set; }
        public string AddedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsError { get; set; }
        public string Error { get; set; }

        public List<string> ItemCodeList { get; set; }
        public ProductModel()
        {

        }

        public ProductModel(string uid, string desc, string secDesc, string thirdDesc, int upc,
            double cost, double price, string addedDatetime, string updateDatetime, string addBy, string updateBy)
        {
            UId = uid;
            Description = desc;
            SecondDescription = secDesc;
            ThirdDescription = thirdDesc;
            Upc = upc;
            Cost = cost;
            Price = price;
            AddedDateTime = addedDatetime;
            UpdatedDateTime = updateDatetime;
            AddedBy = addBy;
            UpdatedBy = updateBy;
        }

        //Use In Add Product
        public ProductModel(string desc, string secDesc, string thirdDesc, int upc,
           double cost, double price)
        {
            Description = desc;
            SecondDescription = secDesc;
            ThirdDescription = thirdDesc;
            Upc = upc;
            Cost = cost;
            Price = price;
        }


        //Use In Update Product
        public ProductModel(string uid, string desc, string secDesc, string thirdDesc, int upc,
           double cost, double price)
        {
            UId = uid;
            Description = desc;
            SecondDescription = secDesc;
            ThirdDescription = thirdDesc;
            Upc = upc;
            Cost = cost;
            Price = price;
        }

    }
}
