using HR_Management.Application.DTOs;
using HR_Management.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management.Web.Controllers;

[Authorize]
public class DepartmentController : Controller
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetAllAsync(cancellationToken);
        return View(departments);
    }

    public async Task<IActionResult> Statistics(CancellationToken cancellationToken)
    {
        var statistics = await _departmentService.GetStatisticsAsync(cancellationToken);
        return View(statistics);
    }

    public IActionResult Create()
    {
        return View(new DepartmentDTO());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DepartmentDTO model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _departmentService.CreateAsync(model, cancellationToken);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to create department.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Department created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var department = await _departmentService.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return NotFound();
        }

        return View(department);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DepartmentDTO model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _departmentService.UpdateAsync(model, cancellationToken);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to update department.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Department updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var department = await _departmentService.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return NotFound();
        }

        return View(department);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var result = await _departmentService.DeleteAsync(id, cancellationToken);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Delete), new { id });
        }

        TempData["SuccessMessage"] = "Department deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
