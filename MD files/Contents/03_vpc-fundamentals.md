### COURSE INFO
- Category: Networking
- Course Title: Amazon VPC Fundamentals
- Course Description: Amazon Virtual Private Cloud (VPC) lets you define your own logically isolated network within AWS. This course covers subnets, routing, internet access, and the two layers of network security AWS provides.
- Tags (comma-separated): AWS, VPC, Networking, Cloud Practitioner, Solutions Architect Associate

### LESSONS

Lesson 1:
- Title: What Is a VPC?
- Video URL:
- Content (HTML/Text): <p>A Virtual Private Cloud (VPC) is a logically isolated section of the AWS Cloud where you launch resources. You define your own IP address range using a CIDR block (for example <code>10.0.0.0/16</code>) and have full control over the networking environment inside it.</p>

Lesson 2:
- Title: Subnets — Public and Private
- Video URL:
- Content (HTML/Text): <p>A VPC is divided into subnets, each tied to a single Availability Zone.</p><ul><li>A <strong>public subnet</strong> has a route to an Internet Gateway, so its resources can be reached from (and reach) the internet directly.</li><li>A <strong>private subnet</strong> has no such route, and is typically used for backend resources like databases that shouldn't be directly internet-accessible.</li></ul>

Lesson 3:
- Title: Route Tables and Internet Gateways
- Video URL:
- Content (HTML/Text): <p>A <strong>route table</strong> controls where network traffic leaving a subnet is directed. An <strong>Internet Gateway (IGW)</strong> is attached at the VPC level (one per VPC) and allows communication between resources in the VPC and the internet, when a subnet's route table points traffic to it.</p>

Lesson 4:
- Title: NAT Gateways
- Video URL:
- Content (HTML/Text): <p>A <strong>NAT Gateway</strong> lives in a public subnet and allows instances in a private subnet to initiate outbound internet traffic — for example, downloading software updates — without allowing unsolicited inbound connections from the internet to reach them.</p>

Lesson 5:
- Title: Security Groups vs. Network ACLs
- Video URL:
- Content (HTML/Text): <p>Both control traffic, but at different layers:</p><ul><li><strong>Security Groups</strong> operate at the instance level, are stateful (a reply is automatically allowed), and support allow rules only.</li><li><strong>Network ACLs</strong> operate at the subnet level, are stateless (return traffic must be explicitly allowed), and support both allow and deny rules, evaluated in rule-number order.</li></ul>

### QUIZ SETTINGS
- Quiz Title: VPC Fundamentals Assessment
- Quiz Description: Test your understanding of the core Amazon VPC networking concepts covered in this course.
- Time Limit (minutes): 15
- Passing Score (%): 80

### QUIZ QUESTIONS

Question 1:
- Text: What does "VPC" stand for?
- Option A: Virtual Public Cloud
- Option B: Virtual Private Cloud
- Option C: Virtual Provisioned Cluster
- Option D: Verified Private Connection
- Correct Option: B

Question 2:
- Text: What allows instances in a public subnet to communicate directly with the internet?
- Option A: NAT Gateway
- Option B: Internet Gateway
- Option C: VPN Gateway
- Option D: Transit Gateway
- Correct Option: B

Question 3:
- Text: Which component lets a private-subnet instance reach the internet for outbound traffic without accepting inbound connections from it?
- Option A: Internet Gateway
- Option B: NAT Gateway
- Option C: Security Group
- Option D: Route 53
- Correct Option: B

Question 4:
- Text: Which statement about Network ACLs is true?
- Option A: They are stateful
- Option B: They operate at the instance level
- Option C: They can have explicit deny rules
- Option D: They cannot be modified once created
- Correct Option: C

Question 5:
- Text: What is used to divide a VPC's IP address range across Availability Zones?
- Option A: Route tables
- Option B: Subnets
- Option C: Elastic IPs
- Option D: Security groups
- Correct Option: B
