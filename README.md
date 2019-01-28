# aws-xray-dotnet-webapp
An ASP.NET Web API application that has been instrumented for AWS X-Ray.

This application is written to be deployed with Elastic Beanstalk. It uses .ebextensions to setup AWS resources and configuration, which includes:

1. Create a DynamoDB table
2. Set an application confige DDB_TABLE_NAME with the create DynamoDB table name
3. Install AWS X-Ray daemon as a [Windows service](https://docs.aws.amazon.com/xray/latest/devguide/xray-daemon-local.html)

## How to Run The App

1. Create a RDS SQL Server DB instance. [Steps](http://docs.aws.amazon.com/AmazonRDS/latest/UserGuide/CHAP_GettingStarted.CreatingConnecting.SQLServer.html#CHAP_GettingStarted.Creating.SQLServer)

2. Construct the connection string for SQL Server and fill it into web.config key "RDS_CONNECTION_STRING".
  * "Data Source=(RDS endpoint),(port number);User ID=(your user name);Password=(your password);"


3. Deploy the application to Elastic Beanstalk. [Steps](http://docs.aws.amazon.com/elasticbeanstalk/latest/dg/create_deploy_NET.quickstart.html#aws-elastic-beanstalk-tutorial-step-2-publish-application). 
 * Make sure you attached an IAM role to your EC2 instance with the following policy
```
 {
    "Version": "2012-10-17",
    "Statement": [
        {
            "Action": [
                "sns:Publish",
                "xray:PutTelemetryRecords",
                "xray:PutTraceSegments",
                "dynamodb:PutItem",
                "dynamodb:GetItem",
                "dynamodb:DescribeTable"
            ],
            "Resource": [
                "*"
            ],
            "Effect": "Allow"
        }
    ]
}
```

4. Access the application using EB environment URL.

## FAQ

1. What to do if I get an "Error: Internal Server Error"?
  * You can use AWS X-Ray to debug this. Go to AWS X-Ray console and find the failed trace, and look for Exception. Probably because you EC2 instance don't have the enough permission to access DynamoDB or RDS DB instance.
