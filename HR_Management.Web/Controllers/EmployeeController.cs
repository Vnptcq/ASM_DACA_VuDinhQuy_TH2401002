using HR_Management.Application.DTOs;
using HR_Management.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HR_Management.Web.Controllers;

[Authorize]
public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;

    public EmployeeController(IEmployeeService employeeService, IDepartmentService departmentService)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var employees = await _employeeService.GetAllAsync(cancellationToken);
        return View(employees);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        await PopulateDepartmentsAsync(cancellationToken);
        return View(new EmployeeDTO());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeDTO model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDepartmentsAsync(cancellationToken);
            return View(model);
        }

        var result = await _employeeService.CreateAsync(model, cancellationToken);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to add employee.");
            await PopulateDepartmentsAsync(cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = "Employee added successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDepartmentsAsync(CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetAllAsync(cancellationToken);
        ViewBag.Departments = new SelectList(departments, "Id", "DepartmentName");
    }
}
