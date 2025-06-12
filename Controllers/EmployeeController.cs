using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeDataProject.Data;
using EmployeeDataProject.Models;

public class EmployeeController : Controller
{
    private readonly InterviewTestContext _context;
    //DI
    public EmployeeController(InterviewTestContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index(string company = "", string status = "all", string search = "",
                                           string sortField = "EmpCode", string sortDirection = "asc",
                                           int page = 1, int pageSize = 10)
    {
        var companyList = await _context.Companies.ToListAsync();
        ViewBag.Companies = new SelectList(companyList, "CompanyCode", "CompanyDesc", company);

        ViewBag.Status = new SelectList(new[]
        {
            new SelectListItem { Text = "All", Value = "all" },
            new SelectListItem { Text = "Active", Value = "Active" },
            new SelectListItem { Text = "Resigned", Value = "Resigned" }
        }, "Value", "Text", status);

        var employeesQuery = _context.Employees.AsQueryable();

        if (!string.IsNullOrEmpty(company))
            employeesQuery = employeesQuery.Where(e => e.CompanyCode == company);

        if (status == "Active")
            employeesQuery = employeesQuery.Where(e => e.ResignDate == null || e.ResignDate > DateTime.Today);
        else if (status == "Resigned")
            employeesQuery = employeesQuery.Where(e => e.ResignDate != null && e.ResignDate <= DateTime.Today);

        if (!string.IsNullOrEmpty(search))
        {
            employeesQuery = employeesQuery.Where(e =>
                e.EmployeeName.Contains(search) ||
                e.AliasName.Contains(search) ||
                e.ICNO.Contains(search));
        }

        switch (sortField)
        {
            case "EmployeeName":
                employeesQuery = sortDirection == "asc"
                    ? employeesQuery.OrderBy(e => e.EmployeeName)
                    : employeesQuery.OrderByDescending(e => e.EmployeeName);
                break;
            case "AliasName":
                employeesQuery = sortDirection == "asc"
                    ? employeesQuery.OrderBy(e => e.AliasName)
                    : employeesQuery.OrderByDescending(e => e.AliasName);
                break;
            case "EmpCode":
            default:
                employeesQuery = sortDirection == "asc"
                    ? employeesQuery.OrderBy(e => e.EmpCode)
                    : employeesQuery.OrderByDescending(e => e.EmpCode);
                break;
        }

        int totalCount = await employeesQuery.CountAsync();
        ViewBag.TotalCount = totalCount;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;

        var employees = await employeesQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var departments = await _context.CodeRefs
            .Where(c => c.CodeType == "DPT")
            .ToListAsync();

        var designations = await _context.CodeRefs
            .Where(c => c.CodeType == "DES")
            .ToListAsync();

        foreach (var emp in employees)
        {
            emp.Department = departments.FirstOrDefault(d => d.Code == emp.DepartmentCode);
            emp.Designation = designations.FirstOrDefault(d => d.Code == emp.DesignationCode);
        }

        return View(employees);
    }
}
