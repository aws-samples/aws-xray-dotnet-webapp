# aws-xray-dotnet-webapp
An ASP.NET Web API application that has been instrumented for AWS X-Ray.

This application is written to be deployed with Elastic Beanstalk. It uses .ebextensions to setup AWS resources and configuration, which includes:
1. Create a DynamoDB table
2. Set an application confige DDB_TABLE_NAME with the create DynamoDB table name
3. Generate DefaultSamplingRules.json and AWSRequestInfo.json to C:\configs
4. Install AWS X-Ray daemon as a Windows service

##How to Run The App
1. Create a RDS SQL Server DB instance
http://docs.aws.amazon.com/AmazonRDS/latest/UserGuide/CHAP_GettingStarted.CreatingConnecting.SQLServer.html#CHAP_GettingStarted.Creating.SQLServer

2. Construct the connection string to the SQL Server and file into web.config "RDS_CONNECTION_STRING".

3. Deploy the application to Elastic Beanstalk.
http://docs.aws.amazon.com/elasticbeanstalk/latest/dg/create_deploy_NET.quickstart.html#aws-elastic-beanstalk-tutorial-step-2-publish-application

4. Access the application using EB environment URL.

##FAQ
1. What to do if I get an "Error: Internal Server Error"?
You can use AWS X-Ray to debug this. Go to AWS X-Ray console and find the failed traces, and look for Exception. Probably because you RDS DB instance security group do not have an inbound rule for your EC2 instance. 
