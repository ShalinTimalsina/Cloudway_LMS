### COURSE INFO
- Category: Databases
- Course Title: Amazon RDS Fundamentals
- Course Description: Amazon Relational Database Service (RDS) is a managed database service that handles the operational overhead of running a relational database. This course covers what RDS does, how it achieves availability and read scaling, and how it compares to self-managing a database yourself.
- Tags (comma-separated): AWS, RDS, Databases, Cloud Practitioner, Solutions Architect Associate

### LESSONS

Lesson 1:
- Title: What Is Amazon RDS?
- Video URL:
- Content (HTML/Text): <p>Amazon RDS (Relational Database Service) is a managed relational database service that automates time-consuming administrative tasks — provisioning, patching, backups, and recovery. RDS supports several engines, including MySQL, PostgreSQL, MariaDB, Oracle, SQL Server, and Amazon Aurora.</p>

Lesson 2:
- Title: Multi-AZ Deployments
- Video URL:
- Content (HTML/Text): <p>An RDS <strong>Multi-AZ</strong> deployment automatically maintains a synchronous standby replica of your database in a different Availability Zone. If the primary database fails, RDS automatically fails over to the standby — this is a high-availability feature, not a way to improve read performance.</p>

Lesson 3:
- Title: Read Replicas
- Video URL:
- Content (HTML/Text): <p>A <strong>Read Replica</strong> is an asynchronous copy of a database used to offload read traffic and improve performance for read-heavy applications. Unlike a Multi-AZ standby, a Read Replica can also be promoted to a standalone, independent database if needed.</p>

Lesson 4:
- Title: Backups and Snapshots
- Video URL:
- Content (HTML/Text): <p>RDS supports two kinds of backups:</p><ul><li><strong>Automated backups</strong> — enable point-in-time recovery within a configurable retention window, and are deleted when that window passes.</li><li><strong>Manual snapshots</strong> — taken on demand and kept until you explicitly delete them, useful for longer-term recovery points.</li></ul>

Lesson 5:
- Title: RDS vs. Self-Managed Databases on EC2
- Video URL:
- Content (HTML/Text): <p>Running a database on RDS trades some control for significantly reduced operational overhead — AWS manages the underlying OS, patching, and backup infrastructure for you. Running a database yourself on an EC2 instance gives full control over configuration and the OS, but you're responsible for managing all of it, including patching and failover.</p>

### QUIZ SETTINGS
- Quiz Title: RDS Fundamentals Assessment
- Quiz Description: Test your understanding of the core Amazon RDS concepts covered in this course.
- Time Limit (minutes): 15
- Passing Score (%): 80

### QUIZ QUESTIONS

Question 1:
- Text: What type of database service is Amazon RDS?
- Option A: NoSQL database
- Option B: Managed relational database
- Option C: Data warehouse
- Option D: In-memory cache
- Correct Option: B

Question 2:
- Text: What is the primary purpose of a Multi-AZ deployment?
- Option A: Improve read performance
- Option B: Reduce storage cost
- Option C: High availability via a standby replica
- Option D: Enable NoSQL queries
- Correct Option: C

Question 3:
- Text: What is a Read Replica primarily used for?
- Option A: Disaster recovery failover
- Option B: Offloading read traffic
- Option C: Encrypting data at rest
- Option D: Managing IAM permissions
- Correct Option: B

Question 4:
- Text: Which backup type is retained until you explicitly delete it?
- Option A: Automated backup
- Option B: Manual snapshot
- Option C: Read replica
- Option D: Multi-AZ standby
- Correct Option: B

Question 5:
- Text: What is the main trade-off of using RDS instead of running a database yourself on EC2?
- Option A: RDS is always more expensive
- Option B: RDS supports fewer database engines
- Option C: You give up some control in exchange for reduced management overhead
- Option D: RDS cannot be backed up
- Correct Option: C
