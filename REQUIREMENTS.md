# 💇‍♀️ Parlor Booking System - Requirements Specification

## 1. Project Overview
A web-based scheduling platform for a solo-operator parlor (Auntie) to manage appointments, prevent overlaps, and verify deposits.

## 2. Target Users
* **Customer:** Browses services, creates an account, requests bookings, and provides proof of deposit.
* **Admin (Auntie):** Manages services, reviews schedules, and approves/rejects booking requests.

## 3. Core Business Rules
* **Solo Operation:** Only one appointment can be confirmed at any given time.
* **Buffer Time:** Every service automatically adds 15 minutes of "clean-up" time to the end of the appointment.
* **Pre-paid Deposit:** A booking is only "Pending" until a deposit receipt is uploaded.
* **Manual Approval:** Auntie has the final say. A booking is not "Confirmed" until she reviews the receipt and clicks approve.

## 4. Key Features (V1)
* **Booking Engine:** Calculates end times and checks for clashes using "Bouncer" logic.
* **Receipt Upload:** A field in the booking form for customers to attach a screenshot of their GCash/Bank transfer.
* **Status Management:** Appointments flow through states: `Pending` -> `Confirmed` / `Rejected` -> `Completed` / `Cancelled`.
* **Admin Dashboard:** A private view for Auntie to see all incoming requests and their associated payment proof.
