### COURSE INFO
- Category: Compute
- Course Title: Amazon EC2 Fundamentals
- Course Description: Amazon Elastic Compute Cloud (EC2) provides resizable virtual servers ("instances") in the AWS cloud, letting you choose your OS, CPU, memory, storage, and networking without buying physical hardware. This course covers what EC2 is, how instance types and pricing models work, and how to secure and store data on an instance.
- Tags (comma-separated): AWS, EC2, Compute, Cloud Practitioner, Solutions Architect Associate

### LESSONS

Lesson 1:
- Title: What Is Amazon EC2?
- Video URL: (left blank — see 00_CONTENT_OUTLINE.md)
- Content (HTML/Text): <p>Amazon EC2 (Elastic Compute Cloud) is AWS's core Infrastructure-as-a-Service offering, launched in 2006. It lets you launch virtual servers, called <strong>instances</strong>, on demand, without buying or maintaining physical hardware.</p><ul><li>You choose the operating system, CPU, memory, and storage.</li><li>You pay only for what you use, typically billed per second or hour.</li><li>Instances can be resized, stopped, or terminated at any time.</li></ul>

Lesson 2:
- Title: EC2 Instance Types and Families
- Video URL:
- Content (HTML/Text): <p>Instance types are named by family, generation, and size — for example <strong>t3.micro</strong> or <strong>m5.large</strong>.</p><ul><li><strong>General purpose</strong> (T, M families) — balanced CPU/memory, good for web servers and small databases.</li><li><strong>Compute optimized</strong> (C family) — high-performance processors for CPU-intensive workloads.</li><li><strong>Memory optimized</strong> (R, X families) — large RAM for in-memory databases and caching.</li><li><strong>Storage optimized</strong> (I, D families) — high, fast local disk throughput.</li></ul><p>AWS offers a free tier for new accounts with limited EC2 usage — check AWS's current Free Tier terms, as the exact details change over time.</p>

Lesson 3:
- Title: Pricing Models: On-Demand, Reserved, and Spot
- Video URL:
- Content (HTML/Text): <p>EC2 offers several pricing models:</p><ul><li><strong>On-Demand</strong> — pay per second/hour, no commitment, most flexible.</li><li><strong>Reserved Instances</strong> — commit to 1 or 3 years for a significant discount versus On-Demand.</li><li><strong>Spot Instances</strong> — bid on AWS's spare capacity for steep discounts, but AWS can reclaim the instance with short notice.</li></ul><p>Choose based on how predictable and interruption-tolerant your workload is.</p>

Lesson 4:
- Title: Security Groups and Key Pairs
- Video URL:
- Content (HTML/Text): <p>A <strong>Security Group</strong> is a virtual firewall attached to an instance, controlling inbound and outbound traffic. It's stateful, and only supports <em>allow</em> rules — everything not explicitly allowed is denied by default.</p><p>A <strong>Key Pair</strong> is a public/private key used to connect securely to an instance (SSH for Linux, or to decrypt the Windows admin password). AWS stores the public key; you keep the private key (a <code>.pem</code> file) — if you lose it, AWS cannot recover it for you.</p>

Lesson 5:
- Title: EC2 Storage: EBS vs. Instance Store
- Video URL:
- Content (HTML/Text): <p><strong>EBS (Elastic Block Store)</strong> is network-attached, persistent storage that survives an instance stop (and can survive termination, depending on settings) and supports point-in-time snapshots.</p><p><strong>Instance Store</strong> is physically attached to the host machine — faster, but ephemeral: its data is lost when the instance is stopped or terminated. It's used for temporary data or caching, never for anything you need to keep.</p>

### QUIZ SETTINGS
- Quiz Title: EC2 Fundamentals Assessment
- Quiz Description: Test your understanding of the core EC2 concepts covered in this course.
- Time Limit (minutes): 15
- Passing Score (%): 80

### QUIZ QUESTIONS

Question 1:
- Text: What does "EC2" stand for?
- Option A: Elastic Cloud Compute
- Option B: Elastic Compute Cloud
- Option C: Enterprise Compute Cloud
- Option D: Elastic Container Compute
- Correct Option: B

Question 2:
- Text: Which feature acts as a virtual firewall controlling traffic to an EC2 instance?
- Option A: IAM Role
- Option B: Security Group
- Option C: VPC Peering
- Option D: Route Table
- Correct Option: B

Question 3:
- Text: Which EC2 pricing model offers the largest discount but can be interrupted by AWS on short notice?
- Option A: On-Demand
- Option B: Reserved Instances
- Option C: Spot Instances
- Option D: Savings Plans
- Correct Option: C

Question 4:
- Text: What happens to data on an EC2 Instance Store volume when the instance is stopped?
- Option A: It's automatically backed up to S3
- Option B: It persists indefinitely
- Option C: It is lost
- Option D: It moves to EBS automatically
- Correct Option: C

Question 5:
- Text: Which file must you keep secure yourself, since AWS cannot recover it if lost, when connecting to a Linux instance via SSH?
- Option A: The security group ID
- Option B: The AMI ID
- Option C: The private key (.pem file)
- Option D: The instance ID
- Correct Option: C
