🏥 Clinic Management System – Backend API

A clinic management backend system built with ASP.NET Core Web API (.NET 7), focusing on security, business rules, and clean code organization.

✅ What I Built in This Project

Designed and implemented a full clinic management system

Built all core modules:

Person

Users

Doctors

Patients

Appointments

Medical Records

Payments

Payment Audit Logs

Implemented 3-Tier Architecture with a clear separation between:

API Layer

Business Logic Layer

Data Access Layer

Designed the database using SQL Server and Stored Procedures for complex queries and operations

Used DTOs to control the data exposure and improve API performance

🔄 Smart DTO Design

Created multiple DTOs based on use case:

Basic DTOs for standard CRUD operations

Detailed DTOs using Joins in Stored Procedures

Example:

GetAllAppointments → returns basic appointment data

GetAllAppointmentDetails → returns:

Patient Name

Doctor Name

Symptoms

Diagnosis
(joined from Appointment, Doctor, Patient, Person, MedicalRecord)

The same approach is applied to Payments and Payment Details

🔐 Authentication & Authorization

Implemented JWT Authentication

Role-based access control:

Admin: full system access

Doctor: can view only their patients and medical records

Receptionist: can add persons and patients, manage appointments

Applied Ownership Checks to prevent unauthorized data access

📊 Payment Audit Logging

Tracks every modification to payments, including:

Old value

New value

User who made the change

Timestamp

Ensures:

Full traceability of changes

Protection of financial data

Increased reliability of the system

🛠️ Tech Stack

C# .NET 7

ASP.NET Core Web API

3-Tier Architecture

SQL Server & Stored Procedures

JWT Authentication

BCrypt for password hashing

Swagger (OpenAPI)

🎯 Project Importance

This project demonstrates my ability to:

Build secure Back-End systems

Apply real-world business rules

Design databases using SQL and Stored Procedures

Control data and permissions based on user roles

Implement audit logging and data traceability
