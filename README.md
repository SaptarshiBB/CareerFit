# CareerFit: Matching Candidates with Suitable Careers

> **A self-contained, enterprise-grade career guidance and recommendation web application built with C#, ASP.NET Core MVC (.NET 10), Entity Framework Core, SQLite, and ASP.NET Core Identity.**

---

## 1. Executive Summary

**CareerFit** is a full-stack career consultancy platform designed to bridge candidate credentials with industry-verified career pathways. Unlike opaque recommendation systems or basic CRUD applications, CareerFit features an **explainable, rule-based recommendation algorithm**, a **multi-stage career assessment engine (Sections A–H)**, **customizable skill gap analysis**, **3-stage milestone roadmaps (Beginner, Intermediate, Advanced)**, and a **protected administrator portal**.

The application is self-contained: it runs locally with SQLite, requiring no external databases, third-party authentication providers, or paid cloud APIs

---

## 2. Technology Stack

| Layer | Technology | Details |
|---|---|---|
| **Backend Framework** | ASP.NET Core MVC | .NET 10 |
| **Language** | C# 13 | Strong typing, LINQ, Async/Await |
| **Database** | SQLite | `CareerFit.db` (Local relational storage) |
| **ORM** | Entity Framework Core 10 | Code-First modeling, Migrations & Seeding |
| **Security & Auth** | ASP.NET Core Identity | Cookie auth, PBKDF2 hashing, Anti-Forgery CSRF |
| **Frontend** | Razor Views (HTML5/CSS3/JS) | Component-driven UI, Responsive layout |
| **CSS Framework** | Bootstrap 5 | Professional design system with clean borders |
| **Icons** | Bootstrap Icons | SVG/Font icon set |
| **Data Visualization** | Chart.js | Interactive bar and breakdown charts |

---

## 3. Brand Identity & Design System

The application is styled with a classic, elegant career consultancy aesthetic:
- **Default Theme (Light):**
  - Primary Color: `#172B4D` (Navy Blue)
  - Accent Color: `#496A91` (Muted Blue)
  - Light Grey Secondary: `#F5F6F7`
  - Dark Charcoal Typography: `#252B33`
  - Crisp rectangular buttons (4px border-radius) with minimal shadows and subtle borders (`#DDE2E8`).
  - Headings: Serif typography (`Georgia`, `Playfair Display`).
  - Body: Sans-serif typography (`Inter`, `system-ui`).
- **Complete Dark Theme:**
  - Dark Background: `#121820`
  - Card Background: `#1D2733`
  - Primary Text: `#F1F3F5`
  - Muted Blue Accents: `#6B8CB6`
  - Persistent theme toggle in navbar with `localStorage` state retention.

---

## 4. Key Functional Modules

### 4.1. Candidate Profile Management
- Comprehensive profile capturing full name, email, phone, highest education, degree, specialization, graduation year, experience seniority, and preferred work environment (Remote, Hybrid, On-site).
- Dynamic profile completion tracker (0–100%).
- Interactive skill management: add technical and soft skills with proficiency levels and years of experience.

### 4.2. Database-Driven Career Catalogue (20+ Seeded Roles)
Curated database containing full profiles for at least 20 in-demand professions:
1. **Software Developer**
2. **.NET Developer**
3. **Java Developer**
4. **Python Developer**
5. **Frontend Developer**
6. **Backend Developer**
7. **Full Stack Developer**
8. **Data Analyst**
9. **Data Scientist**
10. **Machine Learning Engineer**
11. **Cloud Engineer**
12. **DevOps Engineer**
13. **Cybersecurity Analyst**
14. **Database Administrator**
15. **UI/UX Designer**
16. **Business Analyst**
17. **Product Manager**
18. **Digital Marketing Specialist**
19. **Financial Analyst**
20. **QA Engineer**

*Every role features detailed descriptions, responsibilities, required education, experience prerequisites, suggested learning topics, career growth paths, and illustrative salary ranges.*

### 4.3. Multi-Step Career Assessment System
An 8-stage step-by-step questionnaire evaluating:
- **Section A:** Educational Background (degree level and discipline focus)
- **Section B:** Technical Skills & Tooling (programming languages, frameworks, cloud)
- **Section C:** Interpersonal & Soft Skills (problem-solving, communication, teamwork)
- **Section D:** Personal Interests (backend, data, security, design, product)
- **Section E:** Problem-Solving Abilities (algorithmic debugging comfort rating scale)
- **Section F:** Preferred Work Environment (remote, hybrid, on-site, flexible)
- **Section G:** Long-Term Career Goals (technical specialist, management, research)
- **Section H:** Cumulative Experience Level (entry-level through senior)

### 4.4. Rule-Based Career Recommendation Engine
Implemented in `CareerMatchingService.cs`, using the exact weighted formula:

$$\text{Final Score} = (0.40 \times \text{Tech Skills}) + (0.25 \times \text{Interests}) + (0.20 \times \text{Education}) + (0.15 \times \text{Work Preference})$$

- **Normalized Scoring:** Every sub-dimension evaluates to 0–100%.
- **Explainability:** Generates personalized narrative explanations explaining why the role fits the candidate and which skills remain to be acquired.
- **Top 5 Ranking:** Produces top ranked opportunities with visual component progress bars and Chart.js comparisons.

### 4.5. Skill Gap Analysis
- Direct comparison between candidate skills and role prerequisites.
- Identifies **Possessed Skills** with current proficiency.
- Categorizes **Missing Skills** by priority (**High**, **Medium**, **Low**).
- Computes overall prerequisite competency percentage with progress bars.
- One-click trigger to generate a targeted remediation roadmap.

### 4.6. Career Roadmap & Milestone Tracker
- Generates structured learning paths broken into 3 progressive tiers:
  - **Stage 1 (Beginner):** Core fundamentals, syntactical fluency, and relational persistence.
  - **Stage 2 (Intermediate):** Application engineering, RESTful APIs, and integration testing.
  - **Stage 3 (Advanced):** Cloud containerization, observability, and portfolio capstone.
- Includes concrete project ideas for each milestone.
- Interactive completion checkboxes backed by SQLite persistence.

### 4.7. Side-by-Side Career Comparison
- Allows candidates to select any two careers from the database and compare them across skills, responsibilities, education, experience, benchmark salary, and personal match percentage.

### 4.8. Protected Administrator Portal
- **Dashboard Analytics:** Live counts of registered users, completed assessments, career profiles, and industry skills.
- **Top Recommended Roles:** Chart.js bar chart displaying frequency of recommendations across the candidate base.
- **User Registry:** View all registered candidate accounts and their educational profiles.
- **Career Management:** Full CRUD capability to add new career profiles, update descriptions, toggle active status, and map prerequisite skills.
- **Skill Management:** Create and delete skills across Technical and Soft categories.
- **Questionnaire Inspection:** Audit questions and option weightings across sections A through H.

---

## 5. Pre-Seeded Default Accounts

For instant local evaluation, the database auto-seeds on initial application startup:

### Administrator Account
- **Email:** `admin@careerfit.com`
- **Password:** `Admin@CareerFit2026!`
- **Role:** `Admin` (Access to `/Admin` dashboard, User Registry, Career CRUD)

### Sample Candidate Account
- **Email:** `alex.rivera@example.com`
- **Password:** `User@CareerFit2026!`
- **Role:** `User` (Pre-populated with B.S. in Computer Science and software skills)

---

## 6. How to Run the Application

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/) installed on your machine.

### Launching the Application
Navigate to the project root directory and execute:

```bash
dotnet run
```

Once running, open your web browser at:
- **HTTP:** `http://localhost:5000` (or `http://localhost:5200` depending on your port configuration)
- **HTTPS:** `https://localhost:5001` (or `https://localhost:7200`)

*The SQLite database (`CareerFit.db`) will automatically initialize and seed on the first startup.*

### Running Built-In Integration Verification Tests
To run all 10 automated test suites verifying database connectivity, seeder counts, the recommendation algorithm, skill gap analysis, and roadmap milestone updates:

```bash
dotnet run -- --verify
```

---

## 7. Solution Architecture

```
CareerFit/
├── Controllers/
│   ├── HomeController.cs           # Landing page, How It Works, About
│   ├── AccountController.cs        # Identity Auth (Login, Register, Logout)
│   ├── ProfileController.cs        # Candidate profile & skills management
│   ├── CareerController.cs         # Explore catalogue, details, compare
│   ├── AssessmentController.cs     # 8-step assessment wizard & scoring
│   ├── DashboardController.cs      # User dashboard overview
│   ├── RecommendationController.cs # Explainable recommendations view
│   ├── SkillGapController.cs       # Dedicated skill gap analysis
│   ├── RoadmapController.cs        # 3-stage learning roadmaps & milestones
│   ├── SavedCareerController.cs    # Bookmark saved careers
│   └── AdminController.cs          # Protected administrative management
├── Data/
│   ├── ApplicationDbContext.cs     # EF Core SQLite database context
│   └── DbInitializer.cs            # Seeder for roles, admin, careers & questions
├── Models/
│   ├── ApplicationUser.cs          # Identity user model
│   ├── CandidateProfile.cs         # Candidate education, background, interests
│   ├── Career.cs                   # Detailed career profile specification
│   ├── CareerCategory.cs           # Career classification grouping
│   ├── CareerSkill.cs              # Many-to-many career prerequisite mapping
│   ├── Skill.cs                    # Technical and soft skill dictionary
│   ├── UserSkill.cs                # Candidate-possessed skills
│   ├── Question.cs                 # Assessment questionnaire item
│   ├── QuestionOption.cs           # Weighted option response
│   ├── Assessment.cs               # Candidate assessment instance
│   ├── AssessmentAnswer.cs         # Recorded response
│   ├── Recommendation.cs           # Persistent computed recommendation
│   ├── Roadmap.cs                  # Structured career roadmap
│   ├── RoadmapMilestone.cs         # Roadmap stage milestone
│   └── SavedCareer.cs              # User career bookmarks
├── Services/
│   ├── ICareerMatchingService.cs   # 40/25/20/15 weighted matching algorithm
│   ├── ISkillGapService.cs         # Gap analysis and remediation logic
│   ├── IRoadmapService.cs          # 3-stage roadmap milestone generator
│   ├── IProfileService.cs          # Profile calculation & completion metric
│   └── IAdminService.cs            # Administrative statistics & catalogue CRUD
├── ViewModels/                     # Strongly-typed models for forms and views
├── Views/                          # Razor views organized by controller
├── wwwroot/
│   ├── css/site.css                # CareerFit Design System & dark theme
│   └── js/site.js                  # Theme switcher with localStorage persistence
├── CareerFit.db                    # Self-contained SQLite database
└── Program.cs                      # Application pipeline, DI, and startup configuration
```

---

## 8. Compliance & Standards

- **Zero External API Dependencies:** Requires no third-party AI keys or paid external services.
- **Relational Integrity:** Implements foreign keys, cascade rules, and composite indexes.
- **Accessibility & Responsiveness:** Built on Bootstrap 5 with responsive grid design for mobile, tablet, and desktop viewports.
- **Auditable Security:** Anti-forgery validation (`[ValidateAntiForgeryToken]`), password hashing, and role checks (`[Authorize(Roles = "Admin")]`).
- **Illustrative Compensation Notice:** Clearly labels salary figures as benchmark estimates for career exploration.
