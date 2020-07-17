# aws-xray-dotnet-webapp

Folder `DotNET` and `DotNETCore` contains ASP.NET and ASP.NET Core applications that have been instrumented for [AWS X-Ray](https://aws.amazon.com/xray/) and are written to be deployed with Elastic Beanstalk or run locally.

Folder `DotNET-Agent` and `DotNETCore-Agent` contains ASP.NET and ASP.NET Core applications that are for [AWS X-Ray .NET Agent](https://github.com/aws/aws-xray-dotnet-agent) and are written to run locally.

## How to Run The App for X-Ray .NET SDK

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
2. Create a DynamoDB table with name `SampleProduct` in the desired region. The partion key for the table should be `Id` and of type `Number`.
3. Install AWS X-Ray daemon as a [Windows service](https://docs.aws.amazon.com/xray/latest/devguide/xray-daemon-local.html)
4. Comment DDB client creation for [.NET](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNET/src/Controllers/ProductsController.cs#L21) and [.NET Core](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNETCore/Controllers/ProductsController.cs#L27), which is used for Elasticbeanstalk and uncomment line for [.NET](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNET/src/Controllers/ProductsController.cs#L23) and [.NET Core](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNETCore/Controllers/ProductsController.cs#L29)  
5. Make sure, the region is same for DDB table on the AWS console and DDB client in the code for [.NET](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNET/src/Controllers/ProductsController.cs#L23) and [.NETCore](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNETCore/Controllers/ProductsController.cs#L29) 
6. Configure Sampling Rules in the [AWS X-Ray Console](https://docs.aws.amazon.com/xray/latest/devguide/xray-console-sampling.html).
7. The X-Ray daemon running locally should be configured in the same region as that of sampling rules through X-Ray console

### Enable SQL query (optional)

1. By default, SQL query is disabled. 
2. Create a RDS SQL Server DB instance. [Steps](http://docs.aws.amazon.com/AmazonRDS/latest/UserGuide/CHAP_GettingStarted.CreatingConnecting.SQLServer.html#CHAP_GettingStarted.Creating.SQLServer)
3. Construct the connection string for SQL Server `"Data Source=(RDS endpoint),(port number);User ID=(your user name);Password=(your password);"`
4. Fill it into web.config key "RDS_CONNECTION_STRING" for [.NET](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNET/src/Web.config#L38) and fill the string for [.NETCore](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNETCore/Controllers/ProductsController.cs#L142)
5. Uncomment call to `QuerySql()` for [.NET](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNET/src/Controllers/ProductsController.cs#L42) and [.NETCore](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNETCore/Controllers/ProductsController.cs#L64)

## How to Run The App for X-Ray .NET Agent

Sample apps for .NET Agent are identical to the ones for .NET SDK, except that the later have been instrumented with X-Ray .NET SDK, while the former are not.

You can install .NET Agent to automatically instrument .NET SDK into the sample applications by following the requirement and steps below.

### Requirement

1. AWS Credentials on the local box should have the [policy](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/README.md#policy)
2. Create a DynamoDB table with name `SampleProduct` in the desired region. The partion key for the table should be `Id` and of type `Number`.
3. Install AWS X-Ray daemon as a [Windows service](https://docs.aws.amazon.com/xray/latest/devguide/xray-daemon-local.html)
4. Make sure, the region is same for DDB table on the AWS console and DDB client in the code for [.NET](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNET-Agent/src/Controllers/ProductsController.cs#L18) and [.NETCore](https://github.com/aws-samples/aws-xray-dotnet-webapp/blob/master/DotNETCore-Agent/Controllers/ProductsController.cs#L24) 
5. Configure Sampling Rules in the [AWS X-Ray Console](https://docs.aws.amazon.com/xray/latest/devguide/xray-console-sampling.html).
6. The X-Ray daemon running locally should be configured in the same region as that of sampling rules through X-Ray console
7. If host sample application on IIS, make sure you follow the instructions [here](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-3.1) to complete the setups and configurations before installing .NET Agent.

### Installation

Below are the general steps to install .NET Agent. For more information, please take reference to [page](https://github.com/aws/aws-xray-dotnet-agent).

1. Make sure you meet the [prerequisites](https://github.com/aws/aws-xray-dotnet-agent#prerequisites) and [minimum requirements](https://github.com/aws/aws-xray-dotnet-agent#minimum-requirements) for using/building the .NET agent.
2. Follow the [steps](https://github.com/aws/aws-xray-dotnet-agent#internet-information-services-iis) if you're running on IIS or [steps](https://github.com/aws/aws-xray-dotnet-agent#others-not-iis) otherwise on how to install X-Ray .NET Agent for your apps.
3. Launch your application, open the web link in the browser, perform some operations, and you can see traces in the AWS X-Ray Console.

## URL for the Apps

Access the application : <Default_URL>/index.html.

## Policy

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
                "xray:GetSamplingStatisticSummaries",
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

## Documentation

1. Code repository for [AWS X-Ray .NET SDK](https://github.com/aws/aws-xray-sdk-dotnet) and for [AWS X-Ray .NET Agent](https://github.com/aws/aws-xray-dotnet-agent)
2. AWS Documentation for using X-Ray .NET SDK and X-Ray .NET Agent can be found [here](https://docs.aws.amazon.com/xray/latest/devguide/xray-sdk-dotnet.html)

## FAQ

1. What to do if I get an "Error: Internal Server Error"?
  * You can use AWS X-Ray to debug this. Go to AWS X-Ray console and find the failed trace, and look for Exception. Probably because you EC2 instance don't have the enough permission to access DynamoDB or RDS DB instance.

