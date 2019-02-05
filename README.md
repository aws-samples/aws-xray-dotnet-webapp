# aws-xray-dotnet-webapp
An ASP.NET and ASP.NET Core application that has been instrumented for [AWS X-Ray](https://aws.amazon.com/xray/).

These applications are written to be deployed with Elastic Beanstalk or run locally.

## How to Run The App

### Elastic beanstalk

##### *Deploy*

1. Attach an IAM role to your EC2 instance with the [policy](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/README.md#policy)
2. Deploy the application to Elastic Beanstalk. [Steps](http://docs.aws.amazon.com/elasticbeanstalk/latest/dg/create_deploy_NET.quickstart.html#aws-elastic-beanstalk-tutorial-step-2-publish-application)
3. Configure Sampling Rules in the [AWS X-Ray Console](https://docs.aws.amazon.com/xray/latest/devguide/xray-console-sampling.html) 

##### *EbExtensions*

The App uses .ebextensions to setup AWS resources and configuration, which includes:

1. Create a DynamoDB table with name `SampleProduct`
2. Set an application config DDB_TABLE_NAME with the create DynamoDB table name
3. Install AWS X-Ray daemon as a [Windows service](https://docs.aws.amazon.com/xray/latest/devguide/xray-daemon-local.html)

### Locally

1. AWS Credentials on the local box should have the [policy](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/README.md#policy)
2. Create a DynamoDB table with name `SampleProduct` in the desired region
3. Install AWS X-Ray daemon as a [Windows service](https://docs.aws.amazon.com/xray/latest/devguide/xray-daemon-local.html)
4. Comment DDB client creation for [.NET](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNET/src/Controllers/ProductsController.cs#L21) and [.NET Core](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNETCore/Controllers/ProductsController.cs#L27), which is used for Elasticbeanstalk and uncomment line for [.NET](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNET/src/Controllers/ProductsController.cs#L23) and [.NET Core](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNETCore/Controllers/ProductsController.cs#L29)  
5. Make sure, the region is same for DDB table on the AWS console and DDB client in the code for [.NET](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNET/src/Controllers/ProductsController.cs#L23) and [.NETCore](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNETCore/Controllers/ProductsController.cs#L29) 
6. Configure Sampling Rules in the [AWS X-Ray Console](https://docs.aws.amazon.com/xray/latest/devguide/xray-console-sampling.html).
7. The X-Ray daemon running locally should be configured in the same region as that of sampling rules through X-Ray console

### URL for the App

Access the application : <Default_URL>/index.html.

### Policy

```json
 {
    "Version": "2012-10-17",
    "Statement": [
        {
            "Action": [
                "sns:Publish",
                "xray:PutTraceSegments",
                "xray:PutTelemetryRecords",
                "xray:GetSamplingRules",
                "xray:GetSamplingTargets",
                "xray:GetSamplingStatisticSummaries"
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

### Enable SQL query (optional)

1. By default, SQL query is disabled. 
2. Create a RDS SQL Server DB instance. [Steps](http://docs.aws.amazon.com/AmazonRDS/latest/UserGuide/CHAP_GettingStarted.CreatingConnecting.SQLServer.html#CHAP_GettingStarted.Creating.SQLServer)
3. Construct the connection string for SQL Server `"Data Source=(RDS endpoint),(port number);User ID=(your user name);Password=(your password);"`
4. Fill it into web.config key "RDS_CONNECTION_STRING" for [.NET](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNET/src/Web.config#L38) and fill the string for [.NETCore](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNETCore/Controllers/ProductsController.cs#L142)
5. Uncomment call to `QuerySql()` for [.NET](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNET/src/Controllers/ProductsController.cs#L42) and [.NETCore](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNETCore/Controllers/ProductsController.cs#L64)


## Documentation

1. Code repository for AWS X-Ray SDK for [.NET/Core](https://github.com/aws/aws-xray-sdk-dotnet)
2. AWS Documentation for using X-Ray SDK for [.NET](https://docs.aws.amazon.com/xray/latest/devguide/xray-sdk-dotnet.html)

## FAQ

1. What to do if I get an "Error: Internal Server Error"?
  * You can use AWS X-Ray to debug this. Go to AWS X-Ray console and find the failed trace, and look for Exception. Probably because you EC2 instance don't have the enough permission to access DynamoDB or RDS DB instance.
