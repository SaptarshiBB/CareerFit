using CareerFit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CareerFit.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        // Automatically ensure SQLite database schema is created
        await context.Database.EnsureCreatedAsync();

        // 1. Seed Roles
        string[] roles = { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Seed Default Admin User
        var adminEmail = configuration["AdminAccount:Email"] ?? "admin@careerfit.com";
        var adminPassword = configuration["AdminAccount:Password"] ?? "Admin@CareerFit2026!";
        var adminFullName = configuration["AdminAccount:FullName"] ?? "Platform Administrator";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = adminFullName,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // 3. Seed Sample Demo User
        var demoEmail = "alex.rivera@example.com";
        var demoUser = await userManager.FindByEmailAsync(demoEmail);
        if (demoUser == null)
        {
            demoUser = new ApplicationUser
            {
                UserName = demoEmail,
                Email = demoEmail,
                EmailConfirmed = true,
                FullName = "Alex Rivera",
                CreatedAt = DateTime.UtcNow
            };

            var demoResult = await userManager.CreateAsync(demoUser, "User@CareerFit2026!");
            if (demoResult.Succeeded)
            {
                await userManager.AddToRoleAsync(demoUser, "User");

                // Seed candidate profile
                var profile = new CandidateProfile
                {
                    UserId = demoUser.Id,
                    FullName = "Alex Rivera",
                    Email = demoEmail,
                    PhoneNumber = "+1 (555) 234-5678",
                    HighestEducation = "Bachelor's Degree",
                    Degree = "B.S. in Computer Science",
                    Specialization = "Software Engineering",
                    GraduationYear = 2024,
                    ExperienceLevel = "Entry-Level",
                    YearsOfExperience = 1,
                    WorkExperience = "1 year junior software developer experience building web APIs and database features.",
                    CareerInterests = "Software Development, Cloud Computing",
                    PreferredWorkDomain = "Information Technology",
                    PreferredWorkEnvironment = "Hybrid",
                    UpdatedAt = DateTime.UtcNow
                };
                context.CandidateProfiles.Add(profile);
                await context.SaveChangesAsync();
            }
        }

        // 4. Seed Career Categories
        if (!await context.CareerCategories.AnyAsync())
        {
            var categories = new List<CareerCategory>
            {
                new() { Name = "Software Development", Description = "Design, build, and maintain software applications, web platforms, and mobile solutions.", IconClass = "bi-code-slash", DisplayOrder = 1 },
                new() { Name = "Data Science", Description = "Transform complex structured and unstructured data into predictive intelligence and actionable insights.", IconClass = "bi-bar-chart-line", DisplayOrder = 2 },
                new() { Name = "Cybersecurity", Description = "Safeguard infrastructure, identities, cryptographic assets, and enterprise systems from cyber threats.", IconClass = "bi-shield-check", DisplayOrder = 3 },
                new() { Name = "Cloud Computing", Description = "Architect, automate, and orchestrate resilient global cloud infrastructure and CI/CD pipelines.", IconClass = "bi-cloud-arrow-up", DisplayOrder = 4 },
                new() { Name = "Business and Management", Description = "Bridge corporate strategy, process analysis, product roadmaps, and stakeholder execution.", IconClass = "bi-briefcase", DisplayOrder = 5 },
                new() { Name = "Finance", Description = "Analyze fiscal metrics, financial forecasting, corporate valuations, and market investment portfolios.", IconClass = "bi-currency-dollar", DisplayOrder = 6 },
                new() { Name = "Design", Description = "Craft intuitive visual interactions, user journeys, wireframes, and accessible digital design systems.", IconClass = "bi-palette", DisplayOrder = 7 },
                new() { Name = "Marketing", Description = "Scale brand acquisition, omnichannel growth strategies, content engines, and performance campaigns.", IconClass = "bi-megaphone", DisplayOrder = 8 }
            };
            context.CareerCategories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // 5. Seed Skills (Technical & Soft)
        if (!await context.Skills.AnyAsync())
        {
            var skills = new List<Skill>
            {
                // Technical - Languages & Core
                new() { Name = "C#", Category = "Technical", Description = "Modern object-oriented programming language on the .NET platform." },
                new() { Name = ".NET", Category = "Technical", Description = "Cross-platform developer platform by Microsoft." },
                new() { Name = "ASP.NET Core", Category = "Technical", Description = "High-performance framework for building modern web applications and APIs." },
                new() { Name = "Entity Framework Core", Category = "Technical", Description = "Modern object-database mapper for .NET." },
                new() { Name = "Java", Category = "Technical", Description = "Enterprise object-oriented language for robust enterprise backends." },
                new() { Name = "Spring Boot", Category = "Technical", Description = "Enterprise application framework for Java microservices." },
                new() { Name = "Python", Category = "Technical", Description = "Dynamic programming language widely used in AI, data science, and scripting." },
                new() { Name = "JavaScript", Category = "Technical", Description = "Standard programming language of the web." },
                new() { Name = "TypeScript", Category = "Technical", Description = "Typed superset of JavaScript enhancing code maintainability." },
                new() { Name = "HTML5 & CSS3", Category = "Technical", Description = "Foundational technologies for structuring and styling web interfaces." },
                new() { Name = "React", Category = "Technical", Description = "Component-based JavaScript library for building interactive user interfaces." },
                new() { Name = "Angular", Category = "Technical", Description = "Comprehensive TypeScript framework for enterprise web frontends." },
                new() { Name = "SQL", Category = "Technical", Description = "Relational query language for data extraction, manipulation, and schema definition." },
                new() { Name = "PostgreSQL", Category = "Technical", Description = "Advanced open-source relational database system." },
                new() { Name = "SQLite", Category = "Technical", Description = "Lightweight embedded relational database engine." },
                new() { Name = "Git", Category = "Technical", Description = "Distributed version control system for tracking changes in source code." },
                new() { Name = "Docker", Category = "Technical", Description = "Platform for developing, shipping, and running containerized applications." },
                new() { Name = "Kubernetes", Category = "Technical", Description = "Automated container orchestration system for enterprise scaling." },
                new() { Name = "AWS", Category = "Technical", Description = "Amazon Web Services comprehensive cloud computing suite." },
                new() { Name = "Microsoft Azure", Category = "Technical", Description = "Enterprise cloud computing service created by Microsoft." },
                new() { Name = "Linux", Category = "Technical", Description = "Open-source Unix-like operating system powering cloud and server infrastructure." },
                new() { Name = "CI/CD Pipelines", Category = "Technical", Description = "Continuous Integration and Continuous Delivery automation practices." },
                new() { Name = "REST APIs", Category = "Technical", Description = "Architectural style for interconnected web services." },
                new() { Name = "Machine Learning", Category = "Technical", Description = "Algorithms that improve automatically through data analysis and training." },
                new() { Name = "TensorFlow / PyTorch", Category = "Technical", Description = "Deep learning and neural network frameworks." },
                new() { Name = "Pandas & NumPy", Category = "Technical", Description = "Python computational and tabular data manipulation libraries." },
                new() { Name = "Power BI & Tableau", Category = "Technical", Description = "Business intelligence and visual analytics platforms." },
                new() { Name = "Penetration Testing", Category = "Technical", Description = "Authorized simulated cyberattacks against computer systems to evaluate security." },
                new() { Name = "Network Security", Category = "Technical", Description = "Firewall, VPN, IDS/IPS, and transport encryption management." },
                new() { Name = "Database Administration", Category = "Technical", Description = "Performance tuning, backup restoration, indexing, and high availability." },
                new() { Name = "UI/UX Design", Category = "Technical", Description = "User journey mapping, wireframing, usability heuristics, and visual hierarchy." },
                new() { Name = "Figma", Category = "Technical", Description = "Collaborative web interface design and prototyping tool." },
                new() { Name = "Automated Testing", Category = "Technical", Description = "Unit, integration, and end-to-end automated testing frameworks (xUnit, Selenium)." },
                new() { Name = "Business Process Modeling", Category = "Technical", Description = "Requirements elicitation, user stories, and BPMN workflow documentation." },
                new() { Name = "Agile / Scrum", Category = "Technical", Description = "Iterative software delivery frameworks and sprint planning." },
                new() { Name = "Financial Modeling", Category = "Technical", Description = "DCF valuation, balance sheet analysis, and financial forecasting in Excel." },
                new() { Name = "SEO & Content Strategy", Category = "Technical", Description = "Search engine visibility, keyword research, and audience engagement." },
                new() { Name = "Google Analytics", Category = "Technical", Description = "Digital performance tracking and web conversion metrics." },

                // Soft Skills
                new() { Name = "Problem Solving", Category = "Soft", Description = "Analytical capability to diagnose root causes and devise elegant solutions." },
                new() { Name = "Critical Thinking", Category = "Soft", Description = "Objective analysis and evaluation of an issue in order to form a judgment." },
                new() { Name = "Communication", Category = "Soft", Description = "Clear oral and written presentation of ideas to technical and non-technical stakeholders." },
                new() { Name = "Team Collaboration", Category = "Soft", Description = "Cooperative working dynamics within cross-functional multidisciplinary teams." },
                new() { Name = "Time Management", Category = "Soft", Description = "Prioritizing tasks and delivering milestones under project deadlines." },
                new() { Name = "Adaptability", Category = "Soft", Description = "Agility to master emerging tools, paradigms, and evolving project specifications." },
                new() { Name = "Leadership", Category = "Soft", Description = "Inspiring, mentoring, and steering team members toward collective goals." },
                new() { Name = "Attention to Detail", Category = "Soft", Description = "Thoroughness and accuracy when accomplishing tasks and reviewing deliverables." }
            };

            context.Skills.AddRange(skills);
            await context.SaveChangesAsync();
        }

        // Attach demo user's skills if profile exists
        if (demoUser != null)
        {
            var demoProfile = await context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == demoUser.Id);
            if (demoProfile != null && !await context.UserSkills.AnyAsync(us => us.CandidateProfileId == demoProfile.Id))
            {
                var skillNames = new[] { "C#", ".NET", "ASP.NET Core", "SQL", "Git", "REST APIs", "Problem Solving", "Team Collaboration" };
                var existingSkills = await context.Skills.Where(s => skillNames.Contains(s.Name)).ToListAsync();
                foreach (var sk in existingSkills)
                {
                    context.UserSkills.Add(new UserSkill
                    {
                        CandidateProfileId = demoProfile.Id,
                        SkillId = sk.Id,
                        ProficiencyLevel = sk.Category == "Technical" ? "Intermediate" : "Advanced",
                        YearsOfExperience = 1
                    });
                }
                await context.SaveChangesAsync();
            }
        }

        // 6. Seed Careers & CareerSkills
        if (!await context.Careers.AnyAsync())
        {
            await SeedCareersAndSkillsAsync(context);
        }

        // 7. Seed Assessment Questions & Options
        if (!await context.Questions.AnyAsync())
        {
            await SeedAssessmentQuestionsAsync(context);
        }
    }

    private static async Task SeedCareersAndSkillsAsync(ApplicationDbContext context)
    {
        var categoryMap = await context.CareerCategories.ToDictionaryAsync(c => c.Name, c => c.Id);
        var skillMap = await context.Skills.ToDictionaryAsync(s => s.Name, s => s.Id);

        var careers = new List<Career>
        {
            new()
            {
                Title = "Software Developer",
                Category = "Software Development",
                CategoryId = categoryMap.GetValueOrDefault("Software Development"),
                Summary = "Build and maintain versatile software systems, applying data structures, algorithms, and clean architecture.",
                Description = "Software Developers engineer robust software applications across platforms. They collaborate with product teams to design algorithms, implement code, write automated tests, and resolve production bottlenecks.",
                Responsibilities = "• Write clean, testable, and maintainable code in modern languages.\n• Collaborate in sprint planning, code reviews, and architecture discussions.\n• Troubleshoot, debug, and optimize software subsystems.\n• Participate in CI/CD pipeline deployments and integration testing.",
                RequiredEducation = "Bachelor's Degree in Computer Science, Software Engineering, or equivalent practical experience.",
                ExperienceLevel = "Entry-Level",
                ExperienceRequirements = "0-2 years of programming experience, strong grasp of OOP, algorithms, and version control.",
                SuggestedLearningTopics = "Object-Oriented Programming, Data Structures, Git Workflows, Unit Testing, Design Patterns.",
                SalaryMin = 75000,
                SalaryMax = 115000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Progress to Senior Software Engineer, Technical Lead, or Software Architect.",
                DemandLevel = "High",
                WorkEnvironment = "Hybrid",
                IsActive = true
            },
            new()
            {
                Title = ".NET Developer",
                Category = "Software Development",
                CategoryId = categoryMap.GetValueOrDefault("Software Development"),
                Summary = "Engineer enterprise applications and cloud-ready web APIs utilizing C#, ASP.NET Core, and Microsoft technologies.",
                Description = ".NET Developers specialize in crafting high-throughput backend services, enterprise web apps, and microservices using C# and ASP.NET Core. They leverage Entity Framework Core for data modeling and adhere to domain-driven design.",
                Responsibilities = "• Architect and implement robust RESTful APIs and MVC web applications using ASP.NET Core.\n• Build efficient data access layers using Entity Framework Core and relational databases.\n• Implement authentication, authorization, and role management with ASP.NET Core Identity.\n• Ensure high performance, resilience, and security across .NET microservices.",
                RequiredEducation = "Bachelor's Degree in Computer Science, Information Technology, or relevant discipline.",
                ExperienceLevel = "Mid-Level",
                ExperienceRequirements = "1-3 years of hands-on C# and .NET ecosystem development experience.",
                SuggestedLearningTopics = "C# 12+, ASP.NET Core MVC/WebAPI, Entity Framework Core, LINQ, Dependency Injection, Azure Services.",
                SalaryMin = 85000,
                SalaryMax = 130000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Advance to Lead .NET Architect, Solutions Architect, or Engineering Manager.",
                DemandLevel = "Very High",
                WorkEnvironment = "Hybrid",
                IsActive = true
            },
            new()
            {
                Title = "Java Developer",
                Category = "Software Development",
                CategoryId = categoryMap.GetValueOrDefault("Software Development"),
                Summary = "Develop scalable, enterprise-grade distributed backends and microservices with Java and Spring Boot.",
                Description = "Java Developers build enterprise business systems, financial payment engines, and cloud distributed services. They work heavily with Spring Boot, Hibernate, messaging queues (Kafka/RabbitMQ), and SQL databases.",
                Responsibilities = "• Design and maintain multi-tiered enterprise Java applications.\n• Implement microservices using Spring Boot, Spring Cloud, and RESTful APIs.\n• Optimize database queries and persistence layers with Hibernate/JPA.\n• Conduct unit and integration testing with JUnit and Mockito.",
                RequiredEducation = "Bachelor's Degree in Computer Science or related engineering field.",
                ExperienceLevel = "Mid-Level",
                ExperienceRequirements = "2-4 years of enterprise Java development experience.",
                SuggestedLearningTopics = "Java 17+, Spring Boot, Microservice Patterns, JPA/Hibernate, Kafka, Docker.",
                SalaryMin = 88000,
                SalaryMax = 135000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Progress to Principal Java Engineer, Enterprise Architect, or VP of Engineering.",
                DemandLevel = "High",
                WorkEnvironment = "Hybrid",
                IsActive = true
            },
            new()
            {
                Title = "Python Developer",
                Category = "Software Development",
                CategoryId = categoryMap.GetValueOrDefault("Software Development"),
                Summary = "Develop web backends, automation scripts, and data pipeline integrations using the versatile Python ecosystem.",
                Description = "Python Developers leverage frameworks like FastAPI, Django, and Flask alongside scripting modules to construct scalable web services, API gateways, and data processing services.",
                Responsibilities = "• Build fast and scalable asynchronous web services using FastAPI or Django.\n• Write automated ETL scripts and data synchronization workflows.\n• Integrate third-party APIs and manage relational and NoSQL storage.\n• Profile application bottlenecks and optimize Python memory and execution performance.",
                RequiredEducation = "Bachelor's Degree in Computer Science, Data Science, or related field.",
                ExperienceLevel = "Entry-Level",
                ExperienceRequirements = "1-3 years of Python programming and web framework experience.",
                SuggestedLearningTopics = "FastAPI, Asyncio, Django, PostgreSQL, Docker, PyTest, Celery.",
                SalaryMin = 80000,
                SalaryMax = 125000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Transition to Machine Learning Engineer, Backend Lead, or Data Engineering Lead.",
                DemandLevel = "Very High",
                WorkEnvironment = "Remote",
                IsActive = true
            },
            new()
            {
                Title = "Frontend Developer",
                Category = "Software Development",
                CategoryId = categoryMap.GetValueOrDefault("Software Development"),
                Summary = "Craft modern, responsive, and accessible client-side experiences with HTML5, CSS3, JavaScript, and React.",
                Description = "Frontend Developers bridge UI/UX design with client-side engineering. They build single-page applications, design systems, and responsive web interfaces with strong attention to web accessibility, performance, and cross-browser fidelity.",
                Responsibilities = "• Translate UI wireframes and Figma designs into pixel-perfect, accessible code.\n• Build modular component architectures using React, TypeScript, or modern CSS.\n• Manage state management and asynchronous data fetching from backend REST APIs.\n• Optimize client bundle sizes, rendering performance, and Core Web Vitals.",
                RequiredEducation = "Bachelor's Degree in Computer Science, Web Development, or equivalent portfolio.",
                ExperienceLevel = "Entry-Level",
                ExperienceRequirements = "0-2 years building web frontends with modern JavaScript frameworks.",
                SuggestedLearningTopics = "TypeScript, React, Next.js, CSS Grid/Flexbox, State Management, Accessibility (WCAG).",
                SalaryMin = 75000,
                SalaryMax = 120000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Advance to Senior Frontend Engineer, UI Architect, or Frontend Engineering Manager.",
                DemandLevel = "High",
                WorkEnvironment = "Flexible",
                IsActive = true
            },
            new()
            {
                Title = "Backend Developer",
                Category = "Software Development",
                CategoryId = categoryMap.GetValueOrDefault("Software Development"),
                Summary = "Engineer mission-critical server-side infrastructure, database models, business logic, and high-volume APIs.",
                Description = "Backend Developers focus on server-side architecture, data pipelines, authorization protocols, and API scalability. They ensure applications run fast, reliably, and securely under heavy concurrent traffic.",
                Responsibilities = "• Design and build secure RESTful and gRPC web services.\n• Implement complex database transactions, caching strategies, and indexing.\n• Oversee security compliance, authentication tokens, and rate limiting.\n• Monitor server telemetry, latency metrics, and system logging.",
                RequiredEducation = "Bachelor's Degree in Computer Science, Information Systems, or related field.",
                ExperienceLevel = "Mid-Level",
                ExperienceRequirements = "2-4 years of backend API and relational database development experience.",
                SuggestedLearningTopics = "Distributed Systems, C# / Java / Python, Database Indexing, Redis Caching, Microservices, CI/CD.",
                SalaryMin = 90000,
                SalaryMax = 140000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Progress to Principal Backend Architect, Cloud Architect, or Chief Technology Officer.",
                DemandLevel = "High",
                WorkEnvironment = "Hybrid",
                IsActive = true
            },
            new()
            {
                Title = "Full Stack Developer",
                Category = "Software Development",
                CategoryId = categoryMap.GetValueOrDefault("Software Development"),
                Summary = "Deliver end-to-end features spanning modern user interfaces, backend services, and relational database layers.",
                Description = "Full Stack Developers master the entire web stack, from frontend user interaction (HTML/CSS/JS/React) to backend business logic (.NET/Java/Python) and database modeling. They deliver complete features autonomously.",
                Responsibilities = "• Build responsive frontend views and bind them to server-side controllers and APIs.\n• Implement database entities, migrations, and repository access layers.\n• Configure authentication, cookie sessions, and secure authorization rules.\n• Coordinate end-to-end deployment scripts and automated testing suites.",
                RequiredEducation = "Bachelor's Degree in Computer Science or Software Engineering.",
                ExperienceLevel = "Mid-Level",
                ExperienceRequirements = "2-4 years across both client-side and server-side production codebases.",
                SuggestedLearningTopics = "ASP.NET Core MVC, React/TypeScript, SQL, REST APIs, Git, Docker, System Design.",
                SalaryMin = 92000,
                SalaryMax = 145000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Advance to Full Stack Lead, Technical Product Manager, or Startup CTO.",
                DemandLevel = "Very High",
                WorkEnvironment = "Hybrid",
                IsActive = true
            },
            new()
            {
                Title = "Data Analyst",
                Category = "Data Science",
                CategoryId = categoryMap.GetValueOrDefault("Data Science"),
                Summary = "Inspect, clean, and model datasets to produce executive dashboards, trends, and business intelligence.",
                Description = "Data Analysts bridge raw corporate datasets with business strategy. Using SQL queries, spreadsheet models, and BI visualization dashboards (Power BI/Tableau), they uncover performance trends and guide leadership decisions.",
                Responsibilities = "• Query relational and warehouse databases to extract analytical datasets.\n• Design interactive executive dashboards in Power BI and Tableau.\n• Perform exploratory data analysis and statistical validation in Python/Excel.\n• Synthesize complex quantitative findings into clear business presentations.",
                RequiredEducation = "Bachelor's Degree in Data Analytics, Statistics, Computer Science, or Business.",
                ExperienceLevel = "Entry-Level",
                ExperienceRequirements = "0-2 years of data querying, spreadsheet modeling, and dashboard creation experience.",
                SuggestedLearningTopics = "Advanced SQL, Power BI, Python (Pandas), Data Storytelling, Statistical Methods.",
                SalaryMin = 68000,
                SalaryMax = 105000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Advance to Senior BI Analyst, Analytics Manager, or Data Scientist.",
                DemandLevel = "High",
                WorkEnvironment = "Hybrid",
                IsActive = true
            },
            new()
            {
                Title = "Data Scientist",
                Category = "Data Science",
                CategoryId = categoryMap.GetValueOrDefault("Data Science"),
                Summary = "Apply statistical modeling, machine learning, and deep exploratory analysis to extract predictive value from data.",
                Description = "Data Scientists develop statistical algorithms, predictive classifiers, and regression models. They formulate hypotheses, engineer features from messy inputs, and deploy mathematical models into production workflows.",
                Responsibilities = "• Formulate mathematical models to predict user behavior and operational patterns.\n• Clean, normalize, and engineer features from heterogeneous data lakes.\n• Train and evaluate machine learning models using scikit-learn and Python.\n• Present predictive insights to stakeholders with actionable recommendations.",
                RequiredEducation = "Master's or Bachelor's Degree in Data Science, Statistics, Mathematics, or Computer Science.",
                ExperienceLevel = "Mid-Level",
                ExperienceRequirements = "2-4 years of statistical analysis and predictive modeling experience.",
                SuggestedLearningTopics = "Statistical Inference, Python (NumPy/Pandas), Scikit-Learn, Feature Engineering, SQL.",
                SalaryMin = 95000,
                SalaryMax = 150000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Progress to Lead Data Scientist, Head of AI Research, or Chief Data Officer.",
                DemandLevel = "High",
                WorkEnvironment = "Remote",
                IsActive = true
            },
            new()
            {
                Title = "Machine Learning Engineer",
                Category = "Data Science",
                CategoryId = categoryMap.GetValueOrDefault("Data Science"),
                Summary = "Design, train, and deploy production-scale neural networks and machine learning inference services.",
                Description = "Machine Learning Engineers take prototype models from data science notebooks and turn them into scalable, real-time production inference services using TensorFlow, PyTorch, Docker, and distributed GPU clusters.",
                Responsibilities = "• Architect distributed ML pipelines for automated model retraining.\n• Deploy high-throughput, low-latency model inference APIs.\n• Monitor model drift, data degradation, and prediction accuracy in production.\n• Optimize tensor operations and model quantization for production servers.",
                RequiredEducation = "Master's or Bachelor's Degree in Computer Science, Artificial Intelligence, or Robotics.",
                ExperienceLevel = "Mid-Level",
                ExperienceRequirements = "2-4 years of software engineering combined with hands-on ML framework experience.",
                SuggestedLearningTopics = "PyTorch, TensorFlow, MLOps, Docker/Kubernetes, Python, Model Serving (Triton/FastAPI).",
                SalaryMin = 110000,
                SalaryMax = 165000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Advance to Principal ML Architect, Director of AI Systems, or VP of Applied AI.",
                DemandLevel = "Very High",
                WorkEnvironment = "Hybrid",
                IsActive = true
            },
            new()
            {
                Title = "Cloud Engineer",
                Category = "Cloud Computing",
                CategoryId = categoryMap.GetValueOrDefault("Cloud Computing"),
                Summary = "Architect, secure, and maintain scalable infrastructure on cloud platforms such as AWS and Microsoft Azure.",
                Description = "Cloud Engineers design enterprise landing zones, cloud networks, compute instances, serverless functions, and storage tiers. They champion high availability, disaster recovery, and cost efficiency in the cloud.",
                Responsibilities = "• Provision cloud infrastructure using Infrastructure as Code (Terraform, Bicep, CloudFormation).\n• Manage Virtual Private Clouds (VPCs), subnets, routing tables, and security groups.\n• Implement automated backup policies and failover mechanisms.\n• Continuously audit cloud usage to optimize performance and monthly spending.",
                RequiredEducation = "Bachelor's Degree in Information Technology, Computer Science, or equivalent certifications.",
                ExperienceLevel = "Mid-Level",
                ExperienceRequirements = "2-4 years managing cloud environments on AWS or Azure.",
                SuggestedLearningTopics = "AWS/Azure Solutions Architect, Terraform, Linux Systems, Cloud Networking, IAM.",
                SalaryMin = 95000,
                SalaryMax = 145000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Advance to Principal Cloud Architect, Director of Infrastructure, or VP of Cloud Engineering.",
                DemandLevel = "Very High",
                WorkEnvironment = "Remote",
                IsActive = true
            },
            new()
            {
                Title = "DevOps Engineer",
                Category = "Cloud Computing",
                CategoryId = categoryMap.GetValueOrDefault("Cloud Computing"),
                Summary = "Bridge software development and IT operations through automated CI/CD pipelines, containerization, and monitoring.",
                Description = "DevOps Engineers automate software delivery from developer git commit to production deployment. They maintain Docker containers, Kubernetes clusters, CI/CD workflows, and telemetry observability stacks.",
                Responsibilities = "• Build and optimize continuous integration and continuous deployment (CI/CD) pipelines.\n• Orchestrate containerized microservices using Kubernetes and Helm.\n• Maintain infrastructure telemetry (Prometheus, Grafana, OpenTelemetry).\n• Promote security automation (DevSecOps) and zero-downtime rolling releases.",
                RequiredEducation = "Bachelor's Degree in Computer Science, Systems Engineering, or related technical field.",
                ExperienceLevel = "Mid-Level",
                ExperienceRequirements = "2-4 years in DevOps, CI/CD automation, and Linux server management.",
                SuggestedLearningTopics = "Docker, Kubernetes, GitHub Actions / Azure DevOps, Linux Shell, Terraform, Observability.",
                SalaryMin = 98000,
                SalaryMax = 150000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Transition to Site Reliability Engineering (SRE) Director or Head of Platform Engineering.",
                DemandLevel = "Very High",
                WorkEnvironment = "Remote",
                IsActive = true
            },
            new()
            {
                Title = "Cybersecurity Analyst",
                Category = "Cybersecurity",
                CategoryId = categoryMap.GetValueOrDefault("Cybersecurity"),
                Summary = "Protect enterprise networks, endpoints, and digital identities against cyber intrusions and vulnerabilities.",
                Description = "Cybersecurity Analysts monitor SIEM systems, analyze network packet captures, remediate security vulnerabilities, and formulate incidence response strategies to defend organizations against evolving cyber threats.",
                Responsibilities = "• Monitor enterprise security operations center (SOC) alerts and investigate anomalies.\n• Perform vulnerability scans, threat assessments, and penetration testing.\n• Enforce access controls, multi-factor authentication, and endpoint security agents.\n• Formulate security incident response plans and disaster mitigation drills.",
                RequiredEducation = "Bachelor's Degree in Cybersecurity, Information Assurance, or Computer Science.",
                ExperienceLevel = "Entry-Level",
                ExperienceRequirements = "1-3 years in network administration or security operations.",
                SuggestedLearningTopics = "Network Security Protocols, SIEM / SOC Tools, Penetration Testing, CompTIA Security+, Incident Response.",
                SalaryMin = 82000,
                SalaryMax = 130000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Advance to Chief Information Security Officer (CISO), Security Architect, or Lead Penetration Tester.",
                DemandLevel = "Very High",
                WorkEnvironment = "Hybrid",
                IsActive = true
            },
            new()
            {
                Title = "Database Administrator",
                Category = "Software Development",
                CategoryId = categoryMap.GetValueOrDefault("Software Development"),
                Summary = "Oversee database integrity, storage allocation, query indexing, replication, and high-availability backup systems.",
                Description = "Database Administrators (DBAs) guarantee relational and distributed database clusters operate at peak performance and never lose customer data. They configure replication, index tuning, transaction recovery, and user permission hierarchies.",
                Responsibilities = "• Install, patch, and monitor database servers (SQL Server, PostgreSQL, SQLite, Oracle).\n• Analyze slow query execution plans and create targeted indexes.\n• Plan, test, and execute automated point-in-time database backups and restore drills.\n• Manage database user credentials, table permissions, and encryption-at-rest.",
                RequiredEducation = "Bachelor's Degree in Information Technology, Computer Science, or Database Engineering.",
                ExperienceLevel = "Mid-Level",
                ExperienceRequirements = "2-5 years administering relational database engines in production.",
                SuggestedLearningTopics = "Database Indexing, Query Plan Analysis, High Availability (AlwaysOn), PostgreSQL / SQL Server, Backup Scripting.",
                SalaryMin = 85000,
                SalaryMax = 130000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Advance to Principal Data Architect, VP of Data Infrastructure, or Database Operations Manager.",
                DemandLevel = "Moderate",
                WorkEnvironment = "Hybrid",
                IsActive = true
            },
            new()
            {
                Title = "UI/UX Designer",
                Category = "Design",
                CategoryId = categoryMap.GetValueOrDefault("Design"),
                Summary = "Conduct user research, design wireframes, craft design systems, and deliver delightful digital user experiences.",
                Description = "UI/UX Designers solve customer friction through rigorous usability research, wireframing, interactive prototyping, and beautiful UI component design. They ensure products are both visually pleasing and intuitive to navigate.",
                Responsibilities = "• Conduct user interviews, create personas, and map out end-to-end user journeys.\n• Build low and high-fidelity wireframes and interactive prototypes in Figma.\n• Develop cohesive design systems with typographic scales, color palettes, and components.\n• Collaborate with frontend developers to ensure design fidelity in implemented web code.",
                RequiredEducation = "Bachelor's Degree in Graphic Design, Human-Computer Interaction (HCI), or equivalent portfolio.",
                ExperienceLevel = "Entry-Level",
                ExperienceRequirements = "0-2 years of UI/UX design experience, demonstrated through an active design portfolio.",
                SuggestedLearningTopics = "Figma, User Research Methods, Design Systems, Usability Testing, Web Accessibility (WCAG).",
                SalaryMin = 72000,
                SalaryMax = 115000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Advance to Lead Product Designer, Head of Design, or Director of User Experience.",
                DemandLevel = "High",
                WorkEnvironment = "Remote",
                IsActive = true
            },
            new()
            {
                Title = "Business Analyst",
                Category = "Business and Management",
                CategoryId = categoryMap.GetValueOrDefault("Business and Management"),
                Summary = "Analyze corporate workflows, gather software requirements, and align technical solutions with business goals.",
                Description = "Business Analysts serve as the essential bridge between commercial stakeholders and technical engineering squads. They document operational requirements, map process diagrams, and ensure software implementations yield measurable ROI.",
                Responsibilities = "• Elicit and document business requirements, user stories, and acceptance criteria.\n• Map existing business processes and identify automation or efficiency gains.\n• Facilitate alignment workshops between department heads and software architects.\n• Analyze operational metrics to validate project outcomes and business impact.",
                RequiredEducation = "Bachelor's Degree in Business Administration, Information Systems, or related field.",
                ExperienceLevel = "Entry-Level",
                ExperienceRequirements = "1-3 years in business analysis, requirements gathering, or operations.",
                SuggestedLearningTopics = "Agile / Scrum Methodologies, BPMN Process Modeling, SQL Queries, Requirements Elicitation, Jira.",
                SalaryMin = 74000,
                SalaryMax = 118000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Advance to Senior Business Analyst, Product Owner, or Management Consultant.",
                DemandLevel = "High",
                WorkEnvironment = "Hybrid",
                IsActive = true
            },
            new()
            {
                Title = "Product Manager",
                Category = "Business and Management",
                CategoryId = categoryMap.GetValueOrDefault("Business and Management"),
                Summary = "Lead product vision, prioritize roadmap backlogs, and drive cross-functional execution from inception to market.",
                Description = "Product Managers own the 'Why' and 'What' of software applications. They assess market needs, balance user pain points with technical feasibility, prioritize backlog sprints, and drive go-to-market strategies.",
                Responsibilities = "• Define long-term product vision, strategy, and measurable quarterly OKRs.\n• Prioritize feature backlogs in close partnership with design and engineering leads.\n• Conduct competitor benchmarking and customer discovery interviews.\n• Track product KPIs (activation, retention, churn) and iterate based on metrics.",
                RequiredEducation = "Bachelor's Degree in Business, Engineering, or Computer Science; MBA is a plus.",
                ExperienceLevel = "Mid-Level",
                ExperienceRequirements = "2-5 years in product management, technical project management, or UX strategy.",
                SuggestedLearningTopics = "Product Strategy, Agile / Scrum, User Discovery, Analytics & Metrics, Roadmapping.",
                SalaryMin = 95000,
                SalaryMax = 155000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Progress to Group Product Manager, VP of Product, or Chief Product Officer (CPO).",
                DemandLevel = "Very High",
                WorkEnvironment = "Hybrid",
                IsActive = true
            },
            new()
            {
                Title = "Digital Marketing Specialist",
                Category = "Marketing",
                CategoryId = categoryMap.GetValueOrDefault("Marketing"),
                Summary = "Drive customer acquisition and brand visibility across organic search, paid channels, and content funnels.",
                Description = "Digital Marketing Specialists manage omnichannel digital growth engines. They execute SEO strategies, pay-per-click ad campaigns, content marketing, and email conversion funnels while tracking CAC and LTV metrics.",
                Responsibilities = "• Design and execute search engine optimization (SEO) and content marketing strategies.\n• Manage paid campaigns across Google Ads, LinkedIn, and social media channels.\n• Analyze web traffic patterns and user conversion funnels in Google Analytics.\n• Coordinate email marketing sequences and customer lifecycle retention messaging.",
                RequiredEducation = "Bachelor's Degree in Marketing, Communications, Business, or equivalent experience.",
                ExperienceLevel = "Entry-Level",
                ExperienceRequirements = "1-3 years in digital marketing, content creation, or SEO management.",
                SuggestedLearningTopics = "Technical SEO, Google Analytics 4, Content Strategy, PPC Advertising, Copywriting.",
                SalaryMin = 60000,
                SalaryMax = 95000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Advance to Digital Marketing Director, Growth Lead, or Chief Marketing Officer (CMO).",
                DemandLevel = "High",
                WorkEnvironment = "Remote",
                IsActive = true
            },
            new()
            {
                Title = "Financial Analyst",
                Category = "Finance",
                CategoryId = categoryMap.GetValueOrDefault("Finance"),
                Summary = "Evaluate financial performance, build valuation models, and prepare fiscal forecasts to advise business investments.",
                Description = "Financial Analysts assess corporate balance sheets, revenue streams, and market variables. Using advanced financial models, discounted cash flow (DCF) formulas, and statistical metrics, they advise capital allocation.",
                Responsibilities = "• Build financial forecasting, budgeting, and DCF valuation models in Excel.\n• Perform quarterly variance analysis comparing actual revenues against forecasts.\n• Prepare executive briefing packs for corporate treasury and investment boards.\n• Monitor macroeconomic trends, interest rate movements, and sector valuations.",
                RequiredEducation = "Bachelor's Degree in Finance, Accounting, Economics, or Mathematics; CFA track is valued.",
                ExperienceLevel = "Entry-Level",
                ExperienceRequirements = "1-3 years in financial analysis, banking, or corporate accounting.",
                SuggestedLearningTopics = "Financial Statement Analysis, DCF Valuation, Excel Modeling, Corporate Finance, Power BI.",
                SalaryMin = 72000,
                SalaryMax = 115000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Advance to Senior Financial Analyst, Portfolio Manager, Director of Finance, or CFO.",
                DemandLevel = "High",
                WorkEnvironment = "Hybrid",
                IsActive = true
            },
            new()
            {
                Title = "QA Engineer",
                Category = "Software Development",
                CategoryId = categoryMap.GetValueOrDefault("Software Development"),
                Summary = "Design automated test suites, execute regression checks, and verify software quality and performance standards.",
                Description = "Quality Assurance (QA) Engineers build automated test frameworks, regression suites, and load test scripts. They collaborate closely with developers to catch software bugs before release and ensure high reliability.",
                Responsibilities = "• Author detailed test plans, test cases, and quality acceptance criteria.\n• Develop automated test scripts using frameworks like Selenium, Cypress, or xUnit.\n• Execute automated regression, performance, and API test suites.\n• Track and document bugs with reproducible steps in issue trackers.",
                RequiredEducation = "Bachelor's Degree in Computer Science, Software Engineering, or equivalent testing experience.",
                ExperienceLevel = "Entry-Level",
                ExperienceRequirements = "1-2 years in software quality assurance or automated testing.",
                SuggestedLearningTopics = "Test Automation (Selenium/Playwright), API Testing (Postman/REST), CI/CD Integration, C# / Python.",
                SalaryMin = 68000,
                SalaryMax = 110000,
                SalaryCurrency = "USD",
                SalaryNotes = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.",
                GrowthOpportunities = "Advance to Lead QA Automation Engineer, Director of Quality Engineering, or DevOps Engineer.",
                DemandLevel = "High",
                WorkEnvironment = "Hybrid",
                IsActive = true
            }
        };

        context.Careers.AddRange(careers);
        await context.SaveChangesAsync();

        // Now link CareerSkills with Priority and Requirement flags
        var careerSkillMappings = new Dictionary<string, List<(string SkillName, bool IsReq, string Priority, string MinProf)>>
        {
            ["Software Developer"] = new()
            {
                ("C#", true, "High", "Intermediate"),
                ("SQL", true, "High", "Intermediate"),
                ("Git", true, "High", "Intermediate"),
                ("Problem Solving", true, "High", "Intermediate"),
                ("REST APIs", false, "Medium", "Beginner"),
                ("Team Collaboration", true, "Medium", "Intermediate")
            },
            [".NET Developer"] = new()
            {
                ("C#", true, "High", "Advanced"),
                (".NET", true, "High", "Advanced"),
                ("ASP.NET Core", true, "High", "Intermediate"),
                ("Entity Framework Core", true, "High", "Intermediate"),
                ("SQL", true, "High", "Intermediate"),
                ("REST APIs", true, "Medium", "Intermediate"),
                ("Problem Solving", true, "High", "Advanced")
            },
            ["Java Developer"] = new()
            {
                ("Java", true, "High", "Advanced"),
                ("Spring Boot", true, "High", "Intermediate"),
                ("SQL", true, "High", "Intermediate"),
                ("REST APIs", true, "Medium", "Intermediate"),
                ("Git", true, "Medium", "Intermediate"),
                ("Problem Solving", true, "High", "Intermediate")
            },
            ["Python Developer"] = new()
            {
                ("Python", true, "High", "Advanced"),
                ("SQL", true, "High", "Intermediate"),
                ("REST APIs", true, "High", "Intermediate"),
                ("Git", true, "Medium", "Intermediate"),
                ("Docker", false, "Medium", "Beginner"),
                ("Problem Solving", true, "High", "Intermediate")
            },
            ["Frontend Developer"] = new()
            {
                ("HTML5 & CSS3", true, "High", "Advanced"),
                ("JavaScript", true, "High", "Advanced"),
                ("TypeScript", true, "High", "Intermediate"),
                ("React", true, "High", "Intermediate"),
                ("UI/UX Design", false, "Medium", "Beginner"),
                ("Attention to Detail", true, "High", "Intermediate")
            },
            ["Backend Developer"] = new()
            {
                ("C#", false, "High", "Intermediate"),
                ("Python", false, "High", "Intermediate"),
                ("SQL", true, "High", "Advanced"),
                ("PostgreSQL", true, "High", "Intermediate"),
                ("REST APIs", true, "High", "Advanced"),
                ("Docker", true, "Medium", "Intermediate"),
                ("Problem Solving", true, "High", "Advanced")
            },
            ["Full Stack Developer"] = new()
            {
                ("JavaScript", true, "High", "Advanced"),
                ("React", true, "High", "Intermediate"),
                ("C#", false, "High", "Intermediate"),
                ("ASP.NET Core", false, "High", "Intermediate"),
                ("SQL", true, "High", "Intermediate"),
                ("REST APIs", true, "High", "Intermediate"),
                ("Git", true, "High", "Intermediate")
            },
            ["Data Analyst"] = new()
            {
                ("SQL", true, "High", "Advanced"),
                ("Power BI & Tableau", true, "High", "Intermediate"),
                ("Pandas & NumPy", true, "Medium", "Intermediate"),
                ("Python", false, "Medium", "Beginner"),
                ("Critical Thinking", true, "High", "Advanced"),
                ("Communication", true, "High", "Intermediate")
            },
            ["Data Scientist"] = new()
            {
                ("Python", true, "High", "Advanced"),
                ("Machine Learning", true, "High", "Advanced"),
                ("Pandas & NumPy", true, "High", "Advanced"),
                ("SQL", true, "High", "Intermediate"),
                ("TensorFlow / PyTorch", true, "Medium", "Intermediate"),
                ("Critical Thinking", true, "High", "Advanced")
            },
            ["Machine Learning Engineer"] = new()
            {
                ("Python", true, "High", "Advanced"),
                ("Machine Learning", true, "High", "Advanced"),
                ("TensorFlow / PyTorch", true, "High", "Advanced"),
                ("Docker", true, "High", "Intermediate"),
                ("REST APIs", true, "Medium", "Intermediate"),
                ("Problem Solving", true, "High", "Advanced")
            },
            ["Cloud Engineer"] = new()
            {
                ("AWS", true, "High", "Intermediate"),
                ("Microsoft Azure", true, "High", "Intermediate"),
                ("Linux", true, "High", "Intermediate"),
                ("Docker", true, "High", "Intermediate"),
                ("CI/CD Pipelines", true, "Medium", "Intermediate"),
                ("Problem Solving", true, "High", "Intermediate")
            },
            ["DevOps Engineer"] = new()
            {
                ("Docker", true, "High", "Advanced"),
                ("Kubernetes", true, "High", "Intermediate"),
                ("CI/CD Pipelines", true, "High", "Advanced"),
                ("Linux", true, "High", "Advanced"),
                ("AWS", true, "Medium", "Intermediate"),
                ("Git", true, "High", "Advanced")
            },
            ["Cybersecurity Analyst"] = new()
            {
                ("Network Security", true, "High", "Advanced"),
                ("Penetration Testing", true, "High", "Intermediate"),
                ("Linux", true, "High", "Intermediate"),
                ("Attention to Detail", true, "High", "Advanced"),
                ("Critical Thinking", true, "High", "Advanced"),
                ("Problem Solving", true, "High", "Advanced")
            },
            ["Database Administrator"] = new()
            {
                ("SQL", true, "High", "Advanced"),
                ("Database Administration", true, "High", "Advanced"),
                ("PostgreSQL", true, "High", "Intermediate"),
                ("Linux", true, "Medium", "Intermediate"),
                ("Attention to Detail", true, "High", "Advanced"),
                ("Problem Solving", true, "High", "Advanced")
            },
            ["UI/UX Designer"] = new()
            {
                ("UI/UX Design", true, "High", "Advanced"),
                ("Figma", true, "High", "Advanced"),
                ("HTML5 & CSS3", false, "Medium", "Beginner"),
                ("Attention to Detail", true, "High", "Advanced"),
                ("Communication", true, "High", "Advanced"),
                ("Critical Thinking", true, "Medium", "Intermediate")
            },
            ["Business Analyst"] = new()
            {
                ("Business Process Modeling", true, "High", "Advanced"),
                ("Agile / Scrum", true, "High", "Intermediate"),
                ("SQL", false, "Medium", "Beginner"),
                ("Communication", true, "High", "Advanced"),
                ("Critical Thinking", true, "High", "Advanced"),
                ("Team Collaboration", true, "High", "Advanced")
            },
            ["Product Manager"] = new()
            {
                ("Agile / Scrum", true, "High", "Advanced"),
                ("Business Process Modeling", true, "High", "Intermediate"),
                ("Leadership", true, "High", "Advanced"),
                ("Communication", true, "High", "Advanced"),
                ("Critical Thinking", true, "High", "Advanced"),
                ("Time Management", true, "High", "Intermediate")
            },
            ["Digital Marketing Specialist"] = new()
            {
                ("SEO & Content Strategy", true, "High", "Advanced"),
                ("Google Analytics", true, "High", "Advanced"),
                ("Communication", true, "High", "Advanced"),
                ("Critical Thinking", true, "Medium", "Intermediate"),
                ("Attention to Detail", true, "Medium", "Intermediate")
            },
            ["Financial Analyst"] = new()
            {
                ("Financial Modeling", true, "High", "Advanced"),
                ("Critical Thinking", true, "High", "Advanced"),
                ("Attention to Detail", true, "High", "Advanced"),
                ("Communication", true, "High", "Intermediate"),
                ("SQL", false, "Medium", "Beginner")
            },
            ["QA Engineer"] = new()
            {
                ("Automated Testing", true, "High", "Advanced"),
                ("SQL", true, "High", "Intermediate"),
                ("Git", true, "Medium", "Intermediate"),
                ("Attention to Detail", true, "High", "Advanced"),
                ("Problem Solving", true, "High", "Intermediate"),
                ("Team Collaboration", true, "Medium", "Intermediate")
            }
        };

        var loadedCareers = await context.Careers.ToListAsync();
        foreach (var c in loadedCareers)
        {
            if (careerSkillMappings.TryGetValue(c.Title, out var skillsList))
            {
                foreach (var item in skillsList)
                {
                    if (skillMap.TryGetValue(item.SkillName, out var skillId))
                    {
                        context.CareerSkills.Add(new CareerSkill
                        {
                            CareerId = c.Id,
                            SkillId = skillId,
                            IsRequired = item.IsReq,
                            Priority = item.Priority,
                            MinProficiency = item.MinProf
                        });
                    }
                }
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedAssessmentQuestionsAsync(ApplicationDbContext context)
    {
        var questions = new List<Question>
        {
            // Section A: Educational Background
            new()
            {
                SectionKey = "Education",
                SectionTitle = "Section A: Educational Background",
                QuestionText = "What is your highest level of completed or current education?",
                QuestionType = "SingleChoice",
                HelpText = "Educational qualification provides baseline alignment for technical and enterprise roles.",
                OrderNumber = 1,
                IsRequired = true,
                Options = new List<QuestionOption>
                {
                    new() { OptionText = "High School Diploma / GED", OptionValue = "High School", CategoryMapping = "General", ScoreWeight = 60, OrderNumber = 1 },
                    new() { OptionText = "Associate Degree / Technical Diploma", OptionValue = "Associate", CategoryMapping = "General", ScoreWeight = 75, OrderNumber = 2 },
                    new() { OptionText = "Bachelor's Degree (CS, IT, Engineering, or related)", OptionValue = "Bachelor's Degree", CategoryMapping = "Software Development", ScoreWeight = 95, OrderNumber = 3 },
                    new() { OptionText = "Bachelor's Degree (Business, Finance, or Humanities)", OptionValue = "Bachelor's Business", CategoryMapping = "Business and Management", ScoreWeight = 85, OrderNumber = 4 },
                    new() { OptionText = "Master's Degree / Doctorate (STEM or MBA)", OptionValue = "Master's Degree", CategoryMapping = "Data Science", ScoreWeight = 100, OrderNumber = 5 }
                }
            },
            new()
            {
                SectionKey = "Education",
                SectionTitle = "Section A: Educational Background",
                QuestionText = "Which primary field of study best reflects your academic coursework?",
                QuestionType = "SingleChoice",
                HelpText = "Select the domain closest to your major, concentration, or self-directed coursework.",
                OrderNumber = 2,
                IsRequired = true,
                Options = new List<QuestionOption>
                {
                    new() { OptionText = "Computer Science / Software Engineering / IT", OptionValue = "CS_IT", CategoryMapping = "Software Development", ScoreWeight = 100, OrderNumber = 1 },
                    new() { OptionText = "Data Analytics / Mathematics / Statistics", OptionValue = "Data_Math", CategoryMapping = "Data Science", ScoreWeight = 100, OrderNumber = 2 },
                    new() { OptionText = "Information Security / Computer Networking", OptionValue = "Infosec", CategoryMapping = "Cybersecurity", ScoreWeight = 100, OrderNumber = 3 },
                    new() { OptionText = "Business Administration / Project Management", OptionValue = "Business", CategoryMapping = "Business and Management", ScoreWeight = 100, OrderNumber = 4 },
                    new() { OptionText = "Finance / Accounting / Economics", OptionValue = "Finance", CategoryMapping = "Finance", ScoreWeight = 100, OrderNumber = 5 },
                    new() { OptionText = "Graphic Design / Human-Computer Interaction", OptionValue = "Design", CategoryMapping = "Design", ScoreWeight = 100, OrderNumber = 6 }
                }
            },

            // Section B: Technical Skills
            new()
            {
                SectionKey = "TechnicalSkills",
                SectionTitle = "Section B: Technical Skills",
                QuestionText = "Which programming languages and core development technologies are you proficient in?",
                QuestionType = "MultiSelect",
                HelpText = "Select all languages and stacks in which you can write working, testable code.",
                OrderNumber = 3,
                IsRequired = true,
                Options = new List<QuestionOption>
                {
                    new() { OptionText = "C# and .NET ecosystem", OptionValue = "C#", SkillTag = "C#", CategoryMapping = "Software Development", ScoreWeight = 100, OrderNumber = 1 },
                    new() { OptionText = "Java and Spring Boot", OptionValue = "Java", SkillTag = "Java", CategoryMapping = "Software Development", ScoreWeight = 100, OrderNumber = 2 },
                    new() { OptionText = "Python (Scripting, Data, or Web)", OptionValue = "Python", SkillTag = "Python", CategoryMapping = "Data Science", ScoreWeight = 100, OrderNumber = 3 },
                    new() { OptionText = "JavaScript / TypeScript (React / Node)", OptionValue = "JavaScript", SkillTag = "JavaScript", CategoryMapping = "Software Development", ScoreWeight = 100, OrderNumber = 4 },
                    new() { OptionText = "SQL (Querying, Joins, Relational Schemas)", OptionValue = "SQL", SkillTag = "SQL", CategoryMapping = "Software Development", ScoreWeight = 100, OrderNumber = 5 },
                    new() { OptionText = "HTML5, CSS3, Responsive Design", OptionValue = "HTML5 & CSS3", SkillTag = "HTML5 & CSS3", CategoryMapping = "Software Development", ScoreWeight = 90, OrderNumber = 6 }
                }
            },
            new()
            {
                SectionKey = "TechnicalSkills",
                SectionTitle = "Section B: Technical Skills",
                QuestionText = "Which infrastructure, cloud, and tooling platforms do you have hands-on experience with?",
                QuestionType = "MultiSelect",
                HelpText = "Select all tools you have utilized in academic, professional, or personal projects.",
                OrderNumber = 4,
                IsRequired = false,
                Options = new List<QuestionOption>
                {
                    new() { OptionText = "Git and GitHub / GitLab workflows", OptionValue = "Git", SkillTag = "Git", CategoryMapping = "Software Development", ScoreWeight = 90, OrderNumber = 1 },
                    new() { OptionText = "Docker containerization and Kubernetes", OptionValue = "Docker", SkillTag = "Docker", CategoryMapping = "Cloud Computing", ScoreWeight = 100, OrderNumber = 2 },
                    new() { OptionText = "Cloud Platforms (AWS, Azure, or Google Cloud)", OptionValue = "Cloud", SkillTag = "AWS", CategoryMapping = "Cloud Computing", ScoreWeight = 100, OrderNumber = 3 },
                    new() { OptionText = "CI/CD automated deployment pipelines", OptionValue = "CI/CD", SkillTag = "CI/CD Pipelines", CategoryMapping = "Cloud Computing", ScoreWeight = 95, OrderNumber = 4 },
                    new() { OptionText = "Machine Learning libraries (Scikit-Learn, PyTorch)", OptionValue = "ML", SkillTag = "Machine Learning", CategoryMapping = "Data Science", ScoreWeight = 100, OrderNumber = 5 },
                    new() { OptionText = "UI Prototyping tools (Figma, Adobe XD)", OptionValue = "Figma", SkillTag = "Figma", CategoryMapping = "Design", ScoreWeight = 100, OrderNumber = 6 }
                }
            },

            // Section C: Soft Skills
            new()
            {
                SectionKey = "SoftSkills",
                SectionTitle = "Section C: Soft Skills",
                QuestionText = "Which interpersonal and workplace competencies are your strongest strengths?",
                QuestionType = "MultiSelect",
                HelpText = "Select 2 to 4 core strengths that describe your professional working style.",
                OrderNumber = 5,
                IsRequired = true,
                Options = new List<QuestionOption>
                {
                    new() { OptionText = "Analytical Problem Solving and Root Cause Investigation", OptionValue = "Problem Solving", SkillTag = "Problem Solving", CategoryMapping = "General", ScoreWeight = 100, OrderNumber = 1 },
                    new() { OptionText = "Cross-Functional Communication and Presentation", OptionValue = "Communication", SkillTag = "Communication", CategoryMapping = "Business and Management", ScoreWeight = 100, OrderNumber = 2 },
                    new() { OptionText = "Cooperative Teamwork and Collaborative Code/Design Reviews", OptionValue = "Team Collaboration", SkillTag = "Team Collaboration", CategoryMapping = "General", ScoreWeight = 100, OrderNumber = 3 },
                    new() { OptionText = "Leadership, Mentorship, and Initiative", OptionValue = "Leadership", SkillTag = "Leadership", CategoryMapping = "Business and Management", ScoreWeight = 100, OrderNumber = 4 },
                    new() { OptionText = "High Attention to Detail and Meticulous Testing", OptionValue = "Attention to Detail", SkillTag = "Attention to Detail", CategoryMapping = "Cybersecurity", ScoreWeight = 100, OrderNumber = 5 },
                    new() { OptionText = "Fast Adaptability to Emerging Stacks and Shifting Requirements", OptionValue = "Adaptability", SkillTag = "Adaptability", CategoryMapping = "General", ScoreWeight = 100, OrderNumber = 6 }
                }
            },

            // Section D: Personal Interests
            new()
            {
                SectionKey = "Interests",
                SectionTitle = "Section D: Personal Interests",
                QuestionText = "Which core technical or business activity excites you the most?",
                QuestionType = "SingleChoice",
                HelpText = "Consider what kind of work you would look forward to doing every single workday.",
                OrderNumber = 6,
                IsRequired = true,
                Options = new List<QuestionOption>
                {
                    new() { OptionText = "Architecting backend software, APIs, and data models", OptionValue = "Software Development", CategoryMapping = "Software Development", ScoreWeight = 100, OrderNumber = 1 },
                    new() { OptionText = "Uncovering patterns, statistics, and training predictive AI models", OptionValue = "Data Science", CategoryMapping = "Data Science", ScoreWeight = 100, OrderNumber = 2 },
                    new() { OptionText = "Securing networks, ethical hacking, and defending against attacks", OptionValue = "Cybersecurity", CategoryMapping = "Cybersecurity", ScoreWeight = 100, OrderNumber = 3 },
                    new() { OptionText = "Automating cloud deployments, infrastructure, and server resilience", OptionValue = "Cloud Computing", CategoryMapping = "Cloud Computing", ScoreWeight = 100, OrderNumber = 4 },
                    new() { OptionText = "Shaping product strategy, sprint roadmaps, and business solutions", OptionValue = "Business and Management", CategoryMapping = "Business and Management", ScoreWeight = 100, OrderNumber = 5 },
                    new() { OptionText = "Designing intuitive user interfaces, visual design, and user flows", OptionValue = "Design", CategoryMapping = "Design", ScoreWeight = 100, OrderNumber = 6 }
                }
            },

            // Section E: Problem-Solving Abilities
            new()
            {
                SectionKey = "ProblemSolving",
                SectionTitle = "Section E: Problem-Solving Abilities",
                QuestionText = "How do you rate your comfort level with complex algorithmic debugging and system analysis?",
                QuestionType = "RatingScale",
                HelpText = "1 = Prefer clear, routine guidelines; 5 = Thrive on deep technical ambiguity, profiling, and complex logical puzzles.",
                OrderNumber = 7,
                IsRequired = true,
                Options = new List<QuestionOption>
                {
                    new() { OptionText = "1 - Prefer well-defined, structured, routine procedures", OptionValue = "1", ScoreWeight = 20, OrderNumber = 1 },
                    new() { OptionText = "2 - Comfortable following established architectural patterns", OptionValue = "2", ScoreWeight = 40, OrderNumber = 2 },
                    new() { OptionText = "3 - Capable of solving moderate algorithmic challenges with documentation", OptionValue = "3", ScoreWeight = 60, OrderNumber = 3 },
                    new() { OptionText = "4 - Strong logical troubleshooter; enjoy profiling and refactoring", OptionValue = "4", ScoreWeight = 85, OrderNumber = 4 },
                    new() { OptionText = "5 - Highly analytical; thrive on solving complex distributed bugs", OptionValue = "5", ScoreWeight = 100, OrderNumber = 5 }
                }
            },

            // Section F: Preferred Work Environment
            new()
            {
                SectionKey = "WorkEnvironment",
                SectionTitle = "Section F: Preferred Work Environment",
                QuestionText = "What is your preferred work setup and organizational environment?",
                QuestionType = "SingleChoice",
                HelpText = "Work style preference influences team fit and recommended company profiles.",
                OrderNumber = 8,
                IsRequired = true,
                Options = new List<QuestionOption>
                {
                    new() { OptionText = "Fully Remote (Autonomous asynchronous delivery)", OptionValue = "Remote", CategoryMapping = "Software Development", ScoreWeight = 100, OrderNumber = 1 },
                    new() { OptionText = "Hybrid (Balanced collaboration with focused remote days)", OptionValue = "Hybrid", CategoryMapping = "General", ScoreWeight = 100, OrderNumber = 2 },
                    new() { OptionText = "On-site / Corporate Office (Direct face-to-face team synergy)", OptionValue = "On-site", CategoryMapping = "Business and Management", ScoreWeight = 100, OrderNumber = 3 },
                    new() { OptionText = "Flexible / Contract Consultancy", OptionValue = "Flexible", CategoryMapping = "General", ScoreWeight = 95, OrderNumber = 4 }
                }
            },

            // Section G: Career Goals
            new()
            {
                SectionKey = "CareerGoals",
                SectionTitle = "Section G: Career Goals",
                QuestionText = "Where do you see your primary career progression heading in the next 3 to 5 years?",
                QuestionType = "SingleChoice",
                HelpText = "Select your desired direction of professional mastery.",
                OrderNumber = 9,
                IsRequired = true,
                Options = new List<QuestionOption>
                {
                    new() { OptionText = "Deep Technical Specialist (Staff Engineer, Architect, Principal Developer)", OptionValue = "Tech_Specialist", CategoryMapping = "Software Development", ScoreWeight = 100, OrderNumber = 1 },
                    new() { OptionText = "Data & Machine Learning Leader (Lead Data Scientist, Head of AI)", OptionValue = "Data_Leader", CategoryMapping = "Data Science", ScoreWeight = 100, OrderNumber = 2 },
                    new() { OptionText = "Engineering Management (Tech Lead, Engineering Manager, Director)", OptionValue = "Eng_Management", CategoryMapping = "Business and Management", ScoreWeight = 100, OrderNumber = 3 },
                    new() { OptionText = "Product Ownership (Product Manager, Head of Product, Founder)", OptionValue = "Product_Leader", CategoryMapping = "Business and Management", ScoreWeight = 100, OrderNumber = 4 },
                    new() { OptionText = "Cyber Defense Specialist (CISO, Security Architect, Forensics)", OptionValue = "Sec_Leader", CategoryMapping = "Cybersecurity", ScoreWeight = 100, OrderNumber = 5 }
                }
            },

            // Section H: Experience Level
            new()
            {
                SectionKey = "ExperienceLevel",
                SectionTitle = "Section H: Experience Level",
                QuestionText = "What is your total cumulative software or professional industry experience?",
                QuestionType = "SingleChoice",
                HelpText = "Includes professional internships, freelance contracts, and full-time employment.",
                OrderNumber = 10,
                IsRequired = true,
                Options = new List<QuestionOption>
                {
                    new() { OptionText = "Entry-Level (0 to 1 year, student, or recent graduate)", OptionValue = "Entry-Level", CategoryMapping = "General", ScoreWeight = 60, OrderNumber = 1 },
                    new() { OptionText = "Junior Professional (1 to 2 years of active project work)", OptionValue = "Junior", CategoryMapping = "General", ScoreWeight = 75, OrderNumber = 2 },
                    new() { OptionText = "Mid-Level Professional (3 to 5 years of production experience)", OptionValue = "Mid-Level", CategoryMapping = "General", ScoreWeight = 90, OrderNumber = 3 },
                    new() { OptionText = "Senior Professional (5+ years of production experience)", OptionValue = "Senior", CategoryMapping = "General", ScoreWeight = 100, OrderNumber = 4 }
                }
            }
        };

        context.Questions.AddRange(questions);
        await context.SaveChangesAsync();
    }
}
