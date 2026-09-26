### COURSE INFO
- Category: Security & IAM
- Course Title: AWS IAM Fundamentals
- Course Description: AWS Identity and Access Management (IAM) is how you securely control who can do what across your AWS account. This course covers users, groups, roles, policies, and the core security practices every AWS account should follow.
- Tags (comma-separated): AWS, IAM, Security, Cloud Practitioner

### LESSONS

Lesson 1:
- Title: What Is IAM?
- Video URL:
- Content (HTML/Text): <p>AWS Identity and Access Management (IAM) is a global service (not tied to a specific region) that lets you manage <strong>authentication</strong> (who you are) and <strong>authorization</strong> (what you're allowed to do) across your AWS account.</p>

Lesson 2:
- Title: Users, Groups, and Roles
- Video URL:
- Content (HTML/Text): <ul><li>An <strong>IAM User</strong> represents a person or application with long-term credentials.</li><li>An <strong>IAM Group</strong> is a collection of users that share the same set of permissions.</li><li>An <strong>IAM Role</strong> provides temporary credentials that can be assumed by a user, an AWS service, or an external application — safer than long-term keys, and the standard way to grant an EC2 instance or another AWS service access to resources.</li></ul>

Lesson 3:
- Title: IAM Policies
- Video URL:
- Content (HTML/Text): <p>An IAM policy is a JSON document that defines permissions, using an <code>Effect</code> (Allow or Deny), an <code>Action</code>, and a <code>Resource</code>. Policies can be attached to users, groups, or roles, and come in three flavors: AWS-managed (maintained by AWS), customer-managed (created and maintained by you, reusable), and inline (embedded directly on a single user/group/role).</p>

Lesson 4:
- Title: The Principle of Least Privilege
- Video URL:
- Content (HTML/Text): <p>Least privilege means granting only the permissions required to perform a specific task, and nothing more. This limits the damage if a set of credentials is ever compromised. Tools like IAM Access Analyzer can help review and tighten permissions that are broader than necessary.</p>

Lesson 5:
- Title: Multi-Factor Authentication and Root Account Security
- Video URL:
- Content (HTML/Text): <p>The AWS <strong>root account</strong> has unrestricted access to everything in the account and should not be used for everyday tasks — create individual IAM users (or roles) instead. Enabling <strong>Multi-Factor Authentication (MFA)</strong> on the root account and on privileged users adds a critical second layer of protection beyond just a password.</p>

### QUIZ SETTINGS
- Quiz Title: IAM Fundamentals Assessment
- Quiz Description: Test your understanding of the core IAM concepts covered in this course.
- Time Limit (minutes): 15
- Passing Score (%): 80

### QUIZ QUESTIONS

Question 1:
- Text: What does IAM primarily manage?
- Option A: Network traffic
- Option B: Billing alerts
- Option C: Access to AWS resources
- Option D: Server hardware
- Correct Option: C

Question 2:
- Text: Which IAM entity provides temporary credentials rather than long-term access keys?
- Option A: User
- Option B: Group
- Option C: Role
- Option D: Policy
- Correct Option: C

Question 3:
- Text: In what format are IAM policies written?
- Option A: XML
- Option B: YAML
- Option C: JSON
- Option D: CSV
- Correct Option: C

Question 4:
- Text: What security principle means granting only the permissions necessary to perform a task?
- Option A: Least Privilege
- Option B: Defense in Depth
- Option C: Shared Responsibility
- Option D: Zero Trust
- Correct Option: A

Question 5:
- Text: What should you do with the AWS root account?
- Option A: Use it for daily administrative tasks
- Option B: Share its credentials with the whole team
- Option C: Enable MFA on it and avoid routine use
- Option D: Delete it after account creation
- Correct Option: C
