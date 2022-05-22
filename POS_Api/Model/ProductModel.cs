using System;
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
        public bool ApplyToUI { get; set; }
        public string AddedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }
        public string AddedBy { get; set; }
        public string UpdatedBy { get; set; }

        public List<string> DepartmentList { get; set; }
        public List<string> CategoryList { get; set; }
        public List<string> VendorList { get; set; }
        public List<string> SectionList { get; set; }
        public List<string> DiscountList { get; set; }
        public List<string> TaxList { get; set; }
        public List<string> ItemCodeList { get; set; }
        public List<string> UpcList { get; set; }

        public string UserUId { get; set; }
        public string LocationUId { get; set; }

        public bool IsError { get; set; }
        public string Error { get; set; }

        public void print()
        {
            Console.WriteLine(UId);
            Console.WriteLine(Description);
            Console.WriteLine(SecondDescription);
            Console.WriteLine(ThirdDescription);
            Console.WriteLine(Upc);
            Console.WriteLine(ItemCode);
            Console.WriteLine(Cost);
            Console.WriteLine(Price);
            Console.WriteLine(AddedDateTime);
            Console.WriteLine(UpdatedDateTime);
            Console.WriteLine(AddedBy);
            Console.WriteLine(UpdatedBy);
            Console.WriteLine(Error);
        }

        public ProductModel()
        {

        }

        public ProductModel(string uid, string desc, string secDesc, string thirdDesc, int upc,
            double cost, double price, string addedDatetime, string updateDatetime, string addBy, string updateBy, bool applyToUI)
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
            ApplyToUI = applyToUI;
        }

        //Use In Add Product
        public ProductModel(string desc, string secDesc, string thirdDesc,
           double cost, double price, List<string> departmentList,
           List<string> categoryList, List<string> vendorList,
           List<string> sectionList, List<string> discountList,
           List<string> taxList, List<string> itemCodeList,
           List<string> upcList, string userUId, string locationUId)
        {
            Description = desc;
            SecondDescription = secDesc;
            ThirdDescription = thirdDesc;
            Cost = cost;
            Price = price;
            DepartmentList = departmentList;
            CategoryList = categoryList;
            SectionList = sectionList;
            VendorList = vendorList;
            DiscountList = discountList;
            TaxList = taxList;
            ItemCodeList = itemCodeList;
            UpcList = upcList;
            UserUId = userUId;
            LocationUId = locationUId;

        }


        //Use In Update Product
        public ProductModel(string desc, string secDesc, string thirdDesc,
           double cost, double price, List<string> departmentList,
           List<string> categoryList, List<string> vendorList,
           List<string> sectionList, List<string> discountList,
           List<string> taxList, List<string> itemCodeList,
           List<string> upcList, string userUId, string locationUId, string productUid)
        {
            Description = desc;
            SecondDescription = secDesc;
            ThirdDescription = thirdDesc;
            Cost = cost;
            Price = price;
            DepartmentList = departmentList;
            CategoryList = categoryList;
            SectionList = sectionList;
            VendorList = vendorList;
            DiscountList = discountList;
            TaxList = taxList;
            UserUId = userUId;
            LocationUId = locationUId;
            UpcList = upcList;
            ItemCodeList = itemCodeList;
            UId = productUid;

        }

    }
}
