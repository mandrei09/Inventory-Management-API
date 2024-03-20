using AutoMapper;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Optima.Faia.Model;
using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Optima.Fais.Api
{
    public class MappingProfile : Profile
    {
        int? defaultId = null;
        public MappingProfile()
        {
            CreateMap<Model.ConfigValue, Dto.ConfigValueBase>();
            CreateMap<Model.ConfigValue, Dto.ConfigValue>();
            CreateMap<Dto.ConfigValue, Model.ConfigValue>();
            CreateMap<Model.InvState, Dto.InvState>();
            CreateMap<Dto.InvState, Model.InvState>();
            CreateMap<Model.AssetState, Dto.AssetState>();
            CreateMap<Dto.AssetState, Model.AssetState>();
            CreateMap<Model.Company, Dto.Company>();
            CreateMap<Dto.Company, Model.Company>();
            CreateMap<Model.Uom, Dto.Uom>();
            CreateMap<Dto.Uom, Model.Uom>();
            CreateMap<Model.PartnerLocation, Dto.PartnerLocation>();
            CreateMap<Dto.PartnerLocation, Model.PartnerLocation>();
            CreateMap<Model.AssetNature, Dto.AssetNature>()
                .ForMember(i => i.AssetType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AssetType.Id, Code = i.AssetType.Code, Name = i.AssetType.Name }));
            CreateMap<Dto.AssetNature, Model.AssetNature>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                .ForMember(i => i.AssetType, opt => opt.Ignore())
                .ForMember(i => i.AssetTypeId, opt => opt.MapFrom(i => i.AssetTypeId));
            CreateMap<Model.BudgetManager, Dto.BudgetManager>()
              .ForMember(i => i.Uom, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Uom.Id, Code = i.Uom.Code, Name = i.Uom.Name }));
            CreateMap<Dto.BudgetManager, Model.BudgetManager>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                .ForMember(i => i.Uom, opt => opt.Ignore())
                .ForMember(i => i.UomId, opt => opt.MapFrom(i => i.UomId));
            CreateMap<Model.Dimension, Dto.Dimension>()
                   .ForMember(i => i.AssetCategory, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AssetCategory.Id, Code = i.AssetCategory.Code, Name = i.AssetCategory.Name }));
                   //.ForMember(i => i.AssetType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AssetCategory.AssetType.Id, Code = i.AssetCategory.AssetType.Code, Name = i.AssetCategory.AssetType.Name }));
            CreateMap<Dto.Dimension, Model.Dimension>()
                   .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                   .ForMember(i => i.Length, opt => opt.MapFrom(i => i.Length))
                   .ForMember(i => i.Width, opt => opt.MapFrom(i => i.Width))
                   .ForMember(i => i.Height, opt => opt.MapFrom(i => i.Height))
                   .ForMember(i => i.AssetCategory, opt => opt.Ignore())
                   .ForMember(i => i.AssetCategoryId, opt => opt.MapFrom(i => i.AssetCategory.Id));
            CreateMap<Model.Account, Dto.Account>();
            CreateMap<Dto.Account, Model.Account>();
            CreateMap<Model.Material, Dto.Material>()
                  .ForMember(i => i.EAN, opt => opt.MapFrom(i => i.EAN))
                  .ForMember(i => i.PartNumber, opt => opt.MapFrom(i => i.PartNumber))
                  .ForMember(i => i.Value, opt => opt.MapFrom(i => i.Value))
                  .ForMember(i => i.Price, opt => opt.MapFrom(i => i.Price))
                  .ForMember(i => i.Quantity, opt => opt.MapFrom(i => i.Quantity))
                  .ForMember(i => i.EAN, opt => opt.MapFrom(i => i.EAN))
                  .ForMember(i => i.PartNumber, opt => opt.MapFrom(i => i.PartNumber))
                 .ForMember(i => i.Account, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Account.Id, Code = i.Account.Code, Name = i.Account.Name }))
                 .ForMember(i => i.ExpAccount, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.ExpAccount.Id, Code = i.ExpAccount.Code, Name = i.ExpAccount.Name }))
                 .ForMember(i => i.AssetCategory, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AssetCategory.Id, Code = i.AssetCategory.Code, Name = i.AssetCategory.Name }))
                 .ForMember(i => i.SubType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubType.Id, Code = i.SubType.Code, Name = i.SubType.Name }));
            CreateMap<Dto.Material, Model.Material>()
                 .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                 .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                 .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                  .ForMember(i => i.Value, opt => opt.MapFrom(i => i.Price * i.Quantity))
                  .ForMember(i => i.Price, opt => opt.MapFrom(i => i.Price))
                  .ForMember(i => i.Quantity, opt => opt.MapFrom(i => i.Quantity))
                  .ForMember(i => i.EAN, opt => opt.MapFrom(i => i.EAN))
                  .ForMember(i => i.PartNumber, opt => opt.MapFrom(i => i.PartNumber))
                 .ForMember(i => i.Account, opt => opt.Ignore())
                 .ForMember(i => i.AccountId, opt => opt.MapFrom(i => i.Account.Id))
                 .ForMember(i => i.ExpAccount, opt => opt.Ignore())
                 .ForMember(i => i.ExpAccountId, opt => opt.MapFrom(i => i.ExpAccount.Id))
                 .ForMember(i => i.AssetCategory, opt => opt.Ignore())
                 .ForMember(i => i.AssetCategoryId, opt => opt.MapFrom(i => i.AssetCategory.Id))
                 .ForMember(i => i.SubType, opt => opt.Ignore())
                 .ForMember(i => i.SubTypeId, opt => opt.MapFrom(i => i.SubType.Id));
            CreateMap<Model.Stock, Dto.Stock>()
                 .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                 .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                 .ForMember(i => i.Last_Incoming_Date, opt => opt.MapFrom(i => i.Last_Incoming_Date))
                 .ForMember(i => i.LongName, opt => opt.MapFrom(i => i.LongName))
                 .ForMember(i => i.Plant, opt => opt.MapFrom(i => i.Plant))
                 .ForMember(i => i.Storage_Location, opt => opt.MapFrom(i => i.Storage_Location))
                 .ForMember(i => i.UM, opt => opt.MapFrom(i => i.UM))
                 .ForMember(i => i.Quantity, opt => opt.MapFrom(i => i.Quantity))
                 .ForMember(i => i.Value, opt => opt.MapFrom(i => i.Value))
                 .ForMember(i => i.Validated, opt => opt.MapFrom(i => i.Validated))
                 .ForMember(i => i.Imported, opt => opt.MapFrom(i => i.Imported))
                 .ForMember(i => i.Error, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Error.Id, Code = i.Error.Code, Name = i.Error.Name }))
                 .ForMember(i => i.PlantInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.PlantInitial.Id, Code = i.PlantInitial.Code, Name = i.PlantInitial.Name }))
                 .ForMember(i => i.PlantActual, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.PlantActual.Id, Code = i.PlantActual.Code, Name = i.PlantActual.Name }))
                 .ForMember(i => i.Company, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Company.Id, Code = i.Company.Code, Name = i.Company.Name }))
                 .ForMember(i => i.Uom, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Uom.Id, Code = i.Uom.Code, Name = i.Uom.Name }))
                 .ForMember(i => i.Material, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Material.Id, Code = i.Material.Code, Name = i.Material.Name }))
                 .ForMember(i => i.Brand, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Brand.Id, Code = i.Brand.Code, Name = i.Brand.Name }))
                 .ForMember(i => i.Partner, opt => opt.MapFrom(i => new Dto.CodePartnerEntity { Id = i.Partner.Id, RegistryNumber = i.Partner.RegistryNumber, Name = i.Partner.Name }));

            CreateMap<Dto.Stock, Model.Stock>()
                 .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                 .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                 .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                  .ForMember(i => i.Last_Incoming_Date, opt => opt.MapFrom(i => i.Last_Incoming_Date))
                 .ForMember(i => i.LongName, opt => opt.MapFrom(i => i.LongName))
                 .ForMember(i => i.Plant, opt => opt.MapFrom(i => i.Plant))
                 .ForMember(i => i.Storage_Location, opt => opt.MapFrom(i => i.Storage_Location))
                 .ForMember(i => i.UM, opt => opt.MapFrom(i => i.UM))
                 .ForMember(i => i.Quantity, opt => opt.MapFrom(i => i.Quantity))
                 .ForMember(i => i.Value, opt => opt.MapFrom(i => i.Value))
                 .ForMember(i => i.Company, opt => opt.Ignore())
                 .ForMember(i => i.CompanyId, opt => opt.MapFrom(i => i.Company.Id))
                 .ForMember(i => i.Uom, opt => opt.Ignore())
                 .ForMember(i => i.UomId, opt => opt.MapFrom(i => i.Uom.Id))
                 .ForMember(i => i.Material, opt => opt.Ignore())
                 .ForMember(i => i.MaterialId, opt => opt.MapFrom(i => i.Material.Id))
                 .ForMember(i => i.Brand, opt => opt.Ignore())
                 .ForMember(i => i.BrandId, opt => opt.MapFrom(i => i.Brand.Id))
                 .ForMember(i => i.Partner, opt => opt.Ignore())
                 .ForMember(i => i.PartnerId, opt => opt.MapFrom(i => i.Partner.Id));
            CreateMap<Model.ExpAccount, Dto.ExpAccount>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                .ForMember(i => i.Description, opt => opt.MapFrom(i => i.Description));
            CreateMap<Dto.ExpAccount, Model.ExpAccount>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                .ForMember(i => i.Description, opt => opt.MapFrom(i => i.Description));
            CreateMap<Model.Article, Dto.Article>();
            CreateMap<Dto.Article, Model.Article>();
            CreateMap<Model.Department, Dto.Department>();
            CreateMap<Dto.Department, Model.Department>();
            CreateMap<Dto.AssetNiInvDet, Model.AssetNi>();
            CreateMap<Model.AssetNi, Dto.AssetNiInvDet>()
                .ForMember(a => a.AllowLabel, opt => opt.MapFrom(a => a.AllowLabel))
                .ForMember(a => a.RoomCode, opt => opt.MapFrom(a => a.Room.Code))
                .ForMember(a => a.RoomName, opt => opt.MapFrom(a => a.Room.Name))
                .ForMember(a => a.LocationCode, opt => opt.MapFrom(a => a.Room.Location.Code))
                .ForMember(a => a.LocationName, opt => opt.MapFrom(a => a.Room.Location.Name))
                .ForMember(a => a.RegionCode, opt => opt.MapFrom(a => a.Room.Location.Region.Code))
                .ForMember(a => a.RegionName, opt => opt.MapFrom(a => a.Room.Location.Region.Name))
                .ForMember(a => a.RoomId, opt => opt.MapFrom(a => a.Room.Id));

            //CreateMap<Model.AssetInventoryDetail, Dto.Asset>()
            //         .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Asset.Id))
            //         .ForMember(a => a.InvNo, opt => opt.MapFrom(a => a.Asset.InvNo))
            //         .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Asset.Name))
            //         .ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Asset.Quantity))
            //         .ForMember(a => a.PurchaseDate, opt => opt.MapFrom(a => a.Asset.PurchaseDate))
            //         .ForMember(a => a.CreatedAt, opt => opt.MapFrom(a => a.Asset.CreatedAt))
            //         .ForMember(a => a.ERPCode, opt => opt.MapFrom(a => a.Asset.ERPCode))
            //         //.ForMember(a => a.SerialNumberInitial, opt => opt.MapFrom(a => a.Asset.SerialNumber))
            //         //.ForMember(a => a.SerialNumberFinal, opt => opt.MapFrom(a => a.Inventory.SerialNumber))
            //         .ForMember(i => i.Custody, opt => opt.MapFrom(i => i.Asset.Custody))
            //         .ForMember(a => a.Document, opt => opt.MapFrom(
            //             a => new Dto.DocumentMainDetail
            //             {
            //                 Id = a.Asset.Document.Id,
            //                 DocNo1 = a.Asset.Document.DocNo1,
            //                 DocNo2 = a.Asset.Document.DocNo2,
            //                 DocumentDate = a.Asset.Document.DocumentDate,
            //                 RegisterDate = a.Asset.Document.RegisterDate,

            //                 DocumentType = new Dto.CodeNameEntity
            //                 {
            //                     Id = a.Asset.Document.DocumentTypeId,
            //                     Code = a.Asset.Document.DocumentType.Code,
            //                     Name = a.Asset.Document.DocumentType.Name
            //                 },
            //                 Partner = new Dto.PartnerBase
            //                 {
            //                     Id = a.Asset.Document.Partner.Id,
            //                     Name = a.Asset.Document.Partner.Name,
            //                     FiscalCode = a.Asset.Document.Partner.FiscalCode,
            //                     RegistryNumber = a.Asset.Document.Partner.RegistryNumber,
            //                     ErpCode = a.Asset.Document.Partner.ErpCode
            //                 }
            //             }))
            //         .ForMember(assetDto => assetDto.Uom, opt => opt.MapFrom(p => (p.Asset.Uom != null) ? new Dto.CodeNameEntity { Id = p.Asset.Uom.Id, Code = p.Asset.Uom.Code, Name = p.Asset.Uom.Name } : null))
            //         .ForMember(assetDto => assetDto.Type, opt => opt.MapFrom(p => (p.Asset.SubType != null) ? new Dto.CodeNameEntity { Id = p.Asset.SubType.Id, Code = p.Asset.SubType.Code, Name = p.Asset.SubType.Name } : null))
            //         .ForMember(assetDto => assetDto.MasterType, opt => opt.MapFrom(p => (p.Asset.SubType != null) ? new Dto.CodeNameEntity { Id = p.Asset.SubType.Type.MasterType.Id, Code = p.Asset.SubType.Type.MasterType.Code, Name = p.Asset.SubType.Type.MasterType.Name } : null))
            //         .ForMember(a => a.Inventory, opt => opt.MapFrom(a => new Dto.AssetInventoryDetail
            //         {
            //             ModifiedBy = ((a.Inventory.ModifiedByUser == null) || (a.Inventory.ModifiedByUser.Employee == null)) ? null : new Dto.EmployeeResource()
            //             {
            //                 Id = a.Inventory.ModifiedByUser.Employee.Id,
            //                 FirstName = a.Inventory.ModifiedByUser.Employee.FirstName,
            //                 LastName = a.Inventory.ModifiedByUser.Employee.LastName,
            //                 InternalCode = a.Inventory.ModifiedByUser.Employee.InternalCode
            //             },
            //             ModifiedAt = a.Inventory.ModifiedAt,
            //             //ScanDate = a.Inventory.ScanDate,
            //             Initial = new Dto.AssetInventoryBaseDetail()
            //             {
            //                 Inv = new Dto.AssetInvDetail()
            //                 {
            //                     SerialNumber = a.Inventory.SerialNumber,
            //                     Producer = a.Inventory.Producer,
            //                     Model = a.Inventory.Model,
            //                     Info = a.Inventory.Info,
            //                     Quantity = a.Inventory.QInitial,
            //                     State = (a.Inventory.StateInitial != null) ? new Dto.CodeNameEntity() { Id = a.Inventory.StateInitial.Id, Code = a.Inventory.StateInitial.Code, Name = a.Inventory.StateInitial.Name } : null
            //                 },
            //                 Adm = new Dto.AssetAdmBaseViewResource()
            //                 {
            //                     Room = (a.Inventory.RoomInitial != null ?
            //                     new Dto.RoomViewResource
            //                     {
            //                         Id = a.Inventory.RoomInitial.Id,
            //                         Code = a.Inventory.RoomInitial.Code,
            //                         Name = a.Inventory.RoomInitial.Name,
            //                         IsDeleted = a.Inventory.RoomInitial.IsDeleted,
            //                         Location = a.Inventory.RoomInitial.Location != null
            //                              ? new Dto.LocationViewResource()
            //                              {
            //                                  Id = a.Inventory.RoomInitial.Location.Id,
            //                                  Code = a.Inventory.RoomInitial.Location.Code,
            //                                  Name = a.Inventory.RoomInitial.Location.Name,
            //                                  IsDeleted = a.Inventory.RoomInitial.Location.IsDeleted,
            //                                  AdmCenter = (a.Inventory.RoomInitial.Location.AdmCenter != null
            //                                      ? new Dto.AdmCenterViewResource()
            //                                      {
            //                                          Id = a.Inventory.RoomInitial.Location.AdmCenter.Id,
            //                                          Code = a.Inventory.RoomInitial.Location.AdmCenter.Code,
            //                                          Name = a.Inventory.RoomInitial.Location.AdmCenter.Name,
            //                                      }
            //                                      : null),
            //                                  Region = (a.Inventory.RoomInitial.Location.Region != null
            //                                      ? new Dto.RegionViewResource()
            //                                      {
            //                                          Id = a.Inventory.RoomInitial.Location.Region.Id,
            //                                          Code = a.Inventory.RoomInitial.Location.Region.Code,
            //                                          Name = a.Inventory.RoomInitial.Location.Region.Name,
            //                                      }
            //                                      : null),
            //                              }
            //                              : null

            //                     }
            //                     : null),
            //                     CostCenter = (a.Inventory.CostCenterInitial != null ?
            //                     new Dto.CostCenterViewResource
            //                     {
            //                         Id = a.Inventory.CostCenterInitial.Id,
            //                         Code = a.Inventory.CostCenterInitial.Code,
            //                         Name = a.Inventory.CostCenterInitial.Name,
            //                         AdmCenter = a.Inventory.CostCenterInitial.AdmCenter != null
            //                              ? new Dto.AdmCenterViewResource()
            //                              {
            //                                  Id = a.Inventory.CostCenterInitial.AdmCenter.Id,
            //                                  Code = a.Inventory.CostCenterInitial.AdmCenter.Code,
            //                                  Name = a.Inventory.CostCenterInitial.AdmCenter.Name,
            //                              }
            //                              : null
            //                     }
            //                     : null),
            //                     Employee = (a.Inventory.EmployeeInitial != null ?
            //                     new Dto.EmployeeViewResource
            //                     {
            //                         Id = a.Inventory.EmployeeInitial.Id,
            //                         InternalCode = a.Inventory.EmployeeInitial.InternalCode,
            //                         FirstName = a.Inventory.EmployeeInitial.FirstName,
            //                         LastName = a.Inventory.EmployeeInitial.LastName,
            //                         IsDeleted = a.Inventory.EmployeeInitial.IsDeleted,
            //                         Department = a.Inventory.EmployeeInitial.Department != null
            //                              ? new Dto.DepartmentViewResource()
            //                              {
            //                                  Id = a.Inventory.EmployeeInitial.Department.Id,
            //                                  Code = a.Inventory.EmployeeInitial.Department.Code,
            //                                  Name = a.Inventory.EmployeeInitial.Department.Name,
            //                              }
            //                              : null
            //                     }
            //                     : null)
            //                 }
            //             },
            //             Final = new Dto.AssetInventoryBaseDetail()
            //             {
            //                 Inv = new Dto.AssetInvDetail()
            //                 {
            //                     SerialNumber = a.Inventory.SerialNumber,
            //                     Producer = a.Inventory.Producer,
            //                     Model = a.Inventory.Model,
            //                     Info = a.Inventory.Info,
            //                     Info2019 = a.Inventory.Info2019,
            //                     Quantity = a.Inventory.QFinal,
            //                     State = (a.Inventory.StateFinal != null) ? new Dto.CodeNameEntity() { Id = a.Inventory.StateFinal.Id, Code = a.Inventory.StateFinal.Code, Name = a.Inventory.StateFinal.Name } : null
            //                 },
            //                 Adm = new Dto.AssetAdmBaseViewResource()
            //                 {
            //                     Room = (a.Inventory.RoomFinal != null ?
            //                     new Dto.RoomViewResource
            //                     {
            //                         Id = a.Inventory.RoomFinal.Id,
            //                         Code = a.Inventory.RoomFinal.Code,
            //                         Name = a.Inventory.RoomFinal.Name,
            //                         IsDeleted = a.Inventory.RoomFinal.IsDeleted,
            //                         Location = a.Inventory.RoomFinal.Location != null
            //                              ? new Dto.LocationViewResource()
            //                              {
            //                                  Id = a.Inventory.RoomFinal.Location.Id,
            //                                  Code = a.Inventory.RoomFinal.Location.Code,
            //                                  Name = a.Inventory.RoomFinal.Location.Name,
            //                                  IsDeleted = a.Inventory.RoomFinal.Location.IsDeleted,
            //                                  AdmCenter = (a.Inventory.RoomFinal.Location.AdmCenter != null
            //                                      ? new Dto.AdmCenterViewResource()
            //                                      {
            //                                          Id = a.Inventory.RoomFinal.Location.AdmCenter.Id,
            //                                          Code = a.Inventory.RoomFinal.Location.AdmCenter.Code,
            //                                          Name = a.Inventory.RoomFinal.Location.AdmCenter.Name,
            //                                      }
            //                                      : null),
            //                                  Region = (a.Inventory.RoomFinal.Location.Region != null
            //                                      ? new Dto.RegionViewResource()
            //                                      {
            //                                          Id = a.Inventory.RoomFinal.Location.Region.Id,
            //                                          Code = a.Inventory.RoomFinal.Location.Region.Code,
            //                                          Name = a.Inventory.RoomFinal.Location.Region.Name,
            //                                      }
            //                                      : null),
            //                              }
            //                              : null

            //                     }
            //                     : null),
            //                     CostCenter = (a.Inventory.CostCenterFinal != null ?
            //                     new Dto.CostCenterViewResource
            //                     {
            //                         Id = a.Inventory.CostCenterFinal.Id,
            //                         Code = a.Inventory.CostCenterFinal.Code,
            //                         Name = a.Inventory.CostCenterFinal.Name,
            //                         AdmCenter = a.Inventory.CostCenterFinal.AdmCenter != null
            //                              ? new Dto.AdmCenterViewResource()
            //                              {
            //                                  Id = a.Inventory.CostCenterFinal.AdmCenter.Id,
            //                                  Code = a.Inventory.CostCenterFinal.AdmCenter.Code,
            //                                  Name = a.Inventory.CostCenterFinal.AdmCenter.Name,
            //                              }
            //                              : null
            //                     }
            //                     : null),
            //                     Employee = (a.Inventory.EmployeeFinal != null ?
            //                     new Dto.EmployeeViewResource
            //                     {
            //                         Id = a.Inventory.EmployeeFinal.Id,
            //                         InternalCode = a.Inventory.EmployeeFinal.InternalCode,
            //                         FirstName = a.Inventory.EmployeeFinal.FirstName,
            //                         LastName = a.Inventory.EmployeeFinal.LastName,
            //                         Department = a.Inventory.EmployeeFinal.Department != null
            //                              ? new Dto.DepartmentViewResource()
            //                              {
            //                                  Id = a.Inventory.EmployeeFinal.Department.Id,
            //                                  Code = a.Inventory.EmployeeFinal.Department.Code,
            //                                  Name = a.Inventory.EmployeeFinal.Department.Name,
            //                              }
            //                              : null
            //                     }
            //                     : null)
            //                 }
            //             }
            //         }))
            //         .ForMember(a => a.Dep, opt => opt.MapFrom(a => new Dto.AssetDepDetail
            //         {
            //             ValueInv = a.Dep.ValueInv,
            //             ValueRem = a.Dep.ValueRem,
            //             ValueDepPU = a.Dep.ValueDepPU,
            //             DepPeriodMonth = a.Dep.DepPeriodMonth,
            //             ValueDep = a.Dep.ValueDep,
            //             ValueDepYTD = a.Dep.ValueDepYTD,
            //             DepPeriod = a.Dep.DepPeriod,
            //             DepPeriodRem = a.Dep.DepPeriodRem
            //         }
            //         ))
            //         ;
            //CreateMap<Model.AssetDetail, Dto.Asset>()
            //    .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Asset.Id))
            //    .ForMember(a => a.InvNo, opt => opt.MapFrom(a => a.Asset.InvNo))
            //    .ForMember(a => a.ERPCode, opt => opt.MapFrom(a => a.Asset.ERPCode))
            //    .ForMember(a => a.SerialNumber, opt => opt.MapFrom(a => a.Asset.SerialNumber))
            //    .ForMember(a => a.ValueInv, opt => opt.MapFrom(a => a.Asset.ValueInv))
            //    .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Asset.Name))
            //    .ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Asset.Quantity))
            //    .ForMember(a => a.PurchaseDate, opt => opt.MapFrom(a => a.Asset.PurchaseDate))
            //    .ForMember(a => a.CreatedAt, opt => opt.MapFrom(a => a.Asset.CreatedAt))
            //    .ForMember(a => a.Document, opt => opt.MapFrom(
            //        a => new Dto.DocumentMainDetail
            //        {
            //            Id = a.Asset.Document.Id,
            //            DocNo1 = a.Asset.Document.DocNo1,
            //            DocNo2 = a.Asset.Document.DocNo2,
            //            DocumentDate = a.Asset.Document.DocumentDate,
            //            RegisterDate = a.Asset.Document.RegisterDate,

            //            DocumentType = new Dto.CodeNameEntity
            //            {
            //                Id = a.Asset.Document.DocumentTypeId,
            //                Code = a.Asset.Document.DocumentType.Code,
            //                Name = a.Asset.Document.DocumentType.Name
            //            },
            //            Partner = new Dto.PartnerBase
            //            {
            //                Id = a.Asset.Document.Partner.Id,
            //                Name = a.Asset.Document.Partner.Name,
            //                FiscalCode = a.Asset.Document.Partner.FiscalCode,
            //                RegistryNumber = a.Asset.Document.Partner.RegistryNumber,
            //                ErpCode = a.Asset.Document.Partner.ErpCode
            //            }
            //        }))
            //    .ForMember(assetDto => assetDto.Uom, opt => opt.MapFrom(p => (p.Asset.Uom != null) ? new Dto.CodeNameEntity { Id = p.Asset.Uom.Id, Code = p.Asset.Uom.Code, Name = p.Asset.Uom.Name } : null))
            //    .ForMember(a => a.Adm, opt => opt.MapFrom(a => a.AdmMD != null ? new Dto.AssetAdmDetail
            //    {
            //        Location = ((a.AdmMD.Room != null && a.AdmMD.Room.Location != null) ? new Dto.CodeNameEntity { Id = a.AdmMD.Room.LocationId, Code = a.AdmMD.Room.Location.Code, Name = a.AdmMD.Room.Location.Name } : null),
            //        Room = (a.AdmMD.Room != null ? new Dto.CodeNameEntity { Id = a.AdmMD.Room.Id, Code = a.AdmMD.Room.Code, Name = a.AdmMD.Room.Name } : null),
            //        AssetCategory = (a.AdmMD.AssetCategory != null ? new Dto.CodeNameEntity { Id = a.AdmMD.AssetCategory.Id, Code = a.AdmMD.AssetCategory.Code, Name = a.AdmMD.AssetCategory.Name } : null),
            //        AssetType = (a.AdmMD.AssetType != null ? new Dto.CodeNameEntity { Id = a.AdmMD.AssetType.Id, Code = a.AdmMD.AssetType.Code, Name = a.AdmMD.AssetType.Name } : null),
            //        AssetState = (a.AdmMD.AssetState != null ? new Dto.CodeNameEntity { Id = a.AdmMD.AssetState.Id, Code = a.AdmMD.AssetState.Code, Name = a.AdmMD.AssetState.Name } : null),
            //        CostCenter = (a.AdmMD.CostCenter != null ? new Dto.CodeNameEntity { Id = a.AdmMD.CostCenter.Id, Code = a.AdmMD.CostCenter.Code, Name = a.AdmMD.CostCenter.Name } : null),
            //        Employee = (a.AdmMD.Employee != null ? new Dto.EmployeeResource { Id = a.AdmMD.Employee.Id, InternalCode = a.AdmMD.Employee.InternalCode, FirstName = a.AdmMD.Employee.FirstName, LastName = a.AdmMD.Employee.LastName, ErpCode = a.AdmMD.Employee.ERPCode } : null)
            //    }
            //    :
            //    new Dto.AssetAdmDetail
            //    {
            //        Location = ((a.Asset.Room != null && a.Asset.Room.Location != null) ? new Dto.CodeNameEntity { Id = a.Asset.Room.LocationId, Code = a.Asset.Room.Location.Code, Name = a.Asset.Room.Location.Name } : null),
            //        Room = (a.Asset.Room != null ? new Dto.CodeNameEntity { Id = a.Asset.Room.Id, Code = a.Asset.Room.Code, Name = a.Asset.Room.Name } : null),
            //        AssetCategory = (a.Asset.AssetCategory != null ? new Dto.CodeNameEntity { Id = a.Asset.AssetCategory.Id, Code = a.Asset.AssetCategory.Code, Name = a.Asset.AssetCategory.Name } : null),
            //        AssetType = (a.Asset.AssetType != null ? new Dto.CodeNameEntity { Id = a.Asset.AssetType.Id, Code = a.Asset.AssetType.Code, Name = a.Asset.AssetType.Name } : null),
            //        Administration = (a.Asset.Administration != null ? new Dto.CodeNameEntity { Id = a.Asset.Administration.Id, Code = a.Asset.Administration.Code, Name = a.Asset.Administration.Name } : null),
            //        AssetState = (a.Asset.AssetState != null ? new Dto.CodeNameEntity { Id = a.Asset.AssetState.Id, Code = a.Asset.AssetState.Code, Name = a.Asset.AssetState.Name } : null),
            //        CostCenter = (a.Asset.CostCenter != null ? new Dto.CodeNameEntity { Id = a.Asset.CostCenter.Id, Code = a.Asset.CostCenter.Code, Name = a.Asset.CostCenter.Name } : null),
            //        Employee = (a.Asset.Employee != null ? new Dto.EmployeeResource { Id = a.Asset.Employee.Id, InternalCode = a.Asset.Employee.InternalCode, FirstName = a.Asset.Employee.FirstName, LastName = a.Asset.Employee.LastName } : null)
            //    }))
            //    .ForMember(a => a.Dep, opt => opt.MapFrom(a => a.DepMD != null ?
            //    new Dto.AssetDepDetail
            //    {
            //        ValueInv = a.DepMD.ValueInv,
            //        ValueRem = a.DepMD.ValueRem,
            //        ValueDepPU = a.DepMD.ValueDepPU,
            //        DepPeriodMonth = a.DepMD.DepPeriodMonth,
            //        ValueDep = a.DepMD.ValueDep,
            //        ValueDepYTD = a.DepMD.ValueDepYTD,
            //        DepPeriod = a.DepMD.DepPeriod,
            //        DepPeriodRem = a.DepMD.DepPeriodRem
            //    }
            //    :
            //    new Dto.AssetDepDetail
            //    {
            //        ValueInv = a.Dep.ValueInv,
            //        ValueRem = a.Dep.ValueRem,
            //        ValueDepPU = a.Dep.ValueDepPU,
            //        DepPeriodMonth = a.Dep.DepPeriodMonth,
            //        ValueDep = a.Dep.ValueDep,
            //        ValueDepYTD = a.Dep.ValueDepYTD,
            //        DepPeriod = a.Dep.DepPeriod,
            //        DepPeriodRem = a.Dep.DepPeriodRem
            //    }
            //    ))
            //    ;

            CreateMap<Model.AssetDetail, Dto.Asset>()
               .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Asset.Id))
               .ForMember(a => a.InvNo, opt => opt.MapFrom(a => a.Asset.InvNo))
               .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Asset.Name))
               .ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Asset.Quantity))
               .ForMember(a => a.PurchaseDate, opt => opt.MapFrom(a => a.Asset.PurchaseDate))
               .ForMember(a => a.CreatedAt, opt => opt.MapFrom(a => a.Asset.CreatedAt))
               .ForMember(a => a.ERPCode, opt => opt.MapFrom(a => a.Asset.ERPCode))
               .ForMember(a => a.SerialNumber, opt => opt.MapFrom(a => a.Asset.SerialNumber))
               .ForMember(a => a.InSapValidation, opt => opt.MapFrom(a => a.Asset.InSapValidation))

               .ForMember(a => a.Document, opt => opt.MapFrom(
                   a => new Dto.DocumentMainDetail
                   {
                       Id = a.Asset.Document.Id,
                       DocNo1 = a.Asset.Document.DocNo1,
                       DocNo2 = a.Asset.Document.DocNo2,
                       DocumentDate = a.Asset.Document.DocumentDate,
                       RegisterDate = a.Asset.Document.RegisterDate,

                       DocumentType = new Dto.CodeNameEntity
                       {
                           Id = a.Asset.Document.DocumentTypeId,
                           Code = a.Asset.Document.DocumentType.Code,
                           Name = a.Asset.Document.DocumentType.Name
                       },
                       Partner = new Dto.Partner
                       {
                           Id = a.Asset.Document.Partner.Id,
                           Name = a.Asset.Document.Partner.Name,
                           FiscalCode = a.Asset.Document.Partner.FiscalCode,
                           RegistryNumber = a.Asset.Document.Partner.RegistryNumber,
                           ErpCode = a.Asset.Document.Partner.ErpCode,
                           PartnerLocation = new Dto.PartnerLocation()
                           {
                               Cui = a.Asset.Document.Partner.PartnerLocation.Cui,
                               Data = a.Asset.Document.Partner.PartnerLocation.Data,
                               Denumire = a.Asset.Document.Partner.PartnerLocation.Denumire,
                               Adresa = a.Asset.Document.Partner.PartnerLocation.Adresa,
                               NrRegCom = a.Asset.Document.Partner.PartnerLocation.NrRegCom,
                               Telefon = a.Asset.Document.Partner.PartnerLocation.Telefon,
                               Fax = a.Asset.Document.Partner.PartnerLocation.Fax,
                               CodPostal = a.Asset.Document.Partner.PartnerLocation.CodPostal,
                               Act = a.Asset.Document.Partner.PartnerLocation.Act,
                               Stare_inregistrare = a.Asset.Document.Partner.PartnerLocation.Stare_inregistrare,
                               ScpTVA = a.Asset.Document.Partner.PartnerLocation.ScpTVA,
                               Data_inceput_ScpTVA = a.Asset.Document.Partner.PartnerLocation.Data_inceput_ScpTVA,
                               Data_sfarsit_ScpTVA = a.Asset.Document.Partner.PartnerLocation.Data_sfarsit_ScpTVA,
                               Data_anul_imp_ScpTVA = a.Asset.Document.Partner.PartnerLocation.Data_anul_imp_ScpTVA,
                               Mesaj_ScpTVA = a.Asset.Document.Partner.PartnerLocation.Mesaj_ScpTVA,
                               DataInceputTvaInc = a.Asset.Document.Partner.PartnerLocation.DataInceputTvaInc,
                               DataSfarsitTvaInc = a.Asset.Document.Partner.PartnerLocation.DataSfarsitTvaInc,
                               DataActualizareTvaInc = a.Asset.Document.Partner.PartnerLocation.DataActualizareTvaInc,
                               DataPublicareTvaInc = a.Asset.Document.Partner.PartnerLocation.DataPublicareTvaInc,
                               TipActTvaInc = a.Asset.Document.Partner.PartnerLocation.TipActTvaInc,
                               StatusTvaIncasare = a.Asset.Document.Partner.PartnerLocation.StatusTvaIncasare,
                               DataInactivare = a.Asset.Document.Partner.PartnerLocation.DataInactivare,
                               DataReactivare = a.Asset.Document.Partner.PartnerLocation.DataReactivare,
                               DataPublicare = a.Asset.Document.Partner.PartnerLocation.DataPublicare,
                               DataRadiere = a.Asset.Document.Partner.PartnerLocation.DataRadiere,
                               StatusInactivi = a.Asset.Document.Partner.PartnerLocation.StatusInactivi,
                               DataInceputSplitTVA = a.Asset.Document.Partner.PartnerLocation.DataInceputSplitTVA,
                               DataAnulareSplitTVA = a.Asset.Document.Partner.PartnerLocation.DataAnulareSplitTVA,
                               Iban = a.Asset.Document.Partner.PartnerLocation.Iban,
                               StatusRO_e_Factura = a.Asset.Document.Partner.PartnerLocation.StatusRO_e_Factura
                           }
                       }
                   }))
               .ForMember(assetDto => assetDto.Uom, opt => opt.MapFrom(p => (p.Asset.Uom != null) ? new Dto.CodeNameEntity { Id = p.Asset.Uom.Id, Code = p.Asset.Uom.Code, Name = p.Asset.Uom.Name } : null))
               .ForMember(a => a.Adm, opt => opt.MapFrom(a => new Dto.AssetAdmDetail
               {
                   Location = ((a.Asset.Room != null && a.Asset.Room.Location != null) ? new Dto.CodeNameEntity { Id = a.Asset.Room.LocationId, Code = a.Asset.Room.Location.Code, Name = a.Asset.Room.Location.Name } : null),
                   Room = (a.Asset.Room != null ? new Dto.CodeNameEntity { Id = a.Asset.Room.Id, Code = a.Asset.Room.Code, Name = a.Asset.Room.Name } : null),
                   AssetCategory = (a.Asset.AssetCategory != null ? new Dto.CodeNameEntity { Id = a.Asset.AssetCategory.Id, Code = a.Asset.AssetCategory.Code, Name = a.Asset.AssetCategory.Name } : null),
                   AssetType = (a.Asset.AssetType != null ? new Dto.CodeNameEntity { Id = a.Asset.AssetType.Id, Code = a.Asset.AssetType.Code, Name = a.Asset.AssetType.Name } : null),
                   AssetState = (a.Asset.AssetState != null ? new Dto.CodeNameEntity { Id = a.Asset.AssetState.Id, Code = a.Asset.AssetState.Code, Name = a.Asset.AssetState.Name } : null),
                   CostCenter = (a.Asset.CostCenter != null ? new Dto.CodeNameEntity { Id = a.Asset.CostCenter.Id, Code = a.Asset.CostCenter.Code, Name = a.Asset.CostCenter.Name } : null),
                //   AdmCenter = (a.Asset.Room.Location.AdmCenter != null ? new Dto.CodeNameEntity { Id = a.Asset.Room.Location.AdmCenter.Id, Code = a.Asset.Room.Location.AdmCenter.Code, Name = a.Asset.Room.Location.AdmCenter.Name } : null),
                   Employee = (a.Asset.Employee != null ? new Dto.EmployeeResource { Id = a.Asset.Employee.Id, InternalCode = a.Asset.Employee.InternalCode, FirstName = a.Asset.Employee.FirstName, LastName = a.Asset.Employee.LastName } : null)
               }))
               .ForMember(a => a.Dep, opt => opt.MapFrom(a => new Dto.AssetDepDetail
               {
                   DepForYear = a.Dep.ValueInv,
                   PosCap = a.Dep.ValueRem,
                   BkValFYStart = a.Dep.ValueDepPU,
                   RemLifeInPeriods = a.Dep.DepPeriodMonth,
                   AccumulDep = a.Dep.ValueDep,
                   CurrentAPC = a.Dep.ValueDepYTD,
                   ExpLifeInPeriods = a.Dep.DepPeriod,
                   TotLifeInpPeriods = a.Dep.DepPeriodRem
               }
               ))
               ;

            CreateMap<Model.AssetMonthDetail, Dto.Asset>()
               .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Asset.Id))
               .ForMember(a => a.InvNo, opt => opt.MapFrom(a => a.Asset.InvNo))
               .ForMember(a => a.InSapValidation, opt => opt.MapFrom(a => a.Asset.InSapValidation))
               .ForMember(a => a.Adm, opt => opt.MapFrom(a => a.Adm))
               .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Asset.Name == null ? "" : a.Asset.Name))
               .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.Asset.IsAccepted))
               .ForMember(a => a.IsReconcile, opt => opt.MapFrom(a => a.Asset.IsReconcile))
               .ForMember(a => a.IsLocked, opt => opt.MapFrom(a => a.Asset.IsLocked))
               .ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Asset.Quantity))
               .ForMember(a => a.PurchaseDate, opt => opt.MapFrom(a => a.Asset.PurchaseDate))
               .ForMember(a => a.ReceptionDate, opt => opt.MapFrom(a => a.Asset.ReceptionDate))
               .ForMember(a => a.PODate, opt => opt.MapFrom(a => a.Asset.PODate))
               .ForMember(a => a.InvoiceDate, opt => opt.MapFrom(a => a.Asset.InvoiceDate))
               .ForMember(a => a.RemovalDate, opt => opt.MapFrom(a => a.Asset.RemovalDate))
               .ForMember(a => a.CreatedAt, opt => opt.MapFrom(a => a.Asset.CreatedAt))
               .ForMember(a => a.ModifiedAt, opt => opt.MapFrom(a => a.Asset.ModifiedAt))
               .ForMember(a => a.ERPCode, opt => opt.MapFrom(a => a.Asset.ERPCode))
               .ForMember(a => a.SAPCode, opt => opt.MapFrom(a => a.Asset.SAPCode == null ? "" : a.Asset.SAPCode))
			   .ForMember(a => a.PhoneNumber, opt => opt.MapFrom(a => a.Asset.PhoneNumber == null ? "" : a.Asset.PhoneNumber))
			   .ForMember(a => a.Imei, opt => opt.MapFrom(a => a.Asset.Imei == null ? "" : a.Asset.Imei))
			   .ForMember(a => a.Info, opt => opt.MapFrom(a => a.Asset.Info == null ? "" : a.Asset.Info))
			   .ForMember(a => a.IsInTransfer, opt => opt.MapFrom(a => a.Asset.IsInTransfer))
               .ForMember(a => a.InvName, opt => opt.MapFrom(a => a.Asset.AssetInv.InvName))
               .ForMember(a => a.SerialNumber, opt => opt.MapFrom(a => a.Asset.SerialNumber == null ? "" : a.Asset.SerialNumber))
               .ForMember(a => a.SubNo, opt => opt.MapFrom(a => a.Asset.SubNo))
               .ForMember(a => a.DocNo2, opt => opt.MapFrom(a => a.Asset.Document.DocNo2))
               .ForMember(a => a.DocNo1, opt => opt.MapFrom(a => a.Asset.Document.DocNo1))
               .ForMember(a => a.Details, opt => opt.MapFrom(a => a.Asset.Document.Details))
               .ForMember(a => a.IsDuplicate, opt => opt.MapFrom(a => a.Asset.IsDuplicate))
               .ForMember(a => a.IsPrinted, opt => opt.MapFrom(a => a.Asset.IsPrinted))
               .ForMember(a => a.PrintDate, opt => opt.MapFrom(a => a.Asset.PrintDate))
               .ForMember(a => a.InfoIni, opt => opt.MapFrom(a => a.Asset.AssetInv.Info))
               .ForMember(a => a.Manufacturer, opt => opt.MapFrom(a => a.Asset.Manufacturer))
               .ForMember(a => a.ModelIni, opt => opt.MapFrom(a => a.Asset.AssetInv.Model))
               .ForMember(a => a.NotSync, opt => opt.MapFrom(a => a.Asset.NotSync))
			   .ForMember(a => a.Storno, opt => opt.MapFrom(a => a.Asset.Storno))
			   .ForMember(a => a.StornoQuantity, opt => opt.MapFrom(a => a.Asset.StornoQuantity))
			   .ForMember(a => a.StornoValue, opt => opt.MapFrom(a => a.Asset.StornoValue))
			   .ForMember(a => a.StornoValueRon, opt => opt.MapFrom(a => a.Asset.StornoValueRon))
			   .ForMember(i => i.Cassation, opt => opt.MapFrom(i => i.Asset.Cassation))
			    .ForMember(i => i.CassationQuantity, opt => opt.MapFrom(i => i.Asset.CassationQuantity))
			    .ForMember(i => i.CassationValue, opt => opt.MapFrom(i => i.Asset.CassationValue))
			    .ForMember(i => i.CassationValueRon, opt => opt.MapFrom(i => i.Asset.CassationValueRon))
			   .ForMember(a => a.Document, opt => opt.MapFrom(
                   a => new Dto.DocumentMainDetail
                   {
                       Id = a.Asset.Document.Id,
                       DocNo1 = a.Asset.Document.DocNo1,
                       DocNo2 = a.Asset.Document.DocNo2,
                       DocumentDate = a.Asset.Document.DocumentDate,
                       RegisterDate = a.Asset.Document.RegisterDate,
                      
                       DocumentType = new Dto.CodeNameEntity
                       {
                           Id = a.Asset.Document.DocumentTypeId,
                           Code = a.Asset.Document.DocumentType.Code,
                           Name = a.Asset.Document.DocumentType.Name
                       },
                       Partner = new Dto.Partner
                       {
                           Id = a.Asset.Document.Partner.Id,
                           Name = a.Asset.Document.Partner.Name,
                           FiscalCode = a.Asset.Document.Partner.FiscalCode,
                           RegistryNumber = a.Asset.Document.Partner.RegistryNumber,
                           ErpCode = a.Asset.Document.Partner.ErpCode,
                           PartnerLocation = new Dto.PartnerLocation()
                           {
                               Cui = a.Asset.Document.Partner.PartnerLocation.Cui,
                               Data = a.Asset.Document.Partner.PartnerLocation.Data,
                               Denumire = a.Asset.Document.Partner.PartnerLocation.Denumire,
                               Adresa = a.Asset.Document.Partner.PartnerLocation.Adresa,
                               NrRegCom = a.Asset.Document.Partner.PartnerLocation.NrRegCom,
                               Telefon = a.Asset.Document.Partner.PartnerLocation.Telefon,
                               Fax = a.Asset.Document.Partner.PartnerLocation.Fax,
                               CodPostal = a.Asset.Document.Partner.PartnerLocation.CodPostal,
                               Act = a.Asset.Document.Partner.PartnerLocation.Act,
                               Stare_inregistrare = a.Asset.Document.Partner.PartnerLocation.Stare_inregistrare,
                               ScpTVA = a.Asset.Document.Partner.PartnerLocation.ScpTVA,
                               Data_inceput_ScpTVA = a.Asset.Document.Partner.PartnerLocation.Data_inceput_ScpTVA,
                               Data_sfarsit_ScpTVA = a.Asset.Document.Partner.PartnerLocation.Data_sfarsit_ScpTVA,
                               Data_anul_imp_ScpTVA = a.Asset.Document.Partner.PartnerLocation.Data_anul_imp_ScpTVA,
                               Mesaj_ScpTVA = a.Asset.Document.Partner.PartnerLocation.Mesaj_ScpTVA,
                               DataInceputTvaInc = a.Asset.Document.Partner.PartnerLocation.DataInceputTvaInc,
                               DataSfarsitTvaInc = a.Asset.Document.Partner.PartnerLocation.DataSfarsitTvaInc,
                               DataActualizareTvaInc = a.Asset.Document.Partner.PartnerLocation.DataActualizareTvaInc,
                               DataPublicareTvaInc = a.Asset.Document.Partner.PartnerLocation.DataPublicareTvaInc,
                               TipActTvaInc = a.Asset.Document.Partner.PartnerLocation.TipActTvaInc,
                               StatusTvaIncasare = a.Asset.Document.Partner.PartnerLocation.StatusTvaIncasare,
                               DataInactivare = a.Asset.Document.Partner.PartnerLocation.DataInactivare,
                               DataReactivare = a.Asset.Document.Partner.PartnerLocation.DataReactivare,
                               DataPublicare = a.Asset.Document.Partner.PartnerLocation.DataPublicare,
                               DataRadiere = a.Asset.Document.Partner.PartnerLocation.DataRadiere,
                               StatusInactivi = a.Asset.Document.Partner.PartnerLocation.StatusInactivi,
                               DataInceputSplitTVA = a.Asset.Document.Partner.PartnerLocation.DataInceputSplitTVA,
                               DataAnulareSplitTVA = a.Asset.Document.Partner.PartnerLocation.DataAnulareSplitTVA,
                               Iban = a.Asset.Document.Partner.PartnerLocation.Iban,
                               StatusRO_e_Factura = a.Asset.Document.Partner.PartnerLocation.StatusRO_e_Factura
                           }
                       }
                   }))
			     .ForMember(assetDto => assetDto.CreatedByUser, opt => opt.MapFrom(p => (p.Asset.CreatedByUser != null) ? new Dto.ApplicationUser { Id = p.Asset.CreatedByUser.Id, Email = p.Asset.CreatedByUser.Email, UserName = p.Asset.CreatedByUser.UserName } : null))
				.ForMember(assetDto => assetDto.EmployeeTransfer, opt => opt.MapFrom(p => (p.Asset.EmployeeTransfer != null) ? new Dto.EmployeeResource { Id = p.Asset.EmployeeTransfer.Id, InternalCode = p.Asset.EmployeeTransfer.InternalCode, Email = p.Asset.EmployeeTransfer.Email } : null))
                .ForMember(assetDto => assetDto.Material, opt => opt.MapFrom(p => (p.Asset.Material != null) ? new Dto.CodeNameEntity { Id = p.Asset.Material.Id, Code = p.Asset.Material.Code, Name = p.Asset.Material.Name } : null))
                .ForMember(assetDto => assetDto.State, opt => opt.MapFrom(p => (p.Asset.AppState != null) ? new Dto.CodeNameEntity { Id = p.Asset.AppState.Id, Code = p.Asset.AppState.Code, Name = p.Asset.AppState.Name } : null))
                .ForMember(assetDto => assetDto.WFHState, opt => opt.MapFrom(p => (p.Asset.WFHState != null) ? new Dto.AppState { Id = p.Asset.WFHState.Id, Code = p.Asset.WFHState.Code, Name = p.Asset.WFHState.Name, BadgeColor = p.Asset.WFHState.BadgeColor, BadgeIcon = p.Asset.WFHState.BadgeIcon } : null))
                .ForMember(assetDto => assetDto.Tax, opt => opt.MapFrom(p => (p.Asset.Tax != null) ? new Dto.Tax { Id = p.Asset.Tax.Id, Code = p.Asset.Tax.Code, Name = p.Asset.Tax.Name } : null))
			   // .ForMember(assetDto => assetDto.Model, opt => opt.MapFrom(p => (p.Asset.Model != null) ? new Dto.CodeNameEntity { Id = p.Asset.Model.Id, Code = p.Asset.Model.Code, Name = p.Asset.Model.Name } : null))
			   //.ForMember(assetDto => assetDto.Brand, opt => opt.MapFrom(p => (p.Asset.Brand != null) ? new Dto.CodeNameEntity { Id = p.Asset.Brand.Id, Code = p.Asset.Brand.Code, Name = p.Asset.Brand.Name } : null))
			   //.ForMember(a => a.Dimension, opt => opt.MapFrom(a => new Dto.Dimension { Id = a.Asset.Dimension.Id, Length = a.Asset.Dimension.Length, Width = a.Asset.Dimension.Width, Height = a.Asset.Dimension.Height }))
			   .ForMember(assetDto => assetDto.Uom, opt => opt.MapFrom(p => (p.Asset.Uom != null) ? new Dto.CodeNameEntity { Id = p.Asset.Uom.Id, Code = p.Asset.Uom.Code, Name = p.Asset.Uom.Name } : null))
               .ForMember(assetDto => assetDto.Order, opt => opt.MapFrom(p => (p.Asset.Order != null) ? new Dto.Order { Id = p.Asset.Order.Id, Code = p.Asset.Order.Code, Name = p.Asset.Order.Name, Offer = (p.Asset.Order.Offer != null ? new Dto.Offer { Id = p.Asset.Order.Offer.Id, Code = p.Asset.Order.Offer.Code, Name = p.Asset.Order.Offer.Name } : null )} : null))
               .ForMember(assetDto => assetDto.DictionaryItem, opt => opt.MapFrom(p => (p.Asset.DictionaryItem != null) ? new Dto.CodeNameEntity { Id = p.Asset.DictionaryItem.Id, Code = p.Asset.DictionaryItem.Code, Name = p.Asset.DictionaryItem.Name } : null))
               .ForMember(assetDto => assetDto.InvState, opt => opt.MapFrom(p => (p.Asset.InvState != null) ? new Dto.InvState { Id = p.Asset.InvState.Id, Code = p.Asset.InvState.Code, Name = p.Asset.InvState.Name, BadgeIcon = p.Asset.InvState.BadgeIcon, BadgeColor = p.Asset.InvState.BadgeColor } : null))
               .ForMember(assetDto => assetDto.Company, opt => opt.MapFrom(p => (p.Asset.Company != null) ? new Dto.CodeNameEntity { Id = p.Asset.Company.Id, Code = p.Asset.Company.Code, Name = p.Asset.Company.Name } : null))
			   .ForMember(assetDto => assetDto.TempUser, opt => opt.MapFrom(p => (p.Asset.TempUser != null) ? new Dto.ApplicationUser { Id = p.Asset.TempUser.Id, Email = p.Asset.TempUser.Email, UserName = p.Asset.TempUser.UserName } : null))
			   .ForMember(a => a.Partner, opt => opt.MapFrom(a => new Dto.CodePartnerEntity { Id = a.Asset.Document.Partner.Id, Name = a.Asset.Document.Partner.Name, RegistryNumber= a.Asset.Document.Partner.RegistryNumber }))
			   .ForMember(a => a.Adm, opt => opt.MapFrom(a => new Dto.AssetAdmDetail
               {
                   Account = (a.Adm.Account != null ? new Dto.CodeNameEntity { Id = a.Adm.Account.Id, Code = a.Adm.Account.Code, Name = a.Adm.Account.Name } : null),
                   ExpAccount = (a.Adm.ExpAccount != null ? new Dto.CodeNameEntity { Id = a.Adm.ExpAccount.Id, Code = a.Adm.ExpAccount.Code, Name = a.Adm.ExpAccount.Name } : null),
                   AssetCategory = (a.Adm.AssetCategory != null ? new Dto.CodeNameEntity { Id = a.Adm.AssetCategory.Id, Code = a.Adm.AssetCategory.Code, Name = a.Adm.AssetCategory.Name } : null),
                   Article = (a.Adm.Article != null ? new Dto.CodeNameEntity { Id = a.Adm.Article.Id, Code = a.Adm.Article.Code, Name = a.Adm.Article.Name } : null),
                   CostCenter = (a.Adm.CostCenter != null ? new Dto.CodeNameEntity { Id = a.Adm.CostCenter.Id, Code = a.Adm.CostCenter.Code, Name = a.Adm.CostCenter.Name } : null),
                   Department = (a.Adm.Department != null ? new Dto.CodeNameEntity { Id = a.Adm.Department.Id, Code = a.Adm.Department.Code, Name = a.Adm.Department.Name } : null),
                   Division = ((a.Adm.Division != null && a.Adm.Division != null) ? new Dto.CodeNameEntity { Id = a.Adm.Division.Id, Code = a.Adm.Division.Code, Name = a.Adm.Division.Name } : null),
                   Administration = (a.Adm.Administration != null ? new Dto.CodeNameEntity { Id = a.Adm.Administration.Id, Code = a.Adm.Administration.Code, Name = a.Adm.Administration.Name } : null),
                   Room = (a.Adm.Room != null ? new Dto.CodeNameEntity { Id = a.Adm.Room.Id, Code = a.Adm.Room.Code, Name = a.Adm.Room.Name } : null),
                   City = ((a.Adm.Room != null && a.Adm.Room.Location != null && a.Adm.Room.Location.City != null) ? new Dto.CodeNameEntity { Id = a.Adm.Room.Location.City.Id, Code = a.Adm.Room.Location.City.Code, Name = a.Adm.Room.Location.City.Name } : null),
                   Country = ((a.Adm.Room != null && a.Adm.Room.Location != null && a.Adm.Room.Location.City != null && a.Adm.Room.Location.City.County != null && a.Adm.Room.Location.City.County.Country != null) ? new Dto.CodeNameEntity { Id = a.Adm.Room.Location.City.County.Country.Id, Code = a.Adm.Room.Location.City.County.Country.Code, Name = a.Adm.Room.Location.City.County.Country.Name } : null),
                   BudgetManager = (a.Adm.BudgetManager != null ? new Dto.CodeNameEntity { Id = a.Adm.BudgetManager.Id, Code = a.Adm.BudgetManager.Code, Name = a.Adm.BudgetManager.Name } : null),
                   AssetNature = (a.Adm.AssetNature != null ? new Dto.CodeNameEntity { Id = a.Adm.AssetNature.Id, Code = a.Adm.AssetNature.Code, Name = a.Adm.AssetNature.Name } : null),
                   Type = (a.Adm.SubType != null && a.Adm.SubType.Type != null ? new Dto.CodeNameEntity { Id = a.Adm.SubType.Type.Id, Code = a.Adm.SubType.Type.Code, Name = a.Adm.SubType.Type.Name } : null),
                   SubType = (a.Adm.SubType != null ? new Dto.CodeNameEntity { Id = a.Adm.SubType.Id, Code = a.Adm.SubType.Code, Name = a.Adm.SubType.Name } : null),
                   Employee = (a.Adm.Employee != null ? new Dto.EmployeeResource 
                   { 
                       Id = a.Adm.Employee != null ? a.Adm.Employee.Id : 0, 
                       InternalCode = a.Adm.Employee != null ? a.Adm.Employee.InternalCode : "", 
                       FirstName = a.Adm.Employee != null ? a.Adm.Employee.FirstName : "", 
                       LastName = a.Adm.Employee != null ? a.Adm.Employee.LastName : "", 
                       ErpCode = a.Adm.Employee != null ? a.Adm.Employee.ERPCode : "", 
                       IsDeleted = a.Adm.Employee != null ? a.Adm.Employee.IsDeleted : false, 
                       Email = a.Adm.Employee != null ? a.Adm.Employee.Email : "",
                       Company = new Dto.CodeNameEntity
                       {
                           Id = a.Adm.Employee != null && a.Adm.Employee.Company != null ? a.Adm.Employee.Company.Id : 0,
						   Code = a.Adm.Employee != null && a.Adm.Employee.Company != null ? a.Adm.Employee.Company.Code : "",
						   Name = a.Adm.Employee != null && a.Adm.Employee.Company != null ? a.Adm.Employee.Company.Name : "",
					   },
					   CostCenter = new Dto.CodeNameEntity
					   {
						   Id = a.Adm.Employee != null && a.Adm.Employee.CostCenter != null ? a.Adm.Employee.CostCenter.Id : 0,
						   Code = a.Adm.Employee != null && a.Adm.Employee.CostCenter != null ? a.Adm.Employee.CostCenter.Code : "",
						   Name = a.Adm.Employee != null && a.Adm.Employee.CostCenter != null ? a.Adm.Employee.CostCenter.Name : "",
					   },
					   Division = new Dto.CodeNameEntity
					   {
						   Id = a.Adm.Employee != null && a.Adm.Employee.CostCenter != null && a.Adm.Employee.CostCenter.Division != null ? a.Adm.Employee.CostCenter.Division.Id : 0,
						   Code = a.Adm.Employee != null && a.Adm.Employee.CostCenter != null && a.Adm.Employee.CostCenter.Division != null ? a.Adm.Employee.CostCenter.Division.Code : "",
						   Name = a.Adm.Employee != null && a.Adm.Employee.CostCenter != null && a.Adm.Employee.CostCenter.Division != null ? a.Adm.Employee.CostCenter.Division.Name : "",
					   },
					   Department = new Dto.CodeNameEntity
					   {
						   Id = a.Adm.Employee != null && a.Adm.Employee.CostCenter != null && a.Adm.Employee.CostCenter.Division != null && a.Adm.Employee.CostCenter.Division.Department != null ? a.Adm.Employee.CostCenter.Division.Department.Id : 0,
						   Code = a.Adm.Employee != null && a.Adm.Employee.CostCenter != null && a.Adm.Employee.CostCenter.Division != null && a.Adm.Employee.CostCenter.Division.Department != null ? a.Adm.Employee.CostCenter.Division.Department.Code : "",
						   Name = a.Adm.Employee != null && a.Adm.Employee.CostCenter != null && a.Adm.Employee.CostCenter.Division != null && a.Adm.Employee.CostCenter.Division.Department != null ? a.Adm.Employee.CostCenter.Division.Department.Name : "",
					   }
				   } : null),
                   AdmCenter = (a.Adm.CostCenter != null && a.Adm.CostCenter.AdmCenter != null ? new Dto.CodeNameEntity { Id = a.Adm.CostCenter.AdmCenter.Id, Code = a.Adm.CostCenter.AdmCenter.Code, Name = a.Adm.CostCenter.AdmCenter.Name } : null),
                   Region = ((a.Adm.CostCenter != null && a.Adm.CostCenter.AdmCenter != null && a.Adm.CostCenter.Region != null) ? new Dto.CodeNameEntity { Id = a.Adm.CostCenter.Region.Id, Code = a.Adm.CostCenter.Region.Code, Name = a.Adm.CostCenter.Region.Name } : null),
                   Location = ((a.Adm.Room != null && a.Adm.Room.Location != null) ? new Dto.CodeNameEntity { Id = a.Adm.Room.LocationId, Code = a.Adm.Room.Location.Code, Name = a.Adm.Room.Location.Name } : null),
                   InsuranceCategory = (a.Adm.InsuranceCategory != null ? new Dto.CodeNameEntity { Id = a.Adm.InsuranceCategory.Id, Code = a.Adm.InsuranceCategory.Code, Name = a.Adm.InsuranceCategory.Name } : null),
                   Project = (a.Adm.Project != null ? new Dto.CodeNameEntity { Id = a.Adm.Project.Id, Code = a.Adm.Project.Code, Name = a.Adm.Project.Name } : null),
                   AssetType = (a.Adm.AssetType != null ? new Dto.CodeNameEntity { Id = a.Adm.AssetType.Id, Code = a.Adm.AssetType.Code, Name = a.Adm.AssetType.Name } : null),
                   AssetState = (a.Adm.AssetState != null ? new Dto.CodeNameEntity { Id = a.Adm.AssetState.Id, Code = a.Adm.AssetState.Code, Name = a.Adm.AssetState.Name } : null),
                   AssetClass = (a.Adm.AssetClass != null ? new Dto.CodeNameEntity { Id = a.Adm.AssetClass.Id, Code = a.Adm.AssetClass.Code, Name = a.Adm.AssetClass.Name } : null),


                   //Company = (a.Adm.Company != null ? new Dto.CodeNameEntity { Id = a.Adm.Company.Id, Code = a.Adm.Company.Code, Name = a.Adm.Company.Name } : null),



                   //AdmCenter = ((a.Adm.Room != null && a.Adm.Room.Location != null && a.Adm.Room.Location.AdmCenter != null) ? new Dto.CodeNameEntity { Id = a.Adm.Room.Location.AdmCenter.Id, Code = a.Adm.Room.Location.AdmCenter.Code, Name = a.Adm.Room.Location.AdmCenter.Name } : null),
                   //MasterType = (a.Adm.SubType != null && a.Adm.SubType.Type != null && a.Adm.SubType.Type.MasterType != null ? new Dto.CodeNameEntity { Id = a.Adm.SubType.Type.MasterType.Id, Code = a.Adm.SubType.Type.MasterType.Code, Name = a.Adm.SubType.Type.MasterType.Name } : null),


                   Model = (a.Adm.Model != null ? new Dto.Model { Id = a.Adm.Model.Id, Code = a.Adm.Model.Code, Name = a.Adm.Model.Name, SNLength = a.Adm.Model.SNLength , IMEILength = a.Adm.Model.IMEILength } : null),
                   Brand = (a.Adm.Brand != null ? new Dto.Brand
                   {
                       Id = a.Adm.Brand != null ? a.Adm.Brand.Id : 0,
                       Code = a.Adm.Brand != null ? a.Adm.Brand.Code : "",
                       Name = a.Adm.Brand != null ? a.Adm.Brand.Name : "",
                       Imei1Pattern = a.Adm.Brand != null ? a.Adm.Brand.Imei1Pattern : "",
                       Imei2Pattern = a.Adm.Brand != null ? a.Adm.Brand.Imei1Pattern : "",
                       Imei3Pattern = a.Adm.Brand != null ? a.Adm.Brand.Imei1Pattern : "",
                       Imei4Pattern = a.Adm.Brand != null ? a.Adm.Brand.Imei1Pattern : "",
                       Imei5Pattern = a.Adm.Brand != null ? a.Adm.Brand.Imei1Pattern : "",
                       PhoneNumber1Pattern = a.Adm.Brand != null ? a.Adm.Brand.PhoneNumber1Pattern : "",
					   PhoneNumber2Pattern = a.Adm.Brand != null ? a.Adm.Brand.PhoneNumber2Pattern : "",
					   PhoneNumber3Pattern = a.Adm.Brand != null ? a.Adm.Brand.PhoneNumber3Pattern : "",
					   PhoneNumber4Pattern = a.Adm.Brand != null ? a.Adm.Brand.PhoneNumber4Pattern : "",
					   PhoneNumber5Pattern = a.Adm.Brand != null ? a.Adm.Brand.PhoneNumber5Pattern : "",
					   Serial1Pattern = a.Adm.Brand != null ? a.Adm.Brand.Serial1Pattern : "",
					   Serial2Pattern = a.Adm.Brand != null ? a.Adm.Brand.Serial2Pattern : "",
					   Serial3Pattern = a.Adm.Brand != null ? a.Adm.Brand.Serial3Pattern : "",
					   Serial4Pattern = a.Adm.Brand != null ? a.Adm.Brand.Serial4Pattern : "",
					   Serial5Pattern = a.Adm.Brand != null ? a.Adm.Brand.Serial5Pattern : "",
					   Tag1Pattern = a.Adm.Brand != null ? a.Adm.Brand.Tag1Pattern : "",
					   Tag2Pattern = a.Adm.Brand != null ? a.Adm.Brand.Tag2Pattern : "",
					   Tag3Pattern = a.Adm.Brand != null ? a.Adm.Brand.Tag3Pattern : "",
					   Tag4Pattern = a.Adm.Brand != null ? a.Adm.Brand.Tag4Pattern : "",
					   Tag5Pattern = a.Adm.Brand != null ? a.Adm.Brand.Tag5Pattern : ""
				   } : null),




                   //Region = (a.Adm.Room.Location.Region != null ? new Dto.CodeNameEntity { Id = a.Adm.Room.Location.Region.Id, Code = a.Adm.Room.Location.Region.Code, Name = a.Adm.Room.Location.Region.Name } : null),


               }))
               .ForMember(a => a.Dep, opt => opt.MapFrom(a => new Dto.AssetDepDetail
               {

                   UsefulLife = a.Dep.UsefulLife,
                   ExpLifeInPeriods = a.Dep.ExpLifeInPeriods,
                   RemLifeInPeriods = a.Dep.RemLifeInPeriods,
                   TotLifeInpPeriods = a.Dep.TotLifeInpPeriods,
                   DepFYStart = a.Dep.DepFYStart,
                   APCFYStart = a.Dep.APCFYStart,
                   BkValFYStart = a.Dep.BkValFYStart,
                   CurrBkValue = a.Dep.CurrBkValue,
                   CurrentAPC = a.Dep.CurrentAPC,
                   DepRetirement = a.Dep.DepRetirement,
                   DepTransfer = a.Dep.DepTransfer,
                   Acquisition = a.Dep.Acquisition,
                   Retirement = a.Dep.Retirement,
                   Transfer = a.Dep.Transfer,
                   PosCap = a.Dep.PosCap,
                   DepPostCap = a.Dep.DepPostCap,
                   DepForYear = a.Dep.DepForYear,
                   AccumulDep = a.Dep.AccumulDep,
                   InvestSupport = a.Dep.InvestSupport,
                   WriteUps = a.Dep.WriteUps,
                   AccSystem = (a.Dep.AccSystem != null ? new Model.AccSystem{ Id = a.Dep.AccSystem.Id, Code = a.Dep.AccSystem.Code, Name = a.Dep.AccSystem.Name } : null)
               }
               ))
               ;

            CreateMap<Model.AssetMonthDetail, Dto.DashboardView>()
               .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Asset.Id))
               .ForMember(a => a.InvNo, opt => opt.MapFrom(a => a.Asset.InvNo))
               .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Asset.Name))
               .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.Asset.IsAccepted))
               .ForMember(a => a.IsReconcile, opt => opt.MapFrom(a => a.Asset.IsReconcile))
               .ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Asset.Quantity))
               .ForMember(a => a.PurchaseDate, opt => opt.MapFrom(a => a.Asset.PurchaseDate))
               .ForMember(a => a.ReceptionDate, opt => opt.MapFrom(a => a.Asset.ReceptionDate))
               .ForMember(a => a.PODate, opt => opt.MapFrom(a => a.Asset.PODate))
               .ForMember(a => a.InvoiceDate, opt => opt.MapFrom(a => a.Asset.InvoiceDate))
               .ForMember(a => a.RemovalDate, opt => opt.MapFrom(a => a.Asset.RemovalDate))
               .ForMember(a => a.CreatedAt, opt => opt.MapFrom(a => a.Asset.CreatedAt))
               .ForMember(a => a.ERPCode, opt => opt.MapFrom(a => a.Asset.ERPCode))
               .ForMember(a => a.SAPCode, opt => opt.MapFrom(a => a.Asset.SAPCode))
               .ForMember(a => a.IsInTransfer, opt => opt.MapFrom(a => a.Asset.IsInTransfer))
			   .ForMember(a => a.InvName, opt => opt.MapFrom(a => a.Asset.AssetInv.InvName))
               .ForMember(a => a.SerialNumber, opt => opt.MapFrom(a => a.Asset.SerialNumber))
               .ForMember(a => a.SubNo, opt => opt.MapFrom(a => a.Asset.SubNo))
               .ForMember(a => a.DocNo2, opt => opt.MapFrom(a => a.Asset.Document.DocNo2))
               .ForMember(a => a.DocNo1, opt => opt.MapFrom(a => a.Asset.Document.DocNo1))
               .ForMember(a => a.IsDuplicate, opt => opt.MapFrom(a => a.Asset.IsDuplicate))
               .ForMember(a => a.IsPrinted, opt => opt.MapFrom(a => a.Asset.IsPrinted))
               .ForMember(a => a.PrintDate, opt => opt.MapFrom(a => a.Asset.PrintDate))
               .ForMember(a => a.InfoIni, opt => opt.MapFrom(a => a.Asset.AssetInv.Info))
               .ForMember(a => a.Manufacturer, opt => opt.MapFrom(a => a.Asset.Manufacturer))
               .ForMember(a => a.ModelIni, opt => opt.MapFrom(a => a.Asset.AssetInv.Model))
               .ForMember(a => a.Document, opt => opt.MapFrom(
                   a => new Dto.DocumentMainDetail
                   {
                       Id = a.Asset.Document.Id,
                       DocNo1 = a.Asset.Document.DocNo1,
                       DocNo2 = a.Asset.Document.DocNo2,
                       DocumentDate = a.Asset.Document.DocumentDate,
                       RegisterDate = a.Asset.Document.RegisterDate,

                       DocumentType = new Dto.CodeNameEntity
                       {
                           Id = a.Asset.Document.DocumentTypeId,
                           Code = a.Asset.Document.DocumentType.Code,
                           Name = a.Asset.Document.DocumentType.Name
                       },
                       Partner = new Dto.Partner
                       {
                           Id = a.Asset.Document.Partner.Id,
                           Name = a.Asset.Document.Partner.Name,
                           FiscalCode = a.Asset.Document.Partner.FiscalCode,
                           RegistryNumber = a.Asset.Document.Partner.RegistryNumber,
                           ErpCode = a.Asset.Document.Partner.ErpCode,
                           PartnerLocation = new Dto.PartnerLocation()
                           {
                               Cui = a.Asset.Document.Partner.PartnerLocation.Cui,
                               Data = a.Asset.Document.Partner.PartnerLocation.Data,
                               Denumire = a.Asset.Document.Partner.PartnerLocation.Denumire,
                               Adresa = a.Asset.Document.Partner.PartnerLocation.Adresa,
                               NrRegCom = a.Asset.Document.Partner.PartnerLocation.NrRegCom,
                               Telefon = a.Asset.Document.Partner.PartnerLocation.Telefon,
                               Fax = a.Asset.Document.Partner.PartnerLocation.Fax,
                               CodPostal = a.Asset.Document.Partner.PartnerLocation.CodPostal,
                               Act = a.Asset.Document.Partner.PartnerLocation.Act,
                               Stare_inregistrare = a.Asset.Document.Partner.PartnerLocation.Stare_inregistrare,
                               ScpTVA = a.Asset.Document.Partner.PartnerLocation.ScpTVA,
                               Data_inceput_ScpTVA = a.Asset.Document.Partner.PartnerLocation.Data_inceput_ScpTVA,
                               Data_sfarsit_ScpTVA = a.Asset.Document.Partner.PartnerLocation.Data_sfarsit_ScpTVA,
                               Data_anul_imp_ScpTVA = a.Asset.Document.Partner.PartnerLocation.Data_anul_imp_ScpTVA,
                               Mesaj_ScpTVA = a.Asset.Document.Partner.PartnerLocation.Mesaj_ScpTVA,
                               DataInceputTvaInc = a.Asset.Document.Partner.PartnerLocation.DataInceputTvaInc,
                               DataSfarsitTvaInc = a.Asset.Document.Partner.PartnerLocation.DataSfarsitTvaInc,
                               DataActualizareTvaInc = a.Asset.Document.Partner.PartnerLocation.DataActualizareTvaInc,
                               DataPublicareTvaInc = a.Asset.Document.Partner.PartnerLocation.DataPublicareTvaInc,
                               TipActTvaInc = a.Asset.Document.Partner.PartnerLocation.TipActTvaInc,
                               StatusTvaIncasare = a.Asset.Document.Partner.PartnerLocation.StatusTvaIncasare,
                               DataInactivare = a.Asset.Document.Partner.PartnerLocation.DataInactivare,
                               DataReactivare = a.Asset.Document.Partner.PartnerLocation.DataReactivare,
                               DataPublicare = a.Asset.Document.Partner.PartnerLocation.DataPublicare,
                               DataRadiere = a.Asset.Document.Partner.PartnerLocation.DataRadiere,
                               StatusInactivi = a.Asset.Document.Partner.PartnerLocation.StatusInactivi,
                               DataInceputSplitTVA = a.Asset.Document.Partner.PartnerLocation.DataInceputSplitTVA,
                               DataAnulareSplitTVA = a.Asset.Document.Partner.PartnerLocation.DataAnulareSplitTVA,
                               Iban = a.Asset.Document.Partner.PartnerLocation.Iban,
                               StatusRO_e_Factura = a.Asset.Document.Partner.PartnerLocation.StatusRO_e_Factura
                           }
                       }
                   }))
                .ForMember(assetDto => assetDto.Material, opt => opt.MapFrom(p => (p.Asset.Material != null) ? new Dto.CodeNameEntity { Id = p.Asset.Material.Id, Code = p.Asset.Material.Code, Name = p.Asset.Material.Name } : null))
               // .ForMember(assetDto => assetDto.Model, opt => opt.MapFrom(p => (p.Asset.Model != null) ? new Dto.CodeNameEntity { Id = p.Asset.Model.Id, Code = p.Asset.Model.Code, Name = p.Asset.Model.Name } : null))
               //.ForMember(assetDto => assetDto.Brand, opt => opt.MapFrom(p => (p.Asset.Brand != null) ? new Dto.CodeNameEntity { Id = p.Asset.Brand.Id, Code = p.Asset.Brand.Code, Name = p.Asset.Brand.Name } : null))
               //.ForMember(a => a.Dimension, opt => opt.MapFrom(a => new Dto.Dimension { Id = a.Asset.Dimension.Id, Length = a.Asset.Dimension.Length, Width = a.Asset.Dimension.Width, Height = a.Asset.Dimension.Height }))
               .ForMember(assetDto => assetDto.Uom, opt => opt.MapFrom(p => (p.Asset.Uom != null) ? new Dto.CodeNameEntity { Id = p.Asset.Uom.Id, Code = p.Asset.Uom.Code, Name = p.Asset.Uom.Name } : null))
               .ForMember(assetDto => assetDto.Order, opt => opt.MapFrom(p => (p.Asset.Order != null) ? new Dto.CodeNameEntity { Id = p.Asset.Order.Id, Code = p.Asset.Order.Code, Name = p.Asset.Order.Name } : null))
               .ForMember(assetDto => assetDto.DictionaryItem, opt => opt.MapFrom(p => (p.Asset.DictionaryItem != null) ? new Dto.CodeNameEntity { Id = p.Asset.DictionaryItem.Id, Code = p.Asset.DictionaryItem.Code, Name = p.Asset.DictionaryItem.Name } : null))
               .ForMember(assetDto => assetDto.InvState, opt => opt.MapFrom(p => (p.Asset.InvState != null) ? new Dto.CodeNameEntity { Id = p.Asset.InvState.Id, Code = p.Asset.InvState.Code, Name = p.Asset.InvState.Name } : null))
               .ForMember(assetDto => assetDto.Company, opt => opt.MapFrom(p => (p.Asset.Company != null) ? new Dto.CodeNameEntity { Id = p.Asset.Company.Id, Code = p.Asset.Company.Code, Name = p.Asset.Company.Name } : null))
               .ForMember(a => a.Partner, opt => opt.MapFrom(a => new Dto.CodePartnerEntity { Id = a.Asset.Document.Partner.Id, Name = a.Asset.Document.Partner.Name, RegistryNumber = a.Asset.Document.Partner.RegistryNumber }))
               .ForMember(a => a.Adm, opt => opt.MapFrom(a => new Dto.AssetAdmDetail
               {
                   Account = (a.Adm.Account != null ? new Dto.CodeNameEntity { Id = a.Adm.Account.Id, Code = a.Adm.Account.Code, Name = a.Adm.Account.Name } : null),
                   ExpAccount = (a.Adm.ExpAccount != null ? new Dto.CodeNameEntity { Id = a.Adm.ExpAccount.Id, Code = a.Adm.ExpAccount.Code, Name = a.Adm.ExpAccount.Name } : null),
                   AssetCategory = (a.Adm.AssetCategory != null ? new Dto.CodeNameEntity { Id = a.Adm.AssetCategory.Id, Code = a.Adm.AssetCategory.Code, Name = a.Adm.AssetCategory.Name } : null),
                   Article = (a.Adm.Article != null ? new Dto.CodeNameEntity { Id = a.Adm.Article.Id, Code = a.Adm.Article.Code, Name = a.Adm.Article.Name } : null),
                   CostCenter = (a.Adm.CostCenter != null ? new Dto.CodeNameEntity { Id = a.Adm.CostCenter.Id, Code = a.Adm.CostCenter.Code, Name = a.Adm.CostCenter.Name } : null),
                   Department = (a.Adm.Department != null ? new Dto.CodeNameEntity { Id = a.Adm.Department.Id, Code = a.Adm.Department.Code, Name = a.Adm.Department.Name } : null),
                   Division = ((a.Adm.Division != null && a.Adm.Division != null) ? new Dto.CodeNameEntity { Id = a.Adm.Division.Id, Code = a.Adm.Division.Code, Name = a.Adm.Division.Name } : null),
                   Administration = (a.Adm.Administration != null ? new Dto.CodeNameEntity { Id = a.Adm.Administration.Id, Code = a.Adm.Administration.Code, Name = a.Adm.Administration.Name } : null),
                   Room = (a.Adm.Room != null ? new Dto.CodeNameEntity { Id = a.Adm.Room.Id, Code = a.Adm.Room.Code, Name = a.Adm.Room.Name } : null),
                   City = ((a.Adm.Room != null && a.Adm.Room.Location != null && a.Adm.Room.Location.City != null) ? new Dto.CodeNameEntity { Id = a.Adm.Room.Location.City.Id, Code = a.Adm.Room.Location.City.Code, Name = a.Adm.Room.Location.City.Name } : null),
                   Country = ((a.Adm.Room != null && a.Adm.Room.Location != null && a.Adm.Room.Location.City != null && a.Adm.Room.Location.City.County != null && a.Adm.Room.Location.City.County.Country != null) ? new Dto.CodeNameEntity { Id = a.Adm.Room.Location.City.County.Country.Id, Code = a.Adm.Room.Location.City.County.Country.Code, Name = a.Adm.Room.Location.City.County.Country.Name } : null),
                   BudgetManager = (a.Adm.BudgetManager != null ? new Dto.CodeNameEntity { Id = a.Adm.BudgetManager.Id, Code = a.Adm.BudgetManager.Code, Name = a.Adm.BudgetManager.Name } : null),
                   AssetNature = (a.Adm.AssetNature != null ? new Dto.CodeNameEntity { Id = a.Adm.AssetNature.Id, Code = a.Adm.AssetNature.Code, Name = a.Adm.AssetNature.Name } : null),
                   Type = (a.Adm.SubType != null && a.Adm.SubType.Type != null ? new Dto.CodeNameEntity { Id = a.Adm.SubType.Type.Id, Code = a.Adm.SubType.Type.Code, Name = a.Adm.SubType.Type.Name } : null),
                   SubType = (a.Adm.SubType != null ? new Dto.CodeNameEntity { Id = a.Adm.SubType.Id, Code = a.Adm.SubType.Code, Name = a.Adm.SubType.Name } : null),
                   Employee = (a.Adm.Employee != null ? new Dto.EmployeeResource { Id = a.Adm.Employee.Id, InternalCode = a.Adm.Employee.InternalCode, FirstName = a.Adm.Employee.FirstName, LastName = a.Adm.Employee.LastName, ErpCode = a.Adm.Employee.ERPCode, IsDeleted = a.Adm.Employee.IsDeleted } : null),
                   AdmCenter = (a.Adm.CostCenter != null && a.Adm.CostCenter.AdmCenter != null ? new Dto.CodeNameEntity { Id = a.Adm.CostCenter.AdmCenter.Id, Code = a.Adm.CostCenter.AdmCenter.Code, Name = a.Adm.CostCenter.AdmCenter.Name } : null),
                   Region = ((a.Adm.CostCenter != null && a.Adm.CostCenter.AdmCenter != null && a.Adm.CostCenter.Region != null) ? new Dto.CodeNameEntity { Id = a.Adm.CostCenter.Region.Id, Code = a.Adm.CostCenter.Region.Code, Name = a.Adm.CostCenter.Region.Name } : null),
                   Location = ((a.Adm.Room != null && a.Adm.Room.Location != null) ? new Dto.CodeNameEntity { Id = a.Adm.Room.LocationId, Code = a.Adm.Room.Location.Code, Name = a.Adm.Room.Location.Name } : null),
                   InsuranceCategory = (a.Adm.InsuranceCategory != null ? new Dto.CodeNameEntity { Id = a.Adm.InsuranceCategory.Id, Code = a.Adm.InsuranceCategory.Code, Name = a.Adm.InsuranceCategory.Name } : null),
                   Project = (a.Adm.Project != null ? new Dto.CodeNameEntity { Id = a.Adm.Project.Id, Code = a.Adm.Project.Code, Name = a.Adm.Project.Name } : null),
                   AssetType = (a.Adm.AssetType != null ? new Dto.CodeNameEntity { Id = a.Adm.AssetType.Id, Code = a.Adm.AssetType.Code, Name = a.Adm.AssetType.Name } : null),
                   AssetState = (a.Adm.AssetState != null ? new Dto.CodeNameEntity { Id = a.Adm.AssetState.Id, Code = a.Adm.AssetState.Code, Name = a.Adm.AssetState.Name } : null),
                   AssetClass = (a.Adm.AssetClass != null ? new Dto.CodeNameEntity { Id = a.Adm.AssetClass.Id, Code = a.Adm.AssetClass.Code, Name = a.Adm.AssetClass.Name } : null),


                   //Company = (a.Adm.Company != null ? new Dto.CodeNameEntity { Id = a.Adm.Company.Id, Code = a.Adm.Company.Code, Name = a.Adm.Company.Name } : null),



                   //AdmCenter = ((a.Adm.Room != null && a.Adm.Room.Location != null && a.Adm.Room.Location.AdmCenter != null) ? new Dto.CodeNameEntity { Id = a.Adm.Room.Location.AdmCenter.Id, Code = a.Adm.Room.Location.AdmCenter.Code, Name = a.Adm.Room.Location.AdmCenter.Name } : null),
                   //MasterType = (a.Adm.SubType != null && a.Adm.SubType.Type != null && a.Adm.SubType.Type.MasterType != null ? new Dto.CodeNameEntity { Id = a.Adm.SubType.Type.MasterType.Id, Code = a.Adm.SubType.Type.MasterType.Code, Name = a.Adm.SubType.Type.MasterType.Name } : null),


                   //Model = (a.Adm.Model != null ? new Dto.CodeNameEntity { Id = a.Adm.Model.Id, Code = a.Adm.Model.Code, Name = a.Adm.Model.Name } : null),
                   //Brand = (a.Adm.Brand != null ? new Dto.CodeNameEntity { Id = a.Adm.Brand.Id, Code = a.Adm.Brand.Code, Name = a.Adm.Brand.Name } : null),




                   //Region = (a.Adm.Room.Location.Region != null ? new Dto.CodeNameEntity { Id = a.Adm.Room.Location.Region.Id, Code = a.Adm.Room.Location.Region.Code, Name = a.Adm.Room.Location.Region.Name } : null),


               }))
               .ForMember(a => a.Dep, opt => opt.MapFrom(a => new Dto.AssetDepDetail
               {

				   UsefulLife = a.Dep.UsefulLife,
				   ExpLifeInPeriods = a.Dep.ExpLifeInPeriods,
				   RemLifeInPeriods = a.Dep.RemLifeInPeriods,
				   TotLifeInpPeriods = a.Dep.TotLifeInpPeriods,
				   DepFYStart = a.Dep.DepFYStart,
				   APCFYStart = a.Dep.APCFYStart,
				   BkValFYStart = a.Dep.BkValFYStart,
				   CurrBkValue = a.Dep.CurrBkValue,
				   CurrentAPC = a.Dep.CurrentAPC,
				   DepRetirement = a.Dep.DepRetirement,
				   DepTransfer = a.Dep.DepTransfer,
				   Acquisition = a.Dep.Acquisition,
				   Retirement = a.Dep.Retirement,
				   Transfer = a.Dep.Transfer,
				   PosCap = a.Dep.PosCap,
				   DepPostCap = a.Dep.DepPostCap,
				   DepForYear = a.Dep.DepForYear,
				   AccumulDep = a.Dep.AccumulDep,
				   InvestSupport = a.Dep.InvestSupport,
				   WriteUps = a.Dep.WriteUps,
				   AccSystem = (a.Dep.AccSystem != null ? new Model.AccSystem { Id = a.Dep.AccSystem.Id, Code = a.Dep.AccSystem.Code, Name = a.Dep.AccSystem.Name } : null)
               }
               ))
               ;

            CreateMap<Model.Asset, Dto.Asset>()
                .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Id))
                .ForMember(a => a.InvNo, opt => opt.MapFrom(a => a.InvNo))
                .ForMember(a => a.SubNo, opt => opt.MapFrom(a => a.SubNo))
                .ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Quantity))
                .ForMember(a => a.InSapValidation, opt => opt.MapFrom(a => a.InSapValidation))
                  //.ForMember(a => a.License, opt => opt.MapFrom(a => a.License))
                  //.ForMember(a => a.Invoice, opt => opt.MapFrom(a => a.Invoice))
                .ForMember(a => a.SerialNumber, opt => opt.MapFrom(a => a.SerialNumber))
                .ForMember(a => a.AgreementNo, opt => opt.MapFrom(a => a.AgreementNo))
                .ForMember(a => a.Manufacturer, opt => opt.MapFrom(a => a.Manufacturer))
                .ForMember(a => a.SAPCode, opt => opt.MapFrom(a => a.SAPCode))
                .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Name))
               
                .ForMember(a => a.PurchaseDate, opt => opt.MapFrom(a => a.PurchaseDate))
                .ForMember(a => a.InvoiceDate, opt => opt.MapFrom(a => a.InvoiceDate))
                .ForMember(a => a.RemovalDate, opt => opt.MapFrom(a => a.RemovalDate))
                .ForMember(a => a.ReceptionDate, opt => opt.MapFrom(a => a.ReceptionDate))
				.ForMember(a => a.PODate, opt => opt.MapFrom(a => a.PODate))
                .ForMember(a => a.ERPCode, opt => opt.MapFrom(a => a.ERPCode))
				.ForMember(a => a.RateValue, opt => opt.MapFrom(a => a.Rate != null ? a.Rate.Value : 0))
				.ForMember(a => a.Document, opt => opt.MapFrom(
                    a => new Dto.DocumentMainDetail
                    {
                        Id = a.Document.Id,
                        DocNo1 = a.Document.DocNo1,
                        DocNo2 = a.Document.DocNo2,
                        Details = a.Document.Details,
                        DocumentDate = a.Document.DocumentDate,
                        RegisterDate = a.Document.RegisterDate,
                        CreationDate = a.Document.CreationDate,
                        DocumentType = new Dto.CodeNameEntity
                        {
                            Id = a.Document.DocumentTypeId,
                            Code = a.Document.DocumentType.Code,
                            Name = a.Document.DocumentType.Name
                        }
                        ,
                        Partner = new Dto.Partner
                        {
                            Id = a.Document.Partner.Id,
                            Name = a.Document.Partner.Name,
                            FiscalCode = a.Document.Partner.FiscalCode,
                            RegistryNumber = a.Document.Partner.RegistryNumber,
                            ErpCode = a.Document.Partner.ErpCode,
                            PartnerLocation = new Dto.PartnerLocation()
							{
                                Cui = a.Document.Partner.PartnerLocation.Cui,
                                Data = a.Document.Partner.PartnerLocation.Data,
                                Denumire = a.Document.Partner.PartnerLocation.Denumire,
                                Adresa = a.Document.Partner.PartnerLocation.Adresa,
                                NrRegCom = a.Document.Partner.PartnerLocation.NrRegCom,
                                Telefon = a.Document.Partner.PartnerLocation.Telefon,
                                Fax = a.Document.Partner.PartnerLocation.Fax,
                                CodPostal = a.Document.Partner.PartnerLocation.CodPostal,
                                Act = a.Document.Partner.PartnerLocation.Act,
                                Stare_inregistrare = a.Document.Partner.PartnerLocation.Stare_inregistrare,
                                ScpTVA = a.Document.Partner.PartnerLocation.ScpTVA,
                                Data_inceput_ScpTVA = a.Document.Partner.PartnerLocation.Data_inceput_ScpTVA,
                                Data_sfarsit_ScpTVA = a.Document.Partner.PartnerLocation.Data_sfarsit_ScpTVA,
                                Data_anul_imp_ScpTVA = a.Document.Partner.PartnerLocation.Data_anul_imp_ScpTVA,
                                Mesaj_ScpTVA = a.Document.Partner.PartnerLocation.Mesaj_ScpTVA,
                                DataInceputTvaInc = a.Document.Partner.PartnerLocation.DataInceputTvaInc,
                                DataSfarsitTvaInc = a.Document.Partner.PartnerLocation.DataSfarsitTvaInc,
                                DataActualizareTvaInc = a.Document.Partner.PartnerLocation.DataActualizareTvaInc,
                                DataPublicareTvaInc = a.Document.Partner.PartnerLocation.DataPublicareTvaInc,
                                TipActTvaInc = a.Document.Partner.PartnerLocation.TipActTvaInc,
                                StatusTvaIncasare = a.Document.Partner.PartnerLocation.StatusTvaIncasare,
                                DataInactivare = a.Document.Partner.PartnerLocation.DataInactivare,
                                DataReactivare = a.Document.Partner.PartnerLocation.DataReactivare,
                                DataPublicare = a.Document.Partner.PartnerLocation.DataPublicare,
                                DataRadiere = a.Document.Partner.PartnerLocation.DataRadiere,
                                StatusInactivi = a.Document.Partner.PartnerLocation.StatusInactivi,
                                DataInceputSplitTVA = a.Document.Partner.PartnerLocation.DataInceputSplitTVA,
                                DataAnulareSplitTVA = a.Document.Partner.PartnerLocation.DataAnulareSplitTVA,
                                Iban = a.Document.Partner.PartnerLocation.Iban,
                                StatusRO_e_Factura = a.Document.Partner.PartnerLocation.StatusRO_e_Factura
                            }

                        }
                    }))
                 .ForMember(a => a.Inv, opt => opt.MapFrom(
                    a => new Dto.AssetInv
                    {
                        Model = a.AssetInv.Model,
                        Producer = a.AssetInv.Producer
                    }))
                 .ForMember(a => a.EmployeeTransfer, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.EmployeeTransfer.Id, InternalCode = a.EmployeeTransfer.InternalCode, FirstName = a.EmployeeTransfer.FirstName, LastName = a.EmployeeTransfer.LastName, Email = a.EmployeeTransfer.Email }))
                 .ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Company.Id, Code = a.Company.Code, Name = a.Company.Name }))
                 .ForMember(a => a.Material, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Material.Id, Code = a.Material.Code, Name = a.Material.Name }))
                 .ForMember(a => a.Tax, opt => opt.MapFrom(a => new Dto.Tax { Id = a.Tax.Id, Code = a.Tax.Code, Name = a.Tax.Name, Value = a.Tax.Value }))
                 .ForMember(a => a.Request, opt => opt.MapFrom(a => new Dto.Request { Id = a.Request.Id, Code = a.Request.Code, Name = a.Request.Name }))
                 .ForMember(a => a.Rate, opt => opt.MapFrom(a => new Dto.Rate { Id = a.Tax.Id, Code = a.Rate.Code, Value = a.Rate.Value, Uom = new Dto.CodeNameEntity { Id = a.Rate.Uom.Id, Code = a.Rate.Uom.Code, Name= a.Rate.Uom.Name} }))
                 .ForMember(a => a.InvState, opt => opt.MapFrom(a => new Dto.InvState { Id = a.InvState.Id, Code = a.InvState.Code, Name = a.InvState.Name, BadgeColor = a.InvState.BadgeColor, BadgeIcon = a.InvState.BadgeIcon }))
                 .ForMember(a => a.DictionaryItem, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.DictionaryItem.Id, Code = a.DictionaryItem.Code, Name = a.DictionaryItem.Name }))
                 .ForMember(a => a.Uom, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Uom.Id, Code = a.Uom.Code, Name = a.Uom.Name }))
                  //.ForMember(a => a.ErrorRetire, opt => opt.MapFrom(a => a.Error.Where(a => a.ErrorType.Code == "RETIREASSET" && a.IsDeleted == false).OrderByDescending(a => a.Id).Take(1)))
                  .ForMember(i => i.ErrorRetire, opt => opt.MapFrom(i => i.Error != null ?  i.Error.Where(p => p.IsDeleted == false && p.ErrorType.Code == "RETIREASSET").OrderByDescending(a => a.Id).Take(1).Select(p => new Dto.Error()
                  {
                      Id = p.Id,
                      BUKRS = p.BUKRS,
                      BELNR = p.BELNR,
                      GJAHR = p.GJAHR,
                      IsDeleted = p.IsDeleted,
                      Code = p.Code,
                      Name = p.Name
                  }) : null))
                 .ForMember(i => i.LastAcquisition, opt => opt.MapFrom(i => i.Error != null ?  i.Error.Where(p => p.IsDeleted == false && p.ErrorType.Code == "ACQUISITIONASSET").OrderByDescending(a => a.Id).Take(1).Select(p => new Dto.Error()
                 {
                     Id = p.Id,
                     BUKRS = p.BUKRS,
                     BELNR = p.BELNR,
                     GJAHR = p.GJAHR,
                     IsDeleted = p.IsDeleted,
                     Code = p.Code,
                     Name = p.Name
                 }) : null))
                 .ForMember(i => i.BudgetForecast, opt => opt.MapFrom(i => new Dto.BudgetForecast
                 {
                     Id = i.BudgetForecast.Id,
                     Code = i.BudgetForecast.BudgetBase.Code,
                     Total = i.BudgetForecast.Total,
                     TotalRem = i.BudgetForecast.TotalRem,
					 BudgetBase = new Dto.BudgetBase
					 {
						 Id = i.BudgetForecast.BudgetBase.Id,
						 Code = i.BudgetForecast.BudgetBase.Code,
						 Info = i.BudgetForecast.BudgetBase.Info
					 }
				 }))
                 .ForMember(a => a.Order, opt => opt.MapFrom(a => new Dto.Order { Id = a.Order.Id, Code = a.Order.Code, Name = a.Order.Name }))
                 .ForMember(a => a.Adm, opt => opt.MapFrom(a => new Dto.AssetAdmDetail
                 {
                    
                    Account = (a.Account != null ? new Dto.CodeNameEntity { Id = a.Account.Id, Code = a.Account.Code, Name = a.Account.Name } : null),
                    ExpAccount = (a.ExpAccount != null ? new Dto.CodeNameEntity { Id = a.ExpAccount.Id, Code = a.ExpAccount.Code, Name = a.ExpAccount.Name } : null),
                    AssetCategory = (a.AssetCategory != null ? new Dto.CodeNameEntity { Id = a.AssetCategory.Id, Code = a.AssetCategory.Code, Name = a.AssetCategory.Name } : null),
                    Article = (a.Article != null ? new Dto.CodeNameEntity { Id = a.Article.Id, Code = a.Article.Code, Name = a.Article.Name } : null),
                    CostCenter = (a.CostCenter != null ? new Dto.CodeNameEntity { Id = a.CostCenter.Id, Code = a.CostCenter.Code, Name = a.CostCenter.Name } : null),
                    Department = (a.CostCenter != null && a.Division != null && a.Department != null ? new Dto.CodeNameEntity { Id = a.CostCenter.Division.Department.Id, Code = a.CostCenter.Division.Department.Code, Name = a.CostCenter.Division.Department.Name } : null),
                    Division = (a.CostCenter != null &&  a.Division != null ? new Dto.CodeNameEntity { Id = a.CostCenter.Division.Id, Code = a.CostCenter.Division.Code, Name = a.CostCenter.Division.Name } : null),
                    Room = (a.CostCenter != null && a.CostCenter.Room != null ? new Dto.CodeNameEntity { Id = a.CostCenter.Room.Id, Code = a.CostCenter.Room.Code, Name = a.CostCenter.Room.Name } : null),
                    Administration = (a.CostCenter != null && a.CostCenter.Administration != null ? new Dto.CodeNameEntity { Id = a.CostCenter.Administration.Id, Code = a.CostCenter.Administration.Code, Name = a.CostCenter.Administration.Name } : null),
                    Location = ((a.CostCenter != null && a.CostCenter.Room != null && a.CostCenter.Room.Location != null) ? new Dto.CodeNameEntity { Id = a.CostCenter.Room.LocationId, Code = a.CostCenter.Room.Location.Code, Name = a.CostCenter.Room.Location.Name } : null),
                    City = ((a.CostCenter != null && a.CostCenter.Room != null && a.CostCenter.Room.Location != null && a.CostCenter.Room.Location.City != null) ? new Dto.CodeNameEntity { Id = a.CostCenter.Room.Location.City.Id, Code = a.CostCenter.Room.Location.City.Code, Name = a.CostCenter.Room.Location.City.Name } : null),
                    County = ((a.CostCenter != null && a.CostCenter.Room != null && a.CostCenter.Room.Location != null && a.CostCenter.Room.Location.City != null && a.CostCenter.Room.Location.City.County !=null) ? new Dto.CodeNameEntity { Id = a.CostCenter.Room.Location.City.County.Id, Code = a.CostCenter.Room.Location.City.County.Code, Name = a.CostCenter.Room.Location.City.County.Name } : null),
                    Country = ((a.CostCenter != null && a.CostCenter.Room != null && a.CostCenter.Room.Location != null && a.CostCenter.Room.Location.City != null && a.CostCenter.Room.Location.City.County != null && a.CostCenter.Room.Location.City.County.Country !=null) ? new Dto.CodeNameEntity { Id = a.CostCenter.Room.Location.City.County.Country.Id, Code = a.CostCenter.Room.Location.City.County.Country.Code, Name = a.CostCenter.Room.Location.City.County.Country.Name } : null),
                    BudgetManager = (a.BudgetManager != null ? new Dto.CodeNameEntity { Id = a.BudgetManager.Id, Code = a.BudgetManager.Code, Name = a.BudgetManager.Name } : null),
                    AssetNature = (a.AssetNature != null ? new Dto.CodeNameEntity { Id = a.AssetNature.Id, Code = a.AssetNature.Code, Name = a.AssetNature.Name } : null),
                    Type = (a.SubType != null && a.SubType.Type != null ? new Dto.CodeNameEntity { Id = a.SubType.Type.Id, Code = a.SubType.Type.Code, Name = a.SubType.Type.Name } : null),
                    SubType = (a.SubType != null ? new Dto.CodeNameEntity { Id = a.SubType.Id, Code = a.SubType.Code, Name = a.SubType.Name } : null),
                    Employee = (a.Employee != null ? new Dto.EmployeeResource { Id = a.Employee.Id, InternalCode = a.Employee.InternalCode, FirstName = a.Employee.FirstName, LastName = a.Employee.LastName, Email = a.Employee.Email } : null),
                    //AssetClass = (a.AssetAdmMDs.SingleOrDefault().AssetClass != null ? new Dto.CodeNameEntity { Id = a.AssetAdmMDs.SingleOrDefault().AssetClass.Id, Code = a.AssetAdmMDs.SingleOrDefault().AssetClass.Code, Name = a.AssetAdmMDs.SingleOrDefault().AssetClass.Name } : null),
                    AdmCenter = ((a.CostCenter != null && a.CostCenter.AdmCenter != null) ? new Dto.CodeNameEntity { Id = a.CostCenter.AdmCenterId.Value, Code = a.CostCenter.AdmCenter.Code, Name = a.CostCenter.AdmCenter.Name } : null),
                    Region = (a.CostCenter != null && a.CostCenter.Region != null ? new Dto.CodeNameEntity { Id = a.CostCenter.Region.Id, Code = a.CostCenter.Region.Code, Name = a.CostCenter.Region.Name } : null),
                    Company = (a.Company != null ? new Dto.CodeNameEntity { Id = a.Company.Id, Code = a.Company.Code, Name = a.Company.Name } : null),
                    InsuranceCategory = (a.InsuranceCategory != null ? new Dto.CodeNameEntity { Id = a.InsuranceCategory.Id, Code = a.InsuranceCategory.Code, Name = a.InsuranceCategory.Name } : null),
                    AssetType = (a.AssetType != null ? new Dto.CodeNameEntity { Id = a.AssetType.Id, Code = a.AssetType.Code, Name = a.AssetType.Name } : null),
                    Project = (a.Project != null ? new Dto.CodeNameEntity { Id = a.Project.Id, Code = a.Project.Code, Name = a.Project.Name } : null),
                    AssetState = (a.AssetState != null ? new Dto.CodeNameEntity { Id = a.AssetState.Id, Code = a.AssetState.Code, Name = a.AssetState.Name } : null),
					Model = (a.Model != null ? new Dto.Model { Id = a.Model.Id, Code = a.Model.Code, Name = a.Model.Name } : null),
					Brand = (a.Brand != null ? new Dto.Brand { Id = a.Brand.Id, Code = a.Brand.Code, Name = a.Brand.Name } : null),
                    //AssetClass = (a.AssetAdmMDs.SingleOrDefault() != null ? new Dto.CodeNameEntity { Id = a.AssetAdmMDs.SingleOrDefault().AssetClass.Id, Code = a.AssetAdmMDs.SingleOrDefault().AssetClass.Code, Name = a.AssetAdmMDs.SingleOrDefault().AssetClass.Name } : null),
                    //InvState = (a.InvState != null ? new Dto.CodeNameEntity { Id = a.InvState.Id, Code = a.InvState.Code, Name = a.InvState.Name } : null),

                }))
               .ForMember(a => a.Dep, opt => opt.MapFrom(a => new Dto.AssetDepDetail
               {

                   UsefulLife = a.AssetDeps.SingleOrDefault().DepPeriodRemIn,
                   ExpLifeInPeriods = a.AssetDeps.SingleOrDefault().DepPeriod,
                   RemLifeInPeriods = a.AssetDeps.SingleOrDefault().DepPeriodMonth,
                   TotLifeInpPeriods = a.AssetDeps.SingleOrDefault().DepPeriodRem,
                   DepFYStart = a.AssetDeps.SingleOrDefault().ValueInvIn,
                   APCFYStart = a.AssetDeps.SingleOrDefault().ValueDepIn,
                   BkValFYStart = a.AssetDeps.SingleOrDefault().ValueDepPU,
                   CurrBkValue = a.AssetDeps.SingleOrDefault().ValueDepYTDIn,
                   CurrentAPC = a.AssetDeps.SingleOrDefault().ValueDepYTD,
                   DepRetirement = a.AssetDeps.SingleOrDefault().ValueRet,
                   DepTransfer = a.AssetDeps.SingleOrDefault().ValueRetIn,
                   Acquisition = a.AssetDeps.SingleOrDefault().ValueDepPUIn,
                   Retirement = a.AssetDeps.SingleOrDefault().ValueTr,
                   Transfer = a.AssetDeps.SingleOrDefault().ValueTrIn,
                   PosCap = a.AssetDeps.SingleOrDefault().ValueRem,
                   DepPostCap = a.AssetDeps.SingleOrDefault().ValueRemIn,
                   DepForYear = a.AssetDeps.SingleOrDefault().ValueInv,
                   AccumulDep = a.AssetDeps.SingleOrDefault().ValueDep,
                   InvestSupport = a.AssetDepMDs.SingleOrDefault().InvestSupport,
                   WriteUps = a.AssetDepMDs.SingleOrDefault().WriteUps,
                   //AssetClass = a.AssetAdmMDs.SingleOrDefault().AssetClass,


               }));

            CreateMap<Model.BudgetDetail, Dto.Budget>()
             .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Budget.Id))
             .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.Budget.IsAccepted))
             .ForMember(a => a.Code, opt => opt.MapFrom(a => a.Budget.Code))
             .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Budget.Name))
             .ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Budget.Quantity))
             .ForMember(a => a.QuantityRem, opt => opt.MapFrom(a => a.Budget.QuantityRem))
             .ForMember(a => a.InfoMinus, opt => opt.MapFrom(a => a.Budget.Name))
             .ForMember(a => a.ValueIni, opt => opt.MapFrom(a => a.Budget.ValueIni))
             .ForMember(a => a.ValueFin, opt => opt.MapFrom(a => a.Budget.ValueFin))
             .ForMember(a => a.Info, opt => opt.MapFrom(a => a.Budget.Info))
             .ForMember(a => a.DepPeriod, opt => opt.MapFrom(a => a.Budget.DepPeriod))
             .ForMember(a => a.DepPeriodRem, opt => opt.MapFrom(a => a.Budget.DepPeriodRem))
             .ForMember(a => a.Total, opt => opt.MapFrom(a => a.Budget.Total))
             .ForMember(a => a.AppStateId, opt => opt.MapFrom(a => a.Budget.AppStateId))
             .ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Company.Id, Code = a.Budget.Company.Code, Name = a.Budget.Company.Name }))
             .ForMember(a => a.Project, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Project.Id, Code = a.Budget.Project.Code, Name = a.Budget.Project.Name }))
             //.ForMember(a => a.Administration, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Administration.Id, Code = a.Budget.Administration.Code, Name = a.Budget.Administration.Name }))
             //.ForMember(a => a.MasterType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.SubType.Type.MasterType.Id, Code = a.Budget.SubType.Type.MasterType.Code, Name = a.Budget.SubType.Type.MasterType.Name }))
             //.ForMember(a => a.Type, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.SubType.Type.Id, Code = a.Budget.SubType.Type.Code, Name = a.Budget.SubType.Type.Name }))
             //.ForMember(a => a.SubType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.SubType.Id, Code = a.Budget.SubType.Code, Name = a.Budget.SubType.Name }))
             .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Budget.Employee.Id, InternalCode = a.Budget.Employee.InternalCode, FirstName = a.Budget.Employee.FirstName, LastName = a.Budget.Employee.LastName }))
             .ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.Budget.AccMonth.Id, Year = a.Budget.AccMonth.Year }))
             //.ForMember(a => a.InterCompany, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.InterCompany.Id, Code = a.Budget.InterCompany.Code, Name = a.Budget.InterCompany.Name }))
             //.ForMember(a => a.Partner, opt => opt.MapFrom(a => new Dto.CodePartnerEntity { Id = a.Budget.Partner.Id, Name = a.Budget.Partner.Name }))
             //.ForMember(a => a.Account, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Account.Id, Code = a.Budget.Account.Code, Name = a.Budget.Account.Name }))
             .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.AppState.Id, Code = a.Budget.AppState.Code, Name = a.Budget.AppState.Name }))
             //.ForMember(a => a.CostCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.CostCenter.Id, Code = a.Budget.CostCenter.Code, Name = a.Budget.CostCenter.Name }))
             //.ForMember(a => a.BudgetManager, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.BudgetManager.Id, Code = a.Budget.BudgetManager.Code, Name = a.Budget.BudgetManager.Name }))
             .ForMember(a => a.Country, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Country.Id, Code = a.Budget.Country.Code, Name = a.Budget.Country.Name }))
             .ForMember(a => a.Activity, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Activity.Id, Code = a.Budget.Activity.Code, Name = a.Budget.Activity.Name }))
             .ForMember(a => a.Region, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Region.Id, Code = a.Budget.Region.Code, Name = a.Budget.Region.Name }))
             .ForMember(a => a.AdmCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.AdmCenter.Id, Code = a.Budget.AdmCenter.Code, Name = a.Budget.AdmCenter.Name }))
             .ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.AssetType.Id, Code = a.Budget.AssetType.Code, Name = a.Budget.AssetType.Name }))
             .ForMember(a => a.ProjectType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.ProjectType.Id, Code = a.Budget.ProjectType.Code, Name = a.Budget.ProjectType.Name }))
             .ForMember(i => i.BudgetMonths, opt => opt.MapFrom(i => i.Budget.BudgetMonths.Select(p => new Dto.BudgetMonth()
              {
                  Id = p.Id,
                  BudgetId = p.BudgetId,
                  //Budget = new Dto.Budget() { Month = p.AccMonth != null ? p.AccMonth.Month : 0, Year = p.AccMonth != null ? p.AccMonth.Year : 0 },
                  Value = p.Value,
                  ValueDep = p.ValueDep,
                  AccMonth = new Dto.AccMonth() { Month = p.AccMonth != null ? p.AccMonth.Month : 0, Year = p.AccMonth != null ? p.AccMonth.Year : 0 },
                  //ValueTotal = p.ValueTotal,
                  //ValueDepTotal = p.ValueDepTotal
              })));


            CreateMap<Model.BudgetDetail, Dto.BudgetTree>()
            .ForMember(i => i.Data, opt => opt.MapFrom(i => new Dto.CodeNameEntity()
            {
                Id = i.Budget.Id,
                Code = i.Budget.Code,
                Name = i.Budget.Name
            }))
            .ForMember(i => i.Children, opt => opt.MapFrom(i => i.Budget.BudgetMonths.Select(p => new Dto.CodeNameEntity()
            {
                Id = i.Budget.Id,
                Code = i.Budget.Code,
                Name = i.Budget.Name
            })))
             .ForMember(i => i.BudgetMonths, opt => opt.MapFrom(i => i.Budget.BudgetMonths.Select(p => new Dto.BudgetMonth()
             {
                 Id = p.Id,
                 BudgetId = p.BudgetId,
                 //Budget = new Dto.Budget() { Month = p.AccMonth != null ? p.AccMonth.Month : 0, Year = p.AccMonth != null ? p.AccMonth.Year : 0 },
                 Value = p.Value,
                 ValueDep = p.ValueDep,
                 AccMonth = new Dto.AccMonth() { Month = p.AccMonth != null ? p.AccMonth.Month : 0, Year = p.AccMonth != null ? p.AccMonth.Year : 0 },
                 //ValueTotal = p.ValueTotal,
                 //ValueDepTotal = p.ValueDepTotal
             })));

            // CreateMap<Model.Budget, Dto.Budget>();
            CreateMap<Model.BudgetMonth, Dto.BudgetMonth>();
            //.ForMember(i => i.BudgetMonth, opt => opt.MapFrom(i => i.BudgetMonths.Select(p => new Dto.BudgetMonth()
            //{
            //    Value = p.Value,
            //    ValueDep = p.ValueDep,
            //    AccMonth = new Dto.AccMonth() { Month = p.AccMonth != null ? p.AccMonth.Month : 0, Year = p.AccMonth != null ? p.AccMonth.Year : 0 },
            //})));

            CreateMap<Model.BudgetDetail, Dto.BudgetUI>()
             .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Budget.Id))
             //.ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.Budget.IsAccepted))
             .ForMember(a => a.Code, opt => opt.MapFrom(a => a.Budget.Code))
             .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Budget.Name))
           //.ForMember(a => a.InfoMinus, opt => opt.MapFrom(a => a.Budget.Name))
            .ForMember(a => a.ValueIni, opt => opt.MapFrom(a => a.Budget.ValueIni))
            .ForMember(a => a.ValueFin, opt => opt.MapFrom(a => a.Budget.ValueFin))
            .ForMember(a => a.ValueUsed, opt => opt.MapFrom(a => a.Budget.ValueUsed))
             .ForMember(a => a.ValueOrder, opt => opt.MapFrom(a => a.Budget.ValueOrder))
            .ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Budget.Quantity))
            .ForMember(a => a.QuantityRem, opt => opt.MapFrom(a => a.Budget.QuantityRem))
            .ForMember(a => a.QuantityUsed, opt => opt.MapFrom(a => a.Budget.QuantityUsed))
             .ForMember(a => a.QuantityOrder, opt => opt.MapFrom(a => a.Budget.QuantityOrder))
             //.ForMember(a => a.Info, opt => opt.MapFrom(a => a.Budget.Info))
             .ForMember(a => a.AppStateId, opt => opt.MapFrom(a => a.Budget.AppStateId))
             .ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Company.Id, Code = a.Budget.Company.Code, Name = a.Budget.Company.Name }))
             .ForMember(a => a.Uom, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Uom.Id, Code = a.Budget.Uom.Code, Name = a.Budget.Uom.Name }))
             .ForMember(a => a.Project, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Project.Id, Code = a.Budget.Project.Code, Name = a.Budget.Project.Name }))
             .ForMember(a => a.AdmCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.AdmCenter.Id, Code = a.Budget.AdmCenter.Code, Name = a.Budget.AdmCenter.Name }))
             .ForMember(a => a.Region, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Region.Id, Code = a.Budget.Region.Code, Name = a.Budget.Region.Name }))
             .ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.AssetType.Id, Code = a.Budget.AssetType.Code, Name = a.Budget.AssetType.Name }))
             //.ForMember(a => a.Administration, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Administration.Id, Code = a.Budget.Administration.Code, Name = a.Budget.Administration.Name }))
             //.ForMember(a => a.MasterType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.SubType.Type.MasterType.Id, Code = a.Budget.SubType.Type.MasterType.Code, Name = a.Budget.SubType.Type.MasterType.Name }))
             //.ForMember(a => a.Type, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.SubType.Type.Id, Code = a.Budget.SubType.Type.Code, Name = a.Budget.SubType.Type.Name }))
             //.ForMember(a => a.SubType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.SubType.Id, Code = a.Budget.SubType.Code, Name = a.Budget.SubType.Name }))
             //.ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Budget.Employee.Id, InternalCode = a.Budget.Employee.InternalCode, FirstName = a.Budget.Employee.FirstName, LastName = a.Budget.Employee.LastName }))
             //.ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.Budget.AccMonth.Id, Year = a.Budget.AccMonth.Year.ToString() }))
             //.ForMember(a => a.InterCompany, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.InterCompany.Id, Code = a.Budget.InterCompany.Code, Name = a.Budget.InterCompany.Name }))
             //.ForMember(a => a.Partner, opt => opt.MapFrom(a => new Dto.CodePartnerEntity { Id = a.Budget.Partner.Id, Name = a.Budget.Partner.Name }))
             //.ForMember(a => a.Account, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Account.Id, Code = a.Budget.Account.Code, Name = a.Budget.Account.Name }))
             .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.AppState.Id, Code = a.Budget.AppState.Code, Name = a.Budget.AppState.Name }));
            //.ForMember(a => a.CostCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.CostCenter.Id, Code = a.Budget.CostCenter.Code, Name = a.Budget.CostCenter.Name }));



            CreateMap<Model.Budget, Dto.Budget>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Id))
            .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.IsAccepted))
            .ForMember(a => a.Code, opt => opt.MapFrom(a => a.Code))
            .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Name))
            .ForMember(a => a.QuantityRem, opt => opt.MapFrom(a => a.QuantityRem))
            .ForMember(a => a.ValueIni, opt => opt.MapFrom(a => a.ValueIni))
            .ForMember(a => a.ValueFin, opt => opt.MapFrom(a => a.ValueFin))
            .ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Company.Id, Code = a.Company.Code, Name = a.Company.Name }))
            .ForMember(a => a.Project, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Project.Id, Code = a.Project.Code, Name = a.Project.Name }))
            .ForMember(a => a.AdmCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.AdmCenter.Id, Code = a.AdmCenter.Code, Name = a.AdmCenter.Name }))
            .ForMember(a => a.Region, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Region.Id, Code = a.Region.Code, Name = a.Region.Name }))
            .ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.AssetType.Id, Code = a.AssetType.Code, Name = a.AssetType.Name }))
            //.ForMember(a => a.Administration, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Administration.Id, Code = a.Administration.Code, Name = a.Administration.Name }))
            //.ForMember(a => a.MasterType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.SubType.Type.MasterType.Id, Code = a.SubType.Type.MasterType.Code, Name = a.SubType.Type.MasterType.Name }))
            //.ForMember(a => a.Type, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.SubType.Type.Id, Code = a.SubType.Type.Code, Name = a.SubType.Type.Name }))
            //.ForMember(a => a.SubType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.SubType.Id, Code = a.SubType.Code, Name = a.SubType.Name }))
            .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Employee.Id, InternalCode = a.Employee.InternalCode, FirstName = a.Employee.FirstName, LastName = a.Employee.LastName }))
            .ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.AccMonth.Id, Year = a.AccMonth.Year }))
            //.ForMember(a => a.InterCompany, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.InterCompany.Id, Code = a.InterCompany.Code, Name = a.InterCompany.Name }))
            //.ForMember(a => a.Partner, opt => opt.MapFrom(a => new Dto.CodePartnerEntity { Id = a.Partner.Id, Name = a.Partner.Name }))
            //.ForMember(a => a.Account, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Account.Id, Code = a.Account.Code, Name = a.Account.Name }))
            .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.AppState.Id, Code = a.AppState.Code, Name = a.AppState.Name }));
            //.ForMember(a => a.CostCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.CostCenter.Id, Code = a.CostCenter.Code, Name = a.CostCenter.Name }));


            // BUDGET BASE //

            CreateMap<Model.BudgetBaseDetail, Dto.BudgetBase>()
             .ForMember(a => a.Id, opt => opt.MapFrom(a => a.BudgetBase.Id))
             .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.BudgetBase.IsAccepted))
             .ForMember(a => a.Code, opt => opt.MapFrom(a => a.BudgetBase.Code))
             .ForMember(a => a.Name, opt => opt.MapFrom(a => a.BudgetBase.Name))
             .ForMember(a => a.InfoMinus, opt => opt.MapFrom(a => a.BudgetBase.Name))
             .ForMember(a => a.ValueIni, opt => opt.MapFrom(a => a.BudgetBase.ValueIni))
             .ForMember(a => a.ValueFin, opt => opt.MapFrom(a => a.BudgetBase.ValueFin))
             .ForMember(a => a.Info, opt => opt.MapFrom(a => a.BudgetBase.Info))
             .ForMember(a => a.DepPeriod, opt => opt.MapFrom(a => a.BudgetBase.DepPeriod))
             .ForMember(a => a.DepPeriodRem, opt => opt.MapFrom(a => a.BudgetBase.DepPeriodRem))
             .ForMember(a => a.Total, opt => opt.MapFrom(a => a.BudgetBase.Total))
             .ForMember(a => a.AppStateId, opt => opt.MapFrom(a => a.BudgetBase.AppStateId))
             .ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Company.Id, Code = a.BudgetBase.Company.Code, Name = a.BudgetBase.Company.Name }))
             .ForMember(a => a.Project, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Project.Id, Code = a.BudgetBase.Project.Code, Name = a.BudgetBase.Project.Name }))
             .ForMember(a => a.Department, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Department.Id, Code = a.BudgetBase.Department.Code, Name = a.BudgetBase.Department.Name }))
             .ForMember(a => a.Division, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Division.Id, Code = a.BudgetBase.Division.Code, Name = a.BudgetBase.Division.Name }))
             //.ForMember(a => a.Type, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.SubType.Type.Id, Code = a.Budget.SubType.Type.Code, Name = a.Budget.SubType.Type.Name }))
             //.ForMember(a => a.SubType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.SubType.Id, Code = a.Budget.SubType.Code, Name = a.Budget.SubType.Name }))
             .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.BudgetBase.Employee.Id, InternalCode = a.BudgetBase.Employee.InternalCode, FirstName = a.BudgetBase.Employee.FirstName, LastName = a.BudgetBase.Employee.LastName, Email = a.BudgetBase.Employee.Email }))
             .ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.BudgetBase.AccMonth.Id, Year = a.BudgetBase.AccMonth.Year }))
             .ForMember(a => a.StartMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.BudgetBase.StartMonth.Id, Year = a.BudgetBase.StartMonth.Year }))
              .ForMember(a => a.Country, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Country.Id, Code = a.BudgetBase.Country.Code, Name = a.BudgetBase.Country.Name }))
             //.ForMember(a => a.InterCompany, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.InterCompany.Id, Code = a.Budget.InterCompany.Code, Name = a.Budget.InterCompany.Name }))
             //.ForMember(a => a.Partner, opt => opt.MapFrom(a => new Dto.CodePartnerEntity { Id = a.Budget.Partner.Id, Name = a.Budget.Partner.Name }))
             //.ForMember(a => a.Account, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Account.Id, Code = a.Budget.Account.Code, Name = a.Budget.Account.Name }))
             .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.AppState.Id, Code = a.BudgetBase.AppState.Code, Name = a.BudgetBase.AppState.Name }))
             //.ForMember(a => a.CostCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.CostCenter.Id, Code = a.Budget.CostCenter.Code, Name = a.Budget.CostCenter.Name }))
             .ForMember(a => a.BudgetManager, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.BudgetManager.Id, Code = a.BudgetBase.BudgetManager.Code, Name = a.BudgetBase.BudgetManager.Name }))
            
             .ForMember(a => a.Activity, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Activity.Id, Code = a.BudgetBase.Activity.Code, Name = a.BudgetBase.Activity.Name }))
             .ForMember(a => a.Region, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Region.Id, Code = a.BudgetBase.Region.Code, Name = a.BudgetBase.Region.Name }))
             .ForMember(a => a.AdmCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.AdmCenter.Id, Code = a.BudgetBase.AdmCenter.Code, Name = a.BudgetBase.AdmCenter.Name }))
             .ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.AssetType.Id, Code = a.BudgetBase.AssetType.Code, Name = a.BudgetBase.AssetType.Name }))
             .ForMember(a => a.ProjectType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.ProjectType.Id, Code = a.BudgetBase.ProjectType.Code, Name = a.BudgetBase.ProjectType.Name }));
            //.ForMember(i => i.BudgetMonths, opt => opt.MapFrom(i => i.BudgetBase.BudgetMonths.Select(p => new Dto.BudgetMonth()
            //{
            //    Id = p.Id,
            //    BudgetId = p.BudgetId,
            //    //Budget = new Dto.Budget() { Month = p.AccMonth != null ? p.AccMonth.Month : 0, Year = p.AccMonth != null ? p.AccMonth.Year : 0 },
            //    Value = p.Value,
            //    ValueDep = p.ValueDep,
            //    AccMonth = new Dto.AccMonth() { Month = p.AccMonth != null ? p.AccMonth.Month : 0, Year = p.AccMonth != null ? p.AccMonth.Year : 0 },
            //    //ValueTotal = p.ValueTotal,
            //    //ValueDepTotal = p.ValueDepTotal
            //})));


            //CreateMap<Model.BudgetBaseDetail, Dto.BudgetTree>()
            //.ForMember(i => i.Data, opt => opt.MapFrom(i => new Dto.CodeNameEntity()
            //{
            //    Id = i.Budget.Id,
            //    Code = i.Budget.Code,
            //    Name = i.Budget.Name
            //}))
            //.ForMember(i => i.Children, opt => opt.MapFrom(i => i.Budget.BudgetMonths.Select(p => new Dto.CodeNameEntity()
            //{
            //    Id = i.Budget.Id,
            //    Code = i.Budget.Code,
            //    Name = i.Budget.Name
            //})))
            // .ForMember(i => i.BudgetMonths, opt => opt.MapFrom(i => i.Budget.BudgetMonths.Select(p => new Dto.BudgetMonth()
            // {
            //     Id = p.Id,
            //     BudgetId = p.BudgetId,
            //     //Budget = new Dto.Budget() { Month = p.AccMonth != null ? p.AccMonth.Month : 0, Year = p.AccMonth != null ? p.AccMonth.Year : 0 },
            //     Value = p.Value,
            //     ValueDep = p.ValueDep,
            //     AccMonth = new Dto.AccMonth() { Month = p.AccMonth != null ? p.AccMonth.Month : 0, Year = p.AccMonth != null ? p.AccMonth.Year : 0 },
            //     //ValueTotal = p.ValueTotal,
            //     //ValueDepTotal = p.ValueDepTotal
            // })));

            // BUDGET BASE //

            CreateMap<Model.BudgetMonthBaseDetail, Dto.BudgetMonthBase>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.BudgetMonthBase.Id))
            .ForMember(a => a.DepPeriod, opt => opt.MapFrom(a => a.BudgetMonthBase.BudgetBase.DepPeriod))
            .ForMember(a => a.DepPeriodRem, opt => opt.MapFrom(a => a.BudgetMonthBase.BudgetBase.DepPeriodRem))
            .ForMember(a => a.January, opt => opt.MapFrom(a => a.BudgetMonthBase.January))
            .ForMember(a => a.February, opt => opt.MapFrom(a => a.BudgetMonthBase.February))
            .ForMember(a => a.March, opt => opt.MapFrom(a => a.BudgetMonthBase.March))
            .ForMember(a => a.April, opt => opt.MapFrom(a => a.BudgetMonthBase.April))
            .ForMember(a => a.May, opt => opt.MapFrom(a => a.BudgetMonthBase.May))
            .ForMember(a => a.June, opt => opt.MapFrom(a => a.BudgetMonthBase.June))
            .ForMember(a => a.July, opt => opt.MapFrom(a => a.BudgetMonthBase.July))
            .ForMember(a => a.August, opt => opt.MapFrom(a => a.BudgetMonthBase.August))
            .ForMember(a => a.September, opt => opt.MapFrom(a => a.BudgetMonthBase.September))
            .ForMember(a => a.Octomber, opt => opt.MapFrom(a => a.BudgetMonthBase.Octomber))
            .ForMember(a => a.November, opt => opt.MapFrom(a => a.BudgetMonthBase.November))
            .ForMember(a => a.December, opt => opt.MapFrom(a => a.BudgetMonthBase.December))
            .ForMember(a => a.BudgetBase, opt => opt.MapFrom(a => new Dto.BudgetBase { Id = a.BudgetMonthBase.BudgetBase.Id, Code = a.BudgetMonthBase.BudgetBase.Code, Name = a.BudgetMonthBase.BudgetBase.Name }))
            .ForMember(a => a.BudgetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetMonthBase.BudgetBase.BudgetType.Id, Code = a.BudgetMonthBase.BudgetBase.BudgetType.Code, Name = a.BudgetMonthBase.BudgetBase.BudgetType.Name }))
            .ForMember(a => a.BudgetManager, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetMonthBase.BudgetBase.BudgetManager.Id, Code = a.BudgetMonthBase.BudgetBase.BudgetManager.Code, Name = a.BudgetMonthBase.BudgetBase.BudgetManager.Name }))
            .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetMonthBase.BudgetBase.AppState.Id, Code = a.BudgetMonthBase.BudgetBase.AppState.Code, Name = a.BudgetMonthBase.BudgetBase.AppState.Name }))
            .ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.BudgetMonthBase.BudgetBase.AccMonth.Id, Month = a.BudgetMonthBase.BudgetBase.AccMonth.Month }))
            .ForMember(a => a.StartMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.BudgetMonthBase.BudgetBase.StartMonth.Id, Month = a.BudgetMonthBase.BudgetBase.StartMonth.Month }))
            .ForMember(a => a.Project, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetMonthBase.BudgetBase.Project.Id, Code = a.BudgetMonthBase.BudgetBase.Project.Code, Name = a.BudgetMonthBase.BudgetBase.Project.Name }))
            .ForMember(a => a.Total, opt => opt.MapFrom(a => a.BudgetMonthBase.Total));

            CreateMap<Model.BudgetForecastDetail, Dto.BudgetForecast>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.BudgetForecast.Id))
            .ForMember(a => a.DepPeriod, opt => opt.MapFrom(a => a.BudgetForecast.DepPeriod))
            .ForMember(a => a.DepPeriodRem, opt => opt.MapFrom(a => a.BudgetForecast.DepPeriodRem))
            .ForMember(a => a.HasChangeApril, opt => opt.MapFrom(a => a.BudgetForecast.HasChangeApril))
            .ForMember(a => a.HasChangeMay, opt => opt.MapFrom(a => a.BudgetForecast.HasChangeMay))
            .ForMember(a => a.HasChangeJune, opt => opt.MapFrom(a => a.BudgetForecast.HasChangeJune))
            .ForMember(a => a.HasChangeJuly, opt => opt.MapFrom(a => a.BudgetForecast.HasChangeJuly))
            .ForMember(a => a.HasChangeAugust, opt => opt.MapFrom(a => a.BudgetForecast.HasChangeAugust))
            .ForMember(a => a.HasChangeSeptember, opt => opt.MapFrom(a => a.BudgetForecast.HasChangeSeptember))
            .ForMember(a => a.HasChangeOctomber, opt => opt.MapFrom(a => a.BudgetForecast.HasChangeOctomber))
            .ForMember(a => a.HasChangeNovember, opt => opt.MapFrom(a => a.BudgetForecast.HasChangeNovember))
            .ForMember(a => a.HasChangeDecember, opt => opt.MapFrom(a => a.BudgetForecast.HasChangeDecember))
            .ForMember(a => a.HasChangeJanuary, opt => opt.MapFrom(a => a.BudgetForecast.HasChangeJanuary))
            .ForMember(a => a.HasChangeFebruary, opt => opt.MapFrom(a => a.BudgetForecast.HasChangeFebruary))
            .ForMember(a => a.HasChangeMarch, opt => opt.MapFrom(a => a.BudgetForecast.HasChangeMarch))
            .ForMember(a => a.January, opt => opt.MapFrom(a => a.BudgetForecast.January))
            .ForMember(a => a.February, opt => opt.MapFrom(a => a.BudgetForecast.February))
            .ForMember(a => a.March, opt => opt.MapFrom(a => a.BudgetForecast.March))
            .ForMember(a => a.April, opt => opt.MapFrom(a => a.BudgetForecast.April))
            .ForMember(a => a.May, opt => opt.MapFrom(a => a.BudgetForecast.May))
            .ForMember(a => a.June, opt => opt.MapFrom(a => a.BudgetForecast.June))
            .ForMember(a => a.July, opt => opt.MapFrom(a => a.BudgetForecast.July))
            .ForMember(a => a.August, opt => opt.MapFrom(a => a.BudgetForecast.August))
            .ForMember(a => a.September, opt => opt.MapFrom(a => a.BudgetForecast.September))
            .ForMember(a => a.Octomber, opt => opt.MapFrom(a => a.BudgetForecast.Octomber))
            .ForMember(a => a.November, opt => opt.MapFrom(a => a.BudgetForecast.November))
            .ForMember(a => a.December, opt => opt.MapFrom(a => a.BudgetForecast.December))
			.ForMember(a => a.InTransfer, opt => opt.MapFrom(a => a.BudgetForecast.InTransfer))
			.ForMember(a => a.BudgetBase, opt => opt.MapFrom(a => new Dto.BudgetBase { 
                Id = a.BudgetForecast.BudgetBase.Id, 
                Code = a.BudgetForecast.BudgetBase.Code, 
                Name = a.BudgetForecast.BudgetBase.Name, 
                Info = a.BudgetForecast.BudgetBase.Info }))
            .ForMember(a => a.BudgetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.BudgetType.Id, Code = a.BudgetForecast.BudgetBase.BudgetType.Code, Name = a.BudgetForecast.BudgetBase.BudgetType.Name }))
			.ForMember(a => a.Division, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.Division.Id, Code = a.BudgetForecast.BudgetBase.Division.Code, Name = a.BudgetForecast.BudgetBase.Division.Name }))
			.ForMember(a => a.Department, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.Division.Department.Id, Code = a.BudgetForecast.BudgetBase.Division.Department.Code, Name = a.BudgetForecast.BudgetBase.Division.Department.Name }))
			.ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.AssetType.Id, Code = a.BudgetForecast.BudgetBase.AssetType.Code, Name = a.BudgetForecast.BudgetBase.AssetType.Name }))
            .ForMember(a => a.AdmCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.AdmCenter.Id, Code = a.BudgetForecast.BudgetBase.AdmCenter.Code, Name = a.BudgetForecast.BudgetBase.AdmCenter.Name }))
			.ForMember(a => a.BudgetManager, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.BudgetManager.Id, Code = a.BudgetForecast.BudgetBase.BudgetManager.Code, Name = a.BudgetForecast.BudgetBase.BudgetManager.Name }))
            .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.AppState.Id, Code = a.BudgetForecast.BudgetBase.AppState.Code, Name = a.BudgetForecast.BudgetBase.AppState.Name }))
            .ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.BudgetForecast.BudgetBase.AccMonth.Id, Month = a.BudgetForecast.BudgetBase.AccMonth.Month }))
            .ForMember(a => a.StartMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.BudgetForecast.StartMonth.Id, Month = a.BudgetForecast.StartMonth.Month }))
			.ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.BudgetForecast.BudgetBase.Employee.Id, Email = a.BudgetForecast.BudgetBase.Employee.Email }))
			.ForMember(a => a.Project, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.Project.Id, Code = a.BudgetForecast.BudgetBase.Project.Code, Name = a.BudgetForecast.BudgetBase.Project.Name }))
			.ForMember(a => a.ProjectType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.ProjectType.Id, Code = a.BudgetForecast.BudgetBase.ProjectType.Code, Name = a.BudgetForecast.BudgetBase.ProjectType.Name }))
			.ForMember(a => a.Region, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.Region.Id, Code = a.BudgetForecast.BudgetBase.Region.Code, Name = a.BudgetForecast.BudgetBase.Region.Name }))
			.ForMember(a => a.Activity, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.Activity.Id, Code = a.BudgetForecast.BudgetBase.Activity.Code, Name = a.BudgetForecast.BudgetBase.Activity.Name }))
			.ForMember(a => a.Country, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.Country.Id, Code = a.BudgetForecast.BudgetBase.Country.Code, Name = a.BudgetForecast.BudgetBase.Country.Name }))
			.ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.BudgetForecast.AccMonth.Id,  Month = a.BudgetForecast.AccMonth.Month, Year = a.BudgetForecast.AccMonth.Year }))
			.ForMember(a => a.Total, opt => opt.MapFrom(a => a.BudgetForecast.Total))
            .ForMember(a => a.TotalRem, opt => opt.MapFrom(a => a.BudgetForecast.TotalRem))
            .ForMember(a => a.ValueOrder, opt => opt.MapFrom(a => a.BudgetForecast.ValueOrder))
            .ForMember(a => a.ImportValueOrder, opt => opt.MapFrom(a => a.BudgetForecast.ImportValueOrder))
			.ForMember(a => a.ValueOrderApproved, opt => opt.MapFrom(a => a.BudgetForecast.ValOrderApproved))
			.ForMember(a => a.ValueOrderPending, opt => opt.MapFrom(a => a.BudgetForecast.ValOrderPending))
			.ForMember(a => a.ValueRequest, opt => opt.MapFrom(a => a.BudgetForecast.ValRequest))
			.ForMember(a => a.ValueAssetYTD, opt => opt.MapFrom(a => a.BudgetForecast.ValueAssetYTD))
			.ForMember(a => a.ValueAssetYTG, opt => opt.MapFrom(a => a.BudgetForecast.ValueAssetYTG))
			.ForMember(a => a.ValueAsset, opt => opt.MapFrom(a => a.BudgetForecast.ValueAsset));
            // CreateMap<Model.Budget, Dto.Budget>();
            CreateMap<Model.BudgetMonthBase, Dto.BudgetMonthBase>();
            //.ForMember(i => i.BudgetMonth, opt => opt.MapFrom(i => i.BudgetMonths.Select(p => new Dto.BudgetMonth()
            //{
            //    Value = p.Value,
            //    ValueDep = p.ValueDep,
            //    AccMonth = new Dto.AccMonth() { Month = p.AccMonth != null ? p.AccMonth.Month : 0, Year = p.AccMonth != null ? p.AccMonth.Year : 0 },
            //})));


            CreateMap<Model.BudgetForecastDetail, Dto.BudgetForecastUI>()
             .ForMember(a => a.Id, opt => opt.MapFrom(a => a.BudgetForecast.Id))
             .ForMember(a => a.BudgetBase, opt => opt.MapFrom(a => new Dto.BudgetBase { Id = a.BudgetForecast.BudgetBase.Id, Code = a.BudgetForecast.BudgetBase.Code, Name = a.BudgetForecast.BudgetBase.Name, Info = a.BudgetForecast.BudgetBase.Info }))
             .ForMember(a => a.Project, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.Project.Id, Code = a.BudgetForecast.BudgetBase.Project.Code, Name = a.BudgetForecast.BudgetBase.Project.Name }))
             .ForMember(a => a.AdmCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.AdmCenter.Id, Code = a.BudgetForecast.BudgetBase.AdmCenter.Code, Name = a.BudgetForecast.BudgetBase.AdmCenter.Name }))
             .ForMember(a => a.Region, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.Region.Id, Code = a.BudgetForecast.BudgetBase.Region.Code, Name = a.BudgetForecast.BudgetBase.Region.Name }))
             .ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.AssetType.Id, Code = a.BudgetForecast.BudgetBase.AssetType.Code, Name = a.BudgetForecast.BudgetBase.AssetType.Name }))
             .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.AppState.Id, Code = a.BudgetForecast.BudgetBase.AppState.Code, Name = a.BudgetForecast.BudgetBase.AppState.Name }))
             .ForMember(a => a.Department, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.Division.Department.Id, Code = a.BudgetForecast.BudgetBase.Division.Department.Code, Name = a.BudgetForecast.BudgetBase.Division.Department.Name }))
             .ForMember(a => a.Division, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.Division.Id, Code = a.BudgetForecast.BudgetBase.Division.Code, Name = a.BudgetForecast.BudgetBase.Division.Name }))
             .ForMember(a => a.ProjectType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetForecast.BudgetBase.Project.ProjectType.Id, Code = a.BudgetForecast.BudgetBase.Project.ProjectType.Code, Name = a.BudgetForecast.BudgetBase.Project.ProjectType.Name }))
			 .ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.BudgetForecast.AccMonth.Id, Month = a.BudgetForecast.AccMonth.Month, Year = a.BudgetForecast.AccMonth.Year }))
			 .ForMember(a => a.Total, opt => opt.MapFrom(a => a.BudgetForecast.Total))
             .ForMember(a => a.TotalRem, opt => opt.MapFrom(a => a.BudgetForecast.TotalRem))
             .ForMember(a => a.ValueAsset, opt => opt.MapFrom(a => a.BudgetForecast.ValueAsset))
             .ForMember(a => a.ValueOrder, opt => opt.MapFrom(a => a.BudgetForecast.ValueOrder))
             .ForMember(a => a.ImportValueOrder, opt => opt.MapFrom(a => a.BudgetForecast.ImportValueOrder))
             .ForMember(a => a.InTransfer, opt => opt.MapFrom(a => a.BudgetForecast.InTransfer));




			CreateMap<Model.BudgetForecast, Dto.BudgetForecast>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Id))
            .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.BudgetBase.IsAccepted))
            .ForMember(a => a.Code, opt => opt.MapFrom(a => a.BudgetBase.Code))
            .ForMember(a => a.Name, opt => opt.MapFrom(a => a.BudgetBase.Name))
            .ForMember(a => a.Info, opt => opt.MapFrom(a => a.BudgetBase.Info))
            .ForMember(a => a.ValueIni, opt => opt.MapFrom(a => a.BudgetBase.ValueIni))
            .ForMember(a => a.ValueFin, opt => opt.MapFrom(a => a.BudgetBase.ValueFin))
            .ForMember(a => a.January, opt => opt.MapFrom(a => a.January))
            .ForMember(a => a.February, opt => opt.MapFrom(a => a.February))
            .ForMember(a => a.March, opt => opt.MapFrom(a => a.March))
            .ForMember(a => a.April, opt => opt.MapFrom(a => a.April))
            .ForMember(a => a.May, opt => opt.MapFrom(a => a.May))
            .ForMember(a => a.June, opt => opt.MapFrom(a => a.June))
            .ForMember(a => a.July, opt => opt.MapFrom(a => a.July))
            .ForMember(a => a.August, opt => opt.MapFrom(a => a.August))
            .ForMember(a => a.September, opt => opt.MapFrom(a => a.September))
            .ForMember(a => a.Octomber, opt => opt.MapFrom(a => a.Octomber))
            .ForMember(a => a.November, opt => opt.MapFrom(a => a.November))
            .ForMember(a => a.December, opt => opt.MapFrom(a => a.December))
			.ForMember(a => a.InTransfer, opt => opt.MapFrom(a => a.InTransfer))
			.ForMember(a => a.Total, opt => opt.MapFrom(a => a.Total))
            .ForMember(a => a.TotalRem, opt => opt.MapFrom(a => a.TotalRem))
            .ForMember(a => a.ImportValueOrder, opt => opt.MapFrom(a => a.ImportValueOrder))
            .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.BudgetBase.Employee.Id, InternalCode = a.BudgetBase.Employee.InternalCode, FirstName = a.BudgetBase.Employee.FirstName, LastName = a.BudgetBase.Employee.LastName, Email = a.BudgetBase.Employee.Email }))
            .ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Company.Id, Code = a.BudgetBase.Company.Code, Name = a.BudgetBase.Company.Name }))
            .ForMember(a => a.Project, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Project.Id, Code = a.BudgetBase.Project.Code, Name = a.BudgetBase.Project.Name }))
            .ForMember(a => a.Country, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Country.Id, Code = a.BudgetBase.Country.Code, Name = a.BudgetBase.Country.Name }))
            .ForMember(a => a.Activity, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Activity.Id, Code = a.BudgetBase.Activity.Code, Name = a.BudgetBase.Activity.Name }))
            .ForMember(a => a.Department, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Department.Id, Code = a.BudgetBase.Department.Code, Name = a.BudgetBase.Department.Name }))
            .ForMember(a => a.AdmCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.AdmCenter.Id, Code = a.BudgetBase.AdmCenter.Code, Name = a.BudgetBase.AdmCenter.Name }))
            .ForMember(a => a.Region, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Region.Id, Code = a.BudgetBase.Region.Code, Name = a.BudgetBase.Region.Name }))
            .ForMember(a => a.Division, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Division.Id, Code = a.BudgetBase.Division.Code, Name = a.BudgetBase.Division.Name }))
            .ForMember(a => a.ProjectType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.ProjectType.Id, Code = a.BudgetBase.ProjectType.Code, Name = a.BudgetBase.ProjectType.Name }))
            .ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.AssetType.Id, Code = a.BudgetBase.AssetType.Code, Name = a.BudgetBase.AssetType.Name }))
            .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.AppState.Id, Code = a.BudgetBase.AppState.Code, Name = a.BudgetBase.AppState.Name }))
            .ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.AccMonth.Id, Year = a.AccMonth.Year, Month = a.AccMonth.Month }))
            .ForMember(a => a.StartMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.BudgetBase.StartMonth.Id, Year = a.BudgetBase.StartMonth.Year }));
            //.ForMember(i => i.BudgetMonthBases, opt => opt.MapFrom(i => i.BudgetBase.BudgetMonthBase.Where(p => p.IsDeleted == false && p.IsFirst == true).Select(p => new Dto.BudgetMonthBase()
            //{
            //    January = p.January,
            //    February = p.February,
            //    March = p.March,
            //    April = p.April,
            //    May = p.May,
            //    June = p.June,
            //    July = p.July,
            //    August = p.August,
            //    September = p.September,
            //    Octomber = p.Octomber,
            //    November = p.November,
            //    December = p.December,
            //    Total = p.Total
            //})));

            CreateMap<Model.BudgetBaseDetail, Dto.BudgetBaseUI>()
             .ForMember(a => a.Id, opt => opt.MapFrom(a => a.BudgetBase.Id))
             //.ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.Budget.IsAccepted))
             .ForMember(a => a.Code, opt => opt.MapFrom(a => a.BudgetBase.Code))
             .ForMember(a => a.Name, opt => opt.MapFrom(a => a.BudgetBase.Name))
            //.ForMember(a => a.InfoMinus, opt => opt.MapFrom(a => a.Budget.Name))
            .ForMember(a => a.ValueIni, opt => opt.MapFrom(a => a.BudgetBase.ValueIni))
            .ForMember(a => a.ValueFin, opt => opt.MapFrom(a => a.BudgetBase.ValueFin))
            .ForMember(a => a.ValueUsed, opt => opt.MapFrom(a => a.BudgetBase.ValueUsed))
             .ForMember(a => a.ValueOrder, opt => opt.MapFrom(a => a.BudgetBase.ValueOrder))
             //.ForMember(a => a.Info, opt => opt.MapFrom(a => a.Budget.Info))
             .ForMember(a => a.AppStateId, opt => opt.MapFrom(a => a.BudgetBase.AppStateId))
             .ForMember(a => a.BudgetMonthBase, opt => opt.MapFrom(a => new Dto.BudgetMonthBase
             { 
                 Id = a.BudgetBase.BudgetMonthBase.Where(a => a.IsDeleted == false && a.IsLast == true).FirstOrDefault().Id,
                 January = a.BudgetBase.BudgetMonthBase.Where(a => a.IsDeleted == false && a.IsLast == true).FirstOrDefault().January,
                 February = a.BudgetBase.BudgetMonthBase.Where(a => a.IsDeleted == false && a.IsLast == true).FirstOrDefault().February,
                 March = a.BudgetBase.BudgetMonthBase.Where(a => a.IsDeleted == false && a.IsLast == true).FirstOrDefault().March,
                 April = a.BudgetBase.BudgetMonthBase.Where(a => a.IsDeleted == false && a.IsLast == true).FirstOrDefault().April,
                 May = a.BudgetBase.BudgetMonthBase.Where(a => a.IsDeleted == false && a.IsLast == true).FirstOrDefault().May,
                 June = a.BudgetBase.BudgetMonthBase.Where(a => a.IsDeleted == false && a.IsLast == true).FirstOrDefault().June,
                 July = a.BudgetBase.BudgetMonthBase.Where(a => a.IsDeleted == false && a.IsLast == true).FirstOrDefault().July,
                 August = a.BudgetBase.BudgetMonthBase.Where(a => a.IsDeleted == false && a.IsLast == true).FirstOrDefault().August,
                 September = a.BudgetBase.BudgetMonthBase.Where(a => a.IsDeleted == false && a.IsLast == true).FirstOrDefault().September,
                 Octomber = a.BudgetBase.BudgetMonthBase.Where(a => a.IsDeleted == false && a.IsLast == true).FirstOrDefault().Octomber,
                 November = a.BudgetBase.BudgetMonthBase.Where(a => a.IsDeleted == false && a.IsLast == true).FirstOrDefault().November,
                 December = a.BudgetBase.BudgetMonthBase.Where(a => a.IsDeleted == false && a.IsLast == true).FirstOrDefault().December,
             }))
             .ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Company.Id, Code = a.BudgetBase.Company.Code, Name = a.BudgetBase.Company.Name }))
             .ForMember(a => a.Uom, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Uom.Id, Code = a.BudgetBase.Uom.Code, Name = a.BudgetBase.Uom.Name }))
             .ForMember(a => a.Project, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Project.Id, Code = a.BudgetBase.Project.Code, Name = a.BudgetBase.Project.Name }))
             .ForMember(a => a.AdmCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.AdmCenter.Id, Code = a.BudgetBase.AdmCenter.Code, Name = a.BudgetBase.AdmCenter.Name }))
             .ForMember(a => a.Region, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Region.Id, Code = a.BudgetBase.Region.Code, Name = a.BudgetBase.Region.Name }))
             .ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.AssetType.Id, Code = a.BudgetBase.AssetType.Code, Name = a.BudgetBase.AssetType.Name }))
             //.ForMember(a => a.Administration, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Administration.Id, Code = a.Budget.Administration.Code, Name = a.Budget.Administration.Name }))
             //.ForMember(a => a.MasterType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.SubType.Type.MasterType.Id, Code = a.Budget.SubType.Type.MasterType.Code, Name = a.Budget.SubType.Type.MasterType.Name }))
             //.ForMember(a => a.Type, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.SubType.Type.Id, Code = a.Budget.SubType.Type.Code, Name = a.Budget.SubType.Type.Name }))
             //.ForMember(a => a.SubType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.SubType.Id, Code = a.Budget.SubType.Code, Name = a.Budget.SubType.Name }))
             //.ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Budget.Employee.Id, InternalCode = a.Budget.Employee.InternalCode, FirstName = a.Budget.Employee.FirstName, LastName = a.Budget.Employee.LastName }))
             //.ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.Budget.AccMonth.Id, Year = a.Budget.AccMonth.Year.ToString() }))
             //.ForMember(a => a.InterCompany, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.InterCompany.Id, Code = a.Budget.InterCompany.Code, Name = a.Budget.InterCompany.Name }))
             //.ForMember(a => a.Partner, opt => opt.MapFrom(a => new Dto.CodePartnerEntity { Id = a.Budget.Partner.Id, Name = a.Budget.Partner.Name }))
             //.ForMember(a => a.Account, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.Account.Id, Code = a.Budget.Account.Code, Name = a.Budget.Account.Name }))
             .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.AppState.Id, Code = a.BudgetBase.AppState.Code, Name = a.BudgetBase.AppState.Name }));
            //.ForMember(a => a.CostCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Budget.CostCenter.Id, Code = a.Budget.CostCenter.Code, Name = a.Budget.CostCenter.Name }));

            CreateMap<Model.BudgetBaseDetail, Dto.BudgetBaseFreezeUI>()
             .ForMember(a => a.Id, opt => opt.MapFrom(a => a.BudgetBase.Id))
             .ForMember(a => a.Code, opt => opt.MapFrom(a => a.BudgetBase.Code))
             .ForMember(a => a.Name, opt => opt.MapFrom(a => a.BudgetBase.Name))
             .ForMember(a => a.Info, opt => opt.MapFrom(a => a.BudgetBase.Info))
             .ForMember(a => a.Project, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetBase.Project.Id, Code = a.BudgetBase.Project.Code, Name = a.BudgetBase.Project.Name }));



            CreateMap<Model.BudgetBase, Dto.BudgetBase>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Id))
            .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.IsAccepted))
            .ForMember(a => a.Code, opt => opt.MapFrom(a => a.Code))
            .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Name))
            .ForMember(a => a.Info, opt => opt.MapFrom(a => a.Info))
            .ForMember(a => a.ValueIni, opt => opt.MapFrom(a => a.ValueIni))
            .ForMember(a => a.ValueFin, opt => opt.MapFrom(a => a.ValueFin))
             .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Employee.Id, InternalCode = a.Employee.InternalCode, FirstName = a.Employee.FirstName, LastName = a.Employee.LastName, Email = a.Employee.Email }))
            .ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Company.Id, Code = a.Company.Code, Name = a.Company.Name }))
            .ForMember(a => a.Project, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Project.Id, Code = a.Project.Code, Name = a.Project.Name }))
            .ForMember(a => a.Country, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Country.Id, Code = a.Country.Code, Name = a.Country.Name }))
            .ForMember(a => a.Activity, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Activity.Id, Code = a.Activity.Code, Name = a.Activity.Name }))
             .ForMember(a => a.Department, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Department.Id, Code = a.Department.Code, Name = a.Department.Name }))
            .ForMember(a => a.AdmCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.AdmCenter.Id, Code = a.AdmCenter.Code, Name = a.AdmCenter.Name }))
            .ForMember(a => a.Region, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Region.Id, Code = a.Region.Code, Name = a.Region.Name }))
            .ForMember(a => a.Division, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Division.Id, Code = a.Division.Code, Name = a.Division.Name }))
             .ForMember(a => a.ProjectType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.ProjectType.Id, Code = a.ProjectType.Code, Name = a.ProjectType.Name }))
            .ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.AssetType.Id, Code = a.AssetType.Code, Name = a.AssetType.Name }))
            .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.AppState.Id, Code = a.AppState.Code, Name = a.AppState.Name }))
            .ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.AccMonth.Id, Year = a.AccMonth.Year }))
            .ForMember(a => a.StartMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.StartMonth.Id, Year = a.StartMonth.Year }))
            .ForMember(i => i.BudgetMonthBases, opt => opt.MapFrom(i => i.BudgetMonthBase.Where(p => p.IsDeleted == false && p.IsFirst == true).Select(p => new Dto.BudgetMonthBase()
             {
                 January = p.January,
                 February = p.February,
                 March = p.March,
                 April = p.April,
                 May = p.May,
                 June = p.June,
                 July = p.July,
                 August = p.August,
                 September = p.September,
                 Octomber = p.Octomber,
                 November = p.November,
                 December = p.December,
                 Total = p.Total,
                 BudgetManager = new Dto.CodeNameEntity() { Id = p.BudgetManager != null ? p.BudgetManager.Id : 0, Code = p.BudgetManager != null ? p.BudgetManager.Code : "", Name = p.BudgetManager != null ? p.BudgetManager.Name : "" },
                 BudgetType = new Dto.CodeNameEntity() { Id = p.BudgetType != null ? p.BudgetType.Id : 0, Code = p.BudgetType != null ? p.BudgetType.Code : "", Name = p.BudgetType != null ? p.BudgetType.Name : "" }
             })))
             .ForMember(i => i.BudgetForecasts, opt => opt.MapFrom(i => i.BudgetForecast.Where(p => p.IsDeleted == false && p.IsLast == true).Select(p => new Dto.BudgetForecast()
             {
                 Id = p.Id,
                 January = p.January,
                 February = p.February,
                 March = p.March,
                 April = p.April,
                 May = p.May,
                 June = p.June,
                 July = p.July,
                 August = p.August,
                 September = p.September,
                 Octomber = p.Octomber,
                 November = p.November,
                 December = p.December,
                 Total = p.Total,
                 BudgetManager = new Dto.CodeNameEntity() { Id = p.BudgetManager != null ? p.BudgetManager.Id : 0, Code = p.BudgetManager != null ? p.BudgetManager.Code : "", Name = p.BudgetManager != null ? p.BudgetManager.Name : "" },
                 BudgetType = new Dto.CodeNameEntity() { Id = p.BudgetType != null ? p.BudgetType.Id : 0, Code = p.BudgetType != null ? p.BudgetType.Code : "", Name = p.BudgetType != null ? p.BudgetType.Name : "" }
             })));


            // BUDGET BASE //



            CreateMap<Model.OrderDetail, Dto.Order>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Order.Id))
            .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.Order.IsAccepted))
			.ForMember(a => a.CreatedAt, opt => opt.MapFrom(a => a.Order.CreatedAt))
			.ForMember(a => a.ModifiedAt, opt => opt.MapFrom(a => a.Order.ModifiedAt))
			.ForMember(a => a.Code, opt => opt.MapFrom(a => a.Order.Code))
            .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Order.Name))
            .ForMember(a => a.PreAmount, opt => opt.MapFrom(a => a.Order.PreAmount))
            .ForMember(a => a.Contract, opt => opt.MapFrom(a => new Dto.Contract { 
                Id = a.Order.Contract != null ? a.Order.Contract.Id : 0, 
                ContractID = a.Order.Contract != null ? a.Order.Contract.ContractId : "", 
                Title = a.Order.Contract != null ? a.Order.Contract.Title : "" }))
            .ForMember(a => a.Offer, opt => opt.MapFrom(a => new Dto.Offer { 
                Id = a.Order.Offer != null ? a.Order.Offer.Id : 0, 
                Code = a.Order.Offer != null ? a.Order.Offer.Code : "", 
                Name = a.Order.Offer != null ? a.Order.Offer.Name : "", 
                Request = new Dto.Request { 
                    Id = a.Order.Offer != null && a.Order.Offer.Request != null ? a.Order.Offer.Request.Id : 0, 
                    Code = a.Order.Offer != null && a.Order.Offer.Request != null ? a.Order.Offer.Request.Code :"",
					Name = a.Order.Offer != null && a.Order.Offer.Request != null ? a.Order.Offer.Request.Name : "" } }))
            .ForMember(a => a.InfoMinus, opt => opt.MapFrom(a => a.Order.Name))
            .ForMember(a => a.ValueIni, opt => opt.MapFrom(a => a.Order.ValueIni))
            .ForMember(a => a.ValueIniRon, opt => opt.MapFrom(a => a.Order.ValueIniRon))
            .ForMember(a => a.ValueFin, opt => opt.MapFrom(a => a.Order.ValueFin))
            .ForMember(a => a.ValueFinRon, opt => opt.MapFrom(a => a.Order.ValueFinRon))
            .ForMember(a => a.ValueUsed, opt => opt.MapFrom(a => a.Order.ValueUsed))
			 .ForMember(a => a.ValueUsedRon, opt => opt.MapFrom(a => a.Order.ValueUsedRon))
			.ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Order.Quantity))
            .ForMember(a => a.QuantityRem, opt => opt.MapFrom(a => a.Order.QuantityRem))
            .ForMember(a => a.QuantityUsed, opt => opt.MapFrom(a => a.Order.QuantityUsed))
            .ForMember(a => a.Price, opt => opt.MapFrom(a => a.Order.Price))
            .ForMember(a => a.Info, opt => opt.MapFrom(a => a.Order.Info))
            .ForMember(a => a.AppStateId, opt => opt.MapFrom(a => a.Order.AppStateId))
			.ForMember(a => a.StartDate, opt => opt.MapFrom(a => a.Order.StartDate))
			.ForMember(a => a.EndDate, opt => opt.MapFrom(a => a.Order.EndDate))
			.ForMember(a => a.Uom, opt => opt.MapFrom(a => new Dto.CodeNameEntity { 
                Id = a.Order.Uom != null ? a.Order.Uom.Id : 0, 
                Code = a.Order.Uom != null ? a.Order.Uom.Code : "", 
                Name = a.Order.Uom != null ? a.Order.Uom.Name: "" }))
            .ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { 
                Id = a.Order.Company != null ? a.Order.Company.Id : 0, 
                Code = a.Order.Company != null ? a.Order.Company.Code : "", 
                Name = a.Order.Company != null ? a.Order.Company.Name: "" }))
            .ForMember(a => a.Division, opt => opt.MapFrom(a => new Dto.CodeNameEntity { 
                Id = a.Order.Division != null ? a.Order.Division.Id : 0, 
                Code = a.Order.Division != null ? a.Order.Division.Code :"", 
                Name = a.Order.Division != null ? a.Order.Division.Name: "" }))
            .ForMember(a => a.ProjectType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { 
                Id = a.Order.Project != null && a.Order.Project.ProjectType != null ? a.Order.Project.ProjectType.Id : 0, 
                Code = a.Order.Project != null && a.Order.Project.ProjectType != null ? a.Order.Project.ProjectType.Code : "", 
                Name = a.Order.Project != null && a.Order.Project.ProjectType != null ? a.Order.Project.ProjectType.Name : "" }))
            .ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { 
                Id = a.Order.Offer != null && a.Order.Offer.AssetType != null ? a.Order.Offer.AssetType.Id : 0, 
                Code = a.Order.Offer != null && a.Order.Offer.AssetType != null ? a.Order.Offer.AssetType.Code : "", 
                Name = a.Order.Offer != null && a.Order.Offer.AssetType != null ? a.Order.Offer.AssetType.Name : "" }))
            .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { 
                Id = a.Order.Employee != null ? a.Order.Employee.Id : 0, 
                InternalCode = a.Order.Employee != null ? a.Order.Employee.InternalCode : "", 
                FirstName = a.Order.Employee != null ? a.Order.Employee.FirstName : "", 
                LastName = a.Order.Employee != null ? a.Order.Employee.LastName : "", 
                Email = a.Order.Employee != null ? a.Order.Employee.Email : "" }))
			 .ForMember(a => a.EmployeeB1, opt => opt.MapFrom(a => new Dto.EmployeeResource
			 {
				 Id = a.Order.EmployeeB1 != null ? a.Order.EmployeeB1.Id : 0,
				 InternalCode = a.Order.EmployeeB1 != null ? a.Order.EmployeeB1.InternalCode : "",
				 FirstName = a.Order.EmployeeB1 != null ? a.Order.EmployeeB1.FirstName : "",
				 LastName = a.Order.EmployeeB1 != null ? a.Order.EmployeeB1.LastName : "",
				 Email = a.Order.EmployeeB1 != null ? a.Order.EmployeeB1.Email : ""
			 }))
			  .ForMember(a => a.EmployeeL4, opt => opt.MapFrom(a => new Dto.EmployeeResource
			  {
				  Id = a.Order.EmployeeL4 != null ? a.Order.EmployeeL4.Id : 0,
				  InternalCode = a.Order.EmployeeL4 != null ? a.Order.EmployeeL4.InternalCode : "",
				  FirstName = a.Order.EmployeeL4 != null ? a.Order.EmployeeL4.FirstName : "",
				  LastName = a.Order.EmployeeL4 != null ? a.Order.EmployeeL4.LastName : "",
				  Email = a.Order.EmployeeL4 != null ? a.Order.EmployeeL4.Email : ""
			  }))
		   .ForMember(a => a.EmployeeL3, opt => opt.MapFrom(a => new Dto.EmployeeResource
		   {
			   Id = a.Order.EmployeeL3 != null ? a.Order.EmployeeL3.Id : 0,
			   InternalCode = a.Order.EmployeeL3 != null ? a.Order.EmployeeL3.InternalCode : "",
			   FirstName = a.Order.EmployeeL3 != null ? a.Order.EmployeeL3.FirstName : "",
			   LastName = a.Order.EmployeeL3 != null ? a.Order.EmployeeL3.LastName : "",
			   Email = a.Order.EmployeeL3 != null ? a.Order.EmployeeL3.Email : ""
		   }))
			.ForMember(a => a.EmployeeL2, opt => opt.MapFrom(a => new Dto.EmployeeResource
			{
				Id = a.Order.EmployeeL2 != null ? a.Order.EmployeeL2.Id : 0,
				InternalCode = a.Order.EmployeeL2 != null ? a.Order.EmployeeL2.InternalCode : "",
				FirstName = a.Order.EmployeeL2 != null ? a.Order.EmployeeL2.FirstName : "",
				LastName = a.Order.EmployeeL2 != null ? a.Order.EmployeeL2.LastName : "",
				Email = a.Order.EmployeeL2 != null ? a.Order.EmployeeL2.Email : ""
			}))
			 .ForMember(a => a.EmployeeL1, opt => opt.MapFrom(a => new Dto.EmployeeResource
			 {
				 Id = a.Order.EmployeeL1 != null ? a.Order.EmployeeL1.Id : 0,
				 InternalCode = a.Order.EmployeeL1 != null ? a.Order.EmployeeL1.InternalCode : "",
				 FirstName = a.Order.EmployeeL1 != null ? a.Order.EmployeeL1.FirstName : "",
				 LastName = a.Order.EmployeeL1 != null ? a.Order.EmployeeL1.LastName : "",
				 Email = a.Order.EmployeeL1 != null ? a.Order.EmployeeL1.Email : ""
			 }))
			  .ForMember(a => a.EmployeeS1, opt => opt.MapFrom(a => new Dto.EmployeeResource
			  {
				  Id = a.Order.EmployeeS1 != null ? a.Order.EmployeeS1.Id : 0,
				  InternalCode = a.Order.EmployeeS1 != null ? a.Order.EmployeeS1.InternalCode : "",
				  FirstName = a.Order.EmployeeS1 != null ? a.Order.EmployeeS1.FirstName : "",
				  LastName = a.Order.EmployeeS1 != null ? a.Order.EmployeeS1.LastName : "",
				  Email = a.Order.EmployeeS1 != null ? a.Order.EmployeeS1.Email : ""
			  }))
			   .ForMember(a => a.EmployeeS2, opt => opt.MapFrom(a => new Dto.EmployeeResource
			   {
				   Id = a.Order.EmployeeS2 != null ? a.Order.EmployeeS2.Id : 0,
				   InternalCode = a.Order.EmployeeS2 != null ? a.Order.EmployeeS2.InternalCode : "",
				   FirstName = a.Order.EmployeeS2 != null ? a.Order.EmployeeS2.FirstName : "",
				   LastName = a.Order.EmployeeS2 != null ? a.Order.EmployeeS2.LastName : "",
				   Email = a.Order.EmployeeS2 != null ? a.Order.EmployeeS2.Email : ""
			   }))
				.ForMember(a => a.EmployeeS3, opt => opt.MapFrom(a => new Dto.EmployeeResource
				{
					Id = a.Order.EmployeeS3 != null ? a.Order.EmployeeS3.Id : 0,
					InternalCode = a.Order.EmployeeS3 != null ? a.Order.EmployeeS3.InternalCode : "",
					FirstName = a.Order.EmployeeS3 != null ? a.Order.EmployeeS3.FirstName : "",
					LastName = a.Order.EmployeeS3 != null ? a.Order.EmployeeS3.LastName : "",
					Email = a.Order.EmployeeS3 != null ? a.Order.EmployeeS3.Email : ""
				}))
				
			.ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { 
                Id = a.Order.AccMonth != null ? a.Order.AccMonth.Id : 0, 
                Year = a.Order.AccMonth != null ? a.Order.AccMonth.Year : 0 }))
            .ForMember(a => a.Partner, opt => opt.MapFrom(a => new Dto.CodePartnerEntity { 
                Id = a.Order.Partner != null ? a.Order.Partner.Id : 0, 
                Name = a.Order.Partner != null ? a.Order.Partner.Name : "" }))
            .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.AppState { 
                Id = a.Order.AppState!= null ?  a.Order.AppState.Id : 0, 
                Code = a.Order.AppState != null ? a.Order.AppState.Code : "",
				BadgeColor = a.Order.AppState != null ? a.Order.AppState.BadgeColor : "",
				BadgeIcon = a.Order.AppState != null ? a.Order.AppState.BadgeIcon : "",
				Name = a.Order.AppState != null ? a.Order.AppState.Name: "" }))
            .ForMember(a => a.OrderType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { 
                Id = a.Order.OrderType != null ? a.Order.OrderType.Id : 0, 
                Code = a.Order.OrderType != null ? a.Order.OrderType.Code : "", 
                Name = a.Order.OrderType != null ? a.Order.OrderType.Name : "" }));

            CreateMap<Model.OrderDetail, Dto.OrderUI>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Order.Id))
            .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.Order.IsAccepted))
            .ForMember(a => a.Code, opt => opt.MapFrom(a => a.Order.Code))
            .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Order.Name))
            .ForMember(a => a.Info, opt => opt.MapFrom(a => a.Order.Offer.Request.Info))
            .ForMember(a => a.Contract, opt => opt.MapFrom(a => new Dto.Contract { Id = a.Order.Contract.Id, ContractID = a.Order.Contract.ContractId, Title = a.Order.Contract.Title }))
            .ForMember(a => a.Offer, opt => opt.MapFrom(a => new Dto.Offer
            {
                Id = a.Order.Offer.Id,
                Code = a.Order.Offer.Code,
                Name = a.Order.Offer.Name,
                AssetType = new Dto.CodeNameEntity() { Id = a.Order.Offer.AssetType.Id, Code = a.Order.Offer.AssetType.Code, Name = a.Order.Offer.AssetType.Name },
                Request = new Dto.Request { Id = a.Order.Offer.Request.Id, Code = a.Order.Offer.Request.Code, Name = a.Order.Offer.Request.Name }
            }))
            .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Order.Offer.Request.Employee.Id, InternalCode = a.Order.Offer.Request.Employee.InternalCode, FirstName = a.Order.Offer.Request.Employee.FirstName, LastName = a.Order.Offer.Request.Employee.LastName, Email = a.Order.Offer.Request.Employee.Email }))
            .ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Order.Offer.Request.AssetType.Id, Code = a.Order.Offer.Request.AssetType.Code, Name = a.Order.Offer.Request.AssetType.Name }))
            .ForMember(a => a.Division, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Order.Division.Id, Code = a.Order.Division.Code, Name = a.Order.Division.Name }))
            .ForMember(a => a.ProjectType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Order.Offer.Request.ProjectType.Id, Code = a.Order.Offer.Request.ProjectType.Code, Name = a.Order.Offer.Request.ProjectType.Name }))
            .ForMember(a => a.Division, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Order.Offer.Request.CostCenter.Division.Id, Code = a.Order.Offer.Request.CostCenter.Division.Code, Name = a.Order.Offer.Request.CostCenter.Division.Name }))
            .ForMember(a => a.ValueIni, opt => opt.MapFrom(a => a.Order.ValueIni))
            .ForMember(a => a.ValueFin, opt => opt.MapFrom(a => a.Order.ValueFin))
            .ForMember(a => a.ValueUsed, opt => opt.MapFrom(a => a.Order.ValueUsed))
            .ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Order.Quantity))
            .ForMember(a => a.QuantityRem, opt => opt.MapFrom(a => a.Order.QuantityRem))
            .ForMember(a => a.QuantityUsed, opt => opt.MapFrom(a => a.Order.QuantityUsed))
            .ForMember(a => a.BudgetValueNeed, opt => opt.MapFrom(a => a.Order.BudgetValueNeed))
            .ForMember(a => a.Price, opt => opt.MapFrom(a => a.Order.Price))
            .ForMember(a => a.AppStateId, opt => opt.MapFrom(a => a.Order.AppStateId))
            .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Order.AppState.Id, Code = a.Order.AppState.Code, Name = a.Order.AppState.Name }));



            CreateMap<Model.Order, Dto.Order>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Id))
            .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.IsAccepted))
            .ForMember(a => a.Code, opt => opt.MapFrom(a => a.Code))
            .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Name))
            .ForMember(a => a.Offer, opt => opt.MapFrom(a => new Dto.Offer 
            { 
                Id = a.Offer.Id, 
                Code = a.Offer.Code, 
                Name = a.Offer.Name, 
                Partner = new Dto.CodePartnerEntity 
                { 
                    Id = a.Offer.Partner != null ? a.Offer.Partner.Id : 0, 
                    Name = a.Offer.Partner != null ? a.Offer.Partner.Name : "" 
                },
				Request = new Dto.Request
				{
					Id = a.Offer.Request != null ? a.Offer.Request.Id : 0,
					Code = a.Offer.Request != null ? a.Offer.Request.Code : ""
				}
			}))
            .ForMember(a => a.OrderType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.OrderType.Id, Code = a.OrderType.Code, Name = a.OrderType.Name }))
			.ForMember(a => a.Project, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.Request.Project.Id, Code = a.Offer.Request.Project.Code, Name = a.Offer.Request.Project.Name }))
			.ForMember(a => a.ProjectType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.Request.ProjectType.Id, Code = a.Offer.Request.ProjectType.Code, Name = a.Offer.Request.ProjectType.Name }))
			.ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.Request.AssetType.Id, Code = a.Offer.Request.AssetType.Code, Name = a.Offer.Request.AssetType.Name }))
			.ForMember(a => a.Contract, opt => opt.MapFrom(a => new Dto.Contract { Id = a.Contract.Id, Title = a.Contract.Title, ContractID = a.Contract.ContractId }))
            .ForMember(a => a.Uom, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Uom.Id, Code = a.Uom.Code, Name = a.Uom.Name }))
            .ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Company.Id, Code = a.Company.Code, Name = a.Company.Name }))
            .ForMember(a => a.Division, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Division.Id, Code = a.Division.Code, Name = a.Division.Name }))
            .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Employee.Id, InternalCode = a.Employee.InternalCode, FirstName = a.Employee.FirstName, LastName = a.Employee.LastName, Email = a.Employee.Email }))
			.ForMember(a => a.Owner, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Offer.Request.Owner.Id, Email = a.Offer.Request.Owner.Email }))
			.ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.AccMonth.Id, Year = a.AccMonth.Year }))
            .ForMember(a => a.Partner, opt => opt.MapFrom(a => new Dto.CodePartnerEntity { Id = a.Partner.Id, Name = a.Partner.Name }))
            .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.AppState { Id = a.AppState.Id, Code = a.AppState.Code, Name = a.AppState.Name,
				BadgeColor = a.AppState.BadgeColor,
				BadgeIcon = a.AppState.BadgeIcon
			}));


            CreateMap<Model.RequestDetail, Dto.Request>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Request.Id))
            .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.Request.IsAccepted))
			.ForMember(a => a.CreatedAt, opt => opt.MapFrom(a => a.Request.CreatedAt))
			.ForMember(a => a.ModifiedAt, opt => opt.MapFrom(a => a.Request.ModifiedAt))
			.ForMember(a => a.Code, opt => opt.MapFrom(a => a.Request.Code))
            .ForMember(a => a.BudgetValueNeed, opt => opt.MapFrom(a => a.Request.BudgetValueNeed))
            .ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Request.Quantity))
            .ForMember(a => a.EndDate, opt => opt.MapFrom(a => a.Request.EndDate))
            .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Request.Name))
            .ForMember(a => a.StartAccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.Request.StartAccMonth.Id, Month = a.Request.StartAccMonth.Month, Year = a.Request.StartAccMonth.Year }))
            .ForMember(a => a.InfoMinus, opt => opt.MapFrom(a => a.Request.Name))
            .ForMember(a => a.Info, opt => opt.MapFrom(a => a.Request.Info))
            .ForMember(a => a.AppStateId, opt => opt.MapFrom(a => a.Request.AppStateId))
            .ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Request.Company.Id, Code = a.Request.Company.Code, Name = a.Request.Company.Name }))
            .ForMember(a => a.Division, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Request.Division.Id, Code = a.Request.Division.Code, Name = a.Request.Division.Name }))
            .ForMember(a => a.ProjectType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Request.ProjectType.Id, Code = a.Request.ProjectType.Code, Name = a.Request.ProjectType.Name }))
            .ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Request.AssetType.Id, Code = a.Request.AssetType.Code, Name = a.Request.AssetType.Name }))
            .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Request.Employee.Id, InternalCode = a.Request.Employee.InternalCode, FirstName = a.Request.Employee.FirstName, LastName = a.Request.Employee.LastName, Email = a.Request.Employee.Email }))
            .ForMember(a => a.Owner, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Request.Owner.Id, InternalCode = a.Request.Owner.InternalCode, FirstName = a.Request.Owner.FirstName, LastName = a.Request.Owner.LastName, Email = a.Request.Owner.Email }))
            .ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.Request.AccMonth.Id, Year = a.Request.AccMonth.Year }))
            .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.AppState { Id = a.Request.AppState.Id, Code = a.Request.AppState.Code, Name = a.Request.AppState.Name, BadgeColor = a.Request.AppState.BadgeColor, BadgeIcon= a.Request.AppState.BadgeIcon }));

            CreateMap<Model.RequestDetail, Dto.RequestUI>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Request.Id))
            .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.Request.IsAccepted))
			.ForMember(a => a.CreatedAt, opt => opt.MapFrom(a => a.Request.CreatedAt))
			.ForMember(a => a.ModifiedAt, opt => opt.MapFrom(a => a.Request.ModifiedAt))
			.ForMember(a => a.Code, opt => opt.MapFrom(a => a.Request.Code))
            .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Request.Name))
            .ForMember(a => a.Info, opt => opt.MapFrom(a => a.Request.Info))
            .ForMember(a => a.BudgetValueNeed, opt => opt.MapFrom(a => a.Request.BudgetValueNeed))
            .ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Request.Quantity))
            .ForMember(a => a.EndDate, opt => opt.MapFrom(a => a.Request.EndDate))
            .ForMember(a => a.StartAccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.Request.StartAccMonth.Id, Month = a.Request.StartAccMonth.Month, Year = a.Request.StartAccMonth.Year }))
            .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Request.Employee.Id, InternalCode = a.Request.Employee.InternalCode, FirstName = a.Request.Employee.FirstName, LastName = a.Request.Employee.LastName, Email = a.Request.Employee.Email }))
            .ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Request.AssetType.Id, Code = a.Request.AssetType.Code, Name = a.Request.AssetType.Name }))
            .ForMember(a => a.ProjectType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Request.ProjectType.Id, Code = a.Request.ProjectType.Code, Name = a.Request.ProjectType.Name }))
            .ForMember(a => a.Division, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Request.Division.Id, Code = a.Request.Division.Code, Name = a.Request.Division.Name }))
			.ForMember(a => a.Department, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Request.Division.Department.Id, Code = a.Request.Division.Department.Code, Name = a.Request.Division.Department.Name }))
			.ForMember(a => a.AppStateId, opt => opt.MapFrom(a => a.Request.AppStateId))
            .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Request.AppState.Id, Code = a.Request.AppState.Code, Name = a.Request.AppState.Name }));



            CreateMap<Model.Request, Dto.Request>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Id))
            .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.IsAccepted))
			.ForMember(a => a.CreatedAt, opt => opt.MapFrom(a => a.CreatedAt))
			.ForMember(a => a.Code, opt => opt.MapFrom(a => a.Code))
            .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Name))
			.ForMember(a => a.CreatedAt, opt => opt.MapFrom(a => a.CreatedAt))
			.ForMember(a => a.ModifiedAt, opt => opt.MapFrom(a => a.ModifiedAt))
			.ForMember(a => a.StartExecution, opt => opt.MapFrom(a => a.StartExecution))
			.ForMember(a => a.EndExecution, opt => opt.MapFrom(a => a.EndExecution))
			.ForMember(a => a.User, opt => opt.MapFrom(a => new Dto.ApplicationUser { Id = a.User.Id, Email = a.User.Email, UserName = a.User.UserName }))
			.ForMember(a => a.BudgetManager, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.BudgetManager.Id, Code = a.BudgetManager.Code, Name = a.BudgetManager.Name }))
			.ForMember(a => a.ProjectType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.ProjectType.Id, Code = a.ProjectType.Code, Name = a.ProjectType.Name }))
			.ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.AssetType.Id, Code = a.AssetType.Code, Name = a.AssetType.Name }))
			.ForMember(a => a.Project, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Project.Id, Code = a.Project.Code, Name = a.Project.Name }))
			.ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Company.Id, Code = a.Company.Code, Name = a.Company.Name }))
            .ForMember(a => a.Division, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Division.Id, Code = a.Division.Code, Name = a.Division.Name }))
            .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Employee.Id, InternalCode = a.Employee.InternalCode, FirstName = a.Employee.FirstName, LastName = a.Employee.LastName }))
            .ForMember(a => a.Owner, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Owner.Id, InternalCode = a.Owner.InternalCode, FirstName = a.Owner.FirstName, LastName = a.Owner.LastName, Email = a.Owner.Email }))
            .ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.AccMonth.Id, Year = a.AccMonth.Year, Month = a.AccMonth.Month, FiscalMonth = a.AccMonth.FiscalMonth , FiscalYear = a.AccMonth.FiscalYear }))
            .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.AppState { Id = a.AppState.Id, Code = a.AppState.Code, Name = a.AppState.Name, BadgeIcon = a.AppState.BadgeIcon, BadgeColor = a.AppState.BadgeColor }));




            CreateMap<Model.OfferDetail, Dto.Offer>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Offer.Id))
            .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.Offer.IsAccepted))
			.ForMember(a => a.CreatedAt, opt => opt.MapFrom(a => a.Offer.CreatedAt))
			.ForMember(a => a.ModifiedAt, opt => opt.MapFrom(a => a.Offer.ModifiedAt))
			.ForMember(a => a.Code, opt => opt.MapFrom(a => a.Offer.Code))
            .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Offer.Name))
            .ForMember(a => a.InfoMinus, opt => opt.MapFrom(a => a.Offer.Name))
            .ForMember(a => a.ValueIni, opt => opt.MapFrom(a => a.Offer.ValueIni))
            .ForMember(a => a.ValueFin, opt => opt.MapFrom(a => a.Offer.ValueFin))
            .ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Offer.Quantity))
            .ForMember(a => a.QuantityRem, opt => opt.MapFrom(a => a.Offer.QuantityRem))
            .ForMember(a => a.QuantityUsed, opt => opt.MapFrom(a => a.Offer.QuantityUsed))
            .ForMember(a => a.Info, opt => opt.MapFrom(a => a.Offer.Info))
            .ForMember(a => a.AppStateId, opt => opt.MapFrom(a => a.Offer.AppStateId))
            //.ForMember(i => i.OfferMaterialss, opt => opt.MapFrom(i => i.Offer.OfferMaterials.ToList().FirstOrDefault().Offer.OfferMaterials))
			.ForMember(i => i.OfferMaterials, opt => opt.MapFrom(i => i.Offer.OfferMaterials.Where(p => p.IsDeleted == false).Select(p => new Dto.OfferMaterial()
			{
				Id = p.Id
			})))
			//.ForMember(dest => dest.OfferMaterials,
			//             opt => opt.MapFrom(
			//                 src => Mapper.Map<List<Dto.OfferMaterial>,
			//                                   List<Dto.OfferMaterial>>(src.Offer.OfferMaterials.ToList().Select(b => new Dto.OfferMaterial { Id = b.Id }))))
			.ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.Company.Id, Code = a.Offer.Company.Code, Name = a.Offer.Company.Name }))
            .ForMember(a => a.Division, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.Division.Id, Code = a.Offer.Division.Code, Name = a.Offer.Division.Name }))
            .ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.AssetType.Id, Code = a.Offer.AssetType.Code, Name = a.Offer.AssetType.Name }))
            .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Offer.Employee.Id, InternalCode = a.Offer.Employee.InternalCode, FirstName = a.Offer.Employee.FirstName, LastName = a.Offer.Employee.LastName }))
            .ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.Offer.AccMonth.Id, Year = a.Offer.AccMonth.Year }))
            .ForMember(a => a.Partner, opt => opt.MapFrom(a => new Dto.CodePartnerEntity { Id = a.Offer.Partner.Id, Name = a.Offer.Partner.Name }))
            .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.AppState { Id = a.Offer.AppState.Id, Code = a.Offer.AppState.Code, Name = a.Offer.AppState.Name, BadgeColor = a.Offer.AppState.BadgeColor, BadgeIcon = a.Offer.AppState.BadgeIcon }))
            .ForMember(a => a.Request, opt => opt.MapFrom(a => new Dto.Request { Id = a.Offer.Request.Id, Code = a.Offer.Request.Code, Name = a.Offer.Request.Name }));
            


            CreateMap<Model.OfferDetail, Dto.OfferUI>()
           .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Offer.Id))
           .ForMember(a => a.Code, opt => opt.MapFrom(a => a.Offer.Code))
           .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Offer.Name))
		   .ForMember(a => a.ValueIni, opt => opt.MapFrom(a => a.Offer.ValueIni))
           .ForMember(a => a.ValueIniRon, opt => opt.MapFrom(a => a.Offer.ValueIniRon))
           .ForMember(a => a.ValueFin, opt => opt.MapFrom(a => a.Offer.ValueFin))
            .ForMember(a => a.ValueFinRon, opt => opt.MapFrom(a => a.Offer.ValueFinRon))
           .ForMember(a => a.ValueUsed, opt => opt.MapFrom(a => a.Offer.ValueUsed))
            .ForMember(a => a.ValueUsedRon, opt => opt.MapFrom(a => a.Offer.ValueUsedRon))
           .ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Offer.Quantity))
		   .ForMember(a => a.QuantityRem, opt => opt.MapFrom(a => a.Offer.QuantityRem))
           .ForMember(a => a.QuantityUsed, opt => opt.MapFrom(a => a.Offer.QuantityUsed))
           .ForMember(a => a.AppStateId, opt => opt.MapFrom(a => a.Offer.AppStateId))
           .ForMember(a => a.OfferType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.OfferType.Id, Code = a.Offer.OfferType.Code, Name = a.Offer.OfferType.Name }))
           .ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.Company.Id, Code = a.Offer.Company.Code, Name = a.Offer.Company.Name }))
           .ForMember(a => a.Uom, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.Uom.Id, Code = a.Offer.Uom.Code, Name = a.Offer.Uom.Name }))
           .ForMember(a => a.Division, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.Division.Id, Code = a.Offer.Division.Code, Name = a.Offer.Division.Name }))
           .ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.AssetType.Id, Code = a.Offer.AssetType.Code, Name = a.Offer.AssetType.Name }))
           .ForMember(a => a.ProjectType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.ProjectType.Id, Code = a.Offer.ProjectType.Code, Name = a.Offer.ProjectType.Name }))
           .ForMember(a => a.Request, opt => opt.MapFrom(a => new Dto.Request { Id = a.Offer.Request.Id, Code = a.Offer.Request.Code, Name = a.Offer.Request.Name }))
           .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Offer.Employee.Id, InternalCode = a.Offer.Employee.InternalCode, FirstName = a.Offer.Employee.FirstName, LastName = a.Offer.Employee.LastName }))
		   .ForMember(a => a.Partner, opt => opt.MapFrom(a => new Dto.CodePartnerEntity { Id = a.Offer.Partner.Id, Name = a.Offer.Partner.Name }))
		   .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.AppState.Id, Code = a.Offer.AppState.Code, Name = a.Offer.AppState.Name }));


            CreateMap<Model.Offer, Dto.Offer>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Id))
            .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.IsAccepted))
            .ForMember(a => a.Code, opt => opt.MapFrom(a => a.Code))
            .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Name))
            .ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Company.Id, Code = a.Company.Code, Name = a.Company.Name }))
            .ForMember(a => a.Division, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Division.Id, Code = a.Division.Code, Name = a.Division.Name }))
            .ForMember(a => a.AssetType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.AssetType.Id, Code = a.AssetType.Code, Name = a.AssetType.Name }))
            .ForMember(a => a.ProjectType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.ProjectType.Id, Code = a.ProjectType.Code, Name = a.ProjectType.Name }))
            .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Employee.Id, InternalCode = a.Employee.InternalCode, FirstName = a.Employee.FirstName, LastName = a.Employee.LastName }))
            .ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.AccMonth.Id, Year = a.AccMonth.Year }))
            .ForMember(a => a.Partner, opt => opt.MapFrom(a => new Dto.CodePartnerEntity { Id = a.Partner.Id, Name = a.Partner.Name }))
            .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.AppState { Id = a.AppState.Id, Code = a.AppState.Code, Name = a.AppState.Name, BadgeIcon = a.AppState.BadgeIcon, BadgeColor = a.AppState.BadgeColor }));


            CreateMap<Model.ContractDetail, Dto.Contract>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Contract.Id))
            .ForMember(a => a.Title, opt => opt.MapFrom(a => a.Contract.Title))
            .ForMember(a => a.ContractID, opt => opt.MapFrom(a => a.Contract.ContractId))
            .ForMember(a => a.Code, opt => opt.MapFrom(a => a.Contract.Code))
            .ForMember(a => a.EffectiveDate, opt => opt.MapFrom(a => a.Contract.EffectiveDate))
            .ForMember(a => a.AgreementDate, opt => opt.MapFrom(a => a.Contract.AgreementDate))
            .ForMember(a => a.ExpirationDate, opt => opt.MapFrom(a => a.Contract.ExpirationDate))
            .ForMember(a => a.CreationDate, opt => opt.MapFrom(a => a.Contract.CreationDate))
            .ForMember(a => a.Version, opt => opt.MapFrom(a => a.Contract.Version))
            .ForMember(a => a.TemplateId, opt => opt.MapFrom(a => a.Contract.TemplateId))
            .ForMember(a => a.AmendmentType, opt => opt.MapFrom(a => a.Contract.AmendmentType))
            .ForMember(a => a.AmendmentReason, opt => opt.MapFrom(a => a.Contract.AmendmentReason))
            .ForMember(a => a.Origin, opt => opt.MapFrom(a => a.Contract.Origin))
            .ForMember(a => a.HierarchicalType, opt => opt.MapFrom(a => a.Contract.HierarchicalType))
            .ForMember(a => a.ExpirationTermType, opt => opt.MapFrom(a => a.Contract.ExpirationTermType))
            .ForMember(a => a.RelatedId, opt => opt.MapFrom(a => a.Contract.RelatedId))
            .ForMember(a => a.MaximumNumberOfRenewals, opt => opt.MapFrom(a => a.Contract.MaximumNumberOfRenewals))
            .ForMember(a => a.AutoRenewalInterval, opt => opt.MapFrom(a => a.Contract.AutoRenewalInterval))
            .ForMember(a => a.IsTestProject, opt => opt.MapFrom(a => a.Contract.IsTestProject))
            .ForMember(a => a.AppStateId, opt => opt.MapFrom(a => a.Contract.AppStateId))
            .ForMember(a => a.BusinessSystem, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Contract.BusinessSystem.Id, Code = a.Contract.BusinessSystem.Code, Name = a.Contract.BusinessSystem.Name }))
            .ForMember(a => a.ContractAmount, opt => opt.MapFrom(a => new Dto.ContractAmountEntity { 
                Id = a.Contract.ContractAmount.Id, 
                Amount = a.Contract.ContractAmount.Amount,
                AmountRon = a.Contract.ContractAmount.AmountRon,
                AmountRem = a.Contract.ContractAmount.AmountRem,
                AmountRonRem = a.Contract.ContractAmount.AmountRonRem,
                AmountUsed = a.Contract.ContractAmount.AmountUsed,
                AmountRonUsed = a.Contract.ContractAmount.AmountRonUsed,
                Uom = new Dto.CodeNameEntity() { Id = a.Contract.ContractAmount.Uom.Id, Code = a.Contract.ContractAmount.Uom.Code}, 
                Rate = new Dto.RateEntity() { Id = a.Contract.ContractAmount.Rate.Id, Code = a.Contract.ContractAmount.Rate.Code, Name = a.Contract.ContractAmount.Rate.Name, Value = a.Contract.ContractAmount.Rate.Value, Multiplier = a.Contract.ContractAmount.Rate.Multiplier } }))
            .ForMember(a => a.Owner, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Contract.Owner.Id, InternalCode = a.Contract.Owner.FullName }))
            //.ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.Contract.AccMonth.Id, Year = a.Contract.AccMonth.Year.ToString() }))
            .ForMember(a => a.Partner, opt => opt.MapFrom(a => new Dto.CodePartnerEntity { Id = a.Contract.Partner.Id, Name = a.Contract.Partner.Name, RegistryNumber = a.Contract.Partner.RegistryNumber }))
            .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.AppState { Id = a.Contract.AppState.Id, Code = a.Contract.AppState.Code, Name = a.Contract.AppState.Name, BadgeColor = a.Contract.AppState.BadgeColor, BadgeIcon = a.Contract.AppState.BadgeIcon }));


            CreateMap<Model.ContractDetail, Dto.ContractUI>()
           .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Contract.Id))
           //.ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.Offer.IsAccepted))
           .ForMember(a => a.Title, opt => opt.MapFrom(a => a.Contract.Title))
           .ForMember(a => a.ContractID, opt => opt.MapFrom(a => a.Contract.ContractId))
           .ForMember(a => a.BusinessSystem, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Contract.BusinessSystem.Id, Code = a.Contract.BusinessSystem.Code, Name = a.Contract.BusinessSystem.Name }))
            .ForMember(a => a.ContractAmount, opt => opt.MapFrom(a => new Dto.ContractAmountEntity { 
                Id = a.Contract.ContractAmount.Id, 
                Amount = a.Contract.ContractAmount.Amount,
                AmountRon = a.Contract.ContractAmount.AmountRon,
                AmountRem = a.Contract.ContractAmount.AmountRem,
                AmountRonRem = a.Contract.ContractAmount.AmountRonRem,
                AmountUsed = a.Contract.ContractAmount.AmountUsed,
                AmountRonUsed = a.Contract.ContractAmount.AmountRonUsed,
                Uom = a.Contract.ContractAmount.Uom != null ? new Dto.CodeNameEntity() { Id = a.Contract.ContractAmount.Uom.Id, Code = a.Contract.ContractAmount.Uom.Code } : null,
                Rate = a.Contract.ContractAmount.Rate != null ? new Dto.RateEntity() { Id = a.Contract.ContractAmount.Rate.Id, Code = a.Contract.ContractAmount.Rate.Code, Name = a.Contract.ContractAmount.Rate.Name, Value = a.Contract.ContractAmount.Rate.Value } : null,
                RateRon = a.Contract.ContractAmount.RateRon != null ? new Dto.RateEntity() {
                    Id = a.Contract.ContractAmount.RateRon.Id,
                    Code = a.Contract.ContractAmount.RateRon.Code,
                    Name = a.Contract.ContractAmount.RateRon.Name,
                    Value = a.Contract.ContractAmount.RateRon.Value
                } : null
            }))
            .ForMember(a => a.Owner, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Contract.Owner.Id, InternalCode = a.Contract.Owner.FullName }))
            //.ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.Contract.AccMonth.Id, Year = a.Contract.AccMonth.Year.ToString() }))
            .ForMember(a => a.Partner, opt => opt.MapFrom(a => new Dto.CodePartnerEntity { Id = a.Contract.Partner.Id, Name = a.Contract.Partner.Name, RegistryNumber = a.Contract.Partner.RegistryNumber }))
           //.ForMember(a => a.InfoMinus, opt => opt.MapFrom(a => a.Offer.Name))
           //.ForMember(a => a.ValueIni, opt => opt.MapFrom(a => a.Offer.ValueIni))
           //.ForMember(a => a.ValueFin, opt => opt.MapFrom(a => a.Offer.ValueFin))
           //.ForMember(a => a.Info, opt => opt.MapFrom(a => a.Offer.Info))
           .ForMember(a => a.AppStateId, opt => opt.MapFrom(a => a.Contract.AppStateId))
           // .ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Contract.Company.Id, Code = a.Contract.Company.Code, Name = a.Contract.Company.Name }))
           //.ForMember(a => a.Administration, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.Administration.Id, Code = a.Offer.Administration.Code, Name = a.Offer.Administration.Name }))
           //.ForMember(a => a.MasterType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.SubType.Type.MasterType.Id, Code = a.Offer.SubType.Type.MasterType.Code, Name = a.Offer.SubType.Type.MasterType.Name }))
           //.ForMember(a => a.Type, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.SubType.Type.Id, Code = a.Offer.SubType.Type.Code, Name = a.Offer.SubType.Type.Name }))
           //.ForMember(a => a.SubType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.SubType.Id, Code = a.Offer.SubType.Code, Name = a.Offer.SubType.Name }))
           // .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Contract.Employee.Id, InternalCode = a.Contract.Employee.InternalCode, FirstName = a.Contract.Employee.FirstName, LastName = a.Contract.Employee.LastName }))
           //.ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.Offer.AccMonth.Id, Year = a.Offer.AccMonth.Year.ToString() }))
           //.ForMember(a => a.InterCompany, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.InterCompany.Id, Code = a.Offer.InterCompany.Code, Name = a.Offer.InterCompany.Name }))
           //.ForMember(a => a.Account, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.Account.Id, Code = a.Offer.Account.Code, Name = a.Offer.Account.Name }))
           .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Contract.AppState.Id, Code = a.Contract.AppState.Code, Name = a.Contract.AppState.Name }));
            //.ForMember(a => a.CostCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Offer.CostCenter.Id, Code = a.Offer.CostCenter.Code, Name = a.Offer.CostCenter.Name }));


            CreateMap<Model.Contract, Dto.Contract>()
            .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Id))
            //.ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.IsAccepted))
            //.ForMember(a => a.Code, opt => opt.MapFrom(a => a.Code))
            //.ForMember(a => a.Name, opt => opt.MapFrom(a => a.Name))
            //.ForMember(a => a.Company, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Company.Id, Code = a.Company.Code, Name = a.Company.Name }))
            // .ForMember(a => a.Administration, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Administration.Id, Code = a.Administration.Code, Name = a.Administration.Name }))
            //.ForMember(a => a.MasterType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.SubType.Type.MasterType.Id, Code = a.SubType.Type.MasterType.Code, Name = a.SubType.Type.MasterType.Name }))
            //.ForMember(a => a.Type, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.SubType.Type.Id, Code = a.SubType.Type.Code, Name = a.SubType.Type.Name }))
            //.ForMember(a => a.SubType, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.SubType.Id, Code = a.SubType.Code, Name = a.SubType.Name }))
            //.ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Employee.Id, InternalCode = a.Employee.InternalCode, FirstName = a.Employee.FirstName, LastName = a.Employee.LastName }))
            //.ForMember(a => a.AccMonth, opt => opt.MapFrom(a => new Dto.MonthEntity { Id = a.AccMonth.Id, Year = a.AccMonth.Year.ToString() }))
            //.ForMember(a => a.Partner, opt => opt.MapFrom(a => new Dto.CodePartnerEntity { Id = a.Partner.Id, Name = a.Partner.Name }))
            .ForMember(a => a.AppState, opt => opt.MapFrom(a => new Dto.AppState { Id = a.AppState.Id, Code = a.AppState.Code, Name = a.AppState.Name, BadgeColor= a.AppState.BadgeColor, BadgeIcon = a.AppState.BadgeIcon }));


            CreateMap<Model.Asset, Dto.AssetUI>()
               .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Id))
               .ForMember(a => a.IsAccepted, opt => opt.MapFrom(a => a.IsAccepted))
               .ForMember(a => a.InvNo, opt => opt.MapFrom(a => a.InvNo))
               .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Name))
               .ForMember(a => a.PurchaseDate, opt => opt.MapFrom(a => a.PurchaseDate))
               .ForMember(a => a.SerialNumber, opt => opt.MapFrom(a => a.SerialNumber))
               .ForMember(a => a.Room, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Room.Id, Code = a.Room.Code, Name = a.Room.Name }));


            CreateMap<Model.AssetComponent, Dto.AssetUI>()
             .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Id))
             .ForMember(a => a.InvNo, opt => opt.MapFrom(a => a.Code))
             .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Name));


            //.ForMember(d => d.Dep, o => o.MapFrom(s => s.AssetDeps.Select(c => c.ValueInvIn).ToArray()))
            //.ForMember(dest => dest.Dep,
            //           opt => opt.MapFrom(
            //               src => Mapper.Map<List<Model.AssetDep>,
            //                                 Dto.AssetDepDetail>(dest)))

            // .ForMember(a => a.Inv, opt => opt.MapFrom(a => new Dto.AssetInvDetail
            // {

            //   }));

            //CreateMap<List<Model.Asset>, Dto.Asset>()
            //   .ForMember(dest => dest.Dep,
            //              opt => opt.MapFrom(
            //                  src => Mapper.Map<List<Model.Asset>,
            //                                    List<Dto.Asset>>(src)));


            //.ForMember(a => a.CostCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.CostCenter.Id, Code = a.CostCenter.Code, Name = a.CostCenter.Name }));
            //.ForMember(a => a.CostCenter, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.CostCenter.Id, Code = a.CostCenter.Code, Name = a.CostCenter.Name }))
            //.ForMember(a => a.AssetState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.AssetState.Id, Code = a.AssetState.Code, Name = a.AssetState.Name }))
            //.ForMember(a => a.AssetState, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.AssetState.Id, Code = a.AssetState.Code, Name = a.AssetState.Name }))
            //.ForMember(a => a.Department, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Department.Id, Code = a.Department.Code, Name = a.Department.Name }))
            //.ForMember(a => a.Department, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Department.Id, Code = a.Department.Code, Name = a.Department.Name }))
            //.ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Employee.Id, InternalCode = a.Employee.InternalCode, FirstName = a.Employee.FirstName, LastName = a.Employee.LastName }))
            //.ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.EmployeeResource { Id = a.Employee.Id, InternalCode = a.Employee.InternalCode, FirstName = a.Employee.FirstName, LastName = a.Employee.LastName }))
            //.ForMember(a => a.Room, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Room.Id, Code = a.Room.Code, Name = a.Room.Name }))
            //.ForMember(a => a.Room, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.Room.Id, Code = a.Room.Code, Name = a.Room.Name }))
            //.ForMember(a => a.AssetCategory, opt => opt.MapFrom(a => new Dto.CodeNameEntity { Id = a.AssetCategory.Id, Code = a.AssetCategory.Code, Name = a.AssetCategory.Name }));

            CreateMap<Model.AssetOp, Dto.AssetOp>()
            .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
            .ForMember(i => i.Asset, opt => opt.MapFrom(i => new Dto.AssetBase { Id = i.Asset.Id, InvNo = i.Asset.InvNo, Name = i.Asset.Name, PurchaseDate = i.Asset.PurchaseDate, ERPCode = i.Asset.ERPCode, Quantity = i.Asset.Quantity, ValueInv = i.Asset.ValueInv, SerialNumber = i.Asset.SerialNumber, TempReco = i.Asset.TempReco, TempName = i.Asset.TempName, TempSerialNumber = i.Asset.TempSerialNumber }))
            .ForMember(i => i.AccSystem, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AccSystem.Id, Code = i.AccSystem.Code, Name = i.AccSystem.Name }))
            .ForMember(i => i.AdministrationInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AdministrationInitial.Id, Code = i.AdministrationInitial.Code, Name = i.AdministrationInitial.Name }))
            .ForMember(i => i.AdministrationFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AdministrationFinal.Id, Code = i.AdministrationFinal.Code, Name = i.AdministrationFinal.Name }))
            .ForMember(i => i.CostCenterInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenterInitial.Id, Code = i.CostCenterInitial.Code, Name = i.CostCenterInitial.Name }))
            .ForMember(i => i.CostCenterFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenterFinal.Id, Code = i.CostCenterFinal.Code, Name = i.CostCenterFinal.Name }))
            .ForMember(i => i.AssetStateFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AssetStateFinal.Id, Code = i.AssetStateFinal.Code, Name = i.AssetStateFinal.Name }))
            .ForMember(i => i.AssetStateInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AssetStateInitial.Id, Code = i.AssetStateInitial.Code, Name = i.AssetStateInitial.Name }))
            .ForMember(i => i.DepartmentInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.DepartmentInitial.Id, Code = i.DepartmentInitial.Code, Name = i.DepartmentInitial.Name }))
            .ForMember(i => i.DepartmentFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.DepartmentFinal.Id, Code = i.DepartmentFinal.Code, Name = i.DepartmentFinal.Name }))
            .ForMember(i => i.EmployeeInitial, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeInitial.Id, InternalCode = i.EmployeeInitial.InternalCode, FirstName = i.EmployeeInitial.FirstName, LastName = i.EmployeeInitial.LastName, Email = i.EmployeeInitial.Email }))
            .ForMember(i => i.EmployeeFinal, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeFinal.Id, InternalCode = i.EmployeeFinal.InternalCode, FirstName = i.EmployeeFinal.FirstName, LastName = i.EmployeeFinal.LastName, Email = i.EmployeeFinal.Email,
            Manager = new Dto.EmployeeResource
            {
                Email = i.EmployeeFinal != null && i.EmployeeFinal.Manager != null ? i.EmployeeFinal.Manager.Email : "",
            }
            }))
            .ForMember(i => i.RoomInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RoomInitial.Id, Code = i.RoomInitial.Code, Name = i.RoomInitial.Name }))
            .ForMember(i => i.RoomFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RoomFinal.Id, Code = i.RoomFinal.Code, Name = i.RoomFinal.Name }))
            .ForMember(i => i.AssetCategoryInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AssetCategoryInitial.Id, Code = i.AssetCategoryInitial.Code, Name = i.AssetCategoryInitial.Name }))
            .ForMember(i => i.AssetCategoryFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AssetCategoryFinal.Id, Code = i.AssetCategoryFinal.Code, Name = i.AssetCategoryFinal.Name }))
            .ForMember(i => i.InvStateInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.InvStateInitial.Id, Code = i.InvStateInitial.Code, Name = i.InvStateInitial.Name }))
            .ForMember(i => i.InvStateFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.InvStateFinal.Id, Code = i.InvStateFinal.Code, Name = i.InvStateFinal.Name }))
            .ForMember(i => i.AssetOpState, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AssetOpState.Id, Code = i.AssetOpState.Code, Name = i.AssetOpState.Name }))

            .ForMember(i => i.DstConfAt, opt => opt.MapFrom(i => i.DstConfAt))

            .ForMember(i => i.SrcConfAt, opt => opt.MapFrom(i => i.SrcConfAt))
            .ForMember(i => i.SrcConfUser, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.SrcConfUser.EmployeeId.GetValueOrDefault(), InternalCode = i.SrcConfUser.Employee.InternalCode, FirstName = i.SrcConfUser.Employee.FirstName, LastName = i.SrcConfUser.Employee.LastName }))
            .ForMember(i => i.SrcConfDepartment, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SrcConfUser.Employee.DepartmentId.GetValueOrDefault(), Code = i.SrcConfUser.Employee.Department.Code, Name = i.SrcConfUser.Employee.Department.Name }))

            .ForMember(i => i.State, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AssetOpStateId.GetValueOrDefault(), Code = i.AssetOpState.Code, Name = i.AssetOpState.Name }))

            .ForMember(i => i.ReleaseConfAt, opt => opt.MapFrom(i => i.ReleaseConfAt))
            .ForMember(i => i.ReleaseConfUser, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.ReleaseConfUser.EmployeeId.GetValueOrDefault(), InternalCode = i.ReleaseConfUser.Employee.InternalCode, FirstName = i.ReleaseConfUser.Employee.FirstName, LastName = i.ReleaseConfUser.Employee.LastName }))
            .ForMember(i => i.ReleaseConfDepartment, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.ReleaseConfUser.Employee.DepartmentId.GetValueOrDefault(), Code = i.ReleaseConfUser.Employee.Department.Code, Name = i.ReleaseConfUser.Employee.Department.Name }))

            .ForMember(i => i.RegisterConfAt, opt => opt.MapFrom(i => i.RegisterConfAt))
            .ForMember(i => i.RegisterConfUser, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.RegisterConfUser.EmployeeId.GetValueOrDefault(), InternalCode = i.RegisterConfUser.Employee.InternalCode, FirstName = i.RegisterConfUser.Employee.FirstName, LastName = i.RegisterConfUser.Employee.LastName }))
            .ForMember(i => i.RegisterConfDepartment, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RegisterConfUser.Employee.DepartmentId.GetValueOrDefault(), Code = i.RegisterConfUser.Employee.Department.Code, Name = i.RegisterConfUser.Employee.Department.Name }))

            .ForMember(i => i.DepUpdate, opt => opt.MapFrom(i => i.DepUpdate))
            .ForMember(i => i.ValueAdd, opt => opt.MapFrom(i => i.ValueAdd))
            .ForMember(i => i.From, opt => opt.MapFrom(i => i.Document.DocumentTypeId == 11 ? i.Document.RegisterDate : (DateTime?)null))
            .ForMember(i => i.To, opt => opt.MapFrom(i => i.Document.DocumentTypeId == 11 ? i.Document.ValidationDate : (DateTime?)null))
            .ForMember(i => i.LocationInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RoomInitial.Location.Id, Code = i.RoomInitial.Location.Code, Name = i.RoomInitial.Location.Name }))
            .ForMember(i => i.LocationFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RoomFinal.Location.Id, Code = i.RoomFinal.Location.Code, Name = i.RoomFinal.Location.Name }))
            .ForMember(i => i.RegionInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RoomInitial.Location.Region.Id, Code = i.RoomInitial.Location.Region.Code, Name = i.RoomInitial.Location.Region.Name }))
            .ForMember(i => i.RegionFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RoomFinal.Location.Region.Id, Code = i.RoomFinal.Location.Region.Code, Name = i.RoomFinal.Location.Region.Name }))
            .ForMember(i => i.Document, opt => opt.MapFrom(i => new Dto.Document { Id = i.Document.Id, ValidationDate = i.Document.ValidationDate }))
            .ForMember(i => i.DocumentType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Document.DocumentType.Id, Code = i.Document.DocumentType.Code, Name = i.Document.DocumentType.Name }))
            .ForMember(i => i.ModifiedAt, opt => opt.MapFrom(i => i.ModifiedAt))
            .ForMember(i => i.AssetType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Asset.AssetType.Id, Code = i.Asset.AssetType.Code, Name = i.Asset.AssetType.Name }))

            .ForMember(i => i.BudgetManagerInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.BudgetManagerInitial.Id, Code = i.BudgetManagerInitial.Code, Name = i.BudgetManagerInitial.Name }))
            .ForMember(i => i.BudgetManagerFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.BudgetManagerFinal.Id, Code = i.BudgetManagerFinal.Code, Name = i.BudgetManagerFinal.Name }))

            .ForMember(i => i.ProjectInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.ProjectInitial.Id, Code = i.ProjectInitial.Code, Name = i.ProjectInitial.Name }))
            .ForMember(i => i.ProjectFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.ProjectFinal.Id, Code = i.ProjectFinal.Code, Name = i.ProjectFinal.Name }))

            .ForMember(i => i.DimensionInitial, opt => opt.MapFrom(i => new Dto.Dimension { Id = i.DimensionInitial.Id, Length = i.DimensionInitial.Length }))
            .ForMember(i => i.DimensionFinal, opt => opt.MapFrom(i => new Dto.Dimension { Id = i.DimensionFinal.Id, Length = i.DimensionFinal.Length }));

            //.ForMember(i => i.AssetNatureInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AssetNatureInitial.Id, Code = i.AssetNatureInitial.Code, Name = i.AssetNatureInitial.Name }))
            //.ForMember(i => i.AssetNatureFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AssetNatureFinal.Id, Code = i.AssetNatureFinal.Code, Name = i.AssetNatureFinal.Name }));
            //.ForMember(i => i.InvStateInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.InvStateIdInitial.GetValueOrDefault(), Code = i.InvStateInitial.Code, Name = i.InvStateInitial.Name }))
            //.ForMember(i => i.InvStateFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.InvStateIdFinal.GetValueOrDefault(), Code = i.InvStateFinal.Code, Name = i.InvStateFinal.Name }));






            CreateMap<Dto.AssetOp, Model.AssetOp>();


            CreateMap<Model.BudgetOp, Dto.BudgetOp>()
                 .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                 .ForMember(i => i.CompanyInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CompanyInitial.Id, Code = i.CompanyInitial.Code, Name = i.CompanyInitial.Name }))
                 .ForMember(i => i.CompanyFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CompanyFinal.Id, Code = i.CompanyFinal.Code, Name = i.CompanyFinal.Name }))
                 .ForMember(i => i.ProjectInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.ProjectInitial.Id, Code = i.ProjectInitial.Code, Name = i.ProjectInitial.Name }))
                 .ForMember(i => i.ProjectFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.ProjectFinal.Id, Code = i.ProjectFinal.Code, Name = i.ProjectFinal.Name }))
                 .ForMember(i => i.AdministrationInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AdministrationInitial.Id, Code = i.AdministrationInitial.Code, Name = i.AdministrationInitial.Name }))
                 .ForMember(i => i.AdministrationFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AdministrationFinal.Id, Code = i.AdministrationFinal.Code, Name = i.AdministrationFinal.Name }))
                 .ForMember(i => i.MasterTypeInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeInitial.Type.MasterType.Id, Code = i.SubTypeInitial.Type.MasterType.Code, Name = i.SubTypeInitial.Type.MasterType.Name }))
                 .ForMember(i => i.MasterTypeFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeFinal.Type.MasterType.Id, Code = i.SubTypeFinal.Type.MasterType.Code, Name = i.SubTypeFinal.Type.MasterType.Name }))
                 .ForMember(i => i.TypeInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeInitial.Type.Id, Code = i.SubTypeInitial.Type.Code, Name = i.SubTypeInitial.Type.Name }))
                 .ForMember(i => i.TypeFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeFinal.Type.Id, Code = i.SubTypeFinal.Type.Code, Name = i.SubTypeFinal.Type.Name }))
                 .ForMember(i => i.SubTypeInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeInitial.Id, Code = i.SubTypeInitial.Code, Name = i.SubTypeInitial.Name }))
                 .ForMember(i => i.SubTypeFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeFinal.Id, Code = i.SubTypeFinal.Code, Name = i.SubTypeFinal.Name }))
                 .ForMember(i => i.EmployeeInitial, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeInitial.Id, FirstName = i.EmployeeInitial.FirstName, LastName = i.EmployeeInitial.LastName }))
                 .ForMember(i => i.EmployeeFinal, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeFinal.Id, FirstName = i.EmployeeFinal.FirstName, LastName = i.EmployeeFinal.LastName }))
                 .ForMember(i => i.AccMonth, opt => opt.MapFrom(i => new Dto.MonthEntity { Id = i.AccMonth.Id, Year = i.AccMonth.Year }))
                 .ForMember(i => i.PartnerInitial, opt => opt.MapFrom(i => new Dto.CodePartnerEntity { Id = i.PartnerInitial.Id, Name = i.PartnerInitial.Name }))
                 .ForMember(i => i.PartnerFinal, opt => opt.MapFrom(i => new Dto.CodePartnerEntity { Id = i.PartnerFinal.Id, Name = i.PartnerFinal.Name }))
                 .ForMember(i => i.AccountInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AccountInitial.Id, Code = i.AccountInitial.Code, Name = i.AccountInitial.Name }))
                 .ForMember(i => i.AccountFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AccountFinal.Id, Code = i.AccountFinal.Code, Name = i.AccountFinal.Name }))
                 .ForMember(i => i.CostCenterInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenterInitial.Id, Code = i.CostCenterInitial.Code, Name = i.CostCenterInitial.Name }))
                 .ForMember(i => i.CostCenterFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenterFinal.Id, Code = i.CostCenterFinal.Code, Name = i.CostCenterFinal.Name }))
                 .ForMember(i => i.State, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.BudgetState.Id, Code = i.BudgetState.Code, Name = i.BudgetState.Name }))
                 .ForMember(i => i.DstConfAt, opt => opt.MapFrom(i => i.DstConfAt))
                 .ForMember(i => i.DstConfUser, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.DstConfUser.EmployeeId.GetValueOrDefault(), FirstName = i.DstConfUser.Employee.FirstName, LastName = i.DstConfUser.Employee.LastName }))
                 .ForMember(i => i.InfoFin, opt => opt.MapFrom(i => i.InfoFin))
                 .ForMember(i => i.Document, opt => opt.MapFrom(i => new Dto.Document { Id = i.Document.Id, ValidationDate = i.Document.ValidationDate }))
                 .ForMember(i => i.DocumentType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Document.DocumentType.Id, Code = i.Document.DocumentType.Code, Name = i.Document.DocumentType.Name }))
                 .ForMember(i => i.ModifiedAt, opt => opt.MapFrom(i => i.ModifiedAt));

            CreateMap<Model.BudgetBaseOp, Dto.BudgetBaseOp>()
                 .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                  .ForMember(i => i.Document, opt => opt.MapFrom(i => new Dto.Document { Id = i.Document.Id, ValidationDate = i.Document.ValidationDate }))
                  .ForMember(i => i.DocumentType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Document.DocumentType.Id, Code = i.Document.DocumentType.Code, Name = i.Document.DocumentType.Name }));


            CreateMap<Model.OfferOp, Dto.OfferOp>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.CompanyInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CompanyInitial.Id, Code = i.CompanyInitial.Code, Name = i.CompanyInitial.Name }))
                .ForMember(i => i.CompanyFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CompanyFinal.Id, Code = i.CompanyFinal.Code, Name = i.CompanyFinal.Name }))
                .ForMember(i => i.ProjectInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.ProjectInitial.Id, Code = i.ProjectInitial.Code, Name = i.ProjectInitial.Name }))
                .ForMember(i => i.ProjectFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.ProjectFinal.Id, Code = i.ProjectFinal.Code, Name = i.ProjectFinal.Name }))
                .ForMember(i => i.AdministrationInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AdministrationInitial.Id, Code = i.AdministrationInitial.Code, Name = i.AdministrationInitial.Name }))
                .ForMember(i => i.AdministrationFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AdministrationFinal.Id, Code = i.AdministrationFinal.Code, Name = i.AdministrationFinal.Name }))
                .ForMember(i => i.MasterTypeInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeInitial.Type.MasterType.Id, Code = i.SubTypeInitial.Type.MasterType.Code, Name = i.SubTypeInitial.Type.MasterType.Name }))
                .ForMember(i => i.MasterTypeFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeFinal.Type.MasterType.Id, Code = i.SubTypeFinal.Type.MasterType.Code, Name = i.SubTypeFinal.Type.MasterType.Name }))
                .ForMember(i => i.TypeInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeInitial.Type.Id, Code = i.SubTypeInitial.Type.Code, Name = i.SubTypeInitial.Type.Name }))
                .ForMember(i => i.TypeFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeFinal.Type.Id, Code = i.SubTypeFinal.Type.Code, Name = i.SubTypeFinal.Type.Name }))
                .ForMember(i => i.SubTypeInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeInitial.Id, Code = i.SubTypeInitial.Code, Name = i.SubTypeInitial.Name }))
                .ForMember(i => i.SubTypeFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeFinal.Id, Code = i.SubTypeFinal.Code, Name = i.SubTypeFinal.Name }))
                .ForMember(i => i.EmployeeInitial, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeInitial.Id, FirstName = i.EmployeeInitial.FirstName, LastName = i.EmployeeInitial.LastName }))
                .ForMember(i => i.EmployeeFinal, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeFinal.Id, FirstName = i.EmployeeFinal.FirstName, LastName = i.EmployeeFinal.LastName }))
                .ForMember(i => i.AccMonth, opt => opt.MapFrom(i => new Dto.MonthEntity { Id = i.AccMonth.Id, Year = i.AccMonth.Year }))
                .ForMember(i => i.PartnerInitial, opt => opt.MapFrom(i => new Dto.CodePartnerEntity { Id = i.PartnerInitial.Id, Name = i.PartnerInitial.Name }))
                .ForMember(i => i.PartnerFinal, opt => opt.MapFrom(i => new Dto.CodePartnerEntity { Id = i.PartnerFinal.Id, Name = i.PartnerFinal.Name }))
                .ForMember(i => i.AccountInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AccountInitial.Id, Code = i.AccountInitial.Code, Name = i.AccountInitial.Name }))
                .ForMember(i => i.AccountFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AccountFinal.Id, Code = i.AccountFinal.Code, Name = i.AccountFinal.Name }))
                .ForMember(i => i.CostCenterInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenterInitial.Id, Code = i.CostCenterInitial.Code, Name = i.CostCenterInitial.Name }))
                .ForMember(i => i.CostCenterFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenterFinal.Id, Code = i.CostCenterFinal.Code, Name = i.CostCenterFinal.Name }))
                .ForMember(i => i.State, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.BudgetState.Id, Code = i.BudgetState.Code, Name = i.BudgetState.Name }))
                .ForMember(i => i.DstConfAt, opt => opt.MapFrom(i => i.DstConfAt))
                .ForMember(i => i.DstConfUser, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.DstConfUser.EmployeeId.GetValueOrDefault(), FirstName = i.DstConfUser.Employee.FirstName, LastName = i.DstConfUser.Employee.LastName }))
                .ForMember(i => i.InfoFin, opt => opt.MapFrom(i => i.InfoFin))
                .ForMember(i => i.Document, opt => opt.MapFrom(i => new Dto.Document { Id = i.Document.Id, ValidationDate = i.Document.ValidationDate }))
                .ForMember(i => i.DocumentType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Document.DocumentType.Id, Code = i.Document.DocumentType.Code, Name = i.Document.DocumentType.Name }))
                .ForMember(i => i.ModifiedAt, opt => opt.MapFrom(i => i.ModifiedAt));

            CreateMap<Model.OrderOp, Dto.OrderOp>()
               .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.OfferFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.OfferFinal.Id, Code = i.OfferFinal.Code, Name = i.OfferFinal.Name }))
                .ForMember(i => i.BudgetFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.BudgetFinal.Id, Code = i.BudgetFinal.Code, Name = i.BudgetFinal.Name }))
               .ForMember(i => i.CompanyInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CompanyInitial.Id, Code = i.CompanyInitial.Code, Name = i.CompanyInitial.Name }))
               .ForMember(i => i.CompanyFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CompanyFinal.Id, Code = i.CompanyFinal.Code, Name = i.CompanyFinal.Name }))
               .ForMember(i => i.ProjectInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.ProjectInitial.Id, Code = i.ProjectInitial.Code, Name = i.ProjectInitial.Name }))
               .ForMember(i => i.ProjectFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.ProjectFinal.Id, Code = i.ProjectFinal.Code, Name = i.ProjectFinal.Name }))
               .ForMember(i => i.AdministrationInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AdministrationInitial.Id, Code = i.AdministrationInitial.Code, Name = i.AdministrationInitial.Name }))
               .ForMember(i => i.AdministrationFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AdministrationFinal.Id, Code = i.AdministrationFinal.Code, Name = i.AdministrationFinal.Name }))
               .ForMember(i => i.MasterTypeInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeInitial.Type.MasterType.Id, Code = i.SubTypeInitial.Type.MasterType.Code, Name = i.SubTypeInitial.Type.MasterType.Name }))
               .ForMember(i => i.MasterTypeFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeFinal.Type.MasterType.Id, Code = i.SubTypeFinal.Type.MasterType.Code, Name = i.SubTypeFinal.Type.MasterType.Name }))
               .ForMember(i => i.TypeInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeInitial.Type.Id, Code = i.SubTypeInitial.Type.Code, Name = i.SubTypeInitial.Type.Name }))
               .ForMember(i => i.TypeFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeFinal.Type.Id, Code = i.SubTypeFinal.Type.Code, Name = i.SubTypeFinal.Type.Name }))
               .ForMember(i => i.SubTypeInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeInitial.Id, Code = i.SubTypeInitial.Code, Name = i.SubTypeInitial.Name }))
               .ForMember(i => i.SubTypeFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubTypeFinal.Id, Code = i.SubTypeFinal.Code, Name = i.SubTypeFinal.Name }))
               .ForMember(i => i.EmployeeInitial, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeInitial.Id, FirstName = i.EmployeeInitial.FirstName, LastName = i.EmployeeInitial.LastName, Email = i.EmployeeInitial.Email }))
               .ForMember(i => i.EmployeeFinal, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeFinal.Id, FirstName = i.EmployeeFinal.FirstName, LastName = i.EmployeeFinal.LastName, Email = i.EmployeeFinal.Email }))
               .ForMember(i => i.AccMonth, opt => opt.MapFrom(i => new Dto.MonthEntity { Id = i.AccMonth.Id, Year = i.AccMonth.Year }))
               .ForMember(i => i.PartnerInitial, opt => opt.MapFrom(i => new Dto.CodePartnerEntity { Id = i.PartnerInitial.Id, Name = i.PartnerInitial.Name }))
               .ForMember(i => i.PartnerFinal, opt => opt.MapFrom(i => new Dto.CodePartnerEntity { Id = i.PartnerFinal.Id, Name = i.PartnerFinal.Name }))
               .ForMember(i => i.AccountInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AccountInitial.Id, Code = i.AccountInitial.Code, Name = i.AccountInitial.Name }))
               .ForMember(i => i.AccountFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AccountFinal.Id, Code = i.AccountFinal.Code, Name = i.AccountFinal.Name }))
               .ForMember(i => i.CostCenterInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenterInitial.Id, Code = i.CostCenterInitial.Code, Name = i.CostCenterInitial.Name }))
               .ForMember(i => i.CostCenterFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenterFinal.Id, Code = i.CostCenterFinal.Code, Name = i.CostCenterFinal.Name }))
               .ForMember(i => i.State, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.BudgetState.Id, Code = i.BudgetState.Code, Name = i.BudgetState.Name }))
               .ForMember(i => i.DstConfAt, opt => opt.MapFrom(i => i.DstConfAt))
               .ForMember(i => i.DstConfUser, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.DstConfUser.EmployeeId.GetValueOrDefault(), FirstName = i.DstConfUser.Employee.FirstName, LastName = i.DstConfUser.Employee.LastName, Email = i.DstConfUser.Employee.Email }))
               .ForMember(i => i.InfoFin, opt => opt.MapFrom(i => i.InfoFin))
               .ForMember(i => i.Document, opt => opt.MapFrom(i => new Dto.Document { Id = i.Document.Id, ValidationDate = i.Document.ValidationDate }))
               .ForMember(i => i.DocumentType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Document.DocumentType.Id, Code = i.Document.DocumentType.Code, Name = i.Document.DocumentType.Name }))
               .ForMember(i => i.ModifiedAt, opt => opt.MapFrom(i => i.ModifiedAt));


            CreateMap<Model.RequestOp, Dto.RequestOp>()
              .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
              .ForMember(i => i.BudgetInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.BudgetFinal.Id, Code = i.BudgetFinal.Code, Name = i.BudgetFinal.Name }))
              .ForMember(i => i.Company, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Company.Id, Code = i.Company.Code, Name = i.Company.Name }))
              .ForMember(i => i.ProjectInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.ProjectInitial.Id, Code = i.ProjectInitial.Code, Name = i.ProjectInitial.Name }))
              .ForMember(i => i.ProjectFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.ProjectFinal.Id, Code = i.ProjectFinal.Code, Name = i.ProjectFinal.Name }))
              .ForMember(i => i.EmployeeInitial, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeInitial.Id, FirstName = i.EmployeeInitial.FirstName, LastName = i.EmployeeInitial.LastName, Email = i.EmployeeInitial.Email }))
              .ForMember(i => i.EmployeeFinal, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeFinal.Id, FirstName = i.EmployeeFinal.FirstName, LastName = i.EmployeeFinal.LastName, Email = i.EmployeeFinal.Email }))
              .ForMember(i => i.AccMonth, opt => opt.MapFrom(i => new Dto.MonthEntity { Id = i.AccMonth.Id, Year = i.AccMonth.Year }))
              .ForMember(i => i.CostCenterInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenterInitial.Id, Code = i.CostCenterInitial.Code, Name = i.CostCenterInitial.Name }))
              .ForMember(i => i.CostCenterFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenterFinal.Id, Code = i.CostCenterFinal.Code, Name = i.CostCenterFinal.Name }))
              .ForMember(i => i.State, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RequestState.Id, Code = i.RequestState.Code, Name = i.RequestState.Name }))
              .ForMember(i => i.DstConfAt, opt => opt.MapFrom(i => i.DstConfAt))
              .ForMember(i => i.DstConfUser, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.DstConfUser.EmployeeId.GetValueOrDefault(), FirstName = i.DstConfUser.Employee.FirstName, LastName = i.DstConfUser.Employee.LastName, Email = i.DstConfUser.Employee.Email }))
              .ForMember(i => i.InfoFin, opt => opt.MapFrom(i => i.InfoFin))
              .ForMember(i => i.Document, opt => opt.MapFrom(i => new Dto.Document { Id = i.Document.Id, ValidationDate = i.Document.ValidationDate }))
              .ForMember(i => i.DocumentType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Document.DocumentType.Id, Code = i.Document.DocumentType.Code, Name = i.Document.DocumentType.Name }))
              .ForMember(i => i.ModifiedAt, opt => opt.MapFrom(i => i.ModifiedAt));


            CreateMap<Model.AssetComponentOp, Dto.AssetComponentOp>()
           .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
           .ForMember(i => i.AssetComponent, opt => opt.MapFrom(i => new Dto.AssetComponent { Id = i.AssetComponent.Id, Code = i.AssetComponent.Code, Name = i.AssetComponent.Name }))
           .ForMember(i => i.EmployeeInitial, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeInitial.Id, InternalCode = i.EmployeeInitial.InternalCode, FirstName = i.EmployeeInitial.FirstName, LastName = i.EmployeeInitial.LastName }))
           .ForMember(i => i.EmployeeFinal, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeFinal.Id, InternalCode = i.EmployeeFinal.InternalCode, FirstName = i.EmployeeFinal.FirstName, LastName = i.EmployeeFinal.LastName }))
           .ForMember(i => i.Document, opt => opt.MapFrom(i => new Dto.Document { Id = i.Document.Id, ValidationDate = i.Document.ValidationDate }))
           .ForMember(i => i.InvState, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.InvState.Id, Code = i.InvState.Code, Name = i.InvState.Name }))
           .ForMember(i => i.DocumentType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Document.DocumentType.Id, Code = i.Document.DocumentType.Code, Name = i.Document.DocumentType.Name }))
           .ForMember(i => i.ModifiedAt, opt => opt.MapFrom(i => i.ModifiedAt));









            CreateMap<Dto.AssetComponentOp, Model.AssetComponentOp>();

            CreateMap<Model.AssetOp, Dto.Sync.AssetOp>()
            //CreateMap<Tuple<Model.InventoryAsset, Model.AssetOp>, Dto.Sync.AssetOp>()
                 .ForMember(i => i.StateIdInitial, opt => opt.MapFrom(i => i.InvStateIdInitial))
                 .ForMember(i => i.StateIdFinal, opt => opt.MapFrom(i => i.InvStateIdFinal))
                 .ForMember(i => i.SNFinal, opt => opt.MapFrom(i => i.Asset.SerialNumber))
                 .ForMember(i => i.SNInitial, opt => opt.MapFrom(i => i.Asset.SerialNumber))
                 .ForMember(i => i.QuantityInitial, opt => opt.MapFrom(i => i.Asset.Quantity))
                 .ForMember(i => i.QuantityFinal, opt => opt.MapFrom(i => i.Asset.Quantity))
                 .ForMember(i => i.Info, opt => opt.MapFrom(i => i.Info))
                 .ForMember(i => i.Info, opt => opt.MapFrom(i => i.Info))
                 .ForMember(i => i.ValueInv, opt => opt.MapFrom(i => i.Asset.ValueInv));

            CreateMap<Model.Asset, Dto.Sync.Asset>()
            //CreateMap<Tuple<Model.InventoryAsset, Model.AssetOp>, Dto.Sync.AssetOp>()



            .ForMember(i => i.ValueInv, opt => opt.MapFrom(i => i.ValueInv))
            .ForMember(i => i.Quantity, opt => opt.MapFrom(i => i.Quantity));
           
            //.ForMember(i => i.ProducerInitial, opt => opt.MapFrom(i => i.Producer))
            //.ForMember(i => i.ProducerFinal, opt => opt.MapFrom(i => i.Producer));






            CreateMap<Model.AccMonth, Dto.AccMonth>();
            CreateMap<Dto.AccMonth, Model.AccMonth>();
            CreateMap<Model.AdmCenter, Dto.AdmCenter>()
             .ForMember(i => i.Employee, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Employee.Id, InternalCode = i.Employee.InternalCode, FirstName = i.Employee.FirstName, LastName = i.Employee.LastName, Email = i.Employee.Email }));
            CreateMap<Dto.AdmCenter, Model.AdmCenter>()
                    .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                    .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                    .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                    .ForMember(i => i.Employee, opt => opt.Ignore())
                    .ForMember(i => i.EmployeeId, opt => opt.MapFrom(i => i.EmployeeId));
            CreateMap<Model.Level, Dto.Level>();
            CreateMap<Dto.Level, Model.Level>();
            CreateMap<Model.AssetType, Dto.AssetType>();
            CreateMap<Dto.AssetType, Model.AssetType>();
            CreateMap<Model.AssetCategory, Dto.AssetCategory>();
            CreateMap<Dto.AssetCategory, Model.AssetCategory>();
            CreateMap<Model.Uom, Dto.Uom>();
            CreateMap<Dto.Uom, Model.Uom>();
            //CreateMap<Dto.Administration, Model.Administration>();
            CreateMap<Model.AssetClass, Dto.AssetClass>();
            CreateMap<Dto.AssetClass, Model.AssetClass>();
            CreateMap<Model.DocumentType, Dto.DocumentType>();
            CreateMap<Dto.DocumentType, Model.DocumentType>();
            CreateMap<Model.AppState, Dto.AppState>();
            CreateMap<Dto.AppState, Model.AppState>();
            CreateMap<Model.Plant, Dto.Plant>();
            CreateMap<Dto.Plant, Model.Plant>();
            CreateMap<Model.Partner, Dto.Partner>()
                  .ForMember(i => i.PartnerLocation, opt => opt.MapFrom(i => new Dto.PartnerLocation 
                 { 
                      Id = i.PartnerLocation.Id, 
                      FiscalCode = i.PartnerLocation.Cui,
                      Cui = i.PartnerLocation.Cui,
                      Data = i.PartnerLocation.Data,
                      Denumire = i.PartnerLocation.Denumire,
                      Adresa = i.PartnerLocation.Adresa,
                      NrRegCom = i.PartnerLocation.NrRegCom,
                      Telefon = i.PartnerLocation.Telefon,
                      Fax = i.PartnerLocation.Fax,
                      CodPostal = i.PartnerLocation.CodPostal,
                      Act = i.PartnerLocation.Act,
                      Stare_inregistrare = i.PartnerLocation.Stare_inregistrare,
                      ScpTVA = i.PartnerLocation.ScpTVA,
                      Data_inceput_ScpTVA = i.PartnerLocation.Data_inceput_ScpTVA,
                      Data_sfarsit_ScpTVA = i.PartnerLocation.Data_sfarsit_ScpTVA,
                      Data_anul_imp_ScpTVA = i.PartnerLocation.Data_anul_imp_ScpTVA,
                      Mesaj_ScpTVA = i.PartnerLocation.Mesaj_ScpTVA,
                      DataInceputTvaInc = i.PartnerLocation.DataInceputTvaInc,
                      DataSfarsitTvaInc = i.PartnerLocation.DataSfarsitTvaInc,
                      DataActualizareTvaInc = i.PartnerLocation.DataActualizareTvaInc,
                      DataPublicareTvaInc = i.PartnerLocation.DataPublicareTvaInc,
                      TipActTvaInc = i.PartnerLocation.TipActTvaInc,
                      StatusTvaIncasare = i.PartnerLocation.StatusTvaIncasare,
                      DataInactivare = i.PartnerLocation.DataInactivare,
                      DataReactivare = i.PartnerLocation.DataReactivare,
                      DataPublicare = i.PartnerLocation.DataPublicare,
                      DataRadiere = i.PartnerLocation.DataRadiere,
                      StatusInactivi = i.PartnerLocation.StatusInactivi,
                      DataInceputSplitTVA = i.PartnerLocation.DataInceputSplitTVA,
                      DataAnulareSplitTVA = i.PartnerLocation.DataAnulareSplitTVA,
                      StatusSplitTVA = i.PartnerLocation.StatusSplitTVA,
                      Iban = i.PartnerLocation.Iban,
                      Name = i.PartnerLocation.Denumire, 
                      RegistryNumber = i.PartnerLocation.CodPostal }));
            CreateMap<Dto.Partner, Model.Partner>()
                    .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                    .ForMember(i => i.FiscalCode, opt => opt.MapFrom(i => i.FiscalCode))
                    .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                    .ForMember(i => i.ERPId, opt => opt.MapFrom(i =>  1 == 1 ? 1 : 0))
                    .ForMember(i => i.RegistryNumber, opt => opt.MapFrom(i => i.RegistryNumber))
                    .ForMember(i => i.Bank, opt => opt.MapFrom(i => i.Bank))
                    .ForMember(i => i.BankAccount, opt => opt.MapFrom(i => i.BankAccount))
                    .ForMember(i => i.ContactInfo, opt => opt.MapFrom(i => i.ContactInfo))
                    .ForMember(i => i.Address, opt => opt.MapFrom(i => i.Address))
                    .ForMember(i => i.PartnerLocation, opt => opt.Ignore())
                    .ForMember(i => i.PartnerLocationId, opt => opt.MapFrom(i => i.PartnerLocation != null ? i.PartnerLocation.Id: 1));


		CreateMap<Model.Department, Dto.Department>();
            CreateMap<Dto.Department, Model.Department>();
            CreateMap<Model.CostCenter, Dto.CostCenter>()
                    .ForMember(i => i.AdmCenter, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AdmCenter.Id, Code = i.AdmCenter.Code, Name = i.AdmCenter.Name }))
                    .ForMember(i => i.Region, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Region.Id, Code = i.Region.Code, Name = i.Region.Name }))
                    .ForMember(i => i.Division, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Division.Id, Code = i.Division.Code, Name = i.Division.Name }))
                    .ForMember(i => i.Department, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Division.Department.Id, Code = i.Division.Department.Code, Name = i.Division.Department.Name }))
                    //.ForMember(i => i.Employee, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Employee.Id, FirstName = i.Employee.FirstName, LastName = i.Employee.LastName, Email = i.Employee.Email, InternalCode = i.Employee.InternalCode }))
                    //.ForMember(i => i.Employee2, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Employee2.Id, FirstName = i.Employee2.FirstName, LastName = i.Employee2.LastName, Email = i.Employee2.Email, InternalCode = i.Employee2.InternalCode }))
                    //.ForMember(i => i.Employee3, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Employee3.Id, FirstName = i.Employee3.FirstName, LastName = i.Employee3.LastName, Email = i.Employee3.Email, InternalCode = i.Employee3.InternalCode }))
                    .ForMember(i => i.Administration, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Administration.Id, Code = i.Administration.Code, Name = i.Administration.Name }))
                    .ForMember(i => i.Company, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Company.Id, Code = i.Company.Code, Name = i.Company.Name }))
                    .ForMember(i => i.Room, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Room.Id, Code = i.Room.Code, Name = i.Room.Name }));
            CreateMap<Dto.CostCenter, Model.CostCenter>()
                    .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                    .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                    .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
					.ForMember(i => i.IsFinished, opt => opt.MapFrom(i => i.IsFinished))
					.ForMember(i => i.AllowLabelList, opt => opt.MapFrom(i => i.AllowLabelList))
					.ForMember(i => i.InventoryList, opt => opt.MapFrom(i => i.InventoryList))
					.ForMember(i => i.BookBefore, opt => opt.MapFrom(i => i.BookBefore))
					.ForMember(i => i.BookAfter, opt => opt.MapFrom(i => i.BookAfter))
					.ForMember(i => i.PvBook, opt => opt.MapFrom(i => i.PvBook))
					.ForMember(i => i.DateFinished, opt => opt.MapFrom(i => i.DateFinished))
					.ForMember(i => i.AdmCenter, opt => opt.Ignore())
                    .ForMember(i => i.AdmCenterId, opt => opt.MapFrom(i => i.AdmCenter != null ? i.AdmCenter.Id : defaultId))
                    .ForMember(i => i.Region, opt => opt.Ignore())
                    .ForMember(i => i.RegionId, opt => opt.MapFrom(i => i.Region != null ? i.Region.Id : defaultId))
                    .ForMember(i => i.Division, opt => opt.Ignore())
                    .ForMember(i => i.DivisionId, opt => opt.MapFrom(i => i.Division != null ? i.Division.Id : defaultId))
                    //.ForMember(i => i.Employee, opt => opt.Ignore())
                    //.ForMember(i => i.EmployeeId, opt => opt.MapFrom(i => i.EmployeeId))
                    //.ForMember(i => i.Employee2, opt => opt.Ignore())
                    //.ForMember(i => i.EmployeeId2, opt => opt.MapFrom(i => i.EmployeeId2))
                    //.ForMember(i => i.Employee3, opt => opt.Ignore())
                    //.ForMember(i => i.EmployeeId3, opt => opt.MapFrom(i => i.EmployeeId3))
                    .ForMember(i => i.Administration, opt => opt.Ignore())
                    .ForMember(i => i.AdministrationId, opt => opt.MapFrom(i => i.Administration != null ? i.Administration.Id : defaultId))
					.ForMember(i => i.Room, opt => opt.Ignore())
                    .ForMember(i => i.RoomId, opt => opt.MapFrom(i => i.Room != null ? i.Room.Id : defaultId));
            CreateMap<Model.Matrix, Dto.Matrix>()
                   .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                   .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                   //.ForMember(i => i.AssetType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AssetType.Id, Code = i.AssetType.Code, Name = i.AssetType.Name }))
                   //.ForMember(i => i.Area, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Area.Id, Code = i.Area.Code, Name = i.Area.Name }))
                   //.ForMember(i => i.Country, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Country.Id, Code = i.Country.Code, Name = i.Country.Name }))
                   .ForMember(i => i.Company, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Company.Id, Code = i.Company.Code, Name = i.Company.Name }))
                   //.ForMember(i => i.CostCenter, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenter.Id, Code = i.CostCenter.Code, Name = i.CostCenter.Name }))
                   //.ForMember(i => i.AdmCenter, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenter.AdmCenter.Id, Code = i.CostCenter.AdmCenter.Code, Name = i.CostCenter.AdmCenter.Name }))
                   .ForMember(i => i.Division, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Division.Id, Code = i.Division.Code, Name = i.Division.Name }))
                   .ForMember(i => i.Department, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Division.Department.Id, Code = i.Division.Department.Code, Name = i.Division.Department.Name }))
				   //.ForMember(i => i.Project, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Project.Id, Code = i.Project.Code, Name = i.Project.Name }))
				   //.ForMember(i => i.ProjectType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Project.ProjectType.Id, Code = i.Project.ProjectType.Code, Name = i.Project.ProjectType.Name }))
				   .ForMember(i => i.EmployeeB1, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeB1.Id, FirstName = i.EmployeeB1.FirstName, LastName = i.EmployeeB1.LastName, Email = i.EmployeeB1.Email }))
				   .ForMember(i => i.EmployeeL1, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeL1.Id, FirstName = i.EmployeeL1.FirstName, LastName = i.EmployeeL1.LastName, Email = i.EmployeeL1.Email }))
                   .ForMember(i => i.EmployeeL2, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeL2.Id, FirstName = i.EmployeeL2.FirstName, LastName = i.EmployeeL2.LastName, Email = i.EmployeeL2.Email }))
                   .ForMember(i => i.EmployeeL3, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeL3.Id, FirstName = i.EmployeeL3.FirstName, LastName = i.EmployeeL3.LastName, Email = i.EmployeeL3.Email }))
                   .ForMember(i => i.EmployeeL4, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeL4.Id, FirstName = i.EmployeeL4.FirstName, LastName = i.EmployeeL4.LastName, Email = i.EmployeeL4.Email }))
                   .ForMember(i => i.EmployeeS1, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeS1.Id, FirstName = i.EmployeeS1.FirstName, LastName = i.EmployeeS1.LastName, Email = i.EmployeeS1.Email }))
                   .ForMember(i => i.EmployeeS2, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeS2.Id, FirstName = i.EmployeeS2.FirstName, LastName = i.EmployeeS2.LastName, Email = i.EmployeeS2.Email }))
                   .ForMember(i => i.EmployeeS3, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeS3.Id, FirstName = i.EmployeeS3.FirstName, LastName = i.EmployeeS3.LastName, Email = i.EmployeeS3.Email }));
            CreateMap<Dto.Matrix, Model.Matrix>()
                    .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                    .ForMember(i => i.Code, opt => opt.MapFrom(i => ""))
                    .ForMember(i => i.Name, opt => opt.MapFrom(i => ""))
					.ForMember(i => i.AmountL1, opt => opt.MapFrom(i => i.AmountL1))
					.ForMember(i => i.AmountL2, opt => opt.MapFrom(i => i.AmountL2))
					.ForMember(i => i.AmountL3, opt => opt.MapFrom(i => i.AmountL3))
					.ForMember(i => i.AmountL4, opt => opt.MapFrom(i => i.AmountL4))
					.ForMember(i => i.AmountS1, opt => opt.MapFrom(i => i.AmountS1))
					.ForMember(i => i.AmountS2, opt => opt.MapFrom(i => i.AmountS2))
					.ForMember(i => i.AmountS3, opt => opt.MapFrom(i => i.AmountS3))
                    //.ForMember(i => i.AssetType, opt => opt.Ignore())
                    //.ForMember(i => i.AssetTypeId, opt => opt.MapFrom(i => i.AssetType.Id))
                    //.ForMember(i => i.Area, opt => opt.Ignore())
                    //.ForMember(i => i.AreaId, opt => opt.MapFrom(i => i.Area.Id))
                    //.ForMember(i => i.Country, opt => opt.Ignore())
                    //.ForMember(i => i.CountryId, opt => opt.MapFrom(i => i.Country.Id))
                    .ForMember(i => i.Company, opt => opt.Ignore())
                    .ForMember(i => i.CompanyId, opt => opt.MapFrom(i => i.Company.Id))
					.ForMember(i => i.Division, opt => opt.Ignore())
					.ForMember(i => i.DivisionId, opt => opt.MapFrom(i => i.Division.Id))
					.ForMember(i => i.EmployeeB1, opt => opt.Ignore())
					.ForMember(i => i.EmployeeB1Id, opt => opt.MapFrom(i => i.EmployeeB1.Id))
					.ForMember(i => i.EmployeeL1, opt => opt.Ignore())
					.ForMember(i => i.EmployeeL1Id, opt => opt.MapFrom(i => i.EmployeeL1.Id))
					.ForMember(i => i.EmployeeL2, opt => opt.Ignore())
					.ForMember(i => i.EmployeeL2Id, opt => opt.MapFrom(i => i.EmployeeL2.Id))
					.ForMember(i => i.EmployeeL3, opt => opt.Ignore())
					.ForMember(i => i.EmployeeL3Id, opt => opt.MapFrom(i => i.EmployeeL3.Id))
					.ForMember(i => i.EmployeeL4, opt => opt.Ignore())
					.ForMember(i => i.EmployeeL4Id, opt => opt.MapFrom(i => i.EmployeeL4.Id))
					.ForMember(i => i.EmployeeS1, opt => opt.Ignore())
					.ForMember(i => i.EmployeeS1Id, opt => opt.MapFrom(i => i.EmployeeS1.Id))
					.ForMember(i => i.EmployeeS2, opt => opt.Ignore())
					.ForMember(i => i.EmployeeS2Id, opt => opt.MapFrom(i => i.EmployeeS2.Id))
					.ForMember(i => i.EmployeeS3, opt => opt.Ignore())
					.ForMember(i => i.EmployeeS3Id, opt => opt.MapFrom(i => i.EmployeeS3.Id))
					//.ForMember(i => i.CostCenter, opt => opt.Ignore())
					//.ForMember(i => i.CostCenterId, opt => opt.MapFrom(i => i.CostCenter.Id))
					//.ForMember(i => i.Project, opt => opt.Ignore())
					//.ForMember(i => i.ProjectId, opt => opt.MapFrom(i => i.Project.Id))
					.ForMember(i => i.AppState, opt => opt.Ignore())
                    .ForMember(i => i.AppStateId, opt => opt.MapFrom(i => 12));
            CreateMap<Model.Employee, Dto.Employee>()
                     .ForMember(i => i.CostCenter, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenter.Id, Code = i.CostCenter.Code, Name = i.CostCenter.Name }))
                     .ForMember(i => i.Company, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Company.Id, Code = i.Company.Code, Name = i.Company.Name }))
                     .ForMember(i => i.Manager, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Manager.Id, FirstName = i.Manager.FirstName, LastName = i.Manager.LastName, Email = i.Manager.Email }))
                     .ForMember(i => i.Department, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Department.Id, Code = i.Department.Code, Name = i.Department.Name }));
            CreateMap<Dto.Employee, Model.Employee>()
                    .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                    .ForMember(i => i.FirstName, opt => opt.MapFrom(i => i.FirstName))
                    .ForMember(i => i.LastName, opt => opt.MapFrom(i => i.LastName))
                    .ForMember(i => i.Email, opt => opt.MapFrom(i => i.Email))
                    .ForMember(i => i.ERPCode, opt => opt.MapFrom(i => i.ErpCode))
                    .ForMember(i => i.Department, opt => opt.Ignore())
                    .ForMember(i => i.DepartmentId, opt => opt.MapFrom(i => i.DepartmentId));

            CreateMap<Model.EmployeeCostCenter, Dto.EmployeeCostCenter>()
             .ForMember(i => i.CostCenter, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenter.Id, Code = i.CostCenter.Code, Name = i.CostCenter.Name }))
             .ForMember(i => i.Employee, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Employee.Id, InternalCode = i.Employee.InternalCode, FirstName = i.Employee.FirstName, LastName = i.Employee.LastName, Email = i.Employee.Email }));
            CreateMap<Dto.EmployeeCostCenter, Model.EmployeeCostCenter>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.CostCenter, opt => opt.Ignore())
                .ForMember(i => i.CostCenterId, opt => opt.MapFrom(i => i.CostCenter.Id))
                .ForMember(i => i.Employee, opt => opt.Ignore())
                .ForMember(i => i.EmployeeId, opt => opt.MapFrom(i => i.Employee.Id));

			CreateMap<Model.EmployeeCompany, Dto.EmployeeCompany>()
		   .ForMember(i => i.Company, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Company.Id, Code = i.Company.Code, Name = i.Company.Name }))
		   .ForMember(i => i.Employee, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Employee.Id, InternalCode = i.Employee.InternalCode, FirstName = i.Employee.FirstName, LastName = i.Employee.LastName, Email = i.Employee.Email }));
			CreateMap<Dto.EmployeeCompany, Model.EmployeeCompany>()
				.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				.ForMember(i => i.Company, opt => opt.Ignore())
				.ForMember(i => i.CompanyId, opt => opt.MapFrom(i => i.Company.Id))
				.ForMember(i => i.Employee, opt => opt.Ignore())
				.ForMember(i => i.EmployeeId, opt => opt.MapFrom(i => i.Employee.Id));

			CreateMap<Model.EmployeeDivision, Dto.EmployeeDivision>()
             .ForMember(i => i.Division, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Division.Id, Code = i.Division.Code, Name = i.Division.Name }))
             .ForMember(i => i.Employee, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Employee.Id, InternalCode = i.Employee.InternalCode, FirstName = i.Employee.FirstName, LastName = i.Employee.LastName, Email = i.Employee.Email }));
            CreateMap<Dto.EmployeeDivision, Model.EmployeeDivision>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Division, opt => opt.Ignore())
                .ForMember(i => i.DivisionId, opt => opt.MapFrom(i => i.Division.Id))
                .ForMember(i => i.Employee, opt => opt.Ignore())
                .ForMember(i => i.EmployeeId, opt => opt.MapFrom(i => i.Employee.Id));

            CreateMap<Model.RequestBudgetForecast, Dto.RequestBudgetForecast>()
            .ForMember(i => i.NeedBudget, opt => opt.MapFrom(i => i.NeedBudget))
            .ForMember(i => i.NeedContract, opt => opt.MapFrom(i => i.NeedContract))
            .ForMember(i => i.Quantity, opt => opt.MapFrom(i => i.Quantity))
            .ForMember(i => i.MaxQuantity, opt => opt.MapFrom(i => i.MaxQuantity))
            .ForMember(i => i.TotalOrderQuantity, opt => opt.MapFrom(i => i.TotalOrderQuantity))
            //.ForMember(i => i.RequestBudgetForecastMaterials, opt => opt.MapFrom(i => i.RequestBudgetForecastMaterial.Where(p => p.IsDeleted == false).Select(p => new Dto.RequestBudgetForecastMaterial()
            //{
            // Material = new Dto.Material() { Id = p.Material != null ? p.Material.Id : 0, Code = p.Material != null ? p.Material.Code : "", Name = p.Material != null ? p.Material.Name : "" }
            //})))
            .ForMember(i => i.Value, opt => opt.MapFrom(i => i.Value))
            .ForMember(i => i.MaxValue, opt => opt.MapFrom(i => i.MaxValue))
            .ForMember(i => i.Materials, opt => opt.MapFrom(i => i.Materials))
            .ForMember(i => i.ValueRon, opt => opt.MapFrom(i => i.ValueRon))
            .ForMember(i => i.MaxValueRon, opt => opt.MapFrom(i => i.MaxValueRon))
			.ForMember(i => i.ValueUsed, opt => opt.MapFrom(i => i.ValueUsed))
			.ForMember(i => i.ValueUsedRon, opt => opt.MapFrom(i => i.ValueUsedRon))
			.ForMember(i => i.TotalOrderValue, opt => opt.MapFrom(i => i.TotalOrderValue))
            .ForMember(i => i.TotalOrderValueRon, opt => opt.MapFrom(i => i.TotalOrderValueRon))
            .ForMember(i => i.Price, opt => opt.MapFrom(i => i.Price))
            .ForMember(i => i.Info, opt => opt.MapFrom(i => i.BudgetForecast.BudgetBase.Info))
            .ForMember(i => i.PriceRon, opt => opt.MapFrom(i => i.PriceRon))
            .ForMember(i => i.NeedBudgetValue, opt => opt.MapFrom(i => i.NeedBudgetValue))
            .ForMember(i => i.NeedContractValue, opt => opt.MapFrom(i => i.NeedContractValue))
            .ForMember(i => i.Contract, opt => opt.MapFrom(i => new Dto.ContractAmountEntity { Id = i.Contract.Id, ContractId = i.Contract.ContractId, Title = i.Contract.Title, AmountRem = i.Contract.ContractAmount.AmountRem, AmountRonRem = i.Contract.ContractAmount.AmountRonRem }))
            .ForMember(i => i.Project, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.BudgetForecast.BudgetBase.Project.Id, Code = i.BudgetForecast.BudgetBase.Project.Code, Name = i.BudgetForecast.BudgetBase.Project.Name }))
            .ForMember(i => i.OfferType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.OfferType.Id, Code = i.OfferType.Code, Name = i.OfferType.Name }))
            .ForMember(i => i.Request, opt => opt.MapFrom(i => new Dto.Request { Id = i.Request.Id, Code = i.Request.Code, Name = i.Request.Name, Info = i.Request.Info }))
            .ForMember(i => i.AppState, opt => opt.MapFrom(i => new Dto.AppState { Id = i.AppState.Id, Code = i.AppState.Code, Name = i.AppState.Name, BadgeIcon = i.AppState.BadgeIcon, BadgeColor = i.AppState.BadgeColor }))
            .ForMember(i => i.BudgetForecast, opt => opt.MapFrom(i => new Dto.BudgetForecast { 
                Id = i.BudgetForecast.Id, 
                Code = i.BudgetForecast.BudgetBase.Code,
                Total = i.BudgetForecast.Total,
                TotalRem = i.BudgetForecast.TotalRem,
                //BudgetBase = new Dto.BudgetBase
                //{
                //    Id = i.Request.BudgetForecast.BudgetBase.Id,
                //    Code = i.Request.BudgetForecast.BudgetBase.Code,
                //    Info = i.Request.BudgetForecast.BudgetBase.Info,
                //    AdmCenter = new Dto.CodeNameEntity() { Id = i.Request.BudgetForecast.BudgetBase.AdmCenter.Id, Code = i.Request.BudgetForecast.BudgetBase.AdmCenter.Code, Name = i.Request.BudgetForecast.BudgetBase.AdmCenter.Name }
                //}

            }));
            CreateMap<Dto.RequestBudgetForecast, Model.RequestBudgetForecast>();

            CreateMap<Model.RequestBudgetForecast, Dto.RequestBudgetForecastData>()
            .ForMember(i => i.Request, opt => opt.MapFrom(i => new Dto.Request { Id = i.Request.Id, Code = i.Request.Code, Name = i.Request.Name }))
            .ForMember(i => i.Children, opt => opt.MapFrom(i => i.RequestBudgetForecastMaterials.Where(p => p.IsDeleted == false).Select(p => new Dto.RequestBudgetForecastChildrenBase()
            {
                Material = new Dto.Material() { Id = p.Material != null ? p.Material.Id : 0, Code = p.Material != null ? p.Material.Code : "", Name = p.Material != null ? p.Material.Name : "" }
            })));


            CreateMap<Model.RequestBudgetForecastMaterial, Dto.RequestBudgetForecastMaterial>()
            .ForMember(i => i.ReceptionsQuantity, opt => opt.MapFrom(i => i.ReceptionsQuantity))
            .ForMember(i => i.ReceptionsValue, opt => opt.MapFrom(i => i.ReceptionsValue))
            .ForMember(i => i.ReceptionsValueRon, opt => opt.MapFrom(i => i.ReceptionsValueRon))
            .ForMember(i => i.WIP, opt => opt.MapFrom(i => i.WIP))
            .ForMember(i => i.BudgetForecastTimeStamp, opt => opt.MapFrom(i => i.BudgetForecastTimeStamp))
            .ForMember(i => i.Value, opt => opt.MapFrom(i => i.Value))
            .ForMember(i => i.Value, opt => opt.MapFrom(i => i.Value))
            .ForMember(i => i.Value, opt => opt.MapFrom(i => i.Value))
            .ForMember(i => i.ValueRon, opt => opt.MapFrom(i => i.ValueRon))
            .ForMember(i => i.ValueRem, opt => opt.MapFrom(i => i.ValueRem))
			.ForMember(i => i.ValueUsed, opt => opt.MapFrom(i => i.ValueUsed))
			.ForMember(i => i.ValueUsedRon, opt => opt.MapFrom(i => i.ValueUsedRon))
			.ForMember(i => i.ValueRemRon, opt => opt.MapFrom(i => i.ValueRemRon))
            .ForMember(i => i.Price, opt => opt.MapFrom(i => i.Price))
            .ForMember(i => i.PriceRon, opt => opt.MapFrom(i => i.PriceRon))
            .ForMember(i => i.Quantity, opt => opt.MapFrom(i => i.Quantity))
            .ForMember(i => i.QuantityRem, opt => opt.MapFrom(i => i.QuantityRem))
            .ForMember(i => i.MaxQuantity, opt => opt.MapFrom(i => i.MaxQuantity))
            .ForMember(i => i.TotalCostCenterQuantity, opt => opt.MapFrom(i => i.TotalCostCenterQuantity))
            .ForMember(i => i.MaxValue, opt => opt.MapFrom(i => i.MaxValue))
            .ForMember(i => i.MaxValueRon, opt => opt.MapFrom(i => i.MaxValueRon))
            .ForMember(i => i.TotalCostCenterValue, opt => opt.MapFrom(i => i.TotalCostCenterValue))
            .ForMember(i => i.TotalCostCenterValueRon, opt => opt.MapFrom(i => i.TotalCostCenterValueRon))
            .ForMember(i => i.OfferType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.OfferType.Id, Code = i.OfferType.Code, Name = i.OfferType.Name }))
            .ForMember(i => i.Material, opt => opt.MapFrom(i => new Dto.Material { Id = i.Material.Id, Code = i.Material.Code, Name = i.Material.Name }))
            .ForMember(i => i.AppState, opt => opt.MapFrom(i => new Dto.AppState { Id = i.AppState.Id, Code = i.AppState.Code, Name = i.AppState.Name, BadgeColor = i.AppState.BadgeColor, BadgeIcon = i.AppState.BadgeIcon }))
            .ForMember(i => i.Order, opt => opt.MapFrom(i => new Dto.Order { Id = i.Order.Id, Code = i.Order.Code, Name = i.Order.Name }))
           .ForMember(i => i.RequestBGTF, opt => opt.MapFrom(i => new Dto.RequestBudgetForecast
           {
               Id = i.RequestBudgetForecast.Id,
               BudgetForecast = new Dto.BudgetForecast 
               {
                Id = i.RequestBudgetForecast.BudgetForecast.Id,
                TotalRem = i.RequestBudgetForecast.BudgetForecast.TotalRem,
                Project = new Dto.CodeNameEntity { Id = i.RequestBudgetForecast.BudgetForecast.BudgetBase.Project.Id, Code = i.RequestBudgetForecast.BudgetForecast.BudgetBase.Project.Code }
               }
               

           }));
            CreateMap<Dto.RequestBudgetForecastMaterial, Model.RequestBudgetForecastMaterial>();


            CreateMap<Model.RequestBFMaterialCostCenter, Dto.RequestBFMaterialCostCenter>()
            .ForMember(i => i.ReceptionsQuantity, opt => opt.MapFrom(i => i.ReceptionsQuantity))
            .ForMember(i => i.ReceptionsValue, opt => opt.MapFrom(i => i.ReceptionsValue))
            .ForMember(i => i.ReceptionsValueRon, opt => opt.MapFrom(i => i.ReceptionsValueRon))
            .ForMember(i => i.WIP, opt => opt.MapFrom(i => i.WIP))
            .ForMember(i => i.Value, opt => opt.MapFrom(i => i.Value))
            .ForMember(i => i.Value, opt => opt.MapFrom(i => i.Value))
            .ForMember(i => i.Value, opt => opt.MapFrom(i => i.Value))
            .ForMember(i => i.ValueRon, opt => opt.MapFrom(i => i.ValueRon))
            .ForMember(i => i.ValueRem, opt => opt.MapFrom(i => i.ValueRem))
            .ForMember(i => i.ValueRemRon, opt => opt.MapFrom(i => i.ValueRemRon))
            .ForMember(i => i.Price, opt => opt.MapFrom(i => i.Price))
            .ForMember(i => i.PriceRon, opt => opt.MapFrom(i => i.PriceRon))
            .ForMember(i => i.Quantity, opt => opt.MapFrom(i => i.Quantity))
            .ForMember(i => i.QuantityRem, opt => opt.MapFrom(i => i.QuantityRem))
            .ForMember(i => i.Multiple, opt => opt.MapFrom(i => i.Multiple))
            .ForMember(i => i.MaxQuantity, opt => opt.MapFrom(i => i.MaxQuantity))
            .ForMember(i => i.MaxValue, opt => opt.MapFrom(i => i.MaxValue))
            .ForMember(i => i.MaxValueRon, opt => opt.MapFrom(i => i.MaxValueRon))
			.ForMember(i => i.Storno, opt => opt.MapFrom(i => i.Storno))
			.ForMember(i => i.StornoQuantity, opt => opt.MapFrom(i => i.StornoQuantity))
			.ForMember(i => i.StornoValue, opt => opt.MapFrom(i => i.StornoValue))
			//.ForMember(i => i.StornoValueRon, opt => opt.MapFrom(i => i.StornoValueRon))
			//.ForMember(i => i.Cassation, opt => opt.MapFrom(i => i.Cassation))
			//.ForMember(i => i.CassationQuantity, opt => opt.MapFrom(i => i.CassationQuantity))
			//.ForMember(i => i.CassationValue, opt => opt.MapFrom(i => i.CassationValue))
			//.ForMember(i => i.CassationValueRon, opt => opt.MapFrom(i => i.CassationValueRon))

			.ForMember(i => i.CostCenter, opt => opt.MapFrom(i => new Dto.CostCenter { Id = i.CostCenter.Id, Code = i.CostCenter.Code, Name = i.CostCenter.Name }))
            .ForMember(i => i.AppState, opt => opt.MapFrom(i => new Dto.AppState { Id = i.AppState.Id, Code = i.AppState.Code, Name = i.AppState.Name, BadgeIcon = i.AppState.BadgeIcon, BadgeColor = i.AppState.BadgeColor }))
            .ForMember(i => i.OfferType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.OfferType.Id, Code = i.OfferType.Code, Name = i.OfferType.Name }))
            .ForMember(i => i.Order, opt => opt.MapFrom(i => new Dto.Order { Id = i.Order.Id, Code = i.Order.Code, Name = i.Order.Name }))
			.ForMember(i => i.Employee, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Employee.Id, Email = i.Employee.Email }))
			.ForMember(i => i.RequestBGTFM, opt => opt.MapFrom(i => new Dto.RequestBudgetForecastMaterial
            {
                Id = i.RequestBudgetForecastMaterial.Id,
                //RequestBGTF = new Dto.RequestBudgetForecast
                //{
                //    Id = i.RequestBudgetForecastMaterial.RequestBudgetForecast.BudgetForecast.Id,
                //    //TotalRem = i.RequestBudgetForecastMaterial.RequestBudgetForecast.BudgetForecast.TotalRem,
                //    //Project = new Dto.CodeNameEntity { Id = i.RequestBudgetForecastMaterial.RequestBudgetForecast.BudgetForecast.BudgetBase.Project.Id, Code = i.RequestBudgetForecastMaterial.RequestBudgetForecast.BudgetForecast.BudgetBase.Project.Code }
                //}


            }));
            CreateMap<Dto.RequestBFMaterialCostCenter, Model.RequestBFMaterialCostCenter>();

            CreateMap<Model.ProjectTypeDivision, Dto.ProjectTypeDivision>()
            .ForMember(i => i.ProjectType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.ProjectType.Id, Code = i.ProjectType.Code, Name = i.ProjectType.Name }))
            .ForMember(i => i.Division, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Division.Id, Code = i.Division.Code, Name = i.Division.Name}))
            .ForMember(i => i.Department, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Division.Department.Id, Code = i.Division.Department.Code, Name = i.Division.Department.Name }));
            CreateMap<Dto.ProjectTypeDivision, Model.ProjectTypeDivision>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.ProjectType, opt => opt.Ignore())
                .ForMember(i => i.ProjectTypeId, opt => opt.MapFrom(i => i.ProjectType.Id))
                .ForMember(i => i.Division, opt => opt.Ignore())
                .ForMember(i => i.DivisionId, opt => opt.MapFrom(i => i.Division.Id));

            CreateMap<Model.OfferMaterial, Dto.OfferMaterial>()
		   .ForMember(i => i.RateValue, opt => opt.MapFrom(i => i.Rate != null ? i.Rate.Value : 0))
		   .ForMember(i => i.RateDate, opt => opt.MapFrom(i => i.Rate != null ? i.Rate.Code : ""))
		  .ForMember(i => i.Uom, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Rate.Uom.Id, Code = i.Rate.Uom.Code, Name = i.Rate.Uom.Name }))
		   .ForMember(i => i.Offer, opt => opt.MapFrom(i => new Dto.OfferUI { Id = i.Offer.Id, Code = i.Offer.Code, Name = i.Offer.Name, AppState = new Dto.CodeNameEntity() { Id = i.Offer.AppState.Id, Code = i.Offer.AppState.Code, Name = i.Offer.AppState.Name } }))
           .ForMember(i => i.Material, opt => opt.MapFrom(i => new Dto.Material { Id = i.Material.Id, Code = i.Material.Code, Name = i.Material.Name, EAN = i.Material.EAN, PartNumber= i.Material.PartNumber, Value= i.Material.Value, Price = i.Material.Price , Quantity = i.Material.Quantity}))
           .ForMember(i => i.AppState, opt => opt.MapFrom(i => new Dto.AppState { Id = i.AppState.Id, Code = i.AppState.Code, Name = i.AppState.Name, BadgeColor = i.AppState.BadgeColor, BadgeIcon = i.AppState.BadgeIcon }))
           .ForMember(i => i.Rate, opt => opt.MapFrom(i => new Dto.Rate { Id = i.Rate.Id, Code = i.Rate.Code, Value = i.Rate.Value, Uom = new Dto.CodeNameEntity() { Id = i.Rate.Uom.Id, Code = i.Rate.Uom.Code, Name = i.Rate.Uom.Name} }));
            CreateMap<Dto.OfferMaterial, Model.OfferMaterial>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Offer, opt => opt.Ignore())
                .ForMember(i => i.OfferId, opt => opt.MapFrom(i => i.Offer.Id))
                .ForMember(i => i.Material, opt => opt.Ignore())
                .ForMember(i => i.MaterialId, opt => opt.MapFrom(i => i.Material.Id))
                .ForMember(i => i.AppState, opt => opt.Ignore())
                .ForMember(i => i.AppStateId, opt => opt.MapFrom(i => i.AppState.Id));

            CreateMap<Model.OrderMaterial, Dto.OrderMaterial>()
            .ForMember(i => i.Order, opt => opt.MapFrom(i => new Dto.OrderUI { Id = i.Order.Id, Code = i.Order.Code, Name = i.Order.Name }))
            .ForMember(i => i.Material, opt => opt.MapFrom(i => new Dto.Material { Id = i.Material.Id, Code = i.Material.Code, Name = i.Material.Name, EAN = i.Material.EAN, PartNumber = i.Material.PartNumber, Value = i.Material.Value, Price = i.Material.Price, Quantity = i.Material.Quantity }))
            .ForMember(i => i.AppState, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AppState.Id, Code = i.AppState.Code, Name = i.AppState.Name }))
            .ForMember(i => i.Rate, opt => opt.MapFrom(i => new Dto.Rate { Id = i.Rate.Id, Code = i.Rate.Code, Value = i.Rate.Value, Uom = new Dto.CodeNameEntity() { Id = i.Rate.Uom.Id, Code = i.Rate.Uom.Code, Name = i.Rate.Uom.Name } }));
            CreateMap<Dto.OrderMaterial, Model.OrderMaterial>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Order, opt => opt.Ignore())
                .ForMember(i => i.OrderId, opt => opt.MapFrom(i => i.Order.Id))
                .ForMember(i => i.Material, opt => opt.Ignore())
                .ForMember(i => i.MaterialId, opt => opt.MapFrom(i => i.Material.Id))
                .ForMember(i => i.AppState, opt => opt.Ignore())
                .ForMember(i => i.AppStateId, opt => opt.MapFrom(i => i.AppState.Id));

            CreateMap<Model.MatrixLevel, Dto.MatrixLevel>()
                .ForMember(i => i.Amount, opt => opt.MapFrom(i => i.Amount))
             .ForMember(i => i.Matrix, opt => opt.MapFrom(i => new Dto.MatrixUI { 
                 Id = i.Matrix.Id, 
                 Code = i.Matrix.Code,
                 Name = i.Matrix.Name,
                 Company = new Dto.CodeNameEntity() { Id = i.Matrix.Company.Id, Code = i.Matrix.Company.Code, Name = i.Matrix.Company.Name },
                 CostCenter = new Dto.CodeNameEntity() { Id = i.Matrix.CostCenter.Id, Code = i.Matrix.CostCenter.Code, Name = i.Matrix.CostCenter.Name },
                 Project = new Dto.CodeNameEntity() { Id = i.Matrix.Project.Id, Code = i.Matrix.Project.Code, Name = i.Matrix.Project.Name },
                 AdmCenter = new Dto.CodeNameEntity() { Id = i.Matrix.CostCenter.AdmCenter.Id, Code = i.Matrix.CostCenter.AdmCenter.Code, Name = i.Matrix.CostCenter.AdmCenter.Name }

             }))
             .ForMember(i => i.Level, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Level.Id, Code = i.Level.Code, Name = i.Level.Name }))
             .ForMember(i => i.Employee, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Employee.Id, InternalCode = i.Employee.InternalCode, FirstName = i.Employee.FirstName, LastName = i.Employee.LastName, Email = i.Employee.Email }))
             .ForMember(i => i.Uom, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Uom.Id, Code = i.Uom.Code, Name = i.Uom.Name }));
            CreateMap<Dto.MatrixLevel, Model.MatrixLevel>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Amount, opt => opt.MapFrom(i => i.Amount))
                .ForMember(i => i.Matrix, opt => opt.Ignore())
                .ForMember(i => i.MatrixId, opt => opt.MapFrom(i => i.Matrix.Id))
                .ForMember(i => i.Level, opt => opt.Ignore())
                .ForMember(i => i.LevelId, opt => opt.MapFrom(i => i.Level.Id))
                .ForMember(i => i.Uom, opt => opt.Ignore())
                .ForMember(i => i.UomId, opt => opt.MapFrom(i => i.Uom.Id))
                .ForMember(i => i.Employee, opt => opt.Ignore())
                .ForMember(i => i.EmployeeId, opt => opt.MapFrom(i => i.Employee.Id));

            CreateMap<Model.EntityFile, Dto.EntityFile>()
                      .ForMember(i => i.Selected, opt => opt.MapFrom(i => 1 == 1 ? true : false))
                     .ForMember(i => i.EntityType, opt => opt.MapFrom(i => new Dto.EntityType { Id = i.EntityType.Id, Code = i.EntityType.Code, Name = i.EntityType.Name, UploadFolder = i.EntityType.UploadFolder }))
                     .ForMember(i => i.Request, opt => opt.MapFrom(i => new Dto.Request { Id = i.Request.Id, Code = i.Request.Code, Name = i.Request.Name, Info = i.Request.Info }))
                     .ForMember(i => i.RequestBudgetForecast, opt => opt.MapFrom(i => new Dto.RequestBudgetForecast { Id = i.RequestBudgetForecast.Id, 
                        Info = i.RequestBudgetForecast != null && i.RequestBudgetForecast.BudgetForecast != null && i.RequestBudgetForecast.BudgetForecast.BudgetBase != null && i.RequestBudgetForecast.BudgetForecast.BudgetBase != null ? i.RequestBudgetForecast.BudgetForecast.BudgetBase.Info : "",
                         Project = new Dto.CodeNameEntity { 
                             Id = i.RequestBudgetForecast != null && i.RequestBudgetForecast.BudgetForecast != null && i.RequestBudgetForecast.BudgetForecast.BudgetBase != null && i.RequestBudgetForecast.BudgetForecast.BudgetBase.Project != null ? i.RequestBudgetForecast.BudgetForecast.BudgetBase.Project.Id : 0, 
                             Code = i.RequestBudgetForecast != null && i.RequestBudgetForecast.BudgetForecast != null && i.RequestBudgetForecast.BudgetForecast.BudgetBase != null && i.RequestBudgetForecast.BudgetForecast.BudgetBase.Project != null ? i.RequestBudgetForecast.BudgetForecast.BudgetBase.Project.Code : ""
                         }
                     }))
                     .ForMember(i => i.Partner, opt => opt.MapFrom(i => new Dto.CodePartnerEntity { Id = i.Partner.Id, Name = i.Partner.Name }));
            CreateMap<Dto.EntityFile, Model.EntityFile>();
            CreateMap<Model.EntityType, Dto.EntityType>();
            CreateMap<Dto.EntityType, Model.EntityType>();
            CreateMap<Model.Country, Dto.Country>();
            CreateMap<Dto.Country, Model.Country>();
            CreateMap<Model.County, Dto.County>()
                .ForMember(i => i.Country, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Country.Id, Code = i.Country.Code, Name = i.Country.Name }));
            CreateMap<Dto.County, Model.County>()
                    .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                    .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                    .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                    .ForMember(i => i.Country, opt => opt.Ignore())
                    .ForMember(i => i.CountryId, opt => opt.MapFrom(i => i.CountryId));
            CreateMap<Model.City, Dto.City>()
                    .ForMember(i => i.County, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.County.Id, Code = i.County.Code, Name = i.County.Name }))
                    .ForMember(i => i.Country, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.County.Country.Id, Code = i.County.Country.Code, Name = i.County.Country.Name }));
            CreateMap<Dto.City, Model.City>()
                    .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                    .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                    .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                    .ForMember(i => i.County, opt => opt.Ignore())
                    .ForMember(i => i.CountyId, opt => opt.MapFrom(i => i.CountyId));

            CreateMap<Model.Inventory, Dto.Inventory>()
                 .ForMember(i => i.AccMonth, opt => opt.MapFrom(i => new Dto.AccMonth { Id = i.AccMonth.Id, Month = i.AccMonth.Month, Year = i.AccMonth.Year }))
				 .ForMember(i => i.AccMonthBudget, opt => opt.MapFrom(i => new Dto.AccMonth { Id = i.AccMonthBudget.Id, Month = i.AccMonthBudget.Month, Year = i.AccMonthBudget.Year }))
				 .ForMember(i => i.BudgetManager, opt => opt.MapFrom(i => new Dto.BudgetManager { Id = i.BudgetManager.Id, Code = i.BudgetManager.Code, Name = i.BudgetManager.Name }));
			CreateMap<Dto.Inventory, Model.Inventory>();

            CreateMap<Model.Division, Dto.Division>()
                     .ForMember(i => i.Department, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Department.Id, Code = i.Department.Code, Name = i.Department.Name }));
            //.ForMember(i => i.Location, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Location.Id, Code = i.Location.Code, Name = i.Location.Name }));
            CreateMap<Dto.Division, Model.Division>()
                    .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                    .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                    .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                    .ForMember(i => i.Department, opt => opt.Ignore())
                    .ForMember(i => i.DepartmentId, opt => opt.MapFrom(i => i.DepartmentId));
                    //.ForMember(i => i.Location, opt => opt.Ignore());
                    //.ForMember(i => i.LocationId, opt => opt.MapFrom(i => i.Location.Id));

            CreateMap<Model.DictionaryType, Dto.DictionaryType>();
            CreateMap<Dto.DictionaryType, Model.DictionaryType>();

            CreateMap<Model.EmailType, Dto.EmailType>();
            CreateMap<Dto.EmailType, Model.EmailType>();

			CreateMap<Model.ColumnFilter, Dto.ColumnFilter>();
			CreateMap<Dto.ColumnFilter, Model.ColumnFilter>()
				  .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				  .ForMember(i => i.Active, opt => opt.MapFrom(i => i.Active))
				  .ForMember(i => i.Placeholder, opt => opt.MapFrom(i => i.Placeholder))
				  .ForMember(i => i.Property, opt => opt.MapFrom(i => i.Property))
				  .ForMember(i => i.Model, opt => opt.MapFrom(i => i.Property))
				  .ForMember(i => i.Type, opt => opt.MapFrom(i => i.Type));

			CreateMap<Model.OrderType, Dto.OrderType>();
            CreateMap<Dto.OrderType, Model.OrderType>();

            CreateMap<Model.OfferType, Dto.OfferType>();
            CreateMap<Dto.OfferType, Model.OfferType>();


            CreateMap<Model.EmailManager, Dto.EmailManager>()
                .ForMember(i => i.Info, opt => opt.MapFrom(i => i.Info))
                .ForMember(i => i.CreatedAt, opt => opt.MapFrom(i => i.CreatedAt))
                .ForMember(i => i.EmailType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.EmailType.Id, Code = i.EmailType.Code, Name = i.EmailType.Name }))
                .ForMember(i => i.Asset, opt => opt.MapFrom(i => i.Asset != null ? new Dto.AssetComponent { Id = i.Asset.Id, Code = i.Asset.InvNo, Name = i.Asset.Name } : new Dto.AssetComponent { Id = i.AssetComponent.Id, Code = i.AssetComponent.Code, Name = i.AssetComponent.Name }))
                .ForMember(i => i.AssetComponent, opt => opt.MapFrom(i => new Dto.AssetComponent { Id = i.AssetComponent.Id, Code = i.AssetComponent.Code, Name = i.AssetComponent.Name }))
                .ForMember(i => i.EmployeeInitial, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeInitial.Id, InternalCode = i.EmployeeInitial.InternalCode, FirstName = i.EmployeeInitial.FirstName, LastName = i.EmployeeInitial.LastName }))
                .ForMember(i => i.EmployeeFinal, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeFinal.Id, InternalCode = i.EmployeeFinal.InternalCode, FirstName = i.EmployeeFinal.FirstName, LastName = i.EmployeeFinal.LastName }))
                .ForMember(i => i.RoomInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RoomInitial.Id, Code = i.RoomInitial.Code, Name = i.RoomInitial.Name }))
                .ForMember(i => i.RoomFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RoomFinal.Id, Code = i.RoomFinal.Code, Name = i.RoomFinal.Name }))
                .ForMember(i => i.SubType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubType.Id, Code = i.SubType.Code, Name = i.SubType.Name }))
                .ForMember(i => i.Offer, opt => opt.MapFrom(i => new Dto.Offer { Id = i.Offer.Id, Code = i.Offer.Code, Name = i.Offer.Name, Request = new Dto.Request { Id = i.Offer.Request.Id, Code = i.Offer.Request.Code} }))
                .ForMember(i => i.Order, opt => opt.MapFrom(i => new Dto.Order { Id = i.Order.Id, Name = i.Order.Name }))
                .ForMember(i => i.Partner, opt => opt.MapFrom(i => new Dto.CodePartnerEntity { Id = i.Partner.Id, Name = i.Partner.Name, RegistryNumber = i.Partner.RegistryNumber }))
                .ForMember(i => i.State, opt => opt.MapFrom(i => new Dto.AppState { Id = i.AppState.Id, Code = i.AppState.Code, ParentCode = i.AppState.ParentCode, Name = i.AppState.Name, BadgeColor = i.AppState.BadgeColor, BadgeIcon = i.AppState.BadgeIcon }));
            CreateMap<Dto.EmailManager, Model.EmailManager>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.EmailType, opt => opt.Ignore())
                .ForMember(i => i.EmailTypeId, opt => opt.MapFrom(i => i.EmailType.Id))
                .ForMember(i => i.Asset, opt => opt.Ignore())
                .ForMember(i => i.Asset, opt => opt.MapFrom(i => i.Asset.Id))
                .ForMember(i => i.EmployeeInitial, opt => opt.Ignore())
                .ForMember(i => i.EmployeeIdInitial, opt => opt.MapFrom(i => i.EmployeeInitial.Id))
                .ForMember(i => i.EmployeeFinal, opt => opt.Ignore())
                .ForMember(i => i.EmployeeIdFinal, opt => opt.MapFrom(i => i.EmployeeFinal.Id))
                .ForMember(i => i.RoomInitial, opt => opt.Ignore())
                .ForMember(i => i.RoomIdInitial, opt => opt.MapFrom(i => i.RoomInitial.Id))
                .ForMember(i => i.AppState, opt => opt.Ignore())
                .ForMember(i => i.RoomFinal, opt => opt.Ignore())
                .ForMember(i => i.RoomIdFinal, opt => opt.MapFrom(i => i.RoomFinal.Id));

            CreateMap<Model.EmailStatus, Dto.EmailStatus>();
            CreateMap<Dto.EmailStatus, Model.EmailStatus>();

            CreateMap<Model.EmailOrderStatus, Dto.EmailOrderStatus>();
            CreateMap<Dto.EmailOrderStatus, Model.EmailOrderStatus>();

            CreateMap<Model.Badge, Dto.Badge>();
            CreateMap<Dto.Badge, Model.Badge>();


            CreateMap<Model.PermissionType, Dto.PermissionType>();
            CreateMap<Dto.PermissionType, Model.PermissionType>();


            CreateMap<Model.Permission, Dto.Permission>()
                    .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                    .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                    .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                    .ForMember(i => i.PermissionType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.PermissionType.Id, Code = i.PermissionType.Code, Name = i.PermissionType.Name }));
            CreateMap<Dto.Permission, Model.Permission>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                .ForMember(i => i.PermissionType, opt => opt.Ignore())
                .ForMember(i => i.PermissionTypeId, opt => opt.MapFrom(i => i.PermissionType.Id));


            CreateMap<Model.PermissionRole, Dto.PermissionRole>()
                  .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                  .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                  .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                  .ForMember(i => i.Role, opt => opt.MapFrom(i => new Dto.RoleEntity { Id = i.Role.Id, Name = i.Role.Name }))
                  .ForMember(i => i.Permission, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Permission.Id, Code = i.Permission.Code, Name = i.Permission.Name }))
                  .ForMember(i => i.PermissionType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.PermissionType.Id, Code = i.PermissionType.Code, Name = i.PermissionType.Name }));
            CreateMap<Dto.PermissionRole, Model.PermissionRole>()
                  .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                  .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                  .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                  .ForMember(i => i.Permission, opt => opt.Ignore())
                  .ForMember(i => i.PermissionId, opt => opt.MapFrom(i => i.Permission.Id))
                  .ForMember(i => i.PermissionType, opt => opt.Ignore())
                  .ForMember(i => i.PermissionTypeId, opt => opt.MapFrom(i => i.PermissionType.Id))
                  .ForMember(i => i.Role, opt => opt.Ignore())
                  .ForMember(i => i.RoleId, opt => opt.MapFrom(i => i.Role.Id));




            CreateMap<Model.Location, Dto.Location>()
                 .ForMember(i => i.Region, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Region.Id, Code = i.Region.Code, Name = i.Region.Name }))
                 .ForMember(i => i.LocationType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.LocationType.Id, Code = i.LocationType.Code, Name = i.LocationType.Name }))
                 .ForMember(i => i.AdmCenter, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AdmCenter.Id, Code = i.AdmCenter.Code, Name = i.AdmCenter.Name }))
                 .ForMember(i => i.City, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.City.Id, Code = i.City.Code, Name = i.City.Name }))
                 .ForMember(i => i.County, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.City.County.Id, Code = i.City.County.Code, Name = i.City.County.Name }))
                 .ForMember(i => i.Employee, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.AdmCenter.Employee.Id, FirstName = i.AdmCenter.Employee.FirstName, LastName = i.AdmCenter.Employee.LastName, Email = i.AdmCenter.Employee.Email, InternalCode = i.AdmCenter.Employee.InternalCode }));
            CreateMap<Dto.Location, Model.Location>()
                    .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                    .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                    .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                    .ForMember(i => i.Member1, opt => opt.MapFrom(i => i.Member1))
                    .ForMember(i => i.Member2, opt => opt.MapFrom(i => i.Member2))
                    .ForMember(i => i.Member3, opt => opt.MapFrom(i => i.Member3))
                    .ForMember(i => i.Longitude, opt => opt.MapFrom(i => i.Longitude))
                    .ForMember(i => i.Latitude, opt => opt.MapFrom(i => i.Latitude))
                    .ForMember(i => i.AdmCenter, opt => opt.Ignore())
                    .ForMember(i => i.AdmCenterId, opt => opt.MapFrom(i => i.AdmCenterId))
                    .ForMember(i => i.City, opt => opt.Ignore())
                    .ForMember(i => i.CityId, opt => opt.MapFrom(i => i.CityId))
                    .ForMember(i => i.LocationType, opt => opt.Ignore())
                    .ForMember(i => i.LocationTypeId, opt => opt.Ignore())
                    .ForMember(i => i.Region, opt => opt.Ignore())
                    .ForMember(i => i.RegionId, opt => opt.MapFrom(i => i.RegionId));
            CreateMap<Model.Region, Dto.Region>();
            CreateMap<Dto.Region, Model.Region>();
            CreateMap<Model.Room, Dto.Room>()
                //.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Location, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Location.Id, Code = i.Location.Code, Name = i.Location.Name }))
                .ForMember(i => i.City, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Location.City.Id, Code = i.Location.City.Code, Name = i.Location.City.Name }))
                .ForMember(i => i.County, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Location.City.County.Id, Code = i.Location.City.County.Code, Name = i.Location.City.County.Name }))
                .ForMember(i => i.Region, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Location.Region.Id, Code = i.Location.Region.Code, Name = i.Location.Region.Name }))
                .ForMember(i => i.AdmCenter, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Location.AdmCenter.Id, Code = i.Location.AdmCenter.Code, Name = i.Location.AdmCenter.Name }))
                .ForMember(i => i.Employee, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Location.AdmCenter.Employee.Id, FirstName = i.Location.AdmCenter.Employee.FirstName, LastName = i.Location.AdmCenter.Employee.LastName, Email = i.Location.AdmCenter.Employee.Email, InternalCode = i.Location.AdmCenter.Employee.InternalCode }));
            CreateMap<Dto.Room, Model.Room>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                .ForMember(i => i.IsFinished, opt => opt.MapFrom(i => i.IsFinished))
                .ForMember(i => i.Location, opt => opt.Ignore())
                .ForMember(i => i.LocationId, opt => opt.MapFrom(i => i.Location.Id));

            CreateMap<Model.Administration, Dto.Administration>()
              .ForMember(i => i.Division, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Division.Id, Code = i.Division.Code, Name = i.Division.Name }));
              //.ForMember(i => i.CostCenter, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenter.Id, Code = i.CostCenter.Code, Name = i.CostCenter.Name }));
            CreateMap<Dto.Administration, Model.Administration>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                .ForMember(i => i.Division, opt => opt.Ignore())
                .ForMember(i => i.DivisionId, opt => opt.MapFrom(i => i.Division.Id));
            //.ForMember(i => i.CostCenter, opt => opt.Ignore())
            //.ForMember(i => i.CostCenterId, opt => opt.MapFrom(i => i.CostCenter.Id));

            CreateMap<Model.Rate, Dto.Rate>()
            .ForMember(i => i.Uom, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Uom.Id, Code = i.Uom.Code, Name = i.Uom.Name }));
            //.ForMember(i => i.CostCenter, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenter.Id, Code = i.CostCenter.Code, Name = i.CostCenter.Name }));
            CreateMap<Dto.Rate, Model.Rate>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                .ForMember(i => i.Value, opt => opt.MapFrom(i => i.Value))
                .ForMember(i => i.Uom, opt => opt.Ignore())
                .ForMember(i => i.UomId, opt => opt.MapFrom(i => i.Uom.Id));
            //.ForMember(i => i.CostCenter, opt => opt.Ignore())
            //.ForMember(i => i.CostCenterId, opt => opt.MapFrom(i => i.CostCenter.Id));

            CreateMap<Model.DictionaryItem, Dto.DictionaryItem>()
            .ForMember(i => i.DictionaryType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.DictionaryType.Id, Code = i.DictionaryType.Code, Name = i.DictionaryType.Name }))
            .ForMember(i => i.AssetCategory, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AssetCategory.Id, Code = i.AssetCategory.Code, Name = i.AssetCategory.Name }));
            CreateMap<Dto.DictionaryItem, Model.DictionaryItem>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                .ForMember(i => i.DictionaryType, opt => opt.Ignore())
                .ForMember(i => i.DictionaryTypeId, opt => opt.MapFrom(i => i.DictionaryType.Id))
                .ForMember(i => i.AssetCategory, opt => opt.Ignore())
                .ForMember(i => i.AssetCategoryId, opt => opt.MapFrom(i => i.AssetCategory.Id));

			CreateMap<Model.SubType, Dto.SubType>()
		        .ForMember(i => i.Type, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Type.Id, Code = i.Type.Code, Name = i.Type.Name }))
                .ForMember(i => i.MasterType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Type.MasterType.Id, Code = i.Type.MasterType.Code, Name = i.Type.MasterType.Name }));
            CreateMap<Dto.SubType, Model.SubType>()
				.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				.ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
				.ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
				.ForMember(i => i.Type, opt => opt.Ignore())
				.ForMember(i => i.TypeId, opt => opt.MapFrom(i => i.TypeId));

            CreateMap<Model.SubTypePartner, Dto.SubTypePartner>()
                .ForMember(i => i.SubType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubType.Id, Code = i.SubType.Code, Name = i.SubType.Name }))
                .ForMember(i => i.Partner, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Partner.Id, Code = i.Partner.RegistryNumber, Name = i.Partner.Name }));
            CreateMap<Dto.SubTypePartner, Model.SubTypePartner>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.SubType, opt => opt.Ignore())
                .ForMember(i => i.SubTypeId, opt => opt.MapFrom(i => i.SubTypeId))
                .ForMember(i => i.Partner, opt => opt.Ignore())
                .ForMember(i => i.PartnerId, opt => opt.MapFrom(i => i.PartnerId));

            CreateMap<Model.Type, Dto.Type>()
		   .ForMember(i => i.MasterType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.MasterType.Id, Code = i.MasterType.Code, Name = i.MasterType.Name }));
			CreateMap<Dto.Type, Model.Type>()
				.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				.ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
				.ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
				.ForMember(i => i.MasterType, opt => opt.Ignore())
				.ForMember(i => i.MasterTypeId, opt => opt.MapFrom(i => i.MasterTypeId));

			CreateMap<Model.Brand, Dto.Brand>()
                 .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
				.ForMember(i => i.Tag1Pattern, opt => opt.MapFrom(i => i.Tag1Pattern))
				.ForMember(i => i.Tag2Pattern, opt => opt.MapFrom(i => i.Tag2Pattern))
				.ForMember(i => i.Tag3Pattern, opt => opt.MapFrom(i => i.Tag3Pattern))
				.ForMember(i => i.Tag4Pattern, opt => opt.MapFrom(i => i.Tag4Pattern))
				.ForMember(i => i.Tag5Pattern, opt => opt.MapFrom(i => i.Tag5Pattern))
				.ForMember(i => i.Serial1Pattern, opt => opt.MapFrom(i => i.Serial1Pattern))
				.ForMember(i => i.Serial2Pattern, opt => opt.MapFrom(i => i.Serial2Pattern))
				.ForMember(i => i.Serial3Pattern, opt => opt.MapFrom(i => i.Serial3Pattern))
				.ForMember(i => i.Serial4Pattern, opt => opt.MapFrom(i => i.Serial4Pattern))
				.ForMember(i => i.Serial5Pattern, opt => opt.MapFrom(i => i.Serial5Pattern))
				.ForMember(i => i.Imei1Pattern, opt => opt.MapFrom(i => i.Imei1Pattern))
				.ForMember(i => i.Imei2Pattern, opt => opt.MapFrom(i => i.Imei2Pattern))
				.ForMember(i => i.Imei3Pattern, opt => opt.MapFrom(i => i.Imei3Pattern))
				.ForMember(i => i.Imei4Pattern, opt => opt.MapFrom(i => i.Imei4Pattern))
				.ForMember(i => i.Imei5Pattern, opt => opt.MapFrom(i => i.Imei5Pattern))
				.ForMember(i => i.PhoneNumber1Pattern, opt => opt.MapFrom(i => i.PhoneNumber1Pattern))
				.ForMember(i => i.PhoneNumber2Pattern, opt => opt.MapFrom(i => i.PhoneNumber2Pattern))
				.ForMember(i => i.PhoneNumber3Pattern, opt => opt.MapFrom(i => i.PhoneNumber3Pattern))
				.ForMember(i => i.PhoneNumber4Pattern, opt => opt.MapFrom(i => i.PhoneNumber4Pattern))
				.ForMember(i => i.PhoneNumber5Pattern, opt => opt.MapFrom(i => i.PhoneNumber5Pattern))
				.ForMember(i => i.DictionaryItem, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.DictionaryItem.Id, Code = i.DictionaryItem.Code, Name = i.DictionaryItem.Name }));
            CreateMap<Dto.Brand, Model.Brand>()
				.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				.ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
				.ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
				.ForMember(i => i.Tag1Pattern, opt => opt.MapFrom(i => i.Tag1Pattern))
				.ForMember(i => i.Tag2Pattern, opt => opt.MapFrom(i => i.Tag2Pattern))
				.ForMember(i => i.Tag3Pattern, opt => opt.MapFrom(i => i.Tag3Pattern))
				.ForMember(i => i.Tag4Pattern, opt => opt.MapFrom(i => i.Tag4Pattern))
				.ForMember(i => i.Tag5Pattern, opt => opt.MapFrom(i => i.Tag5Pattern))
				.ForMember(i => i.Serial1Pattern, opt => opt.MapFrom(i => i.Serial1Pattern))
				.ForMember(i => i.Serial2Pattern, opt => opt.MapFrom(i => i.Serial2Pattern))
				.ForMember(i => i.Serial3Pattern, opt => opt.MapFrom(i => i.Serial3Pattern))
				.ForMember(i => i.Serial4Pattern, opt => opt.MapFrom(i => i.Serial4Pattern))
				.ForMember(i => i.Serial5Pattern, opt => opt.MapFrom(i => i.Serial5Pattern))
				.ForMember(i => i.Imei1Pattern, opt => opt.MapFrom(i => i.Imei1Pattern))
				.ForMember(i => i.Imei2Pattern, opt => opt.MapFrom(i => i.Imei2Pattern))
				.ForMember(i => i.Imei3Pattern, opt => opt.MapFrom(i => i.Imei3Pattern))
				.ForMember(i => i.Imei4Pattern, opt => opt.MapFrom(i => i.Imei4Pattern))
				.ForMember(i => i.Imei5Pattern, opt => opt.MapFrom(i => i.Imei5Pattern))
			    .ForMember(i => i.PhoneNumber1Pattern, opt => opt.MapFrom(i => i.PhoneNumber1Pattern))
				.ForMember(i => i.PhoneNumber2Pattern, opt => opt.MapFrom(i => i.PhoneNumber2Pattern))
				.ForMember(i => i.PhoneNumber3Pattern, opt => opt.MapFrom(i => i.PhoneNumber3Pattern))
				.ForMember(i => i.PhoneNumber4Pattern, opt => opt.MapFrom(i => i.PhoneNumber4Pattern))
				.ForMember(i => i.PhoneNumber5Pattern, opt => opt.MapFrom(i => i.PhoneNumber5Pattern))
				.ForMember(i => i.DictionaryItem, opt => opt.Ignore())
                .ForMember(i => i.DictionaryItemId, opt => opt.MapFrom(i => i.DictionaryItem.Id));

            CreateMap<Model.Model, Dto.Model>()
                 .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                .ForMember(i => i.SNLength, opt => opt.MapFrom(i => i.SNLength))
                .ForMember(i => i.IMEILength, opt => opt.MapFrom(i => i.IMEILength))
                .ForMember(i => i.Brand, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Brand.Id, Code = i.Brand.Code, Name = i.Brand.Name }));
            CreateMap<Dto.Model, Model.Model>()
				.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				.ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
				.ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                .ForMember(i => i.SNLength, opt => opt.MapFrom(i => i.SNLength))
                .ForMember(i => i.IMEILength, opt => opt.MapFrom(i => i.IMEILength))
                .ForMember(i => i.Brand, opt => opt.Ignore())
                .ForMember(i => i.BrandId, opt => opt.MapFrom(i => i.Brand.Id));

            CreateMap<Model.InsuranceCategory, Dto.InsuranceCategory>();
			CreateMap<Dto.InsuranceCategory, Model.InsuranceCategory>()
				.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				.ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
				.ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name));

            CreateMap<Model.Company, Dto.Company>();
			CreateMap<Dto.Company, Model.Company>()
				.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				.ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
				.ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name));

			CreateMap<Model.MasterType, Dto.MasterType>();
			CreateMap<Dto.MasterType, Model.MasterType>()
				.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				.ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
				.ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name));

			CreateMap<Model.ApplicationRole, Dto.Role>();
			CreateMap<Dto.Role, Model.ApplicationRole>()
				.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				.ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name));

			CreateMap<Model.Project, Dto.Project>();
			CreateMap<Dto.Project, Model.Project>()
				.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				.ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
				.ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name));

            CreateMap<Model.ProjectType, Dto.ProjectType>();
            CreateMap<Dto.ProjectType, Model.ProjectType>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name));

            CreateMap<Model.Activity, Dto.Activity>();
            CreateMap<Dto.Activity, Model.Activity>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name));

            CreateMap<Model.Tax, Dto.Tax>();
            CreateMap<Dto.Tax, Model.Tax>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                .ForMember(i => i.Value, opt => opt.MapFrom(i => i.Value));


            CreateMap<Model.AssetComponent, Dto.AssetComponent>()
                .ForMember(i => i.Asset, opt => opt.MapFrom(i => new Dto.AssetEntity { Id = i.Asset.Id, InvNo = i.Asset.InvNo, Name = i.Asset.Name }))
                .ForMember(i => i.IsAccepted, opt => opt.MapFrom(i => i.IsAccepted))
                .ForMember(i => i.InvState, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.InvState.Id, Code = i.InvState.Code, Name = i.InvState.Name }))
                .ForMember(i => i.Employee, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Employee.Id, InternalCode = i.Employee.InternalCode, FirstName = i.Employee.FirstName, LastName = i.Employee.LastName }))
                .ForMember(i => i.SubType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubType.Id, Code = i.SubType.Code, Name = i.SubType.Name }))
                .ForMember(i => i.Type, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubType.Type.Id, Code = i.SubType.Type.Code, Name = i.SubType.Type.Name }))
                .ForMember(i => i.MasterType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.SubType.Type.MasterType.Id, Code = i.SubType.Type.MasterType.Code, Name = i.SubType.Type.MasterType.Name }));
            CreateMap<Dto.AssetComponent, Model.AssetComponent>()
                   .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                   .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                   .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                   //.ForMember(i => i.IsAccepted, opt => opt.MapFrom(i => i.IsAccepted))
                   .ForMember(i => i.IsAccepted, opt => opt.MapFrom(i => i.Id == 0 ? true : i.IsAccepted))
                   //.ForMember(i => i.Asset, opt => opt.Ignore())
                   //.ForMember(i => i.AssetId, opt => opt.MapFrom(i => i.Asset.Id))
                   //.ForMember(i => i.Company, opt => opt.Ignore())
                   //.ForMember(i => i.CompanyId, opt => opt.MapFrom(i => i.Company.Id))
                   .ForMember(i => i.Employee, opt => opt.Ignore())
                   .ForMember(i => i.EmployeeId, opt => opt.MapFrom(i => i.Employee.Id))
                   .ForMember(i => i.SubType, opt => opt.Ignore())
                   .ForMember(i => i.SubTypeId, opt => opt.MapFrom(i => i.SubType.Id));



            CreateMap<Model.Room, Dto.Sync.Room>();
            CreateMap<Model.Employee, Dto.EmployeeSync>();

            CreateMap<Model.ConfigValue, Dto.ConfigValue>()
                .ForMember(i => i.Role, opt => opt.MapFrom(i => new Dto.RoleEntity { Name = i.AspNetRole.Name }))
                .ForMember(i => i.RoleId, opt => opt.MapFrom(i => i.RoleId));
            CreateMap<Dto.ConfigValue, Model.ConfigValue>()
                .ForMember(i => i.AspNetRole, opt => opt.Ignore())
                .ForMember(i => i.RoleId, opt => opt.MapFrom(i => i.RoleId));
            CreateMap<Model.TableDefinition, Dto.TableDefinition>();
            CreateMap<Dto.TableDefinition, Model.TableDefinition>();
            CreateMap<Model.ColumnDefinition, Dto.ColumnDefinition>()
				.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				.ForMember(i => i.HeaderCode, opt => opt.MapFrom(i => i.HeaderCode))
				.ForMember(i => i.Property, opt => opt.MapFrom(i => i.Property))
				.ForMember(i => i.Include, opt => opt.MapFrom(i => i.Include))
				.ForMember(i => i.SortBy, opt => opt.MapFrom(i => i.SortBy))
				.ForMember(i => i.Pipe, opt => opt.MapFrom(i => i.Pipe))
				.ForMember(i => i.Format, opt => opt.MapFrom(i => i.Format))
				.ForMember(i => i.TextAlign, opt => opt.MapFrom(i => i.TextAlign))
				.ForMember(i => i.Active, opt => opt.MapFrom(i => i.Active))
				.ForMember(i => i.Position, opt => opt.MapFrom(i => i.Position))
				.ForMember(i => i.TableDefinitionId, opt => opt.MapFrom(i => i.TableDefinitionId))
				.ForMember(i => i.TableDefinition, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.TableDefinition.Id, Code = i.TableDefinition.Code, Name = i.TableDefinition.Name }))
				.ForMember(i => i.Role, opt => opt.MapFrom(i => new Dto.RoleEntity { Id = i.AspNetRole.Id, Name = i.AspNetRole.Name }))
				.ForMember(i => i.ColumnFilter, opt => opt.MapFrom(i => new Dto.ColumnFilter { Id = i.ColumnFilter.Id, Model = i.ColumnFilter.Model, Property = i.ColumnFilter.Property, Active= i.ColumnFilter.Active, Type= i.ColumnFilter.Type, Placeholder = i.ColumnFilter.Placeholder }))
				.ForMember(i => i.RoleId, opt => opt.MapFrom(i => i.RoleId));
			CreateMap<Dto.ColumnDefinition, Model.ColumnDefinition>()
				.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				.ForMember(i => i.HeaderCode, opt => opt.MapFrom(i => i.HeaderCode))
				.ForMember(i => i.Property, opt => opt.MapFrom(i => i.Property))
				.ForMember(i => i.Include, opt => opt.MapFrom(i => i.Include))
				.ForMember(i => i.SortBy, opt => opt.MapFrom(i => i.SortBy))
				.ForMember(i => i.Pipe, opt => opt.MapFrom(i => i.Pipe))
				.ForMember(i => i.Format, opt => opt.MapFrom(i => i.Format))
				.ForMember(i => i.TextAlign, opt => opt.MapFrom(i => i.TextAlign))
				.ForMember(i => i.Active, opt => opt.MapFrom(i => i.Active))
				.ForMember(i => i.Position, opt => opt.MapFrom(i => i.Position))
				.ForMember(i => i.TableDefinition, opt => opt.Ignore())
				.ForMember(i => i.TableDefinitionId, opt => opt.MapFrom(i => i.TableDefinitionId))
				.ForMember(i => i.AspNetRole, opt => opt.Ignore())
				.ForMember(i => i.RoleId, opt => opt.MapFrom(i => i.RoleId))
                .ForMember(i => i.ColumnFilter, opt => opt.Ignore())
				.ForMember(i => i.ColumnFilterId, opt => opt.MapFrom(i => i.ColumnFilter.Id));


			CreateMap<Model.Route, Dto.Route>()
                //.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                .ForMember(i => i.Url, opt => opt.MapFrom(i => i.Url))
                .ForMember(i => i.Icon, opt => opt.MapFrom(i => i.Icon))
                .ForMember(i => i.Divider, opt => opt.MapFrom(i => i.Divider))
                .ForMember(i => i.Variant, opt => opt.MapFrom(i => i.Variant))
                .ForMember(i => i.Active, opt => opt.MapFrom(i => i.Active))
                .ForMember(i => i.Class, opt => opt.MapFrom(i => i.Class))
                 .ForMember(i => i.Position, opt => opt.MapFrom(i => i.Position))
                .ForMember(i => i.Badge, opt => opt.MapFrom(i => new Dto.Badge { Id = i.Badge.Id, Variant = i.Badge.Variant, Text = i.Badge.Text }))
                .ForMember(i => i.Role, opt => opt.MapFrom(i => new Dto.RoleEntity { Id = i.Role.Id, Name = i.Role.Name }))
                .ForMember(i => i.RoleId, opt => opt.MapFrom(i => i.RoleId));


            CreateMap<Dto.Route, Model.Route>()
				.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				.ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
				.ForMember(i => i.Url, opt => opt.MapFrom(i => i.Url))
				.ForMember(i => i.Icon, opt => opt.MapFrom(i => i.Icon))
                .ForMember(i => i.Divider, opt => opt.MapFrom(i => i.Divider))
                 .ForMember(i => i.Variant, opt => opt.MapFrom(i => i.Variant))
                .ForMember(i => i.Active, opt => opt.MapFrom(i => i.Active))
                .ForMember(i => i.Class, opt => opt.MapFrom(i => i.Class))
                .ForMember(i => i.Position, opt => opt.MapFrom(i => i.Position))
                .ForMember(i => i.Badge, opt => opt.Ignore())
                .ForMember(i => i.BadgeId, opt => opt.MapFrom(i => i.Badge.Id))
                .ForMember(i => i.Role, opt => opt.Ignore())
                .ForMember(i => i.RoleId, opt => opt.MapFrom(i => i.RoleId));
            CreateMap<Model.RouteChildren, Dto.RouteChildren>()
              .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
              .ForMember(i => i.Url, opt => opt.MapFrom(i => i.Url))
              .ForMember(i => i.Icon, opt => opt.MapFrom(i => i.Icon))
              .ForMember(i => i.Variant, opt => opt.MapFrom(i => i.Variant))
              .ForMember(i => i.Active, opt => opt.MapFrom(i => i.Active))
              .ForMember(i => i.Class, opt => opt.MapFrom(i => i.Class))
              .ForMember(i => i.Position, opt => opt.MapFrom(i => i.Position))
              .ForMember(i => i.Badge, opt => opt.MapFrom(i => new Dto.Badge { Id = i.Badge.Id, Variant = i.Badge.Variant, Text = i.Badge.Text }))
              .ForMember(i => i.Route, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Route.Id, Name = i.Route.Name }))
              .ForMember(i => i.RouteId, opt => opt.MapFrom(i => i.RouteId))
              .ForMember(i => i.Role, opt => opt.MapFrom(i => new Dto.RoleEntity { Id = i.Role.Id, Name = i.Role.Name }))
              .ForMember(i => i.RoleId, opt => opt.MapFrom(i => i.RoleId));
            CreateMap<Dto.RouteChildren, Model.RouteChildren>()
				.ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
				.ForMember(i => i.Url, opt => opt.MapFrom(i => i.Url))
				.ForMember(i => i.Icon, opt => opt.MapFrom(i => i.Icon))
                .ForMember(i => i.Variant, opt => opt.MapFrom(i => i.Variant))
                .ForMember(i => i.Active, opt => opt.MapFrom(i => i.Active))
                .ForMember(i => i.Class, opt => opt.MapFrom(i => i.Class))
                .ForMember(i => i.Position, opt => opt.MapFrom(i => i.Position))
                .ForMember(i => i.Badge, opt => opt.Ignore())
                .ForMember(i => i.BadgeId, opt => opt.MapFrom(i => i.Badge.Id))
                .ForMember(i => i.Route, opt => opt.Ignore())
                .ForMember(i => i.RouteId, opt => opt.MapFrom(i => i.RouteId))
			    .ForMember(i => i.Role, opt => opt.Ignore())
			    .ForMember(i => i.RoleId, opt => opt.MapFrom(i => i.RoleId));


			CreateMap<Model.ApplicationUser, Dto.ApplicationUser>()
               .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                .ForMember(i => i.UserName, opt => opt.MapFrom(i => i.UserName))
				.ForMember(i => i.LastLogin, opt => opt.MapFrom(i => i.LastLogin))
				//.ForMember(i => i.Region, opt => opt.MapFrom(i => i.Region))
				.ForMember(i => i.Email, opt => opt.MapFrom(i => i.Email))
				.ForMember(i => i.MobilePhone, opt => opt.MapFrom(i => new Dto.MobilePhone { Id = i.MobilePhone.Id, Name = i.MobilePhone.Name }))
				.ForMember(i => i.Role, opt => opt.MapFrom(i => i.Claims.Where(c => c.ClaimType == "role" && c.UserId == i.Id).ToList().FirstOrDefault().ClaimValue))
                //.ForMember(i => i.AdmCenters, opt => opt.MapFrom(i => i.Claims.Where(c => c.ClaimType == "admCenter").ToList().FirstOrDefault().ClaimValue))
                .ForMember(i => i.GivenName, opt => opt.MapFrom(i => i.Claims.Where(c => c.ClaimType == "given_name").ToList().FirstOrDefault().ClaimValue))
                .ForMember(i => i.FamilyName, opt => opt.MapFrom(i => i.Claims.Where(c => c.ClaimType == "family_name").ToList().FirstOrDefault().ClaimValue))
                .ForMember(i => i.Mac, opt => opt.MapFrom(i => i.Claims.Where(c => c.ClaimType == "printerAddress").ToList().FirstOrDefault().ClaimValue))
                .ForMember(i => i.Device, opt => opt.MapFrom(i => new Dto.Device { Id = i.Device.Id, UUI = i.Device.UUI }))
                //.ForMember(i => i.TempInterval7, opt => opt.MapFrom(i => i.Claims.Where(c => c.ClaimType == "tempInterval7").ToList().FirstOrDefault().ClaimValue))
                //.ForMember(i => i.TempInterval8, opt => opt.MapFrom(i => i.Claims.Where(c => c.ClaimType == "tempInterval8").ToList().FirstOrDefault().ClaimValue))
                //.ForMember(i => i.TempInterval9, opt => opt.MapFrom(i => i.Claims.Where(c => c.ClaimType == "tempInterval9").ToList().FirstOrDefault().ClaimValue))
                //.ForMember(i => i.TempInterval10, opt => opt.MapFrom(i => i.Claims.Where(c => c.ClaimType == "tempInterval10").ToList().FirstOrDefault().ClaimValue))
                //.ForMember(i => i.CompanyTypes, opt => opt.MapFrom(i => i.Claims.Where(c => c.ClaimType == "companyType").ToList().FirstOrDefault().ClaimValue))
                //.ForMember(i => i.Locations, opt => opt.MapFrom(i => i.Claims.Where(c => c.ClaimType == "location").ToList().FirstOrDefault().ClaimValue))
                //.ForMember(i => i.AdmCenter, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AdmCenter.Id, Code = i.AdmCenter.Code, Name = i.AdmCenter.Name }))
                .ForMember(i => i.Substitute, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Substitute.Id, InternalCode = i.Substitute.InternalCode, FirstName = i.Substitute.FirstName, LastName = i.Substitute.LastName }))
                .ForMember(i => i.Employee, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Employee.Id, InternalCode = i.Employee.InternalCode, FirstName = i.Employee.FirstName, LastName = i.Employee.LastName, Validate = i.Employee.IsConfirmed, IsDeleted = i.Employee.IsDeleted }));

            CreateMap<Model.InventoryAsset, Dto.InventoryAssetResource>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Asset.Id))
                .ForMember(i => i.InvNo, opt => opt.MapFrom(i => i.Asset.InvNo))
				 .ForMember(i => i.AssetId, opt => opt.MapFrom(i => i.Asset.Id))
				.ForMember(i => i.ERPCode, opt => opt.MapFrom(i => i.Asset.ERPCode))
                .ForMember(i => i.SAPCode, opt => opt.MapFrom(i => i.Asset.SAPCode))
                .ForMember(i => i.TempReco, opt => opt.MapFrom(i => i.TempReco))
                .ForMember(i => i.TempName, opt => opt.MapFrom(i => i.TempName))
				.ForMember(i => i.IsMinus, opt => opt.MapFrom(i => i.IsMinus))
				.ForMember(i => i.InfoMinus, opt => opt.MapFrom(i => i.InfoMinus))
				.ForMember(i => i.IsPlus, opt => opt.MapFrom(i => i.IsPlus))
				.ForMember(i => i.InfoPlus, opt => opt.MapFrom(i => i.InfoPlus))
				.ForMember(i => i.InvName, opt => opt.MapFrom(i => i.Asset.AssetInv.InvName))
                .ForMember(i => i.ValueInv, opt => opt.MapFrom(i => i.Asset.ValueInv))
                .ForMember(i => i.ValueDep, opt => opt.MapFrom(i => i.Asset.ValueRem))
                .ForMember(i => i.QIntial, opt => opt.MapFrom(i => i.QInitial))
                .ForMember(i => i.QFinal, opt => opt.MapFrom(i => i.QFinal))
                .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Asset.Name))
				.ForMember(i => i.CurrentAPC, opt => opt.MapFrom(i => i.CurrentAPC))
				.ForMember(i => i.CurrBkValue, opt => opt.MapFrom(i => i.CurrBkValue))
				.ForMember(i => i.AccumulDep, opt => opt.MapFrom(i => i.AccumulDep))
				.ForMember(i => i.AppState, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Asset.AppState.Id, Code = i.Asset.AppState.Code, Name = i.Asset.AppState.Name }))
                .ForMember(i => i.State, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AssetRecoStateId.GetValueOrDefault(), Code = i.AssetRecoState.Code, Name = i.AssetRecoState.Name }))
                .ForMember(i => i.Model, opt => opt.MapFrom(i => new Dto.Model { Id = i.Asset.Model.Id, Code = i.Asset.Model.Code, Name = i.Asset.Model.Name }))
				.ForMember(i => i.Brand, opt => opt.MapFrom(i => new Dto.Brand { Id = i.Asset.Brand.Id, Code = i.Asset.Brand.Code, Name = i.Asset.Brand.Name }))
                .ForMember(i => i.DictionaryItem, opt => opt.MapFrom(i => new Dto.DictionaryItem { Id = i.Asset.DictionaryItem.Id, Code = i.Asset.DictionaryItem.Code, Name = i.Asset.DictionaryItem.Name }))
                .ForMember(i => i.ImageCount, opt => opt.MapFrom(i => i.ImageCount))
                .ForMember(i => i.Latitude, opt => opt.MapFrom(i => i.RoomInitial.Location.Latitude))
                .ForMember(i => i.Longitude, opt => opt.MapFrom(i => i.RoomInitial.Location.Longitude))
                .ForMember(i => i.PurchaseDate, opt => opt.MapFrom(i => i.Asset.PurchaseDate))
                .ForMember(i => i.RoomInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RoomInitial.Id, Code = i.RoomInitial.Code, Name = i.RoomInitial.Name }))
                .ForMember(i => i.LocationInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RoomInitial.Location.Id, Code = i.RoomInitial.Location.Code, Name = i.RoomInitial.Location.Name }))
                .ForMember(i => i.AdministrationInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AdministrationInitial.Id, Code = i.AdministrationInitial.Code, Name = i.AdministrationInitial.Name }))
                .ForMember(i => i.DivisionInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AdministrationInitial.Division.Id, Code = i.AdministrationInitial.Division.Code, Name = i.AdministrationInitial.Division.Name }))
                .ForMember(i => i.RoomFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RoomFinal.Id, Code = i.RoomFinal.Code, Name = i.RoomFinal.Name }))
                .ForMember(i => i.LocationFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RoomFinal.Location.Id, Code = i.RoomFinal.Location.Code, Name = i.RoomFinal.Location.Name }))
                .ForMember(i => i.EmployeeInitial, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeInitial.Id, InternalCode = i.EmployeeInitial.InternalCode, FirstName = i.EmployeeInitial.FirstName, LastName = i.EmployeeInitial.LastName, Email = i.EmployeeInitial.Email }))
                .ForMember(i => i.EmployeeFinal, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.EmployeeFinal.Id, InternalCode = i.EmployeeFinal.InternalCode, FirstName = i.EmployeeFinal.FirstName, LastName = i.EmployeeFinal.LastName, Email = i.EmployeeFinal.Email }))
				.ForMember(i => i.InventoryTeamManager, opt => opt.MapFrom(i => new Dto.ApplicationUser { Id = i.InventoryTeamManager.Id, UserName = i.InventoryTeamManager.UserName, Email = i.InventoryTeamManager.Email }))
				.ForMember(i => i.InventoryResponsable, opt => opt.MapFrom(i => new Dto.ApplicationUser { Id = i.InventoryResponsable.Id, UserName = i.InventoryResponsable.UserName, Email = i.InventoryResponsable.Email }))
				.ForMember(i => i.CostCenterInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenterInitial.Id, Code = i.CostCenterInitial.Code, Name = i.CostCenterInitial.Name }))
                .ForMember(i => i.AdmCenterInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenterInitial.AdmCenter.Id, Code = i.CostCenterInitial.AdmCenter.Code, Name = i.CostCenterInitial.AdmCenter.Name }))
                .ForMember(i => i.RegionInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RoomInitial.Location.Region.Id, Code = i.RoomInitial.Location.Region.Code, Name = i.RoomInitial.Location.Region.Name }))
                .ForMember(i => i.CostCenterFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenterFinal.Id, Code = i.CostCenterFinal.Code, Name = i.CostCenterFinal.Name }))
                .ForMember(i => i.AdmCenterFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.CostCenterFinal.AdmCenter.Id, Code = i.CostCenterFinal.AdmCenter.Code, Name = i.CostCenterFinal.AdmCenter.Name }))
                .ForMember(i => i.RegionFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.RoomFinal.Location.Region.Id, Code = i.RoomFinal.Location.Region.Code, Name = i.RoomFinal.Location.Region.Name }))
                .ForMember(i => i.Uom, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Asset.Uom.Id, Code = i.Asset.Uom.Code, Name = i.Asset.Uom.Name }))
                .ForMember(i => i.AssetCategory, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Asset.AssetCategory.Id, Code = i.Asset.AssetCategory.Code, Name = i.Asset.AssetCategory.Name }))
                .ForMember(i => i.MasterType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Asset.SubType.Type.MasterType.Id, Code = i.Asset.SubType.Type.MasterType.Code, Name = i.Asset.SubType.Type.MasterType.Name }))
                .ForMember(i => i.Type, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Asset.SubType.Type.Id, Code = i.Asset.SubType.Type.Code, Name = i.Asset.SubType.Type.Name }))
                //.ForMember(i => i.Model, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Asset.Model.Id, Code = i.Asset.Model.Code, Name = i.Asset.Model.Name }))
                //.ForMember(i => i.Brand, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Asset.Brand.Id, Code = i.Asset.Brand.Code, Name = i.Asset.Brand.Name }))
                .ForMember(i => i.ModifiedAt, opt => opt.MapFrom(i => i.ModifiedAt))
                .ForMember(i => i.Info, opt => opt.MapFrom(i => i.Info))
                .ForMember(i => i.UserName, opt => opt.MapFrom(i => i.ModifiedByUser.UserName ?? ""))
                .ForMember(i => i.SerialNumberInitial, opt => opt.MapFrom(i => i.Asset.SerialNumber))
				.ForMember(i => i.SerialNumberFinal, opt => opt.MapFrom(i => i.Asset.SerialNumber))
				.ForMember(i => i.SerialNumber, opt => opt.MapFrom(i => i.Asset.SerialNumber))
                .ForMember(i => i.AllowLabel, opt => opt.MapFrom(i => i.Asset.AssetInv.AllowLabel))
                .ForMember(i => i.Custody, opt => opt.MapFrom(i => i.Asset.Custody))
                .ForMember(i => i.ModifiedBy, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.ModifiedByUser.Employee.Id, InternalCode = i.ModifiedByUser.Employee.InternalCode, FirstName = i.ModifiedByUser.Employee.FirstName, LastName = i.ModifiedByUser.Employee.LastName }))
                .ForMember(i => i.AssetType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Asset.AssetType.Id, Code = i.Asset.AssetType.Code, Name = i.Asset.AssetType.Name }))
                .ForMember(i => i.Partner, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Asset.Document.Partner.Id, Code = i.Asset.Document.Partner.FiscalCode, Name = i.Asset.Document.Partner.Name }))
                .ForMember(i => i.InvStateInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.StateInitial.Id, Code = i.StateInitial.Code, Name = i.StateInitial.Name }))
                .ForMember(i => i.InvStateFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.StateFinal.Id, Code = i.StateFinal.Code, Name = i.StateFinal.Name }))
                .ForMember(i => i.AdministrationFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AdministrationFinal.Id, Code = i.AdministrationFinal.Code, Name = i.AdministrationFinal.Name }))
                .ForMember(i => i.DivisionFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.AdministrationFinal.Division.Id, Code = i.AdministrationFinal.Division.Code, Name = i.AdministrationFinal.Division.Name }))
                .ForMember(i => i.DocNo1, opt => opt.MapFrom(i => i.Asset.Document.DocNo1));

            CreateMap<Model.InventoryAsset, Dto.InventoryAssetWHResource>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Asset.Id))
				.ForMember(i => i.UserName, opt => opt.MapFrom(i => i.InventoryTeamManager.UserName))
				.ForMember(i => i.InventoryTeamManager, opt => opt.MapFrom(i => new Dto.ApplicationUser { Id = i.InventoryTeamManager.Id, UserName = i.InventoryTeamManager.UserName, Email = i.InventoryTeamManager.Email }))
                .ForMember(i => i.InventoryResponsable, opt => opt.MapFrom(i => new Dto.ApplicationUser { Id = i.InventoryResponsable.Id, UserName = i.InventoryResponsable.UserName, Email = i.InventoryResponsable.Email }));
				


			CreateMap<Model.InventoryAsset, Dto.Reporting.InventoryListEmail>()
				   .ForMember(i => i.AssetId, opt => opt.MapFrom(i => i.Asset.Id))
				   .ForMember(i => i.IsMinus, opt => opt.MapFrom(i => i.IsMinus))
				   .ForMember(i => i.InfoMinus, opt => opt.MapFrom(i => i.InfoMinus))
				   .ForMember(i => i.IsPlus, opt => opt.MapFrom(i => i.IsPlus))
				   .ForMember(i => i.InfoPlus, opt => opt.MapFrom(i => i.InfoPlus))
				   .ForMember(i => i.AppStateId, opt => opt.MapFrom(i => i.Asset.AppStateId))
				   .ForMember(i => i.AppState, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Asset.AppState.Id, Code = i.Asset.AppState.Code, Name = i.Asset.AppState.Name }))
				   .ForMember(i => i.InvNo, opt => opt.MapFrom(i => i.Asset.InvNo))
				   .ForMember(i => i.Description, opt => opt.MapFrom(i => i.Asset.Name))
				   .ForMember(i => i.PurchaseDate, opt => opt.MapFrom(i => i.Asset.PurchaseDate))
				   .ForMember(i => i.ValueDep, opt => opt.MapFrom(i => i.Asset.ValueRem))
				   .ForMember(i => i.MasterType, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Asset.SubType.Type.MasterType.Id, Code = i.Asset.SubType.Type.MasterType.Code, Name = i.Asset.SubType.Type.MasterType.Name }))
				   .ForMember(i => i.Type, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Asset.SubType.Type.Id, Code = i.Asset.SubType.Type.Code, Name = i.Asset.SubType.Type.Name }))
				   .ForMember(i => i.Model, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Asset.Model.Id, Code = i.Asset.Model.Code, Name = i.Asset.Model.Name }))
				   .ForMember(i => i.Brand, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Asset.Brand.Id, Code = i.Asset.Brand.Code, Name = i.Asset.Brand.Name }))
				   .ForMember(i => i.SerialNumber, opt => opt.MapFrom(i => i.Asset.SerialNumber))
				   .ForMember(i => i.InternalCode, opt => opt.MapFrom(p => (p.Asset.Employee != null) ? p.Asset.Employee.InternalCode : ""))
				   .ForMember(i => i.FullName, opt => opt.MapFrom(p => (p.Asset.Employee != null) ? new Dto.EmployeeResource { Id = p.Asset.Employee.Id, InternalCode = p.Asset.Employee.InternalCode, FirstName = p.Asset.Employee.FirstName, LastName = p.Asset.Employee.LastName } : new Dto.EmployeeResource() { }))
				   .ForMember(i => i.Reason, opt => opt.MapFrom(i => i.Asset.InfoPlus));
			  


			CreateMap<Model.TableDefinition, Dto.TableDefinitionData>()
                .ForMember(i => i.TableDefinition, opt => opt.MapFrom(i => new Dto.TableDefinitionBase()
                {
                    Code = i.Code,
                    Name = i.Name,
                    Description = i.Description
                }))
                .ForMember(i => i.ColumnDefinitions, opt => opt.MapFrom(i => i.ColumnDefinitions.Select(p => new Dto.ColumnDefinitionBase()
                {
                    HeaderCode = p.HeaderCode, Property = p.Property, Include = p.Include, SortBy = p.SortBy, Position = p.Position, TextAlign = p.TextAlign, Pipe = p.Pipe, Format = p.Format, Active = p.Active
                })));


            CreateMap<Model.Route, Dto.RouteData>()
            .ForMember(i => i.Url, opt => opt.MapFrom(i => i.Url))
            .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
            .ForMember(i => i.Icon, opt => opt.MapFrom(i => i.Icon))
            .ForMember(i => i.Class, opt => opt.MapFrom(i => i.Class))
            .ForMember(i => i.Variant, opt => opt.MapFrom(i => i.Variant))
            .ForMember(i => i.Color, opt => opt.MapFrom(i => i.Variant))
            .ForMember(i => i.Title, opt => opt.MapFrom(i => i.Title))
            .ForMember(i => i.Divider, opt => opt.MapFrom(i => i.Divider))
            .ForMember(i => i.Active, opt => opt.MapFrom(i => i.Active))
            .ForMember(i => i.Badge, opt => opt.MapFrom(i => new Dto.BadgeBase { Variant = i.Badge.Variant, Text = i.Badge.Text }))
            .ForMember(i => i.Children, opt => opt.MapFrom(i => i.RouteChildrens.Where(p => p.IsDeleted == false && p.Active == true).OrderBy(a => a.Position).Select(p => new Dto.RouteChildrenBase()
            {
                Url = p.Url,
                Name = p.Name,
                Icon = p.Icon,
                Divider = p.Divider,
                Title = p.Title,
                Badge = new Dto.BadgeBase() { Variant = p.Badge != null ? p.Badge.Variant : "", Text = p.Badge != null ? p.Badge.Text : "" }
            })));

            CreateMap<Model.Matrix, Dto.MatrixData>()
                  .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                  .ForMember(i => i.Children, opt => opt.MapFrom(i => i.MatrixLevels.Where(p => p.IsDeleted == false).OrderBy(a => a.LevelId).Select(p => new Dto.MatrixChildrenBase()
                  {
                      Employee = new Dto.EmployeeResource() { Email = p.Employee != null ? p.Employee.Email : "" , FirstName = p.Employee != null ? p.Employee.FirstName : "", LastName = p.Employee != null ? p.Employee.LastName : "" },
                      Amount = p.Amount
                  })));

            CreateMap<Model.Department, Dto.DepartmentSync>();
            CreateMap<Model.Division, Dto.DivisionSync>();
            CreateMap<Model.CostCenter, Dto.CostCenterSync>()
                 .ForMember(i => i.IsFinished, opt => opt.MapFrom(i => i.ERPId));
            CreateMap<Model.EmployeeCostCenter, Dto.EmployeeCostCenterSync>();
			CreateMap<Model.EmployeeCompany, Dto.EmployeeCompanySync>();
			CreateMap<Model.Country, Dto.CountrySync>();
            CreateMap<Model.County, Dto.CountySync>();
            CreateMap<Model.City, Dto.CitySync>();
            CreateMap<Model.Location, Dto.LocationSync>();
            CreateMap<Model.Room, Dto.RoomSync>();
            CreateMap<Model.DictionaryType, Dto.DictionaryTypeSync>();
            CreateMap<Model.DictionaryItem, Dto.DictionaryItemSync>();
            CreateMap<Model.Dimension, Dto.DimensionSync>();
            CreateMap<Model.Company, Dto.CompanySync>();
            CreateMap<Model.InsuranceCategory, Dto.InsuranceCategorySync>();
            CreateMap<Model.AssetNature, Dto.AssetNatureSync>();
            CreateMap<Model.Material, Dto.MaterialSync>();

            CreateMap<Model.Asset, Dto.AssetSync>()
              .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Id))
              .ForMember(a => a.InvNo, opt => opt.MapFrom(a => a.InvNo))
              .ForMember(a => a.InvNoOld, opt => opt.MapFrom(a => a.TempReco))
              .ForMember(a => a.Name, opt => opt.MapFrom(a => a.Name))
              .ForMember(a => a.SerialNumber, opt => opt.MapFrom(a => a.SerialNumber))
              .ForMember(a => a.Info, opt => opt.MapFrom(a => a.Info))
              .ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Quantity))
              //.ForMember(a => a.AllowLabel, opt => opt.MapFrom(a => a.AllowLabel))
              .ForMember(a => a.RoomId, opt => opt.MapFrom(a => a.RoomId))
              //.ForMember(a => a.AdministrationId, opt => opt.MapFrom(a => a.AdministrationId))
              .ForMember(a => a.InvStateId, opt => opt.MapFrom(a => a.InvStateId))
              .ForMember(a => a.ModifiedAt, opt => opt.MapFrom(a => a.ModifiedAt))
              .ForMember(a => a.PurchaseDate, opt => opt.MapFrom(a => a.PurchaseDate))
              .ForMember(a => a.ValueInv, opt => opt.MapFrom(a => a.ValueInv))
              //.ForMember(a => a.DimensionId, opt => opt.MapFrom(a => a.DimensionId))
              .ForMember(a => a.EmployeeId, opt => opt.MapFrom(a => a.EmployeeId))
              //.ForMember(a => a.UomId, opt => opt.MapFrom(a => a.UomId))
              //.ForMember(a => a.AssetCategoryId, opt => opt.MapFrom(a => a.AssetCategoryId))
              //.ForMember(a => a.AssetTypeId, opt => opt.MapFrom(a => a.AssetTypeId))
              .ForMember(a => a.ImageCount, opt => opt.MapFrom(a => a.ImageCount))
              .ForMember(a => a.IsDeleted, opt => opt.MapFrom(a => a.IsDeleted))
              //.ForMember(a => a.IsMinus, opt => opt.MapFrom(a => a.IsMinus))
              //.ForMember(a => a.InfoMinus, opt => opt.MapFrom(a => a.InfoMinus))
              .ForMember(a => a.ErpCode, opt => opt.MapFrom(a => a.ERPCode))
              .ForMember(a => a.SapCode, opt => opt.MapFrom(a => a.SAPCode))
              .ForMember(a => a.SubNo, opt => opt.MapFrom(a => a.SubNo))
              .ForMember(a => a.IsPrinted, opt => opt.MapFrom(a => a.IsPrinted))
			  .ForMember(a => a.IsTemp, opt => opt.MapFrom(a => a.IsTemp))
			  //.ForMember(a => a.CompanyId, opt => opt.MapFrom(a => a.CompanyId))
			  //.ForMember(a => a.AssetNatureId, opt => opt.MapFrom(a => a.AssetNatureId))
			  //.ForMember(a => a.InterCompanyId, opt => opt.MapFrom(a => a.InterCompanyId))
			  //.ForMember(a => a.InsuranceCategoryId, opt => opt.MapFrom(a => a.InsuranceCategoryId))
			  .ForMember(a => a.TempUserId, opt => opt.MapFrom(a => a.TempUserName))
			  .ForMember(a => a.CostCenterId, opt => opt.MapFrom(a => a.CostCenterId))
              .ForMember(a => a.MaterialId, opt => opt.MapFrom(a => a.MaterialId));
              //.ForMember(a => a.DictionaryItemId, opt => opt.MapFrom(a => a.DictionaryItemId));
            CreateMap<Model.AssetOp, Dto.AssetOpSync>()
                .ForMember(a => a.Id, opt => opt.MapFrom(a => a.Id))
                .ForMember(a => a.AssetId, opt => opt.MapFrom(a => a.AssetId))
                //.ForMember(a => a.AdministrationId, opt => opt.MapFrom(a => a.AdministrationIdFinal))
                .ForMember(a => a.RoomId, opt => opt.MapFrom(a => a.RoomIdFinal))
                .ForMember(a => a.InvStateId, opt => opt.MapFrom(a => a.InvStateIdFinal))
                .ForMember(a => a.IsDeleted, opt => opt.MapFrom(a => a.IsDeleted))
                .ForMember(a => a.ModifiedAt, opt => opt.MapFrom(a => a.ModifiedAt))
                .ForMember(a => a.SerialNumber, opt => opt.MapFrom(a => a.SerialNumber))
                .ForMember(a => a.Info, opt => opt.MapFrom(a => a.Info))
                .ForMember(a => a.AllowLabel, opt => opt.MapFrom(a => a.AllowLabel))
                .ForMember(a => a.UserId, opt => opt.MapFrom(a => a.SrcConfBy))
                //.ForMember(a => a.DimensionId, opt => opt.MapFrom(a => a.DimensionIdFinal))
                .ForMember(a => a.EmployeeId, opt => opt.MapFrom(a => a.EmployeeIdFinal))
                //.ForMember(a => a.UomId, opt => opt.MapFrom(a => a.UomIdFinal))
                //.ForMember(a => a.IsMinus, opt => opt.MapFrom(a => a.IsMinus))
                //.ForMember(a => a.InfoMinus, opt => opt.MapFrom(a => a.InfoMinus))
                .ForMember(a => a.Quantity, opt => opt.MapFrom(a => a.Quantity))
                //.ForMember(a => a.Info2, opt => opt.MapFrom(a => a.Info2019))
                //.ForMember(a => a.AssetTypeId, opt => opt.MapFrom(a => a.AssetTypeIdFinal))
                //.ForMember(a => a.AssetCategoryId, opt => opt.MapFrom(a => a.AssetCategoryIdFinal))
                //.ForMember(a => a.CompanyId, opt => opt.MapFrom(a => a.CompanyId))
                //.ForMember(a => a.AssetNatureId, opt => opt.MapFrom(a => a.AssetNatureIdFinal))
                //.ForMember(a => a.InterCompanyId, opt => opt.MapFrom(a => a.InterCompanyId))
                //.ForMember(a => a.InsuranceCategoryId, opt => opt.MapFrom(a => a.InsuranceCategoryId))
                .ForMember(a => a.CostCenterId, opt => opt.MapFrom(a => a.CostCenterIdFinal))
                .ForMember(a => a.InvName, opt => opt.MapFrom(a => a.InvName));

            CreateMap<Model.Dashboard, Dto.DashboardReport>()
                     .ForMember(a => a.InvNo, opt => opt.MapFrom(a => a.InventoryAsset.Asset.InvNo))
                     .ForMember(a => a.SerialNumber, opt => opt.MapFrom(a => a.InventoryAsset.Asset.SerialNumber))
                     .ForMember(a => a.PurchaseDate, opt => opt.MapFrom(a => a.InventoryAsset.Asset.PurchaseDate))

                     .ForMember(i => i.DepartmentInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Code = i.InventoryAsset.CostCenterInitial.Division.Department.Code, Name = i.InventoryAsset.CostCenterInitial.Division.Department.Name }))
                     .ForMember(i => i.DepartmentFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Code = i.InventoryAsset.CostCenterFinal.Division.Department.Code, Name = i.InventoryAsset.CostCenterFinal.Division.Department.Name }))
                     .ForMember(i => i.DivisionInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Code = i.InventoryAsset.CostCenterInitial.Division.Code, Name = i.InventoryAsset.CostCenterInitial.Division.Name }))
                     .ForMember(i => i.DivisionFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Code = i.InventoryAsset.CostCenterFinal.Division.Code, Name = i.InventoryAsset.CostCenterFinal.Division.Name }))
                     .ForMember(i => i.CostCenterInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Code = i.InventoryAsset.CostCenterInitial.Code, Name = i.InventoryAsset.CostCenterInitial.Name }))
					 .ForMember(i => i.CostCenterInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Code = i.InventoryAsset.CostCenterFinal.Code, Name = i.InventoryAsset.CostCenterFinal.Name }))

					 .ForMember(a => a.Name, opt => opt.MapFrom(a => a.InventoryAsset.Asset.Name));

            CreateMap<Model.Dashboard, Dto.AssetStatusDashboard>()
                    .ForMember(a => a.InvNo, opt => opt.MapFrom(a => a.InventoryAsset.Asset.InvNo))
                    .ForMember(a => a.SerialNumber, opt => opt.MapFrom(a => a.InventoryAsset.Asset.SerialNumber))
                    .ForMember(a => a.PurchaseDate, opt => opt.MapFrom(a => a.InventoryAsset.Asset.PurchaseDate))
                    .ForMember(a => a.ModifiedAt, opt => opt.MapFrom(a => a.InventoryAsset.ModifiedAt))
                    .ForMember(a => a.isDuplicate, opt => opt.MapFrom(a => a.InventoryAsset.Asset.IsDuplicate))
                    .ForMember(a => a.isPrinted, opt => opt.MapFrom(a => a.InventoryAsset.Asset.IsPrinted))

                    .ForMember(i => i.DepartmentInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Code = i.InventoryAsset.CostCenterInitial.Division.Department.Code, Name = i.InventoryAsset.CostCenterInitial.Division.Department.Name }))
                    //.ForMember(i => i.DepartmentFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Code = i.InventoryAsset.CostCenterFinal.Division.Department.Code, Name = i.InventoryAsset.CostCenterFinal.Division.Department.Name }))
                    //.ForMember(i => i.DivisionInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Code = i.InventoryAsset.CostCenterInitial.Division.Code, Name = i.InventoryAsset.CostCenterInitial.Division.Name }))
                    //.ForMember(i => i.DivisionFinal, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Code = i.InventoryAsset.CostCenterFinal.Division.Code, Name = i.InventoryAsset.CostCenterFinal.Division.Name }))
                    //.ForMember(i => i.CostCenterInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Code = i.InventoryAsset.CostCenterInitial.Code, Name = i.InventoryAsset.CostCenterInitial.Name }))
                    //.ForMember(i => i.CostCenterInitial, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Code = i.InventoryAsset.CostCenterFinal.Code, Name = i.InventoryAsset.CostCenterFinal.Name }))

                    .ForMember(a => a.Name, opt => opt.MapFrom(a => a.InventoryAsset.Asset.Name));

			CreateMap<Model.PrintLabel, Dto.PrintLabel>()
				  .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				  .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
				  .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
				  .ForMember(i => i.UploadDate, opt => opt.MapFrom(i => i.UploadDate))
				  .ForMember(i => i.PrintDate, opt => opt.MapFrom(i => i.PrintDate))
				  .ForMember(i => i.Hidden, opt => opt.MapFrom(i => i.Hidden))
				  .ForMember(i => i.Company, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Company.Id, Code = i.Company.Code, Name = i.Company.Name }))
				  .ForMember(i => i.Asset, opt => opt.MapFrom(i => new Dto.Asset { Id = i.Asset.Id, InvNo = i.Asset.InvNo, SubNo = i.Asset.SubNo, Name = i.Asset.Name, SerialNumber = i.Asset.SerialNumber, PurchaseDate = i.Asset.PurchaseDate, SAPCode = i.Asset.SAPCode }));
			CreateMap<Dto.PrintLabel, Model.PrintLabel>()
                 .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                  .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                  .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name));

			CreateMap<Model.DeviceType, Dto.DeviceType>();
            CreateMap<Dto.DeviceType, Model.DeviceType>()
                 .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                  .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                  .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name));
			CreateMap<Model.MobilePhone, Dto.MobilePhone>();
			CreateMap<Dto.MobilePhone, Model.MobilePhone>()
				 .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
				  .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
				  .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name));
			CreateMap<Model.Device, Dto.Device>()
                    .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                    .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                    .ForMember(i => i.Model, opt => opt.MapFrom(i => i.Model))
                    .ForMember(i => i.Producer, opt => opt.MapFrom(i => i.Producer))
                    .ForMember(i => i.Platform, opt => opt.MapFrom(i => i.Platform))
                    .ForMember(i => i.Version, opt => opt.MapFrom(i => i.Version))
                    .ForMember(i => i.UUI, opt => opt.MapFrom(i => i.UUI))
                    .ForMember(i => i.Serial, opt => opt.MapFrom(i => i.Serial))
                    .ForMember(i => i.Employee, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Employee.Id, LastName = i.Employee.LastName, FirstName = i.Employee.FirstName }))
                     .ForMember(i => i.DeviceType, opt => opt.MapFrom(i => new Dto.DeviceType { Id = i.DeviceType.Id, Code = i.DeviceType.Code, Name = i.DeviceType.Name, IsDeleted = i.DeviceType.IsDeleted }));
            CreateMap<Dto.Device, Model.Device>()
                    .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                    .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                    .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                    .ForMember(i => i.Model, opt => opt.MapFrom(i => i.Model))
                    .ForMember(i => i.Producer, opt => opt.MapFrom(i => i.Producer))
                    .ForMember(i => i.Platform, opt => opt.MapFrom(i => i.Platform))
                    .ForMember(i => i.Version, opt => opt.MapFrom(i => i.Version))
                    .ForMember(i => i.UUI, opt => opt.MapFrom(i => i.UUI))
                    .ForMember(i => i.Serial, opt => opt.MapFrom(i => i.Serial))
                    .ForMember(i => i.DeviceType, opt => opt.Ignore())
                    .ForMember(i => i.DeviceTypeId, opt => opt.MapFrom(i => i.DeviceType.Id))
                    .ForMember(i => i.Employee, opt => opt.Ignore());

			CreateMap<Model.WFHCheck, Dto.WFHCheck>()
					.ForMember(i => i.Imei, opt => opt.MapFrom(i => i.Imei))
					.ForMember(i => i.SerialNumber, opt => opt.MapFrom(i => i.SerialNumber))
					.ForMember(i => i.InventoryNumber, opt => opt.MapFrom(i => i.InventoryNumber))
					.ForMember(i => i.Employee, opt => opt.MapFrom(i => new Dto.EmployeeResource { Id = i.Employee.Id, LastName = i.Employee.LastName, FirstName = i.Employee.FirstName }))
					.ForMember(i => i.DictionaryItem, opt => opt.MapFrom(i => new Dto.DictionaryItem { Id = i.DictionaryItem.Id, Code = i.DictionaryItem.Code, Name = i.DictionaryItem.Name }))
            		.ForMember(i => i.Brand, opt => opt.MapFrom(i => new Dto.Brand { Id = i.Brand.Id, Code = i.Brand.Code, Name = i.Brand.Name }))
            		.ForMember(i => i.Model, opt => opt.MapFrom(i => new Dto.Model { Id = i.Model.Id, Code = i.Model.Code, Name = i.Model.Name }))
            	    .ForMember(i => i.BudgetManager, opt => opt.MapFrom(i => new Dto.BudgetManager { Id = i.BudgetManager.Id, Code = i.BudgetManager.Code, Name = i.BudgetManager.Name }));
			CreateMap<Dto.WFHCheck, Model.WFHCheck>()
					.ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
					.ForMember(i => i.Imei, opt => opt.MapFrom(i => i.Imei))
					.ForMember(i => i.SerialNumber, opt => opt.MapFrom(i => i.SerialNumber))
					.ForMember(i => i.InventoryNumber, opt => opt.MapFrom(i => i.InventoryNumber))
					.ForMember(i => i.DictionaryItem, opt => opt.Ignore())
					.ForMember(i => i.DictionaryItemId, opt => opt.MapFrom(i => i.DictionaryItem.Id))
					.ForMember(i => i.Brand, opt => opt.Ignore())
					.ForMember(i => i.BrandId, opt => opt.MapFrom(i => i.Brand.Id))
					.ForMember(i => i.Model, opt => opt.Ignore())
					.ForMember(i => i.ModelId, opt => opt.MapFrom(i => i.Model.Id))
					.ForMember(i => i.BudgetManager, opt => opt.Ignore())
					.ForMember(i => i.BudgetManagerId, opt => opt.MapFrom(i => i.BudgetManager.Id))
					.ForMember(i => i.Employee, opt => opt.Ignore());

			CreateMap<Model.ErrorType, Dto.ErrorType>();
            CreateMap<Dto.ErrorType, Model.ErrorType>()
                 .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                  .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                  .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name));
            CreateMap<Model.Error, Dto.Error>()
             .ForMember(i => i.Asset, opt => opt.MapFrom(i => new Dto.AssetEntity { Id = i.Asset.Id, InvNo = i.Asset.InvNo, Name = i.Asset.Name }));

			CreateMap<Dto.Error, Model.Error>()
                    .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                    .ForMember(i => i.Code, opt => opt.MapFrom(i => i.Code))
                    .ForMember(i => i.Name, opt => opt.MapFrom(i => i.Name))
                    .ForMember(i => i.ErrorType, opt => opt.Ignore())
                    .ForMember(i => i.ErrorTypeId, opt => opt.MapFrom(i => i.ErrorType.Id));

            CreateMap<Model.AssetDepMDSync, Dto.AssetDepMDSync>()
                     .ForMember(i => i.AccMonth, opt => opt.MapFrom(i => new Dto.AccMonth { Month = i.AccMonth.Month, Year = i.AccMonth.Year }))
                     .ForMember(i => i.BudgetManager, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.BudgetManager.Id, Code = i.BudgetManager.Code, Name = i.BudgetManager.Name }))
                     .ForMember(i => i.Company, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.Company.Id, Code = i.Company.Code, Name = i.Company.Name }));
            CreateMap<Dto.AssetDepMDSync, Model.AssetDepMDSync>();

            CreateMap<Model.AssetDepMDCapSync, Dto.AssetDepMDCapDTOSync>()
                    .ForMember(i => i.AccMonth, opt => opt.MapFrom(i => new Dto.AccMonth { Month = i.AccMonth.Month, Year = i.AccMonth.Year }))
                    .ForMember(i => i.BudgetManager, opt => opt.MapFrom(i => new Dto.CodeNameEntity { Id = i.BudgetManager.Id, Code = i.BudgetManager.Code, Name = i.BudgetManager.Name }));
            CreateMap<Dto.AssetDepMDCapDTOSync, Model.AssetDepMDCapSync>();

            CreateMap<Model.TransferInStockSAP, Dto.TransferInStockSAP>();
            CreateMap<Dto.TransferInStockSAP, Model.TransferInStockSAP>();

            CreateMap<Model.TransferAssetSAP, Dto.TransferAssetSAP>();
            CreateMap<Dto.TransferAssetSAP, Model.TransferAssetSAP>();

            CreateMap<Model.RetireAssetSAP, Dto.RetireAssetSAP>();
            CreateMap<Dto.RetireAssetSAP, Model.RetireAssetSAP>();

            CreateMap<Model.CreateAssetSAP, Dto.CreateAssetSAPDTO>()
                 .ForMember(i => i.ASSET, opt => opt.MapFrom(i => i.ASSET))
                 .ForMember(i => i.NotSync, opt => opt.MapFrom(i => i.NotSync))
                 .ForMember(i => i.SyncErrorCount, opt => opt.MapFrom(i => i.SyncErrorCount))
                 .ForMember(i => i.Error, opt => opt.MapFrom(i => new Dto.Error { Code = i.Error.Code, Name = i.Error.Name, BELNR = i.Error.BELNR, BUKRS = i.Error.BUKRS, GJAHR = i.Error.GJAHR, Request = i.Error.Request }))
                 .ForMember(i => i.AssetEntity, opt => opt.MapFrom(i => new Dto.Asset { InvNo = i.Asset.InvNo, SubNo = i.Asset.SubNo, ERPCode = i.Asset.ERPCode }));
            CreateMap<Dto.CreateAssetSAPDTO, Model.CreateAssetSAP>();

            CreateMap<Model.AssetChangeSAP, Dto.AssetChangeSAPDTO>()
                 .ForMember(i => i.ASSET, opt => opt.MapFrom(i => i.ASSET))
                 .ForMember(i => i.NotSync, opt => opt.MapFrom(i => i.NotSync))
                 .ForMember(i => i.SyncErrorCount, opt => opt.MapFrom(i => i.SyncErrorCount))
                 .ForMember(i => i.Error, opt => opt.MapFrom(i => new Dto.Error { Code = i.Error.Code, Name = i.Error.Name, BELNR = i.Error.BELNR, BUKRS = i.Error.BUKRS, GJAHR = i.Error.GJAHR, Request = i.Error.Request }))
                 .ForMember(i => i.AssetEntity, opt => opt.MapFrom(i => new Dto.Asset { InvNo = i.Asset.InvNo, SubNo = i.Asset.SubNo, ERPCode = i.Asset.ERPCode }));
            CreateMap<Dto.AssetChangeSAPDTO, Model.AssetChangeSAP>();

            CreateMap<Model.RetireAssetSAP, Dto.RetireAssetSAPDTO>()
                 .ForMember(i => i.ASSET, opt => opt.MapFrom(i => i.ASSET))
                 .ForMember(i => i.NotSync, opt => opt.MapFrom(i => i.NotSync))
                 .ForMember(i => i.SyncErrorCount, opt => opt.MapFrom(i => i.SyncErrorCount))
                 .ForMember(i => i.Error, opt => opt.MapFrom(i => new Dto.Error { Code = i.Error.Code, Name = i.Error.Name, BELNR = i.Error.BELNR, BUKRS = i.Error.BUKRS, GJAHR = i.Error.GJAHR, Request = i.Error.Request }))
                 .ForMember(i => i.AssetEntity, opt => opt.MapFrom(i => new Dto.Asset { InvNo = i.Asset.InvNo, SubNo = i.Asset.SubNo, ERPCode = i.Asset.ERPCode }));
            CreateMap<Dto.RetireAssetSAPDTO, Model.RetireAssetSAP>();

            CreateMap<Model.TransferInStockSAP, Dto.TransferInStockSAPDTO>()
                 .ForMember(i => i.NotSync, opt => opt.MapFrom(i => i.NotSync))
                 .ForMember(i => i.SyncErrorCount, opt => opt.MapFrom(i => i.SyncErrorCount))
                 .ForMember(i => i.Error, opt => opt.MapFrom(i => new Dto.Error { Code = i.Error.Code, Name = i.Error.Name, BELNR = i.Error.BELNR, BUKRS = i.Error.BUKRS, GJAHR = i.Error.GJAHR, Request = i.Error.Request }))
                 .ForMember(i => i.AssetStock, opt => opt.MapFrom(i => new Dto.Asset { InvNo = i.AssetStock.InvNo, SubNo = i.AssetStock.SubNo, ERPCode = i.AssetStock.ERPCode }));
            CreateMap<Dto.TransferInStockSAPDTO, Model.TransferInStockSAP>();

            CreateMap<Model.AssetInvPlusSAP, Dto.AssetInvPlusSAP>();
            CreateMap<Dto.AssetInvPlusSAP, Model.AssetInvPlusSAP>();

            CreateMap<Model.AssetInvMinusSAP, Dto.AssetInvMinusSAP>();
            CreateMap<Dto.AssetInvMinusSAP, Model.AssetInvMinusSAP>();

            CreateMap<Model.AssetChangeSAP, Dto.AssetChangeSAP>();
            CreateMap<Dto.AssetChangeSAP, Model.AssetChangeSAP>();

            CreateMap<Model.AcquisitionAssetSAP, Dto.AcquisitionAssetSAP>();
            CreateMap<Dto.AcquisitionAssetSAP, Model.AcquisitionAssetSAP>();

            CreateMap<Model.AcquisitionAssetSAP, Dto.AcquisitionAssetSAPView>();

            CreateMap<Dto.InvCommittee, Model.InvCommittee>();
            CreateMap<Model.InvCommittee, Dto.InvCommittee>();

            CreateMap<Dto.InvCommitteePostition, Model.InvCommitteePosition>();
            CreateMap<Model.InvCommitteePosition, Dto.InvCommitteePostition>();

            CreateMap<InvCommitteeMember, Dto.InvCommitteeMember>()
             .ForMember(a => a.Employee, opt => opt.MapFrom(a => new Dto.Employee { Id = a.Employee.Id, Email = a.Employee.Email, FirstName = a.Employee.FirstName, LastName = a.Employee.LastName }))
             .ForMember(i => i.InvCommitteePosition, opt => opt.MapFrom(i => new Dto.InvCommitteePostition { Id = i.InvCommitteePosition.Id, Code = i.InvCommitteePosition.Code, Name = i.InvCommitteePosition.Name }))
             .ForMember(i => i.InvCommittee, opt => opt.MapFrom(i => new Dto.InvCommittee { Id = i.InvCommittee.Id, Code = i.InvCommittee.Code, Name = i.InvCommittee.Name }));

            CreateMap<Dto.InvCommitteeMember, InvCommitteeMember>()
                .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
                 .ForMember(i => i.Employee, opt => opt.Ignore())
                .ForMember(i => i.InvCommitteePosition, opt => opt.Ignore())
                .ForMember(i => i.InvCommittee, opt => opt.Ignore());

            CreateMap<InventoryPlan, Dto.InventoryPlan>()
             .ForMember(i => i.InvCommittee, opt => opt.MapFrom(i => new Dto.InvCommittee { Id = i.InvCommittee.Id, Code = i.InvCommittee.Code, Name = i.InvCommittee.Name }))
             .ForMember(i => i.Administration, opt => opt.MapFrom(i => new Dto.Administration { Id = i.Administration.Id, Code = i.Administration.Code, Name = i.Administration.Name }))
             .ForMember(i => i.CostCenter, opt => opt.MapFrom(i => new Dto.CostCenter { Id = i.CostCenter.Id, Code = i.CostCenter.Code, Name = i.CostCenter.Name }));

            CreateMap<Dto.InventoryPlan, InventoryPlan>()
             .ForMember(i => i.Id, opt => opt.MapFrom(i => i.Id))
             .ForMember(i => i.InvCommittee, opt => opt.Ignore())
             .ForMember(i => i.Administration, opt => opt.Ignore())
             .ForMember(i => i.CostCenter, opt => opt.Ignore());

            //CreateMap<Model.InventoryAsset, Dto.InventoryAssetSync>()
            //	.ForMember(a => a.Id, opt => opt.MapFrom(a => a.Id))
            //	.ForMember(a => a.InventoryId, opt => opt.MapFrom(a => a.InventoryId))
            //	.ForMember(a => a.AssetId, opt => opt.MapFrom(a => a.AssetId))
            //	.ForMember(a => a.QFinal, opt => opt.MapFrom(a => a.QFinal))
            //	.ForMember(a => a.QInitial, opt => opt.MapFrom(a => a.QInitial))
            //	.ForMember(a => a.RoomIdFinal, opt => opt.MapFrom(a => a.RoomIdFinal))
            //	.ForMember(a => a.RoomIdInitial, opt => opt.MapFrom(a => a.RoomIdInitial))
            //	.ForMember(a => a.SerialNumber, opt => opt.MapFrom(a => a.SerialNumber))
            //	.ForMember(a => a.StateIdFinal, opt => opt.MapFrom(a => a.StateIdFinal))
            //	.ForMember(a => a.StateIdInitial, opt => opt.MapFrom(a => a.StateIdInitial))
            //	.ForMember(a => a.Info, opt => opt.MapFrom(a => a.Info))
            //	.ForMember(a => a.UpdatedAt, opt => opt.MapFrom(a => a.UpdatedAt))
            //	.ForMember(a => a.ModifiedBy, opt => opt.MapFrom(a => a.ModifiedBy))
            //	.ForMember(a => a.CostCenterIdInitial, opt => opt.MapFrom(a => a.CostCenterIdInitial))
            //	.ForMember(a => a.CostCenterIdFinal, opt => opt.MapFrom(a => a.CostCenterIdFinal))
            //	.ForMember(a => a.ImageCount, opt => opt.MapFrom(a => a.ImageCount))
            //	.ForMember(a => a.InfoMinus, opt => opt.MapFrom(a => a.InfoMinus))
            //	.ForMember(a => a.IsMinus, opt => opt.MapFrom(a => a.IsMinus))
            //	.ForMember(a => a.IsTemp, opt => opt.MapFrom(a => a.IsTemp))
            //	.ForMember(a => a.AssetRecoStateId, opt => opt.MapFrom(a => a.AssetRecoStateId))
            //	//.ForMember(a => a.DimensionIdInitial, opt => opt.MapFrom(a => a.DimensionIdInitial))
            //	//.ForMember(a => a.UomIdInitial, opt => opt.MapFrom(a => a.UomIdInitial))
            //	.ForMember(a => a.TempName, opt => opt.MapFrom(a => a.TempName))
            //	.ForMember(a => a.TempReco, opt => opt.MapFrom(a => a.TempReco))
            //	//.ForMember(a => a.NeedLabel, opt => opt.MapFrom(a => a.NeedLabel))
            //	//.ForMember(a => a.AssetTypeId, opt => opt.MapFrom(a => a.AssetTypeId))
            //	.ForMember(a => a.InInventory, opt => opt.MapFrom(a => a.InInventory))
            //	.ForMember(a => a.IsDeleted, opt => opt.MapFrom(a => a.IsDeleted))
            //	.ForMember(a => a.ScanDate, opt => opt.MapFrom(a => a.ScanDate));
        }
    }
}
