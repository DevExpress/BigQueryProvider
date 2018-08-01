BigQueryProvider
===

<a href="http://dataaccess.cloudapp.net:9999/viewType.html?buildTypeId=BigQueryProvider_DebugTest&guest=1">
<img src="http://img.shields.io/teamcity/http/dataaccess.cloudapp.net:9999/s/BigQueryProvider_DebugTest.svg?style=flat&label=DebugTest"/>
</a>
<a href="http://dataaccess.cloudapp.net:9999/viewType.html?buildTypeId=BigQueryProvider_Releas&guest=1">
<img src="http://img.shields.io/teamcity/http/dataaccess.cloudapp.net:9999/s/BigQueryProvider_Release.svg?style=flat&label=Release"/>
</a>
<a href="https://www.nuget.org/packages/DevExpress.DataAccess.BigQuery">
<img src="https://img.shields.io/nuget/v/DevExpress.DataAccess.BigQuery.svg?style=flat"/>
</a>
<a href="http://www.issuestats.com/github/DevExpress/BigQueryProvider"><img src="http://www.issuestats.com/github/DevExpress/BigQueryProvider/badge/pr" /></a>
<a href="http://www.issuestats.com/github/DevExpress/BigQueryProvider"><img src="http://www.issuestats.com/github/DevExpress/BigQueryProvider/badge/issue" /></a>

# What is BigQueryProvider?
  The BigQueryProvider is an open-source ADO.NET data provider that you can use to connect your .NET application to Google BigQuery.
  The BigQueryProvider is built upon the API provided by the **Google.Apis.Bigquery.v2** library and implements the full client-side functionality required to interact with BigQuery datasets.  It uses mechanisms common for all ADO.NET data providers and allows you to utilize the same data binding approaches that you use when connecting to data via standard ADO.NET data providers

# Requirements
  The BigQueryProvider requires the .NET Framework version 4.6.1 or higher. The **Google.Apis.Bigquery.v2** library and all its dependencies are required to build the BigQueryProvider from source and should be deployed with the final application.

# Installation
  The BigQueryProvider is available as a NuGet package. You can download it and install it into your Visual Studio project using the following steps.
  
1. Right click your project in the Visual Studio Solution Explorer and select **Manage NuGet Packages** in the context menu. 
2. In the invoked **NuGet Package Manager**, search for the **DevExpress.DataAccess.BigQuery** package. When the package is found, click **Install**. 
3. You will be prompted to accept the installation of the BigQueryProvider and all its dependencies. Click **OK** to proceed.
4. Next, the License Acceptance dialog will appear requiring you to accept the license terms of some of the BigQueryProvider dependencies. Click **I Accept** to start the package installation.

Alternatively, execute the following command in the NuGet Package Manager Console:
```
Install-Package DevExpress.DataAccess.BigQuery
```
# Build from source
  To build the BigQueryProvider library from source, open its solution in Visual Studio, set the solution configuration to **Release** and click **Build->Build Solution**. On the first build, NuGet will prompt you to download and install the library dependencies. 
  The BigQueryProvider source code comes with a set of NUnit tests, which you can run to ensure that the current version of the library operates correctly.  It is also highly recommended that you run the provided tests to verify the correctness of any changes you have applied to the library source code. 
To be able to run these tests, you first need to provide the required testing infrastructure. To satisfy this requirement, do the following:

1. Add a connection string for your BigQuery dataset to the application’s configuration file. It is recommended that you use a separate data set for testing purposes.
2. Run the ```CreateDBTables``` explicit test available in the ```TestingInfrastructureHelper``` class using a test runner of your choice.   This test will populate the dataset with tables required to correctly run other unit tests supplied with the BigQueryProvider source code.

# Connection String Parameters
  A connection string used to establish a connection to a BigQuery data source requires the following mandatory connection string parameters.
- **ProjectID** – the name of the Google Cloud Platform project hosting the required dataset.
- **DatasetID** – the name of the required dataset.
Additionally, a connection string should contain parameters specifying the required authentication setting. BigQuery supports two authentication methods: OAuth 2.0, and authentication using service account credentials. Depending on the authentication method you use, you need to specify one of the following two sets of connection string parameters.

###### OAuth:
- **OAuthClientID** – The client ID provided by the BigQuery.
- **OAuthClientSecret** – The secret character sequence generated by the BigQuery when creating a new client.
- **OAuthRefreshToken** – The token used to start a new authorization session and receive a new Access Token when an old one has expired.

###### Service Account:
- **ServiceAccountEmail** – specifies the email address identifying a Google Cloud Platform service account.
- **PrivateKeyFileName** – the path to a key file.

# Using BigQueryProvider
A code sample below demonstrates how to use the BigQueryProvider to connect your application to a BigQuery dataset.
```C#
using(var connection = new BigQueryConnection(@"ProjectID=myProject;
                                                DataSetId=myDataSet;
                                                OAuthClientId=myClientId;
                                                OAuthClientSecret=mySecret;
                                                OAuthRefreshToken=myRefreshToken")
                                                 {
    connection.Open();
    using(var command = new BigQueryCommand()) {
        command.Connection = connection;

        // Retrieve all rows
        command.CommandText = @"SELECT myTable.myField 
                                FROM myDataSet.myTable myTable";
        using(var reader = command.ExecuteReader()) {
            while(reader.Read()) {
                Console.WriteLine(reader.GetString(0));
            }
        }
    }
}
```
In this example, a new BigQueryConnection is created with the specified connection string and is used to execute a SELECT query against a BigQuery dataset. Afterwards, a BigQueryDataReader is used to iterate through the rows of the result set.

