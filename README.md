Clinic Management System – Backend API
Overview

A Clinic Management System built using ASP.NET Core Web API and SQL Server.
The system allows patients to register, book appointments online, and enables clinic staff to manage appointments, medical records, and payments.

The project focuses on real-world backend practices, including security, business rules, and clean architecture.

Key Features
User Management

Patient registration and login

JWT-based authentication

Role-based access:

Patient

Reception

Doctor

Admin

Authentication & Authorization

JWT Authentication

Role-Based Authorization

Policy-Based Authorization

Ownership Validation

Patients can only access and manage their own appointments

User identity is extracted from the JWT token to prevent unauthorized actions

Appointment System (Dynamic Scheduling)

Patients can book appointments through the system:

Select a doctor

System returns available dates for the next two weeks

Select a date

System generates available time slots dynamically

Already booked slots are automatically excluded

Appointments can be created:

By the patient (online)

By the reception staff (internally)

Payments

Payment is created during appointment booking

Appointment and Payment are saved using SQL Transaction

PaymentsAudit table for tracking payment changes

Medical Records

Each appointment can be linked to a medical record

Doctors can update patient medical data

Database Tables

Persons

Users

Patients

Doctors

Appointments

MedicalRecords

Payments

PaymentsAudit

Technologies Used

ASP.NET Core Web API (.NET)

SQL Server

ADO.NET

Stored Procedures

JWT Authentication

Role-Based & Policy-Based Authorization

Transactions (SQL)

3-Tier Architecture

Security Highlights

JWT token validation

Role-based endpoint protection

Ownership checks to prevent data access by other users

Secure database operations using parameters

Transaction handling to ensure data consistency

Example Workflow (Patient Booking)

Patient logs in

Chooses a doctor

Views available dates

Selects a date

Views available time slots

Confirms booking

System:

Creates Payment

Creates Appointment

Returns booking details

Project Goal

This project was built to demonstrate:

Real-world backend development skills

Clean architecture and separation of concerns

Secure API design

Business logic implementation beyond basic CRUD
