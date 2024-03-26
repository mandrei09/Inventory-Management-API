using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Optima.Fais.EfRepository
{
    public class MatrixRepository : Repository<Matrix>, IMatrixRepository
    {
        public MatrixRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (c) => (
                c.Company.Name.Contains(filter) ||
				c.Division.Department.Code.Contains(filter) ||
				c.Division.Department.Name.Contains(filter) ||
				c.Division.Code.Contains(filter) ||
				c.Division.Name.Contains(filter) ||
				c.EmployeeB1.Email.Contains(filter) ||
				c.EmployeeL1.Email.Contains(filter) ||
				c.EmployeeL2.Email.Contains(filter) ||
				c.EmployeeL3.Email.Contains(filter) ||
				c.EmployeeL4.Email.Contains(filter) ||
				c.EmployeeS3.Email.Contains(filter) ||
				c.EmployeeS2.Email.Contains(filter) ||
				c.EmployeeS1.Email.Contains(filter)); })
        { }

        private Expression<Func<Model.Matrix, bool>> GetFiltersPredicate(string filter, List<int?> assetTypeIds, List<int?> projectTypeIds, List<int?> areaIds, List<int?> countryIds, List<int?> companyIds, List<int?> divisionIds, List<int?> costCenterIds, List<int?> projectIds,
			List<int?> employeeL1Ids, List<int?> employeeL2Ids, List<int?> employeeL3Ids, List<int?> employeeL4Ids, List<int?> employeeS1Ids, List<int?> employeeS2Ids, List<int?> employeeS3Ids, List<int?> exceptMatrixIds)
        {
            Expression<Func<Model.Matrix, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((companyIds != null) && (companyIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Matrix>(predicate, r => companyIds.Contains(r.CompanyId))
                    : r => companyIds.Contains(r.CompanyId);
            }

			if ((divisionIds != null) && (divisionIds.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.Matrix>(predicate, r => divisionIds.Contains(r.DivisionId))
					: r => divisionIds.Contains(r.DivisionId);
			}

			if ((employeeL1Ids != null) && (employeeL1Ids.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.Matrix>(predicate, r => employeeL1Ids.Contains(r.EmployeeL1Id))
					: r => employeeL1Ids.Contains(r.EmployeeL1Id);
			}

			if ((employeeL2Ids != null) && (employeeL2Ids.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.Matrix>(predicate, r => employeeL2Ids.Contains(r.EmployeeL2Id))
					: r => employeeL2Ids.Contains(r.EmployeeL2Id);
			}

			if ((employeeL3Ids != null) && (employeeL3Ids.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.Matrix>(predicate, r => employeeL3Ids.Contains(r.EmployeeL3Id))
					: r => employeeL3Ids.Contains(r.EmployeeL3Id);
			}

			if ((employeeL4Ids != null) && (employeeL4Ids.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.Matrix>(predicate, r => employeeL4Ids.Contains(r.EmployeeL4Id))
					: r => employeeL4Ids.Contains(r.EmployeeL4Id);
			}

			if ((employeeS1Ids != null) && (employeeS1Ids.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.Matrix>(predicate, r => employeeS1Ids.Contains(r.EmployeeS1Id))
					: r => employeeS1Ids.Contains(r.EmployeeS1Id);
			}

			if ((employeeS2Ids != null) && (employeeS2Ids.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.Matrix>(predicate, r => employeeS2Ids.Contains(r.EmployeeS2Id))
					: r => employeeS2Ids.Contains(r.EmployeeS2Id);
			}

			if ((employeeS3Ids != null) && (employeeS3Ids.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.Matrix>(predicate, r => employeeS3Ids.Contains(r.EmployeeS3Id))
					: r => employeeS3Ids.Contains(r.EmployeeS3Id);
			}

			if ((exceptMatrixIds != null) && (exceptMatrixIds.Count > 0))
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Matrix>(predicate, r => !exceptMatrixIds.Contains(r.Id))
                    : r => !exceptMatrixIds.Contains(r.Id);
            }

            return predicate;
        }

        public IEnumerable<Model.Matrix> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> assetTypeIds, List<int?> projectTypeIds, List<int?> areaIds, List<int?> countryIds, List<int?> companyIds, List<int?> divisionIds, List<int?> costCenterIds, List<int?> projectIds,
			List<int?> employeeL1Ids, List<int?> employeeL2Ids, List<int?> employeeL3Ids, List<int?> employeeL4Ids, List<int?> employeeS1Ids, List<int?> employeeS2Ids, List<int?> employeeS3Ids, List<int?> exceptMatrixIds)
        {
            var predicate = GetFiltersPredicate(filter, assetTypeIds, projectTypeIds, areaIds, countryIds, companyIds, divisionIds, costCenterIds, projectIds, employeeL1Ids, employeeL2Ids, employeeL3Ids, employeeL4Ids, employeeS1Ids, employeeS2Ids, employeeS3Ids, exceptMatrixIds);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter, List<int?> assetTypeIds, List<int?> projectTypeIds, List<int?> areaIds, List<int?> countryIds, List<int?> companyIds, List<int?> divisionIds, List<int?> costCenterIds, List<int?> projectIds,
			List<int?> employeeL1Ids, List<int?> employeeL2Ids, List<int?> employeeL3Ids, List<int?> employeeL4Ids, List<int?> employeeS1Ids, List<int?> employeeS2Ids, List<int?> employeeS3Ids, List<int?> exceptMatrixIds)
        {
            var predicate = GetFiltersPredicate(filter, assetTypeIds, projectTypeIds, areaIds, countryIds, companyIds, divisionIds, costCenterIds, projectIds, employeeL1Ids, employeeL2Ids, employeeL3Ids, employeeL4Ids, employeeS1Ids, employeeS2Ids, employeeS3Ids, exceptMatrixIds);

            return GetQueryable(predicate).Count();
        }

        public async Task<int> MatrixImport(Dto.MatrixImport budgetDto)
        {
            Model.Matrix matrix = null;
			//Model.MatrixLevel matrixLevelL1 = null;
			//Model.MatrixLevel matrixLevelL2 = null;
			//Model.MatrixLevel matrixLevelL3 = null;
			//Model.MatrixLevel matrixLevelL4 = null;
			//Model.MatrixLevel matrixLevelS1 = null;
			//Model.MatrixLevel matrixLevelS2 = null;
			//Model.MatrixLevel matrixLevelS3 = null;
			//Model.Company company = null;
            //Model.CostCenter costCenter = null;
            //Model.Country country = null;
            //Model.Project project = null;
            //Model.AdmCenter admCenter = null;
            //Model.Area area = null;
            List<Model.ProjectType> projectTypes = null;
			//Model.ProjectType projectType = null;
			//Model.AssetType assetType = null;
			Model.AppState appState = null;
            Model.Department department = null;
            Model.Division division = null;
			//Model.Uom uom = null;

			Model.Employee employeeL1 = null;
			Model.Employee employeeL2 = null;
			Model.Employee employeeL3 = null;
			Model.Employee employeeL4 = null;
			Model.Employee employeeS1 = null;
			Model.Employee employeeS2 = null;
			Model.Employee employeeS3 = null;

			//Model.Level levelL1 = null;
			//Model.Level levelL2 = null;
			//Model.Level levelL3 = null;
			//Model.Level levelL4 = null;
			//Model.Level levelS1 = null;
			//Model.Level levelS2 = null;
			//Model.Level levelS3 = null;

			_context.UserId = budgetDto.UserId;

			// uom = await _context.Set<Model.Uom>().AsNoTracking().Where(c => c.Code == "RON").SingleOrDefaultAsync();

			appState = await _context.Set<Model.AppState>().AsNoTracking().Where(c => c.Id == 12).SingleOrDefaultAsync();

			projectTypes = await _context.Set<Model.ProjectType>().Where(p => p.IsDeleted == false).ToListAsync();

			//company = await _context.Set<Model.Company>().Where(c => c.Code == budgetDto.CompanyCode).SingleOrDefaultAsync();

			//if (company == null)
			//{
			//	company = new Model.Company
			//	{
			//		Code = budgetDto.CompanyCode.Trim(),
			//		Name = budgetDto.CompanyName.Trim(),
			//		IsDeleted = false
			//	};
			//	await _context.Set<Model.Company>().AddAsync(company);
			//}
			//else
			//{
			//	company.IsDeleted = false;
			//	_context.Set<Model.Company>().Update(company);
			//}

			//area = await _context.Set<Model.Area>().Where(c => c.Code == budgetDto.Area).SingleOrDefaultAsync();

			//if (area == null)
			//{
			//	area = new Model.Area
			//	{
			//		Code = budgetDto.Area.Trim(),
			//		Name = budgetDto.Area.Trim(),
			//		Company = company,
			//		IsDeleted = false
			//	};
			//	await _context.Set<Model.Area>().AddAsync(area);
			//}
			//else
			//{
			//	area.IsDeleted = false;
			//	area.Company = company;
			//	_context.Set<Model.Area>().Update(area);
			//}

			//country = await _context.Set<Model.Country>().Where(c => c.Code == budgetDto.CountryCode).SingleOrDefaultAsync();

			//if (country == null)
			//{
			//	country = new Model.Country
			//	{
			//		Code = budgetDto.CountryCode.Trim(),
			//		Name = budgetDto.CountryName.Trim(),
			//		Company = company,
			//		IsDeleted = false
			//	};
			//	await _context.Set<Model.Country>().AddAsync(country);
			//}
			//else
			//{
			//	country.IsDeleted = false;
			//	country.Company = company;
			//	_context.Set<Model.Country>().Update(country);
			//}

			department = await _context.Set<Model.Department>().Where(c => c.Code == budgetDto.DepartmentCode).FirstOrDefaultAsync();

			if (department == null)
			{
				department = new Model.Department
				{
					Code = budgetDto.DepartmentCode.Trim(),
					Name = budgetDto.DepartmentName.Trim(),
					//Company = company,
					IsDeleted = false
				};
				await _context.Set<Model.Department>().AddAsync(department);
			}
			else
			{
				department.IsDeleted = false;
				department.Name = budgetDto.DepartmentName;
				//department.Company = company;
				_context.Set<Model.Department>().Update(department);
			}

			division = await _context.Set<Model.Division>().Where(c => c.Code == budgetDto.DivisionCode && c.DepartmentId == department.Id).FirstOrDefaultAsync();

			if (division == null)
			{
				division = new Model.Division
				{
					Code = budgetDto.DivisionCode,
					Name = budgetDto.DivisionName,
					IsDeleted = false,
					Department = department,
				};
				await _context.Set<Model.Division>().AddAsync(division);
			}
			else
			{
				division.IsDeleted = false;
				division.Name = budgetDto.DivisionName;
				division.Department = department;
				_context.Set<Model.Division>().Update(division);
			}

			//admCenter = await _context.Set<Model.AdmCenter>().Where(c => c.Code == budgetDto.AdmCenter).FirstOrDefaultAsync();

			//if (admCenter == null)
			//{
			//	admCenter = new Model.AdmCenter
			//	{
			//		Code = budgetDto.AdmCenter,
			//		Name = budgetDto.AdmCenter,
			//		Company = company,
			//		IsDeleted = false
			//	};
			//	await _context.Set<Model.AdmCenter>().AddAsync(admCenter);
			//}
			//else
			//{
			//	admCenter.IsDeleted = false;
			//	admCenter.Company = company;
			//	_context.Set<Model.AdmCenter>().Update(admCenter);
			//}


			//assetType = await _context.Set<Model.AssetType>().Where(c => c.Code == budgetDto.AssetTypeCode).FirstOrDefaultAsync();

			//if (assetType == null)
			//{
			//	assetType = new Model.AssetType
			//	{
			//		Code = budgetDto.AssetTypeCode.Trim(),
			//		Name = budgetDto.AssetTypeName.Trim(),
			//		Company = company,
			//		IsDeleted = false
			//	};
			//	await _context.Set<Model.AssetType>().AddAsync(assetType);
			//}
			//else
			//{
			//	assetType.IsDeleted = false;
			//	assetType.Name = budgetDto.AssetTypeName;
			//	assetType.Company = company;
			//	_context.Set<Model.AssetType>().Update(assetType);
			//}

			//costCenter = await _context.Set<Model.CostCenter>().Where(c => c.Code == budgetDto.CostCenterCode).FirstOrDefaultAsync();

			//if (costCenter == null)
			//{
			//	costCenter = new Model.CostCenter
			//	{
			//		Code = budgetDto.CostCenterCode.Trim(),
			//		Name = budgetDto.CostCenterName.Trim(),
			//		Division = division,
			//		AdmCenter = admCenter,
			//		Company = company,
			//		IsDeleted = false
			//	};
			//	await _context.Set<Model.CostCenter>().AddAsync(costCenter);
			//}
			//else
			//{
			//	costCenter.IsDeleted = false;
			//	costCenter.Name = budgetDto.CostCenterName;
			//	costCenter.Division = division;
			//	costCenter.AdmCenter = admCenter;
			//	costCenter.Company = company;
			//	_context.Set<Model.CostCenter>().Update(costCenter);
			//}


			for (int p = 0; p < projectTypes.Count; p++)
			{
				matrix = null;
				//projectType = await _context.Set<Model.ProjectType>().Where(a => a.Id == projectTypes[p].Id).SingleAsync();

				//if(budgetDto.Project == "" || budgetDto.Project == null)
				//{
				//	string projectCode = budgetDto.CountryCode + "_" + budgetDto.DepartmentCode + "_" + budgetDto.DivisionCode + "_" + projectType.Code + "_" + budgetDto.AssetTypeCode;

				//	//project = await _context.Set<Model.Project>().Where(c => c.Code == projectCode).FirstOrDefaultAsync();

				//	//if(project == null)
				//	//{
				//	//	project = new Model.Project
				//	//	{
				//	//		Code = projectCode,
				//	//		Name = projectCode,
				//	//		IsDeleted = false,
				//	//		ProjectType = projectType,
				//	//		Company = company
				//	//	};
				//	//	await _context.Set<Model.Project>().AddAsync(project);
				//	//}
				//	//else
				//	//{
				//	//	project.IsDeleted = false;
				//	//	project.ProjectType = projectType;
				//	//	project.Name = projectCode;
				//	//	project.Company = company;
				//	//	_context.Set<Model.Project>().Update(project);
				//	//}

					
				//}


				if (budgetDto.L1UserId != null)
				{
					employeeL1 = await _context.Set<Model.Employee>().AsNoTracking().Where(c => c.Email == budgetDto.L1UserId).SingleOrDefaultAsync();
				}

				if (budgetDto.L2UserId != null)
				{
					employeeL2 = await _context.Set<Model.Employee>().AsNoTracking().Where(c => c.Email == budgetDto.L2UserId).SingleOrDefaultAsync();
				}

				if (budgetDto.L3UserId != null)
				{
					employeeL3 = await _context.Set<Model.Employee>().AsNoTracking().Where(c => c.Email == budgetDto.L3UserId).SingleOrDefaultAsync();
				}

				if (budgetDto.L4UserId != null)
				{
					employeeL4 = await _context.Set<Model.Employee>().AsNoTracking().Where(c => c.Email == budgetDto.L4UserId).SingleOrDefaultAsync();
				}

				if (budgetDto.S1UserId != null)
				{
					employeeS1 = await _context.Set<Model.Employee>().AsNoTracking().Where(c => c.Email == budgetDto.S1UserId).SingleOrDefaultAsync();
				}

				if (budgetDto.S2UserId != null)
				{
					employeeS2 = await _context.Set<Model.Employee>().AsNoTracking().Where(c => c.Email == budgetDto.S2UserId).SingleOrDefaultAsync();
				}

				if (budgetDto.S3UserId != null)
				{
					employeeS3 = await _context.Set<Model.Employee>().AsNoTracking().Where(c => c.Email == budgetDto.S3UserId).SingleOrDefaultAsync();
				}


				//levelL1 = await _context.Set<Model.Level>().AsNoTracking().Where(c => c.Code == "L1").SingleOrDefaultAsync();
				//levelL2 = await _context.Set<Model.Level>().AsNoTracking().Where(c => c.Code == "L2").SingleOrDefaultAsync();
				//levelL3 = await _context.Set<Model.Level>().AsNoTracking().Where(c => c.Code == "L3").SingleOrDefaultAsync();
				//levelL4 = await _context.Set<Model.Level>().AsNoTracking().Where(c => c.Code == "L4").SingleOrDefaultAsync();
				//levelS1 = await _context.Set<Model.Level>().AsNoTracking().Where(c => c.Code == "S1").SingleOrDefaultAsync();
				//levelS2 = await _context.Set<Model.Level>().AsNoTracking().Where(c => c.Code == "S2").SingleOrDefaultAsync();
				//levelS3 = await _context.Set<Model.Level>().AsNoTracking().Where(c => c.Code == "S3").SingleOrDefaultAsync();



				//matrix = await _context.Set<Model.Matrix>().Where(a => a.ProjectId == project.Id && a.CostCenterId == costCenter.Id && a.AssetTypeId == assetType.Id).SingleOrDefaultAsync();


				if (matrix != null)
				{
					matrix.AppStateId = appState.Id;
					//matrix.AreaId = area.Id;
					//matrix.AssetTypeId = assetType.Id;
					//matrix.CompanyId = company.Id;
					//matrix.CostCenterId = costCenter.Id;
					//matrix.CountryId = country.Id;
					matrix.IsDeleted = false;
					matrix.ModifiedAt = DateTime.Now;
					matrix.ModifiedBy = budgetDto.UserId;

					matrix.EmployeeL1 = employeeL1;
					matrix.EmployeeL2 = employeeL2;
					matrix.EmployeeL3 = employeeL3;
					matrix.EmployeeL4 = employeeL4;
					matrix.EmployeeS1 = employeeS1;
					matrix.EmployeeS2 = employeeS2;
					matrix.EmployeeS3 = employeeS3;

					matrix.AmountL1 = budgetDto.L1UserIdSum;
					matrix.AmountL2 = budgetDto.L2UserIdSum;
					matrix.AmountL3 = budgetDto.L3UserIdSum;
					matrix.AmountL4 = budgetDto.L4UserIdSum;
					matrix.AmountS1 = budgetDto.S1UserIdSum;
					matrix.AmountS2 = budgetDto.S2UserIdSum;
					matrix.AmountS3 = budgetDto.S3UserIdSum;

					_context.Set<Model.Matrix>().Update(matrix);

					// List<MatrixLevel> matrixLevels = await _context.Set<MatrixLevel>().Include(l => l.Level).Where(a => a.MatrixId == matrix.Id).ToListAsync();

					//for (int i = 0; i < matrixLevels.Count; i++)
					//{
					//	Model.MatrixLevel matrixLevel = await _context.Set<Model.MatrixLevel>().Where(l => l.Id == matrixLevels[i].Id).SingleOrDefaultAsync();

					//	if (matrixLevel.Level.Code == "L1")
					//	{
					//		matrixLevel.Amount = budgetDto.L1UserIdSum;

					//		if (employeeL1 != null)
					//		{
					//			matrixLevel.EmployeeId = employeeL1.Id;
					//			matrixLevel.IsDeleted = false;
					//		}
					//		else
					//		{
					//			matrixLevel.IsDeleted = true;
					//		}


					//		_context.Update(matrixLevel);
					//	}


					//	if (matrixLevel.Level.Code == "L2")
					//	{
					//		matrixLevel.Amount = budgetDto.L2UserIdSum;

					//		if (employeeL2 != null)
					//		{
					//			matrixLevel.EmployeeId = employeeL2.Id;
					//			matrixLevel.IsDeleted = false;
					//		}
					//		else
					//		{
					//			matrixLevel.IsDeleted = true;
					//		}


					//		_context.Update(matrixLevel);
					//	}


					//	if (matrixLevel.Level.Code == "L3")
					//	{
					//		matrixLevel.Amount = budgetDto.L3UserIdSum;

					//		if (employeeL3 != null)
					//		{
					//			matrixLevel.EmployeeId = employeeL3.Id;
					//			matrixLevel.IsDeleted = false;
					//		}
					//		else
					//		{
					//			matrixLevel.IsDeleted = true;
					//		}


					//		_context.Update(matrixLevel);
					//	}


					//	if (matrixLevel.Level.Code == "L4")
					//	{
					//		matrixLevel.Amount = budgetDto.L4UserIdSum;

					//		if (employeeL4 != null)
					//		{
					//			matrixLevel.EmployeeId = employeeL4.Id;
					//			matrixLevel.IsDeleted = false;
					//		}
					//		else
					//		{
					//			matrixLevel.IsDeleted = true;
					//		}


					//		_context.Update(matrixLevel);
					//	}

					//	if (matrixLevel.Level.Code == "S1")
					//	{
					//		matrixLevel.Amount = budgetDto.S1UserIdSum;

					//		if (employeeS1 != null)
					//		{
					//			matrixLevel.EmployeeId = employeeS1.Id;
					//			matrixLevel.IsDeleted = false;
					//		}
					//		else
					//		{
					//			matrixLevel.IsDeleted = true;
					//		}


					//		_context.Update(matrixLevel);
					//	}


					//	if (matrixLevel.Level.Code == "S2")
					//	{
					//		matrixLevel.Amount = budgetDto.S2UserIdSum;

					//		if (employeeS2 != null)
					//		{
					//			matrixLevel.EmployeeId = employeeS2.Id;
					//			matrixLevel.IsDeleted = false;
					//		}
					//		else
					//		{
					//			matrixLevel.IsDeleted = true;
					//		}


					//		_context.Update(matrixLevel);
					//	}

					//	if (matrixLevel.Level.Code == "S3")
					//	{
					//		matrixLevel.Amount = budgetDto.S3UserIdSum;

					//		if (employeeS3 != null)
					//		{
					//			matrixLevel.EmployeeId = employeeS3.Id;
					//			matrixLevel.IsDeleted = false;
					//		}
					//		else
					//		{
					//			matrixLevel.IsDeleted = true;
					//		}


					//		_context.Update(matrixLevel);
					//	}

					//	matrixLevel.ModifiedAt = DateTime.Now;

					//	_context.SaveChanges();

					//}
				}
				else
				{

					matrix = new Model.Matrix()
					{
						AppStateId = appState.Id,
						//Area = area,
						//AssetType = assetType,
						Code = string.Empty,
						//Company = company,
						//CostCenter = costCenter,
						//Country = country,
						CreatedAt = DateTime.Now,
						CreatedBy = budgetDto.UserId,
						IsDeleted = false,
						ModifiedAt = DateTime.Now,
						ModifiedBy = budgetDto.UserId,
						//Project = project,
						EmployeeL1Id = employeeL1.Id,
						EmployeeL2Id = employeeL2.Id,
						EmployeeL3Id = employeeL3.Id,
						EmployeeL4Id = employeeL4.Id,
						EmployeeS1Id = employeeS1.Id,
						EmployeeS2Id = employeeS2.Id,
						EmployeeS3Id = employeeS3.Id,
						AmountL1 = budgetDto.L1UserIdSum,
						AmountL2 = budgetDto.L2UserIdSum,
						AmountL3 = budgetDto.L3UserIdSum,
						AmountL4 = budgetDto.L4UserIdSum,
						AmountS1 = budgetDto.S1UserIdSum,
						AmountS2 = budgetDto.S2UserIdSum,
						AmountS3 = budgetDto.S3UserIdSum,
					};
					_context.Add(matrix);

					//if (employeeL1 != null)
					//{
					//	matrixLevelL1 = new Model.MatrixLevel()
					//	{
					//		Amount = budgetDto.L1UserIdSum,
					//		CreatedAt = DateTime.Now,
					//		CreatedBy = budgetDto.UserId,
					//		EmployeeId = employeeL1.Id,
					//		IsDeleted = false,
					//		LevelId = levelL1.Id,
					//		Matrix = matrix,
					//		ModifiedAt = DateTime.Now,
					//		ModifiedBy = budgetDto.UserId,
					//		UomId = uom.Id
					//	};
					//	_context.Add(matrixLevelL1);

					//}

					//if (employeeL2 != null)
					//{

					//	matrixLevelL2 = new Model.MatrixLevel()
					//	{
					//		Amount = budgetDto.L2UserIdSum,
					//		CreatedAt = DateTime.Now,
					//		CreatedBy = budgetDto.UserId,
					//		EmployeeId = employeeL2.Id,
					//		IsDeleted = false,
					//		LevelId = levelL2.Id,
					//		Matrix = matrix,
					//		ModifiedAt = DateTime.Now,
					//		ModifiedBy = budgetDto.UserId,
					//		UomId = uom.Id
					//	};
					//	_context.Add(matrixLevelL2);

					//}

					//if (employeeL3 != null)
					//{

					//	matrixLevelL3 = new Model.MatrixLevel()
					//	{
					//		Amount = budgetDto.L3UserIdSum,
					//		CreatedAt = DateTime.Now,
					//		CreatedBy = budgetDto.UserId,
					//		EmployeeId = employeeL3.Id,
					//		IsDeleted = false,
					//		LevelId = levelL3.Id,
					//		Matrix = matrix,
					//		ModifiedAt = DateTime.Now,
					//		ModifiedBy = budgetDto.UserId,
					//		UomId = uom.Id
					//	};
					//	_context.Add(matrixLevelL3);


					//}

					//if (employeeL4 != null)
					//{
					//	matrixLevelL4 = new Model.MatrixLevel()
					//	{
					//		Amount = budgetDto.L4UserIdSum,
					//		CreatedAt = DateTime.Now,
					//		CreatedBy = budgetDto.UserId,
					//		EmployeeId = employeeL4.Id,
					//		IsDeleted = false,
					//		LevelId = levelL4.Id,
					//		Matrix = matrix,
					//		ModifiedAt = DateTime.Now,
					//		ModifiedBy = budgetDto.UserId,
					//		UomId = uom.Id
					//	};
					//	_context.Add(matrixLevelL4);


					//}

					//if (employeeS1 != null)
					//{
					//	matrixLevelS1 = new Model.MatrixLevel()
					//	{
					//		Amount = budgetDto.S1UserIdSum,
					//		CreatedAt = DateTime.Now,
					//		CreatedBy = budgetDto.UserId,
					//		EmployeeId = employeeS1.Id,
					//		IsDeleted = false,
					//		LevelId = levelS1.Id,
					//		Matrix = matrix,
					//		ModifiedAt = DateTime.Now,
					//		ModifiedBy = budgetDto.UserId,
					//		UomId = uom.Id
					//	};
					//	_context.Add(matrixLevelS1);


					//}

					//if (employeeS2 != null)
					//{
					//	matrixLevelS2 = new Model.MatrixLevel()
					//	{
					//		Amount = budgetDto.S2UserIdSum,
					//		CreatedAt = DateTime.Now,
					//		CreatedBy = budgetDto.UserId,
					//		EmployeeId = employeeS2.Id,
					//		IsDeleted = false,
					//		LevelId = levelS2.Id,
					//		Matrix = matrix,
					//		ModifiedAt = DateTime.Now,
					//		ModifiedBy = budgetDto.UserId,
					//		UomId = uom.Id
					//	};
					//	_context.Add(matrixLevelS2);
					//}

					//if (employeeS3 != null)
					//{
					//	matrixLevelS3 = new Model.MatrixLevel()
					//	{
					//		Amount = budgetDto.S3UserIdSum,
					//		CreatedAt = DateTime.Now,
					//		CreatedBy = budgetDto.UserId,
					//		EmployeeId = employeeS3.Id,
					//		IsDeleted = false,
					//		LevelId = levelS3.Id,
					//		Matrix = matrix,
					//		ModifiedAt = DateTime.Now,
					//		ModifiedBy = budgetDto.UserId,
					//		UomId = uom.Id
					//	};
					//	_context.Add(matrixLevelS3);
					//}

					_context.SaveChanges();
				}
			}

			return matrix.Id;

        }

		public async Task<int> Import(Dto.MatrixImport matrixImportDTO)
		{
			Model.MatrixImport matrixImport = null;
			_context.UserId = matrixImportDTO.UserId;

			matrixImport = new Model.MatrixImport()
			{
				//CompanyCode = matrixImportDTO.CompanyCode,
				//CompanyName = matrixImportDTO.CompanyName,
				//CostCenterCode = matrixImportDTO.CostCenterCode,
				//CostCenterName = matrixImportDTO.CostCenterName,
				//AdmCenter = matrixImportDTO.AdmCenter,
				//Area = matrixImportDTO.Area,
				//CountryCode = matrixImportDTO.CountryCode,
				//CountryName = matrixImportDTO.CountryName,
				DepartmentCode = matrixImportDTO.DepartmentCode,
				DepartmentName = matrixImportDTO.DepartmentName,
				DivisionCode = matrixImportDTO.DivisionCode,
				DivisionName = matrixImportDTO.DivisionName,
				//AssetTypeCode = matrixImportDTO.AssetTypeCode,
				//AssetTypeName = matrixImportDTO.AssetTypeName,
				L1UserId = matrixImportDTO.L1UserId,
				L2UserId = matrixImportDTO.L2UserId,
				L3UserId = matrixImportDTO.L3UserId,
				L4UserId = matrixImportDTO.L4UserId,
				S1UserId = matrixImportDTO.S1UserId,
				S2UserId = matrixImportDTO.S2UserId,
				S3UserId = matrixImportDTO.S3UserId,
				L1UserSum = matrixImportDTO.L1UserIdSum,
				L2UserSum = matrixImportDTO.L2UserIdSum,
				L3UserSum = matrixImportDTO.L3UserIdSum,
				L4UserSum = matrixImportDTO.L4UserIdSum,
				S1UserSum = matrixImportDTO.S1UserIdSum,
				S2UserSum = matrixImportDTO.S2UserIdSum,
				S3UserSum = matrixImportDTO.S3UserIdSum,
				Used = false
			};
			await _context.AddAsync(matrixImport);

			_context.SaveChanges();

			return matrixImport.Id;

		}

		//public async Task<List<Matrix>> GetAllMatrixChildrensAsync(int projectId, int costCenterId)
		//{
		//	return await _context.Set<Matrix>()
		//		.Include(i => i.MatrixLevels)
		//			.ThenInclude(e => e.Employee)
		//		.Where(r => r.ProjectId == projectId && r.CostCenterId == costCenterId && r.IsDeleted == false).ToListAsync();
		//}

		public async Task<List<Matrix>> GetMatchMatrixAsync(int divisionId)
		{
			return await _context.Set<Matrix>()
				.Include(E => E.EmployeeB1)
				.Include(E => E.EmployeeL1)
				.Include(E => E.EmployeeL2)
				.Include(E => E.EmployeeL3)
				.Include(E => E.EmployeeL4)
				.Include(E => E.EmployeeS1)
				.Include(E => E.EmployeeS2)
				.Include(E => E.EmployeeS3)
				.Include(E => E.Division)
				.Where(r => r.DivisionId == divisionId && r.IsDeleted == false).ToListAsync();
		}
	}
}
