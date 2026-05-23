-- =============================================================================
-- HR Management - PostgreSQL seed data
-- Database: HR_Manage
--
-- Default login (after seed):
--   Email:    admin@hr.com
--   Password: Admin@1234
--
-- Also creates: hr.user@company.com / Admin@1234
-- =============================================================================

BEGIN;

TRUNCATE TABLE "Employee_Tbl", "User_Tbl", "Department_Tbl" RESTART IDENTITY CASCADE;

-- -----------------------------------------------------------------------------
-- Departments
-- -----------------------------------------------------------------------------
INSERT INTO "Department_Tbl" ("DepartmentName", "DepartmentCode", "Location", "NumberOfPersonals")
VALUES
    ('Human Resources',       'HR',  'Building A - Floor 2', 0),
    ('Information Technology','IT',  'Building B - Floor 4', 0),
    ('Finance',               'FIN', 'Building A - Floor 1', 0),
    ('Sales & Marketing',     'MKT', 'Building C - Floor 3', 0);

-- -----------------------------------------------------------------------------
-- Users (PBKDF2 hash compatible with HR_Management.Application.Security.PasswordHasher)
-- Password for both accounts: Admin@1234
-- -----------------------------------------------------------------------------
INSERT INTO "User_Tbl" ("Email", "PasswordHash", "FullName")
VALUES
    (
        'admin@hr.com',
        'TT+BMzKO+gOoSZR4eBWTkQ==.PRljEfsh75twUIWZufqWkcc0fiWoxyK887iFalo91WY=',
        'System Administrator'
    ),
    (
        'hr.user@company.com',
        'TT+BMzKO+gOoSZR4eBWTkQ==.PRljEfsh75twUIWZufqWkcc0fiWoxyK887iFalo91WY=',
        'HR Staff User'
    );

-- -----------------------------------------------------------------------------
-- Employees (linked by department code)
-- -----------------------------------------------------------------------------
INSERT INTO "Employee_Tbl" ("EmployeeName", "EmployeeCode", "DepartmentId", "Rank")
SELECT 'Nguyen Van An',   'EMP001', d."Id", 'HR Manager'
FROM "Department_Tbl" d WHERE d."DepartmentCode" = 'HR';

INSERT INTO "Employee_Tbl" ("EmployeeName", "EmployeeCode", "DepartmentId", "Rank")
SELECT 'Tran Thi Binh',   'EMP002', d."Id", 'Recruiter'
FROM "Department_Tbl" d WHERE d."DepartmentCode" = 'HR';

INSERT INTO "Employee_Tbl" ("EmployeeName", "EmployeeCode", "DepartmentId", "Rank")
SELECT 'Le Van Cuong',    'EMP003', d."Id", 'Senior Developer'
FROM "Department_Tbl" d WHERE d."DepartmentCode" = 'IT';

INSERT INTO "Employee_Tbl" ("EmployeeName", "EmployeeCode", "DepartmentId", "Rank")
SELECT 'Pham Thi Dung',   'EMP004', d."Id", 'DevOps Engineer'
FROM "Department_Tbl" d WHERE d."DepartmentCode" = 'IT';

INSERT INTO "Employee_Tbl" ("EmployeeName", "EmployeeCode", "DepartmentId", "Rank")
SELECT 'Hoang Van Em',    'EMP005', d."Id", 'Accountant'
FROM "Department_Tbl" d WHERE d."DepartmentCode" = 'FIN';

INSERT INTO "Employee_Tbl" ("EmployeeName", "EmployeeCode", "DepartmentId", "Rank")
SELECT 'Vo Thi Phuong',   'EMP006', d."Id", 'Sales Executive'
FROM "Department_Tbl" d WHERE d."DepartmentCode" = 'MKT';

INSERT INTO "Employee_Tbl" ("EmployeeName", "EmployeeCode", "DepartmentId", "Rank")
SELECT 'Dang Van Giang',  'EMP007', d."Id", 'Marketing Specialist'
FROM "Department_Tbl" d WHERE d."DepartmentCode" = 'MKT';

-- Sync employee counts on departments
UPDATE "Department_Tbl" d
SET "NumberOfPersonals" = (
    SELECT COUNT(*)::integer
    FROM "Employee_Tbl" e
    WHERE e."DepartmentId" = d."Id"
);

COMMIT;

-- Summary
SELECT 'Department_Tbl' AS table_name, COUNT(*) AS row_count FROM "Department_Tbl"
UNION ALL
SELECT 'Employee_Tbl', COUNT(*) FROM "Employee_Tbl"
UNION ALL
SELECT 'User_Tbl', COUNT(*) FROM "User_Tbl";
