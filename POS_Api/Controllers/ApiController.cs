using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POS_Api.Core.Implementation;
using POS_Api.Core.Interface;
using POS_Api.Database.MySql.Configuration;
using POS_Api.Model;
using POS_Api.Model.EnumData;
using POS_Api.Shared.HttpHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace POS_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly IUserLogic _UserLogic, _UserLogicParent;
        private readonly ILocationLogic _LocationLogic;
        private readonly ILocationUserRelationLogic _LocationUserReLogic;
        private readonly IProductLogic _ProductLogic;
        private readonly ILocationProductRelationLogic _LocationProductRelation;
        private readonly ICategoryLogic _categoryLogic;
        private readonly IDepartmentLogic _departmentLogic;
        private readonly IDiscountLogic _discountLogic;
        private readonly ISectionLogic _sectionLogic;
        private readonly ITaxLogic _taxLogic;
        private readonly IVendorLogic _vendorLogic;

        private readonly ILogger<ApiController> _logger;

        public ApiController(ILogger<ApiController> logger)
        {
            _logger = logger;
            _UserLogic = new UserLogic();
            _LocationLogic = new LocationLogic(_UserLogic);
            _ProductLogic = new ProductLogic(_UserLogic);
            _LocationUserReLogic = new LocationUserRelationLogic(_UserLogic);
            _UserLogicParent = new UserLogic(_LocationUserReLogic);
            _LocationProductRelation = new LocationProductRelationLogic();
            _categoryLogic = new CategoryLogic(_UserLogic);
            _departmentLogic = new DepartmentLogic(_UserLogic);
            _discountLogic = new DiscountLogic(_UserLogic, _ProductLogic, _LocationProductRelation);
            _sectionLogic = new SectionLogic(_UserLogic);
            _taxLogic = new TaxLogic(_UserLogic, _ProductLogic, _LocationProductRelation);
            _vendorLogic = new VendorLogic(_UserLogic);
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
            } catch(Exception e)
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
                } else
                {
                    body = "Something, Unable to Registered This User";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
            } catch (Exception e)
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

                if(muserid == null || muserid.Length < 5)
                {
                    isSuccess = _LocationUserReLogic.AddRelationLocationUser(null, userid, locaid, type.ToString().ToUpper());
                } else
                {
                    isSuccess = _LocationUserReLogic.AddRelationLocationUser(muserid, userid, locaid, type.ToString().ToUpper());
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
                } else
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
        [HttpPost , Route("pos/{userid?}/{locid?}/product/add")]
        public dynamic AddProduct(string userid, string locid)
        {
            Debug.WriteLine("USER ID\t\t" + userid);
            dynamic body;
            bool isSucess = false;
            try
            {
                Request.Form.TryGetValue("desc", out var Desc);
                Request.Form.TryGetValue("desc2", out var Desc2);
                Request.Form.TryGetValue("desc3", out var Desc3);
                Request.Form.TryGetValue("upc", out var Upc);
                Request.Form.TryGetValue("code", out var ItemCode);
                Request.Form.TryGetValue("cost", out var Cost);
                Request.Form.TryGetValue("price", out var Price);
 

                if (userid == null || userid.Length < 36 || locid == null || locid.Length < 36)
                {
                    body = "Something Wrong, Check Your Parameter";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
                else
                {
                    ProductModel model = new ProductModel(Desc, Desc2, Desc3, Upc, double.Parse(Cost), double.Parse(Price));
                    isSucess = _ProductLogic.AddProduct(model, userid, locid);
                    if(isSucess)
                    {
                        body = "OK";
                        return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                    } else
                    {
                        body = "INTERNAL ERROR FAILED TO INSERT";
                        return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                    }
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
            Debug.WriteLine("USER ID\t\t" + userid);
            dynamic body;
            try
            {
                Request.Form.TryGetValue("uid", out var Uid);
                Request.Form.TryGetValue("desc", out var Desc);
                Request.Form.TryGetValue("desc2", out var Desc2);
                Request.Form.TryGetValue("desc3", out var Desc3);
                Request.Form.TryGetValue("upc", out var Upc);
                Request.Form.TryGetValue("code", out var ItemCode);
                Request.Form.TryGetValue("cost", out var Cost);
                Request.Form.TryGetValue("price", out var Price);


                if (userid == null || userid.Length < 36 || locid == null || locid.Length < 36)
                {
                    body = "Something Wrong, Check Your Parameter";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
                else
                {
                    ProductModel model = new ProductModel(Uid, Desc, Desc2, Desc3, Upc, double.Parse(Cost), double.Parse(Price));
                    var res = _ProductLogic.UpdateProduct(model, userid, locid);
                    return HttpResponseHelper.HttpResponse(res, HttpStatusCode.OK);
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
        public dynamic AddProductItemCode(string userid, string locid, string productId) {
            dynamic body;
            bool isSucess;
            Request.Form.TryGetValue("itemcode", out var ItemCode);
            try
            {
                isSucess = _LocationProductRelation.AddRelationItemCode(locid, productId, userid, ItemCode);
                if(isSucess)
                {
                    body = JsonSerializer.Serialize(isSucess);
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.OK);
                } else
                {
                    body = "Something Wrong, Check Your Parameter";
                    return HttpResponseHelper.HttpResponse(body, HttpStatusCode.InternalServerError);
                }
            } catch (Exception e)
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
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    { "uid", Uid },
                    { "itemCode", Code },
                    { "upc", Upc }
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
                Request.Form.TryGetValue("desc", out var Desc);
                CategoryModel model = new CategoryModel()
                {
                    Description = Desc
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


        #endregion

        #region DEPARTMENT
        [HttpPost, Route("pos/{userid?}/{locid?}/department/add")]
        public dynamic AddDepartment(string userid, string locid)
        {
            dynamic body;
            try
            {
                Request.Form.TryGetValue("desc", out var Desc);
                DepartmentModel model = new DepartmentModel()
                {
                    Description = Desc
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
                Request.Form.TryGetValue("desc", out var Desc);
                DepartmentModel model = new DepartmentModel()
                {
                    Description = Desc
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
                DiscountModel model = new DiscountModel()
                {
                    Description = Desc,
                    Rate = double.Parse(Rate)
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
                SectionModel model = new SectionModel()
                {
                    Description = Desc
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
                TaxModel model = new TaxModel()
                {
                    Description = Desc,
                    Rate = double.Parse(Rate)
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
                VendorModel model = new VendorModel()
                {
                    Description = Desc
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
        #endregion
    }
}
