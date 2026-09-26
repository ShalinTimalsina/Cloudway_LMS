### COURSE INFO
- Category: Storage
- Course Title: Amazon S3 Essentials
- Course Description: Amazon Simple Storage Service (S3) is AWS's object storage service, built for virtually unlimited scale and very high durability. This course covers how S3 organizes data, its storage classes, versioning, security controls, and static website hosting.
- Tags (comma-separated): AWS, S3, Storage, Cloud Practitioner, Solutions Architect Associate

### LESSONS

Lesson 1:
- Title: What Is Amazon S3?
- Video URL:
- Content (HTML/Text): <p>Amazon S3 (Simple Storage Service) is an object storage service — it stores data as <strong>objects</strong> (files) inside <strong>buckets</strong>, not as a traditional file system.</p><ul><li>Each object is identified by a unique key within its bucket.</li><li>Bucket names must be globally unique across all of AWS.</li><li>S3 is designed for very high durability, making it suitable for backups, static assets, and data lakes.</li></ul>

Lesson 2:
- Title: S3 Storage Classes
- Video URL:
- Content (HTML/Text): <p>S3 offers several storage classes, trading cost against retrieval speed:</p><ul><li><strong>S3 Standard</strong> — frequently accessed data, low latency.</li><li><strong>S3 Intelligent-Tiering</strong> — automatically moves objects between tiers based on access patterns.</li><li><strong>S3 Standard-IA / One Zone-IA</strong> — infrequently accessed data, cheaper storage but a retrieval fee.</li><li><strong>S3 Glacier and Glacier Deep Archive</strong> — long-term archival, the lowest storage cost, but retrieval can take minutes to hours.</li></ul>

Lesson 3:
- Title: Buckets, Objects, and Versioning
- Video URL:
- Content (HTML/Text): <p>Object keys often use prefixes (like <code>images/photo.jpg</code>) to simulate a folder structure, even though S3 has no real directory hierarchy underneath.</p><p><strong>Versioning</strong>, once enabled on a bucket, keeps every version of an object rather than overwriting it — protecting against accidental deletion or overwrite. It can be paired with MFA Delete for extra protection against permanent deletion.</p>

Lesson 4:
- Title: S3 Security and Access Control
- Video URL:
- Content (HTML/Text): <p>S3 buckets are private by default. Access is controlled through:</p><ul><li><strong>IAM policies</strong> — attached to users/roles.</li><li><strong>Bucket policies</strong> — attached directly to the bucket (resource-based).</li><li><strong>Access Control Lists (ACLs)</strong> — an older, more limited mechanism.</li></ul><p>The <strong>Block Public Access</strong> setting exists specifically to prevent a bucket from being accidentally exposed to the internet. Data can also be encrypted at rest (SSE-S3, SSE-KMS) and in transit (HTTPS).</p>

Lesson 5:
- Title: Static Website Hosting with S3
- Video URL:
- Content (HTML/Text): <p>S3 can host a static website (HTML, CSS, JavaScript) directly from a bucket. This requires enabling static website hosting, making the relevant objects publicly readable, and specifying an index document and an error document.</p><p>It's commonly paired with Amazon CloudFront to add a CDN layer and HTTPS support in front of the bucket.</p>

### QUIZ SETTINGS
- Quiz Title: S3 Essentials Assessment
- Quiz Description: Test your understanding of the core Amazon S3 concepts covered in this course.
- Time Limit (minutes): 15
- Passing Score (%): 80

### QUIZ QUESTIONS

Question 1:
- Text: What is the basic unit of storage in S3 called?
- Option A: File
- Option B: Object
- Option C: Block
- Option D: Record
- Correct Option: B

Question 2:
- Text: Which storage class offers the lowest storage cost but the slowest retrieval time, suited for long-term archival?
- Option A: S3 Standard
- Option B: S3 Intelligent-Tiering
- Option C: S3 Glacier Deep Archive
- Option D: S3 One Zone-IA
- Correct Option: C

Question 3:
- Text: Which feature allows S3 to keep multiple copies of an object as it's overwritten or deleted, protecting against accidental loss?
- Option A: Replication
- Option B: Versioning
- Option C: Lifecycle Policy
- Option D: Cross-Region Backup
- Correct Option: B

Question 4:
- Text: Which setting helps prevent accidental public exposure of an S3 bucket?
- Option A: Bucket Policy Simulator
- Option B: Block Public Access
- Option C: IAM Access Analyzer
- Option D: VPC Endpoint
- Correct Option: B

Question 5:
- Text: What must be enabled on a bucket to serve HTML files directly as a website?
- Option A: Versioning
- Option B: Static website hosting
- Option C: Cross-Region Replication
- Option D: Transfer Acceleration
- Correct Option: B
