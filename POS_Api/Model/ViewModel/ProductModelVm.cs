using System.Collections.Generic;

namespace POS_Api.Model.ViewModel
{
    public class ProductModelVm
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

        public List<string> UpcList { get; set; }

        public List<CategoryModel> CategoryList { get; set; }
        public List<DepartmentModel> DepartmentList { get; set; }
        public List<SectionModel> SectionList { get; set; }
        public List<VendorModel> VendorList { get; set; }
        public List<DiscountModel> DiscountList { get; set; }   // Logically, will have only one discount
        public List<TaxModel> TaxList { get; set; } // Logically, will have only one tax

        public ProductModelVm(ProductModel model)
        {
            UId = model.UId;
            Description = model.Description;
            SecondDescription = model.SecondDescription;
            ThirdDescription = model.ThirdDescription;
            Upc = model.Upc;
            ItemCode = model.ItemCode;
            Cost = model.Cost;
            Price = model.Price;
            AddedDateTime = model.AddedDateTime;
            UpdatedDateTime = model.UpdatedDateTime;
            AddedBy = model.AddedBy;
            UpdatedBy = model.UpdatedBy;
            IsError = model.IsError;
            Error = model.Error;
            ItemCodeList = model.ItemCodeList;
            UpcList = model.UpcList;
        }
    }
}
