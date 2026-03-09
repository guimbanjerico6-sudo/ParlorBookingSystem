# 💇‍♀️ Parlor Booking System - Requirements Specification

## 1. Project Overview
A web-based scheduling platform for a solo-operator parlor (Auntie) to manage hair and beauty appointments, prevent overlapping schedules, and verify deposits before confirmation.

## 2. Target Users
* **Customer:** Browses services, creates an account, requests bookings, and uploads proof of deposit.
* **Admin (Auntie):** Manages the service menu, reviews schedules, and approves/rejects incoming booking requests.

## 3. Core Business Rules
* **Solo Operation:** Only one appointment can be "Confirmed" at any given time slot.
* **Buffer Time:** Every service automatically adds **15 minutes** of "clean-up" time to the calculated end time.
* **Pre-paid Deposit:** A booking request is marked as `Pending` and requires a GCash/Bank receipt upload to be valid.
* **Manual Approval:** Auntie has the final authority; a booking is only finalized when she verifies the deposit and manually clicks "Approve."
* **Automated Notifications:** The system must send an **Email Notification** to the customer once Auntie approves or rejects the request.

## 4. Technical Stack
* **Backend:** .NET 8 Web API (C#)
* **Frontend:** React (Vite/TypeScript)
* **Database:** SQL Server (LocalDB for development)
* **ORM:** Entity Framework Core (Code-First)
* **Security:** JWT (JSON Web Tokens) for Authentication and Role-Based Authorization
* **Architecture:** N-Tier Architecture (Controller -> Service Layer -> Data Access Layer)

## 5. Appointment Lifecycle (States)
1. **Pending:** Requested by customer, awaiting Auntie's review.
2. **Confirmed:** Approved by Auntie; slot is officially blocked.
3. **Rejected:** Denied by Auntie (e.g., invalid deposit or personal conflict).
4. **Cancelled:** Withdrawn by the customer.
5. **Completed:** Service rendered and finished.
