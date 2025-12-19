# Student Performance Monitoring System (SPMS)

## 📌 Overview
The **Student Performance Monitoring System (SPMS)** is a centralized, web-based academic management platform designed to monitor and evaluate student performance across multiple PG Diploma courses.  
It automates student enrollment, grading, result generation, and ranking, reducing manual effort, minimizing errors, and improving transparency in academic evaluation.

The system supports **role-based access** for Administrators, Staff (Faculty), and Students, ensuring secure and efficient academic operations.

---

## 👥 User Roles
- **Administrator**
  - Manages the entire system
  - Configures courses, subjects, users, and deadlines
- **Staff (Faculty)**
  - Enters marks and performs grading tasks
- **Student**
  - Views results, ranks, and personal academic progress

---

## 🧩 Key Features
- Centralized academic data management
- Automated PRN generation
- Secure login with role-based access control (RBAC)
- Deadline-based grading enforcement
- Automatic result and ranking generation
- Email notifications for credentials and updates

---

## 📘 Definitions & Abbreviations
- **SPMS** – Student Performance Monitoring System  
- **PRN** – Permanent Registration Number (12-digit unique ID)  
- **RBAC** – Role-Based Access Control  
- **T** – Theory Marks  
- **L** – Lab Marks  
- **I** – Internal Assessment Marks  

---

## 🖥️ System Description

### Product Perspective
SPMS is a **standalone web-based system** that replaces traditional paper-based or spreadsheet-based result processing.  
All academic data (courses, subjects, students, marks, results) is stored securely in a centralized database.

### Operating Environment
- Modern web browsers (Chrome, Firefox, Edge)
- Server-side application environment
- Relational Database Management System
- Email service for notifications

---

## 🎓 Supported Courses
Each course consists of **4 subjects**.

| Course Code | Course Name | Subjects |
|------------|------------|----------|
| PG-DAC | Post Graduate Diploma in Advanced Computing | d1, d2, d3, d4 |
| PG-DMC | Post Graduate Diploma in Mobile Computing | w1, w2, w3, w4 |
| PG-DBDA | Post Graduate Diploma in Big Data Analytics | b1, b2, b3, b4 |
| PG-DESD | Post Graduate Diploma in Embedded System Design | e1, e2, e3, e4 |
| PG-DITIIS | Post Graduate Diploma in IT Infrastructure & Security | c1, c2, c3, c4 |

---

## 🧮 Grading & Evaluation Logic
- **Total Marks per Subject:** 100  
  - Theory: 40  
  - Lab: 40  
  - Internal Assessment: 20  

### Passing Criteria
- Minimum **40% in each component (T, L, I)** is mandatory.
- If all components meet the criteria → **Pass (P)**
- If any component fails → **Fail (F)**

---

## ⚙️ Functional Requirements

### Administrator Module
- Configure courses, subjects, and marking schemes
- Enroll students and auto-generate **12-digit PRN**
- Default student password:  
# Software Requirements Specification (SRS)
## Student Performance Monitoring System (SPMS)

---

## 1. Introduction
The Student Performance Monitoring System is a centralized academic management platform designed to monitor student performance across multiple PG Diploma courses. The system automates student enrollment, grading, result generation, and ranking processes. It reduces manual work, avoids calculation errors, and provides transparency in academic evaluation.

The system supports three types of users:
- **Administrator** – Manages the entire system
- **Staff (Faculty)** – Enters marks and performs grading tasks
- **Student** – Views results, rank, and personal academic progress

SPMS ensures secure access, accurate result processing, and timely availability of performance information.

---

## 1.2 Definitions, Acronyms, and Abbreviations
- **SPMS**: Student Performance Monitoring System  
- **PRN**: Permanent Registration Number (unique student ID)  
- **RBAC**: Role-Based Access Control  
- **T**: Theory Marks  
- **L**: Lab Marks  
- **I**: Internal Assessment Marks  

---

## 1.3 Intended Audience
This document is intended for:
- Academic project evaluators  
- Project guides and mentors  
- Software developers  
- Testing and documentation teams  

---

## 2. Overall Description

### 2.1 Product Perspective
SPMS is a standalone, web-based academic monitoring system. It replaces traditional paper-based or spreadsheet-based result processing with a fully automated digital solution. All academic data such as courses, subjects, students, marks, and results are stored securely in a centralized database.

---

### 2.2 User Classes and Characteristics

| User | Description |
|-----|------------|
| Administrator | Has full control over the system including course setup, user management, task assignment, and monitoring |
| Staff | Faculty members responsible for entering marks for assigned subjects and batches |
| Student | End users who can view their academic performance and ranking |

---

### 2.3 Operating Environment
- Any modern web browser (Chrome, Firefox, Edge)
- Server-side application environment
- Relational Database Management System
- Email service for notifications

---

### 2.4 Design Constraints
- PRN must be exactly 12 digits and unique
- Marks must follow the defined grading structure
- Deadline-based restrictions must be strictly enforced
- Only authorized users can access system features

---

### 2.5 Assumptions and Dependencies
- All users have valid email IDs
- Internet connectivity is available
- Email server is properly configured
- Users have basic computer literacy

---

## 3. System Features

### 3.1 Course and Subject Management
The system supports multiple PG Diploma courses. Each course consists of four subjects and has predefined attributes such as course name, description, duration, and fees.

#### Supported Courses

| Course Code | Course Name | Subjects |
|------------|------------|----------|
| PG-DAC | Post Graduate Diploma in Advanced Computing | d1, d2, d3, d4 |
| PG-DMC | Post Graduate Diploma in Mobile Computing | w1, w2, w3, w4 |
| PG-DBDA | Post Graduate Diploma in Big Data Analytics | b1, b2, b3, b4 |
| PG-DESD | Post Graduate Diploma in Embedded System Design | e1, e2, e3, e4 |
| PG-DITIIS | Post Graduate Diploma in IT Infrastructure & Security | c1, c2, c3, c4 |

---

### 3.2 Grading and Evaluation Logic
Each subject carries a total of 100 marks, divided into three components:
- Theory: 40 marks  
- Lab: 40 marks  
- Internal Assessment: 20 marks  

To pass a subject, a student must score at least 40% in each individual component. If the student satisfies this condition, the result status is marked as **Pass (P)**. If any component fails to meet the criteria, the result status becomes **Fail (F)**.

---

## 4. Functional Requirements

### 4.1 Administrator Module
The Administrator is the most powerful user in the system and is responsible for managing all academic and administrative activities.

- The system shall allow the administrator to configure courses, subjects, and marking schemes
- The system shall allow the administrator to enroll students
- A unique 12-digit PRN shall be automatically generated for each student
- The default student password shall be the last four digits of the PRN followed by `@Sunbeam`
- Login credentials shall be sent automatically to the student via email
- The administrator shall assign grading tasks to staff members with subject, group, and strict deadlines
- The administrator shall create staff accounts using email as username
- Staff passwords shall follow the format `firstname@random`
- The system shall generate student-wise rankings based on total marks
- In case of tied marks, ranking shall be decided alphabetically by student name

---

### 4.2 Staff Module
The Staff module is designed for faculty members who are responsible for grading students.

- Staff members shall be able to login securely
- Staff members shall be able to change their passwords
- Staff members shall view all assigned grading tasks on a dashboard
- Staff members shall enter Theory, Lab, and Internal marks for students
- Once the deadline is crossed, the system shall automatically disable or hide the marks entry option

---

### 4.3 Student Module
The Student module allows students to track their academic progress digitally.

- Students shall be able to login securely
- Students shall be able to change their passwords
- Students shall update their personal profile details such as name, email, and mobile number
- Students shall view a digital mark sheet
- Students shall see subject-wise marks, pass/fail status, and current rank in its respective course

---

## 5. Non-Functional Requirements

### 5.1 Security Requirements
- All passwords shall be stored in encrypted format
- Role-based access control shall restrict unauthorized access

---

### 5.2 Usability Requirements
- The system shall provide a clean and user-friendly interface
- Important actions like deadlines shall be clearly visible

---

### 5.3 Performance Requirements
- The system shall efficiently handle multiple courses and groups
- The system shall support concurrent users without performance degradation

---

### 5.4 Data Integrity and Reliability
- PRN uniqueness must be enforced
- Proper validation shall be applied to marks entry
- The system shall prevent invalid or duplicate data

---

## 6. External Interface Requirements

### 6.1 User Interface
- Web-based responsive interface
- Separate dashboards for Admin, Staff, and Student

---

### 6.2 Software Interfaces
- Email service for sending credentials and notifications
- Database system for storing academic records

---

## 7. Future Enhancements
- Graphical performance analysis and charts
- Downloadable result reports in PDF and Excel formats
- Mobile application support
- Parent access module

---

## 8. Conclusion
The Student Performance Monitoring System provides a reliable, transparent, and automated solution for managing academic performance. By reducing manual effort and improving accuracy, the system benefits administrators, faculty members, and students alike.
