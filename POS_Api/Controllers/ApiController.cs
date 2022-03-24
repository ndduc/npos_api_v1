﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POS_Api.Core.Implementation;
using POS_Api.Core.Interface;
using POS_Api.Model;
using POS_Api.Model.EnumData;
using POS_Api.Shared.HttpHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;

namespace POS_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly IUserLogic _UserLogic, _UserLogicParent;
        private readonly ILocationLogic _LocationLogic;
        private readonly IProductLogic _ProductLogic;
        private readonly ICategoryLogic _categoryLogic;
        private readonly IDepartmentLogic _departmentLogic;
        private readonly IDiscountLogic _discountLogic;
        private readonly ISectionLogic _sectionLogic;
        private readonly ITaxLogic _taxLogic;
        private readonly IVendorLogic _vendorLogic;
        private readonly IItemCodeLogic _itemCodeLogic;
        private readonly IUpcLogic _upcLogic;

        private readonly ILogger<ApiController> _logger;

        public ApiController(ILogger<ApiController> logger)
        {
            _logger = logger;
            _UserLogic = new UserLogic();
            _LocationLogic = new LocationLogic();
            _ProductLogic = new ProductLogic();
            _UserLogicParent = new UserLogic();
            _categoryLogic = new CategoryLogic();
            _departmentLogic = new DepartmentLogic();
            _discountLogic = new DiscountLogic();
            _sectionLogic = new SectionLogic();
            _taxLogic = new TaxLogic();
            _vendorLogic = new VendorLogic();
            _itemCodeLogic = new ItemCodeLogic();
            _upcLogic = new UpcLogic();
        }

        #region USER SECTION

        [HttpPost("pos/user/authorize")]
        public dynamic GetUserByPassAndUserName()
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("user", out var u);
                Request.Form.TryGetValue("pass", out var p);
                UserModel res = _UserLogic.GetUserByPassAndUserName(u, p);
                if (res != null)
                {
                    if (res.IsAuthorize)
                    {
                        body = JsonSerializer.Serialize(res);
                        return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                    }
                    else if (!res.IsAuthorize && !res.IsError)
                    {
                        body = "Non Authorized";
                        return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                    }
                    else
                    {
                        body = "ERROR\t\t" + res.Error;
                        return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                    }
                }
                else
                {
                    body = "INTERNAL ERROR\t\tNo Specific Cause";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("pos/user/register")]
        public dynamic RegisterNewUser()
        {
            dynamic body;
            bool isSuccess;
            try
            {
                Request.Form.TryGetValue("username", out var userName);
                Request.Form.TryGetValue("password", out var Password);
                Request.Form.TryGetValue("firstname", out var FirstName);
                Request.Form.TryGetValue("lastname", out var LastName);
                Request.Form.TryGetValue("email", out var Email);
                Request.Form.TryGetValue("email2", out var Email2);
                Request.Form.TryGetValue("phone", out var Phone);
                Request.Form.TryGetValue("address", out var Address);
                Request.Form.TryGetValue("usertype", out var UserType);
                UserModel userModel = new UserModel()
                {
                    UserName = userName,
                    Password = Password,
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Email2 = Email2,
                    Phone = Phone,
                    Address = Address,
                    UserType = UserType
                };
                isSuccess = _UserLogic.AddUser(userModel);

                if (isSuccess)
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "Something, Unable to Registered This User";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("pos/{userid?}/{locaid?}/user/register")]
        public dynamic RegisterNewUserByParent(string userid, string locaid)
        {
            dynamic body;
            bool isSuccess;
            try
            {
                Request.Form.TryGetValue("username", out var userName);
                Request.Form.TryGetValue("password", out var Password);
                Request.Form.TryGetValue("firstname", out var FirstName);
                Request.Form.TryGetValue("lastname", out var LastName);
                Request.Form.TryGetValue("email", out var Email);
                Request.Form.TryGetValue("email2", out var Email2);
                Request.Form.TryGetValue("phone", out var Phone);
                Request.Form.TryGetValue("address", out var Address);
                Request.Form.TryGetValue("usertype", out var UserType);
                Request.Form.TryGetValue("type", out var LocationRelationType);
                UserModel userModel = new UserModel()
                {
                    UserName = userName,
                    Password = Password,
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Email2 = Email2,
                    Phone = Phone,
                    Address = Address,
                    UserType = UserType,
                };

                userModel.AddedBy = userid;

                isSuccess = _UserLogicParent.AddUserWithParent(userModel, userid, locaid, LocationRelationType);

                if (isSuccess)
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "Something, Unable to Registered This User";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }


        [HttpPost("pos/{userid?}/update/info")]
        public dynamic UpdateUserInfo(string userid)
        {
            dynamic body;
            bool isSuccess;
            try
            {
                Request.Form.TryGetValue("username", out var userName);
                Request.Form.TryGetValue("password", out var Password);
                Request.Form.TryGetValue("firstname", out var FirstName);
                Request.Form.TryGetValue("lastname", out var LastName);
                Request.Form.TryGetValue("email", out var Email);
                Request.Form.TryGetValue("email2", out var Email2);
                Request.Form.TryGetValue("phone", out var Phone);
                Request.Form.TryGetValue("address", out var Address);
                Request.Form.TryGetValue("usertype", out var UserType);
                UserModel userModel = new UserModel()
                {
                    UId = userid,
                    UserName = userName,
                    Password = Password,
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Email2 = Email2,
                    Phone = Phone,
                    Address = Address,
                    UserType = UserType
                };
                isSuccess = _UserLogic.UpdateUser(userModel);

                if (isSuccess)
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "Something, Unable to Registered This User";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("pos/{userid}/update/pass")]
        public dynamic UpdateUserPassword(string userid)
        {
            dynamic body;
            bool isSuccess;
            try
            {
                Request.Form.TryGetValue("password", out var Password);
                UserModel userModel = new UserModel()
                {
                    UId = userid,
                    Password = Password
                };
                isSuccess = _UserLogic.UpdatePassword(userModel);

                if (isSuccess)
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "Something, Unable to Registered This User";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Assinging Selected User to Specific Location
        /// </summary>
        /// <param name="muserid">Logged in user id</param>
        /// <param name="userid">to be assigned user id</param>
        /// <param name="locid">location id</param>
        /// <returns></returns>
        [HttpPost("pos/{muserid?}/assign/location/{userid?}/{locaid?}")]
        public dynamic UpdateUserLocation(string muserid, string userid, string locaid)
        {
            dynamic body;
            bool isSuccess;
            try
            {
                Request.Query.TryGetValue("type", out var type);

                if (muserid == null || muserid.Length < 5)
                {
                    isSuccess = _UserLogic.AddRelationLocationUser(null, userid, locaid, type.ToString().ToUpper());
                }
                else
                {
                    isSuccess = _UserLogic.AddRelationLocationUser(muserid, userid, locaid, type.ToString().ToUpper());
                }
                if (isSuccess)
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "Something, Unable to Registered This User";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }


        #endregion

        #region LOCATION
        [HttpPost, Route("pos/{userid?}/location/register")]
        public dynamic RegisterNewLocation(string userid)
        {
            dynamic body;
            bool isSuccess;
            try
            {
                Request.Form.TryGetValue("name", out var Name);
                Request.Form.TryGetValue("address", out var Address);
                Request.Form.TryGetValue("zip", out var ZipCode);
                Request.Form.TryGetValue("state", out var State);
                Request.Form.TryGetValue("phone", out var Phone);
                Request.Form.TryGetValue("userType", out var UserType);

                if (UserType == GenericEnumType.UserLocationType.CREATED.ToString() || UserType == GenericEnumType.UserLocationType.EMPLOYED.ToString())
                {
                    LocationModel model = new LocationModel()
                    {
                        Name = Name,
                        Address = Address,
                        ZipCode = int.Parse(ZipCode),
                        State = State,
                        PhoneNumber = Phone,
                    };
                    isSuccess = _LocationLogic.AddLocation(model, userid);

                    if (isSuccess)
                    {
                        body = "OK";
                        return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                    }
                    else
                    {
                        body = "Something, Unable to Registered This User";
                        return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                    }
                }
                else
                {
                    body = "Invalid User Type";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }


        [HttpGet, Route("pos/{userid?}/location")]
        public dynamic GetLocationById(string userid)
        {
            Debug.WriteLine("USER ID\t\t" + userid);
            dynamic body;
            try
            {
                if (userid == null || userid.Length < 5)
                {
                    body = "Something, Unable To Get The Location";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
                else
                {
                    return HttpResponseHelper.HttpResponse(_LocationLogic.GetLocationByUserId(userid), HttpStatusCode.OK);
                }
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region PRODUCT
        [HttpPost, Route("pos/{userid?}/{locid?}/product/add")]
        //https://stackoverflow.com/questions/33711629/incorrect-content-type-exception-throws-angular-mvc-6-application
        public dynamic AddProduct(string userid, string locid)
        {
            dynamic body;
            bool isSucess = false;
            try
            {
                Request.Form.TryGetValue("description", out var Desc);
                Request.Form.TryGetValue("second_description", out var Desc2);
                Request.Form.TryGetValue("third_description", out var Desc3);
                Request.Form.TryGetValue("cost", out var Cost);
                Request.Form.TryGetValue("price", out var Price);
                Request.Form.TryGetValue("departmentList", out var department);
                Request.Form.TryGetValue("categoryList", out var category);
                Request.Form.TryGetValue("vendorList", out var vendor);
                Request.Form.TryGetValue("sectionList", out var section);
                Request.Form.TryGetValue("discountList", out var discount);
                Request.Form.TryGetValue("taxList", out var tax);
                Request.Form.TryGetValue("itemCodeList", out var itemcode);
                Request.Form.TryGetValue("upcList", out var upc);
                List<string> departmentList 
                                    = JsonSerializer.Deserialize<List<string>>(department);
                List<string> categoryList
                                    = JsonSerializer.Deserialize<List<string>>(category);
                List<string> vendorList
                                    = JsonSerializer.Deserialize<List<string>>(vendor);
                List<string> sectionList
                                    = JsonSerializer.Deserialize<List<string>>(section);
                List<string> discountList
                                    = JsonSerializer.Deserialize<List<string>>(discount);
                List<string> taxList
                                    = JsonSerializer.Deserialize<List<string>>(tax);
                List<string> itemCodeList
                                    = JsonSerializer.Deserialize<List<string>>(itemcode);
                List<string> upcList
                                    = JsonSerializer.Deserialize<List<string>>(upc);

                if (userid == null || userid.Length < 36 || locid == null || locid.Length < 36)
                {
                    body = "Something Wrong, Check Your Parameter";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
                else
                {

                    ProductModel model = new ProductModel(Desc, Desc2, Desc3, double.Parse(Cost), double.Parse(Price),
                        departmentList, categoryList, vendorList, sectionList, discountList, taxList, itemCodeList,
                        upcList, userid, locid) ;
                    body = JsonSerializer.Serialize(_ProductLogic.AddProduct(model));
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/product/update")]
        public dynamic UpdateProduct(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("uid", out var Uid);
                Request.Form.TryGetValue("description", out var Desc);
                Request.Form.TryGetValue("second_description", out var Desc2);
                Request.Form.TryGetValue("third_description", out var Desc3);
                Request.Form.TryGetValue("cost", out var Cost);
                Request.Form.TryGetValue("price", out var Price);
                Request.Form.TryGetValue("departmentList", out var department);
                Request.Form.TryGetValue("categoryList", out var category);
                Request.Form.TryGetValue("vendorList", out var vendor);
                Request.Form.TryGetValue("sectionList", out var section);
                Request.Form.TryGetValue("discountList", out var discount);
                Request.Form.TryGetValue("taxList", out var tax);
                Request.Form.TryGetValue("itemCodeList", out var itemcode);
                Request.Form.TryGetValue("upcList", out var upc);

                List<string> departmentList
                                    = JsonSerializer.Deserialize<List<string>>(department);
                List<string> categoryList
                                    = JsonSerializer.Deserialize<List<string>>(category);
                List<string> vendorList
                                    = JsonSerializer.Deserialize<List<string>>(vendor);
                List<string> sectionList
                                    = JsonSerializer.Deserialize<List<string>>(section);
                List<string> discountList
                                    = JsonSerializer.Deserialize<List<string>>(discount);
                List<string> taxList
                                    = JsonSerializer.Deserialize<List<string>>(tax);
                List<string> itemCodeList
                                    = JsonSerializer.Deserialize<List<string>>(itemcode);
                List<string> upcList
                                    = JsonSerializer.Deserialize<List<string>>(upc);


                if (userid == null || userid.Length < 36 || locid == null || locid.Length < 36)
                {
                    body = "Something Wrong, Check Your Parameter";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
                else
                {
                    ProductModel model = new ProductModel(Desc, Desc2, Desc3, double.Parse(Cost), double.Parse(Price),
                        departmentList, categoryList, vendorList, sectionList, discountList, taxList, itemCodeList, upcList, userid, locid, Uid);
                    body = JsonSerializer.Serialize(_ProductLogic.UpdateProduct(model));
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/product/get-by-id")]
        public dynamic GetProductByLocationId(string userid, string locid)
        {
            Debug.WriteLine("USER ID\t\t" + userid);
            dynamic body;
            try
            {
                if (userid == null || userid.Length < 36 || locid == null || locid.Length < 36)
                {
                    body = "Something Wrong, Check Your Parameter";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
                else
                {
                    body = JsonSerializer.Serialize(_ProductLogic.GetProductByLocation(userid, locid));
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }


        [HttpPost, Route("pos/{userid?}/{locid?}/{productId?}/product/itemcode")]
        public dynamic AddProductItemCode(string userid, string locid, string productId)
        {
            dynamic body;
            bool isSucess;
            Request.Form.TryGetValue("itemcode", out var ItemCode);
            try
            {
                isSucess = _ProductLogic.AddRelationItemCode(locid, productId, userid, ItemCode);
                if (isSucess)
                {
                    body = JsonSerializer.Serialize(isSucess);
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "Something Wrong, Check Your Parameter";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }

        }

        [HttpPost, Route("pos/{userid?}/{locid?}/product/get-by-map")]
        public dynamic GetProductByParamMap(string userid, string locid)
        {
            dynamic body;
            try
            {

                // To Do Add View Model
                // Pull Category By Product Id if not exist return emptry lst
                // Pull Department By PId ...
                // Pull Section By PId ...
                // Pull Vendor By PId ...

                Request.Form.TryGetValue("uid", out var Uid);
                Request.Form.TryGetValue("itemCode", out var Code);
                Request.Form.TryGetValue("upc", out var Upc);
                Request.Form.TryGetValue("searchText", out var SearchText);
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "uid", Uid },
                    { "itemCode", Code },
                    { "upc", Upc },
                    { "searchText", SearchText }
                };
                body = JsonSerializer.Serialize(_ProductLogic.GetProductById(userid, locid, dict));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/product/get-count")]
        public dynamic GetProductCountForPaginate(string userid, string locid)
        {
            dynamic body;
            try
            {

                // To Do Add View Model
                // Pull Category By Product Id if not exist return emptry lst
                // Pull Department By PId ...
                // Pull Section By PId ...
                // Pull Vendor By PId ...

                Request.Form.TryGetValue("searchType", out var searchType);
                Request.Form.TryGetValue("searchText", out var searchText);
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "locationId", locid},
                    { "searchType", searchType },
                    { "userId", userid },
                    { "searchText", searchText}
                };
                body = JsonSerializer.Serialize(_ProductLogic.GetProductPaginateCount(dict));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/product/get-product-paginate")]
        public dynamic GetProductForPaginate(string userid, string locid)
        {
            dynamic body;
            try
            {

                // To Do Add View Model
                // Pull Category By Product Id if not exist return emptry lst
                // Pull Department By PId ...
                // Pull Section By PId ...
                // Pull Vendor By PId ...

                Request.Form.TryGetValue("searchType", out var searchType);
                Request.Form.TryGetValue("startIdx", out var startIdx);
                Request.Form.TryGetValue("endIdx", out var endIdx);
                Request.Form.TryGetValue("searchText", out var searchText);
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "locationId", locid},
                    { "searchType", searchType },
                    { "userId", userid },
                    { "startIdx", startIdx },
                    { "endIdx", endIdx },
                    { "searchText", searchText}
                };
                body = JsonSerializer.Serialize(_ProductLogic.GetProductPaginate(dict));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region CATEGORY
        [HttpPost, Route("pos/{userid?}/{locid?}/category/add")]
        public dynamic AddCategory(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("desc", out var Desc);
                CategoryModel model = new CategoryModel()
                {
                    Description = Desc
                };
                if (_categoryLogic.AddCategory(model, userid, locid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO INSERT";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/category/update")]
        public dynamic UpdateCategory(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("id", out var Id);
                Request.Form.TryGetValue("desc", out var Desc);
                Request.Form.TryGetValue("note", out var Note);
                CategoryModel model = new CategoryModel()
                {
                    Description = Desc,
                    UId = Id,
                    SecondDescription = Note
                };
                if (_categoryLogic.UpdateCategory(model, userid, locid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO UPDATE CATEGORY";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/category/relation-add")]
        public dynamic AddCategoryProductRelation(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("categoryId", out var categoryId);
                Request.Form.TryGetValue("productId", out var productId);
                if (_categoryLogic.AddCategoryProductRelation(categoryId, productId, locid, userid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO INSERT";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/category/get")]
        public dynamic GetCategoryByLocationId(string userid, string locid)
        {
            dynamic body;
            try
            {
                var res = _categoryLogic.GetCategoryByLocationId(userid, locid);
                return HttpResponseHelper.HttpResponse(res, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/category/get-count")]
        public dynamic GetCategoryCountForPaginate(string userid, string locid)
        {
            dynamic body;
            try
            {

                // To Do Add View Model
                // Pull Category By Product Id if not exist return emptry lst
                // Pull Department By PId ...
                // Pull Section By PId ...
                // Pull Vendor By PId ...

                Request.Form.TryGetValue("searchType", out var searchType);
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "locationId", locid},
                    { "searchType", searchType },
                    { "userId", userid }
                };
                body = JsonSerializer.Serialize(_categoryLogic.GetCategoryPaginateCount(dict));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/category/get-category-paginate")]
        public dynamic GetCategoryForPaginate(string userid, string locid)
        {
            dynamic body;
            try
            {

                // To Do Add View Model
                // Pull Category By Product Id if not exist return emptry lst
                // Pull Department By PId ...
                // Pull Section By PId ...
                // Pull Vendor By PId ...

                Request.Form.TryGetValue("searchType", out var searchType);
                Request.Form.TryGetValue("startIdx", out var startIdx);
                Request.Form.TryGetValue("endIdx", out var endIdx);
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "locationId", locid},
                    { "searchType", searchType },
                    { "userId", userid },
                    { "startIdx", startIdx },
                    { "endIdx", endIdx }
                };
                body = JsonSerializer.Serialize(_categoryLogic.GetCategoryPaginate(dict));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/category/{categoryId?}")]
        public dynamic GetCategoryById(string userid, string locid, string categoryId)
        {
            dynamic body;
            try
            {
                body = JsonSerializer.Serialize(_categoryLogic.GetCategoryById(userid, locid, categoryId));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/category/get-by-description")]
        public dynamic GetCategoryByDescription(string userid, string locid)
        {
            dynamic body;
            Request.Form.TryGetValue("description", out var description);
            try
            {
                body = JsonSerializer.Serialize(_categoryLogic.GetCategoryByDescription(userid, locid, description));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }


        #endregion

        #region DEPARTMENT
        [HttpPost, Route("pos/{userid?}/{locid?}/department/add")]
        public dynamic AddDepartment(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("desc", out var Desc);
                Request.Form.TryGetValue("note", out var Note);
                DepartmentModel model = new DepartmentModel()
                {
                    Description = Desc,
                    SecondDescription = Note,
                };
                if (_departmentLogic.AddDepartment(model, userid, locid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO INSERT";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/department/update")]
        public dynamic UpdateDepartment(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("id", out var Id);
                Request.Form.TryGetValue("desc", out var Desc);
                Request.Form.TryGetValue("note", out var Note);
                DepartmentModel model = new DepartmentModel()
                {
                    UId = Id,
                    Description = Desc,
                    SecondDescription = Note
                };
                if (_departmentLogic.UpdateDepartment(model, userid, locid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO UPDATE DEPARTMENT";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/department/relation-add")]
        public dynamic AddDepartmentProductRelation(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("departmentId", out var departmentId);
                Request.Form.TryGetValue("productId", out var productId);

                if (_departmentLogic.AddDepartmentProductRelation(departmentId, productId, locid, userid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO INSERT";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/department/get")]
        public dynamic GetDepartmentByLocationId(string userid, string locid)
        {
            dynamic body;
            try
            {
                var res = _departmentLogic.GetDepartmentByLocationId(userid, locid);
                return HttpResponseHelper.HttpResponse(res, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/department/get-count")]
        public dynamic GetDepartmentCountForPaginate(string userid, string locid)
        {
            dynamic body;
            try
            {

                // To Do Add View Model
                // Pull Category By Product Id if not exist return emptry lst
                // Pull Department By PId ...
                // Pull Section By PId ...
                // Pull Vendor By PId ...

                Request.Form.TryGetValue("searchType", out var searchType);
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "locationId", locid},
                    { "searchType", searchType },
                    { "userId", userid }
                };
                body = JsonSerializer.Serialize(_departmentLogic.GetDepartmentPaginateCount(dict));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/department/get-department-paginate")]
        public dynamic GetDepartmentForPaginate(string userid, string locid)
        {
            dynamic body;
            try
            {

                // To Do Add View Model
                // Pull Category By Product Id if not exist return emptry lst
                // Pull Department By PId ...
                // Pull Section By PId ...
                // Pull Vendor By PId ...

                Request.Form.TryGetValue("searchType", out var searchType);
                Request.Form.TryGetValue("startIdx", out var startIdx);
                Request.Form.TryGetValue("endIdx", out var endIdx);
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "locationId", locid},
                    { "searchType", searchType },
                    { "userId", userid },
                    { "startIdx", startIdx },
                    { "endIdx", endIdx }
                };
                body = JsonSerializer.Serialize(_departmentLogic.GetDepartmentPaginate(dict));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/department/{departmentId?}")]
        public dynamic GetDepartmentById(string userid, string locid, string departmentId)
        {
            dynamic body;
            try
            {
                body = JsonSerializer.Serialize(_departmentLogic.GetDepartmentById(userid, locid, departmentId));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/department/get-by-description")]
        public dynamic GetDepartmentByDescription(string userid, string locid)
        {
            dynamic body;
            Request.Form.TryGetValue("description", out var description);
            try
            {
                body = JsonSerializer.Serialize(_departmentLogic.GetDepartmentByDescription(userid, locid, description));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region DISCOUNT
        [HttpPost, Route("pos/{userid?}/{locid?}/discount/add")]
        public dynamic AddDiscount(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("desc", out var Desc);
                Request.Form.TryGetValue("rate", out var Rate);
                DiscountModel model = new DiscountModel()
                {
                    Description = Desc,
                    Rate = double.Parse(Rate)
                };
                if (_discountLogic.AddDiscount(model, userid, locid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO INSERT";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/discount/update")]
        public dynamic UpdateDiscount(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("desc", out var Desc);
                Request.Form.TryGetValue("rate", out var Rate);
                Request.Form.TryGetValue("id", out var Id);
                Request.Form.TryGetValue("note", out var Note);
                DiscountModel model = new DiscountModel()
                {
                    Description = Desc,
                    Rate = double.Parse(Rate),
                    UId = Id,
                    SecondDescription = Note
                };
                if (_discountLogic.UpdateDiscount(model, userid, locid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO UPDATE DISCOUNT";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/discount/get")]
        public dynamic GetDiscountByLocationId(string userid, string locid)
        {
            dynamic body;
            try
            {
                var res = _discountLogic.GetDiscountByLocationId(userid, locid);
                return HttpResponseHelper.HttpResponse(res, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/discount/relation-add")]
        public dynamic AddDiscountProductRelation(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("discountId", out var discountId);
                Request.Form.TryGetValue("productId", out var productId);
                if (_discountLogic.AddDiscountProductRelation(productId, locid, discountId, userid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO INSERT";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/discount/get-count")]
        public dynamic GetDiscountCountForPaginate(string userid, string locid)
        {
            dynamic body;
            try
            {

                // To Do Add View Model
                // Pull Category By Product Id if not exist return emptry lst
                // Pull Department By PId ...
                // Pull Section By PId ...
                // Pull Vendor By PId ...

                Request.Form.TryGetValue("searchType", out var searchType);
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "locationId", locid},
                    { "searchType", searchType },
                    { "userId", userid }
                };
                body = JsonSerializer.Serialize(_discountLogic.GetDiscountPaginateCount(dict));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/discount/get-discount-paginate")]
        public dynamic GetDiscountForPaginate(string userid, string locid)
        {
            dynamic body;
            try
            {

                // To Do Add View Model
                // Pull Category By Product Id if not exist return emptry lst
                // Pull Department By PId ...
                // Pull Section By PId ...
                // Pull Vendor By PId ...

                Request.Form.TryGetValue("searchType", out var searchType);
                Request.Form.TryGetValue("startIdx", out var startIdx);
                Request.Form.TryGetValue("endIdx", out var endIdx);
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "locationId", locid},
                    { "searchType", searchType },
                    { "userId", userid },
                    { "startIdx", startIdx },
                    { "endIdx", endIdx }
                };
                body = JsonSerializer.Serialize(_discountLogic.GetDiscountPaginate(dict));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/discount/{discountId?}")]
        public dynamic GetDiscountById(string userid, string locid, string discountId)
        {
            dynamic body;
            try
            {
                body = JsonSerializer.Serialize(_discountLogic.GetDiscountById(userid, locid, discountId));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/discount/get-by-description")]
        public dynamic GetDiscountByDescription(string userid, string locid)
        {
            dynamic body;
            Request.Form.TryGetValue("description", out var description);
            try
            {
                body = JsonSerializer.Serialize(_discountLogic.GetDiscountByDescription(userid, locid, description));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region SECTION
        [HttpPost, Route("pos/{userid?}/{locid?}/section/add")]
        public dynamic AddSection(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("desc", out var Desc);
                SectionModel model = new SectionModel()
                {
                    Description = Desc
                };
                if (_sectionLogic.AddSection(model, userid, locid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO INSERT";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/section/add")]
        public dynamic UpdateSection(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("desc", out var Desc);
                Request.Form.TryGetValue("id", out var Id);
                Request.Form.TryGetValue("note", out var Note);
                SectionModel model = new SectionModel()
                {
                    Description = Desc,
                    UId = Id,
                    SecondDescription = Note
                };
                if (_sectionLogic.UpdateSection(model, userid, locid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO UPDATE SECTION";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/section/relation-add")]
        public dynamic AddSectionProductRelation(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("productId", out var productId);
                Request.Form.TryGetValue("sectionId", out var sectionId);

                if (_sectionLogic.AddSectionProductRelation(sectionId, productId, locid, userid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO INSERT";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/section/get")]
        public dynamic GetSectionByLocationId(string userid, string locid)
        {
            dynamic body;
            try
            {
                var res = _sectionLogic.GetSectionByLocationId(userid, locid);
                return HttpResponseHelper.HttpResponse(res, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/section/get-count")]
        public dynamic GetSectionCountForPaginate(string userid, string locid)
        {
            dynamic body;
            try
            {

                // To Do Add View Model
                // Pull Category By Product Id if not exist return emptry lst
                // Pull Department By PId ...
                // Pull Section By PId ...
                // Pull Vendor By PId ...

                Request.Form.TryGetValue("searchType", out var searchType);
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "locationId", locid},
                    { "searchType", searchType },
                    { "userId", userid }
                };
                body = JsonSerializer.Serialize(_sectionLogic.GetSectionPaginateCount(dict));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/section/get-section-paginate")]
        public dynamic GetSectionForPaginate(string userid, string locid)
        {
            dynamic body;
            try
            {

                // To Do Add View Model
                // Pull Category By Product Id if not exist return emptry lst
                // Pull Department By PId ...
                // Pull Section By PId ...
                // Pull Vendor By PId ...

                Request.Form.TryGetValue("searchType", out var searchType);
                Request.Form.TryGetValue("startIdx", out var startIdx);
                Request.Form.TryGetValue("endIdx", out var endIdx);
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "locationId", locid},
                    { "searchType", searchType },
                    { "userId", userid },
                    { "startIdx", startIdx },
                    { "endIdx", endIdx }
                };
                body = JsonSerializer.Serialize(_sectionLogic.GetSectionPaginate(dict));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/section/{sectionId?}")]
        public dynamic GetSectionById(string userid, string locid, string sectionId)
        {
            dynamic body;
            try
            {
                body = JsonSerializer.Serialize(_sectionLogic.GetSectionById(userid, locid, sectionId));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/section/get-by-section")]
        public dynamic GetSectionByDescription(string userid, string locid)
        {
            dynamic body;
            Request.Form.TryGetValue("description", out var description);
            try
            {
                body = JsonSerializer.Serialize(_sectionLogic.GetSectionByDescription(userid, locid, description));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        #endregion
       
        #region TAX
        [HttpPost, Route("pos/{userid?}/{locid?}/tax/add")]
        public dynamic AddTax(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("desc", out var Desc);
                Request.Form.TryGetValue("rate", out var Rate);
                TaxModel model = new TaxModel()
                {
                    Description = Desc,
                    Rate = double.Parse(Rate)
                };
                if (_taxLogic.AddTax(model, userid, locid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO INSERT";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/tax/update")]
        public dynamic UpdateTax(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("desc", out var Desc);
                Request.Form.TryGetValue("rate", out var Rate);
                Request.Form.TryGetValue("id", out var Id);
                Request.Form.TryGetValue("note", out var Note);
                TaxModel model = new TaxModel()
                {
                    Description = Desc,
                    Rate = double.Parse(Rate),
                    UId = Id,
                    SecondDescription = Note
                };
                if (_taxLogic.UpdateTax(model, userid, locid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO UPDATE TAX";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/tax/get")]
        public dynamic GetTaxByLocationId(string userid, string locid)
        {
            dynamic body;
            try
            {
                var res = _taxLogic.GetTaxByLocationId(userid, locid);
                return HttpResponseHelper.HttpResponse(res, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/tax/relation-add")]
        public dynamic AddTaxProductRelation(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("taxId", out var taxId);
                Request.Form.TryGetValue("productId", out var productId);
                if (_taxLogic.AddTaxProductRelation(productId, locid, taxId, userid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO INSERT";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/tax/get-count")]
        public dynamic GetTaxCountForPaginate(string userid, string locid)
        {
            dynamic body;
            try
            {

                // To Do Add View Model
                // Pull Category By Product Id if not exist return emptry lst
                // Pull Department By PId ...
                // Pull Section By PId ...
                // Pull Vendor By PId ...

                Request.Form.TryGetValue("searchType", out var searchType);
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "locationId", locid},
                    { "searchType", searchType },
                    { "userId", userid }
                };
                body = JsonSerializer.Serialize(_taxLogic.GetTaxPaginateCount(dict));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/tax/get-tax-paginate")]
        public dynamic GetTaxForPaginate(string userid, string locid)
        {
            dynamic body;
            try
            {

                // To Do Add View Model
                // Pull Category By Product Id if not exist return emptry lst
                // Pull Department By PId ...
                // Pull Section By PId ...
                // Pull Vendor By PId ...

                Request.Form.TryGetValue("searchType", out var searchType);
                Request.Form.TryGetValue("startIdx", out var startIdx);
                Request.Form.TryGetValue("endIdx", out var endIdx);
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "locationId", locid},
                    { "searchType", searchType },
                    { "userId", userid },
                    { "startIdx", startIdx },
                    { "endIdx", endIdx }
                };
                body = JsonSerializer.Serialize(_taxLogic.GetTaxPaginate(dict));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/tax/{taxId?}")]
        public dynamic GetTaxById(string userid, string locid, string taxId)
        {
            dynamic body;
            try
            {
                body = JsonSerializer.Serialize(_taxLogic.GetTaxById(userid, locid, taxId));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/tax/get-by-tax")]
        public dynamic GetTaxByDescription(string userid, string locid)
        {
            dynamic body;
            Request.Form.TryGetValue("description", out var description);
            try
            {
                body = JsonSerializer.Serialize(_taxLogic.GetTaxByDescription(userid, locid, description));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region VENDOR
        [HttpPost, Route("pos/{userid?}/{locid?}/vendor/add")]
        public dynamic AddVendor(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("desc", out var Desc);
                VendorModel model = new VendorModel()
                {
                    Description = Desc
                };
                if (_vendorLogic.AddVendor(model, userid, locid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO INSERT";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/vendor/add")]
        public dynamic UpdateVendor(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("desc", out var Desc);
                Request.Form.TryGetValue("id", out var Id);
                Request.Form.TryGetValue("note", out var Note);
                VendorModel model = new VendorModel()
                {
                    Description = Desc,
                    UId = Id,
                    SecondDescription = Note
                };
                if (_vendorLogic.UpdateVendor(model, userid, locid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO UPDATE VENDOR";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }


        [HttpPost, Route("pos/{userid?}/{locid?}/vendor/relation-add")]
        public dynamic AddVendorProductRelation(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("productId", out var productId);
                Request.Form.TryGetValue("vendorId", out var vendorId);

                if (_vendorLogic.AddVendorProductRelation(vendorId, productId, locid, userid))
                {
                    body = "OK";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                }
                else
                {
                    body = "INTERNAL ERROR FAILED TO INSERT";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }

            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/vendor/get")]
        public dynamic GetVendorByLocationId(string userid, string locid)
        {
            dynamic body;
            try
            {
                var res = _vendorLogic.GetVendorByLocationId(userid, locid);
                return HttpResponseHelper.HttpResponse(res, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/vendor/get-count")]
        public dynamic GetVendorCountForPaginate(string userid, string locid)
        {
            dynamic body;
            try
            {

                // To Do Add View Model
                // Pull Category By Product Id if not exist return emptry lst
                // Pull Department By PId ...
                // Pull Section By PId ...
                // Pull Vendor By PId ...

                Request.Form.TryGetValue("searchType", out var searchType);
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "locationId", locid},
                    { "searchType", searchType },
                    { "userId", userid }
                };
                body = JsonSerializer.Serialize(_vendorLogic.GetVendorPaginateCount(dict));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/vendor/get-vendor-paginate")]
        public dynamic GetVendorForPaginate(string userid, string locid)
        {
            dynamic body;
            try
            {

                // To Do Add View Model
                // Pull Category By Product Id if not exist return emptry lst
                // Pull Department By PId ...
                // Pull Section By PId ...
                // Pull Vendor By PId ...

                Request.Form.TryGetValue("searchType", out var searchType);
                Request.Form.TryGetValue("startIdx", out var startIdx);
                Request.Form.TryGetValue("endIdx", out var endIdx);
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "locationId", locid},
                    { "searchType", searchType },
                    { "userId", userid },
                    { "startIdx", startIdx },
                    { "endIdx", endIdx }
                };
                body = JsonSerializer.Serialize(_vendorLogic.GetVendorPaginate(dict));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/vendor/{vendorId?}")]
        public dynamic GetVendorById(string userid, string locid, string vendorId)
        {
            dynamic body;
            try
            {
                body = JsonSerializer.Serialize(_vendorLogic.GetVendorById(userid, locid, vendorId));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/vendor/get-by-vendor")]
        public dynamic GetVendorByDescription(string userid, string locid)
        {
            dynamic body;
            Request.Form.TryGetValue("description", out var description);
            try
            {
                body = JsonSerializer.Serialize(_vendorLogic.GetVendorByDescription(userid, locid, description));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region ITEMCODE

        [HttpGet, Route("pos/{userid?}/{locid?}/{productid?}/item-code/{itemCode?}/get")]
        public dynamic GetByItemCode(string userId, string locId, string productId, string itemCode)
        {
            dynamic body;
            try
            {
                body = JsonSerializer.Serialize(_itemCodeLogic.GetByItemCode(userId, productId, locId, itemCode));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }


        [HttpGet, Route("pos/{userid?}/{locid?}/{productid?}/item-code/get-with-paginate")]
        public dynamic GetProductItemCodeWithPagination(string userId, string locId, string productId)
        {
            dynamic body;
            Request.Query.TryGetValue("limit", out var limit);
            Request.Query.TryGetValue("offset", out var offset);
            Request.Query.TryGetValue("order", out var order);
            try {
                body = JsonSerializer.Serialize(_itemCodeLogic.GetItemCodePagination(userId, productId, locId, int.Parse(limit), int.Parse(offset), order));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/{productid?}/item-code/{itemCode?}/verify")]
        public dynamic VerifyItemCode(string userId, string locId, string productId, string itemCode)
        {
            dynamic body;
            try
            {
                body = JsonSerializer.Serialize(_itemCodeLogic.VerifyItemCode(userId, productId, locId, itemCode));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userId?}/{locId?}/{productId?}/item-code/add")]
        public dynamic AddItemCode(string userId, string locId, string productId)
        {
            dynamic body;
            Request.Form.TryGetValue("itemCode", out var itemCode);

            try
            {
                body = JsonSerializer.Serialize(_itemCodeLogic.AddItemCode(productId, locId, itemCode, userId));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/{productid?}/item-code/remove")]
        public dynamic RemoveItemCode(string userId, string locId, string productId)
        {
            dynamic body;
            Request.Form.TryGetValue("itemCode", out var itemCode);

            try
            {
                body = JsonSerializer.Serialize(_itemCodeLogic.RemoveItemCode(productId, locId, itemCode, userId));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region UPC
        [HttpGet, Route("pos/{userid?}/{locid?}/{productid?}/upc/{upc}/get")]
        public dynamic GetByUpc(string userId, string locId, string productId, string upc)
        {
            dynamic body;
            try
            {
                body = JsonSerializer.Serialize(_upcLogic.GetByUpc(userId, productId, locId, upc));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/{productid?}/upc/get-with-paginate")]
        public dynamic GetProductUpcWithPagination(string userId, string locId, string productId)
        {
            dynamic body;
            Request.Query.TryGetValue("limit", out var limit);
            Request.Query.TryGetValue("offset", out var offset);
            Request.Query.TryGetValue("order", out var order);
            try
            {
                body = JsonSerializer.Serialize(_upcLogic.GetUpcPagination(userId, productId, locId, int.Parse(limit), int.Parse(offset), order));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet, Route("pos/{userid?}/{locid?}/{productid?}/upc/{upc}/verify")]
        public dynamic VerifyUpc(string userId, string locId, string productId, string upc)
        {
            dynamic body;
            try
            {
                body = JsonSerializer.Serialize(_upcLogic.VerifyUpc(userId, productId, locId, upc));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/{productId?}/upc/add")]
        public dynamic AddUpc(string userId, string locId, string productId)
        {
            dynamic body;
            Request.Form.TryGetValue("upc", out var upc);

            try
            {
                body = JsonSerializer.Serialize(_upcLogic.AddUpc(productId, locId, upc, userId));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost, Route("pos/{userid?}/{locid?}/{productid?}/upc/remove")]
        public dynamic RemoveUpc(string userId, string locId, string productId)
        {
            dynamic body;
            Request.Form.TryGetValue("upc", out var upc);

            try
            {
                body = JsonSerializer.Serialize(_upcLogic.RemoveUpc(productId, locId, upc, userId));
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                body = "INTERNAL ERROR\t\t" + e.ToString();
                return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
            }
        }
        #endregion
    }
}
